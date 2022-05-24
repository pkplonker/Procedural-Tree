using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeData", menuName = "TreeData")]
public class TreeDataSO : ScriptableObject
{
	public bool debugEnabled;
	[Range(.1f, .75f)] public float radius = 1f;
	[Range(0.01f, .2f)] public float sliceHeight = .2f;
	[Range(1, 100)] public int numberOfSlices = 5;
	[Range(3, 32)] public int amountOfVertsAroundCircumference = 6;
	public Vector3 startPos;
	public Vector3 rotation;
	public AnimationCurve branchRadiusReductionCurve;
}