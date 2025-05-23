using System;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public static class AprilFools
{
	// Token: 0x06000622 RID: 1570 RVA: 0x00023459 File Offset: 0x00021659
	public static int mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x00023464 File Offset: 0x00021664
	public static float GenerateTarget(string username, string roomName, string areaName, int startTime)
	{
		float num = (float)AprilFools.mod(string.Format("{0}-{1}-{2}-{3}", new object[] { username, roomName, areaName, startTime }).GetHashCode(), 10000) / 10000f;
		float num2 = 1.5f;
		float num3 = 0.5f / 2f;
		num = num * num2 + 0.5f;
		if (num > 0.75f && num < 1.25f)
		{
			if (num < 1f)
			{
				num = 0.75f - num3;
			}
			else
			{
				num = 1.25f + num3;
			}
		}
		return num;
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x000234F4 File Offset: 0x000216F4
	public static float Slerp(float a, float b, float t)
	{
		float num = Mathf.Acos(Mathf.Clamp(Vector2.Dot(new Vector2(a, b).normalized, Vector2.right), -1f, 1f)) * t;
		float num2 = Mathf.Sin(num);
		float num3 = Mathf.Sin((1f - t) * num) / num2;
		float num4 = Mathf.Sin(t * num) / num2;
		return a * num3 + b * num4;
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x0002355C File Offset: 0x0002175C
	public static float SmoothSlerp(float a, float b, float t)
	{
		t = Mathf.Clamp01(t);
		float num = 0.00013888889f;
		float num2 = Mathf.Clamp01(t - num);
		float num3 = Mathf.Clamp01(t + num);
		float num4 = 3f * num2 * num2 - 2f * num2 * num2 * num2;
		float num5 = 3f * num3 * num3 - 2f * num3 * num3 * num3;
		return AprilFools.Slerp(a, b, Mathf.Lerp(num4, num5, (t - num2) / (num3 - num2)));
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x000235D0 File Offset: 0x000217D0
	public static float GenerateSmoothTarget(string username, string roomName, string areaName)
	{
		float num = (float)DateTime.UtcNow.Subtract(new DateTime(2023, 3, 30)).TotalSeconds;
		int num2 = (int)num / 300;
		int num3 = num2 * 300;
		int num4 = (num2 + 1) * 300;
		float num5 = AprilFools.GenerateTarget(username, roomName, areaName, num3);
		float num6 = AprilFools.GenerateTarget(username, roomName, areaName, num4);
		float num7 = (num - (float)num3) / 120f;
		if (num7 > 1f)
		{
			num7 = 1f;
		}
		float num8 = AprilFools.SmoothSlerp(num5, num6, num7);
		if (float.IsInfinity(num8))
		{
			return 0.5f;
		}
		if (float.IsNaN(num8))
		{
			return 0.5f;
		}
		if (num8 < 0.5f)
		{
			return 0.5f;
		}
		if (num8 > 2f)
		{
			return 2f;
		}
		return num8;
	}

	// Token: 0x0400073D RID: 1853
	private const int changeIntervalSeconds = 300;

	// Token: 0x0400073E RID: 1854
	private const int lerpIntervalSeconds = 120;

	// Token: 0x0400073F RID: 1855
	private const float minRange = 0.5f;

	// Token: 0x04000740 RID: 1856
	private const float maxRange = 2f;

	// Token: 0x04000741 RID: 1857
	private const float excludeRangeStart = 0.75f;

	// Token: 0x04000742 RID: 1858
	private const float excludeRangeEnd = 1.25f;
}
