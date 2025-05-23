using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200022E RID: 558
public class WatchableGenericSO<T> : ScriptableObject
{
	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000CE1 RID: 3297 RVA: 0x00044143 File Offset: 0x00042343
	// (set) Token: 0x06000CE2 RID: 3298 RVA: 0x0004414B File Offset: 0x0004234B
	private T _value { get; set; }

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000CE3 RID: 3299 RVA: 0x00044154 File Offset: 0x00042354
	// (set) Token: 0x06000CE4 RID: 3300 RVA: 0x00044164 File Offset: 0x00042364
	public T Value
	{
		get
		{
			this.EnsureInitialized();
			return this._value;
		}
		set
		{
			this.EnsureInitialized();
			this._value = value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x000441C4 File Offset: 0x000423C4
	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<T>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x000441F8 File Offset: 0x000423F8
	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			T value = this._value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x00044268 File Offset: 0x00042468
	public void RemoveCallback(Action<T> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	// Token: 0x0400104D RID: 4173
	public T InitialValue;

	// Token: 0x0400104F RID: 4175
	private EnterPlayID enterPlayID;

	// Token: 0x04001050 RID: 4176
	private List<Action<T>> callbacks;
}
