using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FXP;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C96 RID: 3222
	public class StoreUpdater : MonoBehaviour
	{
		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x06004FDA RID: 20442 RVA: 0x0017C62F File Offset: 0x0017A82F
		public DateTime DateTimeNowServerAdjusted
		{
			get
			{
				return GorillaComputer.instance.GetServerTime();
			}
		}

		// Token: 0x06004FDB RID: 20443 RVA: 0x0017C63D File Offset: 0x0017A83D
		public void Awake()
		{
			if (StoreUpdater.instance == null)
			{
				StoreUpdater.instance = this;
				return;
			}
			if (StoreUpdater.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06004FDC RID: 20444 RVA: 0x0017C671 File Offset: 0x0017A871
		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				this.HandleHMDMounted();
				return;
			}
			this.HandleHMDUnmounted();
		}

		// Token: 0x06004FDD RID: 20445 RVA: 0x0017C684 File Offset: 0x0017A884
		public void Initialize()
		{
			this.FindAllCosmeticItemPrefabs();
			OVRManager.HMDMounted += this.HandleHMDMounted;
			OVRManager.HMDUnmounted += this.HandleHMDUnmounted;
			OVRManager.HMDLost += this.HandleHMDUnmounted;
			OVRManager.HMDAcquired += this.HandleHMDMounted;
			Debug.Log("StoreUpdater - Starting");
			if (this.bLoadFromJSON)
			{
				base.StartCoroutine(this.InitializeTitleData());
			}
		}

		// Token: 0x06004FDE RID: 20446 RVA: 0x0017C6FA File Offset: 0x0017A8FA
		private void ServerTimeUpdater()
		{
			base.StartCoroutine(this.InitializeTitleData());
		}

		// Token: 0x06004FDF RID: 20447 RVA: 0x0017C70C File Offset: 0x0017A90C
		public void OnDestroy()
		{
			OVRManager.HMDMounted -= this.HandleHMDMounted;
			OVRManager.HMDUnmounted -= this.HandleHMDUnmounted;
			OVRManager.HMDLost -= this.HandleHMDUnmounted;
			OVRManager.HMDAcquired -= this.HandleHMDMounted;
		}

		// Token: 0x06004FE0 RID: 20448 RVA: 0x0017C760 File Offset: 0x0017A960
		private void HandleHMDUnmounted()
		{
			foreach (string text in this.pedestalUpdateCoroutines.Keys)
			{
				if (this.pedestalUpdateCoroutines[text] != null)
				{
					base.StopCoroutine(this.pedestalUpdateCoroutines[text]);
				}
			}
			foreach (string text2 in this.cosmeticItemPrefabsDictionary.Keys)
			{
				if (this.cosmeticItemPrefabsDictionary[text2] != null)
				{
					this.cosmeticItemPrefabsDictionary[text2].StopCountdownCoroutine();
				}
			}
		}

		// Token: 0x06004FE1 RID: 20449 RVA: 0x0017C838 File Offset: 0x0017AA38
		private void HandleHMDMounted()
		{
			foreach (string text in this.cosmeticItemPrefabsDictionary.Keys)
			{
				if (this.cosmeticItemPrefabsDictionary[text] != null && this.pedestalUpdateEvents.ContainsKey(text) && this.cosmeticItemPrefabsDictionary[text].gameObject.activeInHierarchy)
				{
					this.CheckEventsOnResume(this.pedestalUpdateEvents[text]);
					this.StartNextEvent(text, false);
				}
			}
		}

		// Token: 0x06004FE2 RID: 20450 RVA: 0x0017C8E0 File Offset: 0x0017AAE0
		private void FindAllCosmeticItemPrefabs()
		{
			foreach (CosmeticItemPrefab cosmeticItemPrefab in Object.FindObjectsOfType<CosmeticItemPrefab>())
			{
				if (this.cosmeticItemPrefabsDictionary.ContainsKey(cosmeticItemPrefab.PedestalID))
				{
					Debug.LogWarning("StoreUpdater - Duplicate Pedestal ID " + cosmeticItemPrefab.PedestalID);
				}
				else
				{
					Debug.Log("StoreUpdater - Adding Pedestal " + cosmeticItemPrefab.PedestalID);
					this.cosmeticItemPrefabsDictionary.Add(cosmeticItemPrefab.PedestalID, cosmeticItemPrefab);
				}
			}
		}

		// Token: 0x06004FE3 RID: 20451 RVA: 0x0017C956 File Offset: 0x0017AB56
		private IEnumerator HandlePedestalUpdate(StoreUpdateEvent updateEvent, bool playFX)
		{
			this.cosmeticItemPrefabsDictionary[updateEvent.PedestalID].SetStoreUpdateEvent(updateEvent, playFX);
			yield return new WaitForSeconds((float)(updateEvent.EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds);
			if (this.pedestalClearCartCoroutines.ContainsKey(updateEvent.PedestalID))
			{
				if (this.pedestalClearCartCoroutines[updateEvent.PedestalID] != null)
				{
					base.StopCoroutine(this.pedestalClearCartCoroutines[updateEvent.PedestalID]);
				}
				this.pedestalClearCartCoroutines[updateEvent.PedestalID] = base.StartCoroutine(this.HandleClearCart(updateEvent));
			}
			else
			{
				this.pedestalClearCartCoroutines.Add(updateEvent.PedestalID, base.StartCoroutine(this.HandleClearCart(updateEvent)));
			}
			if (this.cosmeticItemPrefabsDictionary[updateEvent.PedestalID].gameObject.activeInHierarchy)
			{
				this.pedestalUpdateEvents[updateEvent.PedestalID].RemoveAt(0);
				this.StartNextEvent(updateEvent.PedestalID, true);
			}
			yield break;
		}

		// Token: 0x06004FE4 RID: 20452 RVA: 0x0017C973 File Offset: 0x0017AB73
		private IEnumerator HandleClearCart(StoreUpdateEvent updateEvent)
		{
			float num = Math.Clamp((float)(updateEvent.EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds + 60f, 0f, 60f);
			yield return new WaitForSeconds(num);
			if (CosmeticsController.instance.RemoveItemFromCart(CosmeticsController.instance.GetItemFromDict(updateEvent.ItemName)))
			{
				CosmeticsController.instance.ClearCheckout(true);
				CosmeticsController.instance.UpdateShoppingCart();
				CosmeticsController.instance.UpdateWornCosmetics(true);
			}
			yield break;
		}

		// Token: 0x06004FE5 RID: 20453 RVA: 0x0017C98C File Offset: 0x0017AB8C
		private void StartNextEvent(string pedestalID, bool playFX)
		{
			if (this.pedestalUpdateEvents[pedestalID].Count > 0)
			{
				Coroutine coroutine = base.StartCoroutine(this.HandlePedestalUpdate(this.pedestalUpdateEvents[pedestalID].First<StoreUpdateEvent>(), playFX));
				if (this.pedestalUpdateCoroutines.ContainsKey(pedestalID))
				{
					if (this.pedestalUpdateCoroutines[pedestalID] != null && this.pedestalUpdateCoroutines[pedestalID] != null)
					{
						base.StopCoroutine(this.pedestalUpdateCoroutines[pedestalID]);
					}
					this.pedestalUpdateCoroutines[pedestalID] = coroutine;
				}
				else
				{
					this.pedestalUpdateCoroutines.Add(pedestalID, coroutine);
				}
				if (this.pedestalUpdateEvents[pedestalID].Count == 0 && !this.bLoadFromJSON)
				{
					this.GetStoreUpdateEventsPlaceHolder(pedestalID);
					return;
				}
			}
			else if (!this.bLoadFromJSON)
			{
				this.GetStoreUpdateEventsPlaceHolder(pedestalID);
				this.StartNextEvent(pedestalID, true);
			}
		}

		// Token: 0x06004FE6 RID: 20454 RVA: 0x0017CA64 File Offset: 0x0017AC64
		private void GetStoreUpdateEventsPlaceHolder(string PedestalID)
		{
			List<StoreUpdateEvent> list = new List<StoreUpdateEvent>();
			list = this.CreateTempEvents(PedestalID, 1, 15);
			this.CheckEvents(list);
			if (this.pedestalUpdateEvents.ContainsKey(PedestalID))
			{
				this.pedestalUpdateEvents[PedestalID].AddRange(list);
				return;
			}
			this.pedestalUpdateEvents.Add(PedestalID, list);
		}

		// Token: 0x06004FE7 RID: 20455 RVA: 0x0017CAB8 File Offset: 0x0017ACB8
		private void CheckEvents(List<StoreUpdateEvent> updateEvents)
		{
			for (int i = 0; i < updateEvents.Count; i++)
			{
				if (updateEvents[i].EndTimeUTC.ToUniversalTime() < this.DateTimeNowServerAdjusted)
				{
					updateEvents.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06004FE8 RID: 20456 RVA: 0x0017CB00 File Offset: 0x0017AD00
		private void CheckEventsOnResume(List<StoreUpdateEvent> updateEvents)
		{
			bool flag = false;
			for (int i = 0; i < updateEvents.Count; i++)
			{
				if (updateEvents[i].EndTimeUTC.ToUniversalTime() < this.DateTimeNowServerAdjusted)
				{
					if (Math.Clamp((float)(updateEvents[i].EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds + 60f, 0f, 60f) <= 0f)
					{
						flag ^= CosmeticsController.instance.RemoveItemFromCart(CosmeticsController.instance.GetItemFromDict(updateEvents[i].ItemName));
					}
					else if (this.pedestalClearCartCoroutines.ContainsKey(updateEvents[i].PedestalID))
					{
						if (this.pedestalClearCartCoroutines[updateEvents[i].PedestalID] != null)
						{
							base.StopCoroutine(this.pedestalClearCartCoroutines[updateEvents[i].PedestalID]);
						}
						this.pedestalClearCartCoroutines[updateEvents[i].PedestalID] = base.StartCoroutine(this.HandleClearCart(updateEvents[i]));
					}
					else
					{
						this.pedestalClearCartCoroutines.Add(updateEvents[i].PedestalID, base.StartCoroutine(this.HandleClearCart(updateEvents[i])));
					}
					updateEvents.RemoveAt(i);
					i--;
				}
			}
			if (flag)
			{
				CosmeticsController.instance.ClearCheckout(true);
				CosmeticsController.instance.UpdateShoppingCart();
				CosmeticsController.instance.UpdateWornCosmetics(true);
			}
		}

		// Token: 0x06004FE9 RID: 20457 RVA: 0x0017CC8D File Offset: 0x0017AE8D
		private IEnumerator InitializeTitleData()
		{
			yield return new WaitForSeconds(1f);
			PlayFabTitleDataCache.Instance.UpdateData();
			yield return new WaitForSeconds(1f);
			this.GetEventsFromTitleData();
			yield break;
		}

		// Token: 0x06004FEA RID: 20458 RVA: 0x0017CC9C File Offset: 0x0017AE9C
		private void GetEventsFromTitleData()
		{
			Debug.Log("StoreUpdater - GetEventsFromTitleData");
			if (this.bUsePlaceHolderJSON)
			{
				DateTime dateTime = new DateTime(2024, 2, 13, 16, 0, 0, DateTimeKind.Utc);
				List<StoreUpdateEvent> list = StoreUpdateEvent.DeserializeFromJSonList(StoreUpdateEvent.SerializeArrayAsJSon(this.CreateTempEvents("Pedestal1", 2, 120, dateTime).ToArray()));
				this.HandleRecievingEventsFromTitleData(list);
				return;
			}
			PlayFabTitleDataCache.Instance.GetTitleData("TOTD", delegate(string result)
			{
				Debug.Log("StoreUpdater - Recieved TitleData : " + result);
				List<StoreUpdateEvent> list2 = StoreUpdateEvent.DeserializeFromJSonList(result);
				this.HandleRecievingEventsFromTitleData(list2);
			}, delegate(PlayFabError error)
			{
				Debug.Log("StoreUpdater - Error Title Data : " + error.ErrorMessage);
			});
		}

		// Token: 0x06004FEB RID: 20459 RVA: 0x0017CD30 File Offset: 0x0017AF30
		private void HandleRecievingEventsFromTitleData(List<StoreUpdateEvent> updateEvents)
		{
			Debug.Log("StoreUpdater - HandleRecievingEventsFromTitleData");
			this.CheckEvents(updateEvents);
			if (CosmeticsController.instance.GetItemFromDict("LBAEY.").isNullItem)
			{
				Debug.LogWarning("StoreUpdater - CosmeticsController is not initialized.  Reinitializing TitleData");
				base.StartCoroutine(this.InitializeTitleData());
				return;
			}
			foreach (StoreUpdateEvent storeUpdateEvent in updateEvents)
			{
				if (this.pedestalUpdateEvents.ContainsKey(storeUpdateEvent.PedestalID))
				{
					this.pedestalUpdateEvents[storeUpdateEvent.PedestalID].Add(storeUpdateEvent);
				}
				else
				{
					this.pedestalUpdateEvents.Add(storeUpdateEvent.PedestalID, new List<StoreUpdateEvent>());
					this.pedestalUpdateEvents[storeUpdateEvent.PedestalID].Add(storeUpdateEvent);
				}
			}
			Debug.Log("StoreUpdater - Starting Events");
			foreach (string text in this.pedestalUpdateEvents.Keys)
			{
				if (this.cosmeticItemPrefabsDictionary.ContainsKey(text))
				{
					Debug.Log("StoreUpdater - Starting Event " + text);
					this.StartNextEvent(text, false);
				}
			}
			foreach (string text2 in this.cosmeticItemPrefabsDictionary.Keys)
			{
				if (!this.pedestalUpdateEvents.ContainsKey(text2))
				{
					Debug.Log("StoreUpdater - Adding PlaceHolder Events " + text2);
					this.GetStoreUpdateEventsPlaceHolder(text2);
					this.StartNextEvent(text2, false);
				}
			}
		}

		// Token: 0x06004FEC RID: 20460 RVA: 0x0017CEF4 File Offset: 0x0017B0F4
		private void PrintJSONEvents()
		{
			string text = StoreUpdateEvent.SerializeArrayAsJSon(this.CreateTempEvents("Pedestal1", 5, 28).ToArray());
			foreach (StoreUpdateEvent storeUpdateEvent in StoreUpdateEvent.DeserializeFromJSonList(text))
			{
				Debug.Log(string.Concat(new string[]
				{
					"Event : ",
					storeUpdateEvent.ItemName,
					" : ",
					storeUpdateEvent.StartTimeUTC.ToString(),
					" : ",
					storeUpdateEvent.EndTimeUTC.ToString()
				}));
			}
			Debug.Log("NewEvents :\n" + text);
			this.tempJson = text;
		}

		// Token: 0x06004FED RID: 20461 RVA: 0x0017CFC0 File Offset: 0x0017B1C0
		private List<StoreUpdateEvent> CreateTempEvents(string PedestalID, int minuteDelay, int totalEvents)
		{
			string[] array = new string[]
			{
				"LBAEY.", "LBAEZ.", "LBAFA.", "LBAFB.", "LBAFC.", "LBAFD.", "LBAFE.", "LBAFF.", "LBAFG.", "LBAFH.",
				"LBAFO.", "LBAFP.", "LBAFQ.", "LBAFR."
			};
			List<StoreUpdateEvent> list = new List<StoreUpdateEvent>();
			for (int i = 0; i < totalEvents; i++)
			{
				StoreUpdateEvent storeUpdateEvent = new StoreUpdateEvent(PedestalID, array[i % 14], DateTime.UtcNow + TimeSpan.FromMinutes((double)(minuteDelay * i)), DateTime.UtcNow + TimeSpan.FromMinutes((double)(minuteDelay * (i + 1))));
				list.Add(storeUpdateEvent);
			}
			return list;
		}

		// Token: 0x06004FEE RID: 20462 RVA: 0x0017D09C File Offset: 0x0017B29C
		private List<StoreUpdateEvent> CreateTempEvents(string PedestalID, int minuteDelay, int totalEvents, DateTime startTime)
		{
			string[] array = new string[]
			{
				"LBAEY.", "LBAEZ.", "LBAFA.", "LBAFB.", "LBAFC.", "LBAFD.", "LBAFE.", "LBAFF.", "LBAFG.", "LBAFH.",
				"LBAFO.", "LBAFP.", "LBAFQ.", "LBAFR."
			};
			List<StoreUpdateEvent> list = new List<StoreUpdateEvent>();
			for (int i = 0; i < totalEvents; i++)
			{
				StoreUpdateEvent storeUpdateEvent = new StoreUpdateEvent(PedestalID, array[i % 14], startTime + TimeSpan.FromMinutes((double)(minuteDelay * i)), startTime + TimeSpan.FromMinutes((double)(minuteDelay * (i + 1))));
				list.Add(storeUpdateEvent);
			}
			return list;
		}

		// Token: 0x06004FEF RID: 20463 RVA: 0x0017D16F File Offset: 0x0017B36F
		public void PedestalAsleep(CosmeticItemPrefab pedestal)
		{
			if (this.pedestalUpdateCoroutines.ContainsKey(pedestal.PedestalID) && this.pedestalUpdateCoroutines[pedestal.PedestalID] != null)
			{
				base.StopCoroutine(this.pedestalUpdateCoroutines[pedestal.PedestalID]);
			}
		}

		// Token: 0x06004FF0 RID: 20464 RVA: 0x0017D1B0 File Offset: 0x0017B3B0
		public void PedestalAwakened(CosmeticItemPrefab pedestal)
		{
			if (!this.cosmeticItemPrefabsDictionary.ContainsKey(pedestal.PedestalID))
			{
				this.cosmeticItemPrefabsDictionary.Add(pedestal.PedestalID, pedestal);
			}
			if (this.pedestalUpdateEvents.ContainsKey(pedestal.PedestalID))
			{
				this.CheckEventsOnResume(this.pedestalUpdateEvents[pedestal.PedestalID]);
				this.StartNextEvent(pedestal.PedestalID, false);
			}
		}

		// Token: 0x040052EB RID: 21227
		public static volatile StoreUpdater instance;

		// Token: 0x040052EC RID: 21228
		private DateTime StoreItemsChangeTimeUTC;

		// Token: 0x040052ED RID: 21229
		private Dictionary<string, CosmeticItemPrefab> cosmeticItemPrefabsDictionary = new Dictionary<string, CosmeticItemPrefab>();

		// Token: 0x040052EE RID: 21230
		private Dictionary<string, List<StoreUpdateEvent>> pedestalUpdateEvents = new Dictionary<string, List<StoreUpdateEvent>>();

		// Token: 0x040052EF RID: 21231
		private Dictionary<string, Coroutine> pedestalUpdateCoroutines = new Dictionary<string, Coroutine>();

		// Token: 0x040052F0 RID: 21232
		private Dictionary<string, Coroutine> pedestalClearCartCoroutines = new Dictionary<string, Coroutine>();

		// Token: 0x040052F1 RID: 21233
		private string tempJson;

		// Token: 0x040052F2 RID: 21234
		private bool bLoadFromJSON = true;

		// Token: 0x040052F3 RID: 21235
		private bool bUsePlaceHolderJSON;
	}
}
