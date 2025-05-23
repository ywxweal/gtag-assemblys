using System;
using UnityEngine;

// Token: 0x02000A2D RID: 2605
[Serializable]
public class VoiceLoudnessReactorBlendShapeTarget
{
	// Token: 0x04004276 RID: 17014
	public SkinnedMeshRenderer SkinnedMeshRenderer;

	// Token: 0x04004277 RID: 17015
	public int BlendShapeIndex;

	// Token: 0x04004278 RID: 17016
	public float minValue;

	// Token: 0x04004279 RID: 17017
	public float maxValue = 1f;

	// Token: 0x0400427A RID: 17018
	public bool UseSmoothedLoudness;
}
