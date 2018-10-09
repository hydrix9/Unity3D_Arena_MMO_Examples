using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceAroundScreen : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        StartPingPong();

        pongSpeed = startPongSpeed; //reset
    }



    /// <summary>
    /// start moving player around screen
    /// </summary>
    public void StartPingPong()
    {
        if (isPong)
            return;
        isPong = true;
        StartCoroutine(Pong());
    }

    bool isPong;
    public Vector3 pongPos;

    public float startPongSpeed = 0.1f;

    public float pongSpeed = 0.1f;
    public float minDistance = 0.1f;

    public float minRange = .25f;
    public float maxRange = .75f;
    IEnumerator Pong()
    {

        pongPos = Camera.main.ViewportToWorldPoint(new Vector3(        //init
        Random.Range(minRange, maxRange), Random.Range(minRange, maxRange), 0f)
        );
        pongPos.z = 0;
        while (true)
        {
            transform.position = Vector2.MoveTowards(transform.position, pongPos, pongSpeed);
            if (Vector3.Distance(transform.position, pongPos) < minDistance)
            {
                pongPos = Camera.main.ViewportToWorldPoint(new Vector3( //roll new pong target
                    Random.Range(minRange, maxRange), Random.Range(minRange, maxRange), 0f)
                    );
                pongPos.z = 0;
            }

            yield return new WaitForEndOfFrame();
        }
    }

}
