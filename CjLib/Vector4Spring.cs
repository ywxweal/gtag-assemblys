using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E41 RID: 3649
	public struct Vector4Spring
	{
		// Token: 0x06005B50 RID: 23376 RVA: 0x001C0B8B File Offset: 0x001BED8B
		public void Reset()
		{
			this.Value = Vector4.zero;
			this.Velocity = Vector4.zero;
		}

		// Token: 0x06005B51 RID: 23377 RVA: 0x001C0BA3 File Offset: 0x001BEDA3
		public void Reset(Vector4 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector4.zero;
		}

		// Token: 0x06005B52 RID: 23378 RVA: 0x001C0BB7 File Offset: 0x001BEDB7
		public void Reset(Vector4 initValue, Vector4 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x06005B53 RID: 23379 RVA: 0x001C0BC8 File Offset: 0x001BEDC8
		public Vector4 TrackDampingRatio(Vector4 targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.Velocity = Vector4.zero;
				return this.Value;
			}
			Vector4 vector = targetValue - this.Value;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector4 vector2 = num * this.Value + deltaTime * this.Velocity + num4 * targetValue;
			Vector4 vector3 = this.Velocity + num3 * vector;
			this.Velocity = vector3 * num5;
			this.Value = vector2 * num5;
			if (this.Velocity.magnitude < MathUtil.Epsilon && vector.magnitude < MathUtil.Epsilon)
			{
				this.Velocity = Vector4.zero;
				this.Value = targetValue;
			}
			return this.Value;
		}

		// Token: 0x06005B54 RID: 23380 RVA: 0x001C0CC4 File Offset: 0x001BEEC4
		public Vector4 TrackHalfLife(Vector4 targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector4.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float num2 = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x06005B55 RID: 23381 RVA: 0x001C0D10 File Offset: 0x001BEF10
		public Vector4 TrackExponential(Vector4 targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector4.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = 0.6931472f / halfLife;
			float num2 = 1f;
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x04005F39 RID: 24377
		public static readonly int Stride = 32;

		// Token: 0x04005F3A RID: 24378
		public Vector4 Value;

		// Token: 0x04005F3B RID: 24379
		public Vector4 Velocity;
	}
}
