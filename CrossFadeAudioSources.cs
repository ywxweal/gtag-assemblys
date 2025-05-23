using System;
using UnityEngine;

// Token: 0x02000538 RID: 1336
public class CrossFadeAudioSources : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x06002075 RID: 8309 RVA: 0x000A3024 File Offset: 0x000A1224
	public void Play()
	{
		if (this.source1)
		{
			this.source1.Play();
		}
		if (this.source2)
		{
			this.source2.Play();
		}
	}

	// Token: 0x06002076 RID: 8310 RVA: 0x000A3056 File Offset: 0x000A1256
	public void Stop()
	{
		if (this.source1)
		{
			this.source1.Stop();
		}
		if (this.source2)
		{
			this.source2.Stop();
		}
	}

	// Token: 0x06002077 RID: 8311 RVA: 0x000A3088 File Offset: 0x000A1288
	private void Update()
	{
		if (!this.source1 || !this.source2)
		{
			return;
		}
		float num = this._curve.Evaluate(this._lerp);
		float num2;
		if (this.tween)
		{
			num2 = MathUtils.Xlerp(this._lastT, num, Time.deltaTime, this.tweenSpeed);
		}
		else
		{
			num2 = (this.lerpByClipLength ? this._curve.Evaluate((float)this.source1.timeSamples / (float)this.source1.clip.samples) : num);
		}
		this._lastT = num2;
		this.source2.volume = num2;
		this.source1.volume = 1f - num2;
	}

	// Token: 0x06002078 RID: 8312 RVA: 0x000A313E File Offset: 0x000A133E
	public float Get()
	{
		return this._lerp;
	}

	// Token: 0x06002079 RID: 8313 RVA: 0x000A3146 File Offset: 0x000A1346
	public void Set(float f)
	{
		this._lerp = Mathf.Clamp01(f);
	}

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x0600207A RID: 8314 RVA: 0x000A3154 File Offset: 0x000A1354
	// (set) Token: 0x0600207B RID: 8315 RVA: 0x000023F4 File Offset: 0x000005F4
	public float Min
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x0600207C RID: 8316 RVA: 0x000A315B File Offset: 0x000A135B
	// (set) Token: 0x0600207D RID: 8317 RVA: 0x000023F4 File Offset: 0x000005F4
	public float Max
	{
		get
		{
			return 1f;
		}
		set
		{
		}
	}

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x0600207E RID: 8318 RVA: 0x000A315B File Offset: 0x000A135B
	public float Range
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x0600207F RID: 8319 RVA: 0x000A3162 File Offset: 0x000A1362
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x04002466 RID: 9318
	[SerializeField]
	private float _lerp;

	// Token: 0x04002467 RID: 9319
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002468 RID: 9320
	[Space]
	[SerializeField]
	private AudioSource source1;

	// Token: 0x04002469 RID: 9321
	[SerializeField]
	private AudioSource source2;

	// Token: 0x0400246A RID: 9322
	[Space]
	public bool lerpByClipLength;

	// Token: 0x0400246B RID: 9323
	public bool tween;

	// Token: 0x0400246C RID: 9324
	public float tweenSpeed = 16f;

	// Token: 0x0400246D RID: 9325
	private float _lastT;
}
