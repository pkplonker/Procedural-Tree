using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneralEditor : Editor
{
	[MenuItem("Stuart - ProcTree/GenerateTree")]
	public static void GenerateTree()
	{
		GameObject obj = new GameObject("New Tree");
		//TreeGenerator tg = obj.AddComponent<TreeGenerator>();
		//TreeDataSO so = ScriptableObject.CreateInstance<TreeDataSO>();
		//AssetDatabase.CreateAsset(so, "Assets/newTree.asset");
		AssetDatabase.SaveAssets();

		//tg.SetTreeData(so);
		EditorUtility.FocusProjectWindow();

		//Selection.activeObject = so;

	}
	[CanEditMultipleObjects]
	[CustomEditor(typeof(MonoBehaviour), true)]
	public class MonoBehaviourEditor : Editor
	{
	}

}