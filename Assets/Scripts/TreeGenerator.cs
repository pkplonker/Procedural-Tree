
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TreeGenerator : MonoBehaviour
{
	[SerializeField] private TreeDataSO treeDataSO;
	private BranchGenerator trunkBranchGenerator;

	private Mesh trunkMesh;

	private void Awake()
	{
		trunkMesh = new Mesh
		{
			name = "Tree"
		};
		GetComponent<MeshFilter>().sharedMesh = trunkMesh;
		trunkBranchGenerator = new BranchGenerator();
	}

	private void Update()
	{
		trunkMesh = trunkBranchGenerator.GenerateBranchMesh(trunkMesh, treeDataSO);

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