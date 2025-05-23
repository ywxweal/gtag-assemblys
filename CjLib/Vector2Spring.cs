using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E3F RID: 3647
	public struct Vector2Spring
	{
		// Token: 0x06005B42 RID: 23362 RVA: 0x001C07E2 File Offset: 0x001BE9E2
		public void Reset()
		{
			this.Value = Vector2.zero;
			this.Velocity = Vector2.zero;
		}

		// Token: 0x06005B43 RID: 23363 RVA: 0x001C07FA File Offset: 0x001BE9FA
		public void Reset(Vector2 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector2.zero;
		}

		// Token: 0x06005B44 RID: 23364 RVA: 0x001C080E File Offset: 0x001BEA0E
		public void Reset(Vector2 initValue, Vector2 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x06005B45 RID: 23365 RVA: 0x001C0820 File Offset: 0x001BEA20
		public Vector2 TrackDampingRatio(Vector2 targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.Velocity = Vector2.zero;
				return this.Value;
			}
			Vector2 vector = targetValue - this.Value;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector2 vector2 = num * this.Value + deltaTime * this.Velocity + num4 * targetValue;
			Vector2 vector3 = this.Velocity + num3 * vector;
			this.Velocity = vector3 * num5;
			this.Value = vector2 * num5;
			if (this.Velocity.magnitude < MathUtil.Epsilon && vector.magnitude < MathUtil.Epsilon)
			{
				this.Velocity = Vector2.zero;
				this.Value = targetValue;
			}
			return this.Value;
		}

		// Token: 0x06005B46 RID: 23366 RVA: 0x001C091C File Offset: 0x001BEB1C
		public Vector2 TrackHalfLife(Vector2 targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector2.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float num2 = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x06005B47 RID: 23367 RVA: 0x001C0968 File Offset: 0x001BEB68
		public Vector2 TrackExponential(Vector2 targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector2.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = 0.6931472f / halfLife;
			float num2 = 1f;
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x04005F31 RID: 24369
		public static readonly int Stride = 16;

		// Token: 0x04005F32 RID: 24370
		public Vector2 Value;

		// Token: 0x04005F33 RID: 24371
		public Vector2 Velocity;
	}
}
