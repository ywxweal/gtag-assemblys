using System;

// Token: 0x02000980 RID: 2432
internal class CircularBuffer<T>
{
	// Token: 0x170005CA RID: 1482
	// (get) Token: 0x06003A73 RID: 14963 RVA: 0x00118145 File Offset: 0x00116345
	// (set) Token: 0x06003A74 RID: 14964 RVA: 0x0011814D File Offset: 0x0011634D
	public int Count { get; private set; }

	// Token: 0x170005CB RID: 1483
	// (get) Token: 0x06003A75 RID: 14965 RVA: 0x00118156 File Offset: 0x00116356
	// (set) Token: 0x06003A76 RID: 14966 RVA: 0x0011815E File Offset: 0x0011635E
	public int Capacity { get; private set; }

	// Token: 0x06003A77 RID: 14967 RVA: 0x00118167 File Offset: 0x00116367
	public CircularBuffer(int capacity)
	{
		this.backingArray = new T[capacity];
		this.Capacity = capacity;
		this.Count = 0;
	}

	// Token: 0x06003A78 RID: 14968 RVA: 0x0011818C File Offset: 0x0011638C
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

	// Token: 0x06003A79 RID: 14969 RVA: 0x001181EA File Offset: 0x001163EA
	public void Clear()
	{
		this.Count = 0;
	}

	// Token: 0x06003A7A RID: 14970 RVA: 0x001181F3 File Offset: 0x001163F3
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

	// Token: 0x04003F6D RID: 16237
	private T[] backingArray;

	// Token: 0x04003F70 RID: 16240
	private int nextWriteIdx;

	// Token: 0x04003F71 RID: 16241
	private int lastWriteIdx;
}
