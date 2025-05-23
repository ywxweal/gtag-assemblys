using System;
using UnityEngine;

// Token: 0x020005FF RID: 1535
public class GorillaEyeExpressions : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060025CE RID: 9678 RVA: 0x000BC0C1 File Offset: 0x000BA2C1
	private void Awake()
	{
		this.loudness = base.GetComponent<GorillaSpeakerLoudness>();
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x000BC0CF File Offset: 0x000BA2CF
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.timeLastUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x00010F34 File Offset: 0x0000F134
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060025D1 RID: 9681 RVA: 0x000BC0EE File Offset: 0x000BA2EE
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.timeLastUpdated;
		this.timeLastUpdated = Time.time;
		this.CheckEyeEffects();
		this.UpdateEyeExpression();
	}

	// Token: 0x060025D2 RID: 9682 RVA: 0x000BC11C File Offset: 0x000BA31C
	private void CheckEyeEffects()
	{
		if (this.loudness == null)
		{
			this.loudness = base.GetComponent<GorillaSpeakerLoudness>();
		}
		if (this.loudness.IsSpeaking && this.loudness.Loudness > this.screamVolume)
		{
			this.IsEyeExpressionOverriden = true;
			this.overrideDuration = this.screamDuration;
			this.overrideUV = this.ScreamUV;
			return;
		}
		if (this.IsEyeExpressionOverriden)
		{
			this.overrideDuration -= this.deltaTime;
			if (this.overrideDuration < 0f)
			{
				this.IsEyeExpressionOverriden = false;
			}
		}
	}

	// Token: 0x060025D3 RID: 9683 RVA: 0x000BC1B4 File Offset: 0x000BA3B4
	private void UpdateEyeExpression()
	{
		Material material = this.targetFace.GetComponent<Renderer>().material;
		material.SetFloat(this._EyeOverrideUV, this.IsEyeExpressionOverriden ? 1f : 0f);
		material.SetVector(this._EyeOverrideUVTransform, new Vector4(1f, 1f, this.overrideUV.x, this.overrideUV.y));
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04002A6F RID: 10863
	public GameObject targetFace;

	// Token: 0x04002A70 RID: 10864
	[Space]
	[SerializeField]
	private float screamVolume = 0.2f;

	// Token: 0x04002A71 RID: 10865
	[SerializeField]
	private float screamDuration = 0.5f;

	// Token: 0x04002A72 RID: 10866
	[SerializeField]
	private Vector2 ScreamUV = new Vector2(0.8f, 0f);

	// Token: 0x04002A73 RID: 10867
	private GorillaSpeakerLoudness loudness;

	// Token: 0x04002A74 RID: 10868
	private bool IsEyeExpressionOverriden;

	// Token: 0x04002A75 RID: 10869
	private float overrideDuration;

	// Token: 0x04002A76 RID: 10870
	private Vector2 overrideUV;

	// Token: 0x04002A77 RID: 10871
	private float timeLastUpdated;

	// Token: 0x04002A78 RID: 10872
	private float deltaTime;

	// Token: 0x04002A79 RID: 10873
	private ShaderHashId _EyeOverrideUV = "_EyeOverrideUV";

	// Token: 0x04002A7A RID: 10874
	private ShaderHashId _EyeOverrideUVTransform = "_EyeOverrideUVTransform";
}
