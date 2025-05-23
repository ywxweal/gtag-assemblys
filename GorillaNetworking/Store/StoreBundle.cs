using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C84 RID: 3204
	[Serializable]
	public class StoreBundle
	{
		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x06004F68 RID: 20328 RVA: 0x0017A16B File Offset: 0x0017836B
		public string playfabBundleID
		{
			get
			{
				return this._storeBundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x06004F69 RID: 20329 RVA: 0x0017A178 File Offset: 0x00178378
		public string bundleSKU
		{
			get
			{
				return this._storeBundleDataReference.bundleSKU;
			}
		}

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x06004F6A RID: 20330 RVA: 0x0017A185 File Offset: 0x00178385
		public Sprite bundleImage
		{
			get
			{
				return this._storeBundleDataReference.bundleImage;
			}
		}

		// Token: 0x170007E3 RID: 2019
		// (get) Token: 0x06004F6B RID: 20331 RVA: 0x0017A192 File Offset: 0x00178392
		public string price
		{
			get
			{
				return this._price;
			}
		}

		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x06004F6C RID: 20332 RVA: 0x0017A19C File Offset: 0x0017839C
		public string bundleName
		{
			get
			{
				if (this._bundleName.IsNullOrEmpty())
				{
					int num = CosmeticsController.instance.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.playfabBundleID == x.itemName);
					if (num > -1)
					{
						if (!CosmeticsController.instance.allCosmetics[num].overrideDisplayName.IsNullOrEmpty())
						{
							this._bundleName = CosmeticsController.instance.allCosmetics[num].overrideDisplayName;
						}
						else
						{
							this._bundleName = CosmeticsController.instance.allCosmetics[num].displayName;
						}
					}
					else
					{
						this._bundleName = "NULL_BUNDLE_NAME";
					}
				}
				return this._bundleName;
			}
		}

		// Token: 0x170007E5 RID: 2021
		// (get) Token: 0x06004F6D RID: 20333 RVA: 0x0017A248 File Offset: 0x00178448
		public bool HasPrice
		{
			get
			{
				return !string.IsNullOrEmpty(this.price) && this.price != StoreBundle.defaultPrice;
			}
		}

		// Token: 0x170007E6 RID: 2022
		// (get) Token: 0x06004F6E RID: 20334 RVA: 0x0017A269 File Offset: 0x00178469
		public string bundleDescriptionText
		{
			get
			{
				return this._storeBundleDataReference.bundleDescriptionText;
			}
		}

		// Token: 0x06004F6F RID: 20335 RVA: 0x0017A278 File Offset: 0x00178478
		public StoreBundle()
		{
			this.isOwned = false;
			this.bundleStands = new List<BundleStand>();
		}

		// Token: 0x06004F70 RID: 20336 RVA: 0x0017A2CC File Offset: 0x001784CC
		public StoreBundle(StoreBundleData data)
		{
			this.isOwned = false;
			this.bundleStands = new List<BundleStand>();
			this._storeBundleDataReference = data;
		}

		// Token: 0x06004F71 RID: 20337 RVA: 0x0017A324 File Offset: 0x00178524
		public void InitializebundleStands()
		{
			foreach (BundleStand bundleStand in this.bundleStands)
			{
				bundleStand.UpdateDescriptionText(this.bundleDescriptionText);
				bundleStand.InitializeEventListeners();
			}
		}

		// Token: 0x06004F72 RID: 20338 RVA: 0x0017A380 File Offset: 0x00178580
		public void TryUpdatePrice(uint bundlePrice)
		{
			this.TryUpdatePrice((bundlePrice / 100m).ToString());
		}

		// Token: 0x06004F73 RID: 20339 RVA: 0x0017A3B0 File Offset: 0x001785B0
		public void TryUpdatePrice(string bundlePrice = null)
		{
			if (!string.IsNullOrEmpty(bundlePrice))
			{
				decimal num;
				this._price = (decimal.TryParse(bundlePrice, out num) ? (StoreBundle.defaultCurrencySymbol + bundlePrice) : bundlePrice);
			}
			this.UpdatePurchaseButtonText();
		}

		// Token: 0x06004F74 RID: 20340 RVA: 0x0017A3EC File Offset: 0x001785EC
		public void UpdatePurchaseButtonText()
		{
			this.purchaseButtonText = string.Format(this.purchaseButtonStringFormat, this.bundleName, this.price);
			foreach (BundleStand bundleStand in this.bundleStands)
			{
				bundleStand.UpdatePurchaseButtonText(this.purchaseButtonText);
			}
		}

		// Token: 0x06004F75 RID: 20341 RVA: 0x0017A460 File Offset: 0x00178660
		public void ValidateBundleData()
		{
			if (this._storeBundleDataReference == null)
			{
				Debug.LogError("StoreBundleData is null");
				foreach (BundleStand bundleStand in this.bundleStands)
				{
					if (bundleStand == null)
					{
						Debug.LogError("BundleStand is null");
					}
					else if (bundleStand._bundleDataReference != null)
					{
						this._storeBundleDataReference = bundleStand._bundleDataReference;
						Debug.LogError("BundleStand StoreBundleData is not equal to StoreBundle StoreBundleData");
					}
				}
			}
			if (this._storeBundleDataReference == null)
			{
				Debug.LogError("StoreBundleData is null");
				return;
			}
			if (this._storeBundleDataReference.playfabBundleID.IsNullOrEmpty())
			{
				Debug.LogError("playfabBundleID is null");
			}
			if (this._storeBundleDataReference.bundleSKU.IsNullOrEmpty())
			{
				Debug.LogError("bundleSKU is null");
			}
			if (this._storeBundleDataReference.bundleImage == null)
			{
				Debug.LogError("bundleImage is null");
			}
			if (this._storeBundleDataReference.bundleDescriptionText.IsNullOrEmpty())
			{
				Debug.LogError("bundleDescriptionText is null");
			}
		}

		// Token: 0x0400527C RID: 21116
		private static readonly string defaultPrice = "$--.--";

		// Token: 0x0400527D RID: 21117
		private static readonly string defaultCurrencySymbol = "$";

		// Token: 0x0400527E RID: 21118
		[NonSerialized]
		public string purchaseButtonStringFormat = "THE {0}\n{1}";

		// Token: 0x0400527F RID: 21119
		[SerializeField]
		public List<BundleStand> bundleStands;

		// Token: 0x04005280 RID: 21120
		public bool isOwned;

		// Token: 0x04005281 RID: 21121
		private string _price = StoreBundle.defaultPrice;

		// Token: 0x04005282 RID: 21122
		private string _bundleName = "";

		// Token: 0x04005283 RID: 21123
		public string purchaseButtonText = "";

		// Token: 0x04005284 RID: 21124
		[FormerlySerializedAs("storeBundleDataReference")]
		[SerializeField]
		[ReadOnly]
		private StoreBundleData _storeBundleDataReference;
	}
}
