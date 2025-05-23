using System;
using UnityEngine;

// Token: 0x020004AB RID: 1195
public class AnimatorReset : MonoBehaviour
{
	// Token: 0x06001CD8 RID: 7384 RVA: 0x0008C0B8 File Offset: 0x0008A2B8
	public void Reset()
	{
		if (!this.target)
		{
			return;
		}
		this.target.Rebind();
		this.target.Update(0f);
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x0008C0E3 File Offset: 0x0008A2E3
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.Reset();
		}
	}

	// Token: 0x06001CDA RID: 7386 RVA: 0x0008C0F3 File Offset: 0x0008A2F3
	private void OnDisable()
	{
		if (this.onDisable)
		{
			this.Reset();
		}
	}

	// Token: 0x04002014 RID: 8212
	public Animator target;

	// Token: 0x04002015 RID: 8213
	public bool onEnable;

	// Token: 0x04002016 RID: 8214
	public bool onDisable = true;
}
