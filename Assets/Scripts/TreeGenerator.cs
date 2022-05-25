using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class TreeGenerator : MonoBehaviour
{
	[SerializeField] private TreeDataSO treeDataSO;
	private BranchGenerator trunkBranchGenerator;
	private MeshRenderer meshRenderer;
	private Mesh trunkMesh;
	public void SetTreeData(TreeDataSO td) => treeDataSO = td;

	private void Awake()
	{
		trunkMesh = new Mesh
		{
			name = "Tree"
		};
		GetComponent<MeshFilter>().sharedMesh = trunkMesh;
		meshRenderer = GetComponent<MeshRenderer>();
		trunkBranchGenerator = new BranchGenerator();
	}


	private void Update()
	{
		UpdateBranch();
	}

	public void SaveMesh()
	{
		AssetDatabase.CreateAsset(trunkMesh, "Assets/Resources/GeneratedTrees/treeMesh.asset");
		AssetDatabase.SaveAssets();
	}

	public void UpdateBranch()
	{
		trunkBranchGenerator ??= new BranchGenerator();
		trunkMesh = trunkBranchGenerator.GenerateBranchMesh(trunkMesh, treeDataSO);
		meshRenderer.sharedMaterial = treeDataSO.branchMaterial;
		trunkBranchGenerator.debugEnabled = treeDataSO.debugEnabled;
	}

	public void OnDrawGizmos()
	{
		if (!treeDataSO.debugEnabled) return;
		foreach (var v in trunkBranchGenerator.vertices)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(v, 0.02f);
		}
	}
}