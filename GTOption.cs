using System;
using UnityEngine;

// Token: 0x020001DA RID: 474
[Serializable]
public struct GTOption<T>
{
	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000B1B RID: 2843 RVA: 0x0003B93E File Offset: 0x00039B3E
	public T ResolvedValue
	{
		get
		{
			if (!this.enabled)
			{
				return this.defaultValue;
			}
			return this.value;
		}
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0003B955 File Offset: 0x00039B55
	public GTOption(T defaultValue)
	{
		this.enabled = false;
		this.value = defaultValue;
		this.defaultValue = defaultValue;
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0003B96C File Offset: 0x00039B6C
	public void ResetValue()
	{
		this.value = this.defaultValue;
	}

	// Token: 0x04000D91 RID: 3473
	[Tooltip("When checked, the filter is applied; when unchecked (default), it is ignored.")]
	[SerializeField]
	public bool enabled;

	// Token: 0x04000D92 RID: 3474
	[SerializeField]
	public T value;

	// Token: 0x04000D93 RID: 3475
	[NonSerialized]
	public readonly T defaultValue;
}
