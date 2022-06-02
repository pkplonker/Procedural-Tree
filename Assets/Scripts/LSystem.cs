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
	[Range(2, 100)] [SerializeField] int numberOfSlices = 10;
	[Range(1, 100)] [SerializeField] int quality = 10;

	[SerializeField] private GameObject branchPrefab;
	private const string axiom = "X";
	private Stack<TransformInfo> transformStack;
	private Dictionary<char, string> rules;
	private string currentString = "";
	private List<Mesh> meshes = new List<Mesh>();

	private void Start()
	{
		branchLength = (numberOfSlices-1) * sliceThickness;
		transformStack = new Stack<TransformInfo>();
		rules = new Dictionary<char, string>
		{
			{'X', "[F-[[X]+X]+F[+FX]-X]"},
			//{'X', "[F-[+X]F[-X]+X]"},
			//{'F', "F[+F]F[-F][F]"},


			{'F', "FF"}
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
			Debug.Log("Iteration " + i + ". String = " + currentString);
		}


		foreach (var c in currentString)
		{
			switch (c)
			{
				case 'F': //straight line
					Transform initialTransform = transform;
					transform.Translate(Vector3.up * branchLength);
					Quaternion rot = transform.rotation;
					Debug.DrawLine(initialTransform.position, transform.position, Color.red);
					GameObject segment = Instantiate(branchPrefab);
					var branch = segment.GetComponent<BranchGeneratorLSystem>();
					segment.transform.position = initialTransform.position;
					segment.transform.rotation = initialTransform.rotation;

					meshes.Add(branch.GenerateBranchMesh(radius,
						sliceThickness, 3 + quality, numberOfSlices, true, rot));

					break;
				case 'X': //nothing
					break;
				case '+': //rotate +
					transform.Rotate(Vector3.back * Random.Range(rotationAngleMin, rotationAngleMax));
					transform.Rotate(Vector3.up * Random.Range(upRotationAngleMin, upRotationAngleMax));

					break;
				case '-': //rotate anti-clockwise
					transform.Rotate(Vector3.forward * Random.Range(rotationAngleMin, rotationAngleMax));
					transform.Rotate(Vector3.up * Random.Range(upRotationAngleMin, upRotationAngleMax));

					break;
				case '[': //save
					transformStack.Push(new TransformInfo(transform));
					break;
				case ']': //return
					TransformInfo ti = transformStack.Pop();
					transform.position = ti.position;
					transform.rotation = ti.rotation;
					break;
				default:
					Debug.LogError("Error in string");
					break;
			}
		}
	}
}


public class TransformInfo
{
	public Vector3 position;
	public Quaternion rotation;

	public TransformInfo(Transform t)
	{
		position = t.position;
		rotation = t.rotation;
	}
}