using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020006B2 RID: 1714
public class SpoonClacker : MonoBehaviour
{
	// Token: 0x06002ACF RID: 10959 RVA: 0x000D241E File Offset: 0x000D061E
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06002AD0 RID: 10960 RVA: 0x000D2428 File Offset: 0x000D0628
	private void Setup()
	{
		JointLimits limits = this.hingeJoint.limits;
		this.hingeMin = limits.min;
		this.hingeMax = limits.max;
	}

	// Token: 0x06002AD1 RID: 10961 RVA: 0x000D245C File Offset: 0x000D065C
	private void Update()
	{
		if (!this.transferObject)
		{
			return;
		}
		TransferrableObject.PositionState currentState = this.transferObject.currentState;
		if (currentState != TransferrableObject.PositionState.InLeftHand && currentState != TransferrableObject.PositionState.InRightHand)
		{
			return;
		}
		float num = MathUtils.Linear(this.hingeJoint.angle, this.hingeMin, this.hingeMax, 0f, 1f);
		float num2 = (this.invertOut ? (1f - num) : num) * 100f;
		this.skinnedMesh.SetBlendShapeWeight(this.targetBlendShape, num2);
		if (!this._lockMin && num <= this.minThreshold)
		{
			this.OnHitMin.Invoke();
			this._lockMin = true;
		}
		else if (!this._lockMax && num >= 1f - this.maxThreshold)
		{
			this.OnHitMax.Invoke();
			this._lockMax = true;
			if (this._sincelastHit.HasElapsed(this.multiHitCutoff, true))
			{
				this.soundsSingle.Play();
			}
			else
			{
				this.soundsMulti.Play();
			}
		}
		if (this._lockMin && num > this.minThreshold * this.hysterisisFactor)
		{
			this._lockMin = false;
		}
		if (this._lockMax && num < 1f - this.maxThreshold * this.hysterisisFactor)
		{
			this._lockMax = false;
		}
	}

	// Token: 0x04002FB2 RID: 12210
	public TransferrableObject transferObject;

	// Token: 0x04002FB3 RID: 12211
	public SkinnedMeshRenderer skinnedMesh;

	// Token: 0x04002FB4 RID: 12212
	public HingeJoint hingeJoint;

	// Token: 0x04002FB5 RID: 12213
	public int targetBlendShape;

	// Token: 0x04002FB6 RID: 12214
	public float hingeMin;

	// Token: 0x04002FB7 RID: 12215
	public float hingeMax;

	// Token: 0x04002FB8 RID: 12216
	public bool invertOut;

	// Token: 0x04002FB9 RID: 12217
	public float minThreshold = 0.01f;

	// Token: 0x04002FBA RID: 12218
	public float maxThreshold = 0.01f;

	// Token: 0x04002FBB RID: 12219
	public float hysterisisFactor = 4f;

	// Token: 0x04002FBC RID: 12220
	public UnityEvent OnHitMin;

	// Token: 0x04002FBD RID: 12221
	public UnityEvent OnHitMax;

	// Token: 0x04002FBE RID: 12222
	private bool _lockMin;

	// Token: 0x04002FBF RID: 12223
	private bool _lockMax;

	// Token: 0x04002FC0 RID: 12224
	public SoundBankPlayer soundsSingle;

	// Token: 0x04002FC1 RID: 12225
	public SoundBankPlayer soundsMulti;

	// Token: 0x04002FC2 RID: 12226
	private TimeSince _sincelastHit;

	// Token: 0x04002FC3 RID: 12227
	[FormerlySerializedAs("multiHitInterval")]
	public float multiHitCutoff = 0.1f;
}
