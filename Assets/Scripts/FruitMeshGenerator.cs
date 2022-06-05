using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitMeshGenerator
{
	public static GameObject Generate(Vector3 position, int quality, float radius, float height, bool debugEnabled,
		Material material)
	{
		if (material == null)
		{
			material = new Material(Shader.Find("Universal Render Pipeline/Lit"))
			{
				color = Color.blue
			};
		}

		return GenerateObject(GenerateMeshData(position, quality, radius, height, debugEnabled), material);
	}

	private static GameObject GenerateObject(MeshData md, Material material)
	{
		GameObject go = new GameObject("Fruit");
		var mf = go.AddComponent<MeshFilter>();
		var mr = go.AddComponent<MeshRenderer>();
		mf.mesh = ProcMeshGeneration.GenerateMesh(md.vertices, md.triangles, "Fruit");
		mr.material = material;
		return go;
	}

	private static MeshData GenerateMeshData(Vector3 position, int quality, float radius, float height,
		bool debugEnabled)
	{
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		float rad = 0;
		for (int i = 0; i <= quality; i++)
		{
			float yStep = height / quality;
			Vector3 pos = new Vector3(
				position.x,
				position.y - (height / 2) + (yStep * i),
				position.z
			);
		//	Debug.Log(Mathf.PingPong(i, height / 2));
			//float h = (height / quality) * Mathf.PingPong(i, height / 2);
/*
			float increment = radius / quality;
			if (increment * i > radius / 2)
			{
				rad = radius - increment * i;
			}
			else
			{
				rad = increment * i;
			}
			*/
			float y =  Mathf.Sin(0.5f * Mathf.PI * (i/radius));
			ProcMeshGeneration.GenerateVerts(false, quality, ref radius, pos, debugEnabled, vertices, 0, y);
		}


		ProcMeshGeneration.GenerateTriangles(quality, vertices, triangles);
		return new MeshData(vertices, triangles);
	}
	
	private class MeshData
	{
		public readonly List<Vector3> vertices;
		public readonly List<int> triangles;

		public MeshData(List<Vector3> vertices, List<int> triangles)
		{
			this.vertices = vertices;
			this.triangles = triangles;
		}
	}
}