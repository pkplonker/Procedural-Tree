using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;
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

	private void Start()
	{
		branchLength = sliceThickness - (branchLength / 10);
		transformStack = new Stack<TransformInfo>();
		rules = new Dictionary<char, string>
		{
			{'X', "F&[[X]^X]^F[^FX]&X"},
			//{'X', "F/-[[X]+/X]+F[+?FX]-X?F/-[[X]+X]+F[+?FX]+"},
			//{'X', "F"},

			//{'F', "FF+[+F-F-F]-[-F+F+F]"},
			//{'X', "[F-[+X]F[-X]+X]"},
			//{'F', "F[+F]F[-F][F]"},
			//{'X', "F[+F]F[-F][F]"},
			//{'X', "F[+F]F[-F][F]"},
			//{'F', "F[+F]F[-F][F]"},
			//{'X', "F"},
			{'F', "FF"}
		};
		//{'A', "[&FLA]/////[&FL!A]///////[&FLA]"},
		//{'F', "S ///// F"},
		//{'S', "FL"},
		//{'L',"[∧∧{-f+f+f--f+f+f}]"}
		//{'L', "[∧∧]"}

		Generate();
		GenerateFinalMesh();
	}


	private void Generate()
	{
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

		targetTransform = new GameObject("target").transform;
		//generate start
		GenerateSection();
		GenerateSection();
		for (int i = 0; i < currentString.Length; i++)
		{
			switch (currentString[i])
			{
				case 'F':

					//straight line
					targetTransform.Translate(Vector3.up * branchLength);
					GenerateSection();
					radius = radius - ((radius / 100) * (radiusReductionFactor));

					break;
				case 'f':
					if (Random.value < 0.7f) break;
					transform.Translate(Vector3.up * branchLength);

					break;
				case 'X': //nothing
					break;
				case 'S': //nothing
					break;
				case 'L': //nothing
					break;
				case '+':
					targetTransform.Rotate(Vector3.back * rotationAngleMax);
					break;
				case '-':
					targetTransform.Rotate(Vector3.forward * rotationAngleMax);
					break;
				case '&':
					targetTransform.Rotate(Vector3.left * rotationAngleMax);
					break;
				case '^':
					targetTransform.Rotate(Vector3.right * rotationAngleMax);
					break;
				case '/':
					targetTransform.Rotate(Vector3.up * rotationAngleMax);
					break;
				case '?':
					targetTransform.Rotate(Vector3.down * rotationAngleMax);
					break;


				case '[': //save
					transformStack.Push(new TransformInfo(transform, radius));
					break;
				case ']': //return
					TransformInfo ti = transformStack.Pop();
					transform.position = ti.position;
					transform.rotation = ti.rotation;
					radius = ti.radius;
					break;
				default:
					Debug.LogError("Error in string" + currentString[i]);
					break;
			}
		}
	}

	private void GenerateFinalMesh()
	{
		Mesh mesh = new Mesh
		{
			name = "New tree"
		};
		mesh.Clear();
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		MeshFilter mf = GetComponent<MeshFilter>();
		mf.mesh = mesh;
		transform.position = Vector3.zero;
		transform.rotation = quaternion.identity;
	}

	private void GenerateSection()
	{
		GenerateVerts();

		GenerateTriangles();
	}

	private void GenerateVerts()

	{
		numberOfSlicesGenerated++;
		for (int y = 0; y < quality; y++)
		{
			vertices.Add(CalculateVertPosition(y));
		}
	}

	private Vector3 CalculateVertPosition(int vertIndexAroundCircumference
	)
	{
		float angleRadians = vertIndexAroundCircumference / (float) quality *
		                     MathFunctions.TAU;


		return targetTransform.rotation * new Vector3(
			Mathf.Cos(angleRadians) * radius,
			targetTransform.position.y,
			Mathf.Sin(angleRadians) * radius
		);
	}

	private void GenerateTriangles()
	{
		for (int i = 0; i < numberOfSlicesGenerated - 1; i++)
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