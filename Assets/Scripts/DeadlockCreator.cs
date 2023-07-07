using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeadlockCreator : MonoBehaviour
{
    public const int Agent1Layer = 10;
    Material agentMaterial;

    // Start is called before the first frame update
    void Start()
    {
        agentMaterial = Resources.Load("Materials/Player", typeof(Material)) as Material;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateDeadlock()
    {
        var agentA = CreateAgent("A");
        var agentB = CreateAgent("B");
        var agentC = CreateAgent("C");
        var agentD = CreateAgent("D");
        var agentX = CreateAgent("X");

        var a = agentA.GetComponent<PlayerControllerSM>();
        var b = agentB.GetComponent<PlayerControllerSM>();
        var c = agentC.GetComponent<PlayerControllerSM>();
        var d = agentD.GetComponent<PlayerControllerSM>();
        var x = agentX.GetComponent<PlayerControllerSM>();

        a.source = new Vector3(1f, 0.5f, -5f);
        a.destination = new Vector3(1f, 0.5f, -1f);
        agentA.transform.position = a.source;
        a.stepMode = true;

        b.source = new Vector3(4f, 0.5f, -1f);
        b.destination = new Vector3(4f, 0.5f, -5f);
        agentB.transform.position = b.source;
        b.stepMode = true;

        c.source = new Vector3(5f, 0.5f, -4f);
        c.destination = new Vector3(0f, 0.5f, -4f);
        agentC.transform.position = c.source;
        c.stepMode = true;

        d.source = new Vector3(0f, 0.5f, -2f);
        d.destination = new Vector3(5f, 0.5f, -2f);
        agentD.transform.position = d.source;
        d.stepMode = true;

        x.source = new Vector3(-7f, 0.5f, -3f);
        x.destination = new Vector3(-7f, 0.5f, -3f);
        agentX.transform.position = x.source;
        x.stepMode = true;

    }



    public GameObject CreateAgent(string name)
    {
        //Debug.Log(position);
        var agent = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        agent.name = name;
        agent.layer = Agent1Layer;
        agent.transform.localScale = new Vector3(0.5f, 0.3f, 0.5f);
        agent.GetComponent<MeshRenderer>().material = agentMaterial;
        agent.transform.parent = gameObject.transform;
        var pc = agent.AddComponent<PlayerControllerSM>();
        var gg = agent.AddComponent<GraphGenerator>();
        var lr = agent.AddComponent<LineRenderer>();
        var vap = agent.AddComponent<VisualizeAgentPath>();
        var nma = agent.AddComponent<NavMeshAgent>();
        var rb = agent.AddComponent<Rigidbody>();
        var lb = agent.AddComponent<LaserBehaviour>();
        pc.agent = nma;
        pc.creator = null;
        return agent;
    }
}
