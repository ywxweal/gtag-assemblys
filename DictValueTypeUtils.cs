using System;
using System.Collections.Generic;

// Token: 0x0200098B RID: 2443
public static class DictValueTypeUtils
{
	// Token: 0x06003ABA RID: 15034 RVA: 0x00119275 File Offset: 0x00117475
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
