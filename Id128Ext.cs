using System;
using UnityEngine;

// Token: 0x02000765 RID: 1893
public static class Id128Ext
{
	// Token: 0x06002F39 RID: 12089 RVA: 0x000EBDD0 File Offset: 0x000E9FD0
	public static Id128 ToId128(this Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x000EBDC8 File Offset: 0x000E9FC8
	public static Id128 ToId128(this Guid g)
	{
		return new Id128(g);
	}
}
