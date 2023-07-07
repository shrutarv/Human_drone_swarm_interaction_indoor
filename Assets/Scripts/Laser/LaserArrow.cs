using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static iTween;

public class LaserArrow : LaserLineRenderer
{
    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();

        float body_halfwidth = 0.25f;
        float head_halfwidth = 0.6f;

        points.Add(Vector3.right * body_halfwidth + Vector3.forward * 0.5f);
        points.Add(Vector3.right * body_halfwidth + Vector3.back * 0.5f);
        points.Add(Vector3.left * body_halfwidth + Vector3.back * 0.5f);
        points.Add(Vector3.left * body_halfwidth + Vector3.forward * 0.5f);
        points.Add(Vector3.left * head_halfwidth + Vector3.forward * 0.5f);
        points.Add(Vector3.forward);
        points.Add(Vector3.right * head_halfwidth + Vector3.forward * 0.5f);
        points.Add(Vector3.right * body_halfwidth + Vector3.forward * 0.5f);


    }


    public void DoPulse()
    {
        System.Collections.Hashtable hash =
                  new System.Collections.Hashtable();
        hash.Add("amount", new Vector3(0.35f, 0, 0.35f));
        hash.Add("time", 0.5f);
        hash.Add("looptype", LoopType.loop);
        iTween.PunchScale(gameObject, hash);
    }


    [CustomEditor(typeof(LaserArrow))]
    public class BezierPathCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();

            LaserArrow la = (LaserArrow)target;

            if (GUILayout.Button("Pulse"))
            {
                la.DoPulse();
            }


        }
    }
}


