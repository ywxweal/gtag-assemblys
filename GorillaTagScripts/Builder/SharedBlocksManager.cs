using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Networking;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B73 RID: 2931
	public class SharedBlocksManager : MonoBehaviour
	{
		// Token: 0x14000081 RID: 129
		// (add) Token: 0x0600488F RID: 18575 RVA: 0x0015ACA0 File Offset: 0x00158EA0
		// (remove) Token: 0x06004890 RID: 18576 RVA: 0x0015ACD8 File Offset: 0x00158ED8
		public event Action<string> OnGetTableConfiguration;

		// Token: 0x14000082 RID: 130
		// (add) Token: 0x06004891 RID: 18577 RVA: 0x0015AD10 File Offset: 0x00158F10
		// (remove) Token: 0x06004892 RID: 18578 RVA: 0x0015AD48 File Offset: 0x00158F48
		public event Action<string> OnGetTitleDataBuildComplete;

		// Token: 0x14000083 RID: 131
		// (add) Token: 0x06004893 RID: 18579 RVA: 0x0015AD80 File Offset: 0x00158F80
		// (remove) Token: 0x06004894 RID: 18580 RVA: 0x0015ADB8 File Offset: 0x00158FB8
		public event Action<int> OnSavePrivateScanSuccess;

		// Token: 0x14000084 RID: 132
		// (add) Token: 0x06004895 RID: 18581 RVA: 0x0015ADF0 File Offset: 0x00158FF0
		// (remove) Token: 0x06004896 RID: 18582 RVA: 0x0015AE28 File Offset: 0x00159028
		public event Action<int, string> OnSavePrivateScanFailed;

		// Token: 0x14000085 RID: 133
		// (add) Token: 0x06004897 RID: 18583 RVA: 0x0015AE60 File Offset: 0x00159060
		// (remove) Token: 0x06004898 RID: 18584 RVA: 0x0015AE98 File Offset: 0x00159098
		public event Action<int, bool> OnFetchPrivateScanComplete;

		// Token: 0x14000086 RID: 134
		// (add) Token: 0x06004899 RID: 18585 RVA: 0x0015AED0 File Offset: 0x001590D0
		// (remove) Token: 0x0600489A RID: 18586 RVA: 0x0015AF08 File Offset: 0x00159108
		public event Action<bool, SharedBlocksManager.SharedBlocksMap> OnFoundDefaultSharedBlocksMap;

		// Token: 0x14000087 RID: 135
		// (add) Token: 0x0600489B RID: 18587 RVA: 0x0015AF40 File Offset: 0x00159140
		// (remove) Token: 0x0600489C RID: 18588 RVA: 0x0015AF78 File Offset: 0x00159178
		public event Action<bool> OnGetPopularMapsComplete;

		// Token: 0x14000088 RID: 136
		// (add) Token: 0x0600489D RID: 18589 RVA: 0x0015AFB0 File Offset: 0x001591B0
		// (remove) Token: 0x0600489E RID: 18590 RVA: 0x0015AFE4 File Offset: 0x001591E4
		public static event Action OnRecentMapIdsUpdated;

		// Token: 0x14000089 RID: 137
		// (add) Token: 0x0600489F RID: 18591 RVA: 0x0015B018 File Offset: 0x00159218
		// (remove) Token: 0x060048A0 RID: 18592 RVA: 0x0015B04C File Offset: 0x0015924C
		public static event Action OnSaveTimeUpdated;

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x060048A1 RID: 18593 RVA: 0x0015B07F File Offset: 0x0015927F
		public List<SharedBlocksManager.SharedBlocksMap> LatestPopularMaps
		{
			get
			{
				return this.latestPopularMaps;
			}
		}

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x060048A2 RID: 18594 RVA: 0x0015B087 File Offset: 0x00159287
		public string[] BuildData
		{
			get
			{
				return this.privateScanDataCache;
			}
		}

		// Token: 0x060048A3 RID: 18595 RVA: 0x0015B08F File Offset: 0x0015928F
		public bool IsWaitingOnRequest()
		{
			return this.saveScanInProgress || this.getScanInProgress;
		}

		// Token: 0x060048A4 RID: 18596 RVA: 0x0015B0A4 File Offset: 0x001592A4
		private void Awake()
		{
			if (SharedBlocksManager.instance == null)
			{
				SharedBlocksManager.instance = this;
				for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
				{
					this.privateScanDataCache[i] = string.Empty;
					this.hasPulledPrivateScanMothership[i] = false;
				}
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x060048A5 RID: 18597 RVA: 0x0015B0F4 File Offset: 0x001592F4
		public async void Start()
		{
			SharedBlocksManager.saveDateKeys.Clear();
			for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
			{
				SharedBlocksManager.saveDateKeys.Add(this.GetPlayfabSlotTimeKey(i));
			}
			await this.WaitForPlayfabSessionToken();
			this.FetchConfigurationFromTitleData();
			this.LoadPlayerPrefs();
			this.FetchSharedBlocksStartingMapConfig();
		}

		// Token: 0x060048A6 RID: 18598 RVA: 0x0015B12C File Offset: 0x0015932C
		private bool TryGetCachedSharedBlocksMapByMapID(string mapID, out SharedBlocksManager.SharedBlocksMap result)
		{
			foreach (SharedBlocksManager.SharedBlocksMap sharedBlocksMap in this.mapResponseCache)
			{
				if (sharedBlocksMap.MapID.Equals(mapID))
				{
					result = sharedBlocksMap;
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x060048A7 RID: 18599 RVA: 0x0015B194 File Offset: 0x00159394
		private void AddMapToResponseCache(SharedBlocksManager.SharedBlocksMap map)
		{
			if (map == null)
			{
				return;
			}
			try
			{
				int num = this.mapResponseCache.FindIndex((SharedBlocksManager.SharedBlocksMap x) => x.MapID.Equals(map.MapID));
				if (num < 0)
				{
					this.mapResponseCache.Add(map);
				}
				else
				{
					this.mapResponseCache[num] = map;
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("SharedBlocksManager AddMapToResponseCache Exception " + ex.ToString(), null);
			}
			if (this.mapResponseCache.Count >= 5)
			{
				this.mapResponseCache.RemoveAt(0);
			}
		}

		// Token: 0x060048A8 RID: 18600 RVA: 0x0015B240 File Offset: 0x00159440
		public static bool IsMapIDValid(string mapID)
		{
			if (mapID.IsNullOrEmpty())
			{
				return false;
			}
			if (mapID.Length != 8)
			{
				return false;
			}
			if (!Regex.IsMatch(mapID, "^[CFGHKMNPRTWXZ256789]+$"))
			{
				GTDev.LogError<string>("Invalid Characters in SharedBlocksManager IsMapIDValid map " + mapID, null);
				return false;
			}
			return true;
		}

		// Token: 0x060048A9 RID: 18601 RVA: 0x0015B278 File Offset: 0x00159478
		public static LinkedList<string> GetRecentUpVotes()
		{
			return SharedBlocksManager.recentUpVotes;
		}

		// Token: 0x060048AA RID: 18602 RVA: 0x0015B27F File Offset: 0x0015947F
		public static List<string> GetLocalMapIDs()
		{
			return SharedBlocksManager.localMapIds;
		}

		// Token: 0x060048AB RID: 18603 RVA: 0x0015B288 File Offset: 0x00159488
		private static void SetPublishTimeForSlot(int slotID, DateTime time)
		{
			SharedBlocksManager.LocalPublishInfo localPublishInfo;
			if (SharedBlocksManager.localPublishData.TryGetValue(slotID, out localPublishInfo))
			{
				localPublishInfo.publishTime = time.ToBinary();
				SharedBlocksManager.localPublishData[slotID] = localPublishInfo;
				return;
			}
			SharedBlocksManager.LocalPublishInfo localPublishInfo2 = new SharedBlocksManager.LocalPublishInfo
			{
				mapID = null,
				publishTime = time.ToBinary()
			};
			SharedBlocksManager.localPublishData.Add(slotID, localPublishInfo2);
		}

		// Token: 0x060048AC RID: 18604 RVA: 0x0015B2EC File Offset: 0x001594EC
		private static void SetMapIDAndPublishTimeForSlot(int slotID, string mapID, DateTime time)
		{
			SharedBlocksManager.LocalPublishInfo localPublishInfo = new SharedBlocksManager.LocalPublishInfo
			{
				mapID = mapID,
				publishTime = time.ToBinary()
			};
			SharedBlocksManager.localPublishData.AddOrUpdate(slotID, localPublishInfo);
		}

		// Token: 0x060048AD RID: 18605 RVA: 0x0015B328 File Offset: 0x00159528
		public static SharedBlocksManager.LocalPublishInfo GetPublishInfoForSlot(int slot)
		{
			SharedBlocksManager.LocalPublishInfo localPublishInfo;
			if (SharedBlocksManager.localPublishData.TryGetValue(slot, out localPublishInfo))
			{
				return localPublishInfo;
			}
			return new SharedBlocksManager.LocalPublishInfo
			{
				mapID = null,
				publishTime = DateTime.MinValue.ToBinary()
			};
		}

		// Token: 0x060048AE RID: 18606 RVA: 0x0015B368 File Offset: 0x00159568
		public void RequestDefaultSharedMap()
		{
			if (this.hasDefaultMap && Time.timeAsDouble < this.defaultMapCacheTime + 300.0)
			{
				Action<bool, SharedBlocksManager.SharedBlocksMap> onFoundDefaultSharedBlocksMap = this.OnFoundDefaultSharedBlocksMap;
				if (onFoundDefaultSharedBlocksMap == null)
				{
					return;
				}
				onFoundDefaultSharedBlocksMap(true, this.defaultMap);
				return;
			}
			else
			{
				if (this.getDefaultMapInProgress)
				{
					return;
				}
				this.hasDefaultMap = false;
				this.getDefaultMapInProgress = true;
				if (this.startingMapConfig.useMapID && SharedBlocksManager.IsMapIDValid(this.startingMapConfig.mapID))
				{
					this.defaultMap = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = this.startingMapConfig.mapID
					};
					this.RequestMapDataFromID(this.startingMapConfig.mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundTopMapData));
					return;
				}
				this.OnGetPopularMapsComplete += this.ChooseTopMap;
				this.RequestGetTopMaps(0, this.startingMapConfig.rangeMax, this.startingMapConfig.sortMethod.ToString());
				return;
			}
		}

		// Token: 0x060048AF RID: 18607 RVA: 0x0015B450 File Offset: 0x00159650
		private void ChooseTopMap(bool hasMaps)
		{
			this.OnGetPopularMapsComplete -= this.ChooseTopMap;
			if (hasMaps && this.latestPopularMaps.Count > 0)
			{
				int num = Random.Range(0, this.latestPopularMaps.Count);
				this.defaultMap = this.latestPopularMaps[num];
				if (this.defaultMap != null && SharedBlocksManager.IsMapIDValid(this.defaultMap.MapID))
				{
					this.RequestMapDataFromID(this.defaultMap.MapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundTopMapData));
					return;
				}
				this.getDefaultMapInProgress = false;
				Action<bool, SharedBlocksManager.SharedBlocksMap> onFoundDefaultSharedBlocksMap = this.OnFoundDefaultSharedBlocksMap;
				if (onFoundDefaultSharedBlocksMap == null)
				{
					return;
				}
				onFoundDefaultSharedBlocksMap(false, null);
				return;
			}
			else
			{
				this.getDefaultMapInProgress = false;
				Action<bool, SharedBlocksManager.SharedBlocksMap> onFoundDefaultSharedBlocksMap2 = this.OnFoundDefaultSharedBlocksMap;
				if (onFoundDefaultSharedBlocksMap2 == null)
				{
					return;
				}
				onFoundDefaultSharedBlocksMap2(false, null);
				return;
			}
		}

		// Token: 0x060048B0 RID: 18608 RVA: 0x0015B514 File Offset: 0x00159714
		private void FoundTopMapData(SharedBlocksManager.SharedBlocksMap map)
		{
			this.getDefaultMapInProgress = false;
			if (map == null || !SharedBlocksManager.IsMapIDValid(map.MapID) || map.MapID != this.defaultMap.MapID)
			{
				Action<bool, SharedBlocksManager.SharedBlocksMap> onFoundDefaultSharedBlocksMap = this.OnFoundDefaultSharedBlocksMap;
				if (onFoundDefaultSharedBlocksMap == null)
				{
					return;
				}
				onFoundDefaultSharedBlocksMap(false, null);
				return;
			}
			else
			{
				this.hasDefaultMap = true;
				this.defaultMapCacheTime = Time.timeAsDouble;
				this.defaultMap.MapData = map.MapData;
				Action<bool, SharedBlocksManager.SharedBlocksMap> onFoundDefaultSharedBlocksMap2 = this.OnFoundDefaultSharedBlocksMap;
				if (onFoundDefaultSharedBlocksMap2 == null)
				{
					return;
				}
				onFoundDefaultSharedBlocksMap2(true, this.defaultMap);
				return;
			}
		}

		// Token: 0x060048B1 RID: 18609 RVA: 0x0015B5A0 File Offset: 0x001597A0
		private void LoadPlayerPrefs()
		{
			string recentVotesPrefsKey = this.serializationConfig.recentVotesPrefsKey;
			string localMapsPrefsKey = this.serializationConfig.localMapsPrefsKey;
			string @string = PlayerPrefs.GetString(recentVotesPrefsKey, null);
			string string2 = PlayerPrefs.GetString(localMapsPrefsKey, null);
			if (!@string.IsNullOrEmpty())
			{
				try
				{
					SharedBlocksManager.recentUpVotes = JsonConvert.DeserializeObject<LinkedList<string>>(@string);
					while (SharedBlocksManager.recentUpVotes.Count > 10)
					{
						SharedBlocksManager.recentUpVotes.RemoveLast();
					}
					goto IL_0082;
				}
				catch (Exception ex)
				{
					GTDev.LogWarning<string>("SharedBlocksManager failed to deserialize Recent Up Votes " + ex.Message, null);
					SharedBlocksManager.recentUpVotes.Clear();
					goto IL_0082;
				}
			}
			SharedBlocksManager.recentUpVotes.Clear();
			IL_0082:
			if (!string2.IsNullOrEmpty())
			{
				SharedBlocksManager.localPublishData.Clear();
				SharedBlocksManager.localMapIds.Clear();
				try
				{
					SharedBlocksManager.localPublishData = JsonConvert.DeserializeObject<Dictionary<int, SharedBlocksManager.LocalPublishInfo>>(string2);
				}
				catch (Exception ex2)
				{
					GTDev.LogWarning<string>("SharedBlocksManager failed to deserialize localMapIDs " + ex2.Message, null);
					this.GetPlayfabLastSaveTime();
				}
				foreach (KeyValuePair<int, SharedBlocksManager.LocalPublishInfo> keyValuePair in SharedBlocksManager.localPublishData)
				{
					if (!keyValuePair.Value.mapID.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(keyValuePair.Value.mapID))
					{
						SharedBlocksManager.localMapIds.Add(keyValuePair.Value.mapID);
					}
				}
				Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
				if (onSaveTimeUpdated != null)
				{
					onSaveTimeUpdated();
				}
			}
			else
			{
				SharedBlocksManager.localMapIds.Clear();
				this.GetPlayfabLastSaveTime();
			}
			Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
			if (onRecentMapIdsUpdated == null)
			{
				return;
			}
			onRecentMapIdsUpdated();
		}

		// Token: 0x060048B2 RID: 18610 RVA: 0x0015B744 File Offset: 0x00159944
		private void SaveRecentVotesToPlayerPrefs()
		{
			PlayerPrefs.SetString(this.serializationConfig.recentVotesPrefsKey, JsonConvert.SerializeObject(SharedBlocksManager.recentUpVotes));
			PlayerPrefs.Save();
		}

		// Token: 0x060048B3 RID: 18611 RVA: 0x0015B765 File Offset: 0x00159965
		private void SaveLocalMapIdsToPlayerPrefs()
		{
			PlayerPrefs.SetString(this.serializationConfig.localMapsPrefsKey, JsonConvert.SerializeObject(SharedBlocksManager.localPublishData));
			PlayerPrefs.Save();
		}

		// Token: 0x060048B4 RID: 18612 RVA: 0x0015B788 File Offset: 0x00159988
		public void RequestVote(string mapID, bool up, Action<bool, string> callback)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestVote Client Not Logged into Mothership", null);
				if (callback != null)
				{
					callback(false, "NOT LOGGED IN");
				}
				return;
			}
			if (this.voteInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestVote already in progress", null);
				return;
			}
			this.voteInProgress = true;
			base.StartCoroutine(this.PostVote(new SharedBlocksManager.VoteRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mapId = mapID,
				vote = (up ? 1 : (-1))
			}, callback));
		}

		// Token: 0x060048B5 RID: 18613 RVA: 0x0015B80F File Offset: 0x00159A0F
		private IEnumerator PostVote(SharedBlocksManager.VoteRequest data, Action<bool, string> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/MapVote", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				string mapId = data.mapId;
				if (data.vote == -1)
				{
					if (SharedBlocksManager.recentUpVotes.Remove(mapId))
					{
						this.SaveRecentVotesToPlayerPrefs();
						Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
						if (onRecentMapIdsUpdated != null)
						{
							onRecentMapIdsUpdated();
						}
					}
				}
				else if (!SharedBlocksManager.recentUpVotes.Contains(mapId))
				{
					if (SharedBlocksManager.recentUpVotes.Count >= 10)
					{
						SharedBlocksManager.recentUpVotes.RemoveLast();
					}
					SharedBlocksManager.recentUpVotes.AddFirst(mapId);
					this.SaveRecentVotesToPlayerPrefs();
					Action onRecentMapIdsUpdated2 = SharedBlocksManager.OnRecentMapIdsUpdated;
					if (onRecentMapIdsUpdated2 != null)
					{
						onRecentMapIdsUpdated2();
					}
				}
				this.voteInProgress = false;
				if (callback != null)
				{
					callback(true, "");
				}
			}
			else
			{
				GTDev.LogError<string>(string.Format("PostVote Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text, null);
				long responseCode = request.responseCode;
				if (responseCode > 500L && responseCode < 600L)
				{
					retry = true;
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					retry = true;
				}
				else
				{
					this.voteInProgress = false;
					if (callback != null)
					{
						callback(false, "REQUEST ERROR");
					}
				}
			}
			if (retry)
			{
				if (this.voteRetryCount < this.maxRetriesOnFail)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.voteRetryCount + 1));
					this.voteRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.voteInProgress = false;
					this.RequestVote(data.mapId, data.vote == 1, callback);
				}
				else
				{
					this.voteRetryCount = 0;
					this.voteInProgress = false;
					if (callback != null)
					{
						callback(false, "CONNECTION ERROR");
					}
				}
			}
			yield break;
		}

		// Token: 0x060048B6 RID: 18614 RVA: 0x0015B82C File Offset: 0x00159A2C
		private void RequestPublishMap(string userMetadataKey)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestPublishMap Client Not Logged into Mothership", null);
				this.PublishMapComplete(false, userMetadataKey, string.Empty, 0L);
				return;
			}
			if (this.publishRequestInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestPublishMap Publish Request in progress", null);
				return;
			}
			this.publishRequestInProgress = true;
			base.StartCoroutine(this.PostPublishMapRequest(new SharedBlocksManager.PublishMapRequestData
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				userdataMetadataKey = userMetadataKey,
				playerNickname = GorillaTagger.Instance.offlineVRRig.playerNameVisible
			}, new SharedBlocksManager.PublishMapRequestCallback(this.PublishMapComplete)));
		}

		// Token: 0x060048B7 RID: 18615 RVA: 0x0015B8C8 File Offset: 0x00159AC8
		private void PublishMapComplete(bool success, string key, [CanBeNull] string mapID, long response)
		{
			this.publishRequestInProgress = false;
			if (success)
			{
				int num = this.serializationConfig.scanSlotMothershipKeys.IndexOf(key);
				if (num >= 0)
				{
					SharedBlocksManager.LocalPublishInfo localPublishInfo;
					if (SharedBlocksManager.localPublishData.TryGetValue(num, out localPublishInfo))
					{
						SharedBlocksManager.localMapIds.Remove(localPublishInfo.mapID);
					}
					SharedBlocksManager.SetMapIDAndPublishTimeForSlot(num, mapID, DateTime.Now);
					this.SaveLocalMapIdsToPlayerPrefs();
				}
				if (!SharedBlocksManager.localMapIds.Contains(mapID))
				{
					SharedBlocksManager.localMapIds.Add(mapID);
					Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
					if (onRecentMapIdsUpdated != null)
					{
						onRecentMapIdsUpdated();
					}
				}
				SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = mapID,
					MapData = this.privateScanDataCache[num],
					CreatorNickName = GorillaTagger.Instance.offlineVRRig.playerNameVisible,
					UpdateTime = DateTime.Now
				};
				this.AddMapToResponseCache(sharedBlocksMap);
				Action<int> onSavePrivateScanSuccess = this.OnSavePrivateScanSuccess;
				if (onSavePrivateScanSuccess != null)
				{
					onSavePrivateScanSuccess(this.currentSaveScanIndex);
				}
			}
			else
			{
				Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
				if (onSavePrivateScanFailed != null)
				{
					onSavePrivateScanFailed(this.currentSaveScanIndex, "ERROR PUBLISHING: " + response.ToString());
				}
			}
			this.currentSaveScanIndex = -1;
			this.currentSaveScanData = string.Empty;
		}

		// Token: 0x060048B8 RID: 18616 RVA: 0x0015B9E7 File Offset: 0x00159BE7
		private IEnumerator PostPublishMapRequest(SharedBlocksManager.PublishMapRequestData data, SharedBlocksManager.PublishMapRequestCallback callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/Publish", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				GTDev.Log<string>("PostPublishMapRequest Success: raw response: " + request.downloadHandler.text, null);
				try
				{
					string text = request.downloadHandler.text;
					bool flag = !text.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(text);
					if (callback != null)
					{
						callback(flag, data.userdataMetadataKey, text, request.responseCode);
					}
					goto IL_01F8;
				}
				catch (Exception ex)
				{
					GTDev.LogError<string>("SharedBlocksManager PostPublishMapRequest " + ex.Message, null);
					if (callback != null)
					{
						callback(false, data.userdataMetadataKey, null, request.responseCode);
					}
					goto IL_01F8;
				}
			}
			long responseCode = request.responseCode;
			if (responseCode > 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
			else if (callback != null)
			{
				callback(false, data.userdataMetadataKey, string.Empty, request.responseCode);
			}
			IL_01F8:
			if (retry)
			{
				if (this.postPublishMapRetryCount < this.maxRetriesOnFail)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.postPublishMapRetryCount + 1));
					this.postPublishMapRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.publishRequestInProgress = false;
					this.RequestPublishMap(data.userdataMetadataKey);
				}
				else
				{
					this.postPublishMapRetryCount = 0;
					if (callback != null)
					{
						callback(false, data.userdataMetadataKey, string.Empty, request.responseCode);
					}
				}
			}
			yield break;
		}

		// Token: 0x060048B9 RID: 18617 RVA: 0x0015BA04 File Offset: 0x00159C04
		public void RequestMapDataFromID(string mapID, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestMapDataFromID Client Not Logged into Mothership", null);
				if (callback != null)
				{
					callback(null);
				}
				return;
			}
			SharedBlocksManager.SharedBlocksMap sharedBlocksMap;
			if (this.TryGetCachedSharedBlocksMapByMapID(mapID, out sharedBlocksMap))
			{
				if (callback != null)
				{
					callback(sharedBlocksMap);
				}
				return;
			}
			if (this.getMapDataFromIDInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestMapDataFromID Fetch already in progress", null);
				return;
			}
			this.getMapDataFromIDInProgress = true;
			base.StartCoroutine(this.GetMapDataFromID(new SharedBlocksManager.GetMapDataFromIDRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mapId = mapID
			}, callback));
		}

		// Token: 0x060048BA RID: 18618 RVA: 0x0015BA8F File Offset: 0x00159C8F
		private IEnumerator GetMapDataFromID(SharedBlocksManager.GetMapDataFromIDRequest data, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/GetMapData", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				string text = request.downloadHandler.text;
				this.GetMapDataFromIDComplete(data.mapId, text, callback);
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode > 500L && responseCode < 600L)
				{
					retry = true;
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					retry = true;
				}
				else
				{
					this.GetMapDataFromIDComplete(data.mapId, null, callback);
				}
			}
			if (retry)
			{
				if (this.getMapDataFromIDRetryCount < this.maxRetriesOnFail)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.getMapDataFromIDRetryCount + 1));
					this.getMapDataFromIDRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.getMapDataFromIDInProgress = false;
					this.RequestMapDataFromID(data.mapId, callback);
				}
				else
				{
					this.getMapDataFromIDRetryCount = 0;
					this.GetMapDataFromIDComplete(data.mapId, null, callback);
				}
			}
			yield break;
		}

		// Token: 0x060048BB RID: 18619 RVA: 0x0015BAAC File Offset: 0x00159CAC
		private void GetMapDataFromIDComplete(string mapID, [CanBeNull] string response, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			this.getMapDataFromIDInProgress = false;
			if (response == null)
			{
				if (callback != null)
				{
					callback(null);
					return;
				}
			}
			else
			{
				SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = mapID,
					MapData = response
				};
				this.AddMapToResponseCache(sharedBlocksMap);
				if (callback != null)
				{
					callback(sharedBlocksMap);
				}
			}
		}

		// Token: 0x060048BC RID: 18620 RVA: 0x0015BAF4 File Offset: 0x00159CF4
		private void RequestGetTopMaps(int pageNum, int pageSize, string sort)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps Client Not Logged into Mothership", null);
				return;
			}
			if (this.getTopMapsInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps already in progress", null);
				return;
			}
			if (!this.hasCachedTopMaps || Time.timeAsDouble > this.lastGetTopMapsTime + 300.0)
			{
				this.getTopMapsInProgress = true;
				this.lastGetTopMapsTime = Time.timeAsDouble;
				base.StartCoroutine(this.GetTopMaps(new SharedBlocksManager.GetMapsRequest
				{
					mothershipId = MothershipClientContext.MothershipId,
					mothershipToken = MothershipClientContext.Token,
					page = pageNum,
					pageSize = pageSize,
					sort = sort,
					ShowInactive = false
				}, new Action<List<SharedBlocksManager.SharedBlocksMapMetaData>>(this.GetTopMapsComplete)));
				return;
			}
			GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps sending cached value", null);
			Action<bool> onGetPopularMapsComplete = this.OnGetPopularMapsComplete;
			if (onGetPopularMapsComplete == null)
			{
				return;
			}
			onGetPopularMapsComplete(true);
		}

		// Token: 0x060048BD RID: 18621 RVA: 0x0015BBC6 File Offset: 0x00159DC6
		private IEnumerator GetTopMaps(SharedBlocksManager.GetMapsRequest data, Action<List<SharedBlocksManager.SharedBlocksMapMetaData>> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/GetMaps", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				try
				{
					List<SharedBlocksManager.SharedBlocksMapMetaData> list = JsonConvert.DeserializeObject<List<SharedBlocksManager.SharedBlocksMapMetaData>>(request.downloadHandler.text);
					if (callback != null)
					{
						callback(list);
					}
					goto IL_0162;
				}
				catch (Exception)
				{
					if (callback != null)
					{
						callback(null);
					}
					goto IL_0162;
				}
			}
			long responseCode = request.responseCode;
			if (responseCode > 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
			else if (callback != null)
			{
				callback(null);
			}
			IL_0162:
			if (retry)
			{
				if (this.getTopMapsRetryCount < this.maxRetriesOnFail)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.getTopMapsRetryCount + 1));
					this.getTopMapsRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.getTopMapsInProgress = false;
					this.RequestGetTopMaps(data.page, data.pageSize, data.sort);
				}
				else
				{
					this.getTopMapsRetryCount = 0;
					if (callback != null)
					{
						callback(null);
					}
				}
			}
			yield break;
		}

		// Token: 0x060048BE RID: 18622 RVA: 0x0015BBE4 File Offset: 0x00159DE4
		private void GetTopMapsComplete([CanBeNull] List<SharedBlocksManager.SharedBlocksMapMetaData> maps)
		{
			this.getTopMapsInProgress = false;
			if (maps != null)
			{
				this.latestPopularMaps.Clear();
				foreach (SharedBlocksManager.SharedBlocksMapMetaData sharedBlocksMapMetaData in maps)
				{
					if (sharedBlocksMapMetaData != null && SharedBlocksManager.IsMapIDValid(sharedBlocksMapMetaData.mapId))
					{
						DateTime dateTime = DateTime.MinValue;
						DateTime dateTime2 = DateTime.MinValue;
						try
						{
							dateTime = DateTime.Parse(sharedBlocksMapMetaData.createdTime);
							dateTime2 = DateTime.Parse(sharedBlocksMapMetaData.updatedTime);
						}
						catch (Exception ex)
						{
							GTDev.LogWarning<string>("SharedBlocksManager GetTopMaps bad update or create time" + ex.Message, null);
						}
						SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
						{
							MapID = sharedBlocksMapMetaData.mapId,
							CreatorID = null,
							CreatorNickName = sharedBlocksMapMetaData.nickname,
							CreateTime = dateTime,
							UpdateTime = dateTime2,
							MapData = null
						};
						this.latestPopularMaps.Add(sharedBlocksMap);
					}
				}
				this.hasCachedTopMaps = true;
				Action<bool> onGetPopularMapsComplete = this.OnGetPopularMapsComplete;
				if (onGetPopularMapsComplete == null)
				{
					return;
				}
				onGetPopularMapsComplete(true);
				return;
			}
			else
			{
				Action<bool> onGetPopularMapsComplete2 = this.OnGetPopularMapsComplete;
				if (onGetPopularMapsComplete2 == null)
				{
					return;
				}
				onGetPopularMapsComplete2(false);
				return;
			}
		}

		// Token: 0x060048BF RID: 18623 RVA: 0x0015BD20 File Offset: 0x00159F20
		private void RequestUpdateMapActive(string userMetadataKey, bool active)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestUpdateMapActive Client Not Logged into Mothership", null);
				return;
			}
			if (this.updateMapActiveInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestUpdateMapActive already in progress", null);
				return;
			}
			this.updateMapActiveInProgress = true;
			base.StartCoroutine(this.PostUpdateMapActive(new SharedBlocksManager.UpdateMapActiveRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				userdataMetadataKey = userMetadataKey,
				setActive = active
			}, new Action<bool>(this.OnUpdatedMapActiveComplete)));
		}

		// Token: 0x060048C0 RID: 18624 RVA: 0x0015BD9D File Offset: 0x00159F9D
		private IEnumerator PostUpdateMapActive(SharedBlocksManager.UpdateMapActiveRequest data, Action<bool> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/UpdateMapActive", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == UnityWebRequest.Result.Success)
			{
				if (callback != null)
				{
					callback(true);
				}
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode > 500L && responseCode < 600L)
				{
					retry = true;
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback(false);
				}
			}
			if (retry)
			{
				if (this.updateMapActiveRetryCount < this.maxRetriesOnFail)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.updateMapActiveRetryCount + 1));
					this.updateMapActiveRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.updateMapActiveInProgress = false;
					this.RequestUpdateMapActive(data.userdataMetadataKey, data.setActive);
				}
				else
				{
					this.updateMapActiveRetryCount = 0;
					if (callback != null)
					{
						callback(false);
					}
				}
			}
			yield break;
		}

		// Token: 0x060048C1 RID: 18625 RVA: 0x0015BDBA File Offset: 0x00159FBA
		private void OnUpdatedMapActiveComplete(bool success)
		{
			this.updateMapActiveInProgress = false;
		}

		// Token: 0x060048C2 RID: 18626 RVA: 0x0015BDC4 File Offset: 0x00159FC4
		private async Task WaitForPlayfabSessionToken()
		{
			while (!PlayFabAuthenticator.instance || PlayFabAuthenticator.instance.GetPlayFabPlayerId().IsNullOrEmpty() || PlayFabAuthenticator.instance.GetPlayFabSessionTicket().IsNullOrEmpty() || PlayFabAuthenticator.instance.userID.IsNullOrEmpty())
			{
				await Task.Yield();
				await Task.Delay(1000);
			}
		}

		// Token: 0x060048C3 RID: 18627 RVA: 0x0015BDFF File Offset: 0x00159FFF
		public void RequestTableConfiguration()
		{
			if (this.fetchedTableConfig)
			{
				Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
				if (onGetTableConfiguration == null)
				{
					return;
				}
				onGetTableConfiguration(this.tableConfigResponse);
			}
		}

		// Token: 0x060048C4 RID: 18628 RVA: 0x0015BE20 File Offset: 0x0015A020
		private void FetchConfigurationFromTitleData()
		{
			PlayFabClientAPI.GetTitleData(new GetTitleDataRequest
			{
				Keys = new List<string> { this.serializationConfig.tableConfigurationKey }
			}, new Action<GetTitleDataResult>(this.OnGetConfigurationSuccess), new Action<PlayFabError>(this.OnGetConfigurationFail), null, null);
		}

		// Token: 0x060048C5 RID: 18629 RVA: 0x0015BE70 File Offset: 0x0015A070
		private void OnGetConfigurationSuccess(GetTitleDataResult result)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetConfigurationSuccess", null);
			string text;
			if (result.Data.TryGetValue(this.serializationConfig.tableConfigurationKey, out text))
			{
				this.tableConfigResponse = text;
				this.fetchedTableConfig = true;
				Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
				if (onGetTableConfiguration == null)
				{
					return;
				}
				onGetTableConfiguration(this.tableConfigResponse);
			}
		}

		// Token: 0x060048C6 RID: 18630 RVA: 0x0015BEC8 File Offset: 0x0015A0C8
		private void OnGetConfigurationFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnGetConfigurationFail " + error.Error.ToString(), null);
			if (error.Error == PlayFabErrorCode.ConnectionError && this.fetchTableConfigRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.fetchTableConfigRetryCount + 1));
				this.fetchTableConfigRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(num, new Action(this.FetchConfigurationFromTitleData)));
				return;
			}
			this.tableConfigResponse = string.Empty;
			this.fetchedTableConfig = true;
			Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
			if (onGetTableConfiguration == null)
			{
				return;
			}
			onGetTableConfiguration(this.tableConfigResponse);
		}

		// Token: 0x060048C7 RID: 18631 RVA: 0x0015BF73 File Offset: 0x0015A173
		private IEnumerator RetryAfterWaitTime(int waitTime, Action function)
		{
			yield return new WaitForSeconds((float)waitTime);
			if (function != null)
			{
				function();
			}
			yield break;
		}

		// Token: 0x060048C8 RID: 18632 RVA: 0x0015BF8C File Offset: 0x0015A18C
		private void FetchSharedBlocksStartingMapConfig()
		{
			PlayFabClientAPI.GetTitleData(new GetTitleDataRequest
			{
				Keys = new List<string> { this.serializationConfig.startingMapConfigKey }
			}, new Action<GetTitleDataResult>(this.OnGetStartingMapConfigSuccess), new Action<PlayFabError>(this.OnGetStartingMapConfigFail), null, null);
		}

		// Token: 0x060048C9 RID: 18633 RVA: 0x0015BFDC File Offset: 0x0015A1DC
		private void OnGetStartingMapConfigSuccess(GetTitleDataResult result)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetStartingMapConfigSuccess", null);
			this.fetchedStartingMapConfig = true;
			this.ResetStartingMapConfig();
			string text;
			if (result.Data.TryGetValue(this.serializationConfig.startingMapConfigKey, out text))
			{
				if (text.IsNullOrEmpty())
				{
					return;
				}
				try
				{
					SharedBlocksManager.StartingMapConfig startingMapConfig = JsonUtility.FromJson<SharedBlocksManager.StartingMapConfig>(text);
					if (startingMapConfig.useMapID)
					{
						if (SharedBlocksManager.IsMapIDValid(startingMapConfig.mapID))
						{
							this.startingMapConfig.useMapID = true;
							this.startingMapConfig.mapID = startingMapConfig.mapID;
						}
						else
						{
							GTDev.LogError<string>("SharedBlocksManager OnGetStartingMapConfigSuccess Title Data Default Map Config has Invalid Map ID", null);
						}
					}
					else
					{
						this.startingMapConfig.rangeMax = Mathf.Clamp(startingMapConfig.rangeMax, 1, 1000);
						if (!startingMapConfig.sortMethod.IsNullOrEmpty() && (startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.Top.ToString()) || startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.NewlyCreated.ToString()) || startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.RecentlyUpdated.ToString())))
						{
							this.startingMapConfig.sortMethod = startingMapConfig.sortMethod;
						}
						else
						{
							GTDev.LogError<string>("SharedBlocksManager OnGetStartingMapConfigSuccess Unknown sort method " + startingMapConfig.sortMethod, null);
						}
					}
				}
				catch (Exception ex)
				{
					GTDev.LogError<string>("SharedBlocksManager OnGetStartingMapConfigSuccess Exception Deserializing " + ex.Message, null);
				}
			}
		}

		// Token: 0x060048CA RID: 18634 RVA: 0x0015C148 File Offset: 0x0015A348
		private void ResetStartingMapConfig()
		{
			this.startingMapConfig = new SharedBlocksManager.StartingMapConfig
			{
				rangeMax = 10,
				sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
				useMapID = false,
				mapID = null
			};
		}

		// Token: 0x060048CB RID: 18635 RVA: 0x0015C194 File Offset: 0x0015A394
		private void OnGetStartingMapConfigFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnGetStartingMapConfigFail " + error.Error.ToString(), null);
			if (error.Error == PlayFabErrorCode.ConnectionError && this.fetchStartingMapConfigRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.fetchStartingMapConfigRetryCount + 1));
				this.fetchStartingMapConfigRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(num, new Action(this.FetchSharedBlocksStartingMapConfig)));
				return;
			}
			this.ResetStartingMapConfig();
			this.fetchedStartingMapConfig = true;
		}

		// Token: 0x060048CC RID: 18636 RVA: 0x0015C224 File Offset: 0x0015A424
		public void FetchTitleDataBuild()
		{
			if (!this.fetchTitleDataBuildComplete)
			{
				if (!this.fetchTitleDataBuildInProgress)
				{
					this.fetchTitleDataBuildInProgress = true;
					base.StartCoroutine(this.SendTitleDataRequest(new GetTitleDataRequest
					{
						Keys = new List<string> { this.serializationConfig.titleDataKey }
					}, new Action<GetTitleDataResult>(this.OnGetTitleDataBuildSuccess), new Action<PlayFabError>(this.OnGetTitleDataBuildFail)));
				}
				return;
			}
			Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
			if (onGetTitleDataBuildComplete == null)
			{
				return;
			}
			onGetTitleDataBuildComplete(this.titleDataBuildCache);
		}

		// Token: 0x060048CD RID: 18637 RVA: 0x0015C2A5 File Offset: 0x0015A4A5
		private IEnumerator SendTitleDataRequest(GetTitleDataRequest request, Action<GetTitleDataResult> successCallback, Action<PlayFabError> failCallback)
		{
			while (!PlayFabSettings.staticPlayer.IsClientLoggedIn())
			{
				yield return new WaitForSeconds(5f);
			}
			PlayFabClientAPI.GetTitleData(request, successCallback, failCallback, null, null);
			yield break;
		}

		// Token: 0x060048CE RID: 18638 RVA: 0x0015C2C4 File Offset: 0x0015A4C4
		private void OnGetTitleDataBuildSuccess(GetTitleDataResult result)
		{
			this.fetchTitleDataBuildInProgress = false;
			GTDev.Log<string>("SharedBlocksManager OnGetTitleDataBuildSuccess", null);
			string text;
			if (result.Data.TryGetValue(this.serializationConfig.titleDataKey, out text) && !text.IsNullOrEmpty())
			{
				this.titleDataBuildCache = text;
				this.fetchTitleDataBuildComplete = true;
				Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
				if (onGetTitleDataBuildComplete == null)
				{
					return;
				}
				onGetTitleDataBuildComplete(this.titleDataBuildCache);
				return;
			}
			else
			{
				this.titleDataBuildCache = string.Empty;
				this.fetchTitleDataBuildComplete = true;
				Action<string> onGetTitleDataBuildComplete2 = this.OnGetTitleDataBuildComplete;
				if (onGetTitleDataBuildComplete2 == null)
				{
					return;
				}
				onGetTitleDataBuildComplete2(this.titleDataBuildCache);
				return;
			}
		}

		// Token: 0x060048CF RID: 18639 RVA: 0x0015C354 File Offset: 0x0015A554
		private void OnGetTitleDataBuildFail(PlayFabError error)
		{
			this.fetchTitleDataBuildInProgress = false;
			GTDev.LogWarning<string>("SharedBlocksManager FetchTitleDataBuildFail " + error.Error.ToString(), null);
			if (error.Error == PlayFabErrorCode.ConnectionError && this.fetchTitleDataRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.fetchTitleDataRetryCount + 1));
				this.fetchTitleDataRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(num, new Action(this.FetchTitleDataBuild)));
				return;
			}
			this.titleDataBuildCache = string.Empty;
			this.fetchTitleDataBuildComplete = true;
			Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
			if (onGetTitleDataBuildComplete == null)
			{
				return;
			}
			onGetTitleDataBuildComplete(this.titleDataBuildCache);
		}

		// Token: 0x060048D0 RID: 18640 RVA: 0x0015C406 File Offset: 0x0015A606
		private string GetPlayfabKeyForSlot(int slot)
		{
			return this.serializationConfig.playfabScanKey + slot.ToString("D2");
		}

		// Token: 0x060048D1 RID: 18641 RVA: 0x0015C424 File Offset: 0x0015A624
		private string GetPlayfabSlotTimeKey(int slot)
		{
			return this.serializationConfig.playfabScanKey + slot.ToString("D2") + this.serializationConfig.timeAppend;
		}

		// Token: 0x060048D2 RID: 18642 RVA: 0x0015C450 File Offset: 0x0015A650
		private void GetPlayfabLastSaveTime()
		{
			if (!this.hasQueriedSaveTime)
			{
				global::PlayFab.ClientModels.GetUserDataRequest getUserDataRequest = new global::PlayFab.ClientModels.GetUserDataRequest
				{
					PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
					Keys = SharedBlocksManager.saveDateKeys
				};
				try
				{
					PlayFabClientAPI.GetUserData(getUserDataRequest, new Action<GetUserDataResult>(this.OnGetLastSaveTimeSuccess), new Action<PlayFabError>(this.OnGetLastSaveTimeFailure), null, null);
				}
				catch (PlayFabException ex)
				{
					this.OnGetLastSaveTimeFailure(new PlayFabError
					{
						Error = PlayFabErrorCode.Unknown,
						ErrorMessage = ex.Message
					});
				}
				this.hasQueriedSaveTime = true;
				return;
			}
			Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
			if (onSaveTimeUpdated == null)
			{
				return;
			}
			onSaveTimeUpdated();
		}

		// Token: 0x060048D3 RID: 18643 RVA: 0x0015C4F4 File Offset: 0x0015A6F4
		private void OnGetLastSaveTimeSuccess(GetUserDataResult result)
		{
			bool flag = false;
			for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
			{
				UserDataRecord userDataRecord;
				if (result.Data.TryGetValue(this.GetPlayfabSlotTimeKey(i), out userDataRecord))
				{
					flag = true;
					DateTime lastUpdated = userDataRecord.LastUpdated;
					SharedBlocksManager.SetPublishTimeForSlot(i, lastUpdated + DateTimeOffset.Now.Offset);
				}
			}
			if (flag)
			{
				this.SaveLocalMapIdsToPlayerPrefs();
			}
			Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
			if (onSaveTimeUpdated == null)
			{
				return;
			}
			onSaveTimeUpdated();
		}

		// Token: 0x060048D4 RID: 18644 RVA: 0x0015C564 File Offset: 0x0015A764
		private void OnGetLastSaveTimeFailure(PlayFabError error)
		{
			string text = ((error != null) ? error.ErrorMessage : null) ?? "Null";
			GTDev.LogError<string>("SharedBlocksManager GetLastSaveTimeFailure " + text, null);
		}

		// Token: 0x060048D5 RID: 18645 RVA: 0x0015C598 File Offset: 0x0015A798
		private void FetchBuildFromPlayfab()
		{
			if (this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex])
			{
				Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete != null)
				{
					onFetchPrivateScanComplete(this.currentGetScanIndex, true);
				}
				this.currentGetScanIndex = -1;
				this.getScanInProgress = false;
				return;
			}
			global::PlayFab.ClientModels.GetUserDataRequest getUserDataRequest = new global::PlayFab.ClientModels.GetUserDataRequest
			{
				PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				Keys = new List<string> { this.GetPlayfabKeyForSlot(this.currentGetScanIndex) }
			};
			base.StartCoroutine(this.SendPlayfabUserDataRequest(getUserDataRequest, new Action<GetUserDataResult>(this.OnFetchBuildFromPlayfabSuccess), new Action<PlayFabError>(this.OnFetchBuildFromPlayfabFail)));
		}

		// Token: 0x060048D6 RID: 18646 RVA: 0x0015C636 File Offset: 0x0015A836
		private IEnumerator SendPlayfabUserDataRequest(global::PlayFab.ClientModels.GetUserDataRequest request, Action<GetUserDataResult> resultCallback, Action<PlayFabError> errorCallback)
		{
			while (!PlayFabSettings.staticPlayer.IsClientLoggedIn())
			{
				yield return new WaitForSeconds(5f);
			}
			try
			{
				PlayFabClientAPI.GetUserData(request, resultCallback, errorCallback, null, null);
				yield break;
			}
			catch (PlayFabException ex)
			{
				if (errorCallback != null)
				{
					errorCallback(new PlayFabError
					{
						Error = PlayFabErrorCode.Unknown,
						ErrorMessage = ex.Message
					});
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x060048D7 RID: 18647 RVA: 0x0015C654 File Offset: 0x0015A854
		private void OnFetchBuildFromPlayfabSuccess(GetUserDataResult result)
		{
			this.getScanInProgress = false;
			GTDev.Log<string>("SharedBlocksManager OnFetchBuildsFromPlayfabSuccess", null);
			UserDataRecord userDataRecord;
			if (result != null && result.Data != null && result.Data.TryGetValue(this.GetPlayfabKeyForSlot(this.currentGetScanIndex), out userDataRecord))
			{
				this.privateScanDataCache[this.currentGetScanIndex] = userDataRecord.Value;
				this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
				if (!userDataRecord.Value.IsNullOrEmpty())
				{
					this.RequestSavePrivateScan(this.currentGetScanIndex, userDataRecord.Value);
				}
			}
			else
			{
				this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
				this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
			}
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(this.currentGetScanIndex, true);
			}
			this.currentGetScanIndex = -1;
		}

		// Token: 0x060048D8 RID: 18648 RVA: 0x0015C71C File Offset: 0x0015A91C
		private void OnFetchBuildFromPlayfabFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnFetchBuildsFromPlayfabFail " + (((error != null) ? error.ErrorMessage : null) ?? "Null"), null);
			if (error != null && error.Error == PlayFabErrorCode.ConnectionError && this.fetchPlayfabBuildsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.fetchPlayfabBuildsRetryCount + 1));
				this.fetchPlayfabBuildsRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(num, new Action(this.FetchBuildFromPlayfab)));
				return;
			}
			this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
			this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
			this.getScanInProgress = false;
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(this.currentGetScanIndex, false);
			}
			this.currentGetScanIndex = -1;
		}

		// Token: 0x060048D9 RID: 18649 RVA: 0x0015C7EC File Offset: 0x0015A9EC
		private async Task WaitForMothership()
		{
			while (!MothershipClientContext.IsClientLoggedIn())
			{
				await Task.Yield();
				await Task.Delay(1000);
			}
		}

		// Token: 0x060048DA RID: 18650 RVA: 0x0015C828 File Offset: 0x0015AA28
		public void RequestSavePrivateScan(int scanIndex, string scanData)
		{
			if (scanIndex < 0 || scanIndex >= this.serializationConfig.scanSlotMothershipKeys.Count)
			{
				GTDev.LogError<string>(string.Format("SharedBlocksManager RequestSaveScanToMothership: scan index {0} out of bounds", scanIndex), null);
				return;
			}
			this.currentSaveScanIndex = scanIndex;
			this.currentSaveScanData = scanData;
			if (!this.hasPulledPrivateScanMothership[scanIndex])
			{
				this.PullMothershipPrivateScanThenPush(scanIndex);
				return;
			}
			this.privateScanDataCache[scanIndex] = scanData;
			this.RequestSetMothershipUserData(this.serializationConfig.scanSlotMothershipKeys[scanIndex], scanData);
		}

		// Token: 0x060048DB RID: 18651 RVA: 0x0015C8A4 File Offset: 0x0015AAA4
		private void PullMothershipPrivateScanThenPush(int scanIndex)
		{
			if (this.getScanInProgress && this.currentGetScanIndex != scanIndex)
			{
				GTDev.LogWarning<string>("SharedBLocksManager PullMothershipPrivateScanThenPush GetScan in progress", null);
				Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
				if (onSavePrivateScanFailed != null)
				{
					onSavePrivateScanFailed(scanIndex, "ERROR SAVING: BUSY");
				}
				this.currentSaveScanIndex = -1;
				this.currentSaveScanData = string.Empty;
				return;
			}
			this.OnFetchPrivateScanComplete += this.PushMothershipPrivateScan;
			this.RequestFetchPrivateScan(scanIndex);
		}

		// Token: 0x060048DC RID: 18652 RVA: 0x0015C910 File Offset: 0x0015AB10
		private void PushMothershipPrivateScan(int scan, bool success)
		{
			if (scan == this.currentSaveScanIndex)
			{
				this.OnFetchPrivateScanComplete -= this.PushMothershipPrivateScan;
				this.privateScanDataCache[this.currentSaveScanIndex] = this.currentSaveScanData;
				this.RequestSetMothershipUserData(this.serializationConfig.scanSlotMothershipKeys[this.currentSaveScanIndex], this.currentSaveScanData);
			}
		}

		// Token: 0x060048DD RID: 18653 RVA: 0x0015C970 File Offset: 0x0015AB70
		private void RequestSetMothershipUserData(string keyName, string value)
		{
			if (this.saveScanInProgress)
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: request already in progress");
				return;
			}
			this.saveScanInProgress = true;
			try
			{
				if (!MothershipClientApiUnity.SetUserDataValue(keyName, value, new Action<SetUserDataResponse>(this.OnSetMothershipUserDataSuccess), new Action<MothershipError, int>(this.OnSetMothershipUserDataFail), ""))
				{
					Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: SetUserDataValue Fail");
					this.OnSetMothershipDataComplete(false);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: exception " + ex.Message);
				this.OnSetMothershipDataComplete(false);
			}
		}

		// Token: 0x060048DE RID: 18654 RVA: 0x0015CA00 File Offset: 0x0015AC00
		private void OnSetMothershipUserDataSuccess(SetUserDataResponse response)
		{
			GTDev.Log<string>("SharedBlocksManager OnSetMothershipUserDataSuccess", null);
			this.OnSetMothershipDataComplete(true);
			response.Dispose();
		}

		// Token: 0x060048DF RID: 18655 RVA: 0x0015CA1C File Offset: 0x0015AC1C
		private void OnSetMothershipUserDataFail(MothershipError error, int status)
		{
			string text = ((error == null) ? status.ToString() : error.Message);
			GTDev.LogError<string>("SharedBlocksManager OnSetMothershipUserDataFail: " + text, null);
			this.OnSetMothershipDataComplete(false);
			if (error != null)
			{
				error.Dispose();
			}
		}

		// Token: 0x060048E0 RID: 18656 RVA: 0x0015CA60 File Offset: 0x0015AC60
		private void OnSetMothershipDataComplete(bool success)
		{
			this.saveScanInProgress = false;
			if (this.currentSaveScanIndex < 0 || this.currentSaveScanIndex >= BuilderScanKiosk.NUM_SAVE_SLOTS)
			{
				this.currentSaveScanIndex = -1;
				this.currentSaveScanData = string.Empty;
				return;
			}
			if (success)
			{
				this.RequestPublishMap(this.serializationConfig.scanSlotMothershipKeys[this.currentSaveScanIndex]);
				return;
			}
			Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
			if (onSavePrivateScanFailed != null)
			{
				onSavePrivateScanFailed(this.currentSaveScanIndex, "ERROR SAVING");
			}
			this.currentSaveScanIndex = -1;
			this.currentSaveScanData = string.Empty;
		}

		// Token: 0x060048E1 RID: 18657 RVA: 0x0015CAEB File Offset: 0x0015ACEB
		public bool TryGetPrivateScanResponse(int scanSlot, out string scanData)
		{
			if (scanSlot < 0 || scanSlot >= this.privateScanDataCache.Length || !this.hasPulledPrivateScanMothership[scanSlot])
			{
				scanData = string.Empty;
				return false;
			}
			scanData = this.privateScanDataCache[scanSlot];
			return true;
		}

		// Token: 0x060048E2 RID: 18658 RVA: 0x0015CB1C File Offset: 0x0015AD1C
		public void RequestFetchPrivateScan(int slot)
		{
			if (slot < 0 || slot >= BuilderScanKiosk.NUM_SAVE_SLOTS)
			{
				GTDev.LogError<string>(string.Format("SharedBlocksManager RequestSaveScan: slot {0} OOB", slot), null);
				slot = Mathf.Clamp(slot, 0, BuilderScanKiosk.NUM_SAVE_SLOTS - 1);
			}
			if (this.hasPulledPrivateScanMothership[slot])
			{
				bool flag = this.privateScanDataCache[slot].Length > 0;
				Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete == null)
				{
					return;
				}
				onFetchPrivateScanComplete(slot, flag);
				return;
			}
			else
			{
				if (this.getScanInProgress)
				{
					Debug.LogError("SharedBlocksManager RequestFetchPrivateScan: request already in progress");
					if (slot != this.currentGetScanIndex)
					{
						Action<int, bool> onFetchPrivateScanComplete2 = this.OnFetchPrivateScanComplete;
						if (onFetchPrivateScanComplete2 == null)
						{
							return;
						}
						onFetchPrivateScanComplete2(slot, false);
					}
					return;
				}
				this.currentGetScanIndex = slot;
				this.getScanInProgress = true;
				try
				{
					if (!MothershipClientApiUnity.GetUserDataValue(this.serializationConfig.scanSlotMothershipKeys[slot], new Action<MothershipUserData>(this.OnGetMothershipPrivateScanSuccess), new Action<MothershipError, int>(this.OnGetMothershipPrivateScanFail), ""))
					{
						Debug.LogError("SharedBlocksManager RequestFetchPrivateScan failed ");
						this.currentGetScanIndex = -1;
						this.getScanInProgress = false;
						Action<int, bool> onFetchPrivateScanComplete3 = this.OnFetchPrivateScanComplete;
						if (onFetchPrivateScanComplete3 != null)
						{
							onFetchPrivateScanComplete3(slot, false);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("SharedBlocksManager RequestFetchPrivateScan exception " + ex.Message);
					this.currentGetScanIndex = -1;
					this.getScanInProgress = false;
					Action<int, bool> onFetchPrivateScanComplete4 = this.OnFetchPrivateScanComplete;
					if (onFetchPrivateScanComplete4 != null)
					{
						onFetchPrivateScanComplete4(slot, false);
					}
				}
				return;
			}
		}

		// Token: 0x060048E3 RID: 18659 RVA: 0x0015CC70 File Offset: 0x0015AE70
		private void OnGetMothershipPrivateScanSuccess(MothershipUserData response)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetMothershipPrivateScanSuccess", null);
			bool flag = response != null && response.value != null && response.value.Length > 0;
			int num = this.currentGetScanIndex;
			if (response != null)
			{
				this.privateScanDataCache[this.currentGetScanIndex] = response.value;
				this.hasPulledPrivateScanMothership[this.currentGetScanIndex] = true;
				if (flag)
				{
					SharedBlocksManager.LocalPublishInfo publishInfoForSlot = SharedBlocksManager.GetPublishInfoForSlot(this.currentSaveScanIndex);
					if (publishInfoForSlot.mapID != null)
					{
						SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
						{
							MapID = publishInfoForSlot.mapID,
							MapData = this.privateScanDataCache[this.currentSaveScanIndex],
							CreatorNickName = GorillaTagger.Instance.offlineVRRig.playerNameVisible,
							UpdateTime = DateTime.Now
						};
						this.AddMapToResponseCache(sharedBlocksMap);
					}
					this.currentGetScanIndex = -1;
					this.getScanInProgress = false;
					Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
					if (onFetchPrivateScanComplete != null)
					{
						onFetchPrivateScanComplete(num, true);
					}
				}
				else
				{
					this.FetchBuildFromPlayfab();
				}
			}
			else
			{
				this.currentGetScanIndex = -1;
				this.getScanInProgress = false;
				Action<int, bool> onFetchPrivateScanComplete2 = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete2 != null)
				{
					onFetchPrivateScanComplete2(num, false);
				}
			}
			if (response != null)
			{
				response.Dispose();
			}
		}

		// Token: 0x060048E4 RID: 18660 RVA: 0x0015CD90 File Offset: 0x0015AF90
		private void OnGetMothershipPrivateScanFail(MothershipError error, int status)
		{
			string text = ((error == null) ? status.ToString() : error.Message);
			GTDev.LogError<string>("SharedBlocksManager OnGetMothershipPrivateScanFail: " + text, null);
			int num = this.currentGetScanIndex;
			if (this.currentGetScanIndex >= 0 && this.currentGetScanIndex < BuilderScanKiosk.NUM_SAVE_SLOTS)
			{
				this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
				this.hasPulledPrivateScanMothership[this.currentGetScanIndex] = true;
			}
			this.getScanInProgress = false;
			this.currentGetScanIndex = -1;
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete(num, false);
			}
			if (error != null)
			{
				error.Dispose();
			}
		}

		// Token: 0x04004B70 RID: 19312
		public static SharedBlocksManager instance;

		// Token: 0x04004B7A RID: 19322
		[SerializeField]
		private BuilderTableSerializationConfig serializationConfig;

		// Token: 0x04004B7B RID: 19323
		private int maxRetriesOnFail = 3;

		// Token: 0x04004B7C RID: 19324
		public const int MAP_ID_LENGTH = 8;

		// Token: 0x04004B7D RID: 19325
		private const string MAP_ID_PATTERN = "^[CFGHKMNPRTWXZ256789]+$";

		// Token: 0x04004B7E RID: 19326
		public const float MINIMUM_REFRESH_DELAY = 300f;

		// Token: 0x04004B7F RID: 19327
		public const int VOTE_HISTORY_LENGTH = 10;

		// Token: 0x04004B80 RID: 19328
		private const int NUM_CACHED_MAP_RESULTS = 5;

		// Token: 0x04004B81 RID: 19329
		private SharedBlocksManager.StartingMapConfig startingMapConfig = new SharedBlocksManager.StartingMapConfig
		{
			rangeMax = 10,
			sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
			useMapID = false,
			mapID = null
		};

		// Token: 0x04004B82 RID: 19330
		private bool hasQueriedSaveTime;

		// Token: 0x04004B83 RID: 19331
		private static List<string> saveDateKeys = new List<string>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x04004B84 RID: 19332
		private bool fetchedTableConfig;

		// Token: 0x04004B85 RID: 19333
		private int fetchTableConfigRetryCount;

		// Token: 0x04004B86 RID: 19334
		private string tableConfigResponse;

		// Token: 0x04004B87 RID: 19335
		private bool fetchedStartingMapConfig;

		// Token: 0x04004B88 RID: 19336
		private int fetchStartingMapConfigRetryCount;

		// Token: 0x04004B89 RID: 19337
		private bool fetchTitleDataBuildInProgress;

		// Token: 0x04004B8A RID: 19338
		private bool fetchTitleDataBuildComplete;

		// Token: 0x04004B8B RID: 19339
		private int fetchTitleDataRetryCount;

		// Token: 0x04004B8C RID: 19340
		private string titleDataBuildCache = string.Empty;

		// Token: 0x04004B8D RID: 19341
		private bool[] hasPulledPrivateScanPlayfab = new bool[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x04004B8E RID: 19342
		private int fetchPlayfabBuildsRetryCount;

		// Token: 0x04004B8F RID: 19343
		private readonly int publicSlotIndex = BuilderScanKiosk.NUM_SAVE_SLOTS;

		// Token: 0x04004B90 RID: 19344
		private string[] privateScanDataCache = new string[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x04004B91 RID: 19345
		private bool[] hasPulledPrivateScanMothership = new bool[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x04004B92 RID: 19346
		private bool saveScanInProgress;

		// Token: 0x04004B93 RID: 19347
		private int currentSaveScanIndex = -1;

		// Token: 0x04004B94 RID: 19348
		private string currentSaveScanData = string.Empty;

		// Token: 0x04004B95 RID: 19349
		private bool getScanInProgress;

		// Token: 0x04004B96 RID: 19350
		private int currentGetScanIndex = -1;

		// Token: 0x04004B97 RID: 19351
		private int voteRetryCount;

		// Token: 0x04004B98 RID: 19352
		private bool voteInProgress;

		// Token: 0x04004B99 RID: 19353
		private bool publishRequestInProgress;

		// Token: 0x04004B9A RID: 19354
		private int postPublishMapRetryCount;

		// Token: 0x04004B9B RID: 19355
		private bool getMapDataFromIDInProgress;

		// Token: 0x04004B9C RID: 19356
		private int getMapDataFromIDRetryCount;

		// Token: 0x04004B9D RID: 19357
		private bool getTopMapsInProgress;

		// Token: 0x04004B9E RID: 19358
		private int getTopMapsRetryCount;

		// Token: 0x04004B9F RID: 19359
		private bool hasCachedTopMaps;

		// Token: 0x04004BA0 RID: 19360
		private double lastGetTopMapsTime = double.MinValue;

		// Token: 0x04004BA1 RID: 19361
		private bool updateMapActiveInProgress;

		// Token: 0x04004BA2 RID: 19362
		private int updateMapActiveRetryCount;

		// Token: 0x04004BA3 RID: 19363
		private List<SharedBlocksManager.SharedBlocksMap> latestPopularMaps = new List<SharedBlocksManager.SharedBlocksMap>();

		// Token: 0x04004BA4 RID: 19364
		private static LinkedList<string> recentUpVotes = new LinkedList<string>();

		// Token: 0x04004BA5 RID: 19365
		private static Dictionary<int, SharedBlocksManager.LocalPublishInfo> localPublishData = new Dictionary<int, SharedBlocksManager.LocalPublishInfo>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x04004BA6 RID: 19366
		private static List<string> localMapIds = new List<string>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x04004BA7 RID: 19367
		private List<SharedBlocksManager.SharedBlocksMap> mapResponseCache = new List<SharedBlocksManager.SharedBlocksMap>(5);

		// Token: 0x04004BA8 RID: 19368
		private SharedBlocksManager.SharedBlocksMap defaultMap;

		// Token: 0x04004BA9 RID: 19369
		private bool hasDefaultMap;

		// Token: 0x04004BAA RID: 19370
		private double defaultMapCacheTime = double.MinValue;

		// Token: 0x04004BAB RID: 19371
		private bool getDefaultMapInProgress;

		// Token: 0x02000B74 RID: 2932
		[Serializable]
		public class SharedBlocksMap
		{
			// Token: 0x170006F6 RID: 1782
			// (get) Token: 0x060048E7 RID: 18663 RVA: 0x0015CF4D File Offset: 0x0015B14D
			// (set) Token: 0x060048E8 RID: 18664 RVA: 0x0015CF55 File Offset: 0x0015B155
			public string MapID { get; set; }

			// Token: 0x170006F7 RID: 1783
			// (get) Token: 0x060048E9 RID: 18665 RVA: 0x0015CF5E File Offset: 0x0015B15E
			// (set) Token: 0x060048EA RID: 18666 RVA: 0x0015CF66 File Offset: 0x0015B166
			public string CreatorID { get; set; }

			// Token: 0x170006F8 RID: 1784
			// (get) Token: 0x060048EB RID: 18667 RVA: 0x0015CF6F File Offset: 0x0015B16F
			// (set) Token: 0x060048EC RID: 18668 RVA: 0x0015CF77 File Offset: 0x0015B177
			public string CreatorNickName { get; set; }

			// Token: 0x170006F9 RID: 1785
			// (get) Token: 0x060048ED RID: 18669 RVA: 0x0015CF80 File Offset: 0x0015B180
			// (set) Token: 0x060048EE RID: 18670 RVA: 0x0015CF88 File Offset: 0x0015B188
			public DateTime CreateTime { get; set; }

			// Token: 0x170006FA RID: 1786
			// (get) Token: 0x060048EF RID: 18671 RVA: 0x0015CF91 File Offset: 0x0015B191
			// (set) Token: 0x060048F0 RID: 18672 RVA: 0x0015CF99 File Offset: 0x0015B199
			public DateTime UpdateTime { get; set; }

			// Token: 0x170006FB RID: 1787
			// (get) Token: 0x060048F1 RID: 18673 RVA: 0x0015CFA2 File Offset: 0x0015B1A2
			// (set) Token: 0x060048F2 RID: 18674 RVA: 0x0015CFAA File Offset: 0x0015B1AA
			public string MapData { get; set; }
		}

		// Token: 0x02000B75 RID: 2933
		[Serializable]
		public struct LocalPublishInfo
		{
			// Token: 0x04004BB2 RID: 19378
			public string mapID;

			// Token: 0x04004BB3 RID: 19379
			public long publishTime;
		}

		// Token: 0x02000B76 RID: 2934
		[Serializable]
		private class VoteRequest
		{
			// Token: 0x04004BB4 RID: 19380
			public string mothershipId;

			// Token: 0x04004BB5 RID: 19381
			public string mothershipToken;

			// Token: 0x04004BB6 RID: 19382
			public string mapId;

			// Token: 0x04004BB7 RID: 19383
			public int vote;
		}

		// Token: 0x02000B77 RID: 2935
		[Serializable]
		private class PublishMapRequestData
		{
			// Token: 0x04004BB8 RID: 19384
			public string mothershipId;

			// Token: 0x04004BB9 RID: 19385
			public string mothershipToken;

			// Token: 0x04004BBA RID: 19386
			public string userdataMetadataKey;

			// Token: 0x04004BBB RID: 19387
			public string playerNickname;
		}

		// Token: 0x02000B78 RID: 2936
		public enum MapSortMethod
		{
			// Token: 0x04004BBD RID: 19389
			Top,
			// Token: 0x04004BBE RID: 19390
			NewlyCreated,
			// Token: 0x04004BBF RID: 19391
			RecentlyUpdated
		}

		// Token: 0x02000B79 RID: 2937
		public struct StartingMapConfig
		{
			// Token: 0x04004BC0 RID: 19392
			public int rangeMax;

			// Token: 0x04004BC1 RID: 19393
			public string sortMethod;

			// Token: 0x04004BC2 RID: 19394
			public bool useMapID;

			// Token: 0x04004BC3 RID: 19395
			public string mapID;
		}

		// Token: 0x02000B7A RID: 2938
		[Serializable]
		private class GetMapsRequest
		{
			// Token: 0x04004BC4 RID: 19396
			public string mothershipId;

			// Token: 0x04004BC5 RID: 19397
			public string mothershipToken;

			// Token: 0x04004BC6 RID: 19398
			public int page;

			// Token: 0x04004BC7 RID: 19399
			public int pageSize;

			// Token: 0x04004BC8 RID: 19400
			public string sort;

			// Token: 0x04004BC9 RID: 19401
			public bool ShowInactive;
		}

		// Token: 0x02000B7B RID: 2939
		[Serializable]
		private class GetMapDataFromIDRequest
		{
			// Token: 0x04004BCA RID: 19402
			public string mothershipId;

			// Token: 0x04004BCB RID: 19403
			public string mothershipToken;

			// Token: 0x04004BCC RID: 19404
			public string mapId;
		}

		// Token: 0x02000B7C RID: 2940
		[Serializable]
		private class GetMapIDFromPlayerRequest
		{
			// Token: 0x04004BCD RID: 19405
			public string mothershipId;

			// Token: 0x04004BCE RID: 19406
			public string mothershipToken;

			// Token: 0x04004BCF RID: 19407
			public string requestId;

			// Token: 0x04004BD0 RID: 19408
			public string requestUserDataMetaKey;
		}

		// Token: 0x02000B7D RID: 2941
		[Serializable]
		private class GetMapIDFromPlayerResponse
		{
			// Token: 0x04004BD1 RID: 19409
			public SharedBlocksManager.SharedBlocksMapMetaData result;

			// Token: 0x04004BD2 RID: 19410
			public int statusCode;

			// Token: 0x04004BD3 RID: 19411
			public string error;
		}

		// Token: 0x02000B7E RID: 2942
		[Serializable]
		private class SharedBlocksMapMetaData
		{
			// Token: 0x04004BD4 RID: 19412
			public string mapId;

			// Token: 0x04004BD5 RID: 19413
			public string mothershipId;

			// Token: 0x04004BD6 RID: 19414
			public string userDataMetadataKey;

			// Token: 0x04004BD7 RID: 19415
			public string nickname;

			// Token: 0x04004BD8 RID: 19416
			public string createdTime;

			// Token: 0x04004BD9 RID: 19417
			public string updatedTime;

			// Token: 0x04004BDA RID: 19418
			public int voteCount;

			// Token: 0x04004BDB RID: 19419
			public bool isActive;
		}

		// Token: 0x02000B7F RID: 2943
		[Serializable]
		private struct GetMapDataFromPlayerRequestData
		{
			// Token: 0x04004BDC RID: 19420
			public string CreatorID;

			// Token: 0x04004BDD RID: 19421
			public string MapScan;

			// Token: 0x04004BDE RID: 19422
			public SharedBlocksManager.BlocksMapRequestCallback Callback;
		}

		// Token: 0x02000B80 RID: 2944
		[Serializable]
		private class UpdateMapActiveRequest
		{
			// Token: 0x04004BDF RID: 19423
			public string mothershipId;

			// Token: 0x04004BE0 RID: 19424
			public string mothershipToken;

			// Token: 0x04004BE1 RID: 19425
			public string userdataMetadataKey;

			// Token: 0x04004BE2 RID: 19426
			public bool setActive;
		}

		// Token: 0x02000B81 RID: 2945
		// (Invoke) Token: 0x060048FD RID: 18685
		public delegate void PublishMapRequestCallback(bool success, string key, string mapID, long responseCode);

		// Token: 0x02000B82 RID: 2946
		// (Invoke) Token: 0x06004901 RID: 18689
		public delegate void BlocksMapRequestCallback(SharedBlocksManager.SharedBlocksMap response);
	}
}
