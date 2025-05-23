using System;
using UnityEngine;

// Token: 0x020006A6 RID: 1702
[Serializable]
public class Ref<T> where T : class
{
	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x06002A95 RID: 10901 RVA: 0x000D1A33 File Offset: 0x000CFC33
	// (set) Token: 0x06002A96 RID: 10902 RVA: 0x000D1A3B File Offset: 0x000CFC3B
	public T AsT
	{
		get
		{
			return this;
		}
		set
		{
			this._target = value as Object;
		}
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x000D1A50 File Offset: 0x000CFC50
	public static implicit operator bool(Ref<T> r)
	{
		Object @object = ((r != null) ? r._target : null);
		return @object != null && @object != null;
	}

	// Token: 0x06002A98 RID: 10904 RVA: 0x000D1A78 File Offset: 0x000CFC78
	public static implicit operator T(Ref<T> r)
	{
		Object @object = ((r != null) ? r._target : null);
		if (@object == null)
		{
			return default(T);
		}
		if (@object == null)
		{
			return default(T);
		}
		return @object as T;
	}

	// Token: 0x06002A99 RID: 10905 RVA: 0x000D1AC0 File Offset: 0x000CFCC0
	public static implicit operator Object(Ref<T> r)
	{
		Object @object = ((r != null) ? r._target : null);
		if (@object == null)
		{
			return null;
		}
		if (@object == null)
		{
			return null;
		}
		return @object;
	}

	// Token: 0x04002F79 RID: 12153
	[SerializeField]
	private Object _target;
}
