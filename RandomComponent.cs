using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200091D RID: 2333
public abstract class RandomComponent<T> : MonoBehaviour
{
	// Token: 0x17000592 RID: 1426
	// (get) Token: 0x060038E2 RID: 14562 RVA: 0x001125E5 File Offset: 0x001107E5
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x17000593 RID: 1427
	// (get) Token: 0x060038E3 RID: 14563 RVA: 0x001125ED File Offset: 0x001107ED
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x060038E4 RID: 14564 RVA: 0x001125F8 File Offset: 0x001107F8
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

	// Token: 0x060038E5 RID: 14565 RVA: 0x00112658 File Offset: 0x00110858
	public void Reset()
	{
		this.ResetRandom(null);
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x060038E6 RID: 14566 RVA: 0x00112687 File Offset: 0x00110887
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x060038E7 RID: 14567 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnNextItem(T item)
	{
	}

	// Token: 0x060038E8 RID: 14568 RVA: 0x0011268F File Offset: 0x0011088F
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x060038E9 RID: 14569 RVA: 0x001126A0 File Offset: 0x001108A0
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		this.OnNextItem(t);
		UnityEvent<T> unityEvent = this.onNextItem;
		if (unityEvent != null)
		{
			unityEvent.Invoke(t);
		}
		return t;
	}

	// Token: 0x04003E19 RID: 15897
	public T[] items = new T[0];

	// Token: 0x04003E1A RID: 15898
	public int seed;

	// Token: 0x04003E1B RID: 15899
	public bool staticSeed;

	// Token: 0x04003E1C RID: 15900
	public bool distinct = true;

	// Token: 0x04003E1D RID: 15901
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x04003E1E RID: 15902
	[NonSerialized]
	private T _lastItem;

	// Token: 0x04003E1F RID: 15903
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x04003E20 RID: 15904
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x04003E21 RID: 15905
	public UnityEvent<T> onNextItem;
}
