using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E40 RID: 3648
	public struct Vector3Spring
	{
		// Token: 0x06005B4A RID: 23370 RVA: 0x001C0A8F File Offset: 0x001BEC8F
		public void Reset()
		{
			this.Value = Vector3.zero;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x06005B4B RID: 23371 RVA: 0x001C0AA7 File Offset: 0x001BECA7
		public void Reset(Vector3 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x06005B4C RID: 23372 RVA: 0x001C0ABB File Offset: 0x001BECBB
		public void Reset(Vector3 initValue, Vector3 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x06005B4D RID: 23373 RVA: 0x001C0ACC File Offset: 0x001BECCC
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

		// Token: 0x06005B4E RID: 23374 RVA: 0x001C0BC8 File Offset: 0x001BEDC8
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

		// Token: 0x06005B4F RID: 23375 RVA: 0x001C0C14 File Offset: 0x001BEE14
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

		// Token: 0x04005F35 RID: 24373
		public static readonly int Stride = 32;

		// Token: 0x04005F36 RID: 24374
		public Vector3 Value;

		// Token: 0x04005F37 RID: 24375
		private float m_padding0;

		// Token: 0x04005F38 RID: 24376
		public Vector3 Velocity;

		// Token: 0x04005F39 RID: 24377
		private float m_padding1;
	}
}
