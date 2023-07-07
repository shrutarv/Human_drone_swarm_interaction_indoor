using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static iTween;

public class MQTTLaserConnection : MonoBehaviour, IMQTTSubscriber
{    
    public string subject;
    public const string TOPIC_REQUEST = "/unity/laser/vicon/animation";
    public GameObject MQTTManagerGO;
    public MQTTManager manager;


    // Start is called before the first frame update
    void Start()
    {
        MQTTManagerGO = GameObject.Find("MQTTManager");
        manager = MQTTManagerGO.GetComponent<MQTTManager>();
        manager.Subscribe(TOPIC_REQUEST, this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Receive(string topic, string message)
    {
        try
        {
            MQTTData data = JsonConvert.DeserializeObject<MQTTData>(message);
            Debug.Log("data is null: " + (data == null));
            Debug.Log("data" + data);
            Debug.Log("data as string: " + message);
            CreateAgent(data);
        }
        catch (JsonSerializationException ex)
        {
            Debug.Log(string.Format("MQTTLaserConnection: MQTT data JSON exception: {0}", ex.Message), gameObject);
        }
    }

    public void CreateAgent(MQTTData data)
    {
        var name = $"Agent_{data.subject}";

        var child = transform.Find(name);

		if (child != null && !data.visible)
		{
			DestroyImmediate(child.gameObject);
			return;
		}

		GameObject agent = null;

		if (data.vicon)
		{
			if (child != null)
			{
				DestroyImmediate(child.gameObject);
			}
			agent = new GameObject();
			agent.name = name;
			agent.transform.parent = gameObject.transform;
			var vtb = agent.AddComponent<ViconTrackingBehavior>();
			vtb.SubjectName = data.subject;
			var lc = agent.AddComponent<LaserObjectController>();
		} else {
			if(child == null){
				agent = new GameObject();
				agent.name = name;
				agent.transform.parent = gameObject.transform;
				var lc = agent.AddComponent<LaserObjectController>();
			} else {
				agent = child.gameObject;
			}
		}

        if (data.shape != "rectangle" && data.shape != "circle" && data.shape != "wlan" && data.shape != "arrow" && data.shape != "line") return;

        if (data.visible && agent != null)
        {
           
			if (!data.vicon)
			{
				agent.transform.position = new Vector3(data.target[0], data.target[1], data.target[2]);
			}
			var lc = agent.GetComponent<LaserObjectController>();
			lc.data = data;
            agent.transform.localScale = new Vector3(data.xscale, 1, data.yscale);
            //agent.transform.position = pos;
            
        }
		if (data.vicon)
		{
			Destroy(agent, data.duration);
		}
	}


    public class MQTTData
    {
        public string subject { get; set; }
        public float duration { get; set; } = 1;
        public string color { get; set; } = "white";
        public string shape { get; set; } = "rectangle"; //rectangle OR circle
        public int pointCount { get; set; } = 1;
        public float xscale { get; set; } = 1;
        public float yscale { get; set; } = 1;
        public string animation { get; set; } = "none";
        public bool visible { get; set; } = true;
        public float[] target { get; set; } = new float[] { 0.0f, 0.0f, 0.0f };
		public bool vicon { get; set; } = true;
    }
}

public class LaserObjectController : MonoBehaviour
{
    Vector3 ZERO = Vector3.zero;
    public MQTTLaserConnection.MQTTData data;
    public LaserBehaviour LaserBehaviour;
    public Component LaserShape;

    void Update()
    {
        if(transform.position != ZERO && LaserBehaviour == null && LaserShape == null && data != null)
        {
            LaserBehaviour = gameObject.AddComponent<LaserBehaviour>();
			// print(data);
            Debug.Log("Farbe: " + data.color);
            ColorUtility.TryParseHtmlString(data.color, out var color);

            if (data.shape == "rectangle")
            {
                Debug.Log("RECTANGLE start");
                var rectangle = gameObject.AddComponent<LaserRectangle>();
                rectangle.gizmoColor = color;
                LaserShape = rectangle;


            }
            if (data.shape == "circle")
            {
                Debug.Log("CIRCLE start");
                var circle = gameObject.AddComponent<LaserCircle>();
                //circle.Size = data.size;
                circle.numberOfPoints = data.pointCount;
                circle.gizmoColor = color;
                LaserShape = circle;
            }
            if (data.shape == "wlan")
            {
                Debug.Log("WLAN start");
                var wlan1 = gameObject.AddComponent<LaserWlan>();
                wlan1.Size = 1.0f;
                var wlan2 = gameObject.AddComponent<LaserWlan>();
                wlan2.Size = 0.8f;
                var wlan3 = gameObject.AddComponent<LaserWlan>();
                wlan3.Size = 0.6f;
                wlan1.gizmoColor = color;
                wlan2.gizmoColor = color;
                wlan3.gizmoColor = color;
                //lasershape = wlan1;
                //lasershape = wlan2;
                //LaserShape = wlan3;
            }
            if (data.shape == "arrow")
            {
                Debug.Log("ARROW start");
                var arrow = gameObject.AddComponent<LaserArrow>();
                arrow.gizmoColor = color;
            }
            if (data.shape == "line")
            {
                Debug.Log("LINE start");
                var line = gameObject.AddComponent<LaserLineConnect>();
                line.x = data.target[0];
                line.y = data.target[1];
            }
            if (data.animation == "pulse")
            {
                DoPulse();
            }


        } 
    }

    public void DoPulse()
    {
        System.Collections.Hashtable hash =
                  new System.Collections.Hashtable
                  {
                      { "amount", new Vector3(0.35f, 0, 0.35f) },
                      { "time", 0.5f },
                      { "looptype", LoopType.loop }
                  };
        iTween.PunchScale(gameObject, hash);
    }
}

[CustomEditor(typeof(MQTTLaserConnection))]
public class MQTTLaserConnectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MQTTLaserConnection m = (MQTTLaserConnection)target;
        if (GUILayout.Button("Create Laser Visu (does not work)"))
        {
            //m.CreateAgent(m.subject);
        }

    }
}
