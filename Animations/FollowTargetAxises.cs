using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetAxises : MonoBehaviour {


    public Transform target;
    public Vector3 offset = new Vector3(0f, 7.5f, 0f);

    Vector3 temp;
    public bool x;
    public bool y;
    public bool z;

    public float speed = 1000;

    public bool fudge;
    public float minFudge =  0.85f;
    public float maxFudge = 1.15f;

    private void LateUpdate()
    {
        temp = transform.position - offset;

        if (x)
            temp.x = target.position.x;
        if (y)
            temp.y = target.position.y;
        if (z)
            temp.z = target.position.z;

        if(fudge)
            transform.position = Vector3.MoveTowards(transform.position, temp + offset, speed * Time.deltaTime * Random.Range(minFudge, maxFudge));
        else
            transform.position = Vector3.MoveTowards(transform.position, temp + offset, speed * Time.deltaTime);
    }
}
