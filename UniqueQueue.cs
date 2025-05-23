using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x020009D6 RID: 2518
public class UniqueQueue<T> : IEnumerable<T>, IEnumerable
{
	// Token: 0x170005E3 RID: 1507
	// (get) Token: 0x06003C3B RID: 15419 RVA: 0x0011FF8B File Offset: 0x0011E18B
	public int Count
	{
		get
		{
			return this.queue.Count;
		}
	}

	// Token: 0x06003C3C RID: 15420 RVA: 0x0011FF98 File Offset: 0x0011E198
	public UniqueQueue()
	{
		this.queuedItems = new HashSet<T>();
		this.queue = new Queue<T>();
	}

	// Token: 0x06003C3D RID: 15421 RVA: 0x0011FFB6 File Offset: 0x0011E1B6
	public UniqueQueue(int capacity)
	{
		this.queuedItems = new HashSet<T>(capacity);
		this.queue = new Queue<T>(capacity);
	}

	// Token: 0x06003C3E RID: 15422 RVA: 0x0011FFD6 File Offset: 0x0011E1D6
	public void Clear()
	{
		this.queuedItems.Clear();
		this.queue.Clear();
	}

	// Token: 0x06003C3F RID: 15423 RVA: 0x0011FFEE File Offset: 0x0011E1EE
	public bool Enqueue(T item)
	{
		if (!this.queuedItems.Add(item))
		{
			return false;
		}
		this.queue.Enqueue(item);
		return true;
	}

	// Token: 0x06003C40 RID: 15424 RVA: 0x00120010 File Offset: 0x0011E210
	public T Dequeue()
	{
		T t = this.queue.Dequeue();
		this.queuedItems.Remove(t);
		return t;
	}

	// Token: 0x06003C41 RID: 15425 RVA: 0x00120037 File Offset: 0x0011E237
	public bool TryDequeue(out T item)
	{
		if (this.queue.Count < 1)
		{
			item = default(T);
			return false;
		}
		item = this.Dequeue();
		return true;
	}

	// Token: 0x06003C42 RID: 15426 RVA: 0x0012005D File Offset: 0x0011E25D
	public T Peek()
	{
		return this.queue.Peek();
	}

	// Token: 0x06003C43 RID: 15427 RVA: 0x0012006A File Offset: 0x0011E26A
	public bool Contains(T item)
	{
		return this.queuedItems.Contains(item);
	}

	// Token: 0x06003C44 RID: 15428 RVA: 0x00120078 File Offset: 0x0011E278
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x06003C45 RID: 15429 RVA: 0x00120078 File Offset: 0x0011E278
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x04004058 RID: 16472
	private HashSet<T> queuedItems;

	// Token: 0x04004059 RID: 16473
	private Queue<T> queue;
}
