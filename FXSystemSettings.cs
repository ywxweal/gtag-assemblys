using System;
using UnityEngine;

// Token: 0x02000931 RID: 2353
[CreateAssetMenu(menuName = "ScriptableObjects/FXSystemSettings", order = 2)]
public class FXSystemSettings : ScriptableObject
{
	// Token: 0x0600392A RID: 14634 RVA: 0x0011302C File Offset: 0x0011122C
	public void Awake()
	{
		int num = ((this.callLimits != null) ? this.callLimits.Length : 0);
		int num2 = ((this.CallLimitsCooldown != null) ? this.CallLimitsCooldown.Length : 0);
		for (int i = 0; i < num; i++)
		{
			FXType fxtype = this.callLimits[i].Key;
			int num3 = (int)fxtype;
			if (this.callSettings[num3] != null)
			{
				Debug.Log("call setting for " + fxtype.ToString() + " already exists, skipping");
			}
			else
			{
				this.callSettings[num3] = this.callLimits[i];
			}
		}
		for (int i = 0; i < num2; i++)
		{
			FXType fxtype = this.CallLimitsCooldown[i].Key;
			int num3 = (int)fxtype;
			if (this.callSettings[num3] != null)
			{
				Debug.Log("call setting for " + fxtype.ToString() + " already exists, skipping");
			}
			else
			{
				this.callSettings[num3] = this.CallLimitsCooldown[i];
			}
		}
		for (int i = 0; i < this.callSettings.Length; i++)
		{
			if (this.callSettings[i] == null)
			{
				this.callSettings[i] = new LimiterType
				{
					CallLimitSettings = new CallLimiter(0, 0f, 0f),
					Key = (FXType)i
				};
			}
		}
	}

	// Token: 0x04003E55 RID: 15957
	[SerializeField]
	private LimiterType[] callLimits;

	// Token: 0x04003E56 RID: 15958
	[SerializeField]
	private CooldownType[] CallLimitsCooldown;

	// Token: 0x04003E57 RID: 15959
	[NonSerialized]
	public bool forLocalRig;

	// Token: 0x04003E58 RID: 15960
	[NonSerialized]
	public CallLimitType<CallLimiter>[] callSettings = new CallLimitType<CallLimiter>[20];
}
