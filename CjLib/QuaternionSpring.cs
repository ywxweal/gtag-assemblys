using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E42 RID: 3650
	public struct QuaternionSpring
	{
		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06005B58 RID: 23384 RVA: 0x001C0E37 File Offset: 0x001BF037
		// (set) Token: 0x06005B59 RID: 23385 RVA: 0x001C0E45 File Offset: 0x001BF045
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
		// (get) Token: 0x06005B5A RID: 23386 RVA: 0x001C0E53 File Offset: 0x001BF053
		// (set) Token: 0x06005B5B RID: 23387 RVA: 0x001C0E61 File Offset: 0x001BF061
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

		// Token: 0x06005B5C RID: 23388 RVA: 0x001C0E6F File Offset: 0x001BF06F
		public void Reset()
		{
			this.ValueVec = QuaternionUtil.ToVector4(Quaternion.identity);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x06005B5D RID: 23389 RVA: 0x001C0E8C File Offset: 0x001BF08C
		public void Reset(Vector4 initValue)
		{
			this.ValueVec = initValue;
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x06005B5E RID: 23390 RVA: 0x001C0EA0 File Offset: 0x001BF0A0
		public void Reset(Vector4 initValue, Vector4 initVelocity)
		{
			this.ValueVec = initValue;
			this.VelocityVec = initVelocity;
		}

		// Token: 0x06005B5F RID: 23391 RVA: 0x001C0EB0 File Offset: 0x001BF0B0
		public void Reset(Quaternion initValue)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x06005B60 RID: 23392 RVA: 0x001C0EC9 File Offset: 0x001BF0C9
		public void Reset(Quaternion initValue, Quaternion initVelocity)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = QuaternionUtil.ToVector4(initVelocity);
		}

		// Token: 0x06005B61 RID: 23393 RVA: 0x001C0EE4 File Offset: 0x001BF0E4
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

		// Token: 0x06005B62 RID: 23394 RVA: 0x001C1018 File Offset: 0x001BF218
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

		// Token: 0x06005B63 RID: 23395 RVA: 0x001C1068 File Offset: 0x001BF268
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

		// Token: 0x04005F3D RID: 24381
		public static readonly int Stride = 32;

		// Token: 0x04005F3E RID: 24382
		public Vector4 ValueVec;

		// Token: 0x04005F3F RID: 24383
		public Vector4 VelocityVec;
	}
}
