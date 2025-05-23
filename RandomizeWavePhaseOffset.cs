using System;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class RandomizeWavePhaseOffset : MonoBehaviour
{
	// Token: 0x060006D0 RID: 1744 RVA: 0x000269E8 File Offset: 0x00024BE8
	private void Start()
	{
		Material material = base.GetComponent<MeshRenderer>().material;
		UberShader.VertexWavePhaseOffset.SetValue<float>(material, Random.Range(this.minPhaseOffset, this.maxPhaseOffset));
	}

	// Token: 0x0400081B RID: 2075
	[SerializeField]
	private float minPhaseOffset;

	// Token: 0x0400081C RID: 2076
	[SerializeField]
	private float maxPhaseOffset;
}
