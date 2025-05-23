using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C6C RID: 3180
	public static class ExtensionMethods
	{
		// Token: 0x06004EE4 RID: 20196 RVA: 0x00178270 File Offset: 0x00176470
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

		// Token: 0x06004EE5 RID: 20197 RVA: 0x001782AC File Offset: 0x001764AC
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
