using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwarmCreator : MonoBehaviour
{

    public int createNextDroneId;
    public GameObject floor;
    public List<Boid> boids = new List<Boid>();
    public List<ObstacleBoid> obstacleBoids = new List<ObstacleBoid>();

    // Start is called before the first frame update
    void Start()
    {
        InitObstacles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitObstacles()
    {
        var obstacles = FindObjectsOfType<ObstacleBoid>();
        foreach(var obstacle in obstacles)
        {
            //Debug.Log($"Found Obstacle: {obstacle.name}");
            boids.Add(obstacle.GetBoid());
            obstacleBoids.Add(obstacle);
        }
    }

    public GameObject CreateDrone(int id, bool viconEnabled)
    {
        string strId = "cf" + id;
        GameObject manager = null;
        Boid boid = new Boid{ Id = id };

        if (!string.IsNullOrEmpty(strId) && transform.Find(strId) == null)
        {
            boids.Add(boid);
            manager = new GameObject(strId);
            manager.transform.parent = transform;

            var drone = new GameObject("Drone");
            DroneController droneController;
            {
                drone.transform.parent = manager.transform;

                var viconTracking = drone.AddComponent<ViconTrackingBehavior>();
                viconTracking.SubjectName = strId;
                viconTracking.IncludeYPosition = true;
                viconTracking.enabled = viconEnabled;

                droneController = drone.AddComponent<DroneController>();
                droneController.id = id;
                droneController.height = 1.6f;
                droneController.velocity = 0.5f;

                droneController.SetToCrazyflieConnection();

                var droneSimulator = drone.AddComponent<DroneSimulator>();
                droneSimulator.floor = floor;
                //droneSimulator.UseSimulator();

                

                var autoPilotGO = new GameObject("Autopilot");
                {
                    autoPilotGO.transform.parent = drone.transform;
                    var autoPilot = autoPilotGO.AddComponent<AutoPilot>();
                }

                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                {
                    cube.name = "Cube";
                    cube.transform.parent = drone.transform;
                    cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    cube.transform.localPosition = new Vector3(0, 0.1f, 0);
                    var boxCollider = cube.GetComponent<BoxCollider>();
                    boxCollider.isTrigger = true;
                    var rigidBody = cube.AddComponent<Rigidbody>();
                    rigidBody.useGravity = false;

                }
                boid.Body = cube.transform;
            }

            var leader = GameObject.CreatePrimitive(PrimitiveType.Cube);
            {
                leader.name = "Leader";
                leader.transform.parent = manager.transform;
                var leaderController = leader.AddComponent<LeaderController>();
                leaderController.drone = drone.transform;
                leaderController.droneController = droneController;
                leaderController.boid = boid;
                leaderController.boids = boids;
                leaderController.obstacleBoids = obstacleBoids;

                var transportStateMachine = leader.AddComponent<TransportStateMachine>();

                CreateMLInferenceAgentIfEnabled(leader);

                leader.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                leader.transform.localPosition = new Vector3(0, 0.1f, 0);
                var boxCollider = leader.GetComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                var rigidBody = leader.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
                rigidBody.useGravity = false;
            }
            boid.Leader = leader.transform;

            var lb = manager.AddComponent<LaserBehaviour>();
            var le = manager.AddComponent<LaserEdge>();
            le.source = drone.transform;
            le.sink = leader.transform;
        }

        return manager;
    }

    private void CreateMLInferenceAgentIfEnabled(GameObject leader)
    {
        var academyGO = GameObject.Find("MLDroneAcademy");
        if (academyGO != null && academyGO.activeInHierarchy)
        {
            var droneAcademy = academyGO.GetComponent<DroneAcademy>();
            InferenceAgent Agent = leader.AddComponent<InferenceAgent>();
            Agent.GiveBrain(droneAcademy.broadcastHub.broadcastingBrains[0]);
        }
    }


    public void CreateSimRow()
    {
        int idCounter = 1;
        for(var i = 0; i < 8; i++)
        {
                var drone = CreateDrone(idCounter++, false);
                drone.transform.position = new Vector3(i, 0, 0);
        }
    }

    public void CreateSimRow(int number)
    {
        int idCounter = 1;
        float step = 0.5f;
        float position = -3.5f;
        for (var i = 0; i < number; i++)
        {
            var drone = CreateDrone(idCounter++, false);
            drone.transform.Find("Drone").position = new Vector3(-2, 0, position);
            position += step;
        }
    }


    public void DeleteAllDrones()
    {
        int i = 0;

        //Array to hold all child obj
        List<GameObject> allChildren = new List<GameObject>();

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            //if (child.GetComponent<TrajectoryManager>() != null)
            //{
                allChildren.Add(child.gameObject);
            //}
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            Destroy(child.gameObject);
        }

        boids.Clear();
        InitObstacles();

    }
}


[CustomEditor(typeof(SwarmCreator))]
public class SwarmCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SwarmCreator s = (SwarmCreator)target;
        if (GUILayout.Button("Create Drone"))
        {
            s.CreateDrone(s.createNextDroneId, false);
        }

        if (GUILayout.Button("Create Simulated Drone Row"))
        {
            s.CreateSimRow();
        }

        if (GUILayout.Button("Delete All Drones"))
        {
            s.DeleteAllDrones();
        }

    }
}
