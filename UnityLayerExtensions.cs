using System;
using UnityEngine;

// Token: 0x02000227 RID: 551
public static class UnityLayerExtensions
{
	// Token: 0x06000CCF RID: 3279 RVA: 0x000430A6 File Offset: 0x000412A6
	public static int ToLayerMask(this UnityLayer self)
	{
		return 1 << (int)self;
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x000430AE File Offset: 0x000412AE
	public static int ToLayerIndex(this UnityLayer self)
	{
		return (int)self;
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x000430B1 File Offset: 0x000412B1
	public static bool IsOnLayer(this GameObject obj, UnityLayer layer)
	{
		return obj.layer == (int)layer;
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x000430BC File Offset: 0x000412BC
	public static void SetLayer(this GameObject obj, UnityLayer layer)
	{
		obj.layer = (int)layer;
	}
}
