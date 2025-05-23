using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000135 RID: 309
public class ProgressionController : MonoBehaviour
{
	// Token: 0x14000020 RID: 32
	// (add) Token: 0x0600082E RID: 2094 RVA: 0x0002CE8C File Offset: 0x0002B08C
	// (remove) Token: 0x0600082F RID: 2095 RVA: 0x0002CEC0 File Offset: 0x0002B0C0
	public static event Action OnQuestSelectionChanged;

	// Token: 0x14000021 RID: 33
	// (add) Token: 0x06000830 RID: 2096 RVA: 0x0002CEF4 File Offset: 0x0002B0F4
	// (remove) Token: 0x06000831 RID: 2097 RVA: 0x0002CF28 File Offset: 0x0002B128
	public static event Action OnProgressEvent;

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06000832 RID: 2098 RVA: 0x0002CF5B File Offset: 0x0002B15B
	// (set) Token: 0x06000833 RID: 2099 RVA: 0x0002CF62 File Offset: 0x0002B162
	public static int WeeklyCap { get; private set; } = 25;

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x06000834 RID: 2100 RVA: 0x0002CF6A File Offset: 0x0002B16A
	public static int TotalPoints
	{
		get
		{
			return ProgressionController._gInstance.totalPointsRaw - ProgressionController._gInstance.unclaimedPoints;
		}
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x0002CF81 File Offset: 0x0002B181
	public static void ReportQuestChanged(bool initialLoad)
	{
		ProgressionController._gInstance.OnQuestProgressChanged(initialLoad);
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x0002CF8E File Offset: 0x0002B18E
	public static void ReportQuestSelectionChanged()
	{
		ProgressionController._gInstance.LoadCompletedQuestQueue();
		Action onQuestSelectionChanged = ProgressionController.OnQuestSelectionChanged;
		if (onQuestSelectionChanged == null)
		{
			return;
		}
		onQuestSelectionChanged();
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0002CFA9 File Offset: 0x0002B1A9
	public static void ReportQuestComplete(int questId, bool isDaily)
	{
		ProgressionController._gInstance.OnQuestComplete(questId, isDaily);
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0002CFB7 File Offset: 0x0002B1B7
	public static void RedeemProgress()
	{
		ProgressionController._gInstance.RequestProgressRedemption(new Action(ProgressionController._gInstance.OnProgressRedeemed));
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0002CFD3 File Offset: 0x0002B1D3
	[return: TupleElementNames(new string[] { "weekly", "unclaimed", "total" })]
	public static ValueTuple<int, int, int> GetProgressionData()
	{
		return ProgressionController._gInstance.GetProgress();
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x0002CFDF File Offset: 0x0002B1DF
	public static void RequestProgressUpdate()
	{
		ProgressionController gInstance = ProgressionController._gInstance;
		if (gInstance == null)
		{
			return;
		}
		gInstance.ReportProgress();
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x0002CFF0 File Offset: 0x0002B1F0
	private void Awake()
	{
		if (ProgressionController._gInstance)
		{
			Debug.LogError("Duplicate ProgressionController detected. Destroying self.", base.gameObject);
			Object.Destroy(this);
			return;
		}
		ProgressionController._gInstance = this;
		this.unclaimedPoints = PlayerPrefs.GetInt("Claimed_Points_Key", 0);
		this.RequestStatus();
		this.LoadCompletedQuestQueue();
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0002D044 File Offset: 0x0002B244
	private async void RequestStatus()
	{
		if (this.<RequestStatus>g__ShouldFetchStatus|36_0())
		{
			this._isFetchingStatus = true;
			await this.WaitForSessionToken();
			this.FetchStatus();
		}
		else
		{
			Debug.LogError("RequestStatus triggered multiple times.  That's probably not good.");
		}
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0002D07C File Offset: 0x0002B27C
	private async Task WaitForSessionToken()
	{
		while (!PlayFabAuthenticator.instance || PlayFabAuthenticator.instance.GetPlayFabPlayerId().IsNullOrEmpty() || PlayFabAuthenticator.instance.GetPlayFabSessionTicket().IsNullOrEmpty())
		{
			await Task.Yield();
			await Task.Delay(1000);
		}
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x0002D0B8 File Offset: 0x0002B2B8
	private void FetchStatus()
	{
		base.StartCoroutine(this.DoFetchStatus(new ProgressionController.GetQuestsStatusRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = "",
			MothershipToken = ""
		}, new Action<ProgressionController.GetQuestStatusResponse>(this.OnFetchStatusResponse)));
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0002D11D File Offset: 0x0002B31D
	private IEnumerator DoFetchStatus(ProgressionController.GetQuestsStatusRequest data, Action<ProgressionController.GetQuestStatusResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl + "/api/GetQuestStatus", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionController.GetQuestStatusResponse getQuestStatusResponse = JsonConvert.DeserializeObject<ProgressionController.GetQuestStatusResponse>(request.downloadHandler.text);
			callback(getQuestStatusResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this._fetchStatusRetryCount < this._maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this._fetchStatusRetryCount + 1));
				this._fetchStatusRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.FetchStatus();
			}
			else
			{
				GTDev.LogError<string>("Maximum FetchStatus retries attempted. Please check your network connection.", null);
				this._fetchStatusRetryCount = 0;
				callback(null);
			}
		}
		yield break;
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0002D13C File Offset: 0x0002B33C
	private void OnFetchStatusResponse([CanBeNull] ProgressionController.GetQuestStatusResponse response)
	{
		this._isFetchingStatus = false;
		this._statusReceived = false;
		if (response != null)
		{
			this.SetProgressionValues(response.result.GetWeeklyPoints(), this.unclaimedPoints, response.result.userPointsTotal);
			this.ReportProgress();
			return;
		}
		GTDev.LogError<string>("Error: Could not fetch status!", null);
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0002D18E File Offset: 0x0002B38E
	private void SendQuestCompleted(int questId)
	{
		if (this._isSendingQuestComplete)
		{
			return;
		}
		this._isSendingQuestComplete = true;
		this.StartSendQuestComplete(questId);
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x0002D1A8 File Offset: 0x0002B3A8
	private void StartSendQuestComplete(int questId)
	{
		base.StartCoroutine(this.DoSendQuestComplete(new ProgressionController.SetQuestCompleteRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = "",
			MothershipToken = "",
			QuestId = questId,
			ClientVersion = Application.version
		}, new Action<ProgressionController.SetQuestCompleteResponse>(this.OnSendQuestCompleteSuccess)));
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x0002D21F File Offset: 0x0002B41F
	private IEnumerator DoSendQuestComplete(ProgressionController.SetQuestCompleteRequest data, Action<ProgressionController.SetQuestCompleteResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl + "/api/SetQuestComplete", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionController.SetQuestCompleteResponse setQuestCompleteResponse = JsonConvert.DeserializeObject<ProgressionController.SetQuestCompleteResponse>(request.downloadHandler.text);
			callback(setQuestCompleteResponse);
			this.ProcessQuestSubmittedSuccess();
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.responseCode == 403L)
			{
				GTDev.LogWarning<string>("User already reached the max number of completion points for this time period!", null);
				callback(null);
				this.ClearQuestQueue();
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this._sendQuestCompleteRetryCount < this._maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this._sendQuestCompleteRetryCount + 1));
				this._sendQuestCompleteRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.StartSendQuestComplete(data.QuestId);
			}
			else
			{
				GTDev.LogError<string>("Maximum SendQuestComplete retries attempted. Please check your network connection.", null);
				this._sendQuestCompleteRetryCount = 0;
				callback(null);
				this.ProcessQuestSubmittedFail();
			}
		}
		else
		{
			this._isSendingQuestComplete = false;
		}
		yield break;
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x0002D23C File Offset: 0x0002B43C
	private void OnSendQuestCompleteSuccess([CanBeNull] ProgressionController.SetQuestCompleteResponse response)
	{
		this._isSendingQuestComplete = false;
		if (response != null)
		{
			this.UpdateProgressionValues(response.result.GetWeeklyPoints(), response.result.userPointsTotal);
			this.ReportProgress();
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x0002D26A File Offset: 0x0002B46A
	private void OnQuestProgressChanged(bool initialLoad)
	{
		this.ReportProgress();
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x0002D272 File Offset: 0x0002B472
	private void OnQuestComplete(int questId, bool isDaily)
	{
		this.QueueQuestCompletion(questId, isDaily);
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x0002D27C File Offset: 0x0002B47C
	private void QueueQuestCompletion(int questId, bool isDaily)
	{
		if (isDaily)
		{
			this._queuedDailyCompletedQuests.Add(questId);
		}
		else
		{
			this._queuedWeeklyCompletedQuests.Add(questId);
		}
		this.SaveCompletedQuestQueue();
		this.SubmitNextQuestInQueue();
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x0002D2A8 File Offset: 0x0002B4A8
	private void SubmitNextQuestInQueue()
	{
		if (this._currentlyProcessingQuest == -1 && this.AreCompletedQuestsQueued())
		{
			int num = -1;
			if (this._queuedWeeklyCompletedQuests.Count > 0)
			{
				num = this._queuedWeeklyCompletedQuests[0];
			}
			else if (this._queuedDailyCompletedQuests.Count > 0)
			{
				num = this._queuedDailyCompletedQuests[0];
			}
			this._currentlyProcessingQuest = num;
			this.SendQuestCompleted(num);
		}
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x0002D30E File Offset: 0x0002B50E
	private void ClearQuestQueue()
	{
		this._currentlyProcessingQuest = -1;
		this._queuedDailyCompletedQuests.Clear();
		this._queuedWeeklyCompletedQuests.Clear();
		this.SaveCompletedQuestQueue();
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0002D334 File Offset: 0x0002B534
	private void ProcessQuestSubmittedSuccess()
	{
		if (this._currentlyProcessingQuest != -1)
		{
			if (this.AreCompletedQuestsQueued())
			{
				if (this._queuedWeeklyCompletedQuests.Remove(this._currentlyProcessingQuest))
				{
					this.SaveCompletedQuestQueue();
				}
				else if (this._queuedDailyCompletedQuests.Remove(this._currentlyProcessingQuest))
				{
					this.SaveCompletedQuestQueue();
				}
			}
			this._currentlyProcessingQuest = -1;
			this.SubmitNextQuestInQueue();
		}
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0002D393 File Offset: 0x0002B593
	private void ProcessQuestSubmittedFail()
	{
		this._currentlyProcessingQuest = -1;
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x0002D39C File Offset: 0x0002B59C
	private bool AreCompletedQuestsQueued()
	{
		return this._queuedDailyCompletedQuests.Count > 0 || this._queuedWeeklyCompletedQuests.Count > 0;
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x0002D3BC File Offset: 0x0002B5BC
	private void SaveCompletedQuestQueue()
	{
		int num = 0;
		for (int i = 0; i < this._queuedDailyCompletedQuests.Count; i++)
		{
			PlayerPrefs.SetInt(string.Format("{0}{1}", "Queued_Quest_Daily_ID_Key", num), this._queuedDailyCompletedQuests[i]);
			num++;
		}
		int dailyQuestSetID = this._questManager.dailyQuestSetID;
		PlayerPrefs.SetInt("Queued_Quest_Daily_SetID_Key", dailyQuestSetID);
		PlayerPrefs.SetInt("Queued_Quest_Daily_SaveCount_Key", num);
		int num2 = 0;
		for (int j = 0; j < this._queuedWeeklyCompletedQuests.Count; j++)
		{
			PlayerPrefs.SetInt(string.Format("{0}{1}", "Queued_Quest_Weekly_ID_Key", num2), this._queuedWeeklyCompletedQuests[j]);
			num2++;
		}
		int weeklyQuestSetID = this._questManager.weeklyQuestSetID;
		PlayerPrefs.SetInt("Queued_Quest_Weekly_SetID_Key", weeklyQuestSetID);
		PlayerPrefs.SetInt("Queued_Quest_Weekly_SaveCount_Key", num2);
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x0002D49C File Offset: 0x0002B69C
	private void LoadCompletedQuestQueue()
	{
		this._queuedDailyCompletedQuests.Clear();
		int @int = PlayerPrefs.GetInt("Queued_Quest_Daily_SetID_Key", -1);
		int int2 = PlayerPrefs.GetInt("Queued_Quest_Daily_SaveCount_Key", -1);
		int dailyQuestSetID = this._questManager.dailyQuestSetID;
		if (@int == dailyQuestSetID)
		{
			for (int i = 0; i < int2; i++)
			{
				int int3 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Queued_Quest_Daily_ID_Key", i), -1);
				if (int3 != -1)
				{
					this._queuedDailyCompletedQuests.Add(int3);
				}
			}
		}
		this._queuedWeeklyCompletedQuests.Clear();
		int int4 = PlayerPrefs.GetInt("Queued_Quest_Weekly_SetID_Key", -1);
		int int5 = PlayerPrefs.GetInt("Queued_Quest_Weekly_SaveCount_Key", -1);
		int weeklyQuestSetID = this._questManager.weeklyQuestSetID;
		if (int4 == weeklyQuestSetID)
		{
			for (int j = 0; j < int5; j++)
			{
				int int6 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Queued_Quest_Weekly_ID_Key", j), -1);
				if (int6 != -1)
				{
					this._queuedWeeklyCompletedQuests.Add(int6);
				}
			}
		}
		this.SubmitNextQuestInQueue();
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0002D594 File Offset: 0x0002B794
	private async void RequestProgressRedemption(Action onComplete)
	{
		await Task.Yield();
		if (onComplete != null)
		{
			onComplete();
		}
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0002D5CB File Offset: 0x0002B7CB
	private void OnProgressRedeemed()
	{
		this.unclaimedPoints = 0;
		PlayerPrefs.SetInt("Claimed_Points_Key", this.unclaimedPoints);
		this.ReportProgress();
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x0002D5EC File Offset: 0x0002B7EC
	private void AddPoints(int points)
	{
		if (this.weeklyPoints >= ProgressionController.WeeklyCap)
		{
			return;
		}
		int num = Mathf.Clamp(points, 0, ProgressionController.WeeklyCap - this.weeklyPoints);
		this.SetProgressionValues(this.weeklyPoints + num, this.unclaimedPoints + num, this.totalPointsRaw + num);
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x0002D63C File Offset: 0x0002B83C
	private void UpdateProgressionValues(int weekly, int totalRaw)
	{
		int num = totalRaw - this.totalPointsRaw;
		this.unclaimedPoints += num;
		this.SetProgressionValues(weekly, this.unclaimedPoints, totalRaw);
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x0002D66E File Offset: 0x0002B86E
	private void SetProgressionValues(int weekly, int unclaimed, int totalRaw)
	{
		this.weeklyPoints = weekly;
		this.unclaimedPoints = unclaimed;
		this.totalPointsRaw = totalRaw;
		this.ReportScoreChange();
		PlayerPrefs.SetInt("Claimed_Points_Key", unclaimed);
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0002D698 File Offset: 0x0002B898
	private async void ReportProgress()
	{
		try
		{
			if (!this._progressReportPending)
			{
				this._progressReportPending = true;
				await Task.Yield();
				this._progressReportPending = false;
				Action onProgressEvent = ProgressionController.OnProgressEvent;
				if (onProgressEvent != null)
				{
					onProgressEvent();
				}
				this.ReportScoreChange();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x0002D6D0 File Offset: 0x0002B8D0
	private void ReportScoreChange()
	{
		ValueTuple<int, int, int> valueTuple = new ValueTuple<int, int, int>(this.weeklyPoints, this.unclaimedPoints, this.totalPointsRaw);
		ValueTuple<int, int, int> lastProgressReport = this._lastProgressReport;
		ValueTuple<int, int, int> valueTuple2 = valueTuple;
		if (lastProgressReport.Item1 == valueTuple2.Item1 && lastProgressReport.Item2 == valueTuple2.Item2 && lastProgressReport.Item3 == valueTuple2.Item3)
		{
			return;
		}
		if (VRRig.LocalRig)
		{
			VRRig.LocalRig.SetQuestScore(ProgressionController.TotalPoints);
		}
		this._lastProgressReport = valueTuple;
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x0002D74C File Offset: 0x0002B94C
	[return: TupleElementNames(new string[] { "weekly", "unclaimed", "total" })]
	private ValueTuple<int, int, int> GetProgress()
	{
		return new ValueTuple<int, int, int>(this.weeklyPoints, this.unclaimedPoints, this.totalPointsRaw - this.unclaimedPoints);
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x0002D7A1 File Offset: 0x0002B9A1
	[CompilerGenerated]
	private bool <RequestStatus>g__ShouldFetchStatus|36_0()
	{
		return !this._isFetchingStatus && !this._statusReceived;
	}

	// Token: 0x040009AB RID: 2475
	private static ProgressionController _gInstance;

	// Token: 0x040009AE RID: 2478
	[SerializeField]
	private RotatingQuestsManager _questManager;

	// Token: 0x040009AF RID: 2479
	private int weeklyPoints;

	// Token: 0x040009B0 RID: 2480
	private int totalPointsRaw;

	// Token: 0x040009B1 RID: 2481
	private int unclaimedPoints;

	// Token: 0x040009B2 RID: 2482
	private bool _progressReportPending;

	// Token: 0x040009B3 RID: 2483
	[TupleElementNames(new string[] { "weeklyPoints", "unclaimedPoints", "totalPointsRaw" })]
	private ValueTuple<int, int, int> _lastProgressReport;

	// Token: 0x040009B4 RID: 2484
	private bool _isFetchingStatus;

	// Token: 0x040009B5 RID: 2485
	private bool _statusReceived;

	// Token: 0x040009B6 RID: 2486
	private bool _isSendingQuestComplete;

	// Token: 0x040009B7 RID: 2487
	private int _fetchStatusRetryCount;

	// Token: 0x040009B8 RID: 2488
	private int _sendQuestCompleteRetryCount;

	// Token: 0x040009B9 RID: 2489
	private int _maxRetriesOnFail = 3;

	// Token: 0x040009BA RID: 2490
	private List<int> _queuedDailyCompletedQuests = new List<int>();

	// Token: 0x040009BB RID: 2491
	private List<int> _queuedWeeklyCompletedQuests = new List<int>();

	// Token: 0x040009BC RID: 2492
	private int _currentlyProcessingQuest = -1;

	// Token: 0x040009BD RID: 2493
	private const string kUnclaimedPointKey = "Claimed_Points_Key";

	// Token: 0x040009BF RID: 2495
	private const string kQueuedDailyQuestSetIDKey = "Queued_Quest_Daily_SetID_Key";

	// Token: 0x040009C0 RID: 2496
	private const string kQueuedDailyQuestSaveCountKey = "Queued_Quest_Daily_SaveCount_Key";

	// Token: 0x040009C1 RID: 2497
	private const string kQueuedDailyQuestIDKey = "Queued_Quest_Daily_ID_Key";

	// Token: 0x040009C2 RID: 2498
	private const string kQueuedWeeklyQuestSetIDKey = "Queued_Quest_Weekly_SetID_Key";

	// Token: 0x040009C3 RID: 2499
	private const string kQueuedWeeklyQuestSaveCountKey = "Queued_Quest_Weekly_SaveCount_Key";

	// Token: 0x040009C4 RID: 2500
	private const string kQueuedWeeklyQuestIDKey = "Queued_Quest_Weekly_ID_Key";

	// Token: 0x02000136 RID: 310
	[Serializable]
	private class GetQuestsStatusRequest
	{
		// Token: 0x040009C5 RID: 2501
		public string PlayFabId;

		// Token: 0x040009C6 RID: 2502
		public string PlayFabTicket;

		// Token: 0x040009C7 RID: 2503
		public string MothershipId;

		// Token: 0x040009C8 RID: 2504
		public string MothershipToken;
	}

	// Token: 0x02000137 RID: 311
	[Serializable]
	public class GetQuestStatusResponse
	{
		// Token: 0x040009C9 RID: 2505
		public ProgressionController.UserQuestsStatus result;
	}

	// Token: 0x02000138 RID: 312
	public class UserQuestsStatus
	{
		// Token: 0x0600085C RID: 2140 RVA: 0x0002D7B8 File Offset: 0x0002B9B8
		public int GetWeeklyPoints()
		{
			int num = 0;
			if (this.dailyPoints != null)
			{
				foreach (KeyValuePair<string, int> keyValuePair in this.dailyPoints)
				{
					num += keyValuePair.Value;
				}
			}
			if (this.weeklyPoints != null)
			{
				foreach (KeyValuePair<int, int> keyValuePair2 in this.weeklyPoints)
				{
					num += keyValuePair2.Value;
				}
			}
			return Mathf.Min(num, ProgressionController.WeeklyCap);
		}

		// Token: 0x040009CA RID: 2506
		public Dictionary<string, int> dailyPoints;

		// Token: 0x040009CB RID: 2507
		public Dictionary<int, int> weeklyPoints;

		// Token: 0x040009CC RID: 2508
		public int userPointsTotal;
	}

	// Token: 0x02000139 RID: 313
	[Serializable]
	private class SetQuestCompleteRequest
	{
		// Token: 0x040009CD RID: 2509
		public string PlayFabId;

		// Token: 0x040009CE RID: 2510
		public string PlayFabTicket;

		// Token: 0x040009CF RID: 2511
		public string MothershipId;

		// Token: 0x040009D0 RID: 2512
		public string MothershipToken;

		// Token: 0x040009D1 RID: 2513
		public int QuestId;

		// Token: 0x040009D2 RID: 2514
		public string ClientVersion;
	}

	// Token: 0x0200013A RID: 314
	[Serializable]
	public class SetQuestCompleteResponse
	{
		// Token: 0x040009D3 RID: 2515
		public ProgressionController.UserQuestsStatus result;
	}
}
