using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000AB3 RID: 2739
	public class FloatAverages : AverageCalculator<float>
	{
		// Token: 0x0600421B RID: 16923 RVA: 0x0013193B File Offset: 0x0012FB3B
		public FloatAverages(int sampleCount)
			: base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x0600421C RID: 16924 RVA: 0x00131925 File Offset: 0x0012FB25
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float PlusEquals(float value, float sample)
		{
			return value + sample;
		}

		// Token: 0x0600421D RID: 16925 RVA: 0x0013192A File Offset: 0x0012FB2A
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float MinusEquals(float value, float sample)
		{
			return value - sample;
		}

		// Token: 0x0600421E RID: 16926 RVA: 0x0013194A File Offset: 0x0012FB4A
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float Divide(float value, int sampleCount)
		{
			return value / (float)sampleCount;
		}

		// Token: 0x0600421F RID: 16927 RVA: 0x00131950 File Offset: 0x0012FB50
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float Multiply(float value, int sampleCount)
		{
			return value * (float)sampleCount;
		}
	}
}
