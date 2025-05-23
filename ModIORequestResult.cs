using System;

// Token: 0x02000714 RID: 1812
public struct ModIORequestResult
{
	// Token: 0x06002D35 RID: 11573 RVA: 0x000DF96C File Offset: 0x000DDB6C
	public static ModIORequestResult CreateFailureResult(string inMessage)
	{
		ModIORequestResult modIORequestResult;
		modIORequestResult.success = false;
		modIORequestResult.message = inMessage;
		return modIORequestResult;
	}

	// Token: 0x06002D36 RID: 11574 RVA: 0x000DF98C File Offset: 0x000DDB8C
	public static ModIORequestResult CreateSuccessResult()
	{
		ModIORequestResult modIORequestResult;
		modIORequestResult.success = true;
		modIORequestResult.message = "";
		return modIORequestResult;
	}

	// Token: 0x04003376 RID: 13174
	public bool success;

	// Token: 0x04003377 RID: 13175
	public string message;
}
