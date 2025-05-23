using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D7E RID: 3454
	[CreateAssetMenu(fileName = "Untitled_CosmeticSO", menuName = "- Gorilla Tag/CosmeticSO", order = 0)]
	public class CosmeticSO : ScriptableObject
	{
		// Token: 0x060055EB RID: 21995 RVA: 0x001A2267 File Offset: 0x001A0467
		public void OnEnable()
		{
			this.info.debugCosmeticSOName = base.name;
		}

		// Token: 0x0400595E RID: 22878
		public CosmeticInfoV2 info = new CosmeticInfoV2("UNNAMED");
	}
}
