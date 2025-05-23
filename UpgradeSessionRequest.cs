using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x020007A9 RID: 1961
[Serializable]
public class UpgradeSessionRequest : KIDRequestData
{
	// Token: 0x04003705 RID: 14085
	public List<RequestedPermission> Permissions;
}
