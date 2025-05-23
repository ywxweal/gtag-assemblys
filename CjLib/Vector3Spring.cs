using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E40 RID: 3648
	public struct Vector3Spring
	{
		// Token: 0x06005B49 RID: 23369 RVA: 0x001C09B7 File Offset: 0x001BEBB7
		public void Reset()
		{
			this.Value = Vector3.zero;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x06005B4A RID: 23370 RVA: 0x001C09CF File Offset: 0x001BEBCF
		public void Reset(Vector3 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x06005B4B RID: 23371 RVA: 0x001C09E3 File Offset: 0x001BEBE3
		public void Reset(Vector3 initValue, Vector3 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x06005B4C RID: 23372 RVA: 0x001C09F4 File Offset: 0x001BEBF4
		public Vector3 TrackDampingRatio(Vector3 targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				return this.Value;
			}
			Vector3 vector = targetValue - this.Value;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector3 vector2 = num * this.Value + deltaTime * this.Velocity + num4 * targetValue;
			Vector3 vector3 = this.Velocity + num3 * vector;
			this.Velocity = vector3 * num5;
			this.Value = vector2 * num5;
			if (this.Velocity.magnitude < MathUtil.Epsilon && vector.magnitude < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
			}
			return this.Value;
		}

		// Token: 0x06005B4D RID: 23373 RVA: 0x001C0AF0 File Offset: 0x001BECF0
		public Vector3 TrackHalfLife(Vector3 targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float num2 = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x06005B4E RID: 23374 RVA: 0x001C0B3C File Offset: 0x001BED3C
		public Vector3 TrackExponential(Vector3 targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = 0.6931472f / halfLife;
			float num2 = 1f;
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x04005F34 RID: 24372
		public static readonly int Stride = 32;

		// Token: 0x04005F35 RID: 24373
		public Vector3 Value;

		// Token: 0x04005F36 RID: 24374
		private float m_padding0;

		// Token: 0x04005F37 RID: 24375
		public Vector3 Velocity;

		// Token: 0x04005F38 RID: 24376
		private float m_padding1;
	}
}
