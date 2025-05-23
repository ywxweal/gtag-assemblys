using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000AB3 RID: 2739
	public class FloatAverages : AverageCalculator<float>
	{
		// Token: 0x0600421A RID: 16922 RVA: 0x00131863 File Offset: 0x0012FA63
		public FloatAverages(int sampleCount)
			: base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x0600421B RID: 16923 RVA: 0x0013184D File Offset: 0x0012FA4D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float PlusEquals(float value, float sample)
		{
			return value + sample;
		}

		// Token: 0x0600421C RID: 16924 RVA: 0x00131852 File Offset: 0x0012FA52
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float MinusEquals(float value, float sample)
		{
			return value - sample;
		}

		// Token: 0x0600421D RID: 16925 RVA: 0x00131872 File Offset: 0x0012FA72
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float Divide(float value, int sampleCount)
		{
			return value / (float)sampleCount;
		}

		// Token: 0x0600421E RID: 16926 RVA: 0x00131878 File Offset: 0x0012FA78
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float Multiply(float value, int sampleCount)
		{
			return value * (float)sampleCount;
		}
	}
}
