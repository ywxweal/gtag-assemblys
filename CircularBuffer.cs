using System;

// Token: 0x02000980 RID: 2432
internal class CircularBuffer<T>
{
	// Token: 0x170005CA RID: 1482
	// (get) Token: 0x06003A72 RID: 14962 RVA: 0x0011806D File Offset: 0x0011626D
	// (set) Token: 0x06003A73 RID: 14963 RVA: 0x00118075 File Offset: 0x00116275
	public int Count { get; private set; }

	// Token: 0x170005CB RID: 1483
	// (get) Token: 0x06003A74 RID: 14964 RVA: 0x0011807E File Offset: 0x0011627E
	// (set) Token: 0x06003A75 RID: 14965 RVA: 0x00118086 File Offset: 0x00116286
	public int Capacity { get; private set; }

	// Token: 0x06003A76 RID: 14966 RVA: 0x0011808F File Offset: 0x0011628F
	public CircularBuffer(int capacity)
	{
		this.backingArray = new T[capacity];
		this.Capacity = capacity;
		this.Count = 0;
	}

	// Token: 0x06003A77 RID: 14967 RVA: 0x001180B4 File Offset: 0x001162B4
	public void Add(T value)
	{
		this.backingArray[this.nextWriteIdx] = value;
		this.lastWriteIdx = this.nextWriteIdx;
		this.nextWriteIdx = (this.nextWriteIdx + 1) % this.Capacity;
		if (this.Count < this.Capacity)
		{
			int count = this.Count;
			this.Count = count + 1;
		}
	}

	// Token: 0x06003A78 RID: 14968 RVA: 0x00118112 File Offset: 0x00116312
	public void Clear()
	{
		this.Count = 0;
	}

	// Token: 0x06003A79 RID: 14969 RVA: 0x0011811B File Offset: 0x0011631B
	public T Last()
	{
		return this.backingArray[this.lastWriteIdx];
	}

	// Token: 0x170005CC RID: 1484
	public T this[int logicalIdx]
	{
		get
		{
			if (logicalIdx < 0 || logicalIdx >= this.Count)
			{
				throw new ArgumentOutOfRangeException("logicalIdx", logicalIdx, string.Format("Out of bounds index into Circular Buffer with length {0}", this.Count));
			}
			int num = (this.lastWriteIdx + this.Capacity - logicalIdx) % this.Capacity;
			return this.backingArray[num];
		}
	}

	// Token: 0x04003F6C RID: 16236
	private T[] backingArray;

	// Token: 0x04003F6F RID: 16239
	private int nextWriteIdx;

	// Token: 0x04003F70 RID: 16240
	private int lastWriteIdx;
}
