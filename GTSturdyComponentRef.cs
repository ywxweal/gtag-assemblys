using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020001AA RID: 426
[Serializable]
public struct GTSturdyComponentRef<T> where T : Component
{
	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000A7B RID: 2683 RVA: 0x00039C99 File Offset: 0x00037E99
	// (set) Token: 0x06000A7C RID: 2684 RVA: 0x00039CA1 File Offset: 0x00037EA1
	public Transform BaseXform
	{
		get
		{
			return this._baseXform;
		}
		set
		{
			this._baseXform = value;
		}
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000A7D RID: 2685 RVA: 0x00039CAC File Offset: 0x00037EAC
	// (set) Token: 0x06000A7E RID: 2686 RVA: 0x00039D1B File Offset: 0x00037F1B
	public T Value
	{
		get
		{
			if (!this._value)
			{
				return this._value;
			}
			if (string.IsNullOrEmpty(this._relativePath))
			{
				return default(T);
			}
			Transform transform;
			if (!this._baseXform.TryFindByPath(this._relativePath, out transform, false))
			{
				return default(T);
			}
			this._value = transform.GetComponent<T>();
			return this._value;
		}
		set
		{
			this._value = value;
			this._relativePath = ((!value) ? this._baseXform.GetRelativePath(value.transform) : string.Empty);
		}
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x00039D54 File Offset: 0x00037F54
	public static implicit operator T(GTSturdyComponentRef<T> sturdyRef)
	{
		return sturdyRef.Value;
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x00039D60 File Offset: 0x00037F60
	public static implicit operator GTSturdyComponentRef<T>(T component)
	{
		return new GTSturdyComponentRef<T>
		{
			Value = component
		};
	}

	// Token: 0x04000CBA RID: 3258
	[SerializeField]
	private T _value;

	// Token: 0x04000CBB RID: 3259
	[SerializeField]
	private string _relativePath;

	// Token: 0x04000CBC RID: 3260
	[SerializeField]
	private Transform _baseXform;
}
