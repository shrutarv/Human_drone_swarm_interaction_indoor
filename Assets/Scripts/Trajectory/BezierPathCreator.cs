using System.Collections;
using System.Collections.Generic;
using BezierSolution;
using UnityEditor;
using UnityEngine;

namespace Pathfinding
{

    public class BezierPathCreator : MonoBehaviour
    {
        public GameObject floor;
        public bool pointMode = false;
        public List<Vector3> points = new List<Vector3>();
        public BezierSpline spline;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (pointMode && floor != null && Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if(hit.transform == floor.transform)
                    {
                        points.Add(new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z));
                    }
                }
            }
        }

        public void CreateCurve()
        {
            if(spline == null && points.Count >= 2)
            {
                spline = gameObject.AddComponent<BezierSpline>();
                spline.Initialize(points.Count);
                //spline.drawGizmos = true;
                int i = 0;
                foreach(var point in points)
                {
                    spline[i].position = point;
                    i++;
                }
                spline.AutoConstructSpline();
            }
        }

        public void Reset()
        {
            if (spline != null) Destroy(spline);
            points.Clear();
        }

        void OnRenderObject()
        {
            
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Vector3 from = default;
            Vector3 to = default;

            if (spline == null)
            {
                foreach (var point in points)
                {
                    from = to;
                    to = point;
                    if (from != default && to != default)
                    {
                        Gizmos.DrawLine(from, to);
                    }
                }
            }
            else
            {
                spline.DrawGizmos(Color.blue, 15);
            }

        }

    }

    [CustomEditor(typeof(BezierPathCreator))]
    public class BezierPathCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();

            BezierPathCreator bpc = (BezierPathCreator)target;

            if (GUILayout.Button("Reset"))
            {
                bpc.Reset();
            }

            if (GUILayout.Button("Create Curve"))
            {
                bpc.CreateCurve();
            }

        }
    }


}
