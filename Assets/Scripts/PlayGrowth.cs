using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LSystem))]
public class PlayGrowth : MonoBehaviour
{
	private LSystem lSystem;
	private LSystemRule lSystemRule;
	[SerializeField] private float delayInSeconds = 2f;
	private int currentIteration;

	private void Start()
	{
		lSystem = GetComponent<LSystem>();
		lSystemRule = lSystem.GetLSystemRule();
	}

	public void Play()
	{
		if (lSystem == null || lSystemRule == null)
		{
			Debug.LogWarning("Missing L System reqs");
			return;
		}

		currentIteration = 1;
		StartCoroutine(Delay());
	}

	private IEnumerator Delay()
	{
		if (currentIteration != 1)
		{
			yield return new WaitForSeconds(delayInSeconds);
		}

		lSystem.SetIterations(currentIteration);
		currentIteration++;

		lSystem.Setup();
		Debug.Log("Delay iteration " + currentIteration);

		Debug.Log("Delay completed for iteration " + currentIteration);

		if (currentIteration < lSystemRule.iterations)
		{
			StartCoroutine(Delay());
		}
	}
}