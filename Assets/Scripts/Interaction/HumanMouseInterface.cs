using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HumanMouseInterface : MonoBehaviour
{

    public Transform floor;
    public bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && Input.GetMouseButtonDown(0))
            {
                if (hit.transform == floor)
                {
                    transform.position = hit.point;
                }
            }
        }
    }

    public void Activate()
    {
        active = true;
        GetComponent<ViconTrackingBehavior>().enabled = false;
    }

    public void Deactivate()
    {
        active = false;
        GetComponent<ViconTrackingBehavior>().enabled = true;
    }

    [CustomEditor(typeof(HumanMouseInterface))]
    public class HumanMouseInterfaceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            HumanMouseInterface h = (HumanMouseInterface)target;
            if (GUILayout.Button("Activate"))
            {
                h.Activate();
            }
            if (GUILayout.Button("Deactivate"))
            {
                h.Deactivate();
            }

        }
    }

}
