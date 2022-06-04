using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LSystem : MonoBehaviour
{
	[HideInInspector] public bool debugEnabled;
	private float rotationAngle = 30f;

	[SerializeField] public LSystemRule currentRule;
	[SerializeField] private Material branchMaterial;
	[SerializeField] private Material leafMaterial;
	[SerializeField] private Material flowerMaterial;
	[SerializeField] private int randomSeed;

	public float runTimeRadius;
	private Stack<TransformInfo> transformStack;
	private string currentString = "";
	float branchLength;

	public Transform targetTransform;
	private MeshFilter mf;
	private int currentIteration;
	private List<GameObject> extraParts = new List<GameObject>();
	private ProcMeshGeneration pmg;
	private Random prng;
	public void SetIterations(int currentIteration) => this.currentIteration = currentIteration;

	/// <summary>
	///   <para>Accessor for current rule.</para>
	/// </summary>
	/// <returns>
	///   <para>The current rule SO.</para>
	/// </returns>
	public LSystemRule GetLSystemRule() => currentRule;

	private void OnValidate() => AlignToRule();
	private void Start() => AlignToRule();

	/// <summary>
	///   <para>Resets to current rule values</para>
	/// </summary>
	public void AlignToRule()
	{
		currentIteration = currentRule.iterations;
		rotationAngle = currentRule.angle;
	}

	/// <summary>
	///   <para>Triggers Mesh generation</para>
	/// </summary>
	public void Setup()
	{
		prng = new Random(randomSeed);
		pmg ??= new ProcMeshGeneration(this, debugEnabled);
		mf = GetComponent<MeshFilter>();
		mf.mesh = null;
		runTimeRadius = currentRule.radius;
		branchLength = currentRule.sliceThickness - (branchLength / 10);
		transformStack = new Stack<TransformInfo>();

		if (targetTransform == null)
			targetTransform = new GameObject("target").transform;
		else
		{
			targetTransform.position = Vector3.zero;
			targetTransform.rotation = quaternion.identity;
		}

		foreach (var e in extraParts)
		{
			Destroy(e);
		}

		Generate();
		CreateObjectWithMesh();
		CombineMeshes();
	}

	/// <summary>
	///   <para>Combine meshes from instantiated children into final mesh</para>
	/// </summary>
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

	/// <summary>
	///   <para>Saves mesh into resources folder</para>
	/// </summary>
	public void SaveMesh()
	{
		AssetDatabase.CreateAsset(GetComponent<MeshFilter>().mesh, "Assets/Resources/GeneratedTrees/treeMesh.asset");
		AssetDatabase.SaveAssets();
	}

	/// <summary>
	///   <para>Runs L System string generation/Mesh generation</para>
	/// </summary>
	private void Generate()
	{
		BuildString();

		foreach (var t in currentString)
		{
			switch (t)
			{
				case 'F': //straight line
					pmg.GenerateVerts(false);
					targetTransform.position += (targetTransform.up * branchLength);
					GenerateSection(true);
					CreateObjectWithMesh();
					break;
				case 'f': //nothing
					break;
				case 'X': //nothing
					break;
				case 'S': //Flower
					if (currentIteration < (currentRule.iterations / 2)) break;
					ProduceFlower();
					break;
				case 'L': //Leaf
					if (currentIteration < (currentRule.iterations / 2)) break;
					ProduceLeaf();
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
					pmg.GenerateVerts(false);
					pmg.Clear();
					break;
				default:
					break;
			}
		}
	}

	/// <summary>
	///   <para>Runs L System string generation</para>
	/// </summary>
	private void BuildString()
	{
		currentString = currentRule.axiom;
		for (int i = 0; i < currentIteration; i++)
		{
			StringBuilder sb = new StringBuilder();
			currentRule.UpdateRules();

			for (int j = 0; j < currentString.Length; j++)
			{
				char letter = currentString[j];
				float value;
				if (j!=currentString.Length-1 &&   currentString[j+1] == '(')
				{
					string s="";
					
					for (int k = 0; k < 7; k++)
					{
						if (currentString[j + k+2] == ')') break;
						s += currentString[j + k+2];
					}
					Debug.Log("string val = " + s);
					value = float.Parse(s);
					Debug.Log("float val = " + value);

					if (value>= UnityEngine.Random.value)
					{
						sb.Append(currentRule.rules.ContainsKey(letter) ? currentRule.rules[letter] : letter.ToString());
						sb.Append(currentRule.rules.ContainsKey(currentString[j+s.Length+3]) ? currentRule.rules[currentString[j+s.Length+3]] : letter.ToString());
						j += s.Length+3;
						Debug.Log("passed random");
					}
					else
					{
						sb.Append(currentRule.rules.ContainsKey(letter) ? currentRule.rules[letter] : letter.ToString());

						j += s.Length+4;
						Debug.Log("failed random");

					}
				}
				else
				{
					sb.Append(currentRule.rules.ContainsKey(letter) ? currentRule.rules[letter] : letter.ToString());
				}
			}


			currentString = sb.ToString();
		}
	}

	/// <summary>
	///   <para>Generates flower object</para>
	/// </summary>
	private void ProduceFlower()
	{
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		extraParts.Add(sphere);
		sphere.transform.position = targetTransform.position;
		sphere.transform.localScale =
			new Vector3(currentRule.radius / 2, currentRule.radius / 2, currentRule.radius / 2);
		sphere.GetComponent<MeshRenderer>().material = flowerMaterial;
	}

	/// <summary>
	///   <para>Generates leaf object</para>
	/// </summary>
	private void ProduceLeaf()
	{
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		extraParts.Add(sphere);
		sphere.transform.position = targetTransform.position;
		sphere.transform.localScale = new Vector3(currentRule.radius, currentRule.radius, currentRule.radius);
		sphere.GetComponent<MeshRenderer>().material = leafMaterial;
	}

	/// <summary>
	///   <para>Rotates current targetPos</para>
	/// </summary>
	/// <param name="dir">Axis of rotation</param>
	///
	/// <param name="positive">true = positive, false = negative</param>
	private void RotateLayer(Vector3 dir, bool positive)
	{
		pmg.GenerateVerts(false);
		targetTransform.Rotate(dir * (positive ? rotationAngle : -rotationAngle));
		GenerateSection(false);
		CreateObjectWithMesh();
	}

	/// <summary>
	///   <para>Generates flower object</para>
	/// </summary>
	private void CreateObjectWithMesh()
	{
		if (pmg.vertices.Count <= currentRule.quality)
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
		var mff = ob.AddComponent<MeshFilter>();
		mff.mesh = GenerateMesh();
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

	/// <summary>
	///   <para>Generates mesh from previously generated list of verts and triangles</para>
	/// <returns>Mesh of produced section</returns>
	/// </summary>
	private Mesh GenerateMesh()
	{
		Mesh mesh = new Mesh
		{
			name = "New tree"
		};
		mesh.Clear();
		mesh.SetVertices(pmg.vertices);
		mesh.SetTriangles(pmg.triangles, 0);
		mesh.RecalculateNormals();
		pmg.Clear();
		return mesh;
	}

	/// <summary>
	///   <para>Generates verts and associated triangles for new mesh section</para>
	/// <param name="reduceRad">True = Reduces end vert radius</param>
	/// </summary>
	private void GenerateSection(bool reduceRad)
	{
		pmg.GenerateVerts(reduceRad);
		pmg.GenerateTriangles();
	}

	/// <summary>
	///   <para>Container for transform data to allow stack popping</para>
	/// </summary>
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