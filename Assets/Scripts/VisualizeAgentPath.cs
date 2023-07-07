using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeAgentPath : MonoBehaviour
{
    public Vector3[] wayPoints; //You have to set these waypoints before you start to use LineRenderer.
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
    }

    // Update is called once per frame
    void Update()
    {
        if (wayPoints == null) return;
        //Debug.Log("WayPoints!");
        lineRenderer.positionCount = wayPoints.Length;
        for (int i = 0; i < wayPoints.Length; i++)
        {
            lineRenderer.SetPosition(i, wayPoints[i]);
        }
    }
}