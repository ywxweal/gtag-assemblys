using System;
using UnityEngine;

// Token: 0x02000A31 RID: 2609
[Serializable]
public class VoiceLoudnessReactorGameObjectEnableTarget
{
	// Token: 0x04004290 RID: 17040
	public GameObject GameObject;

	// Token: 0x04004291 RID: 17041
	public float Threshold;

	// Token: 0x04004292 RID: 17042
	public bool TurnOnAtThreshhold = true;

	// Token: 0x04004293 RID: 17043
	public bool UseSmoothedLoudness;

	// Token: 0x04004294 RID: 17044
	public float Scale = 1f;
}
