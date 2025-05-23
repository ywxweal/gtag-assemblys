using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02000D42 RID: 3394
	public class ObjectPool<T> where T : ObjectPoolEvents, new()
	{
		// Token: 0x060054FF RID: 21759 RVA: 0x0019E00A File Offset: 0x0019C20A
		protected ObjectPool()
		{
		}

		// Token: 0x06005500 RID: 21760 RVA: 0x0019E01D File Offset: 0x0019C21D
		public ObjectPool(int amount)
			: this(amount, amount)
		{
		}

		// Token: 0x06005501 RID: 21761 RVA: 0x0019E027 File Offset: 0x0019C227
		public ObjectPool(int initialAmount, int maxAmount)
		{
			this.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x06005502 RID: 21762 RVA: 0x0019E044 File Offset: 0x0019C244
		protected void InitializePool(int initialAmount, int maxAmount)
		{
			this.maxInstances = maxAmount;
			this.pool = new Stack<T>(initialAmount);
			for (int i = 0; i < initialAmount; i++)
			{
				this.pool.Push(this.CreateInstance());
			}
		}

		// Token: 0x06005503 RID: 21763 RVA: 0x0019E084 File Offset: 0x0019C284
		public T Take()
		{
			T t;
			if (this.pool.Count < 1)
			{
				t = this.CreateInstance();
			}
			else
			{
				t = this.pool.Pop();
			}
			t.OnTaken();
			return t;
		}

		// Token: 0x06005504 RID: 21764 RVA: 0x0019E0C2 File Offset: 0x0019C2C2
		public void Return(T instance)
		{
			instance.OnReturned();
			if (this.pool.Count == this.maxInstances)
			{
				return;
			}
			this.pool.Push(instance);
		}

		// Token: 0x06005505 RID: 21765 RVA: 0x0019E0F1 File Offset: 0x0019C2F1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual T CreateInstance()
		{
			return new T();
		}

		// Token: 0x0400584D RID: 22605
		private Stack<T> pool;

		// Token: 0x0400584E RID: 22606
		public int maxInstances = 500;
	}
}
