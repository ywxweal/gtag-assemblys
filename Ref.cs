using System;
using UnityEngine;

// Token: 0x020006A6 RID: 1702
[Serializable]
public class Ref<T> where T : class
{
	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x06002A94 RID: 10900 RVA: 0x000D198F File Offset: 0x000CFB8F
	// (set) Token: 0x06002A95 RID: 10901 RVA: 0x000D1997 File Offset: 0x000CFB97
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

	// Token: 0x06002A96 RID: 10902 RVA: 0x000D19AC File Offset: 0x000CFBAC
	public static implicit operator bool(Ref<T> r)
	{
		Object @object = ((r != null) ? r._target : null);
		return @object != null && @object != null;
	}

	// Token: 0x06002A97 RID: 10903 RVA: 0x000D19D4 File Offset: 0x000CFBD4
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

	// Token: 0x06002A98 RID: 10904 RVA: 0x000D1A1C File Offset: 0x000CFC1C
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

	// Token: 0x04002F77 RID: 12151
	[SerializeField]
	private Object _target;
}
