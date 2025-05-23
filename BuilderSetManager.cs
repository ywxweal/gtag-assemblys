using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000512 RID: 1298
public class BuilderSetManager : MonoBehaviour
{
	// Token: 0x1700032D RID: 813
	// (get) Token: 0x06001F72 RID: 8050 RVA: 0x0009E144 File Offset: 0x0009C344
	// (set) Token: 0x06001F73 RID: 8051 RVA: 0x0009E14B File Offset: 0x0009C34B
	public static bool hasInstance { get; private set; }

	// Token: 0x06001F74 RID: 8052 RVA: 0x0009E154 File Offset: 0x0009C354
	public string GetStarterSetsConcat()
	{
		if (BuilderSetManager.concatStarterSets.Length > 0)
		{
			return BuilderSetManager.concatStarterSets;
		}
		BuilderSetManager.concatStarterSets = string.Empty;
		foreach (BuilderPieceSet builderPieceSet in this._starterPieceSets)
		{
			BuilderSetManager.concatStarterSets += builderPieceSet.playfabID;
		}
		return BuilderSetManager.concatStarterSets;
	}

	// Token: 0x06001F75 RID: 8053 RVA: 0x0009E1D8 File Offset: 0x0009C3D8
	public string GetAllSetsConcat()
	{
		if (BuilderSetManager.concatAllSets.Length > 0)
		{
			return BuilderSetManager.concatAllSets;
		}
		BuilderSetManager.concatAllSets = string.Empty;
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			BuilderSetManager.concatAllSets += builderPieceSet.playfabID;
		}
		return BuilderSetManager.concatAllSets;
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x0009E25C File Offset: 0x0009C45C
	public void Awake()
	{
		if (BuilderSetManager.instance == null)
		{
			BuilderSetManager.instance = this;
			BuilderSetManager.hasInstance = true;
		}
		else if (BuilderSetManager.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		this.Init();
		if (this.monitor == null)
		{
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x0009E2C4 File Offset: 0x0009C4C4
	private void Init()
	{
		this.catalog = "DLC";
		this.currencyName = "SR";
		this.pulledStoreItems = false;
		BuilderSetManager._setIdToStoreItem = new Dictionary<int, BuilderSetManager.BuilderSetStoreItem>(this._allPieceSets.Count);
		BuilderSetManager._setIdToStoreItem.Clear();
		BuilderSetManager.pieceSetInfos = new List<BuilderSetManager.BuilderPieceSetInfo>(this._allPieceSets.Count * 45);
		BuilderSetManager.pieceSetInfoMap = new Dictionary<int, int>(this._allPieceSets.Count * 45);
		this.livePieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this.scheduledPieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem = new BuilderSetManager.BuilderSetStoreItem
			{
				displayName = builderPieceSet.setName,
				playfabID = builderPieceSet.playfabID,
				setID = builderPieceSet.GetIntIdentifier(),
				cost = 0U,
				setRef = builderPieceSet,
				displayModel = builderPieceSet.displayModel,
				isNullItem = false
			};
			BuilderSetManager._setIdToStoreItem.TryAdd(builderPieceSet.GetIntIdentifier(), builderSetStoreItem);
			int num = -1;
			if (!string.IsNullOrEmpty(builderPieceSet.materialId))
			{
				num = builderPieceSet.materialId.GetHashCode();
			}
			for (int i = 0; i < builderPieceSet.subsets.Count; i++)
			{
				BuilderPieceSet.BuilderPieceSubset builderPieceSubset = builderPieceSet.subsets[i];
				for (int j = 0; j < builderPieceSubset.pieceInfos.Count; j++)
				{
					BuilderPiece piecePrefab = builderPieceSubset.pieceInfos[j].piecePrefab;
					int staticHash = piecePrefab.name.GetStaticHash();
					int num2 = num;
					if (piecePrefab.materialOptions == null)
					{
						num2 = -1;
						this.AddPieceToInfoMap(staticHash, num2, builderPieceSet.GetIntIdentifier());
					}
					else if (builderPieceSubset.pieceInfos[j].overrideSetMaterial)
					{
						if (builderPieceSubset.pieceInfos[j].pieceMaterialTypes.Length == 0)
						{
							Debug.LogErrorFormat("Material List for piece {0} in set {1} is empty", new object[] { piecePrefab.name, builderPieceSet.setName });
						}
						foreach (string text in builderPieceSubset.pieceInfos[j].pieceMaterialTypes)
						{
							if (string.IsNullOrEmpty(text))
							{
								Debug.LogErrorFormat("Material List Entry for piece {0} in set {1} is empty", new object[] { piecePrefab.name, builderPieceSet.setName });
							}
							else
							{
								num2 = text.GetHashCode();
								this.AddPieceToInfoMap(staticHash, num2, builderPieceSet.GetIntIdentifier());
							}
						}
					}
					else
					{
						Material material;
						int num3;
						piecePrefab.materialOptions.GetMaterialFromType(num, out material, out num3);
						if (material == null)
						{
							num2 = -1;
						}
						this.AddPieceToInfoMap(staticHash, num2, builderPieceSet.GetIntIdentifier());
					}
				}
			}
			if (!builderPieceSet.isScheduled)
			{
				this.livePieceSets.Add(builderPieceSet);
			}
			else
			{
				this.scheduledPieceSets.Add(builderPieceSet);
			}
		}
		this._unlockedPieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this._unlockedPieceSets.AddRange(this._starterPieceSets);
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x0009E61C File Offset: 0x0009C81C
	private void OnEnable()
	{
		if (this.monitor == null && this.scheduledPieceSets.Count > 0)
		{
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x0009E646 File Offset: 0x0009C846
	private void OnDisable()
	{
		if (this.monitor != null)
		{
			base.StopCoroutine(this.monitor);
		}
		this.monitor = null;
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x0009E663 File Offset: 0x0009C863
	private IEnumerator MonitorTime()
	{
		while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
		{
			yield return null;
		}
		while (this.scheduledPieceSets.Count > 0)
		{
			bool flag = false;
			for (int i = this.scheduledPieceSets.Count - 1; i >= 0; i--)
			{
				BuilderPieceSet builderPieceSet = this.scheduledPieceSets[i];
				if (GorillaComputer.instance.GetServerTime() > builderPieceSet.GetScheduleDateTime())
				{
					flag = true;
					this.livePieceSets.Add(builderPieceSet);
					this.scheduledPieceSets.RemoveAt(i);
				}
			}
			if (flag)
			{
				this.OnLiveSetsUpdated.Invoke();
			}
			yield return new WaitForSeconds(60f);
		}
		this.monitor = null;
		yield break;
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x0009E674 File Offset: 0x0009C874
	private void AddPieceToInfoMap(int pieceType, int pieceMaterial, int setID)
	{
		int num;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, pieceMaterial), out num))
		{
			BuilderSetManager.BuilderPieceSetInfo builderPieceSetInfo = BuilderSetManager.pieceSetInfos[num];
			if (!builderPieceSetInfo.setIds.Contains(setID))
			{
				builderPieceSetInfo.setIds.Add(setID);
			}
			BuilderSetManager.pieceSetInfos[num] = builderPieceSetInfo;
			return;
		}
		BuilderSetManager.BuilderPieceSetInfo builderPieceSetInfo2 = new BuilderSetManager.BuilderPieceSetInfo
		{
			pieceType = pieceType,
			materialType = pieceMaterial,
			setIds = new List<int> { setID }
		};
		BuilderSetManager.pieceSetInfoMap.Add(HashCode.Combine<int, int>(pieceType, pieceMaterial), BuilderSetManager.pieceSetInfos.Count);
		BuilderSetManager.pieceSetInfos.Add(builderPieceSetInfo2);
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x0009E71C File Offset: 0x0009C91C
	public static bool IsItemIDBuilderItem(string playfabID)
	{
		return BuilderSetManager.instance.GetAllSetsConcat().Contains(playfabID);
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x0009E730 File Offset: 0x0009C930
	public void OnGotInventoryItems(GetUserInventoryResult inventoryResult, GetCatalogItemsResult catalogResult)
	{
		CosmeticsController cosmeticsController = CosmeticsController.instance;
		cosmeticsController.concatStringCosmeticsAllowed += this.GetStarterSetsConcat();
		this._unlockedPieceSets.Clear();
		this._unlockedPieceSets.AddRange(this._starterPieceSets);
		foreach (CatalogItem catalogItem in catalogResult.Catalog)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem;
			if (BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId) && BuilderSetManager._setIdToStoreItem.TryGetValue(catalogItem.ItemId.GetStaticHash(), out builderSetStoreItem))
			{
				bool flag = false;
				uint num = 0U;
				if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out num))
				{
					flag = true;
				}
				builderSetStoreItem.playfabID = catalogItem.ItemId;
				builderSetStoreItem.cost = num;
				builderSetStoreItem.hasPrice = flag;
				BuilderSetManager._setIdToStoreItem[builderSetStoreItem.setRef.GetIntIdentifier()] = builderSetStoreItem;
			}
		}
		foreach (ItemInstance itemInstance in inventoryResult.Inventory)
		{
			if (BuilderSetManager.IsItemIDBuilderItem(itemInstance.ItemId))
			{
				BuilderSetManager.BuilderSetStoreItem builderSetStoreItem2;
				if (BuilderSetManager._setIdToStoreItem.TryGetValue(itemInstance.ItemId.GetStaticHash(), out builderSetStoreItem2))
				{
					Debug.LogFormat("BuilderSetManager: Unlocking Inventory Item {0}", new object[] { itemInstance.ItemId });
					this._unlockedPieceSets.Add(builderSetStoreItem2.setRef);
					CosmeticsController cosmeticsController2 = CosmeticsController.instance;
					cosmeticsController2.concatStringCosmeticsAllowed += itemInstance.ItemId;
				}
				else
				{
					Debug.Log("BuilderSetManager: No store item found with id" + itemInstance.ItemId);
				}
			}
		}
		this.pulledStoreItems = true;
		UnityEvent onOwnedSetsUpdated = this.OnOwnedSetsUpdated;
		if (onOwnedSetsUpdated == null)
		{
			return;
		}
		onOwnedSetsUpdated.Invoke();
	}

	// Token: 0x06001F7E RID: 8062 RVA: 0x0009E914 File Offset: 0x0009CB14
	public BuilderSetManager.BuilderSetStoreItem GetStoreItemFromSetID(int setID)
	{
		return BuilderSetManager._setIdToStoreItem.GetValueOrDefault(setID, BuilderKiosk.nullItem);
	}

	// Token: 0x06001F7F RID: 8063 RVA: 0x0009E928 File Offset: 0x0009CB28
	public BuilderPieceSet GetPieceSetFromID(int setID)
	{
		BuilderSetManager.BuilderSetStoreItem builderSetStoreItem;
		if (BuilderSetManager._setIdToStoreItem.TryGetValue(setID, out builderSetStoreItem))
		{
			return builderSetStoreItem.setRef;
		}
		return null;
	}

	// Token: 0x06001F80 RID: 8064 RVA: 0x0009E94C File Offset: 0x0009CB4C
	public List<BuilderPieceSet> GetAllPieceSets()
	{
		return this._allPieceSets;
	}

	// Token: 0x06001F81 RID: 8065 RVA: 0x0009E954 File Offset: 0x0009CB54
	public List<BuilderPieceSet> GetLivePieceSets()
	{
		return this.livePieceSets;
	}

	// Token: 0x06001F82 RID: 8066 RVA: 0x0009E95C File Offset: 0x0009CB5C
	public List<BuilderPieceSet> GetUnlockedPieceSets()
	{
		return this._unlockedPieceSets;
	}

	// Token: 0x06001F83 RID: 8067 RVA: 0x0009E964 File Offset: 0x0009CB64
	public List<BuilderPieceSet> GetPermanentSetsForSale()
	{
		return this._setsAlwaysForSale;
	}

	// Token: 0x06001F84 RID: 8068 RVA: 0x0009E96C File Offset: 0x0009CB6C
	public List<BuilderPieceSet> GetSeasonalSetsForSale()
	{
		return this._seasonalSetsForSale;
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x0009E974 File Offset: 0x0009CB74
	public bool IsSetSeasonal(string playfabID)
	{
		return !this._seasonalSetsForSale.IsNullOrEmpty<BuilderPieceSet>() && this._seasonalSetsForSale.FindIndex((BuilderPieceSet x) => x.playfabID.Equals(playfabID)) >= 0;
	}

	// Token: 0x06001F86 RID: 8070 RVA: 0x0009E9BC File Offset: 0x0009CBBC
	public bool DoesPlayerOwnPieceSet(Player player, int setID)
	{
		BuilderPieceSet pieceSetFromID = this.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			bool flag = rigContainer.Rig.IsItemAllowed(pieceSetFromID.playfabID);
			Debug.LogFormat("BuilderSetManager: does player {0} own set {1} {2}", new object[] { player.ActorNumber, pieceSetFromID.setName, flag });
			return flag;
		}
		Debug.LogFormat("BuilderSetManager: could not get rig for player {0}", new object[] { player.ActorNumber });
		return false;
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x0009EA50 File Offset: 0x0009CC50
	public bool DoesAnyPlayerInRoomOwnPieceSet(int setID)
	{
		BuilderPieceSet pieceSetFromID = this.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return false;
		}
		if (this.GetStarterSetsConcat().Contains(pieceSetFromID.setName))
		{
			return true;
		}
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer) && rigContainer.Rig.IsItemAllowed(pieceSetFromID.playfabID))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x0009EAF0 File Offset: 0x0009CCF0
	public bool IsPieceOwnedByRoom(int pieceType, int materialType)
	{
		int num;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, materialType), out num))
		{
			foreach (int num2 in BuilderSetManager.pieceSetInfos[num].setIds)
			{
				if (this.DoesAnyPlayerInRoomOwnPieceSet(num2))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x0009EB70 File Offset: 0x0009CD70
	public bool IsPieceOwnedLocally(int pieceType, int materialType)
	{
		int num;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, materialType), out num))
		{
			foreach (int num2 in BuilderSetManager.pieceSetInfos[num].setIds)
			{
				if (this.IsPieceSetOwnedLocally(num2))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x0009EBF0 File Offset: 0x0009CDF0
	public bool IsPieceSetOwnedLocally(int setID)
	{
		return this._unlockedPieceSets.FindIndex((BuilderPieceSet x) => setID == x.GetIntIdentifier()) >= 0;
	}

	// Token: 0x06001F8B RID: 8075 RVA: 0x0009EC28 File Offset: 0x0009CE28
	public void UnlockSet(int setID)
	{
		int num = this._allPieceSets.FindIndex((BuilderPieceSet x) => setID == x.GetIntIdentifier());
		if (num >= 0 && !this._unlockedPieceSets.Contains(this._allPieceSets[num]))
		{
			Debug.Log("BuilderSetManager: unlocking set " + this._allPieceSets[num].setName);
			this._unlockedPieceSets.Add(this._allPieceSets[num]);
		}
		UnityEvent onOwnedSetsUpdated = this.OnOwnedSetsUpdated;
		if (onOwnedSetsUpdated == null)
		{
			return;
		}
		onOwnedSetsUpdated.Invoke();
	}

	// Token: 0x06001F8C RID: 8076 RVA: 0x0009ECC0 File Offset: 0x0009CEC0
	public void TryPurchaseItem(int setID, Action<bool> resultCallback)
	{
		BuilderSetManager.BuilderSetStoreItem storeItem;
		if (!BuilderSetManager._setIdToStoreItem.TryGetValue(setID, out storeItem))
		{
			Debug.Log("BuilderSetManager: no store Item for set " + setID.ToString());
			Action<bool> resultCallback2 = resultCallback;
			if (resultCallback2 == null)
			{
				return;
			}
			resultCallback2(false);
			return;
		}
		else
		{
			if (!this.IsPieceSetOwnedLocally(setID))
			{
				PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
				{
					ItemId = storeItem.playfabID,
					Price = (int)storeItem.cost,
					VirtualCurrency = this.currencyName,
					CatalogVersion = this.catalog
				}, delegate(PurchaseItemResult result)
				{
					if (result.Items.Count > 0)
					{
						foreach (ItemInstance itemInstance in result.Items)
						{
							Debug.Log("BuilderSetManager: unlocking set " + itemInstance.ItemId);
							this.UnlockSet(itemInstance.ItemId.GetStaticHash());
						}
						CosmeticsController.instance.UpdateMyCosmetics();
						if (PhotonNetwork.InRoom)
						{
							this.StartCoroutine(this.CheckIfMyCosmeticsUpdated(storeItem.playfabID));
						}
						Action<bool> resultCallback4 = resultCallback;
						if (resultCallback4 == null)
						{
							return;
						}
						resultCallback4(true);
						return;
					}
					else
					{
						Debug.Log("BuilderSetManager: no items purchased ");
						Action<bool> resultCallback5 = resultCallback;
						if (resultCallback5 == null)
						{
							return;
						}
						resultCallback5(false);
						return;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogErrorFormat("BuilderSetManager: purchase {0} Error {1}", new object[] { setID, error.ErrorMessage });
					Action<bool> resultCallback6 = resultCallback;
					if (resultCallback6 == null)
					{
						return;
					}
					resultCallback6(false);
				}, null, null);
				return;
			}
			Debug.Log("BuilderSetManager: set already owned " + setID.ToString());
			Action<bool> resultCallback3 = resultCallback;
			if (resultCallback3 == null)
			{
				return;
			}
			resultCallback3(false);
			return;
		}
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x0009EDC4 File Offset: 0x0009CFC4
	private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
	{
		yield return new WaitForSeconds(1f);
		this.foundCosmetic = false;
		this.attempts = 0;
		while (!this.foundCosmetic && this.attempts < 10 && PhotonNetwork.InRoom)
		{
			this.playerIDList.Clear();
			if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPath())
			{
				this.playerIDList.Add("Inventory");
				PlayFabClientAPI.GetSharedGroupData(new global::PlayFab.ClientModels.GetSharedGroupDataRequest
				{
					Keys = this.playerIDList,
					SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
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
					bool flag = this.foundCosmetic;
				}, delegate(PlayFabError error)
				{
					this.attempts++;
					CosmeticsController.instance.ReauthOrBan(error);
				}, null, null);
				yield return new WaitForSeconds(1f);
			}
			else
			{
				this.playerIDList.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
				PlayFabClientAPI.GetSharedGroupData(new global::PlayFab.ClientModels.GetSharedGroupDataRequest
				{
					Keys = this.playerIDList,
					SharedGroupId = PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper()
				}, delegate(GetSharedGroupDataResult result)
				{
					this.attempts++;
					foreach (KeyValuePair<string, global::PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair2 in result.Data)
					{
						if (keyValuePair2.Value.Value.Contains(itemToBuyID))
						{
							Debug.Log("BuilderSetManager: found it! updating others cosmetic!");
							PhotonNetwork.RaiseEvent(199, null, new RaiseEventOptions
							{
								Receivers = ReceiverGroup.Others
							}, SendOptions.SendReliable);
							this.foundCosmetic = true;
						}
						else
						{
							Debug.Log("BuilderSetManager: didnt find it, updating attempts and trying again in a bit. current attempt is " + this.attempts.ToString());
						}
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
					Debug.Log("BuilderSetManager: Got error retrieving user data, on attempt " + this.attempts.ToString());
					Debug.Log(error.GenerateErrorReport());
				}, null, null);
				yield return new WaitForSeconds(1f);
			}
		}
		Debug.Log("BuilderSetManager: done!");
		yield break;
	}

	// Token: 0x04002351 RID: 9041
	[SerializeField]
	private List<BuilderPieceSet> _allPieceSets;

	// Token: 0x04002352 RID: 9042
	[SerializeField]
	private List<BuilderPieceSet> _starterPieceSets;

	// Token: 0x04002353 RID: 9043
	[SerializeField]
	private List<BuilderPieceSet> _setsAlwaysForSale;

	// Token: 0x04002354 RID: 9044
	[SerializeField]
	private List<BuilderPieceSet> _seasonalSetsForSale;

	// Token: 0x04002355 RID: 9045
	private List<BuilderPieceSet> livePieceSets;

	// Token: 0x04002356 RID: 9046
	private List<BuilderPieceSet> scheduledPieceSets;

	// Token: 0x04002357 RID: 9047
	private Coroutine monitor;

	// Token: 0x04002358 RID: 9048
	private List<BuilderSetManager.BuilderSetStoreItem> _allStoreItems;

	// Token: 0x04002359 RID: 9049
	private List<BuilderPieceSet> _unlockedPieceSets;

	// Token: 0x0400235A RID: 9050
	private static Dictionary<int, BuilderSetManager.BuilderSetStoreItem> _setIdToStoreItem;

	// Token: 0x0400235B RID: 9051
	private static List<BuilderSetManager.BuilderPieceSetInfo> pieceSetInfos;

	// Token: 0x0400235C RID: 9052
	private static Dictionary<int, int> pieceSetInfoMap;

	// Token: 0x0400235D RID: 9053
	[OnEnterPlay_SetNull]
	public static volatile BuilderSetManager instance;

	// Token: 0x0400235F RID: 9055
	[HideInInspector]
	public string catalog;

	// Token: 0x04002360 RID: 9056
	[HideInInspector]
	public string currencyName;

	// Token: 0x04002361 RID: 9057
	private string[] tempStringArray;

	// Token: 0x04002362 RID: 9058
	[HideInInspector]
	public UnityEvent OnLiveSetsUpdated;

	// Token: 0x04002363 RID: 9059
	[HideInInspector]
	public UnityEvent OnOwnedSetsUpdated;

	// Token: 0x04002364 RID: 9060
	[HideInInspector]
	public bool pulledStoreItems;

	// Token: 0x04002365 RID: 9061
	private static string concatStarterSets = string.Empty;

	// Token: 0x04002366 RID: 9062
	private static string concatAllSets = string.Empty;

	// Token: 0x04002367 RID: 9063
	private bool foundCosmetic;

	// Token: 0x04002368 RID: 9064
	private int attempts;

	// Token: 0x04002369 RID: 9065
	private List<string> playerIDList = new List<string>();

	// Token: 0x02000513 RID: 1299
	[Serializable]
	public struct BuilderSetStoreItem
	{
		// Token: 0x0400236A RID: 9066
		public string displayName;

		// Token: 0x0400236B RID: 9067
		public string playfabID;

		// Token: 0x0400236C RID: 9068
		public int setID;

		// Token: 0x0400236D RID: 9069
		public uint cost;

		// Token: 0x0400236E RID: 9070
		public bool hasPrice;

		// Token: 0x0400236F RID: 9071
		public BuilderPieceSet setRef;

		// Token: 0x04002370 RID: 9072
		public GameObject displayModel;

		// Token: 0x04002371 RID: 9073
		[NonSerialized]
		public bool isNullItem;
	}

	// Token: 0x02000514 RID: 1300
	[Serializable]
	public struct BuilderPieceSetInfo
	{
		// Token: 0x04002372 RID: 9074
		public int pieceType;

		// Token: 0x04002373 RID: 9075
		public int materialType;

		// Token: 0x04002374 RID: 9076
		public List<int> setIds;
	}
}
