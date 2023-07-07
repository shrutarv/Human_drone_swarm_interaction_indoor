using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrajectoryProvider : MonoBehaviour
{

    public string createNextTrajectoryId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateTrajectoryManagerForId(string id)
    {
        if (!string.IsNullOrEmpty(id) && transform.Find(id) == null)
        {
            var manager = new GameObject(id);
            manager.transform.parent = transform;
            manager.AddComponent<TrajectoryManager>();
        }
    }


    public void DeleteAllTrajectoryManagers()
    {
        int i = 0;

        //Array to hold all child obj
        List<GameObject> allChildren = new List<GameObject>();

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            if (child.GetComponent<TrajectoryManager>() != null)
            {
                allChildren.Add(child.gameObject);
            }
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            Destroy(child.gameObject);
        }

    }

}

[CustomEditor(typeof(TrajectoryProvider))]
public class TrajectoryProviderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrajectoryProvider t = (TrajectoryProvider)target;
        if (GUILayout.Button("Create Trajectory Manager For ID"))
        {
            t.CreateTrajectoryManagerForId(t.createNextTrajectoryId);
        }
        if (GUILayout.Button("Delete All Trajectory Managers"))
        {
            t.DeleteAllTrajectoryManagers();
        }

    }
}

