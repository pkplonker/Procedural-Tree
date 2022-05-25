using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeData", menuName = "TreeData")]
public class TreeDataSO : ScriptableObject
{
	public readonly float MIN_RANDOM_FACTOR = 3f;
	public readonly float MAX_RANDOM_FACTOR = 100f;

	public bool debugEnabled;
	[Range(.1f, .75f)] public float radius = .75f;
	[Range(0.01f, .2f)] public float sliceHeight = .2f;
	[Range(2, 200)] public int numberOfSlices = 5;
	[Range(5, 32)] public int amountOfVertsAroundCircumference = 6;
	public Vector3 startPos;
	public Vector3 rotation;
	public AnimationCurve branchRadiusReductionCurve = AnimationCurve.Linear(0, 1, 1, 1);
	public AnimationCurve xPositionShiftCurve = AnimationCurve.Linear(0, 1, 1, 1);
	public AnimationCurve yPositionShiftCurve = AnimationCurve.Linear(0, 1, 1, 1);

	public Material branchMaterial;
	[HideInInspector] public bool randomise;
	[HideInInspector] public float randomFactor=20f;


	private void OnEnable()
	{
		if (branchMaterial == null)
		{
			branchMaterial = Resources.Load("branchTest.mat", typeof(Material)) as Material;
		}
	}
}