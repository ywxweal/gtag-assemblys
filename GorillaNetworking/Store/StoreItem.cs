using System;
using System.IO;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C93 RID: 3219
	[Serializable]
	public class StoreItem
	{
		// Token: 0x06004FCB RID: 20427 RVA: 0x0017C2F8 File Offset: 0x0017A4F8
		public static void SerializeItemsAsJSON(StoreItem[] items)
		{
			string text = "";
			foreach (StoreItem storeItem in items)
			{
				text = text + JsonUtility.ToJson(storeItem) + ";";
			}
			Debug.LogError(text);
			File.WriteAllText(Application.dataPath + "/Resources/StoreItems/FeaturedStoreItemsList.json", text);
		}

		// Token: 0x06004FCC RID: 20428 RVA: 0x0017C34C File Offset: 0x0017A54C
		public static void ConvertCosmeticItemToSToreItem(CosmeticsController.CosmeticItem cosmeticItem, ref StoreItem storeItem)
		{
			storeItem.itemName = cosmeticItem.itemName;
			storeItem.itemCategory = (int)cosmeticItem.itemCategory;
			storeItem.itemPictureResourceString = cosmeticItem.itemPictureResourceString;
			storeItem.displayName = cosmeticItem.displayName;
			storeItem.overrideDisplayName = cosmeticItem.overrideDisplayName;
			storeItem.bundledItems = cosmeticItem.bundledItems;
			storeItem.canTryOn = cosmeticItem.canTryOn;
			storeItem.bothHandsHoldable = cosmeticItem.bothHandsHoldable;
			storeItem.AssetBundleName = "";
			storeItem.bUsesMeshAtlas = cosmeticItem.bUsesMeshAtlas;
			storeItem.MeshResourceName = cosmeticItem.meshResourceString;
			storeItem.MeshAtlasResourceName = cosmeticItem.meshAtlasResourceString;
			storeItem.MaterialResrouceName = cosmeticItem.materialResourceString;
		}

		// Token: 0x040052D3 RID: 21203
		public string itemName = "";

		// Token: 0x040052D4 RID: 21204
		public int itemCategory;

		// Token: 0x040052D5 RID: 21205
		public string itemPictureResourceString = "";

		// Token: 0x040052D6 RID: 21206
		public string displayName = "";

		// Token: 0x040052D7 RID: 21207
		public string overrideDisplayName = "";

		// Token: 0x040052D8 RID: 21208
		public string[] bundledItems = new string[0];

		// Token: 0x040052D9 RID: 21209
		public bool canTryOn;

		// Token: 0x040052DA RID: 21210
		public bool bothHandsHoldable;

		// Token: 0x040052DB RID: 21211
		public string AssetBundleName = "";

		// Token: 0x040052DC RID: 21212
		public bool bUsesMeshAtlas;

		// Token: 0x040052DD RID: 21213
		public string MeshAtlasResourceName = "";

		// Token: 0x040052DE RID: 21214
		public string MeshResourceName = "";

		// Token: 0x040052DF RID: 21215
		public string MaterialResrouceName = "";

		// Token: 0x040052E0 RID: 21216
		public Vector3 translationOffset = Vector3.zero;

		// Token: 0x040052E1 RID: 21217
		public Vector3 rotationOffset = Vector3.zero;

		// Token: 0x040052E2 RID: 21218
		public Vector3 scale = Vector3.one;
	}
}
