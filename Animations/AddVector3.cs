using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddVector3 : MonoBehaviour {

    public Vector3 added;

    public float moveSpeed = 1;

	// Use this for initialization
	void Start () {
		
	}

    Vector3 temp;
	// Update is called once per frame
	void Update () {
        temp = transform.position;

        temp += transform.right.normalized * moveSpeed * added.x;
        temp += transform.up.normalized * moveSpeed * added.y;
        temp += transform.forward.normalized * moveSpeed * added.z;


        transform.position = temp;
	}
}
