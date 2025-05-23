using System;
using UnityEngine;

// Token: 0x0200049C RID: 1180
public class GorillaSurfaceOverride : MonoBehaviour
{
	// Token: 0x04001FE8 RID: 8168
	[GorillaSoundLookup]
	public int overrideIndex;

	// Token: 0x04001FE9 RID: 8169
	public float extraVelMultiplier = 1f;

	// Token: 0x04001FEA RID: 8170
	public float extraVelMaxMultiplier = 1f;

	// Token: 0x04001FEB RID: 8171
	[HideInInspector]
	[NonSerialized]
	public float slidePercentageOverride = -1f;

	// Token: 0x04001FEC RID: 8172
	public bool sendOnTapEvent;

	// Token: 0x04001FED RID: 8173
	public bool disablePushBackEffect;
}
