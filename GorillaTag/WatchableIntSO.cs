using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D23 RID: 3363
	[CreateAssetMenu(fileName = "WatchableIntSO", menuName = "ScriptableObjects/WatchableIntSO")]
	public class WatchableIntSO : WatchableGenericSO<int>
	{
		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x06005410 RID: 21520 RVA: 0x00197622 File Offset: 0x00195822
		private int currentValue
		{
			get
			{
				return base.Value;
			}
		}
	}
}
