using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E42 RID: 3650
	public struct QuaternionSpring
	{
		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06005B57 RID: 23383 RVA: 0x001C0D5F File Offset: 0x001BEF5F
		// (set) Token: 0x06005B58 RID: 23384 RVA: 0x001C0D6D File Offset: 0x001BEF6D
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

		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06005B59 RID: 23385 RVA: 0x001C0D7B File Offset: 0x001BEF7B
		// (set) Token: 0x06005B5A RID: 23386 RVA: 0x001C0D89 File Offset: 0x001BEF89
		public Quaternion VelocityQuat
		{
			get
			{
				return QuaternionUtil.FromVector4(this.VelocityVec, false);
			}
			set
			{
				this.VelocityVec = QuaternionUtil.ToVector4(value);
			}
		}

		// Token: 0x06005B5B RID: 23387 RVA: 0x001C0D97 File Offset: 0x001BEF97
		public void Reset()
		{
			this.ValueVec = QuaternionUtil.ToVector4(Quaternion.identity);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x06005B5C RID: 23388 RVA: 0x001C0DB4 File Offset: 0x001BEFB4
		public void Reset(Vector4 initValue)
		{
			this.ValueVec = initValue;
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x06005B5D RID: 23389 RVA: 0x001C0DC8 File Offset: 0x001BEFC8
		public void Reset(Vector4 initValue, Vector4 initVelocity)
		{
			this.ValueVec = initValue;
			this.VelocityVec = initVelocity;
		}

		// Token: 0x06005B5E RID: 23390 RVA: 0x001C0DD8 File Offset: 0x001BEFD8
		public void Reset(Quaternion initValue)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x06005B5F RID: 23391 RVA: 0x001C0DF1 File Offset: 0x001BEFF1
		public void Reset(Quaternion initValue, Quaternion initVelocity)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = QuaternionUtil.ToVector4(initVelocity);
		}

		// Token: 0x06005B60 RID: 23392 RVA: 0x001C0E0C File Offset: 0x001BF00C
		public Quaternion TrackDampingRatio(Quaternion targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				return QuaternionUtil.FromVector4(this.ValueVec, true);
			}
			Vector4 vector = QuaternionUtil.ToVector4(targetValue);
			if (Vector4.Dot(this.ValueVec, vector) < 0f)
			{
				vector = -vector;
			}
			Vector4 vector2 = vector - this.ValueVec;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector4 vector3 = num * this.ValueVec + deltaTime * this.VelocityVec + num4 * vector;
			Vector4 vector4 = this.VelocityVec + num3 * vector2;
			this.VelocityVec = vector4 * num5;
			this.ValueVec = vector3 * num5;
			if (this.VelocityVec.magnitude < MathUtil.Epsilon && vector2.magnitude < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = vector;
			}
			return QuaternionUtil.FromVector4(this.ValueVec, true);
		}

		// Token: 0x06005B61 RID: 23393 RVA: 0x001C0F40 File Offset: 0x001BF140
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

		// Token: 0x06005B62 RID: 23394 RVA: 0x001C0F90 File Offset: 0x001BF190
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

		// Token: 0x04005F3C RID: 24380
		public static readonly int Stride = 32;

		// Token: 0x04005F3D RID: 24381
		public Vector4 ValueVec;

		// Token: 0x04005F3E RID: 24382
		public Vector4 VelocityVec;
	}
}
