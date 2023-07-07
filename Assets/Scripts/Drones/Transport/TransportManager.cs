using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TransportManager : MonoBehaviour
{
    int nextId = 0;
    List<ITransportManagerListener> listeners = new List<ITransportManagerListener>();

    public GameObject floor;
    public Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && Input.GetMouseButtonDown(0))
        {
            if (hit.transform == floor.transform)
            {
                target = new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
            }
        }
    }

    public Transform GetGlobalSink()
    {
        return transform.Find("Nest");
    }

    public void CreateTransportOrder(Vector3 position)
    {
        var order = new GameObject($"TransportOrder{nextId}");
        TransportOrder transportOrder;
        {
            order.transform.parent = transform;
            order.transform.position = Vector3.zero;
            transportOrder = order.AddComponent<TransportOrder>();
            transportOrder.id = nextId++;
        }

        var laserSource = new GameObject($"LaserSource{transportOrder.id}");
        {
            laserSource.transform.parent = order.transform;
            laserSource.transform.position = new Vector3(position.x, 0.1f, position.z);
            laserSource.transform.localScale = new Vector3(0.25f, 1f, 0.25f);
            var lb = laserSource.AddComponent<LaserBehaviour>();
            var circle = laserSource.AddComponent<LaserCircle>();
            circle.numberOfPoints = 16;
            circle.DoPulse();
        }

        var source = GameObject.CreatePrimitive(PrimitiveType.Cube);
        {
            source.name = $"Source{transportOrder.id}";
            source.transform.parent = order.transform;
            source.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            source.transform.position = new Vector3(position.x, 0.2f, position.z);
        }

        var sink = GameObject.CreatePrimitive(PrimitiveType.Cube);
        {
            sink.name = $"Sink{transportOrder.id}";
            sink.transform.parent = order.transform;
            sink.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            var globalSinkPosition = new Vector3(GetGlobalSink().position.x, 0.2f, GetGlobalSink().position.z);
            var sourcePosition = new Vector3(position.x, 0.2f, position.z);
            var distance = Vector3.Distance(sourcePosition, globalSinkPosition);
            var direction = globalSinkPosition - sourcePosition;
            //direction.Normalize();

            sink.transform.position = sourcePosition + Vector3.ClampMagnitude(direction, distance - 0.45f);

            var lb = sink.AddComponent<LaserBehaviour>();
            var circle = sink.AddComponent<LaserCircle>();
            circle.gizmoColor = Color.green;
            circle.numberOfPoints = 16;
        }

        var load = GameObject.CreatePrimitive(PrimitiveType.Cube);
        {
            load.name = $"Load{transportOrder.id}";
            load.transform.parent = order.transform;
            load.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            load.transform.position = new Vector3(position.x, 0.2f, position.z);

            var lb = load.AddComponent<LaserBehaviour>();
            var rectangle = load.AddComponent<LaserRectangle>();
            rectangle.gizmoColor = Color.white;
        }

        StartCoroutine(DelayedNotify(transportOrder));
    }

    IEnumerator DelayedNotify(TransportOrder order)
    {
        yield return new WaitForEndOfFrame();
        foreach (var listener in listeners)
        {
            listener.TransportOrderCreated(order);
        }
    }

    public void AddListener(ITransportManagerListener listener)
    {
        listeners.Add(listener);
    }

    [CustomEditor(typeof(TransportManager))]
    public class TransportManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TransportManager t = (TransportManager)target;
            if (GUILayout.Button("Create TransportOrder"))
            {
                t.CreateTransportOrder(t.target);
            }

            

        }
    }


}
