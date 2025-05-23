using System;
using UnityEngine;

// Token: 0x020000A1 RID: 161
[Serializable]
public struct KeyValueStringPair
{
	// Token: 0x06000406 RID: 1030 RVA: 0x00017CD1 File Offset: 0x00015ED1
	public KeyValueStringPair(string key, string value)
	{
		this.Key = key;
		this.Value = value;
	}

	// Token: 0x0400047D RID: 1149
	public string Key;

	// Token: 0x0400047E RID: 1150
	[Multiline]
	public string Value;
}
