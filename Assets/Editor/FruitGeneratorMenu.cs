using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FruitGeneratorEditor : MonoBehaviour
{
	[MenuItem("Stuart Heath/Generate Fruit")]
	static void SaveGame()
	{
		FruitMeshGenerator.Generate(Vector3.zero, 12,  1f, 1f, false,null);
	
	}
}
