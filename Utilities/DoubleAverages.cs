using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000AB2 RID: 2738
	public class DoubleAverages : AverageCalculator<double>
	{
		// Token: 0x06004216 RID: 16918 RVA: 0x00131916 File Offset: 0x0012FB16
		public DoubleAverages(int sampleCount)
			: base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x06004217 RID: 16919 RVA: 0x00131925 File Offset: 0x0012FB25
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double PlusEquals(double value, double sample)
		{
			return value + sample;
		}

		// Token: 0x06004218 RID: 16920 RVA: 0x0013192A File Offset: 0x0012FB2A
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double MinusEquals(double value, double sample)
		{
			return value - sample;
		}

		// Token: 0x06004219 RID: 16921 RVA: 0x0013192F File Offset: 0x0012FB2F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double Divide(double value, int sampleCount)
		{
			return value / (double)sampleCount;
		}

		// Token: 0x0600421A RID: 16922 RVA: 0x00131935 File Offset: 0x0012FB35
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double Multiply(double value, int sampleCount)
		{
			return value * (double)sampleCount;
		}
	}
}
