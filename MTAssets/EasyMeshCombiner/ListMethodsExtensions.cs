using System;
using System.Collections.Generic;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000CA2 RID: 3234
	public static class ListMethodsExtensions
	{
		// Token: 0x06005022 RID: 20514 RVA: 0x0017D9F4 File Offset: 0x0017BBF4
		public static void RemoveAllNullItems<T>(this List<T> list)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] == null)
				{
					list.RemoveAt(i);
				}
			}
		}
	}
}
