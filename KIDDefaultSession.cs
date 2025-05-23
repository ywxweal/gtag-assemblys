using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000793 RID: 1939
[Serializable]
public class KIDDefaultSession
{
	// Token: 0x170004D9 RID: 1241
	// (get) Token: 0x06003077 RID: 12407 RVA: 0x000EF48B File Offset: 0x000ED68B
	// (set) Token: 0x06003078 RID: 12408 RVA: 0x000EF493 File Offset: 0x000ED693
	public List<Permission> Permissions { get; set; }

	// Token: 0x170004DA RID: 1242
	// (get) Token: 0x06003079 RID: 12409 RVA: 0x000EF49C File Offset: 0x000ED69C
	// (set) Token: 0x0600307A RID: 12410 RVA: 0x000EF4A4 File Offset: 0x000ED6A4
	public AgeStatusType AgeStatus { get; set; }
}
