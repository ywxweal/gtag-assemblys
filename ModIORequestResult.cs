using System;

// Token: 0x02000714 RID: 1812
public struct ModIORequestResult
{
	// Token: 0x06002D34 RID: 11572 RVA: 0x000DF8C8 File Offset: 0x000DDAC8
	public static ModIORequestResult CreateFailureResult(string inMessage)
	{
		ModIORequestResult modIORequestResult;
		modIORequestResult.success = false;
		modIORequestResult.message = inMessage;
		return modIORequestResult;
	}

	// Token: 0x06002D35 RID: 11573 RVA: 0x000DF8E8 File Offset: 0x000DDAE8
	public static ModIORequestResult CreateSuccessResult()
	{
		ModIORequestResult modIORequestResult;
		modIORequestResult.success = true;
		modIORequestResult.message = "";
		return modIORequestResult;
	}

	// Token: 0x04003374 RID: 13172
	public bool success;

	// Token: 0x04003375 RID: 13173
	public string message;
}
