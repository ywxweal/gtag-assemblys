using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000926 RID: 2342
[Serializable]
public class CallLimitersList<Titem, Tenum> where Titem : CallLimiter, new() where Tenum : Enum
{
	// Token: 0x0600390B RID: 14603 RVA: 0x00112D80 File Offset: 0x00110F80
	public bool IsSpamming(Tenum index)
	{
		return this.IsSpamming((int)((object)index));
	}

	// Token: 0x0600390C RID: 14604 RVA: 0x00112D93 File Offset: 0x00110F93
	public bool IsSpamming(int index)
	{
		return !this.m_callLimiters[index].CheckCallTime(Time.unscaledTime);
	}

	// Token: 0x0600390D RID: 14605 RVA: 0x00112DB3 File Offset: 0x00110FB3
	public bool IsSpamming(Tenum index, double serverTime)
	{
		return this.IsSpamming((int)((object)index), serverTime);
	}

	// Token: 0x0600390E RID: 14606 RVA: 0x00112DC7 File Offset: 0x00110FC7
	public bool IsSpamming(int index, double serverTime)
	{
		return !this.m_callLimiters[index].CheckCallServerTime(serverTime);
	}

	// Token: 0x0600390F RID: 14607 RVA: 0x00112DE4 File Offset: 0x00110FE4
	public void Reset()
	{
		Titem[] callLimiters = this.m_callLimiters;
		for (int i = 0; i < callLimiters.Length; i++)
		{
			callLimiters[i].Reset();
		}
	}

	// Token: 0x04003E3B RID: 15931
	[RequiredListLength("GetMaxLength")]
	[SerializeField]
	private Titem[] m_callLimiters;
}
