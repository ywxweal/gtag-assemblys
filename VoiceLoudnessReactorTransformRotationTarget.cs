using System;
using UnityEngine;

// Token: 0x02000A2F RID: 2607
[Serializable]
public class VoiceLoudnessReactorTransformRotationTarget
{
	// Token: 0x1700060C RID: 1548
	// (get) Token: 0x06003DFB RID: 15867 RVA: 0x00126B48 File Offset: 0x00124D48
	// (set) Token: 0x06003DFC RID: 15868 RVA: 0x00126B50 File Offset: 0x00124D50
	public Quaternion Initial
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

	// Token: 0x0400427F RID: 17023
	public Transform transform;

	// Token: 0x04004280 RID: 17024
	private Quaternion initial;

	// Token: 0x04004281 RID: 17025
	public Quaternion Max = Quaternion.identity;

	// Token: 0x04004282 RID: 17026
	public float Scale = 1f;

	// Token: 0x04004283 RID: 17027
	public bool UseSmoothedLoudness;
}
