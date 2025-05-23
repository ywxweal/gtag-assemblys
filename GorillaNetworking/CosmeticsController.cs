using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking.Store;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking
{
	// Token: 0x02000C0E RID: 3086
	public class CosmeticsController : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
	{
		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06004C32 RID: 19506 RVA: 0x00169189 File Offset: 0x00167389
		// (set) Token: 0x06004C33 RID: 19507 RVA: 0x00169191 File Offset: 0x00167391
		public CosmeticInfoV2[] v2_allCosmetics { get; private set; }

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06004C34 RID: 19508 RVA: 0x0016919A File Offset: 0x0016739A
		// (set) Token: 0x06004C35 RID: 19509 RVA: 0x001691A2 File Offset: 0x001673A2
		public bool v2_allCosmeticsInfoAssetRef_isLoaded { get; private set; }

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06004C36 RID: 19510 RVA: 0x001691AB File Offset: 0x001673AB
		// (set) Token: 0x06004C37 RID: 19511 RVA: 0x001691B3 File Offset: 0x001673B3
		public bool v2_isGetCosmeticsPlayCatalogDataWaitingForCallback { get; private set; }

		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x06004C38 RID: 19512 RVA: 0x001691BC File Offset: 0x001673BC
		// (set) Token: 0x06004C39 RID: 19513 RVA: 0x001691C4 File Offset: 0x001673C4
		public bool v2_isCosmeticPlayFabCatalogDataLoaded { get; private set; }

		// Token: 0x06004C3A RID: 19514 RVA: 0x001691CD File Offset: 0x001673CD
		private void V2Awake()
		{
			this._allCosmetics = null;
			base.StartCoroutine(this.V2_allCosmeticsInfoAssetRefSO_LoadCoroutine());
		}

		// Token: 0x06004C3B RID: 19515 RVA: 0x001691E3 File Offset: 0x001673E3
		private IEnumerator V2_allCosmeticsInfoAssetRefSO_LoadCoroutine()
		{
			while (!PlayFabAuthenticator.instance)
			{
				yield return new WaitForSeconds(1f);
			}
			float[] retryWaitTimes = new float[]
			{
				1f, 2f, 4f, 4f, 10f, 10f, 10f, 10f, 10f, 10f,
				10f, 10f, 10f, 10f, 30f
			};
			int retryCount = 0;
			AsyncOperationHandle<AllCosmeticsArraySO> newSysAllCosmeticsAsyncOp;
			for (;;)
			{
				Debug.Log(string.Format("Attempting to load runtime key \"{0}\" ", this.v2_allCosmeticsInfoAssetRef.RuntimeKey) + string.Format("(Attempt: {0})", retryCount + 1));
				newSysAllCosmeticsAsyncOp = this.v2_allCosmeticsInfoAssetRef.LoadAssetAsync();
				yield return newSysAllCosmeticsAsyncOp;
				if (ApplicationQuittingState.IsQuitting)
				{
					break;
				}
				if (!newSysAllCosmeticsAsyncOp.IsValid())
				{
					Debug.LogError("`newSysAllCosmeticsAsyncOp` (should never happen) became invalid some how.");
				}
				if (newSysAllCosmeticsAsyncOp.Status == AsyncOperationStatus.Succeeded)
				{
					goto Block_4;
				}
				Debug.LogError(string.Format("Failed to load \"{0}\". ", this.v2_allCosmeticsInfoAssetRef.RuntimeKey) + "Error: " + newSysAllCosmeticsAsyncOp.OperationException.Message);
				float num = retryWaitTimes[Mathf.Min(retryCount, retryWaitTimes.Length - 1)];
				yield return new WaitForSecondsRealtime(num);
				int num2 = retryCount;
				retryCount = num2 + 1;
				newSysAllCosmeticsAsyncOp = default(AsyncOperationHandle<AllCosmeticsArraySO>);
			}
			yield break;
			Block_4:
			this.V2_allCosmeticsInfoAssetRef_LoadSucceeded(newSysAllCosmeticsAsyncOp.Result);
			yield break;
		}

		// Token: 0x06004C3C RID: 19516 RVA: 0x001691F4 File Offset: 0x001673F4
		private void V2_allCosmeticsInfoAssetRef_LoadSucceeded(AllCosmeticsArraySO allCosmeticsSO)
		{
			this.v2_allCosmetics = new CosmeticInfoV2[allCosmeticsSO.sturdyAssetRefs.Length];
			for (int i = 0; i < allCosmeticsSO.sturdyAssetRefs.Length; i++)
			{
				this.v2_allCosmetics[i] = allCosmeticsSO.sturdyAssetRefs[i].obj.info;
			}
			this._allCosmetics = new List<CosmeticsController.CosmeticItem>(allCosmeticsSO.sturdyAssetRefs.Length);
			for (int j = 0; j < this.v2_allCosmetics.Length; j++)
			{
				CosmeticInfoV2 cosmeticInfoV = this.v2_allCosmetics[j];
				string playFabID = cosmeticInfoV.playFabID;
				this._allCosmeticsDictV2[playFabID] = cosmeticInfoV;
				CosmeticsController.CosmeticItem cosmeticItem = new CosmeticsController.CosmeticItem
				{
					itemName = playFabID,
					itemCategory = cosmeticInfoV.category,
					isHoldable = cosmeticInfoV.hasHoldableParts,
					displayName = playFabID,
					itemPicture = cosmeticInfoV.icon,
					overrideDisplayName = cosmeticInfoV.displayName,
					bothHandsHoldable = cosmeticInfoV.usesBothHandSlots,
					isNullItem = false
				};
				this._allCosmetics.Add(cosmeticItem);
			}
			this.v2_allCosmeticsInfoAssetRef_isLoaded = true;
			Action v2_allCosmeticsInfoAssetRef_OnPostLoad = this.V2_allCosmeticsInfoAssetRef_OnPostLoad;
			if (v2_allCosmeticsInfoAssetRef_OnPostLoad == null)
			{
				return;
			}
			v2_allCosmeticsInfoAssetRef_OnPostLoad();
		}

		// Token: 0x06004C3D RID: 19517 RVA: 0x00169325 File Offset: 0x00167525
		public bool TryGetCosmeticInfoV2(string playFabId, out CosmeticInfoV2 cosmeticInfo)
		{
			return this._allCosmeticsDictV2.TryGetValue(playFabId, out cosmeticInfo);
		}

		// Token: 0x06004C3E RID: 19518 RVA: 0x00169334 File Offset: 0x00167534
		private void V2_ConformCosmeticItemV1DisplayName(ref CosmeticsController.CosmeticItem cosmetic)
		{
			if (cosmetic.itemName == cosmetic.displayName)
			{
				return;
			}
			cosmetic.overrideDisplayName = cosmetic.displayName;
			cosmetic.displayName = cosmetic.itemName;
		}

		// Token: 0x06004C3F RID: 19519 RVA: 0x00169364 File Offset: 0x00167564
		internal void InitializeCosmeticStands()
		{
			foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
			{
				if (cosmeticStand != null)
				{
					cosmeticStand.InitializeCosmetic();
				}
			}
		}

		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x06004C40 RID: 19520 RVA: 0x00169399 File Offset: 0x00167599
		// (set) Token: 0x06004C41 RID: 19521 RVA: 0x001693A0 File Offset: 0x001675A0
		public static bool hasInstance { get; private set; }

		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x06004C42 RID: 19522 RVA: 0x001693A8 File Offset: 0x001675A8
		// (set) Token: 0x06004C43 RID: 19523 RVA: 0x001693B0 File Offset: 0x001675B0
		public List<CosmeticsController.CosmeticItem> allCosmetics
		{
			get
			{
				return this._allCosmetics;
			}
			set
			{
				this._allCosmetics = value;
			}
		}

		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x06004C44 RID: 19524 RVA: 0x001693B9 File Offset: 0x001675B9
		// (set) Token: 0x06004C45 RID: 19525 RVA: 0x001693C1 File Offset: 0x001675C1
		public bool allCosmeticsDict_isInitialized { get; private set; }

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x06004C46 RID: 19526 RVA: 0x001693CA File Offset: 0x001675CA
		public Dictionary<string, CosmeticsController.CosmeticItem> allCosmeticsDict
		{
			get
			{
				return this._allCosmeticsDict;
			}
		}

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06004C47 RID: 19527 RVA: 0x001693D2 File Offset: 0x001675D2
		// (set) Token: 0x06004C48 RID: 19528 RVA: 0x001693DA File Offset: 0x001675DA
		public bool allCosmeticsItemIDsfromDisplayNamesDict_isInitialized { get; private set; }

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x06004C49 RID: 19529 RVA: 0x001693E3 File Offset: 0x001675E3
		public Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict
		{
			get
			{
				return this._allCosmeticsItemIDsfromDisplayNamesDict;
			}
		}

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x06004C4A RID: 19530 RVA: 0x001693EB File Offset: 0x001675EB
		// (set) Token: 0x06004C4B RID: 19531 RVA: 0x001693F3 File Offset: 0x001675F3
		public bool isHidingCosmeticsFromRemotePlayers { get; private set; }

		// Token: 0x06004C4C RID: 19532 RVA: 0x001693FC File Offset: 0x001675FC
		public void AddWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Add(instance);
			if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
			{
				this.UpdateWardrobeModelsAndButtons();
			}
		}

		// Token: 0x06004C4D RID: 19533 RVA: 0x00169417 File Offset: 0x00167617
		public void RemoveWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Remove(instance);
		}

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06004C4E RID: 19534 RVA: 0x00169426 File Offset: 0x00167626
		public int CurrencyBalance
		{
			get
			{
				return this.currencyBalance;
			}
		}

		// Token: 0x06004C4F RID: 19535 RVA: 0x00169430 File Offset: 0x00167630
		public void Awake()
		{
			if (CosmeticsController.instance == null)
			{
				CosmeticsController.instance = this;
				CosmeticsController.hasInstance = true;
			}
			else if (CosmeticsController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.V2Awake();
			if (base.gameObject.activeSelf)
			{
				this.catalog = "DLC";
				this.currencyName = "SR";
				this.nullItem = default(CosmeticsController.CosmeticItem);
				this.nullItem.itemName = "null";
				this.nullItem.displayName = "NOTHING";
				this.nullItem.itemPicture = Resources.Load<Sprite>("CosmeticNull_Icon");
				this.nullItem.itemPictureResourceString = "";
				this.nullItem.overrideDisplayName = "NOTHING";
				this.nullItem.meshAtlasResourceString = "";
				this.nullItem.meshResourceString = "";
				this.nullItem.materialResourceString = "";
				this.nullItem.isNullItem = true;
				this._allCosmeticsDict[this.nullItem.itemName] = this.nullItem;
				this._allCosmeticsItemIDsfromDisplayNamesDict[this.nullItem.displayName] = this.nullItem.itemName;
				for (int i = 0; i < 16; i++)
				{
					this.tryOnSet.items[i] = this.nullItem;
				}
				this.cosmeticsPages[0] = 0;
				this.cosmeticsPages[1] = 0;
				this.cosmeticsPages[2] = 0;
				this.cosmeticsPages[3] = 0;
				this.cosmeticsPages[4] = 0;
				this.cosmeticsPages[5] = 0;
				this.cosmeticsPages[6] = 0;
				this.cosmeticsPages[7] = 0;
				this.cosmeticsPages[8] = 0;
				this.cosmeticsPages[9] = 0;
				this.cosmeticsPages[10] = 0;
				this.itemLists[0] = this.unlockedHats;
				this.itemLists[1] = this.unlockedFaces;
				this.itemLists[2] = this.unlockedBadges;
				this.itemLists[3] = this.unlockedPaws;
				this.itemLists[4] = this.unlockedFurs;
				this.itemLists[5] = this.unlockedShirts;
				this.itemLists[6] = this.unlockedPants;
				this.itemLists[7] = this.unlockedArms;
				this.itemLists[8] = this.unlockedBacks;
				this.itemLists[9] = this.unlockedChests;
				this.itemLists[10] = this.unlockedTagFX;
				this.updateCosmeticsRetries = 0;
				this.maxUpdateCosmeticsRetries = 5;
				this.inventoryStringList.Clear();
				this.inventoryStringList.Add("Inventory");
				base.StartCoroutine(this.CheckCanGetDaily());
			}
		}

		// Token: 0x06004C50 RID: 19536 RVA: 0x001696D8 File Offset: 0x001678D8
		public void Start()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("BundleData", delegate(string data)
			{
				this.bundleList.FromJson(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting bundle data: {0}", e));
			});
			this.anchorOverrides = GorillaTagger.Instance.offlineVRRig.GetComponent<VRRigAnchorOverrides>();
		}

		// Token: 0x06004C51 RID: 19537 RVA: 0x00169734 File Offset: 0x00167934
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (SteamManager.Initialized && this._steamMicroTransactionAuthorizationResponse == null)
			{
				this._steamMicroTransactionAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.ProcessSteamCallback));
			}
		}

		// Token: 0x06004C52 RID: 19538 RVA: 0x00169763 File Offset: 0x00167963
		public void OnDisable()
		{
			Callback<MicroTxnAuthorizationResponse_t> steamMicroTransactionAuthorizationResponse = this._steamMicroTransactionAuthorizationResponse;
			if (steamMicroTransactionAuthorizationResponse != null)
			{
				steamMicroTransactionAuthorizationResponse.Unregister();
			}
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06004C53 RID: 19539 RVA: 0x000023F4 File Offset: 0x000005F4
		public void SliceUpdate()
		{
		}

		// Token: 0x06004C54 RID: 19540 RVA: 0x00169780 File Offset: 0x00167980
		public static bool CompareCategoryToSavedCosmeticSlots(CosmeticsController.CosmeticCategory category, CosmeticsController.CosmeticSlots slot)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return slot == CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge == slot;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face == slot;
			case CosmeticsController.CosmeticCategory.Paw:
				return slot == CosmeticsController.CosmeticSlots.HandRight || slot == CosmeticsController.CosmeticSlots.HandLeft;
			case CosmeticsController.CosmeticCategory.Chest:
				return CosmeticsController.CosmeticSlots.Chest == slot;
			case CosmeticsController.CosmeticCategory.Fur:
				return CosmeticsController.CosmeticSlots.Fur == slot;
			case CosmeticsController.CosmeticCategory.Shirt:
				return CosmeticsController.CosmeticSlots.Shirt == slot;
			case CosmeticsController.CosmeticCategory.Back:
				return slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.BackRight;
			case CosmeticsController.CosmeticCategory.Arms:
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.ArmRight;
			case CosmeticsController.CosmeticCategory.Pants:
				return CosmeticsController.CosmeticSlots.Pants == slot;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return CosmeticsController.CosmeticSlots.TagEffect == slot;
			default:
				return false;
			}
		}

		// Token: 0x06004C55 RID: 19541 RVA: 0x00169814 File Offset: 0x00167A14
		public static CosmeticsController.CosmeticSlots CategoryToNonTransferrableSlot(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face;
			case CosmeticsController.CosmeticCategory.Paw:
				return CosmeticsController.CosmeticSlots.HandRight;
			case CosmeticsController.CosmeticCategory.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			case CosmeticsController.CosmeticCategory.Fur:
				return CosmeticsController.CosmeticSlots.Fur;
			case CosmeticsController.CosmeticCategory.Shirt:
				return CosmeticsController.CosmeticSlots.Shirt;
			case CosmeticsController.CosmeticCategory.Back:
				return CosmeticsController.CosmeticSlots.Back;
			case CosmeticsController.CosmeticCategory.Arms:
				return CosmeticsController.CosmeticSlots.Arms;
			case CosmeticsController.CosmeticCategory.Pants:
				return CosmeticsController.CosmeticSlots.Pants;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return CosmeticsController.CosmeticSlots.TagEffect;
			default:
				return CosmeticsController.CosmeticSlots.Count;
			}
		}

		// Token: 0x06004C56 RID: 19542 RVA: 0x00169876 File Offset: 0x00167A76
		private CosmeticsController.CosmeticSlots DropPositionToCosmeticSlot(BodyDockPositions.DropPositions pos)
		{
			switch (pos)
			{
			case BodyDockPositions.DropPositions.LeftArm:
				return CosmeticsController.CosmeticSlots.ArmLeft;
			case BodyDockPositions.DropPositions.RightArm:
				return CosmeticsController.CosmeticSlots.ArmRight;
			case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
				break;
			case BodyDockPositions.DropPositions.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			default:
				if (pos == BodyDockPositions.DropPositions.LeftBack)
				{
					return CosmeticsController.CosmeticSlots.BackLeft;
				}
				if (pos == BodyDockPositions.DropPositions.RightBack)
				{
					return CosmeticsController.CosmeticSlots.BackRight;
				}
				break;
			}
			return CosmeticsController.CosmeticSlots.Count;
		}

		// Token: 0x06004C57 RID: 19543 RVA: 0x001698A8 File Offset: 0x00167AA8
		private static BodyDockPositions.DropPositions CosmeticSlotToDropPosition(CosmeticsController.CosmeticSlots slot)
		{
			switch (slot)
			{
			case CosmeticsController.CosmeticSlots.ArmLeft:
				return BodyDockPositions.DropPositions.LeftArm;
			case CosmeticsController.CosmeticSlots.ArmRight:
				return BodyDockPositions.DropPositions.RightArm;
			case CosmeticsController.CosmeticSlots.BackLeft:
				return BodyDockPositions.DropPositions.LeftBack;
			case CosmeticsController.CosmeticSlots.BackRight:
				return BodyDockPositions.DropPositions.RightBack;
			case CosmeticsController.CosmeticSlots.Chest:
				return BodyDockPositions.DropPositions.Chest;
			}
			return BodyDockPositions.DropPositions.None;
		}

		// Token: 0x06004C58 RID: 19544 RVA: 0x001698DC File Offset: 0x00167ADC
		private void SaveItemPreference(CosmeticsController.CosmeticSlots slot, int slotIdx, CosmeticsController.CosmeticItem newItem)
		{
			PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), newItem.itemName);
			PlayerPrefs.Save();
		}

		// Token: 0x06004C59 RID: 19545 RVA: 0x001698F4 File Offset: 0x00167AF4
		public void SaveCurrentItemPreferences()
		{
			for (int i = 0; i < 16; i++)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
				this.SaveItemPreference(cosmeticSlots, i, this.currentWornSet.items[i]);
			}
		}

		// Token: 0x06004C5A RID: 19546 RVA: 0x0016992C File Offset: 0x00167B2C
		private void ApplyCosmeticToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, int slotIdx, CosmeticsController.CosmeticSlots slot, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			CosmeticsController.CosmeticItem cosmeticItem = ((set.items[slotIdx].itemName == newItem.itemName) ? this.nullItem : newItem);
			set.items[slotIdx] = cosmeticItem;
			if (applyToPlayerPrefs)
			{
				this.SaveItemPreference(slot, slotIdx, cosmeticItem);
			}
			appliedSlots.Add(slot);
		}

		// Token: 0x06004C5B RID: 19547 RVA: 0x00169988 File Offset: 0x00167B88
		private void PrivApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			if (newItem.isNullItem)
			{
				return;
			}
			if (CosmeticsController.CosmeticSet.IsHoldable(newItem))
			{
				BodyDockPositions.DockingResult dockingResult = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>().ToggleWithHandedness(newItem.displayName, isLeftHand, newItem.bothHandsHoldable);
				foreach (BodyDockPositions.DropPositions dropPositions in dockingResult.positionsDisabled)
				{
					CosmeticsController.CosmeticSlots cosmeticSlots = this.DropPositionToCosmeticSlot(dropPositions);
					if (cosmeticSlots != CosmeticsController.CosmeticSlots.Count)
					{
						int num = (int)cosmeticSlots;
						set.items[num] = this.nullItem;
						if (applyToPlayerPrefs)
						{
							this.SaveItemPreference(cosmeticSlots, num, this.nullItem);
						}
					}
				}
				using (List<BodyDockPositions.DropPositions>.Enumerator enumerator = dockingResult.dockedPosition.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BodyDockPositions.DropPositions dropPositions2 = enumerator.Current;
						if (dropPositions2 != BodyDockPositions.DropPositions.None)
						{
							CosmeticsController.CosmeticSlots cosmeticSlots2 = this.DropPositionToCosmeticSlot(dropPositions2);
							int num2 = (int)cosmeticSlots2;
							set.items[num2] = newItem;
							if (applyToPlayerPrefs)
							{
								this.SaveItemPreference(cosmeticSlots2, num2, newItem);
							}
							appliedSlots.Add(cosmeticSlots2);
						}
					}
					return;
				}
			}
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Paw)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots3 = (isLeftHand ? CosmeticsController.CosmeticSlots.HandLeft : CosmeticsController.CosmeticSlots.HandRight);
				int num3 = (int)cosmeticSlots3;
				this.ApplyCosmeticToSet(set, newItem, num3, cosmeticSlots3, applyToPlayerPrefs, appliedSlots);
				CosmeticsController.CosmeticSlots cosmeticSlots4 = CosmeticsController.CosmeticSet.OppositeSlot(cosmeticSlots3);
				int num4 = (int)cosmeticSlots4;
				if (newItem.bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num4, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
				if (set.items[num4].itemName == newItem.itemName)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num4, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
				}
				if (set.items[num4].bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num4, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
			}
			else
			{
				CosmeticsController.CosmeticSlots cosmeticSlots5 = CosmeticsController.CategoryToNonTransferrableSlot(newItem.itemCategory);
				int num5 = (int)cosmeticSlots5;
				this.ApplyCosmeticToSet(set, newItem, num5, cosmeticSlots5, applyToPlayerPrefs, appliedSlots);
			}
		}

		// Token: 0x06004C5C RID: 19548 RVA: 0x00169B8C File Offset: 0x00167D8C
		public void ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs)
		{
			this.ApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, CosmeticsController._g_default_outAppliedSlotsList_for_applyCosmeticItemToSet);
		}

		// Token: 0x06004C5D RID: 19549 RVA: 0x00169BA0 File Offset: 0x00167DA0
		public void ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> outAppliedSlotsList)
		{
			outAppliedSlotsList.Clear();
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
			{
				bool flag = false;
				Dictionary<CosmeticsController.CosmeticItem, bool> dictionary = new Dictionary<CosmeticsController.CosmeticItem, bool>();
				foreach (string text in newItem.bundledItems)
				{
					CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(text);
					if (this.AnyMatch(set, itemFromDict))
					{
						flag = true;
						dictionary.Add(itemFromDict, true);
					}
					else
					{
						dictionary.Add(itemFromDict, false);
					}
				}
				using (Dictionary<CosmeticsController.CosmeticItem, bool>.Enumerator enumerator = dictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<CosmeticsController.CosmeticItem, bool> keyValuePair = enumerator.Current;
						if (flag)
						{
							if (keyValuePair.Value)
							{
								this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
							}
						}
						else
						{
							this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
						}
					}
					return;
				}
			}
			this.PrivApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
		}

		// Token: 0x06004C5E RID: 19550 RVA: 0x00169C8C File Offset: 0x00167E8C
		public void RemoveCosmeticItemFromSet(CosmeticsController.CosmeticSet set, string itemName, bool applyToPlayerPrefs)
		{
			this.cachedSet.CopyItems(set);
			for (int i = 0; i < 16; i++)
			{
				if (set.items[i].displayName == itemName)
				{
					set.items[i] = this.nullItem;
					if (applyToPlayerPrefs)
					{
						this.SaveItemPreference((CosmeticsController.CosmeticSlots)i, i, this.nullItem);
					}
				}
			}
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			BodyDockPositions component = offlineVRRig.GetComponent<BodyDockPositions>();
			set.ActivateCosmetics(this.cachedSet, offlineVRRig, component, offlineVRRig.cosmeticsObjectRegistry);
		}

		// Token: 0x06004C5F RID: 19551 RVA: 0x00169D14 File Offset: 0x00167F14
		public void PressFittingRoomButton(FittingRoomButton pressedFittingRoomButton, bool isLeftHand)
		{
			BundleManager.instance._tryOnBundlesStand.ClearSelectedBundle();
			this.ApplyCosmeticItemToSet(this.tryOnSet, pressedFittingRoomButton.currentCosmeticItem, isLeftHand, false);
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06004C60 RID: 19552 RVA: 0x00169D48 File Offset: 0x00167F48
		public CosmeticsController.EWearingCosmeticSet CheckIfCosmeticSetMatchesItemSet(CosmeticsController.CosmeticSet set, string itemName)
		{
			CosmeticsController.EWearingCosmeticSet ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotASet;
			CosmeticsController.CosmeticItem cosmeticItem = this.allCosmeticsDict[itemName];
			if (cosmeticItem.bundledItems.Length != 0)
			{
				foreach (string text in cosmeticItem.bundledItems)
				{
					if (this.AnyMatch(set, this.allCosmeticsDict[text]))
					{
						if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotASet)
						{
							ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Complete;
						}
						else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotWearing)
						{
							ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
						}
					}
					else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotASet)
					{
						ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotWearing;
					}
					else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.Complete)
					{
						ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
					}
				}
			}
			return ewearingCosmeticSet;
		}

		// Token: 0x06004C61 RID: 19553 RVA: 0x00169DBC File Offset: 0x00167FBC
		public void PressCosmeticStandButton(CosmeticStand pressedStand)
		{
			this.searchIndex = this.currentCart.IndexOf(pressedStand.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, pressedStand.thisCosmeticItem);
				this.currentCart.RemoveAt(this.searchIndex);
				pressedStand.isOn = false;
				for (int i = 0; i < 16; i++)
				{
					if (pressedStand.thisCosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
			}
			else
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, pressedStand.thisCosmeticItem);
				this.currentCart.Insert(0, pressedStand.thisCosmeticItem);
				pressedStand.isOn = true;
				if (this.currentCart.Count > this.fittingRoomButtons.Length)
				{
					foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
					{
						if (!(cosmeticStand == null) && cosmeticStand.thisCosmeticItem.itemName == this.currentCart[this.fittingRoomButtons.Length].itemName)
						{
							cosmeticStand.isOn = false;
							cosmeticStand.UpdateColor();
							break;
						}
					}
					this.currentCart.RemoveAt(this.fittingRoomButtons.Length);
				}
			}
			pressedStand.UpdateColor();
			this.UpdateShoppingCart();
		}

		// Token: 0x06004C62 RID: 19554 RVA: 0x00169F28 File Offset: 0x00168128
		public void PressWardrobeItemButton(CosmeticsController.CosmeticItem cosmeticItem, bool isLeftHand)
		{
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(cosmeticItem.itemName);
			List<CosmeticsController.CosmeticSlots> list = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
			if (list.Capacity < 16)
			{
				list.Capacity = 16;
			}
			this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true, list);
			foreach (CosmeticsController.CosmeticSlots cosmeticSlots in list)
			{
				this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
			}
			CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(list);
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true);
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x06004C63 RID: 19555 RVA: 0x00169FE8 File Offset: 0x001681E8
		public void PressWardrobeFunctionButton(string function)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(function);
			if (num <= 2554875734U)
			{
				if (num <= 895779448U)
				{
					if (num != 292255708U)
					{
						if (num != 306900080U)
						{
							if (num == 895779448U)
							{
								if (function == "badge")
								{
									if (this.wardrobeType == 2)
									{
										return;
									}
									this.wardrobeType = 2;
								}
							}
						}
						else if (function == "left")
						{
							this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] - 1;
							if (this.cosmeticsPages[this.wardrobeType] < 0)
							{
								this.cosmeticsPages[this.wardrobeType] = (this.itemLists[this.wardrobeType].Count - 1) / 3;
							}
						}
					}
					else if (function == "face")
					{
						if (this.wardrobeType == 1)
						{
							return;
						}
						this.wardrobeType = 1;
					}
				}
				else if (num != 1538531746U)
				{
					if (num != 2028154341U)
					{
						if (num == 2554875734U)
						{
							if (function == "chest")
							{
								if (this.wardrobeType == 8)
								{
									return;
								}
								this.wardrobeType = 8;
							}
						}
					}
					else if (function == "right")
					{
						this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] + 1;
						if (this.cosmeticsPages[this.wardrobeType] > (this.itemLists[this.wardrobeType].Count - 1) / 3)
						{
							this.cosmeticsPages[this.wardrobeType] = 0;
						}
					}
				}
				else if (function == "back")
				{
					if (this.wardrobeType == 7)
					{
						return;
					}
					this.wardrobeType = 7;
				}
			}
			else if (num <= 3034286914U)
			{
				if (num != 2633735346U)
				{
					if (num != 2953262278U)
					{
						if (num == 3034286914U)
						{
							if (function == "fur")
							{
								if (this.wardrobeType == 4)
								{
									return;
								}
								this.wardrobeType = 4;
							}
						}
					}
					else if (function == "outfit")
					{
						if (this.wardrobeType == 5)
						{
							return;
						}
						this.wardrobeType = 5;
					}
				}
				else if (function == "arms")
				{
					if (this.wardrobeType == 6)
					{
						return;
					}
					this.wardrobeType = 6;
				}
			}
			else if (num <= 3300536096U)
			{
				if (num != 3081164502U)
				{
					if (num == 3300536096U)
					{
						if (function == "hand")
						{
							if (this.wardrobeType == 3)
							{
								return;
							}
							this.wardrobeType = 3;
						}
					}
				}
				else if (function == "tagEffect")
				{
					if (this.wardrobeType == 10)
					{
						return;
					}
					this.wardrobeType = 10;
				}
			}
			else if (num != 3568683773U)
			{
				if (num == 4072609730U)
				{
					if (function == "hat")
					{
						if (this.wardrobeType == 0)
						{
							return;
						}
						this.wardrobeType = 0;
					}
				}
			}
			else if (function == "reserved")
			{
				if (this.wardrobeType == 9)
				{
					return;
				}
				this.wardrobeType = 9;
			}
			this.UpdateWardrobeModelsAndButtons();
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated();
		}

		// Token: 0x06004C64 RID: 19556 RVA: 0x0016A374 File Offset: 0x00168574
		public void ClearCheckout(bool sendEvent)
		{
			if (sendEvent)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_cancel, this.currentCart);
			}
			this.itemToBuy = this.nullItem;
			this.checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState(null, false);
		}

		// Token: 0x06004C65 RID: 19557 RVA: 0x0016A3CC File Offset: 0x001685CC
		public bool RemoveItemFromCart(CosmeticsController.CosmeticItem cosmeticItem)
		{
			this.searchIndex = this.currentCart.IndexOf(cosmeticItem);
			if (this.searchIndex != -1)
			{
				this.currentCart.RemoveAt(this.searchIndex);
				for (int i = 0; i < 16; i++)
				{
					if (cosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06004C66 RID: 19558 RVA: 0x0016A450 File Offset: 0x00168650
		public void PressCheckoutCartButton(CheckoutCartButton pressedCheckoutCartButton, bool isLeftHand)
		{
			if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Buying)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.tryOnSet.ClearSet(this.nullItem);
				if (this.itemToBuy.displayName == pressedCheckoutCartButton.currentCosmeticItem.displayName)
				{
					this.itemToBuy = this.nullItem;
					this.checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
				}
				else
				{
					this.itemToBuy = pressedCheckoutCartButton.currentCosmeticItem;
					this.checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
					if (this.itemToBuy.bundledItems != null && this.itemToBuy.bundledItems.Length != 0)
					{
						List<string> list = new List<string>();
						foreach (string text in this.itemToBuy.bundledItems)
						{
							this.tempItem = this.GetItemFromDict(text);
							list.Add(this.tempItem.displayName);
						}
						this.checkoutHeadModel.SetCosmeticActiveArray(list.ToArray(), new bool[list.Count]);
					}
					this.ApplyCosmeticItemToSet(this.tryOnSet, pressedCheckoutCartButton.currentCosmeticItem, isLeftHand, false);
					this.UpdateWornCosmetics(true);
				}
				this.ProcessPurchaseItemState(null, isLeftHand);
				this.UpdateShoppingCart();
			}
		}

		// Token: 0x06004C67 RID: 19559 RVA: 0x0016A58B File Offset: 0x0016878B
		public void PressPurchaseItemButton(PurchaseItemButton pressedPurchaseItemButton, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(pressedPurchaseItemButton.buttonSide, isLeftHand);
		}

		// Token: 0x06004C68 RID: 19560 RVA: 0x0016A59A File Offset: 0x0016879A
		public void PurchaseBundle(StoreBundle bundleToPurchase)
		{
			if (bundleToPurchase.playfabBundleID != "NULL")
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				this.ProcessPurchaseItemState("left", false);
				this.buyingBundle = true;
				this.itemToPurchase = bundleToPurchase.playfabBundleID;
				this.SteamPurchase();
			}
		}

		// Token: 0x06004C69 RID: 19561 RVA: 0x0016A5DC File Offset: 0x001687DC
		public void PressEarlyAccessButton()
		{
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState("left", false);
			this.buyingBundle = true;
			this.itemToPurchase = this.BundlePlayfabItemName;
			ATM_Manager.instance.shinyRocksCost = (float)this.BundleShinyRocks;
			this.SteamPurchase();
		}

		// Token: 0x06004C6A RID: 19562 RVA: 0x0016A628 File Offset: 0x00168828
		public void PressPurchaseBundleButton(string PlayFabItemName)
		{
			BundleManager.instance.BundlePurchaseButtonPressed(PlayFabItemName);
		}

		// Token: 0x06004C6B RID: 19563 RVA: 0x0016A638 File Offset: 0x00168838
		public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
		{
			switch (this.currentPurchaseItemStage)
			{
			case CosmeticsController.PurchaseItemStages.Start:
				this.itemToBuy = this.nullItem;
				this.FormattedPurchaseText("SELECT AN ITEM FROM YOUR CART TO PURCHASE!");
				this.UpdateShoppingCart();
				return;
			case CosmeticsController.PurchaseItemStages.CheckoutButtonPressed:
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_start, this.currentCart);
				this.searchIndex = this.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.itemToBuy.itemName == x.itemName);
				if (this.searchIndex > -1)
				{
					this.FormattedPurchaseText("YOU ALREADY OWN THIS ITEM!");
					this.leftPurchaseButton.myText.text = "-";
					this.rightPurchaseButton.myText.text = "-";
					this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
					this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemOwned;
					return;
				}
				if (this.itemToBuy.cost <= this.currencyBalance)
				{
					this.FormattedPurchaseText("DO YOU WANT TO BUY THIS ITEM?");
					this.leftPurchaseButton.myText.text = "NO!";
					this.rightPurchaseButton.myText.text = "YES!";
					this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.unpressedMaterial;
					this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.unpressedMaterial;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemSelected;
					return;
				}
				this.FormattedPurchaseText("INSUFFICIENT SHINY ROCKS FOR THIS ITEM!");
				this.leftPurchaseButton.myText.text = "-";
				this.rightPurchaseButton.myText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				return;
			case CosmeticsController.PurchaseItemStages.ItemSelected:
				if (buttonSide == "right")
				{
					GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.item_select, this.itemToBuy);
					this.FormattedPurchaseText("ARE YOU REALLY SURE?");
					this.leftPurchaseButton.myText.text = "YES! I NEED IT!";
					this.rightPurchaseButton.myText.text = "LET ME THINK ABOUT IT";
					this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.unpressedMaterial;
					this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.unpressedMaterial;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement;
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.ItemOwned:
			case CosmeticsController.PurchaseItemStages.Buying:
				break;
			case CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement:
				if (buttonSide == "left")
				{
					this.FormattedPurchaseText("PURCHASING ITEM...");
					this.leftPurchaseButton.myText.text = "-";
					this.rightPurchaseButton.myText.text = "-";
					this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
					this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Buying;
					this.isLastHandTouchedLeft = isLeftHand;
					this.PurchaseItem();
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.Success:
			{
				this.FormattedPurchaseText("SUCCESS! ENJOY YOUR NEW ITEM!");
				VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
				offlineVRRig.concatStringOfCosmeticsAllowed += this.itemToBuy.itemName;
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
				if (itemFromDict.bundledItems != null)
				{
					foreach (string text in itemFromDict.bundledItems)
					{
						VRRig offlineVRRig2 = GorillaTagger.Instance.offlineVRRig;
						offlineVRRig2.concatStringOfCosmeticsAllowed += text;
					}
				}
				this.tryOnSet.ClearSet(this.nullItem);
				this.UpdateShoppingCart();
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics(false);
				this.UpdateWardrobeModelsAndButtons();
				Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
				if (onCosmeticsUpdated != null)
				{
					onCosmeticsUpdated();
				}
				this.leftPurchaseButton.myText.text = "-";
				this.rightPurchaseButton.myText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				break;
			}
			case CosmeticsController.PurchaseItemStages.Failure:
				this.FormattedPurchaseText("ERROR IN PURCHASING ITEM! NO MONEY WAS SPENT. SELECT ANOTHER ITEM.");
				this.leftPurchaseButton.myText.text = "-";
				this.rightPurchaseButton.myText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				return;
			default:
				return;
			}
		}

		// Token: 0x06004C6C RID: 19564 RVA: 0x0016AB18 File Offset: 0x00168D18
		public void FormattedPurchaseText(string finalLineVar)
		{
			this.finalLine = finalLineVar;
			this.purchaseText.text = string.Concat(new string[]
			{
				"SELECTION: ",
				this.GetItemDisplayName(this.itemToBuy),
				"\nITEM COST: ",
				this.itemToBuy.cost.ToString(),
				"\nYOU HAVE: ",
				this.currencyBalance.ToString(),
				"\n\n",
				this.finalLine
			});
		}

		// Token: 0x06004C6D RID: 19565 RVA: 0x0016AB9C File Offset: 0x00168D9C
		public void PurchaseItem()
		{
			PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
			{
				ItemId = this.itemToBuy.itemName,
				Price = this.itemToBuy.cost,
				VirtualCurrency = this.currencyName,
				CatalogVersion = this.catalog
			}, delegate(PurchaseItemResult result)
			{
				if (result.Items.Count > 0)
				{
					foreach (ItemInstance itemInstance in result.Items)
					{
						CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
						if (itemFromDict.itemCategory == CosmeticsController.CosmeticCategory.Set)
						{
							this.UnlockItem(itemInstance.ItemId);
							foreach (string text in itemFromDict.bundledItems)
							{
								this.UnlockItem(text);
							}
						}
						else
						{
							this.UnlockItem(itemInstance.ItemId);
						}
					}
					this.UpdateMyCosmetics();
					if (NetworkSystem.Instance.InRoom)
					{
						base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.itemToBuy.itemName));
					}
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;
					this.currencyBalance -= this.itemToBuy.cost;
					this.UpdateShoppingCart();
					this.ProcessPurchaseItemState(null, this.isLastHandTouchedLeft);
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, delegate(PlayFabError error)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, null, null);
		}

		// Token: 0x06004C6E RID: 19566 RVA: 0x0016AC08 File Offset: 0x00168E08
		private void UnlockItem(string itemIdToUnlock)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => itemIdToUnlock == x.itemName);
			if (num > -1)
			{
				if (!this.unlockedCosmetics.Contains(this.allCosmetics[num]))
				{
					this.unlockedCosmetics.Add(this.allCosmetics[num]);
				}
				this.concatStringCosmeticsAllowed += this.allCosmetics[num].itemName;
				switch (this.allCosmetics[num].itemCategory)
				{
				case CosmeticsController.CosmeticCategory.Hat:
					if (!this.unlockedHats.Contains(this.allCosmetics[num]))
					{
						this.unlockedHats.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Badge:
					if (!this.unlockedBadges.Contains(this.allCosmetics[num]))
					{
						this.unlockedBadges.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Face:
					if (!this.unlockedFaces.Contains(this.allCosmetics[num]))
					{
						this.unlockedFaces.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Paw:
					if (!this.unlockedPaws.Contains(this.allCosmetics[num]))
					{
						this.unlockedPaws.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Chest:
					if (!this.unlockedChests.Contains(this.allCosmetics[num]))
					{
						this.unlockedChests.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Fur:
					if (!this.unlockedFurs.Contains(this.allCosmetics[num]))
					{
						this.unlockedFurs.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Shirt:
					if (!this.unlockedShirts.Contains(this.allCosmetics[num]))
					{
						this.unlockedShirts.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Back:
					if (!this.unlockedBacks.Contains(this.allCosmetics[num]))
					{
						this.unlockedBacks.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Arms:
					if (!this.unlockedArms.Contains(this.allCosmetics[num]))
					{
						this.unlockedArms.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Pants:
					if (!this.unlockedPants.Contains(this.allCosmetics[num]))
					{
						this.unlockedPants.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.TagEffect:
					if (!this.unlockedTagFX.Contains(this.allCosmetics[num]))
					{
						this.unlockedTagFX.Add(this.allCosmetics[num]);
						return;
					}
					break;
				case CosmeticsController.CosmeticCategory.Count:
					break;
				case CosmeticsController.CosmeticCategory.Set:
					foreach (string text in this.allCosmetics[num].bundledItems)
					{
						this.UnlockItem(text);
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06004C6F RID: 19567 RVA: 0x0016AF4A File Offset: 0x0016914A
		private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
		{
			Debug.Log("Cosmetic updated check!");
			yield return new WaitForSeconds(1f);
			this.foundCosmetic = false;
			this.attempts = 0;
			while (!this.foundCosmetic && this.attempts < 10 && NetworkSystem.Instance.InRoom)
			{
				this.playerIDList.Clear();
				if (this.UseNewCosmeticsPath())
				{
					this.playerIDList.Add("Inventory");
					PlayFabClientAPI.GetSharedGroupData(new global::PlayFab.ClientModels.GetSharedGroupDataRequest
					{
						Keys = this.playerIDList,
						SharedGroupId = NetworkSystem.Instance.LocalPlayer.UserId + "Inventory"
					}, delegate(GetSharedGroupDataResult result)
					{
						this.attempts++;
						foreach (KeyValuePair<string, global::PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
						{
							if (keyValuePair.Value.Value.Contains(itemToBuyID))
							{
								PhotonNetwork.RaiseEvent(199, null, new RaiseEventOptions
								{
									Receivers = ReceiverGroup.Others
								}, SendOptions.SendReliable);
								this.foundCosmetic = true;
							}
						}
						if (this.foundCosmetic)
						{
							this.UpdateWornCosmetics(true);
						}
					}, delegate(PlayFabError error)
					{
						this.attempts++;
						this.ReauthOrBan(error);
					}, null, null);
					yield return new WaitForSeconds(1f);
				}
				else
				{
					this.playerIDList.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
					PlayFabClientAPI.GetSharedGroupData(new global::PlayFab.ClientModels.GetSharedGroupDataRequest
					{
						Keys = this.playerIDList,
						SharedGroupId = NetworkSystem.Instance.RoomName + Regex.Replace(NetworkSystem.Instance.CurrentRegion, "[^a-zA-Z0-9]", "").ToUpper()
					}, delegate(GetSharedGroupDataResult result)
					{
						this.attempts++;
						foreach (KeyValuePair<string, global::PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair2 in result.Data)
						{
							if (keyValuePair2.Value.Value.Contains(itemToBuyID))
							{
								NetworkSystemRaiseEvent.RaiseEvent(199, null, NetworkSystemRaiseEvent.neoOthers, true);
								this.foundCosmetic = true;
							}
							else
							{
								Debug.Log("didnt find it, updating attempts and trying again in a bit. current attempt is " + this.attempts.ToString());
							}
						}
						if (this.foundCosmetic)
						{
							this.UpdateWornCosmetics(true);
						}
					}, delegate(PlayFabError error)
					{
						this.attempts++;
						if (error.Error == PlayFabErrorCode.NotAuthenticated)
						{
							PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
						}
						else if (error.Error == PlayFabErrorCode.AccountBanned)
						{
							GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
						}
						Debug.Log("Got error retrieving user data, on attempt " + this.attempts.ToString());
						Debug.Log(error.GenerateErrorReport());
					}, null, null);
					yield return new WaitForSeconds(1f);
				}
			}
			Debug.Log("done!");
			yield break;
		}

		// Token: 0x06004C70 RID: 19568 RVA: 0x0016AF60 File Offset: 0x00169160
		public void UpdateWardrobeModelsAndButtons()
		{
			if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
			{
				return;
			}
			foreach (WardrobeInstance wardrobeInstance in this.wardrobes)
			{
				wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 1 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 1] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 2 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 2] : this.nullItem);
				this.iterator = 0;
				while (this.iterator < wardrobeInstance.wardrobeItemButtons.Length)
				{
					CosmeticsController.CosmeticItem currentCosmeticItem = wardrobeInstance.wardrobeItemButtons[this.iterator].currentCosmeticItem;
					wardrobeInstance.wardrobeItemButtons[this.iterator].isOn = !currentCosmeticItem.isNullItem && this.AnyMatch(this.currentWornSet, currentCosmeticItem);
					wardrobeInstance.wardrobeItemButtons[this.iterator].UpdateColor();
					this.iterator++;
				}
				wardrobeInstance.wardrobeItemButtons[0].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[1].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[2].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem.displayName, false);
				wardrobeInstance.selfDoll.SetCosmeticActiveArray(this.currentWornSet.ToDisplayNameArray(), this.currentWornSet.ToOnRightSideArray());
			}
		}

		// Token: 0x06004C71 RID: 19569 RVA: 0x0016B1E0 File Offset: 0x001693E0
		public int GetCategorySize(CosmeticsController.CosmeticCategory category)
		{
			int indexForCategory = this.GetIndexForCategory(category);
			if (indexForCategory != -1)
			{
				return this.itemLists[indexForCategory].Count;
			}
			return 0;
		}

		// Token: 0x06004C72 RID: 19570 RVA: 0x0016B208 File Offset: 0x00169408
		public CosmeticsController.CosmeticItem GetCosmetic(int category, int cosmeticIndex)
		{
			if (cosmeticIndex >= this.itemLists[category].Count || cosmeticIndex < 0)
			{
				return this.nullItem;
			}
			return this.itemLists[category][cosmeticIndex];
		}

		// Token: 0x06004C73 RID: 19571 RVA: 0x0016B233 File Offset: 0x00169433
		public CosmeticsController.CosmeticItem GetCosmetic(CosmeticsController.CosmeticCategory category, int cosmeticIndex)
		{
			return this.GetCosmetic(this.GetIndexForCategory(category), cosmeticIndex);
		}

		// Token: 0x06004C74 RID: 19572 RVA: 0x0016B244 File Offset: 0x00169444
		private int GetIndexForCategory(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return 0;
			case CosmeticsController.CosmeticCategory.Badge:
				return 2;
			case CosmeticsController.CosmeticCategory.Face:
				return 1;
			case CosmeticsController.CosmeticCategory.Paw:
				return 3;
			case CosmeticsController.CosmeticCategory.Chest:
				return 9;
			case CosmeticsController.CosmeticCategory.Fur:
				return 4;
			case CosmeticsController.CosmeticCategory.Shirt:
				return 5;
			case CosmeticsController.CosmeticCategory.Back:
				return 8;
			case CosmeticsController.CosmeticCategory.Arms:
				return 7;
			case CosmeticsController.CosmeticCategory.Pants:
				return 6;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return 10;
			default:
				return 0;
			}
		}

		// Token: 0x06004C75 RID: 19573 RVA: 0x0016B2A0 File Offset: 0x001694A0
		public bool IsCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic)
		{
			return this.AnyMatch(this.currentWornSet, cosmetic);
		}

		// Token: 0x06004C76 RID: 19574 RVA: 0x0016B2B0 File Offset: 0x001694B0
		public CosmeticsController.CosmeticItem GetSlotItem(CosmeticsController.CosmeticSlots slot, bool checkOpposite = true)
		{
			int num = (int)slot;
			if (checkOpposite)
			{
				num = (int)CosmeticsController.CosmeticSet.OppositeSlot(slot);
			}
			return this.currentWornSet.items[num];
		}

		// Token: 0x06004C77 RID: 19575 RVA: 0x0016B2DA File Offset: 0x001694DA
		public string[] GetCurrentlyWornCosmetics()
		{
			return this.currentWornSet.ToDisplayNameArray();
		}

		// Token: 0x06004C78 RID: 19576 RVA: 0x0016B2E7 File Offset: 0x001694E7
		public bool[] GetCurrentRightEquippedSided()
		{
			return this.currentWornSet.ToOnRightSideArray();
		}

		// Token: 0x06004C79 RID: 19577 RVA: 0x0016B2F4 File Offset: 0x001694F4
		public void UpdateShoppingCart()
		{
			this.iterator = 0;
			while (this.iterator < this.fittingRoomButtons.Length)
			{
				if (this.iterator < this.currentCart.Count)
				{
					this.fittingRoomButtons[this.iterator].currentCosmeticItem = this.currentCart[this.iterator];
					this.checkoutCartButtons[this.iterator].currentCosmeticItem = this.currentCart[this.iterator];
					this.checkoutCartButtons[this.iterator].isOn = this.checkoutCartButtons[this.iterator].currentCosmeticItem.itemName == this.itemToBuy.itemName;
					this.fittingRoomButtons[this.iterator].isOn = this.AnyMatch(this.tryOnSet, this.fittingRoomButtons[this.iterator].currentCosmeticItem);
				}
				else
				{
					this.checkoutCartButtons[this.iterator].currentCosmeticItem = this.nullItem;
					this.fittingRoomButtons[this.iterator].currentCosmeticItem = this.nullItem;
					this.checkoutCartButtons[this.iterator].isOn = false;
					this.fittingRoomButtons[this.iterator].isOn = false;
				}
				this.checkoutCartButtons[this.iterator].currentImage.sprite = this.checkoutCartButtons[this.iterator].currentCosmeticItem.itemPicture;
				this.fittingRoomButtons[this.iterator].currentImage.sprite = this.fittingRoomButtons[this.iterator].currentCosmeticItem.itemPicture;
				this.checkoutCartButtons[this.iterator].UpdateColor();
				this.fittingRoomButtons[this.iterator].UpdateColor();
				this.iterator++;
			}
			if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
			{
				this.UpdateWardrobeModelsAndButtons();
			}
		}

		// Token: 0x06004C7A RID: 19578 RVA: 0x0016B4D8 File Offset: 0x001696D8
		public void UpdateWornCosmetics(bool sync = false)
		{
			GorillaTagger.Instance.offlineVRRig.LocalUpdateCosmeticsWithTryon(this.currentWornSet, this.tryOnSet);
			if (sync && GorillaTagger.Instance.myVRRig != null)
			{
				if (this.isHidingCosmeticsFromRemotePlayers)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", RpcTarget.All, Array.Empty<object>());
					return;
				}
				int[] array = this.currentWornSet.ToPackedIDArray();
				int[] array2 = this.tryOnSet.ToPackedIDArray();
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, new object[] { array, array2 });
			}
		}

		// Token: 0x06004C7B RID: 19579 RVA: 0x0016B573 File Offset: 0x00169773
		public CosmeticsController.CosmeticItem GetItemFromDict(string itemID)
		{
			if (!this.allCosmeticsDict.TryGetValue(itemID, out this.cosmeticItemVar))
			{
				return this.nullItem;
			}
			return this.cosmeticItemVar;
		}

		// Token: 0x06004C7C RID: 19580 RVA: 0x0016B596 File Offset: 0x00169796
		public string GetItemNameFromDisplayName(string displayName)
		{
			if (!this.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(displayName, out this.returnString))
			{
				return "null";
			}
			return this.returnString;
		}

		// Token: 0x06004C7D RID: 19581 RVA: 0x0016B5B8 File Offset: 0x001697B8
		public bool AnyMatch(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem item)
		{
			if (item.itemCategory != CosmeticsController.CosmeticCategory.Set)
			{
				return set.IsActive(item.displayName);
			}
			if (item.bundledItems.Length == 1)
			{
				return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0]));
			}
			if (item.bundledItems.Length == 2)
			{
				return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1]));
			}
			return item.bundledItems.Length >= 3 && (this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[2])));
		}

		// Token: 0x06004C7E RID: 19582 RVA: 0x0016B68C File Offset: 0x0016988C
		public void Initialize()
		{
			if (!base.gameObject.activeSelf || this.v2_isCosmeticPlayFabCatalogDataLoaded || this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback)
			{
				return;
			}
			if (this.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				this.GetCosmeticsPlayFabCatalogData();
				return;
			}
			this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback = true;
			this.V2_allCosmeticsInfoAssetRef_OnPostLoad = (Action)Delegate.Combine(this.V2_allCosmeticsInfoAssetRef_OnPostLoad, new Action(this.GetCosmeticsPlayFabCatalogData));
		}

		// Token: 0x06004C7F RID: 19583 RVA: 0x0016B6EF File Offset: 0x001698EF
		public void GetLastDailyLogin()
		{
			PlayFabClientAPI.GetUserReadOnlyData(new global::PlayFab.ClientModels.GetUserDataRequest(), delegate(GetUserDataResult result)
			{
				if (result.Data.TryGetValue("DailyLogin", out this.userDataRecord))
				{
					this.lastDailyLogin = this.userDataRecord.Value;
					return;
				}
				this.lastDailyLogin = "NONE";
				base.StartCoroutine(this.GetMyDaily());
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error getting read-only user data:");
				Debug.Log(error.GenerateErrorReport());
				this.lastDailyLogin = "FAILED";
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x06004C80 RID: 19584 RVA: 0x0016B715 File Offset: 0x00169915
		private IEnumerator CheckCanGetDaily()
		{
			while (!KIDManager.InitialisationComplete)
			{
				yield return new WaitForSeconds(1f);
			}
			for (;;)
			{
				if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
				{
					this.currentTime = new DateTime((GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) * 10000L);
					this.secondsUntilTomorrow = (int)(this.currentTime.AddDays(1.0).Date - this.currentTime).TotalSeconds;
					if (this.lastDailyLogin == null || this.lastDailyLogin == "")
					{
						this.GetLastDailyLogin();
					}
					else if (this.currentTime.ToString("o").Substring(0, 10) == this.lastDailyLogin)
					{
						this.checkedDaily = true;
						this.gotMyDaily = true;
					}
					else if (this.currentTime.ToString("o").Substring(0, 10) != this.lastDailyLogin)
					{
						this.checkedDaily = true;
						this.gotMyDaily = false;
						base.StartCoroutine(this.GetMyDaily());
					}
					else if (this.lastDailyLogin == "FAILED")
					{
						this.GetLastDailyLogin();
					}
					this.secondsToWaitToCheckDaily = (this.checkedDaily ? 60f : 10f);
					this.UpdateCurrencyBoard();
					yield return new WaitForSeconds(this.secondsToWaitToCheckDaily);
				}
				else
				{
					yield return new WaitForSeconds(1f);
				}
			}
			yield break;
		}

		// Token: 0x06004C81 RID: 19585 RVA: 0x0016B724 File Offset: 0x00169924
		private IEnumerator GetMyDaily()
		{
			yield return new WaitForSeconds(10f);
			GorillaServer.Instance.TryDistributeCurrency(delegate(ExecuteFunctionResult result)
			{
				this.GetCurrencyBalance();
				this.GetLastDailyLogin();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			});
			yield break;
		}

		// Token: 0x06004C82 RID: 19586 RVA: 0x0016B733 File Offset: 0x00169933
		public void GetCosmeticsPlayFabCatalogData()
		{
			this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback = false;
			if (!this.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				throw new Exception("Method `GetCosmeticsPlayFabCatalogData` was called before `v2_allCosmeticsInfoAssetRef` was loaded. Listen to callback `V2_allCosmeticsInfoAssetRef_OnPostLoad` or check `v2_allCosmeticsInfoAssetRef_isLoaded` before trying to get PlayFab catalog data.");
			}
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
				{
					CatalogVersion = this.catalog
				}, delegate(GetCatalogItemsResult result2)
				{
					this.unlockedCosmetics.Clear();
					this.unlockedHats.Clear();
					this.unlockedBadges.Clear();
					this.unlockedFaces.Clear();
					this.unlockedPaws.Clear();
					this.unlockedFurs.Clear();
					this.unlockedShirts.Clear();
					this.unlockedPants.Clear();
					this.unlockedArms.Clear();
					this.unlockedBacks.Clear();
					this.unlockedChests.Clear();
					this.unlockedTagFX.Clear();
					this.catalogItems = result2.Catalog;
					using (List<CatalogItem>.Enumerator enumerator = this.catalogItems.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CatalogItem catalogItem = enumerator.Current;
							if (!BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId))
							{
								this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => catalogItem.ItemId == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempStringArray = null;
									this.hasPrice = false;
									if (catalogItem.Bundle != null)
									{
										this.tempStringArray = catalogItem.Bundle.BundledItems.ToArray();
									}
									uint num;
									if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out num))
									{
										this.hasPrice = true;
									}
									CosmeticsController.CosmeticItem cosmeticItem = this.allCosmetics[this.searchIndex];
									cosmeticItem.itemName = catalogItem.ItemId;
									cosmeticItem.displayName = catalogItem.DisplayName;
									cosmeticItem.cost = (int)num;
									cosmeticItem.bundledItems = this.tempStringArray;
									cosmeticItem.canTryOn = this.hasPrice;
									if (cosmeticItem.displayName == null)
									{
										string text = "null";
										if (this.allCosmetics[this.searchIndex].itemPicture)
										{
											text = this.allCosmetics[this.searchIndex].itemPicture.name;
										}
										string debugCosmeticSOName = this.v2_allCosmetics[this.searchIndex].debugCosmeticSOName;
										Debug.LogError(string.Concat(new string[]
										{
											string.Format("Cosmetic encountered with a null displayName at index {0}! ", this.searchIndex),
											"Setting displayName to id: \"",
											this.allCosmetics[this.searchIndex].itemName,
											"\". iconName=\"",
											text,
											"\".cosmeticSOName=\"",
											debugCosmeticSOName,
											"\". "
										}));
										cosmeticItem.displayName = cosmeticItem.itemName;
									}
									this.V2_ConformCosmeticItemV1DisplayName(ref cosmeticItem);
									this._allCosmetics[this.searchIndex] = cosmeticItem;
									this._allCosmeticsDict[cosmeticItem.itemName] = cosmeticItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[cosmeticItem.displayName] = cosmeticItem.itemName;
									this._allCosmeticsItemIDsfromDisplayNamesDict[cosmeticItem.overrideDisplayName] = cosmeticItem.itemName;
								}
							}
						}
					}
					for (int i = this._allCosmetics.Count - 1; i > -1; i--)
					{
						this.tempItem = this._allCosmetics[i];
						if (this.tempItem.itemCategory == CosmeticsController.CosmeticCategory.Set && this.tempItem.canTryOn)
						{
							string[] array = this.tempItem.bundledItems;
							for (int j = 0; j < array.Length; j++)
							{
								string setItemName2 = array[j];
								this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName2 == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempItem = this._allCosmetics[this.searchIndex];
									this.tempItem.canTryOn = true;
									this._allCosmetics[this.searchIndex] = this.tempItem;
									this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
								}
							}
						}
					}
					foreach (KeyValuePair<string, StoreBundle> keyValuePair in BundleManager.instance.storeBundlesById)
					{
						string text2;
						StoreBundle storeBundle;
						keyValuePair.Deconstruct(out text2, out storeBundle);
						string text3 = text2;
						StoreBundle bundleData = storeBundle;
						int num2 = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => bundleData.playfabBundleID == x.itemName);
						if (num2 > 0 && this._allCosmetics[num2].bundledItems != null)
						{
							string[] array = this._allCosmetics[num2].bundledItems;
							for (int j = 0; j < array.Length; j++)
							{
								string setItemName = array[j];
								this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempItem = this._allCosmetics[this.searchIndex];
									this.tempItem.canTryOn = true;
									this._allCosmetics[this.searchIndex] = this.tempItem;
									this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
								}
							}
						}
						if (!bundleData.HasPrice)
						{
							num2 = this.catalogItems.FindIndex((CatalogItem ci) => ci.Bundle != null && ci.ItemId == bundleData.playfabBundleID);
							if (num2 > 0)
							{
								uint num3;
								if (this.catalogItems[num2].VirtualCurrencyPrices.TryGetValue("RM", out num3))
								{
									BundleManager.instance.storeBundlesById[text3].TryUpdatePrice(num3);
								}
								else
								{
									BundleManager.instance.storeBundlesById[text3].TryUpdatePrice(null);
								}
							}
						}
					}
					this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => "Slingshot" == x.itemName);
					if (this.searchIndex < 0)
					{
						throw new MissingReferenceException("CosmeticsController: Cannot find default slingshot! it is required for players that do not have another slingshot equipped and are playing Paintbrawl.");
					}
					this._allCosmeticsDict["Slingshot"] = this._allCosmetics[this.searchIndex];
					this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this._allCosmetics[this.searchIndex].itemName;
					this.allCosmeticsDict_isInitialized = true;
					this.allCosmeticsItemIDsfromDisplayNamesDict_isInitialized = true;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					using (List<ItemInstance>.Enumerator enumerator3 = result.Inventory.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							ItemInstance item = enumerator3.Current;
							if (!BuilderSetManager.IsItemIDBuilderItem(item.ItemId))
							{
								if (item.ItemId == this.m_earlyAccessSupporterPackCosmeticSO.info.playFabID)
								{
									foreach (CosmeticSO cosmeticSO in this.m_earlyAccessSupporterPackCosmeticSO.info.setCosmetics)
									{
										CosmeticsController.CosmeticItem cosmeticItem2;
										if (this.allCosmeticsDict.TryGetValue(cosmeticSO.info.playFabID, out cosmeticItem2))
										{
											this.unlockedCosmetics.Add(cosmeticItem2);
										}
									}
								}
								BundleManager.instance.MarkBundleOwnedByPlayFabID(item.ItemId);
								if (!dictionary.ContainsKey(item.ItemId))
								{
									this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.ItemId == x.itemName);
									if (this.searchIndex > -1)
									{
										dictionary[item.ItemId] = item.ItemId;
										this.unlockedCosmetics.Add(this.allCosmetics[this.searchIndex]);
									}
								}
							}
						}
					}
					foreach (CosmeticsController.CosmeticItem cosmeticItem3 in this.unlockedCosmetics)
					{
						if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Hat && !this.unlockedHats.Contains(cosmeticItem3))
						{
							this.unlockedHats.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Face && !this.unlockedFaces.Contains(cosmeticItem3))
						{
							this.unlockedFaces.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Badge && !this.unlockedBadges.Contains(cosmeticItem3))
						{
							this.unlockedBadges.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Paw && !this.unlockedPaws.Contains(cosmeticItem3))
						{
							this.unlockedPaws.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Fur && !this.unlockedFurs.Contains(cosmeticItem3))
						{
							this.unlockedFurs.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Shirt && !this.unlockedShirts.Contains(cosmeticItem3))
						{
							this.unlockedShirts.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Arms && !this.unlockedArms.Contains(cosmeticItem3))
						{
							this.unlockedArms.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Back && !this.unlockedBacks.Contains(cosmeticItem3))
						{
							this.unlockedBacks.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Chest && !this.unlockedChests.Contains(cosmeticItem3))
						{
							this.unlockedChests.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Pants && !this.unlockedPants.Contains(cosmeticItem3))
						{
							this.unlockedPants.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.TagEffect && !this.unlockedTagFX.Contains(cosmeticItem3))
						{
							this.unlockedTagFX.Add(cosmeticItem3);
						}
						this.concatStringCosmeticsAllowed += cosmeticItem3.itemName;
					}
					BuilderSetManager.instance.OnGotInventoryItems(result, result2);
					this.currencyBalance = result.VirtualCurrency[this.currencyName];
					int num4;
					this.playedInBeta = result.VirtualCurrency.TryGetValue("TC", out num4) && num4 > 0;
					Action onGetCurrency = this.OnGetCurrency;
					if (onGetCurrency != null)
					{
						onGetCurrency();
					}
					BundleManager.instance.CheckIfBundlesOwned();
					StoreUpdater.instance.Initialize();
					this.currentWornSet.LoadFromPlayerPreferences(this);
					if (!ATM_Manager.instance.alreadyBegan)
					{
						ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Begin);
						ATM_Manager.instance.alreadyBegan = true;
					}
					this.ProcessPurchaseItemState(null, false);
					this.UpdateShoppingCart();
					this.UpdateCurrencyBoard();
					if (this.UseNewCosmeticsPath())
					{
						this.ConfirmIndividualCosmeticsSharedGroup(result);
					}
					Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
					if (onCosmeticsUpdated != null)
					{
						onCosmeticsUpdated();
					}
					this.v2_isCosmeticPlayFabCatalogDataLoaded = true;
					Action v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess = this.V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;
					if (v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess != null)
					{
						v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess();
					}
					if (!CosmeticsV2Spawner_Dirty.startedAllPartsInstantiated && !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
					{
						CosmeticsV2Spawner_Dirty.StartInstantiatingPrefabs();
					}
				}, delegate(PlayFabError error)
				{
					if (error.Error == PlayFabErrorCode.NotAuthenticated)
					{
						PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					}
					else if (error.Error == PlayFabErrorCode.AccountBanned)
					{
						Application.Quit();
						NetworkSystem.Instance.ReturnToSinglePlayer();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GTPlayer.Instance);
						GameObject[] array2 = Object.FindObjectsOfType<GameObject>();
						for (int k = 0; k < array2.Length; k++)
						{
							Object.Destroy(array2[k]);
						}
					}
					if (!this.tryTwice)
					{
						this.tryTwice = true;
						this.GetCosmeticsPlayFabCatalogData();
					}
				}, null, null);
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}
				else if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array3 = Object.FindObjectsOfType<GameObject>();
					for (int l = 0; l < array3.Length; l++)
					{
						Object.Destroy(array3[l]);
					}
				}
				if (!this.tryTwice)
				{
					this.tryTwice = true;
					this.GetCosmeticsPlayFabCatalogData();
				}
			}, null, null);
		}

		// Token: 0x06004C83 RID: 19587 RVA: 0x0016B774 File Offset: 0x00169974
		public void SteamPurchase()
		{
			if (string.IsNullOrEmpty(this.itemToPurchase))
			{
				Debug.Log("Unable to start steam purchase process. itemToPurchase is not set.");
				return;
			}
			Debug.Log(string.Format("attempting to purchase item through steam. Is this a bundle purchase: {0}", this.buyingBundle));
			PlayFabClientAPI.StartPurchase(this.GetStartPurchaseRequest(), new Action<StartPurchaseResult>(this.ProcessStartPurchaseResponse), new Action<PlayFabError>(this.ProcessSteamPurchaseError), null, null);
		}

		// Token: 0x06004C84 RID: 19588 RVA: 0x0016B7D8 File Offset: 0x001699D8
		private StartPurchaseRequest GetStartPurchaseRequest()
		{
			return new StartPurchaseRequest
			{
				CatalogVersion = this.catalog,
				Items = new List<ItemPurchaseRequest>
				{
					new ItemPurchaseRequest
					{
						ItemId = this.itemToPurchase,
						Quantity = 1U,
						Annotation = "Purchased via in-game store"
					}
				}
			};
		}

		// Token: 0x06004C85 RID: 19589 RVA: 0x0016B82C File Offset: 0x00169A2C
		private void ProcessStartPurchaseResponse(StartPurchaseResult result)
		{
			Debug.Log("successfully started purchase. attempted to pay for purchase through steam");
			this.currentPurchaseID = result.OrderId;
			PlayFabClientAPI.PayForPurchase(CosmeticsController.GetPayForPurchaseRequest(this.currentPurchaseID), new Action<PayForPurchaseResult>(CosmeticsController.ProcessPayForPurchaseResult), new Action<PlayFabError>(this.ProcessSteamPurchaseError), null, null);
		}

		// Token: 0x06004C86 RID: 19590 RVA: 0x0016B879 File Offset: 0x00169A79
		private static PayForPurchaseRequest GetPayForPurchaseRequest(string orderId)
		{
			return new PayForPurchaseRequest
			{
				OrderId = orderId,
				ProviderName = "Steam",
				Currency = "RM"
			};
		}

		// Token: 0x06004C87 RID: 19591 RVA: 0x0016B89D File Offset: 0x00169A9D
		private static void ProcessPayForPurchaseResult(PayForPurchaseResult result)
		{
			Debug.Log("succeeded on sending request for paying with steam! waiting for response");
		}

		// Token: 0x06004C88 RID: 19592 RVA: 0x0016B8AC File Offset: 0x00169AAC
		private void ProcessSteamCallback(MicroTxnAuthorizationResponse_t callBackResponse)
		{
			Debug.Log("Steam has called back that the user has finished the payment interaction");
			if (callBackResponse.m_bAuthorized == 0)
			{
				Debug.Log("Steam has indicated that the payment was not authorised.");
			}
			if (this.buyingBundle)
			{
				PlayFabClientAPI.ConfirmPurchase(this.GetConfirmBundlePurchaseRequest(), delegate(ConfirmPurchaseResult _)
				{
					this.ProcessConfirmPurchaseSuccess();
				}, new Action<PlayFabError>(this.ProcessConfirmPurchaseError), null, null);
				return;
			}
			PlayFabClientAPI.ConfirmPurchase(this.GetConfirmATMPurchaseRequest(), delegate(ConfirmPurchaseResult _)
			{
				this.ProcessConfirmPurchaseSuccess();
			}, new Action<PlayFabError>(this.ProcessConfirmPurchaseError), null, null);
		}

		// Token: 0x06004C89 RID: 19593 RVA: 0x0016B928 File Offset: 0x00169B28
		private ConfirmPurchaseRequest GetConfirmBundlePurchaseRequest()
		{
			return new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID
			};
		}

		// Token: 0x06004C8A RID: 19594 RVA: 0x0016B93C File Offset: 0x00169B3C
		private ConfirmPurchaseRequest GetConfirmATMPurchaseRequest()
		{
			return new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID,
				CustomTags = new Dictionary<string, string>
				{
					{
						"NexusCreatorId",
						ATM_Manager.instance.ValidatedCreatorCode
					},
					{
						"PlayerName",
						GorillaComputer.instance.savedName
					}
				}
			};
		}

		// Token: 0x06004C8B RID: 19595 RVA: 0x0016B994 File Offset: 0x00169B94
		private void ProcessConfirmPurchaseSuccess()
		{
			if (this.buyingBundle)
			{
				this.buyingBundle = false;
				if (PhotonNetwork.InRoom)
				{
					object[] array = new object[0];
					NetworkSystemRaiseEvent.RaiseEvent(9, array, NetworkSystemRaiseEvent.newWeb, true);
				}
				base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.BundlePlayfabItemName));
			}
			else
			{
				ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Success);
			}
			this.GetCurrencyBalance();
			this.UpdateCurrencyBoard();
			this.GetCosmeticsPlayFabCatalogData();
			GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
		}

		// Token: 0x06004C8C RID: 19596 RVA: 0x0016BA0F File Offset: 0x00169C0F
		private void ProcessConfirmPurchaseError(PlayFabError error)
		{
			this.ProcessSteamPurchaseError(error);
			ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
			this.UpdateCurrencyBoard();
		}

		// Token: 0x06004C8D RID: 19597 RVA: 0x0016BA2C File Offset: 0x00169C2C
		private void ProcessSteamPurchaseError(PlayFabError error)
		{
			PlayFabErrorCode error2 = error.Error;
			if (error2 <= PlayFabErrorCode.PurchaseInitializationFailure)
			{
				if (error2 <= PlayFabErrorCode.FailedByPaymentProvider)
				{
					if (error2 == PlayFabErrorCode.AccountBanned)
					{
						PhotonNetwork.Disconnect();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GTPlayer.Instance);
						GameObject[] array = Object.FindObjectsOfType<GameObject>();
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
						Application.Quit();
						goto IL_01A1;
					}
					if (error2 != PlayFabErrorCode.FailedByPaymentProvider)
					{
						goto IL_0191;
					}
					Debug.Log(string.Format("Attempted to pay for order, but has been Failed by Steam with error: {0}", error));
					goto IL_01A1;
				}
				else
				{
					if (error2 == PlayFabErrorCode.InsufficientFunds)
					{
						Debug.Log(string.Format("Attempting to do purchase through steam, steam has returned insufficient funds: {0}", error));
						goto IL_01A1;
					}
					if (error2 == PlayFabErrorCode.InvalidPaymentProvider)
					{
						Debug.Log(string.Format("Attempted to connect to steam as payment provider, but received error: {0}", error));
						goto IL_01A1;
					}
					if (error2 != PlayFabErrorCode.PurchaseInitializationFailure)
					{
						goto IL_0191;
					}
				}
			}
			else if (error2 <= PlayFabErrorCode.InvalidPurchaseTransactionStatus)
			{
				if (error2 == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					goto IL_01A1;
				}
				if (error2 == PlayFabErrorCode.PurchaseDoesNotExist)
				{
					Debug.Log(string.Format("Attempting to confirm purchase for order {0} but received error: {1}", this.currentPurchaseID, error));
					goto IL_01A1;
				}
				if (error2 != PlayFabErrorCode.InvalidPurchaseTransactionStatus)
				{
					goto IL_0191;
				}
			}
			else
			{
				if (error2 == PlayFabErrorCode.InternalServerError)
				{
					Debug.Log(string.Format("PlayFab threw an internal server error: {0}", error));
					goto IL_01A1;
				}
				if (error2 == PlayFabErrorCode.StoreNotFound)
				{
					Debug.Log(string.Format("Attempted to load {0} from {1} but received an error: {2}", this.itemToPurchase, this.catalog, error));
					goto IL_01A1;
				}
				if (error2 != PlayFabErrorCode.DuplicatePurchaseTransactionId)
				{
					goto IL_0191;
				}
			}
			Debug.Log(string.Format("Attempted to pay for order {0}, however received an error: {1}", this.currentPurchaseID, error));
			goto IL_01A1;
			IL_0191:
			Debug.Log(string.Format("Steam purchase flow returned error: {0}", error));
			IL_01A1:
			ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
		}

		// Token: 0x06004C8E RID: 19598 RVA: 0x0016BBE8 File Offset: 0x00169DE8
		public void UpdateCurrencyBoard()
		{
			this.FormattedPurchaseText(this.finalLine);
			this.dailyText.text = (this.checkedDaily ? (this.gotMyDaily ? "SUCCESSFULLY GOT DAILY ROCKS!" : "WAITING TO GET DAILY ROCKS...") : "CHECKING DAILY ROCKS...");
			this.currencyBoardText.text = string.Concat(new string[]
			{
				this.currencyBalance.ToString(),
				"\n\n",
				(this.secondsUntilTomorrow / 3600).ToString(),
				" HR, ",
				(this.secondsUntilTomorrow % 3600 / 60).ToString(),
				"MIN"
			});
		}

		// Token: 0x06004C8F RID: 19599 RVA: 0x0016BC9C File Offset: 0x00169E9C
		public void GetCurrencyBalance()
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				this.currencyBalance = result.VirtualCurrency[this.currencyName];
				this.UpdateCurrencyBoard();
				Action onGetCurrency = this.OnGetCurrency;
				if (onGetCurrency == null)
				{
					return;
				}
				onGetCurrency();
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsOfType<GameObject>();
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x06004C90 RID: 19600 RVA: 0x0016BCD5 File Offset: 0x00169ED5
		public string GetItemDisplayName(CosmeticsController.CosmeticItem item)
		{
			if (item.overrideDisplayName != null && item.overrideDisplayName != "")
			{
				return item.overrideDisplayName;
			}
			return item.displayName;
		}

		// Token: 0x06004C91 RID: 19601 RVA: 0x0016BD00 File Offset: 0x00169F00
		public void UpdateMyCosmetics()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldSetSharedGroupData())
				{
					this.UpdateMyCosmeticsForRoom(true);
				}
				if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldSetRoomData())
				{
					this.UpdateMyCosmeticsForRoom(false);
					return;
				}
			}
			else if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldSetSharedGroupData())
			{
				this.UpdateMyCosmeticsNotInRoom();
			}
		}

		// Token: 0x06004C92 RID: 19602 RVA: 0x0016BD85 File Offset: 0x00169F85
		private void UpdateMyCosmeticsNotInRoom()
		{
			if (GorillaServer.Instance != null)
			{
				GorillaServer.Instance.UpdateUserCosmetics();
			}
		}

		// Token: 0x06004C93 RID: 19603 RVA: 0x0016BDA4 File Offset: 0x00169FA4
		private void UpdateMyCosmeticsForRoom(bool shouldSetSharedGroupData)
		{
			byte b = 9;
			if (shouldSetSharedGroupData)
			{
				b = 10;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			WebFlags webFlags = new WebFlags(1);
			raiseEventOptions.Flags = webFlags;
			object[] array = new object[0];
			PhotonNetwork.RaiseEvent(b, array, raiseEventOptions, SendOptions.SendReliable);
		}

		// Token: 0x06004C94 RID: 19604 RVA: 0x0016BDE4 File Offset: 0x00169FE4
		private void AlreadyOwnAllBundleButtons()
		{
			EarlyAccessButton[] array = this.earlyAccessButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AlreadyOwn();
			}
		}

		// Token: 0x06004C95 RID: 19605 RVA: 0x0016BE0E File Offset: 0x0016A00E
		private bool UseNewCosmeticsPath()
		{
			return GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldReadSharedGroupData();
		}

		// Token: 0x06004C96 RID: 19606 RVA: 0x0016BE2D File Offset: 0x0016A02D
		public void CheckCosmeticsSharedGroup()
		{
			this.updateCosmeticsRetries++;
			if (this.updateCosmeticsRetries < this.maxUpdateCosmeticsRetries)
			{
				base.StartCoroutine(this.WaitForNextCosmeticsAttempt());
			}
		}

		// Token: 0x06004C97 RID: 19607 RVA: 0x0016BE58 File Offset: 0x0016A058
		private IEnumerator WaitForNextCosmeticsAttempt()
		{
			int num = (int)Mathf.Pow(3f, (float)(this.updateCosmeticsRetries + 1));
			yield return new WaitForSeconds((float)num);
			this.ConfirmIndividualCosmeticsSharedGroup(this.latestInventory);
			yield break;
		}

		// Token: 0x06004C98 RID: 19608 RVA: 0x0016BE68 File Offset: 0x0016A068
		private void ConfirmIndividualCosmeticsSharedGroup(GetUserInventoryResult inventory)
		{
			Debug.Log("confirming individual cosmetics with shared group");
			this.latestInventory = inventory;
			if (PhotonNetwork.LocalPlayer.UserId == null)
			{
				base.StartCoroutine(this.WaitForNextCosmeticsAttempt());
				return;
			}
			PlayFabClientAPI.GetSharedGroupData(new global::PlayFab.ClientModels.GetSharedGroupDataRequest
			{
				Keys = this.inventoryStringList,
				SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
			}, delegate(GetSharedGroupDataResult result)
			{
				bool flag = true;
				foreach (KeyValuePair<string, global::PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
				{
					if (keyValuePair.Key != "Inventory")
					{
						break;
					}
					foreach (ItemInstance itemInstance in inventory.Inventory)
					{
						if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog && !keyValuePair.Value.Value.Contains(itemInstance.ItemId))
						{
							flag = false;
							break;
						}
					}
				}
				if (!flag || result.Data.Count == 0)
				{
					this.UpdateMyCosmetics();
					return;
				}
				this.updateCosmeticsRetries = 0;
			}, delegate(PlayFabError error)
			{
				this.ReauthOrBan(error);
				this.CheckCosmeticsSharedGroup();
			}, null, null);
		}

		// Token: 0x06004C99 RID: 19609 RVA: 0x0016BF04 File Offset: 0x0016A104
		public void ReauthOrBan(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				Application.Quit();
				PhotonNetwork.Disconnect();
				Object.DestroyImmediate(PhotonNetworkController.Instance);
				Object.DestroyImmediate(GTPlayer.Instance);
				GameObject[] array = Object.FindObjectsOfType<GameObject>();
				for (int i = 0; i < array.Length; i++)
				{
					Object.Destroy(array[i]);
				}
			}
		}

		// Token: 0x06004C9A RID: 19610 RVA: 0x0016BF74 File Offset: 0x0016A174
		public void ProcessExternalUnlock(string itemID, bool autoEquip, bool isLeftHand)
		{
			Debug.Log("[ProcessExternalUnlock] Processing external unlock...");
			this.UnlockItem(itemID);
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			offlineVRRig.concatStringOfCosmeticsAllowed += itemID;
			this.UpdateMyCosmetics();
			if (autoEquip)
			{
				Debug.Log("[ProcessExternalUnlock] Auto-equipping item...");
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(itemID);
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.external_item_claim, itemFromDict);
				List<CosmeticsController.CosmeticSlots> list = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
				if (list.Capacity < 16)
				{
					list.Capacity = 16;
				}
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true, list);
				foreach (CosmeticsController.CosmeticSlots cosmeticSlots in list)
				{
					this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
				}
				CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(list);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics(true);
				Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
				if (onCosmeticsUpdated == null)
				{
					return;
				}
				onCosmeticsUpdated();
			}
		}

		// Token: 0x06004C9B RID: 19611 RVA: 0x0016C078 File Offset: 0x0016A278
		public bool BuildValidationCheck()
		{
			if (this.m_earlyAccessSupporterPackCosmeticSO == null)
			{
				Debug.LogError("m_earlyAccessSupporterPackCosmeticSO is empty, everything will break!");
				return false;
			}
			return true;
		}

		// Token: 0x06004C9C RID: 19612 RVA: 0x0016C095 File Offset: 0x0016A295
		public void SetHideCosmeticsFromRemotePlayers(bool hideCosmetics)
		{
			if (hideCosmetics == this.isHidingCosmeticsFromRemotePlayers)
			{
				return;
			}
			this.isHidingCosmeticsFromRemotePlayers = hideCosmetics;
			GorillaTagger.Instance.offlineVRRig.reliableState.SetIsDirty();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06004C9D RID: 19613 RVA: 0x0016C0C4 File Offset: 0x0016A2C4
		public bool ValidatePackedItems(int[] packed)
		{
			if (packed.Length == 0)
			{
				return true;
			}
			int num = 0;
			int num2 = packed[0];
			for (int i = 0; i < 16; i++)
			{
				if ((num2 & (1 << i)) != 0)
				{
					num++;
				}
			}
			return packed.Length == num + 1;
		}

		// Token: 0x06004CA0 RID: 19616 RVA: 0x00011040 File Offset: 0x0000F240
		bool IGorillaSliceableSimple.get_isActiveAndEnabled()
		{
			return base.isActiveAndEnabled;
		}

		// Token: 0x04004F1D RID: 20253
		[FormerlySerializedAs("v2AllCosmeticsInfoAssetRef")]
		[FormerlySerializedAs("newSysAllCosmeticsAssetRef")]
		[SerializeField]
		public GTAssetRef<AllCosmeticsArraySO> v2_allCosmeticsInfoAssetRef;

		// Token: 0x04004F1F RID: 20255
		private readonly Dictionary<string, CosmeticInfoV2> _allCosmeticsDictV2 = new Dictionary<string, CosmeticInfoV2>();

		// Token: 0x04004F20 RID: 20256
		public Action V2_allCosmeticsInfoAssetRef_OnPostLoad;

		// Token: 0x04004F24 RID: 20260
		public const int maximumTransferrableItems = 5;

		// Token: 0x04004F25 RID: 20261
		[OnEnterPlay_SetNull]
		public static volatile CosmeticsController instance;

		// Token: 0x04004F27 RID: 20263
		public Action V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;

		// Token: 0x04004F28 RID: 20264
		public Action OnGetCurrency;

		// Token: 0x04004F29 RID: 20265
		[FormerlySerializedAs("allCosmetics")]
		[SerializeField]
		private List<CosmeticsController.CosmeticItem> _allCosmetics;

		// Token: 0x04004F2B RID: 20267
		public Dictionary<string, CosmeticsController.CosmeticItem> _allCosmeticsDict = new Dictionary<string, CosmeticsController.CosmeticItem>(2048);

		// Token: 0x04004F2D RID: 20269
		public Dictionary<string, string> _allCosmeticsItemIDsfromDisplayNamesDict = new Dictionary<string, string>(2048);

		// Token: 0x04004F2E RID: 20270
		public CosmeticsController.CosmeticItem nullItem;

		// Token: 0x04004F2F RID: 20271
		public string catalog;

		// Token: 0x04004F30 RID: 20272
		private string[] tempStringArray;

		// Token: 0x04004F31 RID: 20273
		private CosmeticsController.CosmeticItem tempItem;

		// Token: 0x04004F32 RID: 20274
		private VRRigAnchorOverrides anchorOverrides;

		// Token: 0x04004F33 RID: 20275
		public List<CatalogItem> catalogItems;

		// Token: 0x04004F34 RID: 20276
		public bool tryTwice;

		// Token: 0x04004F35 RID: 20277
		[NonSerialized]
		public CosmeticsController.CosmeticSet tryOnSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04004F36 RID: 20278
		public FittingRoomButton[] fittingRoomButtons;

		// Token: 0x04004F37 RID: 20279
		public CosmeticStand[] cosmeticStands;

		// Token: 0x04004F38 RID: 20280
		public List<CosmeticsController.CosmeticItem> currentCart = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x04004F39 RID: 20281
		public CosmeticsController.PurchaseItemStages currentPurchaseItemStage;

		// Token: 0x04004F3A RID: 20282
		public CheckoutCartButton[] checkoutCartButtons;

		// Token: 0x04004F3B RID: 20283
		public PurchaseItemButton leftPurchaseButton;

		// Token: 0x04004F3C RID: 20284
		public PurchaseItemButton rightPurchaseButton;

		// Token: 0x04004F3D RID: 20285
		public Text purchaseText;

		// Token: 0x04004F3E RID: 20286
		public CosmeticsController.CosmeticItem itemToBuy;

		// Token: 0x04004F3F RID: 20287
		public HeadModel checkoutHeadModel;

		// Token: 0x04004F40 RID: 20288
		private List<string> playerIDList = new List<string>();

		// Token: 0x04004F41 RID: 20289
		private List<string> inventoryStringList = new List<string>();

		// Token: 0x04004F42 RID: 20290
		private bool foundCosmetic;

		// Token: 0x04004F43 RID: 20291
		private int attempts;

		// Token: 0x04004F44 RID: 20292
		private string finalLine;

		// Token: 0x04004F45 RID: 20293
		private bool isLastHandTouchedLeft;

		// Token: 0x04004F46 RID: 20294
		private CosmeticsController.CosmeticSet cachedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04004F48 RID: 20296
		public readonly List<WardrobeInstance> wardrobes = new List<WardrobeInstance>();

		// Token: 0x04004F49 RID: 20297
		public List<CosmeticsController.CosmeticItem> unlockedCosmetics = new List<CosmeticsController.CosmeticItem>(2048);

		// Token: 0x04004F4A RID: 20298
		public List<CosmeticsController.CosmeticItem> unlockedHats = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F4B RID: 20299
		public List<CosmeticsController.CosmeticItem> unlockedFaces = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F4C RID: 20300
		public List<CosmeticsController.CosmeticItem> unlockedBadges = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F4D RID: 20301
		public List<CosmeticsController.CosmeticItem> unlockedPaws = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F4E RID: 20302
		public List<CosmeticsController.CosmeticItem> unlockedChests = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F4F RID: 20303
		public List<CosmeticsController.CosmeticItem> unlockedFurs = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F50 RID: 20304
		public List<CosmeticsController.CosmeticItem> unlockedShirts = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F51 RID: 20305
		public List<CosmeticsController.CosmeticItem> unlockedPants = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F52 RID: 20306
		public List<CosmeticsController.CosmeticItem> unlockedBacks = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F53 RID: 20307
		public List<CosmeticsController.CosmeticItem> unlockedArms = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F54 RID: 20308
		public List<CosmeticsController.CosmeticItem> unlockedTagFX = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04004F55 RID: 20309
		public int[] cosmeticsPages = new int[11];

		// Token: 0x04004F56 RID: 20310
		private List<CosmeticsController.CosmeticItem>[] itemLists = new List<CosmeticsController.CosmeticItem>[11];

		// Token: 0x04004F57 RID: 20311
		private int wardrobeType;

		// Token: 0x04004F58 RID: 20312
		[NonSerialized]
		public CosmeticsController.CosmeticSet currentWornSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04004F59 RID: 20313
		public string concatStringCosmeticsAllowed = "";

		// Token: 0x04004F5A RID: 20314
		public Action OnCosmeticsUpdated;

		// Token: 0x04004F5B RID: 20315
		public Text infoText;

		// Token: 0x04004F5C RID: 20316
		public Text earlyAccessText;

		// Token: 0x04004F5D RID: 20317
		public Text[] purchaseButtonText;

		// Token: 0x04004F5E RID: 20318
		public Text dailyText;

		// Token: 0x04004F5F RID: 20319
		public int currencyBalance;

		// Token: 0x04004F60 RID: 20320
		public string currencyName;

		// Token: 0x04004F61 RID: 20321
		public PurchaseCurrencyButton[] purchaseCurrencyButtons;

		// Token: 0x04004F62 RID: 20322
		public Text currencyBoardText;

		// Token: 0x04004F63 RID: 20323
		public Text currencyBoxText;

		// Token: 0x04004F64 RID: 20324
		public string startingCurrencyBoxTextString;

		// Token: 0x04004F65 RID: 20325
		public string successfulCurrencyPurchaseTextString;

		// Token: 0x04004F66 RID: 20326
		public string itemToPurchase;

		// Token: 0x04004F67 RID: 20327
		public bool buyingBundle;

		// Token: 0x04004F68 RID: 20328
		public bool confirmedDidntPlayInBeta;

		// Token: 0x04004F69 RID: 20329
		public bool playedInBeta;

		// Token: 0x04004F6A RID: 20330
		public bool gotMyDaily;

		// Token: 0x04004F6B RID: 20331
		public bool checkedDaily;

		// Token: 0x04004F6C RID: 20332
		public string currentPurchaseID;

		// Token: 0x04004F6D RID: 20333
		public bool hasPrice;

		// Token: 0x04004F6E RID: 20334
		private int searchIndex;

		// Token: 0x04004F6F RID: 20335
		private int iterator;

		// Token: 0x04004F70 RID: 20336
		private CosmeticsController.CosmeticItem cosmeticItemVar;

		// Token: 0x04004F71 RID: 20337
		[SerializeField]
		private CosmeticSO m_earlyAccessSupporterPackCosmeticSO;

		// Token: 0x04004F72 RID: 20338
		public EarlyAccessButton[] earlyAccessButtons;

		// Token: 0x04004F73 RID: 20339
		private BundleList bundleList = new BundleList();

		// Token: 0x04004F74 RID: 20340
		public string BundleSkuName = "2024_i_lava_you_pack";

		// Token: 0x04004F75 RID: 20341
		public string BundlePlayfabItemName = "LSABG.";

		// Token: 0x04004F76 RID: 20342
		public int BundleShinyRocks = 10000;

		// Token: 0x04004F77 RID: 20343
		public DateTime currentTime;

		// Token: 0x04004F78 RID: 20344
		public string lastDailyLogin;

		// Token: 0x04004F79 RID: 20345
		public UserDataRecord userDataRecord;

		// Token: 0x04004F7A RID: 20346
		public int secondsUntilTomorrow;

		// Token: 0x04004F7B RID: 20347
		public float secondsToWaitToCheckDaily = 10f;

		// Token: 0x04004F7C RID: 20348
		private int updateCosmeticsRetries;

		// Token: 0x04004F7D RID: 20349
		private int maxUpdateCosmeticsRetries;

		// Token: 0x04004F7E RID: 20350
		private GetUserInventoryResult latestInventory;

		// Token: 0x04004F7F RID: 20351
		private string returnString;

		// Token: 0x04004F80 RID: 20352
		private Callback<MicroTxnAuthorizationResponse_t> _steamMicroTransactionAuthorizationResponse;

		// Token: 0x04004F81 RID: 20353
		private static readonly List<CosmeticsController.CosmeticSlots> _g_default_outAppliedSlotsList_for_applyCosmeticItemToSet = new List<CosmeticsController.CosmeticSlots>(16);

		// Token: 0x02000C0F RID: 3087
		public enum PurchaseItemStages
		{
			// Token: 0x04004F83 RID: 20355
			Start,
			// Token: 0x04004F84 RID: 20356
			CheckoutButtonPressed,
			// Token: 0x04004F85 RID: 20357
			ItemSelected,
			// Token: 0x04004F86 RID: 20358
			ItemOwned,
			// Token: 0x04004F87 RID: 20359
			FinalPurchaseAcknowledgement,
			// Token: 0x04004F88 RID: 20360
			Buying,
			// Token: 0x04004F89 RID: 20361
			Success,
			// Token: 0x04004F8A RID: 20362
			Failure
		}

		// Token: 0x02000C10 RID: 3088
		public enum CosmeticCategory
		{
			// Token: 0x04004F8C RID: 20364
			None,
			// Token: 0x04004F8D RID: 20365
			Hat,
			// Token: 0x04004F8E RID: 20366
			Badge,
			// Token: 0x04004F8F RID: 20367
			Face,
			// Token: 0x04004F90 RID: 20368
			Paw,
			// Token: 0x04004F91 RID: 20369
			Chest,
			// Token: 0x04004F92 RID: 20370
			Fur,
			// Token: 0x04004F93 RID: 20371
			Shirt,
			// Token: 0x04004F94 RID: 20372
			Back,
			// Token: 0x04004F95 RID: 20373
			Arms,
			// Token: 0x04004F96 RID: 20374
			Pants,
			// Token: 0x04004F97 RID: 20375
			TagEffect,
			// Token: 0x04004F98 RID: 20376
			Count,
			// Token: 0x04004F99 RID: 20377
			Set
		}

		// Token: 0x02000C11 RID: 3089
		public enum CosmeticSlots
		{
			// Token: 0x04004F9B RID: 20379
			Hat,
			// Token: 0x04004F9C RID: 20380
			Badge,
			// Token: 0x04004F9D RID: 20381
			Face,
			// Token: 0x04004F9E RID: 20382
			ArmLeft,
			// Token: 0x04004F9F RID: 20383
			ArmRight,
			// Token: 0x04004FA0 RID: 20384
			BackLeft,
			// Token: 0x04004FA1 RID: 20385
			BackRight,
			// Token: 0x04004FA2 RID: 20386
			HandLeft,
			// Token: 0x04004FA3 RID: 20387
			HandRight,
			// Token: 0x04004FA4 RID: 20388
			Chest,
			// Token: 0x04004FA5 RID: 20389
			Fur,
			// Token: 0x04004FA6 RID: 20390
			Shirt,
			// Token: 0x04004FA7 RID: 20391
			Pants,
			// Token: 0x04004FA8 RID: 20392
			Back,
			// Token: 0x04004FA9 RID: 20393
			Arms,
			// Token: 0x04004FAA RID: 20394
			TagEffect,
			// Token: 0x04004FAB RID: 20395
			Count
		}

		// Token: 0x02000C12 RID: 3090
		[Serializable]
		public class CosmeticSet
		{
			// Token: 0x14000093 RID: 147
			// (add) Token: 0x06004CAE RID: 19630 RVA: 0x0016C6B0 File Offset: 0x0016A8B0
			// (remove) Token: 0x06004CAF RID: 19631 RVA: 0x0016C6E8 File Offset: 0x0016A8E8
			public event CosmeticsController.CosmeticSet.OnSetActivatedHandler onSetActivatedEvent;

			// Token: 0x06004CB0 RID: 19632 RVA: 0x0016C71D File Offset: 0x0016A91D
			protected void OnSetActivated(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer)
			{
				if (this.onSetActivatedEvent != null)
				{
					this.onSetActivatedEvent(prevSet, currentSet, netPlayer);
				}
			}

			// Token: 0x1700079D RID: 1949
			// (get) Token: 0x06004CB1 RID: 19633 RVA: 0x0016C738 File Offset: 0x0016A938
			public static CosmeticsController.CosmeticSet EmptySet
			{
				get
				{
					if (CosmeticsController.CosmeticSet._emptySet == null)
					{
						string[] array = new string[16];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = "NOTHING";
						}
						CosmeticsController.CosmeticSet._emptySet = new CosmeticsController.CosmeticSet(array, CosmeticsController.instance);
					}
					return CosmeticsController.CosmeticSet._emptySet;
				}
			}

			// Token: 0x06004CB2 RID: 19634 RVA: 0x0016C781 File Offset: 0x0016A981
			public CosmeticSet()
			{
				this.items = new CosmeticsController.CosmeticItem[16];
			}

			// Token: 0x06004CB3 RID: 19635 RVA: 0x0016C7A4 File Offset: 0x0016A9A4
			public CosmeticSet(string[] itemNames, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[16];
				for (int i = 0; i < itemNames.Length; i++)
				{
					string text = itemNames[i];
					string itemNameFromDisplayName = controller.GetItemNameFromDisplayName(text);
					this.items[i] = controller.GetItemFromDict(itemNameFromDisplayName);
				}
			}

			// Token: 0x06004CB4 RID: 19636 RVA: 0x0016C800 File Offset: 0x0016AA00
			public CosmeticSet(int[] itemNamesPacked, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[16];
				int num = ((itemNamesPacked.Length != 0) ? itemNamesPacked[0] : 0);
				int num2 = 1;
				for (int i = 0; i < this.items.Length; i++)
				{
					if ((num & (1 << i)) != 0)
					{
						int num3 = itemNamesPacked[num2];
						CosmeticsController.CosmeticSet.nameScratchSpace[0] = (char)(65 + num3 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[1] = (char)(65 + num3 / 26 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[2] = (char)(65 + num3 / 676 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[3] = (char)(65 + num3 / 17576 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[4] = (char)(65 + num3 / 456976 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[5] = '.';
						this.items[i] = controller.GetItemFromDict(new string(CosmeticsController.CosmeticSet.nameScratchSpace));
						num2++;
					}
					else
					{
						this.items[i] = controller.GetItemFromDict("null");
					}
				}
			}

			// Token: 0x06004CB5 RID: 19637 RVA: 0x0016C908 File Offset: 0x0016AB08
			public void CopyItems(CosmeticsController.CosmeticSet other)
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					this.items[i] = other.items[i];
				}
			}

			// Token: 0x06004CB6 RID: 19638 RVA: 0x0016C940 File Offset: 0x0016AB40
			public void MergeSets(CosmeticsController.CosmeticSet tryOn, CosmeticsController.CosmeticSet current)
			{
				for (int i = 0; i < 16; i++)
				{
					if (tryOn == null)
					{
						this.items[i] = current.items[i];
					}
					else
					{
						this.items[i] = (tryOn.items[i].isNullItem ? current.items[i] : tryOn.items[i]);
					}
				}
			}

			// Token: 0x06004CB7 RID: 19639 RVA: 0x0016C9B0 File Offset: 0x0016ABB0
			public void ClearSet(CosmeticsController.CosmeticItem nullItem)
			{
				for (int i = 0; i < 16; i++)
				{
					this.items[i] = nullItem;
				}
			}

			// Token: 0x06004CB8 RID: 19640 RVA: 0x0016C9D8 File Offset: 0x0016ABD8
			public bool IsActive(string name)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06004CB9 RID: 19641 RVA: 0x0016CA10 File Offset: 0x0016AC10
			public bool HasItemOfCategory(CosmeticsController.CosmeticCategory category)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].itemCategory == category)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06004CBA RID: 19642 RVA: 0x0016CA58 File Offset: 0x0016AC58
			public bool HasItem(string name)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06004CBB RID: 19643 RVA: 0x0016CAA3 File Offset: 0x0016ACA3
			public static bool IsSlotLeftHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.HandLeft;
			}

			// Token: 0x06004CBC RID: 19644 RVA: 0x0016CAB3 File Offset: 0x0016ACB3
			public static bool IsSlotRightHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmRight || slot == CosmeticsController.CosmeticSlots.BackRight || slot == CosmeticsController.CosmeticSlots.HandRight;
			}

			// Token: 0x06004CBD RID: 19645 RVA: 0x0016CAC3 File Offset: 0x0016ACC3
			public static bool IsHoldable(CosmeticsController.CosmeticItem item)
			{
				return item.isHoldable;
			}

			// Token: 0x06004CBE RID: 19646 RVA: 0x0016CACC File Offset: 0x0016ACCC
			public static CosmeticsController.CosmeticSlots OppositeSlot(CosmeticsController.CosmeticSlots slot)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Hat:
					return CosmeticsController.CosmeticSlots.Hat;
				case CosmeticsController.CosmeticSlots.Badge:
					return CosmeticsController.CosmeticSlots.Badge;
				case CosmeticsController.CosmeticSlots.Face:
					return CosmeticsController.CosmeticSlots.Face;
				case CosmeticsController.CosmeticSlots.ArmLeft:
					return CosmeticsController.CosmeticSlots.ArmRight;
				case CosmeticsController.CosmeticSlots.ArmRight:
					return CosmeticsController.CosmeticSlots.ArmLeft;
				case CosmeticsController.CosmeticSlots.BackLeft:
					return CosmeticsController.CosmeticSlots.BackRight;
				case CosmeticsController.CosmeticSlots.BackRight:
					return CosmeticsController.CosmeticSlots.BackLeft;
				case CosmeticsController.CosmeticSlots.HandLeft:
					return CosmeticsController.CosmeticSlots.HandRight;
				case CosmeticsController.CosmeticSlots.HandRight:
					return CosmeticsController.CosmeticSlots.HandLeft;
				case CosmeticsController.CosmeticSlots.Chest:
					return CosmeticsController.CosmeticSlots.Chest;
				case CosmeticsController.CosmeticSlots.Fur:
					return CosmeticsController.CosmeticSlots.Fur;
				case CosmeticsController.CosmeticSlots.Shirt:
					return CosmeticsController.CosmeticSlots.Shirt;
				case CosmeticsController.CosmeticSlots.Pants:
					return CosmeticsController.CosmeticSlots.Pants;
				case CosmeticsController.CosmeticSlots.Back:
					return CosmeticsController.CosmeticSlots.Back;
				case CosmeticsController.CosmeticSlots.Arms:
					return CosmeticsController.CosmeticSlots.Arms;
				case CosmeticsController.CosmeticSlots.TagEffect:
					return CosmeticsController.CosmeticSlots.TagEffect;
				default:
					return CosmeticsController.CosmeticSlots.Count;
				}
			}

			// Token: 0x06004CBF RID: 19647 RVA: 0x0016CB4A File Offset: 0x0016AD4A
			public static string SlotPlayerPreferenceName(CosmeticsController.CosmeticSlots slot)
			{
				return "slot_" + slot.ToString();
			}

			// Token: 0x06004CC0 RID: 19648 RVA: 0x0016CB64 File Offset: 0x0016AD64
			private void ActivateCosmetic(CosmeticsController.CosmeticSet prevSet, VRRig rig, int slotIndex, CosmeticItemRegistry cosmeticsObjectRegistry, BodyDockPositions bDock)
			{
				CosmeticsController.CosmeticItem cosmeticItem = prevSet.items[slotIndex];
				string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem.displayName);
				CosmeticsController.CosmeticItem cosmeticItem2 = this.items[slotIndex];
				string itemNameFromDisplayName2 = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem2.displayName);
				BodyDockPositions.DropPositions dropPositions = CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)slotIndex);
				if (cosmeticItem2.itemCategory != CosmeticsController.CosmeticCategory.None && !CosmeticsController.CompareCategoryToSavedCosmeticSlots(cosmeticItem2.itemCategory, (CosmeticsController.CosmeticSlots)slotIndex))
				{
					return;
				}
				if (cosmeticItem2.isHoldable && dropPositions == BodyDockPositions.DropPositions.None)
				{
					return;
				}
				if (!(itemNameFromDisplayName == itemNameFromDisplayName2))
				{
					if (!cosmeticItem.isNullItem)
					{
						if (cosmeticItem.isHoldable)
						{
							bDock.TransferrableItemDisableAtPosition(dropPositions);
						}
						CosmeticItemInstance cosmeticItemInstance = cosmeticsObjectRegistry.Cosmetic(cosmeticItem.displayName);
						if (cosmeticItemInstance != null)
						{
							cosmeticItemInstance.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
						}
					}
					if (!cosmeticItem2.isNullItem)
					{
						if (cosmeticItem2.isHoldable)
						{
							bDock.TransferrableItemEnableAtPosition(cosmeticItem2.displayName, dropPositions);
						}
						CosmeticItemInstance cosmeticItemInstance2 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
						if (rig.IsItemAllowed(itemNameFromDisplayName2) && cosmeticItemInstance2 != null)
						{
							cosmeticItemInstance2.EnableItem((CosmeticsController.CosmeticSlots)slotIndex);
						}
					}
					return;
				}
				if (cosmeticItem2.isNullItem)
				{
					return;
				}
				CosmeticItemInstance cosmeticItemInstance3 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
				if (cosmeticItemInstance3 != null)
				{
					if (!rig.IsItemAllowed(itemNameFromDisplayName2))
					{
						cosmeticItemInstance3.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
						return;
					}
					cosmeticItemInstance3.EnableItem((CosmeticsController.CosmeticSlots)slotIndex);
				}
			}

			// Token: 0x06004CC1 RID: 19649 RVA: 0x0016CCA0 File Offset: 0x0016AEA0
			public void ActivateCosmetics(CosmeticsController.CosmeticSet prevSet, VRRig rig, BodyDockPositions bDock, CosmeticItemRegistry cosmeticsObjectRegistry)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					this.ActivateCosmetic(prevSet, rig, i, cosmeticsObjectRegistry, bDock);
				}
				this.OnSetActivated(prevSet, this, rig.creator);
			}

			// Token: 0x06004CC2 RID: 19650 RVA: 0x0016CCD8 File Offset: 0x0016AED8
			public void DeactivateAllCosmetcs(BodyDockPositions bDock, CosmeticsController.CosmeticItem nullItem, CosmeticItemRegistry cosmeticObjectRegistry)
			{
				bDock.DisableAllTransferableItems();
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticItem cosmeticItem = this.items[i];
					if (!cosmeticItem.isNullItem)
					{
						CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
						CosmeticItemInstance cosmeticItemInstance = cosmeticObjectRegistry.Cosmetic(cosmeticItem.displayName);
						if (cosmeticItemInstance != null)
						{
							cosmeticItemInstance.DisableItem(cosmeticSlots);
						}
						this.items[i] = nullItem;
					}
				}
			}

			// Token: 0x06004CC3 RID: 19651 RVA: 0x0016CD38 File Offset: 0x0016AF38
			public void LoadFromPlayerPreferences(CosmeticsController controller)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
					string @string = PlayerPrefs.GetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(cosmeticSlots), "NOTHING");
					if (@string == "null" || @string == "NOTHING")
					{
						this.items[i] = controller.nullItem;
					}
					else
					{
						CosmeticsController.CosmeticItem item = controller.GetItemFromDict(@string);
						if (item.isNullItem)
						{
							Debug.Log("LoadFromPlayerPreferences: Could not find item stored in player prefs: \"" + @string + "\"");
							this.items[i] = controller.nullItem;
						}
						else if (!CosmeticsController.CompareCategoryToSavedCosmeticSlots(item.itemCategory, cosmeticSlots))
						{
							this.items[i] = controller.nullItem;
						}
						else if (controller.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.itemName == x.itemName) >= 0)
						{
							this.items[i] = item;
						}
						else
						{
							this.items[i] = controller.nullItem;
						}
					}
				}
			}

			// Token: 0x06004CC4 RID: 19652 RVA: 0x0016CE54 File Offset: 0x0016B054
			public string[] ToDisplayNameArray()
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					this.returnArray[i] = (string.IsNullOrEmpty(this.items[i].displayName) ? "null" : this.items[i].displayName);
				}
				return this.returnArray;
			}

			// Token: 0x06004CC5 RID: 19653 RVA: 0x0016CEB0 File Offset: 0x0016B0B0
			public int[] ToPackedIDArray()
			{
				int num = 0;
				int num2 = 0;
				int num3 = 16;
				for (int i = 0; i < num3; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].itemName.Length == 6)
					{
						num |= 1 << i;
						num2++;
					}
				}
				if (num == 0)
				{
					return CosmeticsController.CosmeticSet.intArrays[0];
				}
				int[] array = CosmeticsController.CosmeticSet.intArrays[num2 + 1];
				array[0] = num;
				int num4 = 1;
				for (int j = 0; j < num3; j++)
				{
					if ((num & (1 << j)) != 0)
					{
						string itemName = this.items[j].itemName;
						array[num4] = (int)(itemName[0] - 'A' + '\u001a' * (itemName[1] - 'A' + '\u001a' * (itemName[2] - 'A' + '\u001a' * (itemName[3] - 'A' + '\u001a' * (itemName[4] - 'A')))));
						num4++;
					}
				}
				return array;
			}

			// Token: 0x06004CC6 RID: 19654 RVA: 0x0016CFB0 File Offset: 0x0016B1B0
			public string[] HoldableDisplayNames(bool leftHoldables)
			{
				int num = 16;
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].isHoldable && this.items[i].isHoldable && this.items[i].itemCategory != CosmeticsController.CosmeticCategory.Chest)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
					}
				}
				if (num2 == 0)
				{
					return null;
				}
				int num3 = 0;
				string[] array = new string[num2];
				for (int j = 0; j < num; j++)
				{
					if (this.items[j].isHoldable)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
					}
				}
				return array;
			}

			// Token: 0x06004CC7 RID: 19655 RVA: 0x0016D0C4 File Offset: 0x0016B2C4
			public bool[] ToOnRightSideArray()
			{
				int num = 16;
				bool[] array = new bool[num];
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].isHoldable && this.items[i].itemCategory != CosmeticsController.CosmeticCategory.Chest)
					{
						array[i] = !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i));
					}
					else
					{
						array[i] = false;
					}
				}
				return array;
			}

			// Token: 0x04004FAC RID: 20396
			public CosmeticsController.CosmeticItem[] items;

			// Token: 0x04004FAE RID: 20398
			public string[] returnArray = new string[16];

			// Token: 0x04004FAF RID: 20399
			private static int[][] intArrays = new int[][]
			{
				new int[0],
				new int[1],
				new int[2],
				new int[3],
				new int[4],
				new int[5],
				new int[6],
				new int[7],
				new int[8],
				new int[9],
				new int[10],
				new int[11],
				new int[12],
				new int[13],
				new int[14],
				new int[15],
				new int[16],
				new int[17],
				new int[18],
				new int[19],
				new int[20],
				new int[21]
			};

			// Token: 0x04004FB0 RID: 20400
			private static CosmeticsController.CosmeticSet _emptySet;

			// Token: 0x04004FB1 RID: 20401
			private static char[] nameScratchSpace = new char[6];

			// Token: 0x02000C13 RID: 3091
			// (Invoke) Token: 0x06004CCA RID: 19658
			public delegate void OnSetActivatedHandler(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer);
		}

		// Token: 0x02000C15 RID: 3093
		[Serializable]
		public struct CosmeticItem
		{
			// Token: 0x04004FB3 RID: 20403
			[Tooltip("Should match the spreadsheet item name.")]
			public string itemName;

			// Token: 0x04004FB4 RID: 20404
			[Tooltip("Determines what wardrobe section the item will show up in.")]
			public CosmeticsController.CosmeticCategory itemCategory;

			// Token: 0x04004FB5 RID: 20405
			[Tooltip("If this is a holdable item.")]
			public bool isHoldable;

			// Token: 0x04004FB6 RID: 20406
			[Tooltip("Icon shown in the store menus & hunt watch.")]
			public Sprite itemPicture;

			// Token: 0x04004FB7 RID: 20407
			public string displayName;

			// Token: 0x04004FB8 RID: 20408
			public string itemPictureResourceString;

			// Token: 0x04004FB9 RID: 20409
			[Tooltip("The name shown on the store checkout screen.")]
			public string overrideDisplayName;

			// Token: 0x04004FBA RID: 20410
			[DebugReadout]
			[NonSerialized]
			public int cost;

			// Token: 0x04004FBB RID: 20411
			[DebugReadout]
			[NonSerialized]
			public string[] bundledItems;

			// Token: 0x04004FBC RID: 20412
			[DebugReadout]
			[NonSerialized]
			public bool canTryOn;

			// Token: 0x04004FBD RID: 20413
			[Tooltip("Set to true if the item takes up both left and right wearable hand slots at the same time. Used for things like mittens/gloves.")]
			public bool bothHandsHoldable;

			// Token: 0x04004FBE RID: 20414
			public bool bLoadsFromResources;

			// Token: 0x04004FBF RID: 20415
			public bool bUsesMeshAtlas;

			// Token: 0x04004FC0 RID: 20416
			public Vector3 rotationOffset;

			// Token: 0x04004FC1 RID: 20417
			public Vector3 positionOffset;

			// Token: 0x04004FC2 RID: 20418
			public string meshAtlasResourceString;

			// Token: 0x04004FC3 RID: 20419
			public string meshResourceString;

			// Token: 0x04004FC4 RID: 20420
			public string materialResourceString;

			// Token: 0x04004FC5 RID: 20421
			[HideInInspector]
			public bool isNullItem;
		}

		// Token: 0x02000C16 RID: 3094
		[Serializable]
		public class IAPRequestBody
		{
			// Token: 0x04004FC6 RID: 20422
			public string userID;

			// Token: 0x04004FC7 RID: 20423
			public string nonce;

			// Token: 0x04004FC8 RID: 20424
			public string platform;

			// Token: 0x04004FC9 RID: 20425
			public string sku;

			// Token: 0x04004FCA RID: 20426
			public Dictionary<string, string> customTags;
		}

		// Token: 0x02000C17 RID: 3095
		public enum EWearingCosmeticSet
		{
			// Token: 0x04004FCC RID: 20428
			NotASet,
			// Token: 0x04004FCD RID: 20429
			NotWearing,
			// Token: 0x04004FCE RID: 20430
			Partial,
			// Token: 0x04004FCF RID: 20431
			Complete
		}
	}
}
