using System;
using GorillaTag;

// Token: 0x0200099E RID: 2462
public class LoopingArray<T> : ObjectPoolEvents
{
	// Token: 0x170005D5 RID: 1493
	// (get) Token: 0x06003AF9 RID: 15097 RVA: 0x00119BD8 File Offset: 0x00117DD8
	public int Length
	{
		get
		{
			return this.m_length;
		}
	}

	// Token: 0x170005D6 RID: 1494
	// (get) Token: 0x06003AFA RID: 15098 RVA: 0x00119BE0 File Offset: 0x00117DE0
	public int CurrentIndex
	{
		get
		{
			return this.m_currentIndex;
		}
	}

	// Token: 0x170005D7 RID: 1495
	public T this[int index]
	{
		get
		{
			return this.m_array[index];
		}
		set
		{
			this.m_array[index] = value;
		}
	}

	// Token: 0x06003AFD RID: 15101 RVA: 0x00119C05 File Offset: 0x00117E05
	public LoopingArray()
		: this(0)
	{
	}

	// Token: 0x06003AFE RID: 15102 RVA: 0x00119C0E File Offset: 0x00117E0E
	public LoopingArray(int capicity)
	{
		this.m_length = capicity;
		this.m_array = new T[capicity];
		this.Clear();
	}

	// Token: 0x06003AFF RID: 15103 RVA: 0x00119C2F File Offset: 0x00117E2F
	public int AddAndIncrement(in T value)
	{
		int currentIndex = this.m_currentIndex;
		this.m_array[this.m_currentIndex] = value;
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		return currentIndex;
	}

	// Token: 0x06003B00 RID: 15104 RVA: 0x00119C63 File Offset: 0x00117E63
	public int IncrementAndAdd(in T value)
	{
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		this.m_array[this.m_currentIndex] = value;
		return this.m_currentIndex;
	}

	// Token: 0x06003B01 RID: 15105 RVA: 0x00119C98 File Offset: 0x00117E98
	public void Clear()
	{
		this.m_currentIndex = 0;
		for (int i = 0; i < this.m_array.Length; i++)
		{
			this.m_array[i] = default(T);
		}
	}

	// Token: 0x06003B02 RID: 15106 RVA: 0x00119CD4 File Offset: 0x00117ED4
	void ObjectPoolEvents.OnTaken()
	{
		this.Clear();
	}

	// Token: 0x06003B03 RID: 15107 RVA: 0x000023F4 File Offset: 0x000005F4
	void ObjectPoolEvents.OnReturned()
	{
	}

	// Token: 0x04003FC2 RID: 16322
	private int m_length;

	// Token: 0x04003FC3 RID: 16323
	private int m_currentIndex;

	// Token: 0x04003FC4 RID: 16324
	private T[] m_array;

	// Token: 0x0200099F RID: 2463
	public class Pool : ObjectPool<LoopingArray<T>>
	{
		// Token: 0x06003B04 RID: 15108 RVA: 0x00119CDC File Offset: 0x00117EDC
		private Pool(int amount)
			: base(amount)
		{
		}

		// Token: 0x06003B05 RID: 15109 RVA: 0x00119CE5 File Offset: 0x00117EE5
		public Pool(int size, int amount)
			: this(size, amount, amount)
		{
		}

		// Token: 0x06003B06 RID: 15110 RVA: 0x00119CF0 File Offset: 0x00117EF0
		public Pool(int size, int initialAmount, int maxAmount)
		{
			this.m_size = size;
			base.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x06003B07 RID: 15111 RVA: 0x00119D07 File Offset: 0x00117F07
		public override LoopingArray<T> CreateInstance()
		{
			return new LoopingArray<T>(this.m_size);
		}

		// Token: 0x04003FC5 RID: 16325
		private readonly int m_size;
	}
}
