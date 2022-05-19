using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TreeGenerator : MonoBehaviour
{
	[Range(.1f, 2f)]
	[SerializeField] private float radius = 1f;
	
	[Range(0.01f, 1f)]
	[SerializeField] private float sliceHeight = .2f;
	
	[Range(2, 100)]
	[SerializeField] private int numberOfSlices = 15;
	
	[Range(5, 32)]
	[SerializeField] private int amountOfVertsAroundCircumference = 6;

	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private Mesh treeMesh;

	private void Awake()
	{
		treeMesh = new Mesh
		{
			name = "Tree"
		};
		GetComponent<MeshFilter>().sharedMesh = treeMesh;
		
	}

	private void Update()
	{
		treeMesh = GenerateTreeMesh();
	}

	private Mesh GenerateTreeMesh()
	{
		vertices = GenerateVerts();
		/*
		triangles = GenerateTriangles();

		treeMesh.Clear();
		treeMesh.SetVertices(vertices);
		treeMesh.SetTriangles(triangles,0);
		*/
		return null;
	}



	private List<Vector3> GenerateVerts()
	{
		List<Vector3> verts = new List<Vector3>();
		for (int i = 0; i < numberOfSlices; i++)
		{
			for (int y = 0; y < amountOfVertsAroundCircumference; y++)
			{
				
				verts.Add(CalculateVertPosition(i, y, verts,transform.position));
			}
		}

		return verts;
	}
	private List<int> GenerateTriangles()
	{
		List<int> triangles = new List<int>();
		
		for (int i = 0; i < numberOfSlices; i++)
		{
			for (int y = 0; y < amountOfVertsAroundCircumference; y++)
			{
				triangles.Add(i+y);
				triangles.Add(i+y+3);
				triangles.Add(i+y+2);
				
				triangles.Add(i+y+3);
				triangles.Add(i+y);
				triangles.Add(i+y+1);

			}
		}


		return triangles;
	}
	private Vector3 CalculateVertPosition(int layerIndex, int vertIndexAroundCircumference, List<Vector3> verts, Vector3 centre)
	{
		float percentageOfCircle = vertIndexAroundCircumference / (float) amountOfVertsAroundCircumference;
		float angleRadians = percentageOfCircle * MathFunctions.TAU;
		return new Vector3(
			Mathf.Cos(angleRadians) * radius,
			centre.y + sliceHeight * layerIndex,
		Mathf.Sin(angleRadians) * radius
		);
	}

	private void OnDrawGizmos()
	{
		foreach (var v in vertices)
		{
			Gizmos.color= Color.red;
			Gizmos.DrawSphere(v,0.05f);
		}
	}
}