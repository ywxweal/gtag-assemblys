using System;
using UnityEngine;

// Token: 0x0200031E RID: 798
public static class VectorUtil
{
	// Token: 0x060012F2 RID: 4850 RVA: 0x00059EB5 File Offset: 0x000580B5
	public static Vector4 ToVector(this Rect rect)
	{
		return new Vector4(rect.x, rect.y, rect.width, rect.height);
	}
}
