﻿using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E8C RID: 3724
	public class QuaternionUtil
	{
		// Token: 0x06005D27 RID: 23847 RVA: 0x001C10BC File Offset: 0x001BF2BC
		public static float Magnitude(Quaternion q)
		{
			return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
		}

		// Token: 0x06005D28 RID: 23848 RVA: 0x001C10FA File Offset: 0x001BF2FA
		public static float MagnitudeSqr(Quaternion q)
		{
			return q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
		}

		// Token: 0x06005D29 RID: 23849 RVA: 0x001CB870 File Offset: 0x001C9A70
		public static Quaternion Normalize(Quaternion q)
		{
			float num = 1f / QuaternionUtil.Magnitude(q);
			return new Quaternion(num * q.x, num * q.y, num * q.z, num * q.w);
		}

		// Token: 0x06005D2A RID: 23850 RVA: 0x001CB8B0 File Offset: 0x001C9AB0
		public static Quaternion AxisAngle(Vector3 axis, float angle)
		{
			float num = 0.5f * angle;
			float num2 = Mathf.Sin(num);
			float num3 = Mathf.Cos(num);
			return new Quaternion(num2 * axis.x, num2 * axis.y, num2 * axis.z, num3);
		}

		// Token: 0x06005D2B RID: 23851 RVA: 0x001CB8F0 File Offset: 0x001C9AF0
		public static Vector3 GetAxis(Quaternion q)
		{
			Vector3 vector = new Vector3(q.x, q.y, q.z);
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return Vector3.left;
			}
			return vector / magnitude;
		}

		// Token: 0x06005D2C RID: 23852 RVA: 0x001C1257 File Offset: 0x001BF457
		public static float GetAngle(Quaternion q)
		{
			return 2f * Mathf.Acos(Mathf.Clamp(q.w, -1f, 1f));
		}

		// Token: 0x06005D2D RID: 23853 RVA: 0x001CB934 File Offset: 0x001C9B34
		public static Quaternion FromAngularVector(Vector3 v)
		{
			float magnitude = v.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return Quaternion.identity;
			}
			v /= magnitude;
			float num = 0.5f * magnitude;
			float num2 = Mathf.Sin(num);
			float num3 = Mathf.Cos(num);
			return new Quaternion(num2 * v.x, num2 * v.y, num2 * v.z, num3);
		}

		// Token: 0x06005D2E RID: 23854 RVA: 0x001CB994 File Offset: 0x001C9B94
		public static Vector3 ToAngularVector(Quaternion q)
		{
			Vector3 axis = QuaternionUtil.GetAxis(q);
			return QuaternionUtil.GetAngle(q) * axis;
		}

		// Token: 0x06005D2F RID: 23855 RVA: 0x001CB9B4 File Offset: 0x001C9BB4
		public static Quaternion Pow(Quaternion q, float exp)
		{
			Vector3 axis = QuaternionUtil.GetAxis(q);
			float num = QuaternionUtil.GetAngle(q) * exp;
			return QuaternionUtil.AxisAngle(axis, num);
		}

		// Token: 0x06005D30 RID: 23856 RVA: 0x001CB9D6 File Offset: 0x001C9BD6
		public static Quaternion Integrate(Quaternion q, Quaternion v, float dt)
		{
			return QuaternionUtil.Pow(v, dt) * q;
		}

		// Token: 0x06005D31 RID: 23857 RVA: 0x001CB9E8 File Offset: 0x001C9BE8
		public static Quaternion Integrate(Quaternion q, Vector3 omega, float dt)
		{
			omega *= 0.5f;
			Quaternion quaternion = new Quaternion(omega.x, omega.y, omega.z, 0f) * q;
			return QuaternionUtil.Normalize(new Quaternion(q.x + quaternion.x * dt, q.y + quaternion.y * dt, q.z + quaternion.z * dt, q.w + quaternion.w * dt));
		}

		// Token: 0x06005D32 RID: 23858 RVA: 0x001C12E5 File Offset: 0x001BF4E5
		public static Vector4 ToVector4(Quaternion q)
		{
			return new Vector4(q.x, q.y, q.z, q.w);
		}

		// Token: 0x06005D33 RID: 23859 RVA: 0x001CBA6C File Offset: 0x001C9C6C
		public static Quaternion FromVector4(Vector4 v, bool normalize = true)
		{
			if (normalize)
			{
				float sqrMagnitude = v.sqrMagnitude;
				if (sqrMagnitude < MathUtil.Epsilon)
				{
					return Quaternion.identity;
				}
				v /= Mathf.Sqrt(sqrMagnitude);
			}
			return new Quaternion(v.x, v.y, v.z, v.w);
		}

		// Token: 0x06005D34 RID: 23860 RVA: 0x001CBAC0 File Offset: 0x001C9CC0
		public static void DecomposeSwingTwist(Quaternion q, Vector3 twistAxis, out Quaternion swing, out Quaternion twist)
		{
			Vector3 vector = new Vector3(q.x, q.y, q.z);
			if (vector.sqrMagnitude < MathUtil.Epsilon)
			{
				Vector3 vector2 = q * twistAxis;
				Vector3 vector3 = Vector3.Cross(twistAxis, vector2);
				if (vector3.sqrMagnitude > MathUtil.Epsilon)
				{
					float num = Vector3.Angle(twistAxis, vector2);
					swing = Quaternion.AngleAxis(num, vector3);
				}
				else
				{
					swing = Quaternion.identity;
				}
				twist = Quaternion.AngleAxis(180f, twistAxis);
				return;
			}
			Vector3 vector4 = Vector3.Project(vector, twistAxis);
			twist = new Quaternion(vector4.x, vector4.y, vector4.z, q.w);
			twist = QuaternionUtil.Normalize(twist);
			swing = q * Quaternion.Inverse(twist);
		}

		// Token: 0x06005D35 RID: 23861 RVA: 0x001CBB9C File Offset: 0x001C9D9C
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float t, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			return QuaternionUtil.Sterp(a, b, twistAxis, t, out quaternion, out quaternion2, mode);
		}

		// Token: 0x06005D36 RID: 23862 RVA: 0x001CBBB8 File Offset: 0x001C9DB8
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float t, out Quaternion swing, out Quaternion twist, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			return QuaternionUtil.Sterp(a, b, twistAxis, t, t, out swing, out twist, mode);
		}

		// Token: 0x06005D37 RID: 23863 RVA: 0x001CBBCC File Offset: 0x001C9DCC
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float tSwing, float tTwist, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			return QuaternionUtil.Sterp(a, b, twistAxis, tSwing, tTwist, out quaternion, out quaternion2, mode);
		}

		// Token: 0x06005D38 RID: 23864 RVA: 0x001CBBEC File Offset: 0x001C9DEC
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float tSwing, float tTwist, out Quaternion swing, out Quaternion twist, QuaternionUtil.SterpMode mode)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			QuaternionUtil.DecomposeSwingTwist(b * Quaternion.Inverse(a), twistAxis, out quaternion, out quaternion2);
			if (mode == QuaternionUtil.SterpMode.Nlerp || mode != QuaternionUtil.SterpMode.Slerp)
			{
				swing = Quaternion.Lerp(Quaternion.identity, quaternion, tSwing);
				twist = Quaternion.Lerp(Quaternion.identity, quaternion2, tTwist);
			}
			else
			{
				swing = Quaternion.Slerp(Quaternion.identity, quaternion, tSwing);
				twist = Quaternion.Slerp(Quaternion.identity, quaternion2, tTwist);
			}
			return twist * swing;
		}

		// Token: 0x02000E8D RID: 3725
		public enum SterpMode
		{
			// Token: 0x04006139 RID: 24889
			Nlerp,
			// Token: 0x0400613A RID: 24890
			Slerp
		}
	}
}
