using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchGenerator
{
	public List<Vector3> vertices { get; private set; }=  new List<Vector3>();
	private List<int> triangles = new List<int>();
	public bool debugEnabled = false;
	private TreeDataSO d;
	
	public Mesh GenerateBranchMesh(Mesh treeMesh,TreeDataSO treeDataSO)
	{
		d = treeDataSO;
		vertices = new List<Vector3>();
		triangles = new List<int>();
		vertices = GenerateVerts(d.numberOfSlices,d.amountOfVertsAroundCircumference,d.startPos,d.sliceHeight,d.radius,d.rotation,d.branchRadiusReductionCurve);

		GenerateTriangles(d.numberOfSlices,d.amountOfVertsAroundCircumference);


		treeMesh.Clear();

		treeMesh.SetVertices(vertices);
		treeMesh.SetTriangles(triangles, 0);
		treeMesh.RecalculateNormals();
		return treeMesh;
	}

	private List<Vector3> GenerateVerts(int numberOfSlices, int amountOfVertsAroundCircumference, Vector3 startPos, float sliceHeight, float radius,Vector3 rotation,AnimationCurve radiusDegCurve)
	{
		float currentRadius = radius;
		List<Vector3> verts = new List<Vector3>();
		for (int i = 0; i < numberOfSlices; i++)
		{
			currentRadius = radius * radiusDegCurve.Evaluate((float) i / (numberOfSlices - 1));
			//currentRadius= Mathf.Lerp(radius, 0, (float)i/(numberOfSlices-1));
			for (int y = 0; y < amountOfVertsAroundCircumference; y++)
			{
				verts.Add(CalculateVertPosition(i, y, verts, startPos,amountOfVertsAroundCircumference,sliceHeight,currentRadius,rotation));
			}
		}

		return verts;	}
	


	private List<int> GenerateTriangles(int numberOfSlices, int amountOfVertsAroundCircumference)
	{
		for (int i = 0; i < numberOfSlices - 1; i++)
		{
			GenerateLayerTriangles(i,amountOfVertsAroundCircumference);
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
		Vector3 centre, int amountOfVertsAroundCircumference, float sliceHeight, float radius, Vector3 eulerRotation)
	{
		
		Quaternion rotation = Quaternion.Euler(eulerRotation);
		float percentageOfCircle = vertIndexAroundCircumference / (float) amountOfVertsAroundCircumference;
		float angleRadians = percentageOfCircle * MathFunctions.TAU;
		return  rotation *new Vector3(
			Mathf.Cos(angleRadians) * radius,
			centre.y + sliceHeight * layerIndex,
			Mathf.Sin(angleRadians) * radius
		)+ centre;
	}

	
}