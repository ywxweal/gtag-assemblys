using System;
using UnityEngine;

// Token: 0x02000775 RID: 1909
[Serializable]
public class OptionalRef<T> where T : Object
{
	// Token: 0x170004BA RID: 1210
	// (get) Token: 0x06002F8F RID: 12175 RVA: 0x000ECEFD File Offset: 0x000EB0FD
	// (set) Token: 0x06002F90 RID: 12176 RVA: 0x000ECF05 File Offset: 0x000EB105
	public bool enabled
	{
		get
		{
			return this._enabled;
		}
		set
		{
			this._enabled = value;
		}
	}

	// Token: 0x170004BB RID: 1211
	// (get) Token: 0x06002F91 RID: 12177 RVA: 0x000ECF10 File Offset: 0x000EB110
	// (set) Token: 0x06002F92 RID: 12178 RVA: 0x000ECF38 File Offset: 0x000EB138
	public T Value
	{
		get
		{
			if (this)
			{
				return this._target;
			}
			return default(T);
		}
		set
		{
			this._target = (value ? value : default(T));
		}
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x000ECF64 File Offset: 0x000EB164
	public static implicit operator bool(OptionalRef<T> r)
	{
		if (r == null)
		{
			return false;
		}
		if (!r._enabled)
		{
			return false;
		}
		Object @object = r._target;
		return @object != null && @object;
	}

	// Token: 0x06002F94 RID: 12180 RVA: 0x000ECF98 File Offset: 0x000EB198
	public static implicit operator T(OptionalRef<T> r)
	{
		if (r == null)
		{
			return default(T);
		}
		if (!r._enabled)
		{
			return default(T);
		}
		Object @object = r._target;
		if (@object == null)
		{
			return default(T);
		}
		if (!@object)
		{
			return default(T);
		}
		return @object as T;
	}

	// Token: 0x06002F95 RID: 12181 RVA: 0x000ECFFC File Offset: 0x000EB1FC
	public static implicit operator Object(OptionalRef<T> r)
	{
		if (r == null)
		{
			return null;
		}
		if (!r._enabled)
		{
			return null;
		}
		Object @object = r._target;
		if (@object == null)
		{
			return null;
		}
		if (!@object)
		{
			return null;
		}
		return @object;
	}

	// Token: 0x0400361A RID: 13850
	[SerializeField]
	private bool _enabled;

	// Token: 0x0400361B RID: 13851
	[SerializeField]
	private T _target;
}
