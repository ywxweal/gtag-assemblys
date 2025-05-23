using System;

// Token: 0x02000930 RID: 2352
[Serializable]
public class CallLimitType<T> where T : CallLimiter
{
	// Token: 0x06003928 RID: 14632 RVA: 0x00112FFB File Offset: 0x001111FB
	public static implicit operator CallLimitType<CallLimiter>(CallLimitType<T> clt)
	{
		return new CallLimitType<CallLimiter>
		{
			Key = clt.Key,
			UseNetWorkTime = clt.UseNetWorkTime,
			CallLimitSettings = clt.CallLimitSettings
		};
	}

	// Token: 0x04003E52 RID: 15954
	public FXType Key;

	// Token: 0x04003E53 RID: 15955
	public bool UseNetWorkTime;

	// Token: 0x04003E54 RID: 15956
	public T CallLimitSettings;
}
