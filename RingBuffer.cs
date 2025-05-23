using System;
using System.Collections.Generic;

// Token: 0x0200077A RID: 1914
public class RingBuffer<T>
{
	// Token: 0x170004C1 RID: 1217
	// (get) Token: 0x06002FAB RID: 12203 RVA: 0x000ED31E File Offset: 0x000EB51E
	public int Size
	{
		get
		{
			return this._size;
		}
	}

	// Token: 0x170004C2 RID: 1218
	// (get) Token: 0x06002FAC RID: 12204 RVA: 0x000ED326 File Offset: 0x000EB526
	public int Capacity
	{
		get
		{
			return this._capacity;
		}
	}

	// Token: 0x170004C3 RID: 1219
	// (get) Token: 0x06002FAD RID: 12205 RVA: 0x000ED32E File Offset: 0x000EB52E
	public bool IsFull
	{
		get
		{
			return this._size == this._capacity;
		}
	}

	// Token: 0x170004C4 RID: 1220
	// (get) Token: 0x06002FAE RID: 12206 RVA: 0x000ED33E File Offset: 0x000EB53E
	public bool IsEmpty
	{
		get
		{
			return this._size == 0;
		}
	}

	// Token: 0x06002FAF RID: 12207 RVA: 0x000ED349 File Offset: 0x000EB549
	public RingBuffer(int capacity)
	{
		if (capacity < 1)
		{
			throw new ArgumentException("Can't be zero or negative", "capacity");
		}
		this._size = 0;
		this._capacity = capacity;
		this._items = new T[capacity];
	}

	// Token: 0x06002FB0 RID: 12208 RVA: 0x000ED37F File Offset: 0x000EB57F
	public RingBuffer(IList<T> list)
		: this(list.Count)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		list.CopyTo(this._items, 0);
	}

	// Token: 0x06002FB1 RID: 12209 RVA: 0x000ED3A8 File Offset: 0x000EB5A8
	public ref T PeekFirst()
	{
		return ref this._items[this._head];
	}

	// Token: 0x06002FB2 RID: 12210 RVA: 0x000ED3BB File Offset: 0x000EB5BB
	public ref T PeekLast()
	{
		return ref this._items[this._tail];
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x000ED3D0 File Offset: 0x000EB5D0
	public bool Push(T item)
	{
		if (this._size == this._capacity)
		{
			return false;
		}
		this._items[this._tail] = item;
		this._tail = (this._tail + 1) % this._capacity;
		this._size++;
		return true;
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x000ED424 File Offset: 0x000EB624
	public T Pop()
	{
		if (this._size == 0)
		{
			return default(T);
		}
		T t = this._items[this._head];
		this._head = (this._head + 1) % this._capacity;
		this._size--;
		return t;
	}

	// Token: 0x06002FB5 RID: 12213 RVA: 0x000ED478 File Offset: 0x000EB678
	public bool TryPop(out T item)
	{
		if (this._size == 0)
		{
			item = default(T);
			return false;
		}
		item = this._items[this._head];
		this._head = (this._head + 1) % this._capacity;
		this._size--;
		return true;
	}

	// Token: 0x06002FB6 RID: 12214 RVA: 0x000ED4D1 File Offset: 0x000EB6D1
	public void Clear()
	{
		this._head = 0;
		this._tail = 0;
		this._size = 0;
		Array.Clear(this._items, 0, this._capacity);
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x000ED4FA File Offset: 0x000EB6FA
	public bool TryGet(int i, out T item)
	{
		if (this._size == 0)
		{
			item = default(T);
			return false;
		}
		item = this._items[this._head + i % this._size];
		return true;
	}

	// Token: 0x06002FB8 RID: 12216 RVA: 0x000ED52E File Offset: 0x000EB72E
	public ArraySegment<T> AsSegment()
	{
		return new ArraySegment<T>(this._items);
	}

	// Token: 0x0400362D RID: 13869
	private T[] _items;

	// Token: 0x0400362E RID: 13870
	private int _head;

	// Token: 0x0400362F RID: 13871
	private int _tail;

	// Token: 0x04003630 RID: 13872
	private int _size;

	// Token: 0x04003631 RID: 13873
	private readonly int _capacity;
}
