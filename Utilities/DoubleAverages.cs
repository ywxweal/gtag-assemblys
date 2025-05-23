using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000AB2 RID: 2738
	public class DoubleAverages : AverageCalculator<double>
	{
		// Token: 0x06004215 RID: 16917 RVA: 0x0013183E File Offset: 0x0012FA3E
		public DoubleAverages(int sampleCount)
			: base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x06004216 RID: 16918 RVA: 0x0013184D File Offset: 0x0012FA4D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double PlusEquals(double value, double sample)
		{
			return value + sample;
		}

		// Token: 0x06004217 RID: 16919 RVA: 0x00131852 File Offset: 0x0012FA52
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double MinusEquals(double value, double sample)
		{
			return value - sample;
		}

		// Token: 0x06004218 RID: 16920 RVA: 0x00131857 File Offset: 0x0012FA57
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double Divide(double value, int sampleCount)
		{
			return value / (double)sampleCount;
		}

		// Token: 0x06004219 RID: 16921 RVA: 0x0013185D File Offset: 0x0012FA5D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double Multiply(double value, int sampleCount)
		{
			return value * (double)sampleCount;
		}
	}
}
