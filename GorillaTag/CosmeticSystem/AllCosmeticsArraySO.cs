using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02000D75 RID: 3445
	public class AllCosmeticsArraySO : ScriptableObject
	{
		// Token: 0x060055DE RID: 21982 RVA: 0x001A1FAC File Offset: 0x001A01AC
		public CosmeticSO SearchForCosmeticSO(string playfabId)
		{
			GTDirectAssetRef<CosmeticSO>[] array = this.sturdyAssetRefs;
			for (int i = 0; i < array.Length; i++)
			{
				CosmeticSO cosmeticSO = array[i];
				if (cosmeticSO.info.playFabID == playfabId)
				{
					return cosmeticSO;
				}
			}
			Debug.LogWarning("AllCosmeticsArraySO - SearchForCosmeticSO - No Cosmetic found with playfabId: " + playfabId, this);
			return null;
		}

		// Token: 0x04005930 RID: 22832
		[SerializeField]
		public GTDirectAssetRef<CosmeticSO>[] sturdyAssetRefs;
	}
}
