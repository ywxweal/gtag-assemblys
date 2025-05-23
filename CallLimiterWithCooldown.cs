using System;
using UnityEngine;

// Token: 0x02000925 RID: 2341
[Serializable]
public class CallLimiterWithCooldown : CallLimiter
{
	// Token: 0x06003907 RID: 14599 RVA: 0x00112C55 File Offset: 0x00110E55
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown)
		: base(historyLength, coolDown, 0.5f)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06003908 RID: 14600 RVA: 0x00112C6B File Offset: 0x00110E6B
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown, float latencyMax)
		: base(historyLength, coolDown, latencyMax)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06003909 RID: 14601 RVA: 0x00112C7E File Offset: 0x00110E7E
	public override bool CheckCallTime(float time)
	{
		if (this.blockCall && time < this.blockStartTime + this.spamCoolDown)
		{
			this.blockStartTime = time;
			return false;
		}
		return base.CheckCallTime(time);
	}

	// Token: 0x04003E39 RID: 15929
	[SerializeField]
	private float spamCoolDown;
}
