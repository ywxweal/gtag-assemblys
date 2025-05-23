using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C7C RID: 3196
	public class BundleManager : MonoBehaviour
	{
		// Token: 0x06004F2E RID: 20270 RVA: 0x00179296 File Offset: 0x00177496
		private IEnumerable GetStoreBundles()
		{
			List<StoreBundleData> list = new List<StoreBundleData>();
			list.Add(this.nullBundleData);
			list.AddRange(this._bundleScriptableObjects);
			return list;
		}

		// Token: 0x06004F2F RID: 20271 RVA: 0x001792B5 File Offset: 0x001774B5
		public void Awake()
		{
			if (BundleManager.instance == null)
			{
				BundleManager.instance = this;
				return;
			}
			if (BundleManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}

		// Token: 0x06004F30 RID: 20272 RVA: 0x001792EA File Offset: 0x001774EA
		private void Start()
		{
			this.GenerateBundleDictionaries();
			this.Initialize();
		}

		// Token: 0x06004F31 RID: 20273 RVA: 0x001792F8 File Offset: 0x001774F8
		private void Initialize()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				storeBundle.InitializebundleStands();
			}
		}

		// Token: 0x06004F32 RID: 20274 RVA: 0x00179348 File Offset: 0x00177548
		private void ValidateBundleData()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				storeBundle.ValidateBundleData();
			}
		}

		// Token: 0x06004F33 RID: 20275 RVA: 0x00179398 File Offset: 0x00177598
		private void SpawnBundleStands()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					if (bundleStand != null)
					{
						Object.DestroyImmediate(bundleStand.gameObject);
					}
				}
			}
			this._spawnedBundleStands.Clear();
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			this._storeBundles.Clear();
			this._bundleScriptableObjects.Clear();
			BundleStand[] array = Object.FindObjectsOfType<BundleStand>();
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i].gameObject);
			}
			for (int j = 0; j < this.BundleStands.Count; j++)
			{
				if (this.BundleStands[j].spawnLocation == null)
				{
					Debug.LogError("No spawn location set for Bundle Stand " + j.ToString());
				}
				else if (this.BundleStands[j].bundleStand == null)
				{
					Debug.LogError("No Bundle Stand set for Bundle Stand " + j.ToString());
				}
			}
			this.GenerateAllStoreBundleReferences();
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton1))
			{
				this.tryOnBundleButton1 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton2))
			{
				this.tryOnBundleButton2 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton3))
			{
				this.tryOnBundleButton3 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton4))
			{
				this.tryOnBundleButton4 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton5))
			{
				this.tryOnBundleButton4 = this.nullBundleData;
			}
		}

		// Token: 0x06004F34 RID: 20276 RVA: 0x001795AC File Offset: 0x001777AC
		public void ClearEverything()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					if (bundleStand != null)
					{
						Object.DestroyImmediate(bundleStand.gameObject);
					}
				}
			}
			this._spawnedBundleStands.Clear();
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			this._storeBundles.Clear();
			this._bundleScriptableObjects.Clear();
			this.tryOnBundleButton1 = this.nullBundleData;
			this.tryOnBundleButton2 = this.nullBundleData;
			this.tryOnBundleButton3 = this.nullBundleData;
			this.tryOnBundleButton4 = this.nullBundleData;
			this.tryOnBundleButton5 = this.nullBundleData;
			BundleStand[] array = Object.FindObjectsOfType<BundleStand>();
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i].gameObject);
			}
		}

		// Token: 0x06004F35 RID: 20277 RVA: 0x000023F4 File Offset: 0x000005F4
		public void GenerateAllStoreBundleReferences()
		{
		}

		// Token: 0x06004F36 RID: 20278 RVA: 0x001796E0 File Offset: 0x001778E0
		private void AddNewBundleStand(BundleStand bundleStand)
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				if (storeBundle.playfabBundleID == bundleStand._bundleDataReference.playfabBundleID)
				{
					storeBundle.bundleStands.Add(bundleStand);
					return;
				}
			}
			StoreBundle storeBundle2 = new StoreBundle(bundleStand._bundleDataReference);
			storeBundle2.bundleStands.Add(bundleStand);
			this._storeBundles.Add(storeBundle2);
		}

		// Token: 0x06004F37 RID: 20279 RVA: 0x00179778 File Offset: 0x00177978
		public void GenerateBundleDictionaries()
		{
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				this.storeBundlesById.Add(storeBundle.playfabBundleID, storeBundle);
				this.storeBundlesBySKU.Add(storeBundle.bundleSKU, storeBundle);
			}
		}

		// Token: 0x06004F38 RID: 20280 RVA: 0x00179800 File Offset: 0x00177A00
		public void BundlePurchaseButtonPressed(string playFabItemName)
		{
			CosmeticsController.instance.PurchaseBundle(this.storeBundlesById[playFabItemName]);
		}

		// Token: 0x06004F39 RID: 20281 RVA: 0x0017981C File Offset: 0x00177A1C
		public void FixBundles()
		{
			this._storeBundles.Clear();
			for (int i = this._spawnedBundleStands.Count - 1; i >= 0; i--)
			{
				if (this._spawnedBundleStands[i].bundleStand == null)
				{
					this._spawnedBundleStands.RemoveAt(i);
				}
			}
			BundleStand[] array = Object.FindObjectsOfType<BundleStand>();
			for (int j = 0; j < array.Length; j++)
			{
				BundleStand bundle = array[j];
				if (this._spawnedBundleStands.Any((SpawnedBundle x) => x.spawnLocationPath == bundle.transform.parent.gameObject.GetPath(3)))
				{
					SpawnedBundle spawnedBundle = this._spawnedBundleStands.First((SpawnedBundle x) => x.spawnLocationPath == bundle.transform.parent.gameObject.GetPath(3));
					if (spawnedBundle != null && spawnedBundle.bundleStand != bundle)
					{
						Object.DestroyImmediate(spawnedBundle.bundleStand.gameObject);
						spawnedBundle.bundleStand = bundle;
					}
				}
				else
				{
					this._spawnedBundleStands.Add(new SpawnedBundle
					{
						spawnLocationPath = bundle.transform.parent.gameObject.GetPath(3),
						bundleStand = bundle
					});
				}
			}
			this.GenerateAllStoreBundleReferences();
		}

		// Token: 0x06004F3A RID: 20282 RVA: 0x00179947 File Offset: 0x00177B47
		public StoreBundleData[] GetTryOnButtons()
		{
			return new StoreBundleData[] { this.tryOnBundleButton1, this.tryOnBundleButton2, this.tryOnBundleButton3, this.tryOnBundleButton4, this.tryOnBundleButton5 };
		}

		// Token: 0x06004F3B RID: 20283 RVA: 0x0017997C File Offset: 0x00177B7C
		public void NotifyBundleOfErrorByPlayFabID(string ItemId)
		{
			StoreBundle storeBundle;
			if (this.storeBundlesById.TryGetValue(ItemId, out storeBundle))
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					bundleStand.ErrorHappened();
				}
			}
		}

		// Token: 0x06004F3C RID: 20284 RVA: 0x001799DC File Offset: 0x00177BDC
		public void NotifyBundleOfErrorBySKU(string ItemSKU)
		{
			StoreBundle storeBundle;
			if (this.storeBundlesBySKU.TryGetValue(ItemSKU, out storeBundle))
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					bundleStand.ErrorHappened();
				}
			}
		}

		// Token: 0x06004F3D RID: 20285 RVA: 0x00179A3C File Offset: 0x00177C3C
		public void MarkBundleOwnedByPlayFabID(string ItemId)
		{
			if (this.storeBundlesById.ContainsKey(ItemId))
			{
				this.storeBundlesById[ItemId].isOwned = true;
				foreach (BundleStand bundleStand in this.storeBundlesById[ItemId].bundleStands)
				{
					bundleStand.NotifyAlreadyOwn();
				}
			}
		}

		// Token: 0x06004F3E RID: 20286 RVA: 0x00179AB8 File Offset: 0x00177CB8
		public void MarkBundleOwnedBySKU(string SKU)
		{
			if (this.storeBundlesBySKU.ContainsKey(SKU))
			{
				this.storeBundlesBySKU[SKU].isOwned = true;
				foreach (BundleStand bundleStand in this.storeBundlesBySKU[SKU].bundleStands)
				{
					bundleStand.NotifyAlreadyOwn();
				}
			}
		}

		// Token: 0x06004F3F RID: 20287 RVA: 0x00179B34 File Offset: 0x00177D34
		public void CheckIfBundlesOwned()
		{
			foreach (StoreBundle storeBundle in this.storeBundlesById.Values)
			{
				if (storeBundle.isOwned)
				{
					foreach (BundleStand bundleStand in storeBundle.bundleStands)
					{
						bundleStand.NotifyAlreadyOwn();
					}
				}
			}
		}

		// Token: 0x06004F40 RID: 20288 RVA: 0x00179BCC File Offset: 0x00177DCC
		public void PressTryOnBundleButton(TryOnBundleButton pressedTryOnBundleButton, bool isLeftHand)
		{
			if (this._tryOnBundlesStand.IsNotNull())
			{
				this._tryOnBundlesStand.PressTryOnBundleButton(pressedTryOnBundleButton, isLeftHand);
			}
		}

		// Token: 0x06004F41 RID: 20289 RVA: 0x00179BE8 File Offset: 0x00177DE8
		public void PressPurchaseTryOnBundleButton()
		{
			this._tryOnBundlesStand.PurchaseButtonPressed();
		}

		// Token: 0x06004F42 RID: 20290 RVA: 0x00179BF5 File Offset: 0x00177DF5
		public void UpdateBundlePrice(string productSku, string productFormattedPrice)
		{
			if (this.storeBundlesBySKU.ContainsKey(productSku))
			{
				this.storeBundlesBySKU[productSku].TryUpdatePrice(productFormattedPrice);
			}
		}

		// Token: 0x06004F43 RID: 20291 RVA: 0x00179C18 File Offset: 0x00177E18
		public void CheckForNoPriceBundlesAndDefaultPrice()
		{
			foreach (KeyValuePair<string, StoreBundle> keyValuePair in this.storeBundlesBySKU)
			{
				string text;
				StoreBundle storeBundle;
				keyValuePair.Deconstruct(out text, out storeBundle);
				StoreBundle storeBundle2 = storeBundle;
				if (!storeBundle2.HasPrice)
				{
					storeBundle2.TryUpdatePrice(null);
				}
			}
		}

		// Token: 0x0400525A RID: 21082
		public static volatile BundleManager instance;

		// Token: 0x0400525B RID: 21083
		[FormerlySerializedAs("_TryOnBundlesStand")]
		public TryOnBundlesStand _tryOnBundlesStand;

		// Token: 0x0400525C RID: 21084
		[SerializeField]
		private StoreBundleData nullBundleData;

		// Token: 0x0400525D RID: 21085
		private List<StoreBundleData> _bundleScriptableObjects = new List<StoreBundleData>();

		// Token: 0x0400525E RID: 21086
		[SerializeField]
		private List<StoreBundle> _storeBundles = new List<StoreBundle>();

		// Token: 0x0400525F RID: 21087
		[FormerlySerializedAs("_SpawnedBundleStands")]
		[SerializeField]
		private List<SpawnedBundle> _spawnedBundleStands = new List<SpawnedBundle>();

		// Token: 0x04005260 RID: 21088
		public Dictionary<string, StoreBundle> storeBundlesById = new Dictionary<string, StoreBundle>();

		// Token: 0x04005261 RID: 21089
		public Dictionary<string, StoreBundle> storeBundlesBySKU = new Dictionary<string, StoreBundle>();

		// Token: 0x04005262 RID: 21090
		[Header("Enable Advanced Search window in your settings to easily see all bundle prefabs")]
		[SerializeField]
		private List<BundleManager.BundleStandSpawn> BundleStands = new List<BundleManager.BundleStandSpawn>();

		// Token: 0x04005263 RID: 21091
		[SerializeField]
		private StoreBundleData tryOnBundleButton1;

		// Token: 0x04005264 RID: 21092
		[SerializeField]
		private StoreBundleData tryOnBundleButton2;

		// Token: 0x04005265 RID: 21093
		[SerializeField]
		private StoreBundleData tryOnBundleButton3;

		// Token: 0x04005266 RID: 21094
		[SerializeField]
		private StoreBundleData tryOnBundleButton4;

		// Token: 0x04005267 RID: 21095
		[SerializeField]
		private StoreBundleData tryOnBundleButton5;

		// Token: 0x02000C7D RID: 3197
		[Serializable]
		public class BundleStandSpawn
		{
			// Token: 0x06004F45 RID: 20293 RVA: 0x00179CD9 File Offset: 0x00177ED9
			private static IEnumerable GetEndCapSpawnPoints()
			{
				return from x in Object.FindObjectsOfType<EndCapSpawnPoint>()
					select new ValueDropdownItem(string.Concat(new string[]
					{
						x.transform.parent.parent.name,
						"/",
						x.transform.parent.name,
						"/",
						x.name
					}), x);
			}

			// Token: 0x04005268 RID: 21096
			public EndCapSpawnPoint spawnLocation;

			// Token: 0x04005269 RID: 21097
			public BundleStand bundleStand;
		}
	}
}
