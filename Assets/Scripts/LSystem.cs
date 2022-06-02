using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class LSystem : MonoBehaviour
{
	[SerializeField] private float rotationAngleMin = 0f;
	[SerializeField] private float rotationAngleMax = 30f;
	[SerializeField] private float upRotationAngleMin = -50f;
	[SerializeField] private float upRotationAngleMax = 50f;
	[SerializeField] private int iterations = 5;
	[Range(0.01f, 1f)] [SerializeField] private float radius = 0.1f;
	float branchLength;
	[Range(0.01f, .5f)] [SerializeField] private float sliceThickness = .1f;
	int numberOfSlices = 4;
	[Range(1, 100)] [SerializeField] int quality = 10;
	[SerializeField] float radiusReductionFactor = 5f;

	[SerializeField] private GameObject branchPrefab;
	private const string axiom = "X";
	private Stack<TransformInfo> transformStack;
	private Dictionary<char, string> rules;
	private string currentString = "";
	private List<Mesh> meshes = new List<Mesh>();

	private void Start()
	{
		branchLength = sliceThickness - (branchLength / 10);
		transformStack = new Stack<TransformInfo>();
		rules = new Dictionary<char, string>
		{
//			{'X', "F&[[X]^X]^F[^FX]&X"},
			{'X', "F/-[[X]+/X]+F[+?FX]-X?F/-[[X]+X]+F[+?FX]+"},

			//{'F', "FF+[+F-F-F]-[-F+F+F]"},
			//{'X', "[F-[+X]F[-X]+X]"},
			//{'F', "F[+F]F[-F][F]"},
			//{'X', "F[+F]F[-F][F]"},
			//{'X', "F[+F]F[-F][F]"},
			//{'F', "F[+F]F[-F][F]"},
			//{'X', "F"},
			{'F', "FF"}
			//{'A', "[&FLA]/////[&FL!A]///////[&FLA]"},
			//{'F', "S ///// F"},
			//{'S', "FL"},
			//{'L',"[∧∧{-f+f+f--f+f+f}]"}
			//{'L', "[∧∧]"}
			/*
			case '&':transform.Rotate(Vector3.left * Random.Range(0, rotationAngleMax));
			case '^':transform.Rotate(Vector3.right * Random.Range(0, rotationAngleMax));
			case '/':transform.Rotate(Vector3.up * Random.Range(0, rotationAngleMax));
			case '?':transform.Rotate(Vector3.down * Random.Range(0, rotationAngleMax));
			*/
		};
		Generate();
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

		float baseRadius = radius;
		float tipRadius = radius;
		for (int i = 0; i < currentString.Length; i++)
		{
			Vector3 initialPosition;
			GameObject segment;
			BranchGeneratorLSystem branch;
			switch (currentString[i])
			{
				case 'F':
					if (Random.value < 0.7f) break;

					//straight line
					baseRadius = tipRadius;
					tipRadius = baseRadius - ((baseRadius / 100) * (radiusReductionFactor));

					initialPosition = transform.position;
					transform.Translate(Vector3.up * branchLength);
					segment = Instantiate(branchPrefab);
					branch = segment.GetComponent<BranchGeneratorLSystem>();
					segment.transform.position = initialPosition;

					meshes.Add(branch.GenerateBranchMesh(baseRadius, currentString[i] == ']' ? 0 : tipRadius,
						sliceThickness, 3 + quality, numberOfSlices, true, transform.rotation));

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
					transform.Rotate(Vector3.back * rotationAngleMax);
					break;
				case '-':
					transform.Rotate(Vector3.forward *  rotationAngleMax);
					break;
				case '&':
					transform.Rotate(Vector3.left *  rotationAngleMax);
					break;
				case '^':
					transform.Rotate(Vector3.right *  rotationAngleMax);
					break;
				case '/':
					transform.Rotate(Vector3.up * rotationAngleMax);
					break;
				case '?':
					transform.Rotate(Vector3.down *  rotationAngleMax);
					break;


				case '[': //save
					transformStack.Push(new TransformInfo(transform, baseRadius, tipRadius));
					break;
				case ']': //return
					TransformInfo ti = transformStack.Pop();
					transform.position = ti.position;
					transform.rotation = ti.rotation;
					baseRadius = ti.baseRadius;
					tipRadius = ti.tipRadius;
					break;
				default:
					Debug.LogError("Error in string" + currentString[i]);
					break;
			}
		}
	}
}


public class TransformInfo
{
	public Vector3 position;
	public Quaternion rotation;
	public float baseRadius;
	public float tipRadius;

	public TransformInfo(Transform t, float baseRadius, float tipRadius)
	{
		position = t.position;
		rotation = t.rotation;
		this.tipRadius = tipRadius;
		this.baseRadius = baseRadius;
	}
}