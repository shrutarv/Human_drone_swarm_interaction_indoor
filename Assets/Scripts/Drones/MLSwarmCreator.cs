using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MLSwarmCreator : MonoBehaviour
{

    public int createNextDroneId;
    public GameObject floor;
    public List<Boid> boids = new List<Boid>();

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
            Debug.Log($"Found Obstacle: {obstacle.name}");
            boids.Add(obstacle.GetBoid());
        }
    }

    public GameObject CreateDrone(int id)
    {
        string strId = "cf" + id;
        GameObject manager = null;
        Boid boid = new Boid{ Id = id };

        if (!string.IsNullOrEmpty(strId) && transform.Find(strId) == null)
        {
            boids.Add(boid);
            manager = new GameObject(strId);
            manager.transform.parent = transform;

            /*var drone = new GameObject("Drone");
            DroneController droneController;
            {
                drone.transform.parent = manager.transform;

                var viconTracking = drone.AddComponent<ViconTrackingBehavior>();
                viconTracking.SubjectName = strId;
                viconTracking.IncludeYPosition = true;

                droneController = drone.AddComponent<DroneController>();
                droneController.id = id;
                droneController.height = 1f;
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
            }*/

            var leader = GameObject.CreatePrimitive(PrimitiveType.Cube);
            {
                leader.name = "Leader";
                leader.transform.parent = manager.transform;
                var leaderController = leader.AddComponent<MLLeaderController>();
                //leaderController.drone = drone.transform;
                //leaderController.droneController = droneController;
                leaderController.boid = boid;
                leaderController.boids = boids;

                leader.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                leader.transform.localPosition = new Vector3(0, 0.1f, 0);
                var boxCollider = leader.GetComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                var rigidBody = leader.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
                rigidBody.useGravity = false;
            }
            boid.Leader = leader.transform;

            //var lb = manager.AddComponent<LaserBehaviour>();
            //var le = manager.AddComponent<LaserEdge>();
            //le.source = drone.transform;
            //le.sink = leader.transform;
        }

        return manager;
    }

    public void CreateCf28AndCf32()
    {
        var cf28 = CreateDrone(28);
        cf28.transform.position = new Vector3(-1, 0, 0);
        var cf32 = CreateDrone(32);
        cf32.transform.position = new Vector3(1, 0, 0);
    }

    public void CreateRow()
    {
        int idCounter = 1;
        for(var i = 0; i < 8; i++)
        {
            
                var drone = CreateDrone(idCounter++);
                drone.transform.position = new Vector3(i, 0, 0);
        }
    }

    public void CreateRow(int number)
    {
        int idCounter = 1;
        for (var i = 0; i < number; i++)
        {
            
            var drone = CreateDrone(idCounter++);
            drone.transform.position = new Vector3(i, 0, 0);
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
            DestroyImmediate(child.gameObject);
        }

        boids.Clear();
        InitObstacles();

    }
}


[CustomEditor(typeof(MLSwarmCreator))]
public class MLSwarmCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MLSwarmCreator s = (MLSwarmCreator)target;
        if (GUILayout.Button("Create Drone"))
        {
            s.CreateDrone(s.createNextDroneId);
        }

        if (GUILayout.Button("Create cf28 & cf32"))
        {
            s.CreateCf28AndCf32();
        }

        if (GUILayout.Button("Create Row"))
        {
            s.CreateRow();
        }

        if (GUILayout.Button("Delete All Drones"))
        {
            s.DeleteAllDrones();
        }

    }
}
