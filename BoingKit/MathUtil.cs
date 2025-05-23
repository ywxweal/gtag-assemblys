using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E8B RID: 3723
	public class MathUtil
	{
		// Token: 0x06005D19 RID: 23833 RVA: 0x001C04E0 File Offset: 0x001BE6E0
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06005D1A RID: 23834 RVA: 0x001C04F7 File Offset: 0x001BE6F7
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06005D1B RID: 23835 RVA: 0x001CB5AC File Offset: 0x001C97AC
		public static float InvSafe(float x)
		{
			return 1f / Mathf.Max(MathUtil.Epsilon, x);
		}

		// Token: 0x06005D1C RID: 23836 RVA: 0x001CB5C0 File Offset: 0x001C97C0
		public static float PointLineDist(Vector2 point, Vector2 linePos, Vector2 lineDir)
		{
			Vector2 vector = point - linePos;
			return (vector - Vector2.Dot(vector, lineDir) * lineDir).magnitude;
		}

		// Token: 0x06005D1D RID: 23837 RVA: 0x001CB5F0 File Offset: 0x001C97F0
		public static float PointSegmentDist(Vector2 point, Vector2 segmentPosA, Vector2 segmentPosB)
		{
			Vector2 vector = segmentPosB - segmentPosA;
			float num = 1f / vector.magnitude;
			Vector2 vector2 = vector * num;
			float num2 = Vector2.Dot(point - segmentPosA, vector2) * num;
			return (segmentPosA + Mathf.Clamp(num2, 0f, 1f) * vector - point).magnitude;
		}

		// Token: 0x06005D1E RID: 23838 RVA: 0x001CB658 File Offset: 0x001C9858
		public static float Seek(float current, float target, float maxDelta)
		{
			float num = target - current;
			num = Mathf.Sign(num) * Mathf.Min(maxDelta, Mathf.Abs(num));
			return current + num;
		}

		// Token: 0x06005D1F RID: 23839 RVA: 0x001CB680 File Offset: 0x001C9880
		public static Vector2 Seek(Vector2 current, Vector2 target, float maxDelta)
		{
			Vector2 vector = target - current;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return target;
			}
			vector = Mathf.Min(maxDelta, magnitude) * vector.normalized;
			return current + vector;
		}

		// Token: 0x06005D20 RID: 23840 RVA: 0x001CB6C2 File Offset: 0x001C98C2
		public static float Remainder(float a, float b)
		{
			return a - a / b * b;
		}

		// Token: 0x06005D21 RID: 23841 RVA: 0x001CB6C2 File Offset: 0x001C98C2
		public static int Remainder(int a, int b)
		{
			return a - a / b * b;
		}

		// Token: 0x06005D22 RID: 23842 RVA: 0x001CB6CB File Offset: 0x001C98CB
		public static float Modulo(float a, float b)
		{
			return Mathf.Repeat(a, b);
		}

		// Token: 0x06005D23 RID: 23843 RVA: 0x001CB6D4 File Offset: 0x001C98D4
		public static int Modulo(int a, int b)
		{
			int num = a % b;
			if (num < 0)
			{
				return num + b;
			}
			return num;
		}

		// Token: 0x0400612B RID: 24875
		public static readonly float Pi = 3.1415927f;

		// Token: 0x0400612C RID: 24876
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x0400612D RID: 24877
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x0400612E RID: 24878
		public static readonly float QuaterPi = 0.7853982f;

		// Token: 0x0400612F RID: 24879
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x04006130 RID: 24880
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x04006131 RID: 24881
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x04006132 RID: 24882
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x04006133 RID: 24883
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x04006134 RID: 24884
		public static readonly float Epsilon = 1E-06f;

		// Token: 0x04006135 RID: 24885
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x04006136 RID: 24886
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
