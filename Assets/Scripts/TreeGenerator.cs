using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TreeGenerator : MonoBehaviour
{
	[Range(.1f, 2f)] [SerializeField] private float radius = 1f;

	[Range(0.01f, 1f)] [SerializeField] private float sliceHeight = .2f;

	[Range(1, 100)] [SerializeField] private int numberOfSlices = 5;

	[Range(3, 32)] [SerializeField] private int amountOfVertsAroundCircumference = 6;
	[SerializeField] int vertTestStart = 0;
	[SerializeField] int vertTestEnd = 0;

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
		treeMesh = GenerateTreeMesh(treeMesh);
		VertVisualiser();
	}

	private Mesh GenerateTreeMesh(Mesh treeMesh)
	{
		vertices = GenerateVerts();

		triangles = GenerateTriangles();


		treeMesh.Clear();

		treeMesh.SetVertices(vertices);
		treeMesh.SetTriangles(triangles, 0);

		return treeMesh;
	}


	private List<Vector3> GenerateVerts()
	{
		List<Vector3> verts = new List<Vector3>();
		for (int i = 0; i < numberOfSlices; i++)
		{
			for (int y = 0; y < amountOfVertsAroundCircumference; y++)
			{
				verts.Add(CalculateVertPosition(i, y, verts, transform.position));
			}
		}

		return verts;
	}

	private void VertVisualiser()
	{Debug.DrawLine(vertices[vertTestStart], vertices[vertTestEnd], Color.magenta);

	}
	private List<int> GenerateTriangles()
	{
		List<int> triangles = new List<int>();
		for (int i = 0; i < numberOfSlices-1; i++)
		{
			GenerateLayerTriangles(triangles,i);

		}
		return triangles;
	}

	private void GenerateLayerTriangles(List<int> ints, int layer)
	{
		int layerAddition = layer * amountOfVertsAroundCircumference;
		for (int i = 0; i < amountOfVertsAroundCircumference-1; i++)
		{
			int root = i + layerAddition;
			int rootLeft = root + 1;
			int rootUpleft = root + amountOfVertsAroundCircumference + 1;
			int rootUp = root + amountOfVertsAroundCircumference;

			Debug.DrawLine(vertices[root], vertices[rootLeft], Color.black);
			Debug.DrawLine(vertices[rootLeft], vertices[rootUpleft], Color.black);
			Debug.DrawLine(vertices[rootUpleft], vertices[root], Color.black);

			Debug.DrawLine(vertices[root], vertices[rootUpleft], Color.red);
			Debug.DrawLine(vertices[rootUpleft], vertices[rootUp], Color.red);
			Debug.DrawLine(vertices[rootUp], vertices[root], Color.red);

		}

		int start = amountOfVertsAroundCircumference - 1+layerAddition;
		int startLeft = layerAddition; 
		int startLeftUp = start + 1; 
		int startup = start+ amountOfVertsAroundCircumference ; 
		Debug.DrawLine(vertices[start], vertices[startLeft], Color.green);
		Debug.DrawLine(vertices[startLeft], vertices[startLeftUp], Color.green);
		Debug.DrawLine(vertices[startLeftUp], vertices[start], Color.green);

		Debug.DrawLine(vertices[start], vertices[startLeftUp], Color.blue);
		Debug.DrawLine(vertices[startLeftUp], vertices[startup], Color.blue);
		Debug.DrawLine(vertices[startup], vertices[start], Color.blue);
	}


	private Vector3 CalculateVertPosition(int layerIndex, int vertIndexAroundCircumference, List<Vector3> verts,
		Vector3 centre)
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
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(v, 0.02f);
		}
	}
}