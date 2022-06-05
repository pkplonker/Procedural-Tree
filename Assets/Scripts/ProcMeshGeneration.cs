using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcMeshGeneration
{
	public List<Vector3> vertices { get; private set; } = new List<Vector3>();
	public List<int> triangles { get; private set; } = new List<int>();
	private LSystem lSystem;
	private bool debugEnabled;


	/// <summary>
	///   <para>Generates verts and associated triangles for new mesh section</para>
	/// <param name="lSystem">Reference injection</param>
	/// <param name="debugEnabled">Used for debug output - True to enable</param>
	/// </summary>
	public ProcMeshGeneration(LSystem lSystem, bool debugEnabled)
	{
		this.lSystem = lSystem;
		this.debugEnabled = debugEnabled;
	}

	/// <summary>
	///   <para>Generates verts based of target radius & quality</para>
	/// <param name="reduceRad">True to reduce radius of verts</param>
	/// </summary>
	public static void GenerateVerts(bool reduceRad,int quality,ref float radius,Vector3 position, bool debugEnabled, List<Vector3> vertices, float radiusReductionFactor, float ypos=0)
	{
		if (reduceRad)
		{
			radius = radius -
			         ((radius / 100) * (radiusReductionFactor));
		}

		for (int y = 0; y < quality; y++)
		{
			vertices.Add(CalculateRadialVertPosition(y,quality,radius, position,debugEnabled,ypos));
		}
	}

	/// <summary>
	///   <para>Calculates vertex position</para>
	/// <param name="vertIndexAroundCircumference">Number of points within disc</param>
	/// <returns>Returns Vector3 position of new vert position</returns>
	/// </summary>
	public static Vector3 CalculateRadialVertPosition(int vertIndexAroundCircumference, int quality, float radius,Vector3 position, bool debugEnabled, float y = 0)
	{
		float angleRadians = vertIndexAroundCircumference / (float) quality *
		                     MathFunctions.TAU;

		Vector3 pos = position + new Vector3(
			Mathf.Cos(angleRadians) * radius,
			y,
			Mathf.Sin(angleRadians) * radius
		);
		if (debugEnabled)
		{
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

			sphere.transform.position = pos;
			sphere.transform.localScale = Vector3.one * 0.02f;
			sphere.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
			GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			s.name = "centre";
			s.transform.position = position;
			s.transform.localScale = Vector3.one * 0.02f;
			s.GetComponent<MeshRenderer>().material.color = Color.blue;
		}

		return pos;
	}

	/// <summary>
	///   <para>Generates Triangles for mesh</para>
	/// </summary>
	public static void GenerateTriangles(int quality, List<Vector3> vertices,List<int>triangles)
	{
		for (int i = 0; i < (vertices.Count / quality) - 1; i++)
		{
			GenerateLayerTriangles(i,quality, triangles);
		}
	}

	/// <summary>
	///   <para>Generates Triangles for one layer of mesh</para>
	/// <param name="layer">Number of verts in disc(Quality level)</param>
	/// </summary>
	private static void GenerateLayerTriangles(int layer, int quality, List<int>triangles)
	{
		int layerAddition = layer * quality;
		for (int i = 0; i < quality - 1; i++)
		{
			var root = i + layerAddition;
			var rootLeft = root + 1;
			int rootUpleft = root + quality + 1;
			int rootUp = root + quality;


			triangles.Add(root);
			triangles.Add(rootUpleft);
			triangles.Add(rootLeft);
			triangles.Add(root);
			triangles.Add(rootUp);
			triangles.Add(rootUpleft);
		}

		int start = quality - 1 + layerAddition;
		int startLeft = layerAddition;
		int startLeftUp = start + 1;
		int startup = start + quality;

		triangles.Add(start);
		triangles.Add(startLeftUp);
		triangles.Add(startLeft);
		triangles.Add(start);
		triangles.Add(startup);
		triangles.Add(startLeftUp);
	}

	/// <summary>
	///   <para>Clears vert and tri data</para>
	/// </summary>
	public static void Clear(List<Vector3> vertices, List<int> triangles)
	{
		vertices.Clear();
		triangles.Clear();
	}
	
	/// <summary>
	///   <para>Generates mesh from previously generated list of verts and triangles</para>
	/// <returns>Mesh of produced section</returns>
	/// </summary>
	public static Mesh GenerateMesh(List<Vector3> vertices, List<int> triangles, string n = "Tree")
	{
		Mesh mesh = new Mesh
		{
			name = n
		};
		mesh.Clear();
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		Clear(vertices,triangles);
		return mesh;
	}
}