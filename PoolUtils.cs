using System;
using UnityEngine;

// Token: 0x020009A5 RID: 2469
public static class PoolUtils
{
	// Token: 0x06003B34 RID: 15156 RVA: 0x0011B07C File Offset: 0x0011927C
	public static int GameObjHashCode(GameObject obj)
	{
		return obj.tag.GetHashCode();
	}
}
