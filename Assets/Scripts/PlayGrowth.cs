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
	/// <summary>
	///   <para>Trigers new growth from iteration 0 - Loops until rules max iterations</para>
	/// </summary>
	public void Play()
	{
		if (lSystem == null || lSystemRule == null)
		{
			Debug.LogWarning("Missing L System reqs");
			return;
		}
		lSystemRule = lSystem.GetLSystemRule();

		currentIteration = 1;
		StartCoroutine(Delay());
	}
	/// <summary>
	///   <para>Provides delay between iterations</para>
	/// </summary>
	private IEnumerator Delay()
	{
		if (currentIteration != 1)
		{
			yield return new WaitForSeconds(delayInSeconds);
		}
		lSystem.SetIterations(currentIteration);
		currentIteration++;
		lSystem.Setup();
		if (currentIteration <= lSystemRule.iterations)
		{
			StartCoroutine(Delay());
		}
	}
}