using System;

// Token: 0x020007AF RID: 1967
public class VerifyAgeData
{
	// Token: 0x060030C2 RID: 12482 RVA: 0x000EFB78 File Offset: 0x000EDD78
	public VerifyAgeData(VerifyAgeResponse response)
	{
		if (response == null)
		{
			return;
		}
		this.Status = response.Status;
		if (response.Session == null && response.DefaultSession == null)
		{
			return;
		}
		this.Session = new TMPSession(response.Session, response.DefaultSession, this.Status);
	}

	// Token: 0x0400371A RID: 14106
	public readonly SessionStatus Status;

	// Token: 0x0400371B RID: 14107
	public readonly TMPSession Session;
}
