using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x020007A9 RID: 1961
[Serializable]
public class UpgradeSessionRequest : KIDRequestData
{
	// Token: 0x04003704 RID: 14084
	public List<RequestedPermission> Permissions;
}
