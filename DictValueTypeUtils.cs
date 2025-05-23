using System;
using System.Collections.Generic;

// Token: 0x0200098B RID: 2443
public static class DictValueTypeUtils
{
	// Token: 0x06003AB9 RID: 15033 RVA: 0x0011919D File Offset: 0x0011739D
	public static void TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value) where TValue : struct
	{
		if (dict.TryGetValue(key, out value))
		{
			return;
		}
		value = default(TValue);
		dict.Add(key, value);
	}
}
