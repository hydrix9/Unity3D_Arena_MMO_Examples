using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct TimedEvent
{
    public float time;
    public UnityEvent action;
    public bool fired;
    public TimedEvent(float _time, UnityEvent _action)
    {
        time = _time;
        action = _action;
        fired = false;
    }

    public void Fire()
    {
        fired = true;
        action.Invoke();
    }

    public void Unfire()
    {
        fired = false;
    }
}

//this class holds events to be fired at specific times in the scene. It holds them in a dictionary that way you don't have to call update on these.
public class EventTimers : MonoBehaviour {

    public static float time; //custom time holder, scene can't be trusted since we may not reload scene, or we rewind....

    static List<TimedEvent> events = new List<TimedEvent>();

    private void Awake()
    {
        events = new List<TimedEvent>();
    }

    // Use this for initialization
    void Start () {
		
	}

    int u; //iterator variable to save on GC every frame


	void FixedUpdate () {

        time += Time.fixedDeltaTime;

        for(u = 0; u < events.Count; u++)
        {
            if (events[u].time < time && !events[u].fired)
            { //if it is time and not fired...
                events[u].Fire(); //sets to fired and fires event
            }
        }

	}

    
    //resets to zero
    public static void ResetTime()
    {
        time = 0;
        for (int i = 0; i < events.Count; i++) {
            events[i].Unfire();
        }
    }

    public static void AddEvent(float _time, UnityEvent _action)
    {
        events.Add(new TimedEvent(_time, _action));
    }

    //resets events before time
    public static void SetTime(float _time)
    {
        time = _time;

        for (int i = 0; i < events.Count; i++)
        {
            if(events[i].time > time)
                events[i].Unfire(); //make sure set to not fired
        }
    }

} // end class
