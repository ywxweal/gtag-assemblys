using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000AB1 RID: 2737
	public abstract class AverageCalculator<T> where T : struct
	{
		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x0600420C RID: 16908 RVA: 0x0013171E File Offset: 0x0012F91E
		public T Average
		{
			get
			{
				return this.m_average;
			}
		}

		// Token: 0x0600420D RID: 16909 RVA: 0x00131726 File Offset: 0x0012F926
		public AverageCalculator(int sampleCount)
		{
			this.m_samples = new T[sampleCount];
		}

		// Token: 0x0600420E RID: 16910 RVA: 0x0013173C File Offset: 0x0012F93C
		public virtual void AddSample(T sample)
		{
			T t = this.m_samples[this.m_index];
			this.m_total = this.MinusEquals(this.m_total, t);
			this.m_total = this.PlusEquals(this.m_total, sample);
			this.m_average = this.Divide(this.m_total, this.m_samples.Length);
			this.m_samples[this.m_index] = sample;
			int num = this.m_index + 1;
			this.m_index = num;
			this.m_index = num % this.m_samples.Length;
		}

		// Token: 0x0600420F RID: 16911 RVA: 0x001317D0 File Offset: 0x0012F9D0
		public virtual void Reset()
		{
			T t = this.DefaultTypeValue();
			for (int i = 0; i < this.m_samples.Length; i++)
			{
				this.m_samples[i] = t;
			}
			this.m_index = 0;
			this.m_average = t;
			this.m_total = this.Multiply(t, this.m_samples.Length);
		}

		// Token: 0x06004210 RID: 16912 RVA: 0x00131828 File Offset: 0x0012FA28
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual T DefaultTypeValue()
		{
			return default(T);
		}

		// Token: 0x06004211 RID: 16913
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T PlusEquals(T value, T sample);

		// Token: 0x06004212 RID: 16914
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T MinusEquals(T value, T sample);

		// Token: 0x06004213 RID: 16915
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T Divide(T value, int sampleCount);

		// Token: 0x06004214 RID: 16916
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract T Multiply(T value, int sampleCount);

		// Token: 0x0400448A RID: 17546
		private T[] m_samples;

		// Token: 0x0400448B RID: 17547
		private T m_average;

		// Token: 0x0400448C RID: 17548
		private T m_total;

		// Token: 0x0400448D RID: 17549
		private int m_index;
	}
}
