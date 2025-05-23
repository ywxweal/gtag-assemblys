using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C87 RID: 3207
	public class DynamicCosmeticStand_Link : MonoBehaviour
	{
		// Token: 0x06004F89 RID: 20361 RVA: 0x0017B210 File Offset: 0x00179410
		public void SetStandType(HeadModel_CosmeticStand.BustType type)
		{
			this.stand.SetStandType(type);
		}

		// Token: 0x06004F8A RID: 20362 RVA: 0x0017B21E File Offset: 0x0017941E
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.stand.SpawnItemOntoStand(PlayFabID);
		}

		// Token: 0x06004F8B RID: 20363 RVA: 0x0017B22C File Offset: 0x0017942C
		public void SaveCosmeticMountPosition()
		{
			this.stand.UpdateCosmeticsMountPositions();
		}

		// Token: 0x06004F8C RID: 20364 RVA: 0x0017B239 File Offset: 0x00179439
		public void ClearCosmeticItems()
		{
			this.stand.ClearCosmetics();
		}

		// Token: 0x040052A0 RID: 21152
		public DynamicCosmeticStand stand;
	}
}
