using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotationAt : MonoBehaviour {

    public bool x;
    public bool y;
    public bool z;

    public Vector3 rotation;


    private void Start()
    {

    }



    // Update is called once per frame
    void LateUpdate () {

        transform.rotation = Quaternion.Euler(
            x ? rotation.x : transform.rotation.eulerAngles.x,
            y ? rotation.y : transform.rotation.eulerAngles.y,
            z ? rotation.z : transform.rotation.eulerAngles.z
            );
	}
}
