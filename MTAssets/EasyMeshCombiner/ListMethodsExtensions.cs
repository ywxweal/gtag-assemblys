using System;
using System.Collections.Generic;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000CA2 RID: 3234
	public static class ListMethodsExtensions
	{
		// Token: 0x06005023 RID: 20515 RVA: 0x0017DACC File Offset: 0x0017BCCC
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
