using System;
using GorillaTag;

// Token: 0x0200099E RID: 2462
public class LoopingArray<T> : ObjectPoolEvents
{
	// Token: 0x170005D5 RID: 1493
	// (get) Token: 0x06003AF8 RID: 15096 RVA: 0x00119B00 File Offset: 0x00117D00
	public int Length
	{
		get
		{
			return this.m_length;
		}
	}

	// Token: 0x170005D6 RID: 1494
	// (get) Token: 0x06003AF9 RID: 15097 RVA: 0x00119B08 File Offset: 0x00117D08
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

	// Token: 0x06003AFC RID: 15100 RVA: 0x00119B2D File Offset: 0x00117D2D
	public LoopingArray()
		: this(0)
	{
	}

	// Token: 0x06003AFD RID: 15101 RVA: 0x00119B36 File Offset: 0x00117D36
	public LoopingArray(int capicity)
	{
		this.m_length = capicity;
		this.m_array = new T[capicity];
		this.Clear();
	}

	// Token: 0x06003AFE RID: 15102 RVA: 0x00119B57 File Offset: 0x00117D57
	public int AddAndIncrement(in T value)
	{
		int currentIndex = this.m_currentIndex;
		this.m_array[this.m_currentIndex] = value;
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		return currentIndex;
	}

	// Token: 0x06003AFF RID: 15103 RVA: 0x00119B8B File Offset: 0x00117D8B
	public int IncrementAndAdd(in T value)
	{
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		this.m_array[this.m_currentIndex] = value;
		return this.m_currentIndex;
	}

	// Token: 0x06003B00 RID: 15104 RVA: 0x00119BC0 File Offset: 0x00117DC0
	public void Clear()
	{
		this.m_currentIndex = 0;
		for (int i = 0; i < this.m_array.Length; i++)
		{
			this.m_array[i] = default(T);
		}
	}

	// Token: 0x06003B01 RID: 15105 RVA: 0x00119BFC File Offset: 0x00117DFC
	void ObjectPoolEvents.OnTaken()
	{
		this.Clear();
	}

	// Token: 0x06003B02 RID: 15106 RVA: 0x000023F4 File Offset: 0x000005F4
	void ObjectPoolEvents.OnReturned()
	{
	}

	// Token: 0x04003FC1 RID: 16321
	private int m_length;

	// Token: 0x04003FC2 RID: 16322
	private int m_currentIndex;

	// Token: 0x04003FC3 RID: 16323
	private T[] m_array;

	// Token: 0x0200099F RID: 2463
	public class Pool : ObjectPool<LoopingArray<T>>
	{
		// Token: 0x06003B03 RID: 15107 RVA: 0x00119C04 File Offset: 0x00117E04
		private Pool(int amount)
			: base(amount)
		{
		}

		// Token: 0x06003B04 RID: 15108 RVA: 0x00119C0D File Offset: 0x00117E0D
		public Pool(int size, int amount)
			: this(size, amount, amount)
		{
		}

		// Token: 0x06003B05 RID: 15109 RVA: 0x00119C18 File Offset: 0x00117E18
		public Pool(int size, int initialAmount, int maxAmount)
		{
			this.m_size = size;
			base.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x06003B06 RID: 15110 RVA: 0x00119C2F File Offset: 0x00117E2F
		public override LoopingArray<T> CreateInstance()
		{
			return new LoopingArray<T>(this.m_size);
		}

		// Token: 0x04003FC4 RID: 16324
		private readonly int m_size;
	}
}
