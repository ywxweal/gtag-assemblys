using System;
using UnityEngine;

// Token: 0x02000925 RID: 2341
[Serializable]
public class CallLimiterWithCooldown : CallLimiter
{
	// Token: 0x06003908 RID: 14600 RVA: 0x00112D2D File Offset: 0x00110F2D
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown)
		: base(historyLength, coolDown, 0.5f)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06003909 RID: 14601 RVA: 0x00112D43 File Offset: 0x00110F43
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown, float latencyMax)
		: base(historyLength, coolDown, latencyMax)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x0600390A RID: 14602 RVA: 0x00112D56 File Offset: 0x00110F56
	public override bool CheckCallTime(float time)
	{
		if (this.blockCall && time < this.blockStartTime + this.spamCoolDown)
		{
			this.blockStartTime = time;
			return false;
		}
		return base.CheckCallTime(time);
	}

	// Token: 0x04003E3A RID: 15930
	[SerializeField]
	private float spamCoolDown;
}
