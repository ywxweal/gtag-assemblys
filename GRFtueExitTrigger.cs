using System;
using UnityEngine;

// Token: 0x020005AE RID: 1454
public class GRFtueExitTrigger : GorillaTriggerBox
{
	// Token: 0x0600238B RID: 9099 RVA: 0x000B2F09 File Offset: 0x000B1109
	public override void OnBoxTriggered()
	{
		this.startTime = Time.time;
		this.ftueObject.InterruptWaitingTimer();
		this.ftueObject.playerLight.GetComponentInChildren<Light>().intensity = 0.25f;
	}

	// Token: 0x0600238C RID: 9100 RVA: 0x000B2F3B File Offset: 0x000B113B
	private void Update()
	{
		if (this.startTime > 0f && Time.time - this.startTime > this.delayTime)
		{
			this.ftueObject.ChangeState(GRFirstTimeUserExperience.TransitionState.Flicker);
			this.startTime = -1f;
		}
	}

	// Token: 0x0400283E RID: 10302
	public GRFirstTimeUserExperience ftueObject;

	// Token: 0x0400283F RID: 10303
	public float delayTime = 5f;

	// Token: 0x04002840 RID: 10304
	private float startTime = -1f;
}
