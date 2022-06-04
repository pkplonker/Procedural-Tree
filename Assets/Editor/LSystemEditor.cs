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
		
		t.randomise = EditorGUILayout.Toggle("Randomise?", t.randomise);
		EditorGUILayout.Space();

		if (t.randomise)
		{
			EditorGUILayout.LabelField("Random options",EditorStyles.boldLabel);
			LineBreak();
			EditorGUILayout.LabelField("Chance variables");

			//t.debugEnabled = EditorGUILayout.Slider("Random angle chance",t.randomAngle,0,1);
			//t.debugEnabled = EditorGUILayout.Slider("Flower chance",t.flowerChance,0,1);
			t.flowerChance = EditorGUILayout.Slider("Flower chance",t.flowerChance,0,1);
			t.leafChance = EditorGUILayout.Slider("Leaf chance",t.leafChance,0,1);
			t.growthChance = EditorGUILayout.Slider("Growth chance",t.growthChance,0,1);

			EditorGUILayout.LabelField("Impact");
			t.randomiseGrowthLength = EditorGUILayout.Toggle("Randomise growth?", t.randomiseGrowthLength);
			t.randomiseAngle = EditorGUILayout.Toggle("Randomise Angles?", t.randomiseAngle);


		}
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		DrawDefaultInspector();
		EditorGUILayout.Space();
		t.debugEnabled = EditorGUILayout.Toggle("Enable Debug?", t.debugEnabled);
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