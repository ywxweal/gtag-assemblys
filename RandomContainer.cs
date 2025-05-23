using System;
using UnityEngine;

// Token: 0x0200091E RID: 2334
public abstract class RandomContainer<T> : ScriptableObject
{
	// Token: 0x17000594 RID: 1428
	// (get) Token: 0x060038EC RID: 14572 RVA: 0x00112818 File Offset: 0x00110A18
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x17000595 RID: 1429
	// (get) Token: 0x060038ED RID: 14573 RVA: 0x00112820 File Offset: 0x00110A20
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x060038EE RID: 14574 RVA: 0x00112828 File Offset: 0x00110A28
	public void ResetRandom(int? seedValue = null)
	{
		if (!this.staticSeed)
		{
			this._seed = seedValue ?? StaticHash.Compute(DateTime.UtcNow.Ticks);
		}
		else
		{
			this._seed = this.seed;
		}
		this._rnd = new SRand(this._seed);
	}

	// Token: 0x060038EF RID: 14575 RVA: 0x00112888 File Offset: 0x00110A88
	public void Reset()
	{
		this.ResetRandom(null);
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x060038F0 RID: 14576 RVA: 0x001128B7 File Offset: 0x00110AB7
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x060038F1 RID: 14577 RVA: 0x001128BF File Offset: 0x00110ABF
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x060038F2 RID: 14578 RVA: 0x001128D0 File Offset: 0x00110AD0
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		return t;
	}

	// Token: 0x04003E23 RID: 15907
	public T[] items = new T[0];

	// Token: 0x04003E24 RID: 15908
	public int seed;

	// Token: 0x04003E25 RID: 15909
	public bool staticSeed;

	// Token: 0x04003E26 RID: 15910
	public bool distinct = true;

	// Token: 0x04003E27 RID: 15911
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x04003E28 RID: 15912
	[NonSerialized]
	private T _lastItem;

	// Token: 0x04003E29 RID: 15913
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x04003E2A RID: 15914
	[NonSerialized]
	private SRand _rnd;
}
