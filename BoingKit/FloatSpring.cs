using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E8E RID: 3726
	public struct FloatSpring
	{
		// Token: 0x06005D39 RID: 23865 RVA: 0x001CBBA3 File Offset: 0x001C9DA3
		public void Reset()
		{
			this.Value = 0f;
			this.Velocity = 0f;
		}

		// Token: 0x06005D3A RID: 23866 RVA: 0x001CBBBB File Offset: 0x001C9DBB
		public void Reset(float initValue)
		{
			this.Value = initValue;
			this.Velocity = 0f;
		}

		// Token: 0x06005D3B RID: 23867 RVA: 0x001CBBCF File Offset: 0x001C9DCF
		public void Reset(float initValue, float initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x06005D3C RID: 23868 RVA: 0x001CBBE0 File Offset: 0x001C9DE0
		public float TrackDampingRatio(float targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.Velocity = 0f;
				return this.Value;
			}
			float num = targetValue - this.Value;
			float num2 = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num3 = angularFrequency * angularFrequency;
			float num4 = deltaTime * num3;
			float num5 = deltaTime * num4;
			float num6 = 1f / (num2 + num5);
			float num7 = num2 * this.Value + deltaTime * this.Velocity + num5 * targetValue;
			float num8 = this.Velocity + num4 * num;
			this.Velocity = num8 * num6;
			this.Value = num7 * num6;
			if (Mathf.Abs(this.Velocity) < MathUtil.Epsilon && Mathf.Abs(num) < MathUtil.Epsilon)
			{
				this.Velocity = 0f;
				this.Value = targetValue;
			}
			return this.Value;
		}

		// Token: 0x06005D3D RID: 23869 RVA: 0x001CBCB0 File Offset: 0x001C9EB0
		public float TrackHalfLife(float targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = 0f;
				this.Value = targetValue;
				return this.Value;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float num2 = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x06005D3E RID: 23870 RVA: 0x001CBCFC File Offset: 0x001C9EFC
		public float TrackExponential(float targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = 0f;
				this.Value = targetValue;
				return this.Value;
			}
			float num = 0.6931472f / halfLife;
			float num2 = 1f;
			return this.TrackDampingRatio(targetValue, num, num2, deltaTime);
		}

		// Token: 0x0400613A RID: 24890
		public static readonly int Stride = 8;

		// Token: 0x0400613B RID: 24891
		public float Value;

		// Token: 0x0400613C RID: 24892
		public float Velocity;
	}
}
