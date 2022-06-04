using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LSystem : MonoBehaviour
{
	[HideInInspector] public bool debugEnabled;
	[SerializeField] private float rotationAngleMax = 30f;
	[Range(0.01f, 1f)] [SerializeField] private float radius = 0.1f;
	[Range(0.01f, .5f)] [SerializeField] private float sliceThickness = .1f;
	[Range(5, 100)] [SerializeField] int quality = 10;
	[Range(0.1f, 10f)] [SerializeField] float radiusReductionFactor = 5f;
	[SerializeField] private LSystemRule currentRule;
	private float runTimeRadius;
	private Stack<TransformInfo> transformStack;
	private string currentString = "";
	float branchLength;
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private Transform targetTransform;
	private MeshFilter mf;
	private int currentIteration;


	private void Start()
	{
		currentIteration = currentRule.iterations;
	}

	public void Setup()
	{
		mf = GetComponent<MeshFilter>();
		mf.mesh = null;
		runTimeRadius = radius;
		branchLength = sliceThickness - (branchLength / 10);
		transformStack = new Stack<TransformInfo>();

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
			if (Application.isPlaying)
			{
				Destroy(obj.gameObject);
			}
			else
			{
				DestroyImmediate(obj.gameObject);
			}
		}
	}

	public void SaveMesh()
	{
		AssetDatabase.CreateAsset(GetComponent<MeshFilter>().mesh, "Assets/Resources/GeneratedTrees/treeMesh.asset");
		AssetDatabase.SaveAssets();
	}

	private void Generate()
	{
		currentString =
			"FF[--FF[[[&FFF]FFF]^FFF]][FFFF/////FFFFF][++FF[[[&FFF]FFF]^FFF]]";
		//test string
		currentString = currentRule.axiom;
		for (int i = 0; i < currentIteration; i++)
		{
			StringBuilder sb = new StringBuilder();
			currentRule.UpdateRules();
			foreach (var c in currentString)
			{
				sb.Append(currentRule.rules.ContainsKey(c) ? currentRule.rules[c] : c.ToString());
			}


			currentString = sb.ToString();
		}

		Debug.Log(currentString);

		if (targetTransform == null)
			targetTransform = new GameObject("target").transform;
		else
		{
			targetTransform.position = Vector3.zero;
			targetTransform.rotation = quaternion.identity;
		}


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
				case '+': //rot z+
					RotateLayer(targetTransform.forward, true);
					break;
				case '-': //rot z-
					RotateLayer(targetTransform.forward, false);
					break;
				case '&': //rot x+
					RotateLayer(targetTransform.right, true);
					break;
				case '^': //rot x-
					RotateLayer(targetTransform.right, false);
					break;
				case '?': //twist +
					RotateLayer(targetTransform.up, true);
					break;
				case '/': //twist -
					RotateLayer(targetTransform.up, false);
					break;
				case '[': //save
					transformStack.Push(new TransformInfo(targetTransform, runTimeRadius));
					break;
				case ']': //return
					//	CreateObjectWithMesh();
					TransformInfo ti = transformStack.Pop();
					targetTransform.position = ti.position;
					targetTransform.rotation = ti.rotation;
					runTimeRadius = ti.radius;
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
		targetTransform.Rotate(dir * (positive ? rotationAngleMax : -rotationAngleMax));
		GenerateSection(false);
		CreateObjectWithMesh();
	}

	private void CreateObjectWithMesh()
	{
		if (vertices.Count <= quality)
		{
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
		var r = mff.GetComponent<Renderer>();
		r.shadowCastingMode = ShadowCastingMode.Off;
		r.receiveShadows = false;
		mff.mesh = GenerateMesh();
		mrr.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
	}

	private void OnDisable()
	{
		if (mf != null)
			mf.mesh = null;
	}

	private void OnDestroy()
	{
		if (mf != null)
			mf.mesh = null;
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
			runTimeRadius = runTimeRadius - ((runTimeRadius / 100) * (radiusReductionFactor));
		}

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
			Mathf.Cos(angleRadians) * runTimeRadius,
			0,
			Mathf.Sin(angleRadians) * runTimeRadius
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

	public void SetIterations(int currentIteration) => this.currentIteration = currentIteration;

	public LSystemRule GetLSystemRule() => currentRule;
}