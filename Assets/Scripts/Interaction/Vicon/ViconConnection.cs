using System;
using System.Collections;
using System.Collections.Generic;
using ClassicConsoleApp1;
using UnityEditor;
using UnityEngine;

public class ViconConnection : MonoBehaviour
{
    public bool connected = false;
    public ViconTracker viconTracker;
    public int numberOfTrackingObjects = 0;

    // Start is called before the first frame update
    void Start()
    {
        viconTracker = new ViconTracker();
    }

    // Update is called once per frame
    void Update()
    {
        numberOfTrackingObjects = viconTracker.TrackingObjects.Count;
        connected = viconTracker.IsConnected();
        if (connected)
        {
            viconTracker.Update();
        }
    }


    internal void Connect()
    {
        viconTracker.Connect();
    }

    internal void Disconnect()
    {
        viconTracker.Disconnect();
    }
}

[CustomEditor(typeof(ViconConnection))]
public class ViconConnectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ViconConnection vc = (ViconConnection)target;
        if (GUILayout.Button("Connect"))
        {
            vc.Connect();
        }
        if (GUILayout.Button("Disconnect"))
        {
            vc.Disconnect();
        }

    }
}
