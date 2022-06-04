using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(LSystem))]
public class LSystemEditor : Editor
{
	public override void OnInspectorGUI()
	{
		LSystem t = (LSystem) target;
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		if (GUILayout.Button("Force Regenerate Mesh"))
		{
			t.Setup();
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
			t.SaveMesh();
		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
	}

	private void LineBreak(int height = 1)
	{
		Rect rect = EditorGUILayout.GetControlRect(false, height);
		rect.height = height;
		EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
	}
}