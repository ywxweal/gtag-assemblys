using System;

// Token: 0x02000715 RID: 1813
public struct ModIORequestResultAnd<T>
{
	// Token: 0x06002D36 RID: 11574 RVA: 0x000DF90C File Offset: 0x000DDB0C
	public static ModIORequestResultAnd<T> CreateFailureResult(string inMessage)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateFailureResult(inMessage)
		};
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x000DF930 File Offset: 0x000DDB30
	public static ModIORequestResultAnd<T> CreateSuccessResult(T payload)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateSuccessResult(),
			data = payload
		};
	}

	// Token: 0x04003376 RID: 13174
	public ModIORequestResult result;

	// Token: 0x04003377 RID: 13175
	public T data;
}
