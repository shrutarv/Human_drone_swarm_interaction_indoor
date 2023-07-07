
using System.Text;
using Dependency;
using Helper;
using Reservation;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IResourceUser
{

    public Camera cam;
    public NavMeshAgent agent;
    public AgentCreator creator;
    public Vector3 source;
    public Vector3 destination;
    public bool started = false;
    public bool hasNewDestination = false;
    public bool isTravelling = false;
    public VisualizeAgentPath vap;
    public GraphGenerator gg;
    public int destPoint = 0;
    public ReservationManager lastCylinder = null;
    public ReservationManager currentCylinder = null;

    public string Name { get => gameObject.name; }
    public Vector3 Source { get => Source; }
    public Vector3 Destination { get => Destination; }
    public string CurrentState { get => "state not supported"; }
    public EventHistory history = new EventHistory();
    public int clock = 0;
    public DependencyNode<IResourceUser> DependencyNode { get; set; }
    public ReservationPath ReservationPath { get; private set; }

    //public ThirdPersonCharacter character;

    private void Start()
    {
        agent.updateRotation = false;
        cam = Camera.main;
        vap = GetComponent<VisualizeAgentPath>();
        gg = GetComponent<GraphGenerator>();
    }


    // Update is called once per frame
    void Update()
    {
        // Destroy agent upon reaching destination and recreate agent (at original source)
        if (started && destPoint == gg.cylinders.Count - 1)//agent.remainingDistance <= agent.stoppingDistance)
        {
            gg.DeleteCylinders();
            creator.RecreateAgent(this);
            Destroy(gameObject);
        }

        // Set destination (and start movement) on mouse click
        if (Input.GetKeyDown("r"))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //agent.autoBraking = false;
                agent.SetDestination(destination);
                hasNewDestination = true;

            }
        }

        if (hasNewDestination && !started && !isTravelling && !agent.pathPending)
        {
            hasNewDestination = false;
            started = true;

            gg.updatePath = (Vector3[])agent.path.corners.Clone();
            gg.UpdateCylinders();
            gg.ReservePath(0, 0, this, history, clock);
            agent.isStopped = true;
            isTravelling = true;
            GotoNextPoint();
        }

        if (isTravelling && !agent.pathPending && agent.remainingDistance < 0.05f)
        {
            agent.isStopped = true;
            isTravelling = false;
            GotoNextPoint();
        }

        if (Input.GetKeyDown("space"))
        {
            MoveToNextPoint();
        }

        vap.wayPoints = agent.path.corners;
    }


    void GotoNextPoint()
    {
        var points = gg.cylinders;

        // Returns if no points have been set up
        if (points.Count == 0)
        {
            Debug.Log("Points = 0!!");
            return;
        }

        // Set the agent to go to the currently selected destination.
        
        agent.destination = points[destPoint].transform.position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Count;
}

    void MoveToNextPoint()
    {
        var nextCylinder = gg.cylinders[destPoint - 1].GetComponent<ReservationManager>();
        if (nextCylinder.EntryPermitted(this))
        {
            if (agent.isStopped)
            {
                agent.isStopped = false;
                isTravelling = true;
            }
        }
    }

    public void InvalidateReservation(int pathStepIndex, int timeStamp, string reason, StringBuilder debug, string prefix)
    {
        Debug.Log("InvalidateReservation: " + this.gameObject.name);
    }

    public void InformNextSectionFree(ReservationManager origin, ReservationEntry nextEntry)
    {

    }

    public void RegisterReservationManager(ReservationManager rm)
    {

    }

    public void IncrementTimestampAndPropagate(IResourceUser ru, int pathIndex, int timestamp, StringBuilder debug, string prefix)
    {

    }
}
