using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// creates a random monster spawner within a collider. MonsterSpawner then snaps to grid
/// </summary>
public class SpawnGenerator : MonoBehaviour {

    Collider c;
    public int numSpawns = 5;

	// Use this for initialization
	void Start () {
        c = GetComponent<Collider>();
        c.enabled = (false);
	}


    public GameObject[] monsterSpawners;

    public bool trySpawn;

    void OnDrawGizmos()
    {
        if (!trySpawn) return;
        trySpawn = false;

#if UNITY_EDITOR
        TrySpawn();
#endif

    }

    public static Vector3 oob = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue); //always out of bounds vector3

    public virtual void TrySpawn()
    {
        //try use collider on object to spawn random positions
        if (c == null)
            c = GetComponent<Collider>();
        if (c == null)
            return;

        Vector3 newPos = oob;
        GameObject newSpawn;

        for (int i = 0; i < numSpawns; i++) {
            newPos = oob; //reset

            while(!c.bounds.Contains(newPos))
            { //roll a random pos within collider bounds
                newPos = new Vector3(Random.Range(c.bounds.min.x, c.bounds.max.x), Random.Range(c.bounds.min.y, c.bounds.max.y), Random.Range(c.bounds.min.z, c.bounds.max.z));
            }

            newSpawn = GameObject.Instantiate<GameObject>(monsterSpawners.Random(), newPos, Quaternion.identity, transform);
            newSpawn.GetComponent<MonsterSpawner>().SnapToValid(); //snap to valid position on grid

        }


    }
}
