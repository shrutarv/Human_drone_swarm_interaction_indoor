using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MQTTTestSubscriber : MonoBehaviour, IMQTTSubscriber
{

    public string topic;

    public GameObject MQTTManagerGO;
    public MQTTManager manager;

    // Start is called before the first frame update
    void Start()
    {
        MQTTManagerGO = GameObject.Find("MQTTManager");
        manager = MQTTManagerGO.GetComponent<MQTTManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Receive(string topic, string message)
    {
        Debug.Log($"MQTT Test Subscriber: Received topic {topic}: {message}", gameObject);
    }

    public void Subscribe()
    {
        manager.Subscribe(topic, this);
    }


}

[CustomEditor(typeof(MQTTTestSubscriber))]
public class MQTTTestSubscriberEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MQTTTestSubscriber m = (MQTTTestSubscriber)target;
        if (GUILayout.Button("Subscribe"))
        {
            m.Subscribe();
        }
    }
}
