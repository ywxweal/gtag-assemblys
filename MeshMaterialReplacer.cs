using System;
using GameObjectScheduling;
using UnityEngine;

// Token: 0x0200026C RID: 620
public class MeshMaterialReplacer : MonoBehaviour
{
	// Token: 0x06000E3C RID: 3644 RVA: 0x00048914 File Offset: 0x00046B14
	private void Start()
	{
		MeshRenderer meshRenderer;
		if (base.TryGetComponent<MeshRenderer>(out meshRenderer))
		{
			base.GetComponent<MeshFilter>().mesh = this.meshMaterialReplacement.mesh;
			meshRenderer.materials = this.meshMaterialReplacement.materials;
			return;
		}
		SkinnedMeshRenderer skinnedMeshRenderer;
		if (base.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
		{
			skinnedMeshRenderer.sharedMesh = this.meshMaterialReplacement.mesh;
			skinnedMeshRenderer.materials = this.meshMaterialReplacement.materials;
		}
	}

	// Token: 0x04001191 RID: 4497
	[SerializeField]
	private MeshMaterialReplacement meshMaterialReplacement;
}
