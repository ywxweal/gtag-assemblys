using System;
using System.Collections.Generic;

// Token: 0x0200098A RID: 2442
public static class DictRefTypeUtils
{
	// Token: 0x06003AB8 RID: 15032 RVA: 0x0011916B File Offset: 0x0011736B
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
