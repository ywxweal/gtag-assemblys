using System;
using UnityEngine;

// Token: 0x02000A2F RID: 2607
[Serializable]
public class VoiceLoudnessReactorTransformRotationTarget
{
	// Token: 0x1700060C RID: 1548
	// (get) Token: 0x06003DFC RID: 15868 RVA: 0x00126C20 File Offset: 0x00124E20
	// (set) Token: 0x06003DFD RID: 15869 RVA: 0x00126C28 File Offset: 0x00124E28
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

	// Token: 0x04004280 RID: 17024
	public Transform transform;

	// Token: 0x04004281 RID: 17025
	private Quaternion initial;

	// Token: 0x04004282 RID: 17026
	public Quaternion Max = Quaternion.identity;

	// Token: 0x04004283 RID: 17027
	public float Scale = 1f;

	// Token: 0x04004284 RID: 17028
	public bool UseSmoothedLoudness;
}
