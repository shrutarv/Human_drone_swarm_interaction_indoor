using UnityEngine;
using System.Collections;
using Stateless;
using UnityEngine.AI;
using System;
using Stateless.Graph;
using Helper;
using System.Collections.Generic;
using Reservation;
using Dependency;
using System.Text;

public class PlayerControllerSM : MonoBehaviour, IResourceUser
{

    enum Trigger
    {
        SetDestination,
        FinishedPathFinding,
        FinishedReserving,
        ArrivingAtSection,
        WaitForNextFreeSection,
        NextSectionIsFree,
        StartMovingToNextSection,
        DestinationReached,
        CleaningFinished,
    }

    enum State
    {
        WaitingForNewDestination,
        Pathfinding,
        Reserving,
        Travelling,
        ArrivedAtSection,
        WaitingForFreeNextSection,
        MovingToNextSection,
        Cleaning
    }

    Material highlightMaterial;
    Material rememberMaterial = null;
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
    public int indexNextSection = -1;
    public int indexCurrentSection = -1;
    public ReservationManager currentSection = null;
    public ReservationManager nextSection = null;
    public string state;
    public string AgentHasPath = "Not initialized.";
    public string AgentPathPending = "Not initialized.";
    public string Name { get => gameObject == null ? "null" : gameObject.name; }
    public Vector3 Source { get => source; }
    public Vector3 Destination { get => destination; }
    public string CurrentState { get => _state.ToString(); }
    public EventHistory history;
    public int clock = 0;
    public HashSet<ReservationManager> reservationManagers = new HashSet<ReservationManager>();

    private State _state = State.WaitingForNewDestination;
    private StateMachine<State, Trigger> _machine;

    public bool stepMode = false;

    public int debugButtonPathStepIndex;
    public int debugButtonTimeStamp;

    public DependencyNode<IResourceUser> DependencyNode { get; set; }
    public DependencyManager dependencyManager;
    public ReservationPath ReservationPath { get; private set; }

    private void SetupStateMachine()
    {
        _machine = new StateMachine<State, Trigger>(() => _state, s => { _state = s; state = s.ToString(); });

        _machine.Configure(State.WaitingForNewDestination)
            .OnEntry(() => history.Add(clock, $"Entering state {_state.ToString()}"))
            .Permit(Trigger.SetDestination, State.Pathfinding);

        _machine.Configure(State.Pathfinding)
            .OnEntry(StartPathfinding)
            .Permit(Trigger.FinishedPathFinding, State.Reserving);

        _machine.Configure(State.Reserving)
            .OnEntry(StartReserving)
            .Permit(Trigger.FinishedReserving, State.ArrivedAtSection);

        _machine.Configure(State.Travelling);

        _machine.Configure(State.ArrivedAtSection)
            .SubstateOf(State.Travelling)
            .OnEntry(ArrivingAtSection)
            .Permit(Trigger.WaitForNextFreeSection, State.WaitingForFreeNextSection)
            .Permit(Trigger.DestinationReached, State.Cleaning);

        _machine.Configure(State.WaitingForFreeNextSection)
            .SubstateOf(State.Travelling)
            .OnEntry(TestIfEntryIsAlreadyPermitted)
            .PermitIf(Trigger.StartMovingToNextSection, State.MovingToNextSection, IsEntryPermittedToNextSection);

        _machine.Configure(State.MovingToNextSection)
            .SubstateOf(State.Travelling)
            .OnEntry(MoveToNextPoint)
            .Permit(Trigger.ArrivingAtSection, State.ArrivedAtSection);

        _machine.Configure(State.Cleaning)
            .OnEntry(DoTheCleaning)
            .Permit(Trigger.CleaningFinished, State.WaitingForNewDestination);


        //use output for visualizing the state machine graph on  http://www.webgraphviz.com 
        //Debug.Log(UmlDotGraph.Format(_machine.GetInfo()));

    }
    


    // Use this for initialization
    void Start()
    {
        history = new EventHistory();
        highlightMaterial = Resources.Load("Materials/HighlightCyan", typeof(Material)) as Material;
        SetupStateMachine();
        agent.updateRotation = false;
        cam = Camera.main;
        vap = GetComponent<VisualizeAgentPath>();
        gg = GetComponent<GraphGenerator>();

        var agentManager = GameObject.Find("AgentManager");
        dependencyManager = agentManager?.GetComponent<DependencyManager>();
        DependencyNode = dependencyManager?.CreateDependencyNode(this);
        ReservationPath = new ReservationPath();

        ReservationPath.ResourceUserAdded += (sender, e) =>
        {
            DependencyNode.AddDependency(e.ResourceUser.DependencyNode);
            dependencyManager?.UpdateGlobalOrder();
        };

        ReservationPath.ResourceUserRemoved += (sender, e) =>
        {
            DependencyNode.RemoveDependency(e.ResourceUser.DependencyNode);
            dependencyManager?.UpdateGlobalOrder();
        };

    }

    public void RegisterReservationManager(ReservationManager rm)
    {
        if (!reservationManagers.Contains(rm))
        {
            reservationManagers.Add(rm);
        }
    }

    public void LogHistory()
    {
        Debug.Log(history.TextReversedTime(), gameObject);
    }

    public void ToggleHighlight()
    {
        if (rememberMaterial == null)
        {
            rememberMaterial = gameObject.GetComponent<MeshRenderer>().material;
            gameObject.GetComponent<MeshRenderer>().material = highlightMaterial;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material = rememberMaterial;
            rememberMaterial = null;
        }
    }

    public void ReservePath()
    {
        if (_machine.CanFire(Trigger.SetDestination))
        {
            _machine.Fire(Trigger.SetDestination);
        }
    }

    public void LogReservationDependecies()
    {
        Debug.Log(ReservationPath.Text(), gameObject);
    }

    public void ButtonInvalidateReservation()
    {
        InvalidateReservation(debugButtonPathStepIndex, debugButtonTimeStamp, "DebugButton pressed", new StringBuilder(), "");
    }

    public void StepToNextSection()
    {
        if(stepMode && _machine.CanFire(Trigger.ArrivingAtSection))
        {
            gameObject.transform.position = nextSection.gameObject.transform.position;
            _machine.Fire(Trigger.ArrivingAtSection);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r") && _machine.CanFire(Trigger.SetDestination))
        {
            StartCoroutine(DelayedStart());

        }

        AgentHasPath = agent.hasPath ? "True" : "False";
        AgentPathPending = agent.pathPending ? "True" : "False";

        if(agent.hasPath && !agent.pathPending && _machine.CanFire(Trigger.FinishedPathFinding))
        {
            _machine.Fire(Trigger.FinishedPathFinding);
        }

        if(agent.remainingDistance < 0.05f && _machine.CanFire(Trigger.ArrivingAtSection))
        {
            _machine.Fire(Trigger.ArrivingAtSection);
        }

        if (Input.GetKeyDown("space") && _machine.CanFire(Trigger.StartMovingToNextSection))
        {
            _machine.Fire(Trigger.StartMovingToNextSection);
        }
    }

    public void Go()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {

        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0f, 0.5f));
        if (_machine.CanFire(Trigger.SetDestination))
        {
            _machine.Fire(Trigger.SetDestination);
        }
    }

    public void InformNextSectionFree(ReservationManager origin, ReservationEntry nextEntry)
    {
        history.Add(clock, $"Getting informed by {origin.name} that the next section is free:");
        history.Add(clock, $"     pathIndex {nextEntry.PathIndex} at time {nextEntry.Time}");
        history.Add(clock, $"     myInternal nextSection is {nextSection?.name} with index {indexNextSection} with master {nextSection?.ResolveMaster().name}");
        if (_machine.CanFire(Trigger.StartMovingToNextSection))
        {
            history.Add(clock, $"     start moving");
            _machine.Fire(Trigger.StartMovingToNextSection);
        }
        else
        {
            history.Add(clock, "     " + name + " was informed that the next section is free but cannot StartMovingToNextSection, state is " + _state.ToString());
        }
    }

    private void StartPathfinding()
    {
        history.Add(clock, $"Entering state {_state.ToString()}");
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit))
        //{
        //agent.autoBraking = false;
        history.Add(clock, $"     destination {destination.ToString()}");
        agent.SetDestination(destination);
        hasNewDestination = true;

        //}
    }

    private void StartReserving()
    {
        history.Add(clock, $"Entering state {_state.ToString()}");
        gg.updatePath = (Vector3[])agent.path.corners.Clone();
        Debug.Log("PlayerController for " + gameObject.name + " updating cylinders.");
        gg.UpdateCylinders();
        agent.isStopped = true;
        StartCoroutine(ReservePathAfterWaitingForOneFrame()); //to allow merging happening before reserving
    }

    private IEnumerator ReservePathAfterWaitingForOneFrame()
    {
        yield return new WaitForFixedUpdate();
        Debug.Log("PlayerController for " + gameObject.name + " reserving path.");
        gg.ReservePath(0, 0, this, history, clock);
        StartCoroutine(StartMovementAfterWaitingForOneFrame()); //to allow merging and reserving happening before moving into first cylinder
    }

    private IEnumerator StartMovementAfterWaitingForOneFrame()
    {
        yield return new WaitForFixedUpdate();
        _machine.Fire(Trigger.FinishedReserving);
    }

    private void ArrivingAtSection()
    {
        history.Add(clock, $"Entering state {_state.ToString()}");
        history.Add(clock, $"     arriving at {(nextSection != null ? nextSection.name : "no section")} with master {(nextSection != null ? nextSection.ResolveMaster().name : "no section")}");
        history.Add(clock, $"     exiting {(currentSection != null ? currentSection.name : "no section")} with master {(currentSection != null ? currentSection.ResolveMaster().name : "no section")}");
        history.Add(clock, $"     at position {transform.position.ToString()}");
        //Debug.Log("ArrivingAtSection: " + nextSection?.gameObject.name);
        //This might cause trouble --> agents that have left a resource and not entered the next one yet are in no mans land (14.02.2019, Moritz & Anike)


        currentSection?.NotifyExit(this, indexCurrentSection);
            //nextSection?.Entering(this);
        


        //if arriving at the last section (cylinder) then the destination has been reached
        if (ApproachingLastSection())
        {
            history.Add(clock, $"Approaching Last Section");
            _machine.Fire(Trigger.DestinationReached);
        }
        else
        {
            agent.isStopped = true;
            PrepareMovementToNextSection();
            _machine.Fire(Trigger.WaitForNextFreeSection);

        }
    }

    void TestIfEntryIsAlreadyPermitted()
    {
        history.Add(clock, $"Test if I am permitted to enter {nextSection.name}");
        if (IsEntryPermittedToNextSection())
        {
            history.Add(clock, $"     start moving");
            _machine.Fire(Trigger.StartMovingToNextSection);
        }
        else
        {
            history.Add(clock, $"     Not permitted to enter {nextSection?.ResolveMaster()?.name}");
            history.Add(clock, $"          current users: {nextSection?.ResolveMaster()?.reservationList.CurrentlyPresentResourceUsers}");
            history.Add(clock, nextSection?.ResolveMaster()?.reservationList.ReservationListAsText("          "));
        }
    }

    void PrepareMovementToNextSection()
    {
        history.Add(clock, $"Prepare Movement To Next Section");
        var points = gg.cylinders;

        // Returns if no points have been set up
        if (points.Count == 0)
        {
            Debug.Log("Points = 0!!");
            return;
        }

        // Choose the next point in the array as the destination
        //Debug.Log("indexNextSection: " + indexNextSection);
        //Debug.Log("points.Count: " + points.Count);

        indexCurrentSection = indexNextSection;

        if (indexNextSection < points.Count - 1)
        {
            indexNextSection++;
        }


        currentSection = nextSection;
        nextSection = points[indexNextSection].GetComponent<ReservationManager>();

        history.Add(clock, $"     next section {nextSection.name} with master {nextSection.ResolveMaster().name}");

        // Set the agent to go to the position of the next section.
        agent.destination = nextSection.gameObject.transform.position;

    }


    private bool ApproachingLastSection()
    {
        return indexNextSection == gg.cylinders.Count - 1;
    }

    private bool ApproachingFirstSection()
    {
        return indexNextSection == - 1;
    }


    bool IsEntryPermittedToNextSection()
    {
        return nextSection.EntryPermitted(this);
    }

    void MoveToNextPoint()
    {

        if (IsEntryPermittedToNextSection())
        {

            nextSection.Entering(this, indexNextSection);
            clock = nextSection.ResolveMaster().reservationList.FindReservationEntry(this, indexNextSection).Key;
            history.Add(clock, $"Entering {nextSection.name} at section {indexNextSection} with master {nextSection.ResolveMaster().name} at clock {clock}");
            history.Add(clock, $"{nextSection.ResolveMaster().reservationList.ReservationListAsText("          ")}");
            if (agent.isStopped && !stepMode)
            {
                agent.isStopped = false;
                isTravelling = true;
            }
        }
        else
        {
            history.Add(clock, $"Not permitted to enter {nextSection.name} with master {nextSection.ResolveMaster().name}");
            history.Add(clock, $"     current users: {nextSection.ResolveMaster().reservationList.CurrentlyPresentResourceUsers}");
            history.Add(clock, nextSection.ResolveMaster().reservationList.ReservationListAsText("     "));
        }
    }

    void DoTheCleaning()
    {
        history.Add(clock, $"Cleaning up and deleting");
        //Debug.Log($"History for {name}\n\n{history.TextReversedTime()}");
        DeleteReservationSlots(0, $"Cleaning for {name}");
        foreach(var rm in reservationManagers)
        {
            if (rm.ResolveMaster().reservationList._currentlyPresentResourceUsers.Contains(this))
            {
                rm.ResolveMaster().reservationList._currentlyPresentResourceUsers.Remove(this);
            }
        }
        dependencyManager?.RemoveResourceUserNodes(this);
        gg.DeleteCylinders();
        creator?.RecreateAgent(this);
        Destroy(gameObject);
    }

    public void DeleteReservationSlots(int pathStepIndex, string reason)
    {
        for (int i = pathStepIndex; i < gg.cylinders.Count; i++)
        {
            var rm = gg.cylinders[i].GetComponent<ReservationManager>();
            rm.ResolveMaster().history.Add(rm.reservationList.clock, reason);
            rm.DeleteSlot(this);

        }
    }

    public void IncrementTimestampAndPropagate(IResourceUser ru, int pathIndex, int timestamp, StringBuilder debug, string prefix)
    {
        gg.IncrementTimestampAndPropagate(ru, pathIndex, timestamp, debug, prefix);
    }

    public void InvalidateReservation(int pathStepIndex, int timeStamp, string reason, StringBuilder debug, string prefix)
    {
        //Debug.Log("InvalidateReservation: " + this.gameObject.name + ", newMaster: " + newMaster?.gameObject.name);
        //DeleteReservationSlots(pathStepIndex, reason);

        //gg.ReservePath(pathStepIndex, timeStamp, this, history, clock);
        IncrementTimestampAndPropagate(this, pathStepIndex, timeStamp, debug, prefix);
    }


}
