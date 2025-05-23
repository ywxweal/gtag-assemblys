using System;

// Token: 0x020007AE RID: 1966
public class UpgradeSessionData
{
	// Token: 0x060030C1 RID: 12481 RVA: 0x000EFB4C File Offset: 0x000EDD4C
	public UpgradeSessionData(UpgradeSessionResponse response)
	{
		this.status = response.status;
		this.session = new TMPSession(response.session, null, this.status);
	}

	// Token: 0x04003718 RID: 14104
	public readonly SessionStatus status;

	// Token: 0x04003719 RID: 14105
	public readonly TMPSession session;
}
