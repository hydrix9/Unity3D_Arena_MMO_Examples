using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerAction : MonoBehaviour {

    [SerializeField]
    public UnityEvent<Collider> onTriggerEnter;


    private void OnTriggerEnter(Collider other)
    {
        if(onTriggerEnter != null)
            onTriggerEnter.Invoke(other);
    }
}
