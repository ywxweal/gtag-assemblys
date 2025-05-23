using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace GorillaNetworking
{
	// Token: 0x02000C71 RID: 3185
	public class TitleVotingExample : MonoBehaviour
	{
		// Token: 0x06004F04 RID: 20228 RVA: 0x00178990 File Offset: 0x00176B90
		public async void Start()
		{
			await this.WaitForSessionToken();
			this.FetchPollsAndVote();
		}

		// Token: 0x06004F05 RID: 20229 RVA: 0x000023F4 File Offset: 0x000005F4
		public void Update()
		{
		}

		// Token: 0x06004F06 RID: 20230 RVA: 0x001789C8 File Offset: 0x00176BC8
		private async Task WaitForSessionToken()
		{
			while (!PlayFabAuthenticator.instance || PlayFabAuthenticator.instance.GetPlayFabPlayerId().IsNullOrEmpty() || PlayFabAuthenticator.instance.GetPlayFabSessionTicket().IsNullOrEmpty() || PlayFabAuthenticator.instance.userID.IsNullOrEmpty())
			{
				await Task.Yield();
				await Task.Delay(1000);
			}
		}

		// Token: 0x06004F07 RID: 20231 RVA: 0x00178A04 File Offset: 0x00176C04
		public void FetchPollsAndVote()
		{
			base.StartCoroutine(this.DoFetchPolls(new TitleVotingExample.FetchPollsRequest
			{
				TitleId = PlayFabAuthenticatorSettings.TitleId,
				PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				IncludeInactive = this.includeInactive
			}, new Action<List<TitleVotingExample.FetchPollsResponse>>(this.OnFetchPollsResponse)));
		}

		// Token: 0x06004F08 RID: 20232 RVA: 0x00178A6C File Offset: 0x00176C6C
		private void GetNonceForVotingCallback([CanBeNull] Message<UserProof> message)
		{
			if (message != null)
			{
				UserProof data = message.Data;
				this.Nonce = ((data != null) ? data.ToString() : null);
			}
			base.StartCoroutine(this.DoVote(new TitleVotingExample.VoteRequest
			{
				PollId = this.PollId,
				TitleId = PlayFabAuthenticatorSettings.TitleId,
				PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				OculusId = PlayFabAuthenticator.instance.userID,
				UserPlatform = PlayFabAuthenticator.instance.platform.ToString(),
				UserNonce = this.Nonce,
				PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				OptionIndex = this.Option,
				IsPrediction = this.isPrediction
			}, new Action<TitleVotingExample.VoteResponse>(this.OnVoteSuccess)));
		}

		// Token: 0x06004F09 RID: 20233 RVA: 0x00178B3A File Offset: 0x00176D3A
		public void Vote()
		{
			this.GetNonceForVotingCallback(null);
		}

		// Token: 0x06004F0A RID: 20234 RVA: 0x00178B43 File Offset: 0x00176D43
		private IEnumerator DoFetchPolls(TitleVotingExample.FetchPollsRequest data, Action<List<TitleVotingExample.FetchPollsResponse>> callback)
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
				List<TitleVotingExample.FetchPollsResponse> list = JsonConvert.DeserializeObject<List<TitleVotingExample.FetchPollsResponse>>(request.downloadHandler.text);
				callback(list);
			}
			else
			{
				Debug.LogError(string.Format("FetchPolls Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
				long responseCode = request.responseCode;
				if (responseCode >= 500L && responseCode < 600L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
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
					Debug.LogWarning(string.Format("Retrying Title Voting FetchPolls... Retry attempt #{0}, waiting for {1} seconds", this.fetchPollsRetryCount + 1, num));
					this.fetchPollsRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.FetchPollsAndVote();
				}
				else
				{
					Debug.LogError("Maximum FetchPolls retries attempted. Please check your network connection.");
					this.fetchPollsRetryCount = 0;
					callback(null);
				}
			}
			yield break;
		}

		// Token: 0x06004F0B RID: 20235 RVA: 0x00178B60 File Offset: 0x00176D60
		private IEnumerator DoVote(TitleVotingExample.VoteRequest data, Action<TitleVotingExample.VoteResponse> callback)
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
				TitleVotingExample.VoteResponse voteResponse = JsonConvert.DeserializeObject<TitleVotingExample.VoteResponse>(request.downloadHandler.text);
				callback(voteResponse);
			}
			else
			{
				Debug.LogError(string.Format("Vote Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
				long responseCode = request.responseCode;
				if (responseCode >= 500L && responseCode < 600L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
				}
				else if (request.responseCode == 409L)
				{
					Debug.LogWarning("User already voted on this poll!");
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
					Debug.LogWarning(string.Format("Retrying Voting... Retry attempt #{0}, waiting for {1} seconds", this.voteRetryCount + 1, num));
					this.voteRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.Vote();
				}
				else
				{
					Debug.LogError("Maximum Vote retries attempted. Please check your network connection.");
					this.voteRetryCount = 0;
					callback(null);
				}
			}
			yield break;
		}

		// Token: 0x06004F0C RID: 20236 RVA: 0x00178B7D File Offset: 0x00176D7D
		private void OnFetchPollsResponse([CanBeNull] List<TitleVotingExample.FetchPollsResponse> response)
		{
			if (response != null)
			{
				Debug.Log("Got polls: " + JsonConvert.SerializeObject(response));
				this.Vote();
				return;
			}
			Debug.LogError("Error: Could not fetch polls!");
		}

		// Token: 0x06004F0D RID: 20237 RVA: 0x00178BA8 File Offset: 0x00176DA8
		private void OnVoteSuccess([CanBeNull] TitleVotingExample.VoteResponse response)
		{
			if (response != null)
			{
				Debug.Log("Voted! " + JsonConvert.SerializeObject(response));
				return;
			}
			Debug.LogError("Error: Could not vote!");
		}

		// Token: 0x0400521C RID: 21020
		private string Nonce = "";

		// Token: 0x0400521D RID: 21021
		private int PollId = 5;

		// Token: 0x0400521E RID: 21022
		private bool includeInactive = true;

		// Token: 0x0400521F RID: 21023
		private int Option;

		// Token: 0x04005220 RID: 21024
		private bool isPrediction;

		// Token: 0x04005221 RID: 21025
		private int fetchPollsRetryCount;

		// Token: 0x04005222 RID: 21026
		private int voteRetryCount;

		// Token: 0x04005223 RID: 21027
		private int maxRetriesOnFail = 3;

		// Token: 0x02000C72 RID: 3186
		[Serializable]
		private class FetchPollsRequest
		{
			// Token: 0x04005224 RID: 21028
			public string TitleId;

			// Token: 0x04005225 RID: 21029
			public string PlayFabId;

			// Token: 0x04005226 RID: 21030
			public string PlayFabTicket;

			// Token: 0x04005227 RID: 21031
			public bool IncludeInactive;
		}

		// Token: 0x02000C73 RID: 3187
		[Serializable]
		private class FetchPollsResponse
		{
			// Token: 0x04005228 RID: 21032
			public int PollId;

			// Token: 0x04005229 RID: 21033
			public string Question;

			// Token: 0x0400522A RID: 21034
			public List<string> VoteOptions;

			// Token: 0x0400522B RID: 21035
			public List<int> VoteCount;

			// Token: 0x0400522C RID: 21036
			public List<int> PredictionCount;

			// Token: 0x0400522D RID: 21037
			public DateTime StartTime;

			// Token: 0x0400522E RID: 21038
			public DateTime EndTime;
		}

		// Token: 0x02000C74 RID: 3188
		[Serializable]
		private class VoteRequest
		{
			// Token: 0x0400522F RID: 21039
			public int PollId;

			// Token: 0x04005230 RID: 21040
			public string TitleId;

			// Token: 0x04005231 RID: 21041
			public string PlayFabId;

			// Token: 0x04005232 RID: 21042
			public string OculusId;

			// Token: 0x04005233 RID: 21043
			public string UserNonce;

			// Token: 0x04005234 RID: 21044
			public string UserPlatform;

			// Token: 0x04005235 RID: 21045
			public int OptionIndex;

			// Token: 0x04005236 RID: 21046
			public bool IsPrediction;

			// Token: 0x04005237 RID: 21047
			public string PlayFabTicket;
		}

		// Token: 0x02000C75 RID: 3189
		[Serializable]
		private class VoteResponse
		{
			// Token: 0x170007D4 RID: 2004
			// (get) Token: 0x06004F12 RID: 20242 RVA: 0x00178BF5 File Offset: 0x00176DF5
			// (set) Token: 0x06004F13 RID: 20243 RVA: 0x00178BFD File Offset: 0x00176DFD
			public int PollId { get; set; }

			// Token: 0x170007D5 RID: 2005
			// (get) Token: 0x06004F14 RID: 20244 RVA: 0x00178C06 File Offset: 0x00176E06
			// (set) Token: 0x06004F15 RID: 20245 RVA: 0x00178C0E File Offset: 0x00176E0E
			public string TitleId { get; set; }

			// Token: 0x170007D6 RID: 2006
			// (get) Token: 0x06004F16 RID: 20246 RVA: 0x00178C17 File Offset: 0x00176E17
			// (set) Token: 0x06004F17 RID: 20247 RVA: 0x00178C1F File Offset: 0x00176E1F
			public List<string> VoteOptions { get; set; }

			// Token: 0x170007D7 RID: 2007
			// (get) Token: 0x06004F18 RID: 20248 RVA: 0x00178C28 File Offset: 0x00176E28
			// (set) Token: 0x06004F19 RID: 20249 RVA: 0x00178C30 File Offset: 0x00176E30
			public List<int> VoteCount { get; set; }

			// Token: 0x170007D8 RID: 2008
			// (get) Token: 0x06004F1A RID: 20250 RVA: 0x00178C39 File Offset: 0x00176E39
			// (set) Token: 0x06004F1B RID: 20251 RVA: 0x00178C41 File Offset: 0x00176E41
			public List<int> PredictionCount { get; set; }
		}
	}
}
