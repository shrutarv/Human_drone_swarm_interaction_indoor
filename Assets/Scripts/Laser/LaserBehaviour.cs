
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehaviour : MonoBehaviour
{
    public bool visible;
    public GameObject LaserConnection;
    public LaserRenderer LaserRenderer;
    public List<LaserGraphicsShape> Shapes = new List<LaserGraphicsShape>();
    public float LaserX => 1700f + (transform.position.x * 1000f + 75f) * 2.95f;
    public float LaserY => -15500f + (transform.position.z * 1000f + 4060f) * 2.95f;
    public float x, y;

    // Start is called before the first frame update
    void Start()
    {
        visible = true;
        LaserConnection = GameObject.Find("LaserConnection");
        LaserRenderer = LaserConnection?.GetComponent<LaserRenderer>();
        if (LaserRenderer != null)
        {
            LaserRenderer.AddLaserBehaviour(this);
        }
    }

    public void AddShape(LaserGraphicsShape shape)
    {
        shape.LaserBehaviour = this;
        Shapes.Add(shape);
        
    }

    void OnEnable()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        x = LaserX;
        y = LaserY;

    }

    void OnDisable()
    {
        if (LaserRenderer != null)
        {
            LaserRenderer.RemoveLaserBehaviour(this);
        }
    }
}
