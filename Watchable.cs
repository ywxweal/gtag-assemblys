using System;
using System.Collections.Generic;

// Token: 0x0200022D RID: 557
public class Watchable<T>
{
	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000CDB RID: 3291 RVA: 0x00044034 File Offset: 0x00042234
	// (set) Token: 0x06000CDC RID: 3292 RVA: 0x0004403C File Offset: 0x0004223C
	public T value
	{
		get
		{
			return this._value;
		}
		set
		{
			T value2 = this._value;
			this._value = value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x0004409C File Offset: 0x0004229C
	public Watchable()
	{
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x000440AF File Offset: 0x000422AF
	public Watchable(T initial)
	{
		this._value = initial;
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x000440CC File Offset: 0x000422CC
	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			foreach (Action<T> action in this.callbacks)
			{
				action(this._value);
			}
		}
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x00044134 File Offset: 0x00042334
	public void RemoveCallback(Action<T> callback)
	{
		this.callbacks.Remove(callback);
	}

	// Token: 0x0400104B RID: 4171
	private T _value;

	// Token: 0x0400104C RID: 4172
	private List<Action<T>> callbacks = new List<Action<T>>();
}
