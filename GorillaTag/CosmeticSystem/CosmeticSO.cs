using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D7E RID: 3454
	[CreateAssetMenu(fileName = "Untitled_CosmeticSO", menuName = "- Gorilla Tag/CosmeticSO", order = 0)]
	public class CosmeticSO : ScriptableObject
	{
		// Token: 0x060055EC RID: 21996 RVA: 0x001A233F File Offset: 0x001A053F
		public void OnEnable()
		{
			this.info.debugCosmeticSOName = base.name;
		}

		// Token: 0x0400595F RID: 22879
		public CosmeticInfoV2 info = new CosmeticInfoV2("UNNAMED");
	}
}
