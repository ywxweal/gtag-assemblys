using System;
using UnityEngine;

// Token: 0x02000A2E RID: 2606
[Serializable]
public class VoiceLoudnessReactorTransformTarget
{
	// Token: 0x1700060B RID: 1547
	// (get) Token: 0x06003DF9 RID: 15865 RVA: 0x00126BF1 File Offset: 0x00124DF1
	// (set) Token: 0x06003DFA RID: 15866 RVA: 0x00126BF9 File Offset: 0x00124DF9
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

	// Token: 0x0400427B RID: 17019
	public Transform transform;

	// Token: 0x0400427C RID: 17020
	private Vector3 initial;

	// Token: 0x0400427D RID: 17021
	public Vector3 Max = Vector3.one;

	// Token: 0x0400427E RID: 17022
	public float Scale = 1f;

	// Token: 0x0400427F RID: 17023
	public bool UseSmoothedLoudness;
}
