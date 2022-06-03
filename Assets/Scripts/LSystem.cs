using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class LSystem : MonoBehaviour
{
	[SerializeField] private float rotationAngleMax = 30f;

	[SerializeField] private int iterations = 5;
	[Range(0.01f, 1f)] [SerializeField] private float radius = 0.1f;
	float branchLength;
	[Range(0.01f, .5f)] [SerializeField] private float sliceThickness = .1f;
	[Range(5, 100)] [SerializeField] int quality = 10;
	[SerializeField] float radiusReductionFactor = 5f;

	private const string axiom = "X";
	private Stack<TransformInfo> transformStack;
	private Dictionary<char, string> rules;
	private string currentString = "";

	public List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private Transform targetTransform;
	private int numberOfSlicesGenerated;
	private List<Mesh> meshes = new List<Mesh>();
	[SerializeField] private bool debugEnabled;

	private void Start()
	{
		branchLength = sliceThickness - (branchLength / 10);
		transformStack = new Stack<TransformInfo>();
		rules = new Dictionary<char, string>
		{
			//b - X - 22.5
			//{'X', "F-[[X]+X]+F[+FX]-X"},
			//{'F', "FF"}
			{'X', "F^[[X]+X]&F[^FX]-X"},
			{'F', "FF"}
		};
		//{'A', "[&FLA]/////[&FL!A]///////[&FLA]"},
		//{'F', "S ///// F"},
		//{'S', "FL"},
		//{'L',"[∧∧{-f+f+f--f+f+f}]"}
		//{'L', "[∧∧]"}
		Generate();
		CreateObjectWithMesh();
		CombineMeshes();
	}

	private void CombineMeshes()
	{
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];

		int i = 0;
		while (i < meshFilters.Length)
		{
			if (meshFilters[i].mesh != null)
			{
				combine[i].mesh = meshFilters[i].sharedMesh;
				combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			}

			meshFilters[i].gameObject.SetActive(false);
			i++;
		}

		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.indexFormat = IndexFormat.UInt32;
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
		transform.gameObject.SetActive(true);
		foreach (var obj in meshFilters)
		{
			if (obj.gameObject == gameObject) continue;
			Object.Destroy(obj.gameObject);
		}
	}
	private void Generate()
	{
		currentString =
			"FF[--FF[[[&FFF]FFF]^FFF]][FFFF/////FFFFF][++FF[[[&FFF]FFF]^FFF]]";
		//test string
		currentString = axiom;
		for (int i = 0; i < iterations; i++)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var c in currentString)
			{
				sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
			}


			currentString = sb.ToString();
		}

		Debug.Log(currentString);

		targetTransform = new GameObject("target").transform;


		for (int i = 0; i < currentString.Length; i++)
		{
			switch (currentString[i])
			{
				case 'F':

					//straight line
					GenerateVerts(false);
					targetTransform.position += (targetTransform.up * branchLength);
					GenerateSection(true);
					CreateObjectWithMesh();
					break;
				case 'f':


					break;
				case 'X': //nothing
					break;
				case 'S': //nothing
					break;
				case 'L': //nothing
					break;
				case '+'://rot z+
					RotateLayer(targetTransform.forward, true);
					break; 
				case '-'://rot z-
					RotateLayer(targetTransform.forward, false);
					break;
				case '&'://rot x+
					RotateLayer(targetTransform.right, true);
					break;
				case '^'://rot x-
					RotateLayer(targetTransform.right, false);
					break;
				case '?'://twist +
					RotateLayer(targetTransform.up, true);
					break;
				case '/': //twist -
					RotateLayer(targetTransform.up, false);
					break;
				case '[': //save
					transformStack.Push(new TransformInfo(targetTransform, radius));
					break;
				case ']': //return
					//	CreateObjectWithMesh();
					TransformInfo ti = transformStack.Pop();
					targetTransform.position = ti.position;
					targetTransform.rotation = ti.rotation;
					radius = ti.radius;
					GenerateVerts(false);
					vertices.Clear();
					triangles.Clear();
					break;
				default:
					Debug.LogError("Error in string" + currentString[i]);
					break;
			}
		}
	}

	private void RotateLayer(Vector3 dir, bool positive)
	{
		GenerateVerts(false);
		targetTransform.Rotate(dir *(positive?rotationAngleMax:-rotationAngleMax));
		GenerateSection(false);
		CreateObjectWithMesh();
	}

	private void CreateObjectWithMesh()
	{
		if (vertices.Count <= quality)
		{
			Debug.Log("!");
			return;
		}

		GameObject ob = new GameObject
		{
			transform =
			{
				parent = transform
			}
		};
		var mrr = ob.AddComponent<MeshRenderer>();
		var mff = ob.AddComponent<MeshFilter>();
		mff.mesh = GenerateMesh();
		mrr.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
	}

	private Mesh GenerateMesh()
	{
		Mesh mesh = new Mesh
		{
			name = "New tree"
		};
		mesh.Clear();
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		vertices.Clear();
		triangles.Clear();
		return mesh;
	}

	private void GenerateSection(bool reduceRad)
	{
		GenerateVerts(reduceRad);
		GenerateTriangles();
	}

	private void GenerateVerts(bool reduceRad)
	{
		if (reduceRad)
		{
			radius = radius - ((radius / 100) * (radiusReductionFactor));
		}

		numberOfSlicesGenerated++;
		for (int y = 0; y < quality; y++)
		{
			vertices.Add(CalculateVertPosition(y));
		}
	}

	private Vector3 CalculateVertPosition(int vertIndexAroundCircumference)
	{
		float angleRadians = vertIndexAroundCircumference / (float) quality *
		                     MathFunctions.TAU;

		Vector3 pos = targetTransform.position + new Vector3(
			Mathf.Cos(angleRadians) * radius,
			0,
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
			s.transform.position = targetTransform.position;
			s.transform.localScale = Vector3.one * 0.02f;
			s.GetComponent<MeshRenderer>().material.color = Color.blue;
		}

		return pos;
	}


	private void GenerateTriangles()
	{
		for (int i = 0; i < (vertices.Count / quality) - 1; i++)
		{
			GenerateLayerTriangles(i);
		}
	}

	private void GenerateLayerTriangles(int layer)
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

	private class TransformInfo
	{
		public Vector3 position;
		public Quaternion rotation;
		public float radius;


		public TransformInfo(Transform t, float radius)

		{
			position = t.position;
			rotation = t.rotation;
			this.radius = radius;
		}
	}
}