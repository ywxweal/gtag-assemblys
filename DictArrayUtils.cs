using System;
using System.Collections.Generic;

// Token: 0x0200098C RID: 2444
public static class DictArrayUtils
{
	// Token: 0x06003ABB RID: 15035 RVA: 0x00119296 File Offset: 0x00117496
	public static void TryGetOrAddList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, out List<TValue> list, int capacity)
	{
		if (dict.TryGetValue(key, out list) && list != null)
		{
			return;
		}
		list = new List<TValue>(capacity);
		dict.Add(key, list);
	}

	// Token: 0x06003ABC RID: 15036 RVA: 0x001192B8 File Offset: 0x001174B8
	public static void TryGetOrAddArray<TKey, TValue>(this Dictionary<TKey, TValue[]> dict, TKey key, out TValue[] array, int size)
	{
		if (dict.TryGetValue(key, out array) && array != null)
		{
			return;
		}
		array = new TValue[size];
		dict.Add(key, array);
	}
}
