using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BranchGeneratorLSystem : MonoBehaviour
{
	public List<Vector3> vertices { get; private set; } = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private bool debugEnabled = false;

	public Mesh GenerateBranchMesh(float baseRadius, float tipRadius,
		float sliceThickness, int amountOfVertsAroundCircumference, int numberOfSlices, bool debugEnabled,
		Quaternion rotation)
	{
		this.debugEnabled = debugEnabled;
		Mesh mesh = new Mesh();
		vertices = new List<Vector3>();
		triangles = new List<int>();


		vertices = GenerateVerts(baseRadius, tipRadius, numberOfSlices, amountOfVertsAroundCircumference,
			sliceThickness, rotation);

		GenerateTriangles(numberOfSlices, amountOfVertsAroundCircumference);


		mesh.Clear();
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		MeshFilter mf = GetComponent<MeshFilter>();
		mf.mesh = mesh;
		return mesh;
	}

	private List<Vector3> GenerateVerts(float baseRadius, float tipRadius, int numberOfSlices,
		int amountOfVertsAroundCircumference,
		float sliceThickness,
		Quaternion rotation)
	{
		List<Vector3> verts = new List<Vector3>();
		for (int i = 0; i < numberOfSlices; i++)
		{
			for (int y = 0; y < amountOfVertsAroundCircumference; y++)
			{
				verts.Add(CalculateVertPosition(i, y, baseRadius, tipRadius,
					rotation, amountOfVertsAroundCircumference,
					sliceThickness, numberOfSlices));
			}
		}

		return verts;
	}


	private void GenerateTriangles(int numberOfSlices, int amountOfVertsAroundCircumference)
	{
		for (int i = 0; i < numberOfSlices - 1; i++)
		{
			GenerateLayerTriangles(i, amountOfVertsAroundCircumference);
		}
	}

	private void GenerateLayerTriangles(int layer, int amountOfVertsAroundCircumference)
	{
		int layerAddition = layer * amountOfVertsAroundCircumference;
		for (int i = 0; i < amountOfVertsAroundCircumference - 1; i++)
		{
			var root = i + layerAddition;
			var rootLeft = root + 1;
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


	private Vector3 CalculateVertPosition(int layerIndex, int vertIndexAroundCircumference, float baseRadius,
		float tipRadius,
		Quaternion rotation, int amountOfVertsAroundCircumference,
		float sliceHeight, int numberOfSlices
	)
	{
		//debug
		float radius = baseRadius;
		float angleRadians = vertIndexAroundCircumference / (float) amountOfVertsAroundCircumference *
		                     MathFunctions.TAU;
		if (layerIndex == 0)
		{
			radius = 0;
			return rotation * new Vector3(
				Mathf.Cos(angleRadians) * 0,
				0,
				Mathf.Sin(angleRadians) * 0);
		}

		if (layerIndex == 1)
		{
			return rotation * new Vector3(
				Mathf.Cos(angleRadians) * baseRadius,
				0,
				Mathf.Sin(angleRadians) * baseRadius);
		}

		if (layerIndex == 2)
		{
			return rotation * new Vector3(
				Mathf.Cos(angleRadians) * tipRadius,
				(sliceHeight),
				Mathf.Sin(angleRadians) * tipRadius);
		}

		if (layerIndex == 3)
		{
			return rotation * new Vector3(
				Mathf.Cos(angleRadians) * tipRadius,
				sliceHeight,
				Mathf.Sin(angleRadians) * tipRadius);
		}

		radius = baseRadius - ((baseRadius - tipRadius) / (numberOfSlices + 1));


		Debug.Log("Radius = " + radius + ". Base radius = " + baseRadius + ". Tip radius = " + tipRadius);
		return rotation * new Vector3(
			Mathf.Cos(angleRadians) * radius,
			(sliceHeight * layerIndex) - sliceHeight,
			Mathf.Sin(angleRadians) * radius
		);
	}
}