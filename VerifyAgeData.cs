using System;

// Token: 0x020007AF RID: 1967
public class VerifyAgeData
{
	// Token: 0x060030C3 RID: 12483 RVA: 0x000EFC1C File Offset: 0x000EDE1C
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

	// Token: 0x0400371B RID: 14107
	public readonly SessionStatus Status;

	// Token: 0x0400371C RID: 14108
	public readonly TMPSession Session;
}
