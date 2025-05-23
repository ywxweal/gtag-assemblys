using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D23 RID: 3363
	[CreateAssetMenu(fileName = "WatchableIntSO", menuName = "ScriptableObjects/WatchableIntSO")]
	public class WatchableIntSO : WatchableGenericSO<int>
	{
		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x06005411 RID: 21521 RVA: 0x001976FA File Offset: 0x001958FA
		private int currentValue
		{
			get
			{
				return base.Value;
			}
		}
	}
}
