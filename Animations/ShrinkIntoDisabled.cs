using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkIntoDisabled : MonoBehaviour {

    public float speed = 0.1f;


    public float delay = 0f;

    float delayTimer;

    private void Awake()
    {
        delayTimer = delay; //init
    }

    // Update is called once per frame
    void Update () {
        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            return; //keep waiting until timer done
        }

        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, speed); //move towards Vector3.zero to shrink local scale

        if (transform.localScale == Vector3.zero)
        {
            //disable object, reset, and disable this script to prevent re-triggering
            gameObject.SetActive(false); //disable gameObject if zero
            delayTimer = delay;
            this.enabled = false;
        }
	}



}
