using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E90 RID: 3728
	public struct Vector3Spring
	{
		// Token: 0x06005D47 RID: 23879 RVA: 0x001CBF1F File Offset: 0x001CA11F
		public void Reset()
		{
			this.Value = Vector3.zero;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x06005D48 RID: 23880 RVA: 0x001CBF37 File Offset: 0x001CA137
		public void Reset(Vector3 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x06005D49 RID: 23881 RVA: 0x001CBF4B File Offset: 0x001CA14B
		public void Reset(Vector3 initValue, Vector3 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x06005D4A RID: 23882 RVA: 0x001CBF5C File Offset: 0x001CA15C
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

		// Token: 0x06005D4B RID: 23883 RVA: 0x001CC058 File Offset: 0x001CA258
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

		// Token: 0x06005D4C RID: 23884 RVA: 0x001CC0A4 File Offset: 0x001CA2A4
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

		// Token: 0x04006140 RID: 24896
		public static readonly int Stride = 32;

		// Token: 0x04006141 RID: 24897
		public Vector3 Value;

		// Token: 0x04006142 RID: 24898
		private float m_padding0;

		// Token: 0x04006143 RID: 24899
		public Vector3 Velocity;

		// Token: 0x04006144 RID: 24900
		private float m_padding1;
	}
}
