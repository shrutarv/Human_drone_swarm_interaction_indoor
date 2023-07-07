using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dependency;
using Helper;
using Reservation;
using UnityEngine;
using UnityEngine.AI;

public class GraphGenerator : MonoBehaviour
{

    public const int Agent1Layer = 10;
    public int test;
    public Vector3[] updatePath;
    public Vector3[] currentPath;
    public List<GameObject> cylinders = new List<GameObject>();
    public GameObject testCylinder;
    public float step;
    public float distance = 0f;
    public Vector3 targetTest;
    public Vector3 endTest;
    public int pathUpdates;
    private bool shouldUpdate = false;
    Material cylinderMaterial;
    public DependencyManager dependencyManager;

    // Use this for initialization
    void Start()
    {
        step = 0.5001f;
        pathUpdates = 0;
        cylinderMaterial = Resources.Load("Materials/Obstacle", typeof(Material)) as Material;
        var agentManager = GameObject.Find("AgentManager");
        dependencyManager = agentManager?.GetComponent<DependencyManager>();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateCylinders();
    }

    public bool IsNewPath(Vector3[] current, Vector3[] update)
    {
        if (current.Length != update.Length)
        {
            return true;
        }
        int index = 0;
        foreach (var pos in current)
        {
            var updatePos = update[index];
            if (Vector3.Distance(pos, updatePos) > 0.3f)
            {
                return true;
            }
            index++;
        }
        return false;
    }

    public void PlaceCylinders(Vector3 start, Vector3 end)
    {
        var localStep = step;
        Vector3 target = start;
        int cylinderCounter = 0;



        distance = Vector3.Distance(target, end);

        while (distance > 0.1f && localStep < 100f)
        {

            target = Vector3.MoveTowards(start, end, localStep);
            targetTest = target;
            endTest = end;
            distance = Vector3.Distance(target, end);
            target.y = 0.85f;
            var cylinder = CreateCylinder(target, cylinderCounter++);
            cylinders.Add(cylinder);
            localStep += step;
            if (localStep >= 100f)
            {
                // Dirty workaround: paths can only have a length upto 100f
                Debug.Log("localStep >= 100f");
            }
        }
    }

    public GameObject CreateCylinder(Vector3 position, int cylinderCounter)
    {
        //Debug.Log(position);
        var sr = GameObject.Find("SpatialResources");
        var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = "Cylinder_" + gameObject.name + "_" + cylinderCounter;
        cylinder.layer = Agent1Layer;
        cylinder.transform.localScale = new Vector3(0.5f, 0.3f, 0.5f);
        cylinder.transform.position = position;
        var collider = cylinder.GetComponent<CapsuleCollider>();
        collider.isTrigger = true;
        var renderer = cylinder.GetComponent<MeshRenderer>();
        renderer.material = cylinderMaterial;
        //Destroy(renderer);
        cylinder.transform.parent = sr.transform;
        cylinder.AddComponent<ReservationManager>();
        var rb = cylinder.AddComponent<Rigidbody>();
        rb.useGravity = false;
        return cylinder;
    }

    public void DeleteCylinders()
    {
        foreach (var go in cylinders)
        {
            //Debug.Log(go.GetComponent<ReservationManager>().history.Text());
            Destroy(go);
        }
        cylinders.Clear();
    }

    public void UpdateCylinders()
    {
        if (updatePath != null && updatePath.Length > 1)
        {
            if (currentPath == null || currentPath.Length < 1)
            {
                currentPath = updatePath;
                pathUpdates++;
                shouldUpdate = true;
            }
            else
            {
                if (IsNewPath(currentPath, updatePath))
                {
                    currentPath = updatePath;
                    pathUpdates++;
                    shouldUpdate = true;
                }
                else
                {
                    return;
                }
            }
        }


        if (currentPath != null && shouldUpdate)
        {
            if (currentPath.Length > 1)
            {
                foreach (var go in cylinders)
                {
                    Destroy(go);
                }
                cylinders.Clear();

                for (int i = 0; i < currentPath.Length - 1; i++)
                {
                    var start = currentPath[i];
                    var end = currentPath[i + 1];
                    PlaceCylinders(start, end);
                    shouldUpdate = false;
                }
            }
        }
    }

    public void ReservePath(int startIndex, int startTimeStamp, IResourceUser ru, EventHistory history, int clock)
    {
        history.Add(clock, $"ReservePath from path index {startIndex} with timestamp {startTimeStamp}");
        int currentTimeStamp = startTimeStamp;
        for(int i = startIndex; i < cylinders.Count; i++)
        {
            var rm = cylinders[i].GetComponent<ReservationManager>();
            currentTimeStamp = rm.ReserveSlot(rm, i, currentTimeStamp, ru);

            history.Add(clock, $"     {i} at {rm.name} with time {currentTimeStamp}");
        }
    }

    public void IncrementTimestampAndPropagate(IResourceUser ru, int pathIndex, int timestamp, StringBuilder debug, string prefix)
    {

        SortedDictionary<int, int> pathIndexToTimeStamp = new SortedDictionary<int, int>();
        int time = timestamp;
        for (int i = pathIndex; i < cylinders.Count; i++)
        {
            var rm = cylinders[i].GetComponent<ReservationManager>();
            var entry = rm.ResolveMaster().reservationList.FindReservationEntry(ru, i);
            if(time > entry.Key)
            {
                pathIndexToTimeStamp.Add(i, time);
                time++;
            }
            else
            {
                break;
            }
        }
        debug.Append($"{prefix}IncrementTimestampAndPropagate for {ru.Name} (\n");
        if (pathIndexToTimeStamp.Count > 0)
        {

            foreach (var kvp in pathIndexToTimeStamp.Reverse())
            {
                var rm = cylinders[kvp.Key].GetComponent<ReservationManager>();
                var entry = rm.ResolveMaster().reservationList.FindReservationEntry(ru, kvp.Key);
                debug.Append($"{prefix}    {kvp.Key}: {entry.Value?.User.Name} at {entry.Value?.Resource.name} time {kvp.Value}\n");
            }
        }
        else
        {
            debug.Append($"{prefix}     nothing to update\n");
        }

        debug.Append($"\n");

        foreach (var kvp in pathIndexToTimeStamp.Reverse())
        {
            var rm = cylinders[kvp.Key].GetComponent<ReservationManager>();
            rm.ResolveMaster().reservationList.IncrementTimestampAndPropagate(ru, kvp.Key, kvp.Value, debug, prefix + "     ");
        }
        debug.Append($"{prefix})\n");
    }
}
