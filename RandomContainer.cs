using System;
using UnityEngine;

// Token: 0x0200091E RID: 2334
public abstract class RandomContainer<T> : ScriptableObject
{
	// Token: 0x17000594 RID: 1428
	// (get) Token: 0x060038EB RID: 14571 RVA: 0x00112740 File Offset: 0x00110940
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x17000595 RID: 1429
	// (get) Token: 0x060038EC RID: 14572 RVA: 0x00112748 File Offset: 0x00110948
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x060038ED RID: 14573 RVA: 0x00112750 File Offset: 0x00110950
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

	// Token: 0x060038EE RID: 14574 RVA: 0x001127B0 File Offset: 0x001109B0
	public void Reset()
	{
		this.ResetRandom(null);
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x060038EF RID: 14575 RVA: 0x001127DF File Offset: 0x001109DF
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x060038F0 RID: 14576 RVA: 0x001127E7 File Offset: 0x001109E7
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x060038F1 RID: 14577 RVA: 0x001127F8 File Offset: 0x001109F8
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		return t;
	}

	// Token: 0x04003E22 RID: 15906
	public T[] items = new T[0];

	// Token: 0x04003E23 RID: 15907
	public int seed;

	// Token: 0x04003E24 RID: 15908
	public bool staticSeed;

	// Token: 0x04003E25 RID: 15909
	public bool distinct = true;

	// Token: 0x04003E26 RID: 15910
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x04003E27 RID: 15911
	[NonSerialized]
	private T _lastItem;

	// Token: 0x04003E28 RID: 15912
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x04003E29 RID: 15913
	[NonSerialized]
	private SRand _rnd;
}
