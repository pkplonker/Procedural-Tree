using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayGrowth))]
public class PlayGrowthEditor : Editor
{
	public override void OnInspectorGUI()
	{
		PlayGrowth t = (PlayGrowth) target;
		if (GUILayout.Button("Play Growth"))
		{
			if (Application.isPlaying)
			{
				t.Play();
			}
			else
			{
				Debug.LogWarning("Can only be run in play mode");
			}
			
		}
		DrawDefaultInspector();

	}
}