using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour {

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
        if (Server.instance.active)
            Spawn(default(listing));
        else
            Server.instance.onStartServer += Spawn;



    }

    void Spawn(listing l)
    {
        current = Server.instance.AddEntity(Server.instance.GetEmptyID(), monster.name) as Entity;
        current.transform.parent = transform;
        current.transform.position = transform.position;

        current.level = RollLevel();
    }

    public int RollLevel()
    {
        int totalWeight = spawnChances.Sum(entry => entry.weight); //get all weights
        int roll = UnityEngine.Random.Range(1, totalWeight);
        if (spawnChances.Length > 0)
            return spawnChances.OrderBy((entry) => entry.weight).FirstOrDefault(entry => entry.weight > roll).level; //order from greatest chance to smallest, then return level corresponding to first element that was greater than roll, indicating we rolled that
        else
            return current.level;
    }










    public bool snapToValid = false;

    void OnDrawGizmos()
    {
        if (!snapToValid) return;
        snapToValid = false;

    #if UNITY_EDITOR
            SnapToValid();
    #endif

    }

    Vector3 calcTreesOffset = new Vector3(0, 3f, 0);
    public virtual void SnapToValid()
    {
        NavMeshHit hit;
        for (int i = 10; i > 0; i--)
        {
            if (NavMesh.SamplePosition(transform.position, out hit, 100f, NavMesh.AllAreas))
            {
                if (!Physics.SphereCast(new Ray(hit.position + calcTreesOffset, Vector3.down), 1.5f, 1.5f, WorldFunctions.worldMask))
                {
                    transform.position = hit.position;
                    break;
                }
            }
        }
    }

}
