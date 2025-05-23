using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public static class GTVector3Extensions
{
	// Token: 0x06000ABF RID: 2751 RVA: 0x0003A5DA File Offset: 0x000387DA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 X_Z(this Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}
}
