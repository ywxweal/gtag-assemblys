using System;
using UnityEngine;

// Token: 0x020004A9 RID: 1193
public class SetStateConditional : StateMachineBehaviour
{
	// Token: 0x06001CCF RID: 7375 RVA: 0x0008BFDF File Offset: 0x0008A1DF
	private void OnValidate()
	{
		this._setToID = this.setToState;
	}

	// Token: 0x06001CD0 RID: 7376 RVA: 0x0008BFF2 File Offset: 0x0008A1F2
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!this._didSetup)
		{
			this.parentAnimator = animator;
			this.Setup(animator, stateInfo, layerIndex);
			this._didSetup = true;
		}
		this._sinceEnter = TimeSince.Now();
	}

	// Token: 0x06001CD1 RID: 7377 RVA: 0x0008C020 File Offset: 0x0008A220
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.delay > 0f && !this._sinceEnter.HasElapsed(this.delay, true))
		{
			return;
		}
		if (!this.CanSetState(animator, stateInfo, layerIndex))
		{
			return;
		}
		animator.Play(this._setToID);
	}

	// Token: 0x06001CD2 RID: 7378 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void Setup(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06001CD3 RID: 7379 RVA: 0x00047642 File Offset: 0x00045842
	protected virtual bool CanSetState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		return true;
	}

	// Token: 0x0400200D RID: 8205
	public Animator parentAnimator;

	// Token: 0x0400200E RID: 8206
	public string setToState;

	// Token: 0x0400200F RID: 8207
	[SerializeField]
	private AnimStateHash _setToID;

	// Token: 0x04002010 RID: 8208
	public float delay = 1f;

	// Token: 0x04002011 RID: 8209
	protected TimeSince _sinceEnter;

	// Token: 0x04002012 RID: 8210
	[NonSerialized]
	private bool _didSetup;
}
