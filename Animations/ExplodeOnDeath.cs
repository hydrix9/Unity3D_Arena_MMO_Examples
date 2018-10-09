using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnDeathUpdate : MonoBehaviour
{
    #region Singleton
    //Singleton code
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static ExplodeOnDeathUpdate s_Instance = null;
    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static ExplodeOnDeathUpdate instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(ExplodeOnDeathUpdate)) as ExplodeOnDeathUpdate;
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("ExplodeOnDeathUpdate");
                s_Instance = obj.AddComponent(typeof(ExplodeOnDeathUpdate)) as ExplodeOnDeathUpdate;
                Debug.Log("Could not locate an ExplodeOnDeathUpdate object. ExplodeOnDeathUpdate was Generated Automaticly.");
            }
            return s_Instance;
        }
    }
    #endregion

    public List<ExplodeInstance> queue;

    private void Awake()
    {
        queue = new List<ExplodeInstance>(); //init
    }

    //remove queue
    private void Update()
    {
        while(queue.Count > 0)
        {
            if(queue[0].parent == null)
                GameObject.Instantiate<GameObject>(queue[0].orig, queue[0].location, queue[0].rotation);
            else
                GameObject.Instantiate<GameObject>(queue[0].orig, queue[0].location, queue[0].rotation, queue[0].parent);
            queue.RemoveAt(0);
        }
    }

} //end class


//struct to hold queue items
public struct ExplodeInstance
{
    public GameObject orig;
    public Vector3 location;
    public Quaternion rotation;
    public Transform parent;


    public ExplodeInstance(GameObject _orig, Vector3 _location, Quaternion _rotation, Transform _parent)
    {
        orig = _orig;
        location = _location;
        rotation = _rotation;
        parent = _parent;
    }

} //end struct



//this queues an explosion on the singleton above to allow instant death, explosion occurs on next frame
public class ExplodeOnDeath : MonoBehaviour {

    public GameObject explosionPrefab;

    //called on object destroy
    private void OnDestroy()
    {
        if (explosionPrefab != null)
        {
            ExplodeOnDeathUpdate.instance.queue.Add(new ExplodeInstance(explosionPrefab, transform.position, transform.rotation, null));
        }
    }

} //end class
