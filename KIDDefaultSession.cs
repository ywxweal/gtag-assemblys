using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000793 RID: 1939
[Serializable]
public class KIDDefaultSession
{
	// Token: 0x170004D9 RID: 1241
	// (get) Token: 0x06003078 RID: 12408 RVA: 0x000EF52F File Offset: 0x000ED72F
	// (set) Token: 0x06003079 RID: 12409 RVA: 0x000EF537 File Offset: 0x000ED737
	public List<Permission> Permissions { get; set; }

	// Token: 0x170004DA RID: 1242
	// (get) Token: 0x0600307A RID: 12410 RVA: 0x000EF540 File Offset: 0x000ED740
	// (set) Token: 0x0600307B RID: 12411 RVA: 0x000EF548 File Offset: 0x000ED748
	public AgeStatusType AgeStatus { get; set; }
}
