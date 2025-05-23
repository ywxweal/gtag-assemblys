using System;
using UnityEngine;

// Token: 0x020009A5 RID: 2469
public static class PoolUtils
{
	// Token: 0x06003B33 RID: 15155 RVA: 0x0011AFA4 File Offset: 0x001191A4
	public static int GameObjHashCode(GameObject obj)
	{
		return obj.tag.GetHashCode();
	}
}
