using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateXY : MonoBehaviour {

    public bool useX = true;
    public bool useY = true;

    public bool bothMode = false;

    public float maxX;
    public float maxY;


    public float moveSpeed = 1;

    public float moveBackSpeed = 1;

    Quaternion normalRotation;

	// Use this for initialization
	void Awake () {
        normalRotation = transform.rotation;	
	}

    Vector3 temp;

    public float minFudge = 0.8f;
    public float maxFudge = 1.2f;

    public bool moveBack = false;



    // Update is called once per frame
    void Update () {
        

        if (PlayerMovement.main.x != 0 || PlayerMovement.main.y != 0)
        {
            temp = transform.rotation.eulerAngles;
            if (useX)
            {
                if (PlayerMovement.main.x < 0)
                {
                    temp.y = Mathf.MoveTowards(temp.y, normalRotation.eulerAngles.y - maxX, moveSpeed * Time.deltaTime * Random.Range(minFudge, maxFudge));
                }
                else
                if (PlayerMovement.main.x > 0)
                {
                    temp.y = Mathf.MoveTowards(temp.y, normalRotation.eulerAngles.y + maxX, moveSpeed * Time.deltaTime * Random.Range(minFudge, maxFudge));
                }
            }

            if (useY)
            {

                if (PlayerMovement.main.y < 0)
                {
                    temp.z = Mathf.MoveTowards(temp.z, normalRotation.eulerAngles.z + maxY, moveSpeed * Time.deltaTime * Random.Range(minFudge, maxFudge));
                }
                else
                if (PlayerMovement.main.y > 0)
                {
                    temp.z = Mathf.MoveTowards(temp.z, normalRotation.eulerAngles.z - maxY, moveSpeed * Time.deltaTime * Random.Range(minFudge, maxFudge));
                }

            }
            if (bothMode)
            {
                if (PlayerMovement.main.x < 0 || PlayerMovement.main.y > 0)
                {
                    temp.z = Mathf.MoveTowards(temp.z, normalRotation.eulerAngles.z - maxY, moveSpeed * Time.deltaTime * Random.Range(minFudge, maxFudge));
                }
                else
                if (PlayerMovement.main.x > 0 || PlayerMovement.main.y < 0)
                {
                    temp.z = Mathf.MoveTowards(temp.z, normalRotation.eulerAngles.z + maxY, moveSpeed * Time.deltaTime * Random.Range(minFudge, maxFudge));
                }
            }


            transform.rotation = Quaternion.Euler(temp);
        } else
        {
            if(moveBack)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, normalRotation, moveBackSpeed * Time.deltaTime);
        }
    }
}
