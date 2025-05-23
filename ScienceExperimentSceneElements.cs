using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020006AA RID: 1706
public class ScienceExperimentSceneElements : MonoBehaviour
{
	// Token: 0x06002AA5 RID: 10917 RVA: 0x000D1CEB File Offset: 0x000CFEEB
	private void Awake()
	{
		ScienceExperimentManager.instance.InitElements(this);
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x000D1CFA File Offset: 0x000CFEFA
	private void OnDestroy()
	{
		ScienceExperimentManager.instance.DeInitElements();
	}

	// Token: 0x04002F8C RID: 12172
	public List<ScienceExperimentSceneElements.DisableByLiquidData> disableByLiquidList = new List<ScienceExperimentSceneElements.DisableByLiquidData>();

	// Token: 0x04002F8D RID: 12173
	public ParticleSystem sodaFizzParticles;

	// Token: 0x04002F8E RID: 12174
	public ParticleSystem sodaEruptionParticles;

	// Token: 0x020006AB RID: 1707
	[Serializable]
	public struct DisableByLiquidData
	{
		// Token: 0x04002F8F RID: 12175
		public Transform target;

		// Token: 0x04002F90 RID: 12176
		public float heightOffset;
	}
}
