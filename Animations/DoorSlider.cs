using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlider : MonoBehaviour {

    public Transform openPos;
    public Transform closePos;

    public float moveSpeed = 0.1f;

    //-1 for close, 1 for open, 0 for none
    int movingDir;

    public void Open()
    {
        if(movingDir != 1) //prevent dupe
            StartCoroutine(IOpen());
    }

    public void Close()
    {
        if(movingDir != -1) //prevent dupe
            StartCoroutine(IClose());
    }


    IEnumerator IOpen()
    {
        movingDir = 1;
        while(movingDir == 1 && transform.position != openPos.position) //allow for opening and closing fast with the movingDir
        {
            transform.position = Vector3.MoveTowards(transform.position, openPos.position, moveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        movingDir = 0;
    }

    IEnumerator IClose()
    {
        movingDir = -1;
        while (movingDir == -1 && transform.position != closePos.position) //allow for opening and closing fast with the movingDir
        {
            transform.position = Vector3.MoveTowards(transform.position, closePos.position, moveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        movingDir = 0;
    }

}
