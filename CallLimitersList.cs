using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000926 RID: 2342
[Serializable]
public class CallLimitersList<Titem, Tenum> where Titem : CallLimiter, new() where Tenum : Enum
{
	// Token: 0x0600390A RID: 14602 RVA: 0x00112CA8 File Offset: 0x00110EA8
	public bool IsSpamming(Tenum index)
	{
		return this.IsSpamming((int)((object)index));
	}

	// Token: 0x0600390B RID: 14603 RVA: 0x00112CBB File Offset: 0x00110EBB
	public bool IsSpamming(int index)
	{
		return !this.m_callLimiters[index].CheckCallTime(Time.unscaledTime);
	}

	// Token: 0x0600390C RID: 14604 RVA: 0x00112CDB File Offset: 0x00110EDB
	public bool IsSpamming(Tenum index, double serverTime)
	{
		return this.IsSpamming((int)((object)index), serverTime);
	}

	// Token: 0x0600390D RID: 14605 RVA: 0x00112CEF File Offset: 0x00110EEF
	public bool IsSpamming(int index, double serverTime)
	{
		return !this.m_callLimiters[index].CheckCallServerTime(serverTime);
	}

	// Token: 0x0600390E RID: 14606 RVA: 0x00112D0C File Offset: 0x00110F0C
	public void Reset()
	{
		Titem[] callLimiters = this.m_callLimiters;
		for (int i = 0; i < callLimiters.Length; i++)
		{
			callLimiters[i].Reset();
		}
	}

	// Token: 0x04003E3A RID: 15930
	[RequiredListLength("GetMaxLength")]
	[SerializeField]
	private Titem[] m_callLimiters;
}
