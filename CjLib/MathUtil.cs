using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E3D RID: 3645
	public class MathUtil
	{
		// Token: 0x06005B37 RID: 23351 RVA: 0x001C05B8 File Offset: 0x001BE7B8
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06005B38 RID: 23352 RVA: 0x001C05CF File Offset: 0x001BE7CF
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06005B39 RID: 23353 RVA: 0x001C05E8 File Offset: 0x001BE7E8
		public static float CatmullRom(float p0, float p1, float p2, float p3, float t)
		{
			float num = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
		}

		// Token: 0x04005F20 RID: 24352
		public static readonly float Pi = 3.1415927f;

		// Token: 0x04005F21 RID: 24353
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x04005F22 RID: 24354
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x04005F23 RID: 24355
		public static readonly float ThirdPi = 1.0471976f;

		// Token: 0x04005F24 RID: 24356
		public static readonly float QuarterPi = 0.7853982f;

		// Token: 0x04005F25 RID: 24357
		public static readonly float FifthPi = 0.62831855f;

		// Token: 0x04005F26 RID: 24358
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x04005F27 RID: 24359
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x04005F28 RID: 24360
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x04005F29 RID: 24361
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x04005F2A RID: 24362
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x04005F2B RID: 24363
		public static readonly float Epsilon = 1E-09f;

		// Token: 0x04005F2C RID: 24364
		public static readonly float EpsilonComp = 1f - MathUtil.Epsilon;

		// Token: 0x04005F2D RID: 24365
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x04005F2E RID: 24366
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
