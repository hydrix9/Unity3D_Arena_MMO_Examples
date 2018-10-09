using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Rotate : MonoBehaviour {

    public Vector3 rotation;
    public Transform obj;

    private void Start()
    {
        if (obj == null)
            obj = transform;
    }

    // Update is called once per frame
    void Update () {
        obj.Rotate(rotation);
	}


}
