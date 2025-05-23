using System;

// Token: 0x02000930 RID: 2352
[Serializable]
public class CallLimitType<T> where T : CallLimiter
{
	// Token: 0x06003927 RID: 14631 RVA: 0x00112F23 File Offset: 0x00111123
	public static implicit operator CallLimitType<CallLimiter>(CallLimitType<T> clt)
	{
		return new CallLimitType<CallLimiter>
		{
			Key = clt.Key,
			UseNetWorkTime = clt.UseNetWorkTime,
			CallLimitSettings = clt.CallLimitSettings
		};
	}

	// Token: 0x04003E51 RID: 15953
	public FXType Key;

	// Token: 0x04003E52 RID: 15954
	public bool UseNetWorkTime;

	// Token: 0x04003E53 RID: 15955
	public T CallLimitSettings;
}
