using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
public class SeekUntilHit : MonoBehaviour {
    const float destroyDelay = 0.25f;

    public spellHit spellHit;
    public float speed = 1f;
    bool triggeredHit;
	// Update is called once per frame
	void Update () {
        if (spellHit.target != null)
            transform.position = Vector3.MoveTowards(transform.position, spellHit.target.transform.position, speed);
        else
            GameObject.Destroy(gameObject);
	}





    private void OnTriggerEnter(Collider other)
    {
        if (triggeredHit == true)
            return; //avoid double triggering

        if (other.GetComponent<Entity>() != null && other.GetComponent<Entity>() == spellHit.target)
        {
            triggeredHit = true;
            spellHit.spell.OnAnimationDone(spellHit); //actually do what was assigned

            Ability.DestroyDelayed(gameObject, destroyDelay);
        }
    }

    public void Set(spellHit _spellHit, float _speed)
    {
        spellHit = _spellHit; speed = _speed;
    }


}
