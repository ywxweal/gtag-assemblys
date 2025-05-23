using System;
using System.Collections.Generic;

// Token: 0x0200098A RID: 2442
public static class DictRefTypeUtils
{
	// Token: 0x06003AB9 RID: 15033 RVA: 0x00119243 File Offset: 0x00117443
	public static void TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value) where TValue : class, new()
	{
		if (dict.TryGetValue(key, out value) && value != null)
		{
			return;
		}
		value = new TValue();
		dict.Add(key, value);
	}
}
