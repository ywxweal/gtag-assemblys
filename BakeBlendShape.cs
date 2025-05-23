using System;
using UnityEngine;

// Token: 0x020000DB RID: 219
public class BakeBlendShape : MonoBehaviour
{
	// Token: 0x06000581 RID: 1409 RVA: 0x0001FF70 File Offset: 0x0001E170
	private void Update()
	{
		Mesh mesh = new Mesh();
		MeshCollider component = base.GetComponent<MeshCollider>();
		base.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
		component.sharedMesh = null;
		component.sharedMesh = mesh;
	}
}
