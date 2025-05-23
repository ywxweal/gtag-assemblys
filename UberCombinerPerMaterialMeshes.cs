using System;
using UnityEngine;

// Token: 0x02000A1C RID: 2588
public class UberCombinerPerMaterialMeshes : MonoBehaviour
{
	// Token: 0x040041B8 RID: 16824
	public GameObject rootObject;

	// Token: 0x040041B9 RID: 16825
	public bool deleteSelfOnPrefabBake;

	// Token: 0x040041BA RID: 16826
	[Space]
	public GameObject[] objects = new GameObject[0];

	// Token: 0x040041BB RID: 16827
	public MeshRenderer[] renderers = new MeshRenderer[0];

	// Token: 0x040041BC RID: 16828
	public MeshFilter[] filters = new MeshFilter[0];

	// Token: 0x040041BD RID: 16829
	public Material[] materials = new Material[0];
}
