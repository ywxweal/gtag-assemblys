using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C6C RID: 3180
	public static class ExtensionMethods
	{
		// Token: 0x06004EE3 RID: 20195 RVA: 0x00178198 File Offset: 0x00176398
		public static void SafeInvoke<T>(this Action<T> action, T data)
		{
			try
			{
				if (action != null)
				{
					action(data);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Failure invoking action: {0}", ex));
			}
		}

		// Token: 0x06004EE4 RID: 20196 RVA: 0x001781D4 File Offset: 0x001763D4
		public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = value;
				return;
			}
			dict.Add(key, value);
		}
	}
}
