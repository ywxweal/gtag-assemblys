using System;
using UnityEngine;

// Token: 0x0200097A RID: 2426
public class BezierUtils
{
	// Token: 0x06003A59 RID: 14937 RVA: 0x00117458 File Offset: 0x00115658
	public static Vector3 BezierSolve(float t, Vector3 startPos, Vector3 ctrl1, Vector3 ctrl2, Vector3 endPos)
	{
		float num = 1f - t;
		float num2 = num * num * num;
		float num3 = 3f * num * num * t;
		float num4 = 3f * num * t * t;
		float num5 = t * t * t;
		return startPos * num2 + ctrl1 * num3 + ctrl2 * num4 + endPos * num5;
	}
}
