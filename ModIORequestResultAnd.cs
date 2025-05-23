using System;

// Token: 0x02000715 RID: 1813
public struct ModIORequestResultAnd<T>
{
	// Token: 0x06002D37 RID: 11575 RVA: 0x000DF9B0 File Offset: 0x000DDBB0
	public static ModIORequestResultAnd<T> CreateFailureResult(string inMessage)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateFailureResult(inMessage)
		};
	}

	// Token: 0x06002D38 RID: 11576 RVA: 0x000DF9D4 File Offset: 0x000DDBD4
	public static ModIORequestResultAnd<T> CreateSuccessResult(T payload)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateSuccessResult(),
			data = payload
		};
	}

	// Token: 0x04003378 RID: 13176
	public ModIORequestResult result;

	// Token: 0x04003379 RID: 13177
	public T data;
}
