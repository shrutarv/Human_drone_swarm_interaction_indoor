using ClassicConsoleApp1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViconTrackingBehavior : MonoBehaviour {

    public string SubjectName;
    public bool IncludeYPosition = true;
    public bool UseRotation = true;
    public GameObject ViconConnectionGO;
    ViconConnection viconConnection;
    ViconTracker viconTracker;
    ViconTrackingObject trackingObject;
    SavedTransform manualTransform = null;

	// Use this for initialization
	void Start () {
        ViconConnectionGO = GameObject.Find("ViconConnection");
        viconConnection = ViconConnectionGO.GetComponent<ViconConnection>();
    }
	
	// Update is called once per frame
	void Update () {


        if(viconConnection.viconTracker != null && trackingObject == null && SubjectName != "")
        {
            viconTracker = viconConnection.viconTracker;
            trackingObject = new ViconTrackingObject(SubjectName);
            viconTracker.TrackingObjects.Add(trackingObject);
        }


        if (viconConnection.connected)
        {
            if(manualTransform == null)
            {
                manualTransform = new SavedTransform(transform);
            }
            if (trackingObject.Occluded)
            {
                //GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                //GetComponent<MeshRenderer>().enabled = true;
                Vector3 pos;
                if (IncludeYPosition)
                {
                    pos = new Vector3(trackingObject.ROSposition().x, trackingObject.ROSposition().z, trackingObject.ROSposition().y);
                } else
                {
                    pos = new Vector3(trackingObject.ROSposition().x, transform.position.y, trackingObject.ROSposition().y);
                }
                transform.position = pos;
                if (UseRotation)
                {
                    transform.rotation = trackingObject.Rotation;
                }
                //Debug.Log(string.Format("vicon pos {0}", trackingObject.ToString()));
            }
        }
        else
        {
            if (manualTransform != null)
            {
                manualTransform.Apply(transform);
                manualTransform = null;
            }
        }
    }
}
