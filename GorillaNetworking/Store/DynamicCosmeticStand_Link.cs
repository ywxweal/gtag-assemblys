using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C87 RID: 3207
	public class DynamicCosmeticStand_Link : MonoBehaviour
	{
		// Token: 0x06004F88 RID: 20360 RVA: 0x0017B138 File Offset: 0x00179338
		public void SetStandType(HeadModel_CosmeticStand.BustType type)
		{
			this.stand.SetStandType(type);
		}

		// Token: 0x06004F89 RID: 20361 RVA: 0x0017B146 File Offset: 0x00179346
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.stand.SpawnItemOntoStand(PlayFabID);
		}

		// Token: 0x06004F8A RID: 20362 RVA: 0x0017B154 File Offset: 0x00179354
		public void SaveCosmeticMountPosition()
		{
			this.stand.UpdateCosmeticsMountPositions();
		}

		// Token: 0x06004F8B RID: 20363 RVA: 0x0017B161 File Offset: 0x00179361
		public void ClearCosmeticItems()
		{
			this.stand.ClearCosmetics();
		}

		// Token: 0x0400529F RID: 21151
		public DynamicCosmeticStand stand;
	}
}
