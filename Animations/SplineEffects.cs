using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SplineEffects : MonoBehaviour {

    [Serializable]
    public class SplineEvent : UnityEvent { }


    //these are set to functions below in the inspector for ease
    [SerializeField]
    public SplineEvent onOne = new SplineEvent();
    [SerializeField]
    public SplineEvent onZero = new SplineEvent();
    [SerializeField]
    public SplineEvent onMid = new SplineEvent();


    public SplineWalker walker;

    RotateController rotateController;

    // Use this for initialization
    void Start () {
        if (walker == null)
            walker = GetComponent<SplineWalker>();

        rotateController = GetComponent<RotateController>();

        //init delegates
        if (onOne != null)
            walker.onOne += onOne.Invoke;
        if (onZero != null)
            walker.onZero += onZero.Invoke;
        if (onMid != null)
            walker.onMid += onMid.Invoke;



    }


    public float gibSpring1Percent = 0.5f;
    
    //uses rotate controller and gives it spring power
    public void GibSpring1()
    {
        Debug.Log("gib");
        rotateController.springUsed = Mathf.Max(rotateController.springUsed, rotateController.springMax * gibSpring1Percent); //give spring percent but not if already higher
    }


} //end class
