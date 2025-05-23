using System;

// Token: 0x020007AE RID: 1966
public class UpgradeSessionData
{
	// Token: 0x060030C2 RID: 12482 RVA: 0x000EFBF0 File Offset: 0x000EDDF0
	public UpgradeSessionData(UpgradeSessionResponse response)
	{
		this.status = response.status;
		this.session = new TMPSession(response.session, null, this.status);
	}

	// Token: 0x04003719 RID: 14105
	public readonly SessionStatus status;

	// Token: 0x0400371A RID: 14106
	public readonly TMPSession session;
}
