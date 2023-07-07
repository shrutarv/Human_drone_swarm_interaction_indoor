using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Delete()
    {
        Destroy(gameObject);
    }

}

[CustomEditor(typeof(TrajectoryManager))]
public class TrajectoryManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrajectoryManager m = (TrajectoryManager)target;
        if (GUILayout.Button("Do Nothing"))
        {
            //
        }
        if (GUILayout.Button("Delete"))
        {
            m.Delete();
        }

    }
}
