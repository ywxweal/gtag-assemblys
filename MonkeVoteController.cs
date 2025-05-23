using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000119 RID: 281
public class MonkeVoteController : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x06000745 RID: 1861 RVA: 0x00029626 File Offset: 0x00027826
	// (set) Token: 0x06000746 RID: 1862 RVA: 0x0002962D File Offset: 0x0002782D
	public static MonkeVoteController instance { get; private set; }

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x06000747 RID: 1863 RVA: 0x00029638 File Offset: 0x00027838
	// (remove) Token: 0x06000748 RID: 1864 RVA: 0x00029670 File Offset: 0x00027870
	public event Action OnPollsUpdated;

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x06000749 RID: 1865 RVA: 0x000296A8 File Offset: 0x000278A8
	// (remove) Token: 0x0600074A RID: 1866 RVA: 0x000296E0 File Offset: 0x000278E0
	public event Action OnVoteAccepted;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x0600074B RID: 1867 RVA: 0x00029718 File Offset: 0x00027918
	// (remove) Token: 0x0600074C RID: 1868 RVA: 0x00029750 File Offset: 0x00027950
	public event Action OnVoteFailed;

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x0600074D RID: 1869 RVA: 0x00029788 File Offset: 0x00027988
	// (remove) Token: 0x0600074E RID: 1870 RVA: 0x000297C0 File Offset: 0x000279C0
	public event Action OnCurrentPollEnded;

	// Token: 0x0600074F RID: 1871 RVA: 0x000297F5 File Offset: 0x000279F5
	public void Awake()
	{
		if (MonkeVoteController.instance == null)
		{
			MonkeVoteController.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x00029814 File Offset: 0x00027A14
	public void SliceUpdate()
	{
		if (this.isCurrentPollActive && !this.hasCurrentPollCompleted && this.currentPollCompletionTime < DateTime.UtcNow)
		{
			GTDev.Log<string>("Active vote poll completed.", null);
			this.hasCurrentPollCompleted = true;
			Action onCurrentPollEnded = this.OnCurrentPollEnded;
			if (onCurrentPollEnded == null)
			{
				return;
			}
			onCurrentPollEnded();
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00017251 File Offset: 0x00015451
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x00029868 File Offset: 0x00027A68
	public async void RequestPolls()
	{
		if (!this.isFetchingPoll && (!this.hasPoll || (this.isCurrentPollActive && this.hasCurrentPollCompleted)))
		{
			this.isFetchingPoll = true;
			await this.WaitForSessionToken();
			this.FetchPolls();
		}
		else
		{
			Action onPollsUpdated = this.OnPollsUpdated;
			if (onPollsUpdated != null)
			{
				onPollsUpdated();
			}
		}
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x000298A0 File Offset: 0x00027AA0
	private async Task WaitForSessionToken()
	{
		while (!PlayFabAuthenticator.instance || PlayFabAuthenticator.instance.GetPlayFabPlayerId().IsNullOrEmpty() || PlayFabAuthenticator.instance.GetPlayFabSessionTicket().IsNullOrEmpty() || PlayFabAuthenticator.instance.userID.IsNullOrEmpty())
		{
			await Task.Yield();
			await Task.Delay(1000);
		}
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x000298DC File Offset: 0x00027ADC
	private void FetchPolls()
	{
		base.StartCoroutine(this.DoFetchPolls(new MonkeVoteController.FetchPollsRequest
		{
			TitleId = PlayFabAuthenticatorSettings.TitleId,
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			IncludeInactive = this.includeInactive
		}, new Action<List<MonkeVoteController.FetchPollsResponse>>(this.OnFetchPollsResponse)));
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00029942 File Offset: 0x00027B42
	private IEnumerator DoFetchPolls(MonkeVoteController.FetchPollsRequest data, Action<List<MonkeVoteController.FetchPollsResponse>> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/FetchPoll", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			List<MonkeVoteController.FetchPollsResponse> list = JsonConvert.DeserializeObject<List<MonkeVoteController.FetchPollsResponse>>(request.downloadHandler.text);
			callback(list);
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
			if (this.fetchPollsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.fetchPollsRetryCount + 1));
				this.fetchPollsRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.FetchPolls();
			}
			else
			{
				GTDev.LogError<string>("Maximum FetchPolls retries attempted. Please check your network connection.", null);
				this.fetchPollsRetryCount = 0;
				callback(null);
			}
		}
		yield break;
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x00029960 File Offset: 0x00027B60
	private void OnFetchPollsResponse([CanBeNull] List<MonkeVoteController.FetchPollsResponse> response)
	{
		this.isFetchingPoll = false;
		this.hasPoll = false;
		this.lastPollData = null;
		this.currentPollData = null;
		this.isCurrentPollActive = false;
		this.hasCurrentPollCompleted = false;
		if (response != null)
		{
			DateTime minValue = DateTime.MinValue;
			using (List<MonkeVoteController.FetchPollsResponse>.Enumerator enumerator = response.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MonkeVoteController.FetchPollsResponse fetchPollsResponse = enumerator.Current;
					if (fetchPollsResponse.isActive)
					{
						this.hasPoll = true;
						this.currentPollData = fetchPollsResponse;
						if (this.currentPollData.EndTime > DateTime.UtcNow)
						{
							this.isCurrentPollActive = true;
							this.hasCurrentPollCompleted = false;
							this.currentPollCompletionTime = this.currentPollData.EndTime;
							this.currentPollCompletionTime = this.currentPollCompletionTime.AddMinutes(1.0);
						}
					}
					if (!fetchPollsResponse.isActive && fetchPollsResponse.EndTime > minValue && fetchPollsResponse.EndTime < DateTime.UtcNow)
					{
						this.lastPollData = fetchPollsResponse;
					}
				}
				goto IL_0106;
			}
		}
		GTDev.LogError<string>("Error: Could not fetch polls!", null);
		IL_0106:
		Action onPollsUpdated = this.OnPollsUpdated;
		if (onPollsUpdated == null)
		{
			return;
		}
		onPollsUpdated();
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x00029A94 File Offset: 0x00027C94
	public void Vote(int pollId, int option, bool isPrediction)
	{
		if (!this.hasPoll)
		{
			return;
		}
		if (this.isSendingVote)
		{
			return;
		}
		this.isSendingVote = true;
		this.pollId = pollId;
		this.option = option;
		this.isPrediction = isPrediction;
		this.SendVote();
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00029ACA File Offset: 0x00027CCA
	private void SendVote()
	{
		this.GetNonceForVotingCallback(null);
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x00029AD4 File Offset: 0x00027CD4
	private void GetNonceForVotingCallback([CanBeNull] Message<UserProof> message)
	{
		if (message != null)
		{
			UserProof data = message.Data;
			this.Nonce = ((data != null) ? data.Value : null);
		}
		base.StartCoroutine(this.DoVote(new MonkeVoteController.VoteRequest
		{
			PollId = this.pollId,
			TitleId = PlayFabAuthenticatorSettings.TitleId,
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			OculusId = PlayFabAuthenticator.instance.userID,
			UserPlatform = PlayFabAuthenticator.instance.platform.ToString(),
			UserNonce = this.Nonce,
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			OptionIndex = this.option,
			IsPrediction = this.isPrediction
		}, new Action<MonkeVoteController.VoteResponse>(this.OnVoteSuccess)));
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00029BA2 File Offset: 0x00027DA2
	private IEnumerator DoVote(MonkeVoteController.VoteRequest data, Action<MonkeVoteController.VoteResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/Vote", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			MonkeVoteController.VoteResponse voteResponse = JsonConvert.DeserializeObject<MonkeVoteController.VoteResponse>(request.downloadHandler.text);
			callback(voteResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.responseCode == 429L)
			{
				GTDev.LogWarning<string>("User already voted on this poll!", null);
				callback(null);
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this.voteRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.voteRetryCount + 1));
				this.voteRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.SendVote();
			}
			else
			{
				GTDev.LogError<string>("Maximum Vote retries attempted. Please check your network connection.", null);
				this.voteRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.isSendingVote = false;
		}
		yield break;
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00029BBF File Offset: 0x00027DBF
	private void OnVoteSuccess([CanBeNull] MonkeVoteController.VoteResponse response)
	{
		this.isSendingVote = false;
		if (response != null)
		{
			this.lastVoteData = response;
			Action onVoteAccepted = this.OnVoteAccepted;
			if (onVoteAccepted == null)
			{
				return;
			}
			onVoteAccepted();
			return;
		}
		else
		{
			Action onVoteFailed = this.OnVoteFailed;
			if (onVoteFailed == null)
			{
				return;
			}
			onVoteFailed();
			return;
		}
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x00029BF3 File Offset: 0x00027DF3
	public MonkeVoteController.FetchPollsResponse GetLastPollData()
	{
		return this.lastPollData;
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00029BFB File Offset: 0x00027DFB
	public MonkeVoteController.FetchPollsResponse GetCurrentPollData()
	{
		return this.currentPollData;
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x00029C03 File Offset: 0x00027E03
	public MonkeVoteController.VoteResponse GetVoteData()
	{
		return this.lastVoteData;
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x00029C0B File Offset: 0x00027E0B
	public int GetLastVotePollId()
	{
		return this.pollId;
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x00029C13 File Offset: 0x00027E13
	public int GetLastVoteSelectedOption()
	{
		return this.option;
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x00029C1B File Offset: 0x00027E1B
	public bool GetLastVoteWasPrediction()
	{
		return this.isPrediction;
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x00029C23 File Offset: 0x00027E23
	public DateTime GetCurrentPollCompletionTime()
	{
		return this.currentPollCompletionTime;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x040008BF RID: 2239
	private string Nonce = "";

	// Token: 0x040008C0 RID: 2240
	private bool includeInactive = true;

	// Token: 0x040008C1 RID: 2241
	private int fetchPollsRetryCount;

	// Token: 0x040008C2 RID: 2242
	private int maxRetriesOnFail = 3;

	// Token: 0x040008C3 RID: 2243
	private int voteRetryCount;

	// Token: 0x040008C8 RID: 2248
	private MonkeVoteController.FetchPollsResponse lastPollData;

	// Token: 0x040008C9 RID: 2249
	private MonkeVoteController.FetchPollsResponse currentPollData;

	// Token: 0x040008CA RID: 2250
	private MonkeVoteController.VoteResponse lastVoteData;

	// Token: 0x040008CB RID: 2251
	private bool isFetchingPoll;

	// Token: 0x040008CC RID: 2252
	private bool hasPoll;

	// Token: 0x040008CD RID: 2253
	private bool isCurrentPollActive;

	// Token: 0x040008CE RID: 2254
	private bool hasCurrentPollCompleted;

	// Token: 0x040008CF RID: 2255
	private DateTime currentPollCompletionTime;

	// Token: 0x040008D0 RID: 2256
	private bool isSendingVote;

	// Token: 0x040008D1 RID: 2257
	private int pollId = -1;

	// Token: 0x040008D2 RID: 2258
	private int option;

	// Token: 0x040008D3 RID: 2259
	private bool isPrediction;

	// Token: 0x0200011A RID: 282
	[Serializable]
	private class FetchPollsRequest
	{
		// Token: 0x040008D4 RID: 2260
		public string TitleId;

		// Token: 0x040008D5 RID: 2261
		public string PlayFabId;

		// Token: 0x040008D6 RID: 2262
		public string PlayFabTicket;

		// Token: 0x040008D7 RID: 2263
		public bool IncludeInactive;
	}

	// Token: 0x0200011B RID: 283
	[Serializable]
	public class FetchPollsResponse
	{
		// Token: 0x040008D8 RID: 2264
		public int PollId;

		// Token: 0x040008D9 RID: 2265
		public string Question;

		// Token: 0x040008DA RID: 2266
		public List<string> VoteOptions;

		// Token: 0x040008DB RID: 2267
		public List<int> VoteCount;

		// Token: 0x040008DC RID: 2268
		public List<int> PredictionCount;

		// Token: 0x040008DD RID: 2269
		public DateTime StartTime;

		// Token: 0x040008DE RID: 2270
		public DateTime EndTime;

		// Token: 0x040008DF RID: 2271
		public bool isActive;
	}

	// Token: 0x0200011C RID: 284
	[Serializable]
	private class VoteRequest
	{
		// Token: 0x040008E0 RID: 2272
		public int PollId;

		// Token: 0x040008E1 RID: 2273
		public string TitleId;

		// Token: 0x040008E2 RID: 2274
		public string PlayFabId;

		// Token: 0x040008E3 RID: 2275
		public string OculusId;

		// Token: 0x040008E4 RID: 2276
		public string UserNonce;

		// Token: 0x040008E5 RID: 2277
		public string UserPlatform;

		// Token: 0x040008E6 RID: 2278
		public int OptionIndex;

		// Token: 0x040008E7 RID: 2279
		public bool IsPrediction;

		// Token: 0x040008E8 RID: 2280
		public string PlayFabTicket;
	}

	// Token: 0x0200011D RID: 285
	[Serializable]
	public class VoteResponse
	{
		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000769 RID: 1897 RVA: 0x00029C53 File Offset: 0x00027E53
		// (set) Token: 0x0600076A RID: 1898 RVA: 0x00029C5B File Offset: 0x00027E5B
		public int PollId { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600076B RID: 1899 RVA: 0x00029C64 File Offset: 0x00027E64
		// (set) Token: 0x0600076C RID: 1900 RVA: 0x00029C6C File Offset: 0x00027E6C
		public string TitleId { get; set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600076D RID: 1901 RVA: 0x00029C75 File Offset: 0x00027E75
		// (set) Token: 0x0600076E RID: 1902 RVA: 0x00029C7D File Offset: 0x00027E7D
		public List<string> VoteOptions { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600076F RID: 1903 RVA: 0x00029C86 File Offset: 0x00027E86
		// (set) Token: 0x06000770 RID: 1904 RVA: 0x00029C8E File Offset: 0x00027E8E
		public List<int> VoteCount { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000771 RID: 1905 RVA: 0x00029C97 File Offset: 0x00027E97
		// (set) Token: 0x06000772 RID: 1906 RVA: 0x00029C9F File Offset: 0x00027E9F
		public List<int> PredictionCount { get; set; }
	}
}
