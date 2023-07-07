using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHomeAction : MonoBehaviour, DroneAction
{
    public Vector3 target;
    public DroneController controller;
    public AutoPilot autoPilot;
    public readonly Guid id = Guid.NewGuid();
    public GameObject drone;

    public float velocity = 1;
    public float timeLeft;
    public float nearlyFinishedTime;
    public bool nearlyFinished = false;
    public bool running = false;

    // Start is called before the first frame update
    void Start()
    {
        autoPilot = GetAutoPilot();
        drone = autoPilot.GetDrone();
        controller = autoPilot.GetController();
    }

    internal AutoPilot GetAutoPilot()
    {
        var a = transform.parent.gameObject.GetComponent<AutoPilot>();
        if (a == null)
        {
            a = transform.parent.parent.gameObject.GetComponent<AutoPilot>();
        }
        return a;
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
                nearlyFinished = false;
                timeLeft = 0;
            }
        }
    }

    public void Execute()
    {
        if (!running)
        {
            target = autoPilot.homeHoverPosition;
            var duration = ComputeDuration();
            timeLeft = duration;
            nearlyFinishedTime = timeLeft * 0.1f;
            autoPilot.GetConnection().MoveTo(autoPilot.id, 0, duration, target.x, target.z, target.y, 0);
            running = true;
        }
    }

    public Guid Id()
    {
        return id;
    }

    public Vector3? TargetPoint()
    {
        return autoPilot.homeHoverPosition;
    }

    public GameObject TargetGameObject()
    {
        return null;
    }

    public void Trigger(Collider col)
    {
        //nothing
    }

    internal float ComputeDuration()
    {
        float distance = Vector3.Distance(drone.transform.position, target);
        return Math.Max((distance / velocity), 2f);
    }
}
