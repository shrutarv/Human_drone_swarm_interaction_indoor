using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LaserRenderer))]
public class LaserRendererEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		LaserRenderer lr = (LaserRenderer)target;

		if (GUILayout.Button("Connect"))
		{
			lr.Connect();
		}

		if (GUILayout.Button("Disconnect"))
		{
			lr.Disconnect();
		}
	}
}
