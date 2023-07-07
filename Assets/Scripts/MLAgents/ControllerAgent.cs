using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System;

public class ControllerAgent : Agent
{
    GameObject MLDrones;
    MLSwarmSimulationManager MLSwarmSimulationManager;
    List<MLLeaderController> Leaders;

    MLLeaderController leader;

    Fencing Fencing = new Fencing();
    MLWanderingReward MLWanderingRewardFunctions;

    public float maxAngle = 0;
    public float minAngle = 0;

    GameObject targetGO;
    GameObject sinkGO;
    Vector3 target; //source
    Vector3 sink;
    public Boolean visitedSource;
    public float lastDistanceToSource = 0f;
    public float lastDistanceToSink = 0f;

    public float currentDistanceToSource;
    public float currentDistanceToSink;
    public float currentReward = 0f;

    void Start()
    {

    }

    public override void InitializeAgent()
    {
        MLDrones = GameObject.Find("MLDrones");
        MLSwarmSimulationManager = MLDrones.GetComponent<MLSwarmSimulationManager>();
        MLWanderingRewardFunctions = new MLWanderingReward();
        sink = new Vector3(0, 0.1f, 0);
        visitedSource = false;
        currentReward = 0f;

    }

    public override void AgentReset()
    {
        visitedSource = false;
        MLWanderingRewardFunctions.Reset();
        target = Fencing.GetRandomPosition();
        sink = Fencing.GetRandomPosition();
        target.y = 0.1f;
        sink.y = 0.1f;
        if (targetGO != null)
        {
            Destroy(targetGO);
        }
        if (sinkGO != null)
        {
            Destroy(sinkGO);
        }
        targetGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        DestroyImmediate(targetGO.GetComponent<BoxCollider>());
        targetGO.transform.position = target;
        targetGO.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        //sink
        sinkGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sinkGO.GetComponent<Renderer>().material.color = Color.red;
        DestroyImmediate(sinkGO.GetComponent<BoxCollider>());
        sinkGO.transform.position = sink;
        sinkGO.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        lastDistanceToSource = 0f;
        lastDistanceToSink = 0f;
        currentReward = 0f;
        TryToFindLeader();
    }

    public void TryToFindLeader()
    {
        //MLSwarmSimulationManager.ResetSimulation();
        Leaders = MLSwarmSimulationManager.GetMLLeaderControllers();
        if (Leaders != null && Leaders.Count > 0)
        {
            leader = Leaders[0];
            MLWanderingRewardFunctions.Init(leader.transform);
            leader.GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            Debug.Log("No Leader found");
        }
        leader.GetComponent<Renderer>().material.color = Color.white;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //if (leader != null)
        //{
        //AddReward(-0.05f);
        //currentReward += -0.05f;
        //var direction = new Vector3(0, 0, 0);
        //float movementTol = 0.001f;
        //if (Mathf.Abs(vectorAction[0]) < movementTol || Mathf.Abs(vectorAction[0]) < movementTol) {
        //    direction = new Vector3(vectorAction[0], 0, vectorAction[1]);
        //}
        //else
        //{
        //    direction = new Vector3(-movementTol, 0, movementTol);
        //}   
        Vector3 direction = new Vector3(vectorAction[0], 0, vectorAction[1]);
        leader.nextMLRotation = Quaternion.LookRotation(direction);

        currentDistanceToSource = Vector3.Distance(target, leader.transform.position);
        currentDistanceToSink = Vector3.Distance(leader.transform.position, sink);

        // avoiding the fence
        float xMax = Fencing.xMax; //9f;
        float xMin = Fencing.xMin; // -2f;
        float zMax = Fencing.zMax; // 5f;
        float zMin = Fencing.zMin; // -3f;
        float fenceTol = 0.2f;
        float maxDistance = Mathf.Sqrt((xMax - xMin) * (xMax - xMin) + (zMax - zMin) * (zMax - zMin));

        //var diffToSource = (lastDistanceToSource - currentDistanceToSource) / maxDistance;
        var diffToSource = 0.1f;

        if (xMax - leader.transform.position.x < fenceTol || leader.transform.position.x - xMin < fenceTol || zMax - leader.transform.position.z < fenceTol || leader.transform.position.z - zMin < fenceTol)
        {
            AddReward(-1f);
            currentReward += -1f;
        }
        if (visitedSource == false)
        {

            //currentReward += diff;
            //AddReward(diff);
            if (currentDistanceToSource < lastDistanceToSource) // to source
            {
                //var diff = (lastdistancetosource - currentdistancetosource) / lastdistancetosource;
                currentReward += 1 * diffToSource;
                AddReward(1 * diffToSource);
            }
            else
            {
                currentReward -= 2 * diffToSource;
                AddReward(-2 * diffToSource);
            }
            if (currentDistanceToSource < 0.2f)
            {
                AddReward(1.0f);
                currentReward += 1f;
                //Destroy(targetGO);
                //Done();
                visitedSource = true;
                leader.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        //var diffToSink = (lastDistanceToSink - currentDistanceToSink) / maxDistance;
        var diffToSink = 0.1f;
        if (visitedSource == true)
        {
            //currentReward += diff;
            //AddReward(diff);
            if (currentDistanceToSink < lastDistanceToSink) // to sink
            {
                currentReward += 1 * diffToSink;
                AddReward(1 * diffToSink);
            }
            else
            {
                currentReward -= 2 * diffToSink;
                AddReward(-2 * diffToSink);
            }
            if (currentDistanceToSink < 0.2f)
            {
                AddReward(1.0f);
                currentReward += 1f;
                Destroy(targetGO);
                Destroy(sinkGO);
                visitedSource = false;
                leader.GetComponent<Renderer>().material.color = Color.white;
                Done();
            }
        }

        lastDistanceToSource = currentDistanceToSource;
        lastDistanceToSink = currentDistanceToSink;
        //AddReward(MLWanderingRewardFunctions.ComputeFrequentDirectionChangeReward(Time.deltaTime));
        //AddReward(MLWanderingRewardFunctions.ComputeMaximumVisitedAreaReward());
        /*}
        else
        {
            SetReward(0f);
            TryToFindLeader();
        }*/
    }

    public override void CollectObservations()
    {
        //if (leader != null)
        //{
        //AddVectorObs(currentDistance); // 1 value
        AddVectorObs(leader.transform.position); // 3 values
                                                 //AddVectorObs(leader.transform.rotation); // 4 values
        AddVectorObs(target); // 3 values
        AddVectorObs(Convert.ToSingle(visitedSource)); // 1 value
        AddVectorObs(sink); //3 values

        //currentDistanceToSink = Vector3.Distance(leader.transform.position, sink);
        //AddVectorObs(currentDistanceToSink);

        /*}
        else
        {
            AddVectorObs(new float[6]);
            TryToFindLeader();
        }*/
    }

    public override void AgentOnDone()
    {
        // ?? should Reset or Stop simulation ??
    }
}
