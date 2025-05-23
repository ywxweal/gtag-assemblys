using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x020009D6 RID: 2518
public class UniqueQueue<T> : IEnumerable<T>, IEnumerable
{
	// Token: 0x170005E3 RID: 1507
	// (get) Token: 0x06003C3C RID: 15420 RVA: 0x00120063 File Offset: 0x0011E263
	public int Count
	{
		get
		{
			return this.queue.Count;
		}
	}

	// Token: 0x06003C3D RID: 15421 RVA: 0x00120070 File Offset: 0x0011E270
	public UniqueQueue()
	{
		this.queuedItems = new HashSet<T>();
		this.queue = new Queue<T>();
	}

	// Token: 0x06003C3E RID: 15422 RVA: 0x0012008E File Offset: 0x0011E28E
	public UniqueQueue(int capacity)
	{
		this.queuedItems = new HashSet<T>(capacity);
		this.queue = new Queue<T>(capacity);
	}

	// Token: 0x06003C3F RID: 15423 RVA: 0x001200AE File Offset: 0x0011E2AE
	public void Clear()
	{
		this.queuedItems.Clear();
		this.queue.Clear();
	}

	// Token: 0x06003C40 RID: 15424 RVA: 0x001200C6 File Offset: 0x0011E2C6
	public bool Enqueue(T item)
	{
		if (!this.queuedItems.Add(item))
		{
			return false;
		}
		this.queue.Enqueue(item);
		return true;
	}

	// Token: 0x06003C41 RID: 15425 RVA: 0x001200E8 File Offset: 0x0011E2E8
	public T Dequeue()
	{
		T t = this.queue.Dequeue();
		this.queuedItems.Remove(t);
		return t;
	}

	// Token: 0x06003C42 RID: 15426 RVA: 0x0012010F File Offset: 0x0011E30F
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

	// Token: 0x06003C43 RID: 15427 RVA: 0x00120135 File Offset: 0x0011E335
	public T Peek()
	{
		return this.queue.Peek();
	}

	// Token: 0x06003C44 RID: 15428 RVA: 0x00120142 File Offset: 0x0011E342
	public bool Contains(T item)
	{
		return this.queuedItems.Contains(item);
	}

	// Token: 0x06003C45 RID: 15429 RVA: 0x00120150 File Offset: 0x0011E350
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x06003C46 RID: 15430 RVA: 0x00120150 File Offset: 0x0011E350
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x04004059 RID: 16473
	private HashSet<T> queuedItems;

	// Token: 0x0400405A RID: 16474
	private Queue<T> queue;
}
