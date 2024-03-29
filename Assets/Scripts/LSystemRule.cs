using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LSystemRule", menuName = "LSystemRule")]
public class LSystemRule : ScriptableObject
{
	public string axiom = "X";
	[SerializeField] private List<char> ruleKeys;
	[SerializeField] private List<string> ruleList;
	[Range(0.01f, 1f)] public float radius = 0.1f;
	[Range(0.01f, .5f)] public float sliceThickness = .1f;
	[Range(5, 100)] public int quality = 10;
	[Range(0.1f, 10f)] public float radiusReductionFactor = 5f;
	public Dictionary<char, string> rules;
	[Range(1, 20)] public int iterations = 5;
	public float angle = 22.5f;

	public void UpdateRules()
	{
		if (rules == null)
		{
			rules = new Dictionary<char, string>();
		}

		if (ruleKeys.Count != ruleList.Count)
		{
			Debug.LogWarning("Mismatch in rule length");
			return;
		}

		rules.Clear();

		for (int i = 0; i < ruleKeys.Count; i++)
		{
			rules.Add(ruleKeys[i], ruleList[i]);
		}
	}
}