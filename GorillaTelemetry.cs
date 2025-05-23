using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GorillaNetworking;
using KID.Model;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.EventsModels;
using UnityEngine;

// Token: 0x02000643 RID: 1603
public static class GorillaTelemetry
{
	// Token: 0x06002821 RID: 10273 RVA: 0x000C8200 File Offset: 0x000C6400
	static GorillaTelemetry()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["User"] = null;
		dictionary["EventType"] = null;
		dictionary["ZoneId"] = null;
		dictionary["SubZoneId"] = null;
		GorillaTelemetry.gZoneEventArgs = dictionary;
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		dictionary2["User"] = null;
		dictionary2["EventType"] = null;
		GorillaTelemetry.gNotifEventArgs = dictionary2;
		GorillaTelemetry.CurrentZone = GTZone.none;
		GorillaTelemetry.CurrentSubZone = GTSubZone.none;
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		dictionary3["User"] = null;
		dictionary3["EventType"] = null;
		dictionary3["Items"] = null;
		GorillaTelemetry.gShopEventArgs = dictionary3;
		GorillaTelemetry.gSingleItemParam = new CosmeticsController.CosmeticItem[1];
		GorillaTelemetry.gSingleItemBuilderParam = new BuilderSetManager.BuilderSetStoreItem[1];
		Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
		dictionary4["User"] = null;
		dictionary4["EventType"] = null;
		dictionary4["AgeCategory"] = null;
		dictionary4["VoiceChatEnabled"] = null;
		dictionary4["CustomUsernameEnabled"] = null;
		dictionary4["JoinGroups"] = null;
		GorillaTelemetry.gKidEventArgs = dictionary4;
		Dictionary<string, object> dictionary5 = new Dictionary<string, object>();
		dictionary5["User"] = null;
		dictionary5["WamGameId"] = null;
		dictionary5["WamMachineId"] = null;
		GorillaTelemetry.gWamGameStartArgs = dictionary5;
		Dictionary<string, object> dictionary6 = new Dictionary<string, object>();
		dictionary6["User"] = null;
		dictionary6["WamGameId"] = null;
		dictionary6["WamMachineId"] = null;
		dictionary6["WamMLevelNumber"] = null;
		dictionary6["WamGoodMolesShown"] = null;
		dictionary6["WamHazardMolesShown"] = null;
		dictionary6["WamLevelMinScore"] = null;
		dictionary6["WamLevelScore"] = null;
		dictionary6["WamHazardMolesHit"] = null;
		dictionary6["WamGameState"] = null;
		GorillaTelemetry.gWamLevelEndArgs = dictionary6;
		Dictionary<string, object> dictionary7 = new Dictionary<string, object>();
		dictionary7["CustomMapName"] = null;
		dictionary7["CustomMapModId"] = null;
		dictionary7["LowestFPS"] = null;
		dictionary7["LowestFPSDrawCalls"] = null;
		dictionary7["LowestFPSPlayerCount"] = null;
		dictionary7["AverageFPS"] = null;
		dictionary7["AverageDrawCalls"] = null;
		dictionary7["AveragePlayerCount"] = null;
		dictionary7["HighestFPS"] = null;
		dictionary7["HighestFPSDrawCalls"] = null;
		dictionary7["HighestFPSPlayerCount"] = null;
		dictionary7["PlaytimeInSeconds"] = null;
		GorillaTelemetry.gCustomMapPerfArgs = dictionary7;
		Dictionary<string, object> dictionary8 = new Dictionary<string, object>();
		dictionary8["User"] = null;
		dictionary8["CustomMapName"] = null;
		dictionary8["CustomMapModId"] = null;
		dictionary8["CustomMapCreator"] = null;
		dictionary8["MinPlayerCount"] = null;
		dictionary8["MaxPlayerCount"] = null;
		dictionary8["PlaytimeOnMap"] = null;
		dictionary8["PrivateRoom"] = null;
		GorillaTelemetry.gCustomMapTrackingMetrics = dictionary8;
		Dictionary<string, object> dictionary9 = new Dictionary<string, object>();
		dictionary9["User"] = null;
		dictionary9["CustomMapName"] = null;
		dictionary9["CustomMapModId"] = null;
		dictionary9["CustomMapCreator"] = null;
		GorillaTelemetry.gCustomMapDownloadMetrics = dictionary9;
		GameObject gameObject = new GameObject("GorillaTelemetryBatcher");
		Object.DontDestroyOnLoad(gameObject);
		gameObject.AddComponent<GorillaTelemetry.BatchRunner>();
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x000C8551 File Offset: 0x000C6751
	private static void QueueTelemetryEvent(EventContents eventContent)
	{
		GorillaTelemetry.telemetryEventsQueue.Enqueue(eventContent);
	}

	// Token: 0x06002823 RID: 10275 RVA: 0x000C8560 File Offset: 0x000C6760
	private static void FlushTelemetry()
	{
		int count = GorillaTelemetry.telemetryEventsQueue.Count;
		if (count == 0)
		{
			return;
		}
		EventContents[] array = ArrayPool<EventContents>.Shared.Rent(count);
		try
		{
			int i;
			for (i = 0; i < count; i++)
			{
				EventContents eventContents;
				array[i] = (GorillaTelemetry.telemetryEventsQueue.TryDequeue(out eventContents) ? eventContents : null);
			}
			if (i == 0)
			{
				ArrayPool<EventContents>.Shared.Return(array, false);
			}
			else
			{
				WriteEventsRequest writeEventsRequest = new WriteEventsRequest();
				writeEventsRequest.Events = GorillaTelemetry.GetEventListForArray(array, i);
				PlayFabEventsAPI.WriteTelemetryEvents(writeEventsRequest, delegate(WriteEventsResponse result)
				{
				}, delegate(PlayFabError error)
				{
				}, null, null);
			}
		}
		finally
		{
			ArrayPool<EventContents>.Shared.Return(array, false);
		}
	}

	// Token: 0x06002824 RID: 10276 RVA: 0x000C8630 File Offset: 0x000C6830
	private static List<EventContents> GetEventListForArray(EventContents[] array, int count)
	{
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			if (array[i] != null)
			{
				num++;
			}
		}
		List<EventContents> list;
		if (!GorillaTelemetry.gListPool.TryGetValue(num, out list))
		{
			list = new List<EventContents>(num);
			GorillaTelemetry.gListPool.TryAdd(num, list);
		}
		else
		{
			list.Clear();
		}
		for (int j = 0; j < count; j++)
		{
			if (array[j] != null)
			{
				list.Add(array[j]);
			}
		}
		return list;
	}

	// Token: 0x06002825 RID: 10277 RVA: 0x000C869A File Offset: 0x000C689A
	private static bool IsConnected()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return false;
		}
		if (GorillaTelemetry.gPlayFabAuth == null)
		{
			GorillaTelemetry.gPlayFabAuth = PlayFabAuthenticator.instance;
		}
		return !(GorillaTelemetry.gPlayFabAuth == null);
	}

	// Token: 0x06002826 RID: 10278 RVA: 0x000C86CD File Offset: 0x000C68CD
	private static string PlayFabUserId()
	{
		return GorillaTelemetry.gPlayFabAuth.GetPlayFabPlayerId();
	}

	// Token: 0x06002827 RID: 10279 RVA: 0x000C86DC File Offset: 0x000C68DC
	public static void PostZoneEvent(GTZone zone, GTSubZone subZone, GTZoneEventType zoneEvent)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = zoneEvent.GetName<GTZoneEventType>();
		string name2 = zone.GetName<GTZone>();
		string name3 = subZone.GetName<GTSubZone>();
		Dictionary<string, object> dictionary = GorillaTelemetry.gZoneEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["ZoneId"] = name2;
		dictionary["SubZoneId"] = name3;
		GorillaTelemetry.QueueTelemetryEvent(new EventContents
		{
			Name = "telemetry_zone_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
	}

	// Token: 0x06002828 RID: 10280 RVA: 0x000C876F File Offset: 0x000C696F
	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, CosmeticsController.CosmeticItem item)
	{
		GorillaTelemetry.gSingleItemParam[0] = item;
		GorillaTelemetry.PostShopEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemParam);
		GorillaTelemetry.gSingleItemParam[0] = default(CosmeticsController.CosmeticItem);
	}

	// Token: 0x06002829 RID: 10281 RVA: 0x000C879C File Offset: 0x000C699C
	private static string[] FetchItemArgs(IList<CosmeticsController.CosmeticItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			CosmeticsController.CosmeticItem cosmeticItem = items[i];
			if (!cosmeticItem.isNullItem)
			{
				string itemName = cosmeticItem.itemName;
				if (!string.IsNullOrWhiteSpace(itemName) && !itemName.Contains("NOTHING", StringComparison.InvariantCultureIgnoreCase) && hashSet.Add(itemName))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	// Token: 0x0600282A RID: 10282 RVA: 0x000C8828 File Offset: 0x000C6A28
	public static void PostShopEvent(VRRig playerRig, GTShopEventType shopEvent, IList<CosmeticsController.CosmeticItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!playerRig.isLocal)
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] array = GorillaTelemetry.FetchItemArgs(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["Items"] = array;
		PlayFabClientAPI.WriteTitleEvent(new WriteTitleEventRequest
		{
			EventName = "telemetry_shop_event",
			Body = dictionary
		}, new Action<WriteEventResponse>(GorillaTelemetry.PostShopEvent_OnResult), new Action<PlayFabError>(GorillaTelemetry.PostShopEvent_OnError), null, null);
	}

	// Token: 0x0600282B RID: 10283 RVA: 0x000023F4 File Offset: 0x000005F4
	private static void PostShopEvent_OnResult(WriteEventResponse result)
	{
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x000023F4 File Offset: 0x000005F4
	private static void PostShopEvent_OnError(PlayFabError error)
	{
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x000C88BA File Offset: 0x000C6ABA
	public static void PostBuilderKioskEvent(VRRig playerRig, GTShopEventType shopEvent, BuilderSetManager.BuilderSetStoreItem item)
	{
		GorillaTelemetry.gSingleItemBuilderParam[0] = item;
		GorillaTelemetry.PostBuilderKioskEvent(playerRig, shopEvent, GorillaTelemetry.gSingleItemBuilderParam);
		GorillaTelemetry.gSingleItemBuilderParam[0] = default(BuilderSetManager.BuilderSetStoreItem);
	}

	// Token: 0x0600282E RID: 10286 RVA: 0x000C88E8 File Offset: 0x000C6AE8
	private static string[] BuilderItemsToStrings(IList<BuilderSetManager.BuilderSetStoreItem> items)
	{
		int count = items.Count;
		if (count == 0)
		{
			return Array.Empty<string>();
		}
		HashSet<string> hashSet = new HashSet<string>(count);
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem = items[i];
			if (!builderSetStoreItem.isNullItem)
			{
				string playfabID = builderSetStoreItem.playfabID;
				if (!string.IsNullOrWhiteSpace(playfabID) && !playfabID.Contains("NOTHING", StringComparison.InvariantCultureIgnoreCase) && hashSet.Add(playfabID))
				{
					num++;
				}
			}
		}
		string[] array = new string[num];
		hashSet.CopyTo(array);
		return array;
	}

	// Token: 0x0600282F RID: 10287 RVA: 0x000C8974 File Offset: 0x000C6B74
	public static void PostBuilderKioskEvent(VRRig playerRig, GTShopEventType shopEvent, IList<BuilderSetManager.BuilderSetStoreItem> items)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		if (!playerRig.isLocal)
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = shopEvent.GetName<GTShopEventType>();
		string[] array = GorillaTelemetry.BuilderItemsToStrings(items);
		Dictionary<string, object> dictionary = GorillaTelemetry.gShopEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["Items"] = array;
		PlayFabClientAPI.WriteTitleEvent(new WriteTitleEventRequest
		{
			EventName = "telemetry_shop_event",
			Body = dictionary
		}, new Action<WriteEventResponse>(GorillaTelemetry.PostShopEvent_OnResult), new Action<PlayFabError>(GorillaTelemetry.PostShopEvent_OnError), null, null);
	}

	// Token: 0x06002830 RID: 10288 RVA: 0x000C8A08 File Offset: 0x000C6C08
	public static void PostKidEvent(bool joinGroupsEnabled, bool voiceChatEnabled, bool customUsernamesEnabled, AgeStatusType ageCategory, GTKidEventType kidEvent)
	{
		if ((double)Random.value < 0.1)
		{
			return;
		}
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		string name = kidEvent.GetName<GTKidEventType>();
		string text2 = ((ageCategory == AgeStatusType.LEGALADULT) ? "Not_Managed_Account" : "Managed_Account");
		string text3 = joinGroupsEnabled.ToString().ToUpper();
		string text4 = voiceChatEnabled.ToString().ToUpper();
		string text5 = customUsernamesEnabled.ToString().ToUpper();
		Dictionary<string, object> dictionary = GorillaTelemetry.gKidEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = name;
		dictionary["AgeCategory"] = text2;
		dictionary["VoiceChatEnabled"] = text4;
		dictionary["CustomUsernameEnabled"] = text5;
		dictionary["JoinGroups"] = text3;
		GorillaTelemetry.QueueTelemetryEvent(new EventContents
		{
			Name = "telemetry_kid_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
	}

	// Token: 0x06002831 RID: 10289 RVA: 0x000C8AF8 File Offset: 0x000C6CF8
	public static void WamGameStart(string playerId, string gameId, string machineId)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gWamGameStartArgs["User"] = playerId;
		GorillaTelemetry.gWamGameStartArgs["WamGameId"] = gameId;
		GorillaTelemetry.gWamGameStartArgs["WamMachineId"] = machineId;
		GorillaTelemetry.QueueTelemetryEvent(new EventContents
		{
			Name = "telemetry_wam_gameStartEvent",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gWamGameStartArgs
		});
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x000C8B68 File Offset: 0x000C6D68
	public static void WamLevelEnd(string playerId, int gameId, string machineId, int currentLevelNumber, int levelGoodMolesShown, int levelHazardMolesShown, int levelMinScore, int currentScore, int levelHazardMolesHit, string currentGameResult)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		GorillaTelemetry.gWamLevelEndArgs["User"] = playerId;
		GorillaTelemetry.gWamLevelEndArgs["WamGameId"] = gameId.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamMachineId"] = machineId;
		GorillaTelemetry.gWamLevelEndArgs["WamMLevelNumber"] = currentLevelNumber.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamGoodMolesShown"] = levelGoodMolesShown.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamHazardMolesShown"] = levelHazardMolesShown.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamLevelMinScore"] = levelMinScore.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamLevelScore"] = currentScore.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamHazardMolesHit"] = levelHazardMolesHit.ToString();
		GorillaTelemetry.gWamLevelEndArgs["WamGameState"] = currentGameResult;
		GorillaTelemetry.QueueTelemetryEvent(new EventContents
		{
			Name = "telemetry_wam_levelEndEvent",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = GorillaTelemetry.gWamLevelEndArgs
		});
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x000C8C74 File Offset: 0x000C6E74
	public static void PostCustomMapPerformance(string mapName, long mapModId, int lowestFPS, int lowestDC, int lowestPC, int avgFPS, int avgDC, int avgPC, int highestFPS, int highestDC, int highestPC, int playtime)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapPerfArgs;
		dictionary["CustomMapName"] = mapName;
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["LowestFPS"] = lowestFPS.ToString();
		dictionary["LowestFPSDrawCalls"] = lowestDC.ToString();
		dictionary["LowestFPSPlayerCount"] = lowestPC.ToString();
		dictionary["AverageFPS"] = avgFPS.ToString();
		dictionary["AverageDrawCalls"] = avgDC.ToString();
		dictionary["AveragePlayerCount"] = avgPC.ToString();
		dictionary["HighestFPS"] = highestFPS.ToString();
		dictionary["HighestFPSDrawCalls"] = highestDC.ToString();
		dictionary["HighestFPSPlayerCount"] = highestPC.ToString();
		dictionary["PlaytimeInSeconds"] = playtime.ToString();
		GorillaTelemetry.QueueTelemetryEvent(new EventContents
		{
			Name = "CustomMapPerformance",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
	}

	// Token: 0x06002834 RID: 10292 RVA: 0x000C8D88 File Offset: 0x000C6F88
	public static void PostCustomMapTracking(string mapName, long mapModId, string mapCreatorUsername, int minPlayers, int maxPlayers, int playtime, bool privateRoom)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		int num = playtime % 60;
		int num2 = (playtime - num) / 60;
		int num3 = num2 % 60;
		int num4 = (num2 - num3) / 60;
		string text = string.Format("{0}.{1}.{2}", num4, num3, num);
		Dictionary<string, object> dictionary = GorillaTelemetry.gCustomMapTrackingMetrics;
		dictionary["User"] = GorillaTelemetry.PlayFabUserId();
		dictionary["CustomMapName"] = mapName;
		dictionary["CustomMapModId"] = mapModId.ToString();
		dictionary["CustomMapCreator"] = mapCreatorUsername;
		dictionary["MinPlayerCount"] = minPlayers.ToString();
		dictionary["MaxPlayerCount"] = maxPlayers.ToString();
		dictionary["PlaytimeInSeconds"] = playtime.ToString();
		dictionary["PrivateRoom"] = privateRoom.ToString();
		dictionary["PlaytimeOnMap"] = text;
		GorillaTelemetry.QueueTelemetryEvent(new EventContents
		{
			Name = "CustomMapTracking",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
	}

	// Token: 0x06002835 RID: 10293 RVA: 0x000023F4 File Offset: 0x000005F4
	public static void PostCustomMapDownloadEvent(string mapName, long mapModId, string mapCreatorUsername)
	{
	}

	// Token: 0x06002836 RID: 10294 RVA: 0x000C8E98 File Offset: 0x000C7098
	public static void SendMothershipAnalytics(KIDTelemetryData data)
	{
		if (string.IsNullOrEmpty(data.EventName))
		{
			Debug.LogError("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Event Name is null or empty");
			return;
		}
		if (data.BodyData == null || data.BodyData.Count == 0)
		{
			Debug.LogError("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Body Data KVPs are null or empty - must have at least 1");
			return;
		}
		string text = string.Empty;
		if (data.CustomTags != null && data.CustomTags.Length != 0)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int j = 0; j < data.CustomTags.Length; j++)
			{
				dictionary.Add(string.Format("tag{0}", j + 1), data.CustomTags[j]);
			}
			text = JsonConvert.SerializeObject(dictionary);
		}
		string text2 = JsonConvert.SerializeObject(data.BodyData);
		MothershipWriteEventsRequest mothershipWriteEventsRequest = new MothershipWriteEventsRequest
		{
			title_id = MothershipClientApiUnity.TitleId,
			deployment_id = MothershipClientApiUnity.DeploymentId,
			env_id = MothershipClientApiUnity.EnvironmentId,
			events = new AnalyticsRequestVector(new List<MothershipAnalyticsEvent>
			{
				new MothershipAnalyticsEvent
				{
					event_timestamp = DateTime.UtcNow.ToString("O"),
					event_name = data.EventName,
					custom_tags = text,
					body = text2
				}
			})
		};
		MothershipClientApiUnity.WriteEvents(MothershipClientContext.MothershipId, mothershipWriteEventsRequest, delegate(MothershipWriteEventsResponse resp)
		{
			Debug.Log("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Successfully submitted analytics for event: [" + data.EventName + "]");
		}, delegate(MothershipError err, int i)
		{
			Debug.Log("[GORILLA_TELEMETRY::MOTHERSHIP_ANALYTICS] Failed to submit analytics for event: [" + data.EventName + "], with error:\n" + err.Message);
		});
	}

	// Token: 0x06002837 RID: 10295 RVA: 0x000C901C File Offset: 0x000C721C
	public static void PostNotificationEvent(string notificationType)
	{
		if (!GorillaTelemetry.IsConnected())
		{
			return;
		}
		string text = GorillaTelemetry.PlayFabUserId();
		Dictionary<string, object> dictionary = GorillaTelemetry.gNotifEventArgs;
		dictionary["User"] = text;
		dictionary["EventType"] = notificationType;
		GorillaTelemetry.QueueTelemetryEvent(new EventContents
		{
			Name = "telemetry_ggwp_event",
			EventNamespace = GorillaTelemetry.EVENT_NAMESPACE,
			Payload = dictionary
		});
	}

	// Token: 0x04002CE4 RID: 11492
	private static readonly float TELEMETRY_FLUSH_SEC = 10f;

	// Token: 0x04002CE5 RID: 11493
	private static readonly ConcurrentQueue<EventContents> telemetryEventsQueue = new ConcurrentQueue<EventContents>();

	// Token: 0x04002CE6 RID: 11494
	private static readonly Dictionary<int, List<EventContents>> gListPool = new Dictionary<int, List<EventContents>>();

	// Token: 0x04002CE7 RID: 11495
	private static readonly string namespacePrefix = "custom";

	// Token: 0x04002CE8 RID: 11496
	private static readonly string EVENT_NAMESPACE = GorillaTelemetry.namespacePrefix + "." + PlayFabAuthenticatorSettings.TitleId;

	// Token: 0x04002CE9 RID: 11497
	private static PlayFabAuthenticator gPlayFabAuth;

	// Token: 0x04002CEA RID: 11498
	private static readonly Dictionary<string, object> gZoneEventArgs;

	// Token: 0x04002CEB RID: 11499
	private static readonly Dictionary<string, object> gNotifEventArgs;

	// Token: 0x04002CEC RID: 11500
	public static GTZone CurrentZone;

	// Token: 0x04002CED RID: 11501
	public static GTSubZone CurrentSubZone;

	// Token: 0x04002CEE RID: 11502
	private static readonly Dictionary<string, object> gShopEventArgs;

	// Token: 0x04002CEF RID: 11503
	private static CosmeticsController.CosmeticItem[] gSingleItemParam;

	// Token: 0x04002CF0 RID: 11504
	private static BuilderSetManager.BuilderSetStoreItem[] gSingleItemBuilderParam;

	// Token: 0x04002CF1 RID: 11505
	private static Dictionary<string, object> gKidEventArgs;

	// Token: 0x04002CF2 RID: 11506
	private static readonly Dictionary<string, object> gWamGameStartArgs;

	// Token: 0x04002CF3 RID: 11507
	private static readonly Dictionary<string, object> gWamLevelEndArgs;

	// Token: 0x04002CF4 RID: 11508
	private static Dictionary<string, object> gCustomMapPerfArgs;

	// Token: 0x04002CF5 RID: 11509
	private static Dictionary<string, object> gCustomMapTrackingMetrics;

	// Token: 0x04002CF6 RID: 11510
	private static Dictionary<string, object> gCustomMapDownloadMetrics;

	// Token: 0x02000644 RID: 1604
	public static class k
	{
		// Token: 0x04002CF7 RID: 11511
		public const string User = "User";

		// Token: 0x04002CF8 RID: 11512
		public const string ZoneId = "ZoneId";

		// Token: 0x04002CF9 RID: 11513
		public const string SubZoneId = "SubZoneId";

		// Token: 0x04002CFA RID: 11514
		public const string EventType = "EventType";

		// Token: 0x04002CFB RID: 11515
		public const string Items = "Items";

		// Token: 0x04002CFC RID: 11516
		public const string VoiceChatEnabled = "VoiceChatEnabled";

		// Token: 0x04002CFD RID: 11517
		public const string JoinGroups = "JoinGroups";

		// Token: 0x04002CFE RID: 11518
		public const string CustomUsernameEnabled = "CustomUsernameEnabled";

		// Token: 0x04002CFF RID: 11519
		public const string AgeCategory = "AgeCategory";

		// Token: 0x04002D00 RID: 11520
		public const string telemetry_zone_event = "telemetry_zone_event";

		// Token: 0x04002D01 RID: 11521
		public const string telemetry_shop_event = "telemetry_shop_event";

		// Token: 0x04002D02 RID: 11522
		public const string telemetry_kid_event = "telemetry_kid_event";

		// Token: 0x04002D03 RID: 11523
		public const string telemetry_ggwp_event = "telemetry_ggwp_event";

		// Token: 0x04002D04 RID: 11524
		public const string NOTHING = "NOTHING";

		// Token: 0x04002D05 RID: 11525
		public const string telemetry_wam_gameStartEvent = "telemetry_wam_gameStartEvent";

		// Token: 0x04002D06 RID: 11526
		public const string telemetry_wam_levelEndEvent = "telemetry_wam_levelEndEvent";

		// Token: 0x04002D07 RID: 11527
		public const string WamMachineId = "WamMachineId";

		// Token: 0x04002D08 RID: 11528
		public const string WamGameId = "WamGameId";

		// Token: 0x04002D09 RID: 11529
		public const string WamMLevelNumber = "WamMLevelNumber";

		// Token: 0x04002D0A RID: 11530
		public const string WamGoodMolesShown = "WamGoodMolesShown";

		// Token: 0x04002D0B RID: 11531
		public const string WamHazardMolesShown = "WamHazardMolesShown";

		// Token: 0x04002D0C RID: 11532
		public const string WamLevelMinScore = "WamLevelMinScore";

		// Token: 0x04002D0D RID: 11533
		public const string WamLevelScore = "WamLevelScore";

		// Token: 0x04002D0E RID: 11534
		public const string WamHazardMolesHit = "WamHazardMolesHit";

		// Token: 0x04002D0F RID: 11535
		public const string WamGameState = "WamGameState";

		// Token: 0x04002D10 RID: 11536
		public const string CustomMapName = "CustomMapName";

		// Token: 0x04002D11 RID: 11537
		public const string LowestFPS = "LowestFPS";

		// Token: 0x04002D12 RID: 11538
		public const string LowestFPSDrawCalls = "LowestFPSDrawCalls";

		// Token: 0x04002D13 RID: 11539
		public const string LowestFPSPlayerCount = "LowestFPSPlayerCount";

		// Token: 0x04002D14 RID: 11540
		public const string AverageFPS = "AverageFPS";

		// Token: 0x04002D15 RID: 11541
		public const string AverageDrawCalls = "AverageDrawCalls";

		// Token: 0x04002D16 RID: 11542
		public const string AveragePlayerCount = "AveragePlayerCount";

		// Token: 0x04002D17 RID: 11543
		public const string HighestFPS = "HighestFPS";

		// Token: 0x04002D18 RID: 11544
		public const string HighestFPSDrawCalls = "HighestFPSDrawCalls";

		// Token: 0x04002D19 RID: 11545
		public const string HighestFPSPlayerCount = "HighestFPSPlayerCount";

		// Token: 0x04002D1A RID: 11546
		public const string CustomMapCreator = "CustomMapCreator";

		// Token: 0x04002D1B RID: 11547
		public const string CustomMapModId = "CustomMapModId";

		// Token: 0x04002D1C RID: 11548
		public const string MinPlayerCount = "MinPlayerCount";

		// Token: 0x04002D1D RID: 11549
		public const string MaxPlayerCount = "MaxPlayerCount";

		// Token: 0x04002D1E RID: 11550
		public const string PlaytimeOnMap = "PlaytimeOnMap";

		// Token: 0x04002D1F RID: 11551
		public const string PlaytimeInSeconds = "PlaytimeInSeconds";

		// Token: 0x04002D20 RID: 11552
		public const string PrivateRoom = "PrivateRoom";
	}

	// Token: 0x02000645 RID: 1605
	private class BatchRunner : MonoBehaviour
	{
		// Token: 0x06002838 RID: 10296 RVA: 0x000C907C File Offset: 0x000C727C
		private IEnumerator Start()
		{
			for (;;)
			{
				float start = Time.time;
				while (Time.time < start + GorillaTelemetry.TELEMETRY_FLUSH_SEC)
				{
					yield return null;
				}
				GorillaTelemetry.FlushTelemetry();
			}
			yield break;
		}
	}
}
