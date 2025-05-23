using System;

// Token: 0x020007E6 RID: 2022
public class KIDPlayerPrefs
{
	// Token: 0x04003846 RID: 14406
	public const string SESSION_ID_PREFIX_PLAYER_PREF = "kIDSessionID-";

	// Token: 0x04003847 RID: 14407
	public const string SESSION_ETAG_PLAYER_PREF = "kIDSessionETAG-";

	// Token: 0x04003848 RID: 14408
	public const string SESSION_CHANGED_PLAYER_PREF = "kIDSessionUpdated-";

	// Token: 0x04003849 RID: 14409
	private const string KID_PERMISSIONS_CSV = "kid-permission-csv";

	// Token: 0x0400384A RID: 14410
	private const string KID_DEFAULT_PERMISSIONS_CSV = "kid-default-permission-csv";

	// Token: 0x0400384B RID: 14411
	private const string KID_PERMISSIONS_ENABLED_KEY = "-enabled";

	// Token: 0x0400384C RID: 14412
	private const string KID_PERMISSIONS_MANAGED_BY_KEY = "-managed-by";

	// Token: 0x0400384D RID: 14413
	private const string KID_EMAIL_KEY = "k-id_EmailAddress";
}
