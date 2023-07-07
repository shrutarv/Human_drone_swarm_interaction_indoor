using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentCreator : MonoBehaviour
{
    public const int Agent1Layer = 10;
    Material agentMaterial;
    public int numberAgentX;
    public int numberAgentZ;
    public float pathLengthAgentX;
    public float pathLengthAgentZ;
    public const float xOffset = -4;
    public const float zOffset = -4;


    // Start is called before the first frame update
    void Start()
    {



    }

    public void CreateMatrix()
    {
        numberAgentX = 9;
        numberAgentZ = 9;
        pathLengthAgentX = 9f;
        pathLengthAgentZ = 9f;
        agentMaterial = Resources.Load("Materials/Player", typeof(Material)) as Material;
        for (int i = 0; i < numberAgentX; i++)
        {
            var agent = CreateAgent("agentx" + i);
            var pc = agent.GetComponent<PlayerControllerSM>();
            Vector3 a = new Vector3(i + xOffset, 0.65f, zOffset + pathLengthAgentX);
            Vector3 b = new Vector3(i + xOffset, 0.5f, zOffset - 1);
            if (i % 2 == 0)
            {
                pc.source = a;
                pc.destination = b;
            }
            else
            {
                pc.source = b;
                pc.destination = a;
            }
            agent.transform.position = pc.source;
        }

        for (int i = 0; i < numberAgentZ; i++)
        {
            var agent = CreateAgent("agentz" + i);
            var pc = agent.GetComponent<PlayerControllerSM>();
            Vector3 a = new Vector3(xOffset - 1, 0.65f, i + zOffset);
            Vector3 b = new Vector3(xOffset + pathLengthAgentZ, 0.5f, i + zOffset);
            if (i % 2 == 0)
            {
                pc.source = a;
                pc.destination = b;
            }
            else
            {
                pc.source = b;
                pc.destination = a;
            }
            agent.transform.position = pc.source;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public GameObject CreateAgent(string name)
    {
        //Debug.Log(position);
        var agent = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        agent.name = name;
        agent.layer = Agent1Layer;
        agent.transform.localScale = new Vector3(0.5f, 0.3f, 0.5f);
        var renderer = agent.GetComponent<MeshRenderer>();
        renderer.material = agentMaterial;
        agent.transform.parent = gameObject.transform;
        var pc = agent.AddComponent<PlayerControllerSM>();
        var gg = agent.AddComponent<GraphGenerator>();
        var lr = agent.AddComponent<LineRenderer>();
        var vap = agent.AddComponent<VisualizeAgentPath>();
        var nma = agent.AddComponent<NavMeshAgent>();
        nma.speed = 5.0f;
        var rb = agent.AddComponent<Rigidbody>();
        var lb = agent.AddComponent<LaserBehaviour>();
        pc.agent = nma;
        pc.creator = this;
        return agent;
    }



    public void RecreateAgent(IResourceUser old)
    {
        var agent = CreateAgent(old.Name);
        var pc = agent.GetComponent<PlayerControllerSM>();
        pc.source = old.Source;
        pc.destination = old.Destination;
        agent.transform.position = pc.source;

    }
}
