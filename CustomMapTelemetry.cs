using System;
using System.Collections;
using ModIO;
using Unity.Profiling;
using UnityEngine;

// Token: 0x0200070C RID: 1804
public class CustomMapTelemetry : MonoBehaviour
{
	// Token: 0x17000478 RID: 1144
	// (get) Token: 0x06002D00 RID: 11520 RVA: 0x000DE86B File Offset: 0x000DCA6B
	public static bool IsActive
	{
		get
		{
			return CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted;
		}
	}

	// Token: 0x06002D01 RID: 11521 RVA: 0x000DE87B File Offset: 0x000DCA7B
	private void Awake()
	{
		if (CustomMapTelemetry.instance == null)
		{
			CustomMapTelemetry.instance = this;
			return;
		}
		if (CustomMapTelemetry.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06002D02 RID: 11522 RVA: 0x000DE8AF File Offset: 0x000DCAAF
	private static void OnPlayerJoinedRoom(NetPlayer obj)
	{
		CustomMapTelemetry.runningPlayerCount++;
		CustomMapTelemetry.maxPlayersInMap = Math.Max(CustomMapTelemetry.runningPlayerCount, CustomMapTelemetry.maxPlayersInMap);
	}

	// Token: 0x06002D03 RID: 11523 RVA: 0x000DE8D1 File Offset: 0x000DCAD1
	private static void OnPlayerLeftRoom(NetPlayer obj)
	{
		CustomMapTelemetry.runningPlayerCount--;
		CustomMapTelemetry.minPlayersInMap = Math.Min(CustomMapTelemetry.runningPlayerCount, CustomMapTelemetry.minPlayersInMap);
	}

	// Token: 0x06002D04 RID: 11524 RVA: 0x000DE8F4 File Offset: 0x000DCAF4
	public static void StartMapTracking()
	{
		if (CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.mapEnterTime = Time.unscaledTime;
		float value = Random.value;
		if (value <= 0.01f)
		{
			CustomMapTelemetry.StartMetricsCapture();
		}
		else if (value >= 0.99f)
		{
			CustomMapTelemetry.StartPerfCapture();
		}
		if (CustomMapTelemetry.metricsCaptureStarted || CustomMapTelemetry.perfCaptureStarted)
		{
			ModIOManager.GetModProfile(new ModId(CustomMapLoader.LoadedMapModId), delegate(ModIORequestResultAnd<ModProfile> resultAndProfile)
			{
				if (resultAndProfile.result.success)
				{
					CustomMapTelemetry.mapName = resultAndProfile.data.name;
					CustomMapTelemetry.mapModId = resultAndProfile.data.id.id;
					CustomMapTelemetry.mapCreatorUsername = resultAndProfile.data.creator.username;
				}
			});
		}
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x000DE978 File Offset: 0x000DCB78
	public static void EndMapTracking()
	{
		CustomMapTelemetry.EndMetricsCapture();
		CustomMapTelemetry.EndPerfCapture();
		CustomMapTelemetry.mapName = "NULL";
		CustomMapTelemetry.mapCreatorUsername = "NULL";
		CustomMapTelemetry.mapEnterTime = -1f;
		CustomMapTelemetry.mapModId = 0L;
	}

	// Token: 0x06002D06 RID: 11526 RVA: 0x000DE9AC File Offset: 0x000DCBAC
	private static void StartMetricsCapture()
	{
		if (CustomMapTelemetry.metricsCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.metricsCaptureStarted = true;
		NetworkSystem.Instance.OnPlayerJoined -= CustomMapTelemetry.OnPlayerJoinedRoom;
		NetworkSystem.Instance.OnPlayerJoined += CustomMapTelemetry.OnPlayerJoinedRoom;
		NetworkSystem.Instance.OnPlayerLeft -= CustomMapTelemetry.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnPlayerLeft += CustomMapTelemetry.OnPlayerLeftRoom;
		CustomMapTelemetry.runningPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
		CustomMapTelemetry.minPlayersInMap = CustomMapTelemetry.runningPlayerCount;
		CustomMapTelemetry.maxPlayersInMap = CustomMapTelemetry.runningPlayerCount;
	}

	// Token: 0x06002D07 RID: 11527 RVA: 0x000DEA44 File Offset: 0x000DCC44
	private static void EndMetricsCapture()
	{
		if (!CustomMapTelemetry.metricsCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.metricsCaptureStarted = false;
		NetworkSystem.Instance.OnPlayerJoined -= CustomMapTelemetry.OnPlayerJoinedRoom;
		NetworkSystem.Instance.OnPlayerLeft -= CustomMapTelemetry.OnPlayerLeftRoom;
		CustomMapTelemetry.inPrivateRoom = NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate;
		int num = Mathf.RoundToInt(Time.unscaledTime - CustomMapTelemetry.mapEnterTime);
		if (num < 30)
		{
			return;
		}
		if (CustomMapTelemetry.mapName.Equals("NULL") || CustomMapTelemetry.mapModId == 0L)
		{
			Debug.LogError("[CustomMapTelemetry::EndMetricsCapture] mapName or mapModID is invalid, throwing out this capture data...");
			return;
		}
		GorillaTelemetry.PostCustomMapTracking(CustomMapTelemetry.mapName, CustomMapTelemetry.mapModId, CustomMapTelemetry.mapCreatorUsername, CustomMapTelemetry.minPlayersInMap, CustomMapTelemetry.maxPlayersInMap, num, CustomMapTelemetry.inPrivateRoom);
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x000DEB08 File Offset: 0x000DCD08
	private static void StartPerfCapture()
	{
		if (CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.perfCaptureStarted = true;
		if (CustomMapTelemetry.instance.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.EndPerfCapture();
		}
		CustomMapTelemetry.drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 1, ProfilerRecorderOptions.Default);
		CustomMapTelemetry.LowestFPS = int.MaxValue;
		CustomMapTelemetry.HighestFPS = int.MinValue;
		CustomMapTelemetry.totalFPS = 0;
		CustomMapTelemetry.totalDrawCalls = 0;
		CustomMapTelemetry.totalPlayerCount = 0;
		CustomMapTelemetry.frameCounter = 0;
		CustomMapTelemetry.instance.perfCaptureCoroutine = CustomMapTelemetry.instance.StartCoroutine(CustomMapTelemetry.instance.CaptureMapPerformance());
	}

	// Token: 0x06002D09 RID: 11529 RVA: 0x000DEBA0 File Offset: 0x000DCDA0
	private static void EndPerfCapture()
	{
		if (!CustomMapTelemetry.perfCaptureStarted)
		{
			return;
		}
		CustomMapTelemetry.perfCaptureStarted = false;
		if (CustomMapTelemetry.instance.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.instance.StopAllCoroutines();
			CustomMapTelemetry.instance.perfCaptureCoroutine = null;
		}
		CustomMapTelemetry.drawCallsRecorder.Dispose();
		if (CustomMapTelemetry.frameCounter == 0)
		{
			return;
		}
		int num = Mathf.RoundToInt(Time.unscaledTime - CustomMapTelemetry.mapEnterTime);
		CustomMapTelemetry.AverageFPS = CustomMapTelemetry.totalFPS / CustomMapTelemetry.frameCounter;
		CustomMapTelemetry.AverageDrawCalls = CustomMapTelemetry.totalDrawCalls / CustomMapTelemetry.frameCounter;
		CustomMapTelemetry.AveragePlayerCount = CustomMapTelemetry.totalPlayerCount / CustomMapTelemetry.frameCounter;
		if (num < 30)
		{
			return;
		}
		if (CustomMapTelemetry.mapName.Equals("NULL") || CustomMapTelemetry.mapModId == 0L)
		{
			Debug.LogError("[CustomMapTelemetry::EndPerfCapture] mapName or mapModID is invalid, throwing out this capture data...");
			return;
		}
		GorillaTelemetry.PostCustomMapPerformance(CustomMapTelemetry.mapName, CustomMapTelemetry.mapModId, CustomMapTelemetry.LowestFPS, CustomMapTelemetry.LowestFPSDrawCalls, CustomMapTelemetry.LowestFPSPlayerCount, CustomMapTelemetry.AverageFPS, CustomMapTelemetry.AverageDrawCalls, CustomMapTelemetry.AveragePlayerCount, CustomMapTelemetry.HighestFPS, CustomMapTelemetry.HighestFPSDrawCalls, CustomMapTelemetry.HighestFPSPlayerCount, num);
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x000DEC9B File Offset: 0x000DCE9B
	private IEnumerator CaptureMapPerformance()
	{
		for (;;)
		{
			int num = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
			int num2 = Mathf.RoundToInt((float)CustomMapTelemetry.drawCallsRecorder.LastValue);
			int roomPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
			CustomMapTelemetry.totalFPS += num;
			CustomMapTelemetry.totalDrawCalls += num2;
			CustomMapTelemetry.totalPlayerCount += roomPlayerCount;
			if (num > CustomMapTelemetry.HighestFPS)
			{
				CustomMapTelemetry.HighestFPS = num;
				CustomMapTelemetry.HighestFPSDrawCalls = num2;
				CustomMapTelemetry.HighestFPSPlayerCount = roomPlayerCount;
			}
			if (num < CustomMapTelemetry.LowestFPS)
			{
				CustomMapTelemetry.LowestFPS = num;
				CustomMapTelemetry.LowestFPSDrawCalls = num2;
				CustomMapTelemetry.LowestFPSPlayerCount = roomPlayerCount;
			}
			CustomMapTelemetry.frameCounter++;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06002D0B RID: 11531 RVA: 0x000DECA3 File Offset: 0x000DCEA3
	private void OnDestroy()
	{
		if (this.perfCaptureCoroutine != null)
		{
			CustomMapTelemetry.EndMapTracking();
		}
	}

	// Token: 0x04003341 RID: 13121
	[OnEnterPlay_SetNull]
	private static volatile CustomMapTelemetry instance;

	// Token: 0x04003342 RID: 13122
	private static string mapName;

	// Token: 0x04003343 RID: 13123
	private static long mapModId;

	// Token: 0x04003344 RID: 13124
	private static string mapCreatorUsername;

	// Token: 0x04003345 RID: 13125
	private static bool metricsCaptureStarted;

	// Token: 0x04003346 RID: 13126
	private static float mapEnterTime;

	// Token: 0x04003347 RID: 13127
	private static int runningPlayerCount;

	// Token: 0x04003348 RID: 13128
	private static int minPlayersInMap;

	// Token: 0x04003349 RID: 13129
	private static int maxPlayersInMap;

	// Token: 0x0400334A RID: 13130
	private static bool inPrivateRoom;

	// Token: 0x0400334B RID: 13131
	private const int minimumPlaytimeForTracking = 30;

	// Token: 0x0400334C RID: 13132
	private static int LowestFPS = int.MaxValue;

	// Token: 0x0400334D RID: 13133
	private static int LowestFPSDrawCalls;

	// Token: 0x0400334E RID: 13134
	private static int LowestFPSPlayerCount;

	// Token: 0x0400334F RID: 13135
	private static int AverageFPS;

	// Token: 0x04003350 RID: 13136
	private static int AverageDrawCalls;

	// Token: 0x04003351 RID: 13137
	private static int AveragePlayerCount;

	// Token: 0x04003352 RID: 13138
	private static int HighestFPS = int.MinValue;

	// Token: 0x04003353 RID: 13139
	private static int HighestFPSDrawCalls;

	// Token: 0x04003354 RID: 13140
	private static int HighestFPSPlayerCount;

	// Token: 0x04003355 RID: 13141
	private static int totalFPS;

	// Token: 0x04003356 RID: 13142
	private static int totalDrawCalls;

	// Token: 0x04003357 RID: 13143
	private static int totalPlayerCount;

	// Token: 0x04003358 RID: 13144
	private static int frameCounter;

	// Token: 0x04003359 RID: 13145
	private Coroutine perfCaptureCoroutine;

	// Token: 0x0400335A RID: 13146
	private static ProfilerRecorder drawCallsRecorder;

	// Token: 0x0400335B RID: 13147
	private static bool perfCaptureStarted;
}
