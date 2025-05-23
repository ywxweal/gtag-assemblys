using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E92 RID: 3730
	public struct QuaternionSpring
	{
		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x06005D55 RID: 23893 RVA: 0x001CC2C7 File Offset: 0x001CA4C7
		// (set) Token: 0x06005D56 RID: 23894 RVA: 0x001CC2D5 File Offset: 0x001CA4D5
		public Quaternion ValueQuat
		{
			get
			{
				return QuaternionUtil.FromVector4(this.ValueVec, true);
			}
			set
			{
				this.ValueVec = QuaternionUtil.ToVector4(value);
			}
		}

		// Token: 0x06005D57 RID: 23895 RVA: 0x001CC2E3 File Offset: 0x001CA4E3
		public void Reset()
		{
			this.ValueVec = QuaternionUtil.ToVector4(Quaternion.identity);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x06005D58 RID: 23896 RVA: 0x001CC300 File Offset: 0x001CA500
		public void Reset(Vector4 initValue)
		{
			this.ValueVec = initValue;
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x06005D59 RID: 23897 RVA: 0x001CC314 File Offset: 0x001CA514
		public void Reset(Vector4 initValue, Vector4 initVelocity)
		{
			this.ValueVec = initValue;
			this.VelocityVec = initVelocity;
		}

		// Token: 0x06005D5A RID: 23898 RVA: 0x001CC324 File Offset: 0x001CA524
		public void Reset(Quaternion initValue)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x06005D5B RID: 23899 RVA: 0x001CC33D File Offset: 0x001CA53D
		public void Reset(Quaternion initValue, Quaternion initVelocity)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = QuaternionUtil.ToVector4(initVelocity);
		}

		// Token: 0x06005D5C RID: 23900 RVA: 0x001CC358 File Offset: 0x001CA558
		public Quaternion TrackDampingRatio(Vector4 targetValueVec, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				return QuaternionUtil.FromVector4(this.ValueVec, true);
			}
			if (Vector4.Dot(this.ValueVec, targetValueVec) < 0f)
			{
				targetValueVec = -targetValueVec;
			}
			Vector4 vector = targetValueVec - this.ValueVec;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector4 vector2 = num * this.ValueVec + deltaTime * this.VelocityVec + num4 * targetValueVec;
			Vector4 vector3 = this.VelocityVec + num3 * vector;
			this.VelocityVec = vector3 * num5;
			this.ValueVec = vector2 * num5;
			if (this.VelocityVec.magnitude < MathUtil.Epsilon && vector.magnitude < MathUtil.Epsilon)
			{
				this.VelocityVec = Vector4.zero;
				this.ValueVec = targetValueVec;
			}
			return QuaternionUtil.FromVector4(this.ValueVec, true);
		}

		// Token: 0x06005D5D RID: 23901 RVA: 0x001CC47D File Offset: 0x001CA67D
		public Quaternion TrackDampingRatio(Quaternion targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			return this.TrackDampingRatio(QuaternionUtil.ToVector4(targetValue), angularFrequency, dampingRatio, deltaTime);
		}

		// Token: 0x06005D5E RID: 23902 RVA: 0x001CC490 File Offset: 0x001CA690
		public Quaternion TrackHalfLife(Vector4 targetValueVec, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = Vector4.zero;
				this.ValueVec = targetValueVec;
				return QuaternionUtil.FromVector4(targetValueVec, true);
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float num2 = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValueVec, num, num2, deltaTime);
		}

		// Token: 0x06005D5F RID: 23903 RVA: 0x001CC4DC File Offset: 0x001CA6DC
		public Quaternion TrackHalfLife(Quaternion targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = QuaternionUtil.ToVector4(targetValue);
				return targetValue;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float num2 = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x06005D60 RID: 23904 RVA: 0x001CC52C File Offset: 0x001CA72C
		public Quaternion TrackExponential(Vector4 targetValueVec, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = Vector4.zero;
				this.ValueVec = targetValueVec;
				return QuaternionUtil.FromVector4(targetValueVec, true);
			}
			float num = 0.6931472f / halfLife;
			float num2 = 1f;
			return this.TrackDampingRatio(targetValueVec, num, num2, deltaTime);
		}

		// Token: 0x06005D61 RID: 23905 RVA: 0x001CC574 File Offset: 0x001CA774
		public Quaternion TrackExponential(Quaternion targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = QuaternionUtil.ToVector4(targetValue);
				return targetValue;
			}
			float num = 0.6931472f / halfLife;
			float num2 = 1f;
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x04006148 RID: 24904
		public static readonly int Stride = 32;

		// Token: 0x04006149 RID: 24905
		public Vector4 ValueVec;

		// Token: 0x0400614A RID: 24906
		public Vector4 VelocityVec;
	}
}
