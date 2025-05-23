using System;
using KID.Model;

// Token: 0x020007A6 RID: 1958
[Serializable]
public class GetPlayerDataResponse
{
	// Token: 0x040036F7 RID: 14071
	public SessionStatus? Status;

	// Token: 0x040036F8 RID: 14072
	public Session Session;

	// Token: 0x040036F9 RID: 14073
	public AgeStatusType? AgeStatus;

	// Token: 0x040036FA RID: 14074
	public KIDDefaultSession DefaultSession;
}
