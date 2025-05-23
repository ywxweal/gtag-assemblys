using System;
using UnityEngine;

// Token: 0x02000A30 RID: 2608
[Serializable]
public class VoiceLoudnessReactorParticleSystemTarget
{
	// Token: 0x1700060D RID: 1549
	// (get) Token: 0x06003DFE RID: 15870 RVA: 0x00126B77 File Offset: 0x00124D77
	// (set) Token: 0x06003DFF RID: 15871 RVA: 0x00126B7F File Offset: 0x00124D7F
	public float InitialSpeed
	{
		get
		{
			return this.initialSpeed;
		}
		set
		{
			this.initialSpeed = value;
		}
	}

	// Token: 0x1700060E RID: 1550
	// (get) Token: 0x06003E00 RID: 15872 RVA: 0x00126B88 File Offset: 0x00124D88
	// (set) Token: 0x06003E01 RID: 15873 RVA: 0x00126B90 File Offset: 0x00124D90
	public float InitialRate
	{
		get
		{
			return this.initialRate;
		}
		set
		{
			this.initialRate = value;
		}
	}

	// Token: 0x1700060F RID: 1551
	// (get) Token: 0x06003E02 RID: 15874 RVA: 0x00126B99 File Offset: 0x00124D99
	// (set) Token: 0x06003E03 RID: 15875 RVA: 0x00126BA1 File Offset: 0x00124DA1
	public float InitialSize
	{
		get
		{
			return this.initialSize;
		}
		set
		{
			this.initialSize = value;
		}
	}

	// Token: 0x04004284 RID: 17028
	public ParticleSystem particleSystem;

	// Token: 0x04004285 RID: 17029
	public bool UseSmoothedLoudness;

	// Token: 0x04004286 RID: 17030
	public float Scale = 1f;

	// Token: 0x04004287 RID: 17031
	private float initialSpeed;

	// Token: 0x04004288 RID: 17032
	private float initialRate;

	// Token: 0x04004289 RID: 17033
	private float initialSize;

	// Token: 0x0400428A RID: 17034
	public AnimationCurve speed;

	// Token: 0x0400428B RID: 17035
	public AnimationCurve rate;

	// Token: 0x0400428C RID: 17036
	public AnimationCurve size;

	// Token: 0x0400428D RID: 17037
	[HideInInspector]
	public ParticleSystem.MainModule Main;

	// Token: 0x0400428E RID: 17038
	[HideInInspector]
	public ParticleSystem.EmissionModule Emission;
}
