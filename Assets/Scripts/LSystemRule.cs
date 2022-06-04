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
	public Dictionary<char, string> rules;
	[Range(1, 7)]public int iterations = 5;

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