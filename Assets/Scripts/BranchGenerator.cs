using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchGenerator
{
	public List<Vector3> vertices { get; private set; } = new List<Vector3>();
	private List<int> triangles = new List<int>();
	public bool debugEnabled = false;
	private TreeDataSO d;

	public Mesh GenerateBranchMesh(Mesh treeMesh, TreeDataSO treeDataSO)
	{
		d = treeDataSO;
		vertices = new List<Vector3>();
		triangles = new List<int>();
		vertices = GenerateVerts(d);

		GenerateTriangles(d.numberOfSlices, d.amountOfVertsAroundCircumference);


		treeMesh.Clear();

		treeMesh.SetVertices(vertices);
		treeMesh.SetTriangles(triangles, 0);
		treeMesh.RecalculateNormals();
		return treeMesh;
	}

	private List<Vector3> GenerateVerts(TreeDataSO d)
	{
		float currentRadius = d.radius;
		List<Vector3> verts = new List<Vector3>();
		bool canRandomise = true;
		for (int i = 0; i < d.numberOfSlices; i++)
		{
			
			currentRadius = d.radius * d.branchRadiusReductionCurve.Evaluate((float) i / (d.numberOfSlices - 1));
			if (i == d.numberOfSlices - 1)
			{
				canRandomise = false;
			}
			
			for (int y = 0; y < d.amountOfVertsAroundCircumference; y++)
			{
				verts.Add(CalculateVertPosition(i, y, verts, currentRadius, d,canRandomise));
			}
		}

		return verts;
	}


	private List<int> GenerateTriangles(int numberOfSlices, int amountOfVertsAroundCircumference)
	{
		for (int i = 0; i < numberOfSlices - 1; i++)
		{
			GenerateLayerTriangles(i, amountOfVertsAroundCircumference);
		}

		return triangles;
	}

	private void GenerateLayerTriangles(int layer, int amountOfVertsAroundCircumference)
	{
		int layerAddition = layer * amountOfVertsAroundCircumference;
		for (int i = 0; i < amountOfVertsAroundCircumference - 1; i++)
		{
			int root = i + layerAddition;
			int rootLeft = root + 1;
			int rootUpleft = root + amountOfVertsAroundCircumference + 1;
			int rootUp = root + amountOfVertsAroundCircumference;

			if (debugEnabled)
			{
				Debug.DrawLine(vertices[root], vertices[rootUpleft], Color.black);
				Debug.DrawLine(vertices[rootUpleft], vertices[rootLeft], Color.black);
				Debug.DrawLine(vertices[rootLeft], vertices[root], Color.black);
				Debug.DrawLine(vertices[root], vertices[rootUp], Color.red);
				Debug.DrawLine(vertices[rootUp], vertices[rootUpleft], Color.red);
				Debug.DrawLine(vertices[rootUpleft], vertices[root], Color.red);
			}

			triangles.Add(root);
			triangles.Add(rootUpleft);
			triangles.Add(rootLeft);
			triangles.Add(root);
			triangles.Add(rootUp);
			triangles.Add(rootUpleft);
		}

		int start = amountOfVertsAroundCircumference - 1 + layerAddition;
		int startLeft = layerAddition;
		int startLeftUp = start + 1;
		int startup = start + amountOfVertsAroundCircumference;
		if (debugEnabled)
		{
			Debug.DrawLine(vertices[start], vertices[startLeftUp], Color.green);
			Debug.DrawLine(vertices[startLeftUp], vertices[startLeft], Color.green);
			Debug.DrawLine(vertices[startLeft], vertices[start], Color.green);

			Debug.DrawLine(vertices[start], vertices[startup], Color.blue);
			Debug.DrawLine(vertices[startup], vertices[startLeftUp], Color.blue);
			Debug.DrawLine(vertices[startLeftUp], vertices[start], Color.blue);
		}

		triangles.Add(start);
		triangles.Add(startLeftUp);
		triangles.Add(startLeft);
		triangles.Add(start);
		triangles.Add(startup);
		triangles.Add(startLeftUp);
	}


	private Vector3 CalculateVertPosition(int layerIndex, int vertIndexAroundCircumference, List<Vector3> verts,
		float radius, TreeDataSO d, bool canRandomise
	)
	{
		Quaternion rotation = Quaternion.Euler(d.rotation);
		float angleRadians = (vertIndexAroundCircumference / (float) d.amountOfVertsAroundCircumference) *
		                     MathFunctions.TAU;
		float maxAmountOfRandom = d.sliceHeight / d.randomFactor;
		float randomness = canRandomise && d.randomise ? Random.Range(-maxAmountOfRandom, maxAmountOfRandom) : 0;
		return rotation * new Vector3(
			Mathf.Cos(angleRadians) * radius + randomness,
			d.startPos.y + d.sliceHeight * layerIndex,
			Mathf.Sin(angleRadians) * radius + randomness
		) + d.startPos;
	}
}