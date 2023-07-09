using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static iTween;

public class LaserRectangle : LaserLineRenderer
{
    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();

        points.Add(Vector3.right * 0.5f + Vector3.forward * 0.5f);
        points.Add(Vector3.right * 0.5f + Vector3.back * 0.5f);
        points.Add(Vector3.left * 0.5f + Vector3.back * 0.5f);
        points.Add(Vector3.left * 0.5f + Vector3.forward * 0.5f);
        points.Add(Vector3.right * 0.5f + Vector3.forward * 0.5f);

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

    public void StopAnimation()
    {
        iTween.Stop(gameObject);
    }


    [CustomEditor(typeof(LaserRectangle))]
    public class BezierPathCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();

            LaserRectangle lr = (LaserRectangle)target;

            if (GUILayout.Button("Pulse"))
            {
                lr.DoPulse();
            }

            if (GUILayout.Button("Stop Pulse"))
            {
                lr.StopAnimation();
            }


        }
    }
}
