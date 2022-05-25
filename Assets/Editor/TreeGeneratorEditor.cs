using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(TreeGenerator))]
public class TreeGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		TreeGenerator t = (TreeGenerator) target;

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		if (GUILayout.Button("Force Regenerate Mesh"))
		{
			t.UpdateBranch();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		DrawDefaultInspector();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		if (GUILayout.Button("Save Mesh"))
		{
			t.SaveMesh();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
	}
}