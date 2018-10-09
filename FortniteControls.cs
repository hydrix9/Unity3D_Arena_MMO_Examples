using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortniteControls : MonoBehaviour {

    public OnlineGUI onlineGUI;
    public Player player { get { return onlineGUI.player; } }
    public List<KeyBind> keybinds { get { return onlineGUI.keybinds; } }

    [Serializable]
    public class HotBarItem
    {
        public RectTransform obj;
        public Action onSelect;
        public Action onDeselect; //could do some super over the top animations with this
        public Action action;
        public GameObject object1; //blueprint
        public GameObject object2; //actual
    }
    public List<HotBarItem> hotBar1 = new List<HotBarItem>();
    public HotBarItem currentHotBar1Item;



    // Use this for initialization
    void Awake()
    {
        onlineGUI.onRemovePlayer += RemovePlayer;
        onlineGUI.onSetPlayer += SetPlayer;
        onlineGUI.onValidLeftClick += OnValidLeftClick;
        onlineGUI.onValidRightClick += OnValidRightClick;

    }


    // Use this for initialization
    void Start () {


        //build options. Just set keybinds to trigger onSelect, which we assign below. This will select the item to build, then we just need to listen for either a left click or a cancel.
        keybinds.Add(new KeyBind(0, "f1", null, () =>
        {
            hotBar1[0].onSelect();
        }));
        keybinds.Add(new KeyBind(0, "f2", null, () =>
        {
            hotBar1[1].onSelect();
        }));
        keybinds.Add(new KeyBind(0, "f3", null, () =>
        {
            hotBar1[2].onSelect();
        }));
        keybinds.Add(new KeyBind(0, "f4", null, () =>
        {
            hotBar1[3].onSelect();
        }));




        //initialize hotbar GUI stuff
        for (int i = 0; i < hotBar1.Count; i++)
        {
            int x = i;
            //set up onselect, actual assignment of currentHotBar1Item, and ondeselect using a persistant local scope variable because delegates
            hotBar1[i].onSelect = () => {
                if (currentHotBar1Item != null)
                    HotBarGUIDeselectEffect1(currentHotBar1Item);
                currentHotBar1Item = hotBar1[x];
                HotBarGUISelectEffect1(currentHotBar1Item);
                ShowBlueprint();
            };
            //try to find keybind, then show in GUI
            for (int y = 0; y < keybinds.Count; y++)
            {
                if (keybinds[y].action == hotBar1[i].action && keybinds[y].castButton == null) //if the same action, this is the keybind, show it in GUI && castButton isn't set (it set itself up) should probably revamp this loop later....
                    hotBar1[i].obj.GetChild(0).GetChild(0).GetChild(2).GetComponent<Text>().text = keybinds[y].key; //assign text
            }


        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentBlueprint != null) //if have something selected, build
                HideBlueprint();
        }

        //if have hotbar item selected, but not clicking, show blueprint
        if (currentBlueprint != null)
        {
            ShowBlueprint();
        }
        else
        {
            HideBlueprint();
        }
    }



    public void OnValidLeftClick()
    {

            //if we have a build item selected, try build. Otherwise, try target
            if (currentBlueprint != null)
            {
                TryUseBuild();
            }
    }
    public void OnValidRightClick()
    {

        if (currentBlueprint != null) //if have something selected, build
            HideBlueprint();
        
    }



    //set the local player and delegates and refernces
    public void SetPlayer(Player player)
    {
        buildPosOffset = player.playerMovement.transform.GetChild(0).GetChild(1); //find the object on playerMovement which will dictate slight build pos offset

    }
    //remove the local player and delegates and refernces
    public void RemovePlayer(Player player)
    {

    }



    #region fortniteBuilding
    public void TryUseBuild()
    {
        if (currentBlueprint == null) //if nothing to do, return
            return;

        GameObject.Instantiate(currentHotBar1Item.object2, currentBlueprint.transform.position, currentBlueprint.transform.rotation, null); //create the SyncObject in the position of the blueprint
    }

    public GameObject currentBlueprint; //blueprint as showing in game world, or null
    public Transform buildPosOffset; //small GameObject used to automatically offset where to start building
    public Vector3 buildPosTemp;
    public float gridAlignment = 5;
    public Quaternion wallRotation = Quaternion.Euler(0, 90, 0);
    public Quaternion rampRotation1 = Quaternion.Euler(45, 0, 0);
    public Quaternion rampRotation2 = Quaternion.Euler(45, 90, 0);
    public Quaternion rampRotation3 = Quaternion.Euler(45, 180, 0);
    public Quaternion rampRotation4 = Quaternion.Euler(45, 270, 0);

    float dotTemp;
    public void ShowBlueprint()
    {

        if (currentBlueprint != null && currentHotBar1Item.object1 != null && currentBlueprint.name != currentHotBar1Item.object1.name)
            HideBlueprint(); //delete if exists


        currentBlueprint = GameObject.Instantiate(currentHotBar1Item.object1);

        buildPosTemp.x = Mathf.RoundToInt(buildPosOffset.position.x / gridAlignment) * gridAlignment;
        buildPosTemp.y = Mathf.RoundToInt(buildPosOffset.position.y / gridAlignment) * gridAlignment + gridAlignment / 2;
        buildPosTemp.z = Mathf.RoundToInt(buildPosOffset.position.z / gridAlignment) * gridAlignment;

        //switch based on name of object going to build. Decide where to place blueprint
        switch (currentHotBar1Item.object2.name)
        {
            case "Wall":

                //find way we're facing and apply direction & offset
                dotTemp = Vector3.Dot(Vector3.forward, buildPosOffset.forward);
                if (-0.5f < dotTemp && dotTemp < 0.5f)
                {
                    currentBlueprint.transform.rotation = wallRotation;
                    dotTemp = Vector3.Dot(Vector3.right, buildPosOffset.forward); //have to calculate another dot to figure out our direction, because "left/right" both show 0. With this way we can calculate more accurate offset below

                    if (dotTemp < 0) //if negative, fix "too far" placement, else, fix "too far" other way
                        buildPosTemp.x += gridAlignment / 2; //make it on the edge, not the center
                    else
                        buildPosTemp.x -= gridAlignment / 2; //make it on the edge, not the center

                    // Debug.Log("rot 1");
                    // Debug.Log(dotTemp);
                }
                else
                {
                    currentBlueprint.transform.rotation = Quaternion.identity; //zero
                    if (dotTemp < 0) //if negative, fix "too far" placement, else, fix "too far" other way
                        buildPosTemp.z += gridAlignment / 2; //make it on the edge, not the center
                    else
                        buildPosTemp.z -= gridAlignment / 2; //make it on the edge, not the center

                    // Debug.Log("rot 2");
                    // Debug.Log(dotTemp);
                }
                break;

            case "Floor":
                buildPosTemp.y = Mathf.RoundToInt(buildPosOffset.position.y / gridAlignment) * gridAlignment;
                break;
            case "Ramp":


                //find way we're facing and apply direction & offset
                dotTemp = Vector3.Dot(Vector3.forward, buildPosOffset.forward);
                if (-0.5f < dotTemp && dotTemp < 0.5f)
                {
                    dotTemp = Vector3.Dot(Vector3.right, buildPosOffset.forward); //have to calculate another dot to figure out our direction, because "left/right" both show 0. With this way we can calculate more accurate offset below

                    if (dotTemp < 0)
                        currentBlueprint.transform.rotation = rampRotation1;
                    else
                        currentBlueprint.transform.rotation = rampRotation3;
                }
                else
                {
                    if (dotTemp < 0)
                        currentBlueprint.transform.rotation = rampRotation2;
                    else
                        currentBlueprint.transform.rotation = rampRotation4;
                }


                break;
        }

        currentBlueprint.transform.position = buildPosTemp;

    }

    public void HideBlueprint()
    {
        if (currentBlueprint == null) //if nothing to do
            return;

        GameObject.Destroy(currentBlueprint);
        currentBlueprint = null;
    }
    #endregion



}
