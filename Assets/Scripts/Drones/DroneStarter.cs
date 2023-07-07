using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DroneStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [CustomEditor(typeof(DroneStarter))]
    public class DroneStarterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DroneStarter d = (DroneStarter)target;
            if (GUILayout.Button("Activate Drone Scenario"))
            {
                LaserRenderer laser = GameObject.Find("LaserConnection").GetComponent<LaserRenderer>();
				laser.Connect();
                ViconConnection vicon = GameObject.Find("ViconConnection").GetComponent<ViconConnection>();
                vicon.Connect();
                MQTTManager mqtt = GameObject.Find("MQTTManager").GetComponent<MQTTManager>();
                mqtt.Connect();

                var droneui = GameObject.Find("DroneLaserUI").GetComponent<DroneLaserUIManager>();
                droneui.EnableStateMachine(true);
                var drones = GameObject.Find("Drones");
                var transports = GameObject.Find("Transports");
                var nest = transports.transform.Find("Nest");
                nest.GetComponent<LaserRectangle>().laserAdapter.Active = true;
                var human = GameObject.Find("Human");
                var fencevisu = GameObject.Find("DroneFenceVisualization");


            }
            if (GUILayout.Button("Deactivate Drone Scenario"))
            {

                var droneui = GameObject.Find("DroneLaserUI").GetComponent<DroneLaserUIManager>();
                droneui.EnableStateMachine(false);
                var drones = GameObject.Find("Drones");
                var transports = GameObject.Find("Transports");
                var nest = transports.transform.Find("Nest");
                nest.GetComponent<LaserRectangle>().laserAdapter.Active = false;
                var human = GameObject.Find("Human");
                var fencevisu = GameObject.Find("DroneFenceVisualization");

            }
        }
    }
}
