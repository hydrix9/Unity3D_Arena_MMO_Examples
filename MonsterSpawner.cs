using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour {

    //struct that allows us to serialize a weighted spawn chance in the editor corresponding to the levels specified
    [Serializable]
    public struct SpawnChance {
        public int level;
        public int weight;

        public SpawnChance(int level, int weight)
        {
            this.level = level;
            this.weight = weight;
        }
    }

    public Entity current;
    public GameObject monster;

    [SerializeField]
    public SpawnChance[] spawnChances = new SpawnChance[] { new SpawnChance(1, 85), new SpawnChance(2, 10), new SpawnChance(3, 5) };

	// Use this for initialization
	void Start () {

        //spawn our monster instantly or add to delegate
        if (Server.instance.active)
            Spawn(default(listing)); //onStartServer passes a delegate with the listing from MMClient as a parameter
        else
            Server.instance.onStartServer += Spawn;



    }


    void Spawn(listing l)
    {
        current = Server.instance.AddEntity(Server.instance.GetEmptyID(), monster.name) as Entity; //spawn monster from name on Server

        //set pos and rotation of spawned monster
        current.transform.parent = transform;
        current.transform.position = transform.position;

        current.level = RollLevel(); //figure out level
    }


    /// <summary>
    /// rolls weights that result in possible level. EX: weights 10, 20, 10 would result in 25% level 1, 50% level 2, 25% level 3
    /// </summary>
    public int RollLevel()
    {
        int totalWeight = spawnChances.Sum(entry => entry.weight); //get all weights
        int roll = UnityEngine.Random.Range(1, totalWeight); //roll between 1 and total
        if (spawnChances.Length > 0)
            return spawnChances.OrderBy((entry) => entry.weight).FirstOrDefault(entry => entry.weight > roll).level; //order from greatest chance to smallest, then return level corresponding to first element that was greater than roll, indicating we rolled that
        else
            return current.level;
    }










    public bool snapToValid = false;

    /// <summary>
    /// in the editor GUI this is called constantly. This function calls function SnapToValid when the bool snapToValid is pressed
    /// </summary>
    void OnDrawGizmos()
    {
        if (!snapToValid) return;
        snapToValid = false;

    #if UNITY_EDITOR
            SnapToValid();
    #endif

    }


    Vector3 calcTreesOffset = new Vector3(0, 3f, 0); // how far away to stay to check for trees to prevent spawning in trees

    /// <summary>
    /// snap to a valid position on the current navmesh. Called from a button in the GUI. Also called from SpawnGenerator to snap to a valid position when it generates a spawn
    /// </summary>
    public virtual void SnapToValid()
    {
        NavMeshHit hit;
        for (int i = 10; i > 0; i--)
        { //try 10 times to find valid position with no trees

            if (NavMesh.SamplePosition(transform.position, out hit, 100f, NavMesh.AllAreas))
            {
                if (!Physics.SphereCast(new Ray(hit.position + calcTreesOffset, Vector3.down), 1.5f, 1.5f, WorldFunctions.worldMask))
                { //spherecast from a position above the grid on the worldmask to check for trees
                    transform.position = hit.position;
                    break;
                }
            }
        }
    }

}
