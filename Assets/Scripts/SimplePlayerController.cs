using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimplePlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject destination;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 0.3f)
        {
            Destroy(gameObject);
        }
        if (agent.isOnNavMesh)
        {
            agent.destination = destination.transform.position;
        }
        transform.LookAt(destination.transform);
    }
}
