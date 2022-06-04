using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(LSystem))]
public class LSystemEditor : Editor
{
	/// <summary>
	///   <para>Custom editor for additional buttons to generate mesh and for saving</para>
	/// </summary>
	public override void OnInspectorGUI()
	{
		LSystem t = (LSystem) target;
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		if (GUILayout.Button("Force Regenerate Mesh"))
		{
			if (Application.isPlaying)
			{
				t.AlignToRule();
				t.Setup();
			}
			else
			{
				Debug.LogWarning("Can only be run in play mode");
			}
			
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		DrawDefaultInspector();
		EditorGUILayout.Space();
		LineBreak();
		t.debugEnabled = EditorGUILayout.Toggle("Enable Debug?", t.debugEnabled);
		LineBreak();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		if (GUILayout.Button("Save Mesh"))
		{
			if (Application.isPlaying)
			{
				t.SaveMesh();
			}
			else
			{
				Debug.LogWarning("Can only be run in play mode");
			}
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
	}
	/// <summary>
	///   <para>Produces a line separator for custom editor</para>
	/// </summary>
	private void LineBreak(int height = 1)
	{
		Rect rect = EditorGUILayout.GetControlRect(false, height);
		rect.height = height;
		EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
	}
}