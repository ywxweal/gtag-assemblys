using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E45 RID: 3653
	public class VectorUtil
	{
		// Token: 0x06005B76 RID: 23414 RVA: 0x001C143C File Offset: 0x001BF63C
		public static Vector3 Rotate2D(Vector3 v, float deg)
		{
			Vector3 vector = v;
			float num = Mathf.Cos(MathUtil.Deg2Rad * deg);
			float num2 = Mathf.Sin(MathUtil.Deg2Rad * deg);
			vector.x = num * v.x - num2 * v.y;
			vector.y = num2 * v.x + num * v.y;
			return vector;
		}

		// Token: 0x06005B77 RID: 23415 RVA: 0x001C1496 File Offset: 0x001BF696
		public static Vector3 NormalizeSafe(Vector3 v, Vector3 fallback)
		{
			if (v.sqrMagnitude <= MathUtil.Epsilon)
			{
				return fallback;
			}
			return v.normalized;
		}

		// Token: 0x06005B78 RID: 23416 RVA: 0x001C14B0 File Offset: 0x001BF6B0
		public static Vector3 FindOrthogonal(Vector3 v)
		{
			if (Mathf.Abs(v.x) >= MathUtil.Sqrt3Inv)
			{
				return Vector3.Normalize(new Vector3(v.y, -v.x, 0f));
			}
			return Vector3.Normalize(new Vector3(0f, v.z, -v.y));
		}

		// Token: 0x06005B79 RID: 23417 RVA: 0x001C1508 File Offset: 0x001BF708
		public static void FormOrthogonalBasis(Vector3 v, out Vector3 a, out Vector3 b)
		{
			a = VectorUtil.FindOrthogonal(v);
			b = Vector3.Cross(a, v);
		}

		// Token: 0x06005B7A RID: 23418 RVA: 0x001C1528 File Offset: 0x001BF728
		public static Vector3 Integrate(Vector3 x, Vector3 v, float dt)
		{
			return x + v * dt;
		}

		// Token: 0x06005B7B RID: 23419 RVA: 0x001C1538 File Offset: 0x001BF738
		public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
		{
			float num = Vector3.Dot(a, b);
			if (num > 0.99999f)
			{
				return Vector3.Lerp(a, b, t);
			}
			if (num < -0.99999f)
			{
				Vector3 vector = VectorUtil.FindOrthogonal(a);
				return Quaternion.AngleAxis(180f * t, vector) * a;
			}
			float num2 = MathUtil.AcosSafe(num);
			return (Mathf.Sin((1f - t) * num2) * a + Mathf.Sin(t * num2) * b) / Mathf.Sin(num2);
		}

		// Token: 0x06005B7C RID: 23420 RVA: 0x001C15BC File Offset: 0x001BF7BC
		public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			float num = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
		}
	}
}
