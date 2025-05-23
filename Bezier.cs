using System;
using UnityEngine;

// Token: 0x020009BD RID: 2493
public static class Bezier
{
	// Token: 0x06003BA0 RID: 15264 RVA: 0x0011CDB0 File Offset: 0x0011AFB0
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return num * num * p0 + 2f * num * t * p1 + t * t * p2;
	}

	// Token: 0x06003BA1 RID: 15265 RVA: 0x0011CDF8 File Offset: 0x0011AFF8
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
	}

	// Token: 0x06003BA2 RID: 15266 RVA: 0x0011CE2C File Offset: 0x0011B02C
	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
	}

	// Token: 0x06003BA3 RID: 15267 RVA: 0x0011CE98 File Offset: 0x0011B098
	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 1f - t;
		return 3f * num * num * (p1 - p0) + 6f * num * t * (p2 - p1) + 3f * t * t * (p3 - p2);
	}
}
