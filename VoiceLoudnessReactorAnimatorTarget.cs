using System;
using UnityEngine;

// Token: 0x02000A32 RID: 2610
[Serializable]
public class VoiceLoudnessReactorAnimatorTarget
{
	// Token: 0x04004294 RID: 17044
	public Animator animator;

	// Token: 0x04004295 RID: 17045
	public bool useSmoothedLoudness;

	// Token: 0x04004296 RID: 17046
	public float animatorSpeedToLoudness = 1f;
}
