using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TreeGenerator : MonoBehaviour
{
	[SerializeField] private bool debugEnabled;
	[Range(.1f, .75f)] [SerializeField] private float radius = 1f;

	[Range(0.01f, .2f)] [SerializeField] private float sliceHeight = .2f;

	[Range(1, 100)] [SerializeField] private int numberOfSlices = 5;

	[Range(3, 32)] [SerializeField] private int amountOfVertsAroundCircumference = 6;
	[SerializeField] private Vector3 startPos;


	private Mesh trunkMesh;

	private void Awake()
	{
		trunkMesh = new Mesh
		{
			name = "Tree"
		};
		GetComponent<MeshFilter>().sharedMesh = trunkMesh;
	}

	private void Update()
	{
		trunkMesh = new BranchGenerator().GenerateBranchMesh(trunkMesh,radius,sliceHeight,numberOfSlices,amountOfVertsAroundCircumference, startPos);
	}

}