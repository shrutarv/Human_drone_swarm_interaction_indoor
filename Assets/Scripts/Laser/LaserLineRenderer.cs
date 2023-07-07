using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LaserLineRenderer : MonoBehaviour
{

    public GameObject floor;
    public bool pointMode = false;
    public List<Vector3> points = new List<Vector3>();

    public LaserAdapter laserAdapter;
    public LaserBehaviour laserBehaviour;

    Material gizmoMaterial;
    public bool drawGizmos;
    public Color gizmoColor = Color.yellow;

    // Start is called before the first frame update
    public void Start()
    {
        //Debug.Log("BASE START");
        laserAdapter = new LaserAdapter(this);
        laserBehaviour = gameObject.GetComponent<LaserBehaviour>();
        if (laserBehaviour == null) Debug.Log("Laserbehaviour == null");
        laserBehaviour.AddShape(laserAdapter);
    }

    public void Reset()
    {
        points.Clear();
    }

    void OnRenderObject()
    {
        if (!drawGizmos || points.Count < 2)
            return;

        if (!gizmoMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            gizmoMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            gizmoMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            gizmoMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            gizmoMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            gizmoMaterial.SetInt("_ZWrite", 0);
        }

        gizmoMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(gizmoColor);

        Vector3 lastPos = transform.TransformPoint(points[0]);

        for (int i = 0; i < points.Count; i += 1)
        {
            GL.Vertex3(lastPos.x, lastPos.y, lastPos.z);
            lastPos = transform.TransformPoint(points[i]);
            GL.Vertex3(lastPos.x, lastPos.y, lastPos.z);
        }

        GL.End();
    }


    // Update is called once per frame
    public void Update()
    {
        if (pointMode && floor != null && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == floor.transform)
                {
                    points.Add(new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z));
                }
            }
        }
    }

    public class LaserAdapter : LaserGraphicsShape
    {
        LaserLineRenderer llr;

        public LaserAdapter(LaserLineRenderer llr)
        {
            this.llr = llr;
            this.Color = llr.gizmoColor;
            
            
            UpdatePoints();
        }

        public float LaserX(float x) { return 1700f + (x * 1000f + 75f) * 2.95f; }
        public float LaserY(float y) { return -15500f + (y * 1000f + 4060f) * 2.95f; }

        private void UpdatePoints()
        {

            if (LaserPath == null || LaserPath.Length != llr.points.Count)
            {
                LaserPath = new PointWithColor[llr.points.Count];
                PointCount = llr.points.Count;
                for (int i = 0; i < llr.points.Count; i++)
                {
                    LaserPath[i] = new PointWithColor();
                }
            }

            for(int i = 0; i < llr.points.Count; i++)
            {
                var transformedPoint = llr.transform.TransformPoint(llr.points[i]);
                LaserPath[i].X = LaserX(transformedPoint.x);
                LaserPath[i].Y = LaserY(transformedPoint.z);
                LaserPath[i].Color = llr.gizmoColor;
            }
        }

        public override void Update()
        {
            
            UpdatePoints();

        }
    }

}

[CustomEditor(typeof(LaserLineRenderer))]
public class BezierPathCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        LaserLineRenderer llr = (LaserLineRenderer)target;

        if (GUILayout.Button("Reset"))
        {
            llr.Reset();
        }


    }
}


