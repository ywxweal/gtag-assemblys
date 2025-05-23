using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class NonCosmeticHandItem : MonoBehaviour
{
	// Token: 0x06001170 RID: 4464 RVA: 0x00053F67 File Offset: 0x00052167
	public void EnableItem(bool enable)
	{
		if (this.itemPrefab)
		{
			this.itemPrefab.gameObject.SetActive(enable);
		}
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x06001171 RID: 4465 RVA: 0x00053F87 File Offset: 0x00052187
	public bool IsEnabled
	{
		get
		{
			return this.itemPrefab && this.itemPrefab.gameObject.activeSelf;
		}
	}

	// Token: 0x040013A4 RID: 5028
	public CosmeticsController.CosmeticSlots cosmeticSlots;

	// Token: 0x040013A5 RID: 5029
	public GameObject itemPrefab;
}
