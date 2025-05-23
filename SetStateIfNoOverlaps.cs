using System;
using UnityEngine;

// Token: 0x020004AA RID: 1194
public class SetStateIfNoOverlaps : SetStateConditional
{
	// Token: 0x06001CD5 RID: 7381 RVA: 0x0008C05F File Offset: 0x0008A25F
	protected override void Setup(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this._volume = animator.GetComponent<VolumeCast>();
	}

	// Token: 0x06001CD6 RID: 7382 RVA: 0x0008C06D File Offset: 0x0008A26D
	protected override bool CanSetState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		bool flag = this._volume.CheckOverlaps();
		if (flag)
		{
			this._sinceEnter = 0f;
		}
		return !flag;
	}

	// Token: 0x04002013 RID: 8211
	public VolumeCast _volume;
}
