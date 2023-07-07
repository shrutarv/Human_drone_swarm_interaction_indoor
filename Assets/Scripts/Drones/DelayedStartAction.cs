using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedStartAction : MonoBehaviour, DroneAction
{
    public readonly Guid id = Guid.NewGuid();
    public AutoPilot autoPilot;
    public float delay;

    public float timeLeft;
    public bool running = false;


    // Start is called before the first frame update
    void Start()
    {
        autoPilot = transform.parent.gameObject.GetComponent<AutoPilot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            timeLeft -= Time.deltaTime;
            autoPilot.TimeLeft(id, timeLeft);

            if (timeLeft <= 0)
            {
                autoPilot.FinishedAction(id);
                running = false;
                timeLeft = 0;
            }
        }
    }

    public void Execute()
    {
        if (!running)
        {
            delay = autoPilot.delayedStartDuration;
            timeLeft = delay;
            running = true;
        }
    }

    public Guid Id()
    {
        return id;
    }

    public Vector3? TargetPoint()
    {
        return null;
    }

    public GameObject TargetGameObject()
    {
        return null;
    }

    public void Trigger(Collider col)
    {
       //Nothing
    }
}
