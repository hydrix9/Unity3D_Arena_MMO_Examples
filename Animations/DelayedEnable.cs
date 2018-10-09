using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedEnable : MonoBehaviour {

    public float waitTime = 1f;
    public GameObject obj;

	// Use this for initialization
	void Start () {
        StartCoroutine(delay());
	}
	
    IEnumerator delay()
    {
        yield return new WaitForSeconds(waitTime);
        obj.SetActive(true);
    }

}
