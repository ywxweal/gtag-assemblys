using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200022F RID: 559
[CreateAssetMenu(fileName = "WatchableStringSO", menuName = "ScriptableObjects/WatchableStringSO")]
public class WatchableStringSO : ScriptableObject
{
	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000CE9 RID: 3305 RVA: 0x0004427D File Offset: 0x0004247D
	// (set) Token: 0x06000CEA RID: 3306 RVA: 0x00044285 File Offset: 0x00042485
	private string _value { get; set; }

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000CEB RID: 3307 RVA: 0x0004428E File Offset: 0x0004248E
	// (set) Token: 0x06000CEC RID: 3308 RVA: 0x0004429C File Offset: 0x0004249C
	public string Value
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
			foreach (Action<string> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x000442FC File Offset: 0x000424FC
	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<string>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x00044330 File Offset: 0x00042530
	public void AddCallback(Action<string> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			string value = this._value;
			foreach (Action<string> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x000443A0 File Offset: 0x000425A0
	public void RemoveCallback(Action<string> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x000443B5 File Offset: 0x000425B5
	public override string ToString()
	{
		return this.Value;
	}

	// Token: 0x04001051 RID: 4177
	[TextArea]
	public string InitialValue;

	// Token: 0x04001053 RID: 4179
	private EnterPlayID enterPlayID;

	// Token: 0x04001054 RID: 4180
	private List<Action<string>> callbacks;
}
