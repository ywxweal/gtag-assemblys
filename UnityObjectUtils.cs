using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020009D8 RID: 2520
public static class UnityObjectUtils
{
	// Token: 0x06003C56 RID: 15446 RVA: 0x00120498 File Offset: 0x0011E698
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T AsNull<T>(this T obj) where T : Object
	{
		if (obj == null)
		{
			return default(T);
		}
		if (!(obj == null))
		{
			return obj;
		}
		return default(T);
	}

	// Token: 0x06003C57 RID: 15447 RVA: 0x0003A5F2 File Offset: 0x000387F2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SafeDestroy(this Object obj)
	{
		Object.Destroy(obj);
	}
}
