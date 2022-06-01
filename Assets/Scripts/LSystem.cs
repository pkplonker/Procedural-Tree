using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class LSystem : MonoBehaviour
{
	[SerializeField] private float lineLength = 10f;
	[SerializeField] private float rotationAngleMin = 0f;
	[SerializeField] private float rotationAngleMax = 30f;
	[SerializeField] private float upRotationAngleMin = -50f;
	[SerializeField] private float upRotationAngleMax = 50f;
	[SerializeField] private int iterations = 5;

	[SerializeField] private GameObject branchPrefab;
	private const string axiom = "X";
	private Stack<TransformInfo> transformStack;
	private Dictionary<char, string> rules;
	private string currentString = "";

	private void Start()
	{
		transformStack = new Stack<TransformInfo>();
		rules = new Dictionary<char, string>
		{
			{'X', "[F-[[X]+X]+F[+FX]-X]"},
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
		}

		foreach (var c in currentString)
		{
			switch (c)
			{
				case 'F': //straight line
					Vector3 initialPosition = transform.position;
					transform.Translate(Vector3.up * lineLength);
					GameObject segment = Instantiate(branchPrefab);
					var lr = segment.GetComponent<LineRenderer>();
					lr.SetPosition(0, initialPosition);
					lr.SetPosition(1, transform.position);

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