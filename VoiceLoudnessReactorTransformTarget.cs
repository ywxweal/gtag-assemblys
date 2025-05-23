using System;
using UnityEngine;

// Token: 0x02000A2E RID: 2606
[Serializable]
public class VoiceLoudnessReactorTransformTarget
{
	// Token: 0x1700060B RID: 1547
	// (get) Token: 0x06003DF8 RID: 15864 RVA: 0x00126B19 File Offset: 0x00124D19
	// (set) Token: 0x06003DF9 RID: 15865 RVA: 0x00126B21 File Offset: 0x00124D21
	public Vector3 Initial
	{
		get
		{
			return this.initial;
		}
		set
		{
			this.initial = value;
		}
	}

	// Token: 0x0400427A RID: 17018
	public Transform transform;

	// Token: 0x0400427B RID: 17019
	private Vector3 initial;

	// Token: 0x0400427C RID: 17020
	public Vector3 Max = Vector3.one;

	// Token: 0x0400427D RID: 17021
	public float Scale = 1f;

	// Token: 0x0400427E RID: 17022
	public bool UseSmoothedLoudness;
}
