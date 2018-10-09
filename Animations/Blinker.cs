using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Blinker : MonoBehaviour {

    public float offTime = 1f;
    public float onTime = 2f;

    public bool randomizeTimes;

    public GameObject blunk;

	// Use this for initialization
	void Start () {
        if (blunk == null)
            blunk = gameObject;

        if (randomizeTimes)
        {
            offTime = Random.Range(0, offTime);
            onTime = Random.Range(0, onTime);
        }

        onTimer = onTime;
        offTimer = offTime;
	}

    float offTimer;
    float onTimer;

	// Update is called once per frame
	void Update () {
        if (blunk.activeSelf)
        {
            onTimer -= Time.deltaTime;
            if(onTimer <= 0)
            {
                onTimer = onTime;
                blunk.SetActive(false);
            }
        } else
        {
            offTimer -= Time.deltaTime;
            if (offTimer <= 0)
            {
                offTimer = offTime;
                blunk.SetActive(true);
            }
        }

	}

}
