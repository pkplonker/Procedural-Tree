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
	public void GenerateVerts(bool reduceRad)
	{
		if (reduceRad)
		{
			lSystem.runTimeRadius = lSystem.runTimeRadius -
			                        ((lSystem.runTimeRadius / 100) * (lSystem.currentRule.radiusReductionFactor));
		}

		for (int y = 0; y < lSystem.currentRule.quality; y++)
		{
			vertices.Add(CalculateVertPosition(y));
		}
	}

	/// <summary>
	///   <para>Calculates vertex position</para>
	/// <param name="vertIndexAroundCircumference">Number of points within disc</param>
	/// <returns>Returns Vector3 position of new vert position</returns>
	/// </summary>
	private Vector3 CalculateVertPosition(int vertIndexAroundCircumference)
	{
		float angleRadians = vertIndexAroundCircumference / (float) lSystem.currentRule.quality *
		                     MathFunctions.TAU;

		Vector3 pos = lSystem.targetTransform.position + new Vector3(
			Mathf.Cos(angleRadians) * lSystem.runTimeRadius,
			0,
			Mathf.Sin(angleRadians) * lSystem.runTimeRadius
		);
		if (debugEnabled)
		{
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

			sphere.transform.position = pos;
			sphere.transform.localScale = Vector3.one * 0.02f;
			sphere.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
			GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			s.name = "centre";
			s.transform.position = lSystem.targetTransform.position;
			s.transform.localScale = Vector3.one * 0.02f;
			s.GetComponent<MeshRenderer>().material.color = Color.blue;
		}

		return pos;
	}

	/// <summary>
	///   <para>Generates Triangles for mesh</para>
	/// </summary>
	public void GenerateTriangles()
	{
		for (int i = 0; i < (vertices.Count / lSystem.currentRule.quality) - 1; i++)
		{
			GenerateLayerTriangles(i);
		}
	}

	/// <summary>
	///   <para>Generates Triangles for one layer of mesh</para>
	/// <param name="layer">Number of verts in disc(Quality level)</param>
	/// </summary>
	private void GenerateLayerTriangles(int layer)
	{
		int layerAddition = layer * lSystem.currentRule.quality;
		for (int i = 0; i < lSystem.currentRule.quality - 1; i++)
		{
			var root = i + layerAddition;
			var rootLeft = root + 1;
			int rootUpleft = root + lSystem.currentRule.quality + 1;
			int rootUp = root + lSystem.currentRule.quality;


			triangles.Add(root);
			triangles.Add(rootUpleft);
			triangles.Add(rootLeft);
			triangles.Add(root);
			triangles.Add(rootUp);
			triangles.Add(rootUpleft);
		}

		int start = lSystem.currentRule.quality - 1 + layerAddition;
		int startLeft = layerAddition;
		int startLeftUp = start + 1;
		int startup = start + lSystem.currentRule.quality;

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
	public void Clear()
	{
		vertices.Clear();
		triangles.Clear();
	}
}