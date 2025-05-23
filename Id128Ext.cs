using System;
using UnityEngine;

// Token: 0x02000765 RID: 1893
public static class Id128Ext
{
	// Token: 0x06002F38 RID: 12088 RVA: 0x000EBD2C File Offset: 0x000E9F2C
	public static Id128 ToId128(this Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x06002F39 RID: 12089 RVA: 0x000EBD24 File Offset: 0x000E9F24
	public static Id128 ToId128(this Guid g)
	{
		return new Id128(g);
	}
}
