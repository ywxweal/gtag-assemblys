using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020008B6 RID: 2230
public class FriendBackendController : MonoBehaviour
{
	// Token: 0x1400005E RID: 94
	// (add) Token: 0x060035F0 RID: 13808 RVA: 0x00105638 File Offset: 0x00103838
	// (remove) Token: 0x060035F1 RID: 13809 RVA: 0x00105670 File Offset: 0x00103870
	public event Action<bool> OnGetFriendsComplete;

	// Token: 0x1400005F RID: 95
	// (add) Token: 0x060035F2 RID: 13810 RVA: 0x001056A8 File Offset: 0x001038A8
	// (remove) Token: 0x060035F3 RID: 13811 RVA: 0x001056E0 File Offset: 0x001038E0
	public event Action<bool> OnSetPrivacyStateComplete;

	// Token: 0x14000060 RID: 96
	// (add) Token: 0x060035F4 RID: 13812 RVA: 0x00105718 File Offset: 0x00103918
	// (remove) Token: 0x060035F5 RID: 13813 RVA: 0x00105750 File Offset: 0x00103950
	public event Action<NetPlayer, bool> OnAddFriendComplete;

	// Token: 0x14000061 RID: 97
	// (add) Token: 0x060035F6 RID: 13814 RVA: 0x00105788 File Offset: 0x00103988
	// (remove) Token: 0x060035F7 RID: 13815 RVA: 0x001057C0 File Offset: 0x001039C0
	public event Action<FriendBackendController.Friend, bool> OnRemoveFriendComplete;

	// Token: 0x17000537 RID: 1335
	// (get) Token: 0x060035F8 RID: 13816 RVA: 0x001057F5 File Offset: 0x001039F5
	public List<FriendBackendController.Friend> FriendsList
	{
		get
		{
			return this.lastFriendsList;
		}
	}

	// Token: 0x17000538 RID: 1336
	// (get) Token: 0x060035F9 RID: 13817 RVA: 0x001057FD File Offset: 0x001039FD
	public FriendBackendController.PrivacyState MyPrivacyState
	{
		get
		{
			return this.lastPrivacyState;
		}
	}

	// Token: 0x060035FA RID: 13818 RVA: 0x00105805 File Offset: 0x00103A05
	public void GetFriends()
	{
		if (!this.getFriendsInProgress)
		{
			this.getFriendsInProgress = true;
			this.GetFriendsInternal();
		}
	}

	// Token: 0x060035FB RID: 13819 RVA: 0x0010581C File Offset: 0x00103A1C
	public void SetPrivacyState(FriendBackendController.PrivacyState state)
	{
		if (!this.setPrivacyStateInProgress)
		{
			this.setPrivacyStateInProgress = true;
			this.setPrivacyStateState = state;
			this.SetPrivacyStateInternal();
			return;
		}
		this.setPrivacyStateQueue.Enqueue(state);
	}

	// Token: 0x060035FC RID: 13820 RVA: 0x00105848 File Offset: 0x00103A48
	public void AddFriend(NetPlayer target)
	{
		if (target == null)
		{
			return;
		}
		int hashCode = target.UserId.GetHashCode();
		if (!this.addFriendInProgress)
		{
			this.addFriendInProgress = true;
			this.addFriendTargetIdHash = hashCode;
			this.addFriendTargetPlayer = target;
			this.AddFriendInternal();
			return;
		}
		if (hashCode != this.addFriendTargetIdHash && !this.addFriendRequestQueue.Contains(new ValueTuple<int, NetPlayer>(hashCode, target)))
		{
			this.addFriendRequestQueue.Enqueue(new ValueTuple<int, NetPlayer>(hashCode, target));
		}
	}

	// Token: 0x060035FD RID: 13821 RVA: 0x001058B8 File Offset: 0x00103AB8
	public void RemoveFriend(FriendBackendController.Friend target)
	{
		if (target == null)
		{
			return;
		}
		int hashCode = target.Presence.FriendLinkId.GetHashCode();
		if (!this.removeFriendInProgress)
		{
			this.removeFriendInProgress = true;
			this.removeFriendTargetIdHash = hashCode;
			this.removeFriendTarget = target;
			this.RemoveFriendInternal();
			return;
		}
		if (hashCode != this.addFriendTargetIdHash && !this.removeFriendRequestQueue.Contains(new ValueTuple<int, FriendBackendController.Friend>(hashCode, target)))
		{
			this.removeFriendRequestQueue.Enqueue(new ValueTuple<int, FriendBackendController.Friend>(hashCode, target));
		}
	}

	// Token: 0x060035FE RID: 13822 RVA: 0x0010592D File Offset: 0x00103B2D
	private void Awake()
	{
		if (FriendBackendController.Instance == null)
		{
			FriendBackendController.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060035FF RID: 13823 RVA: 0x00105950 File Offset: 0x00103B50
	private void GetFriendsInternal()
	{
		base.StartCoroutine(this.SendGetFriendsRequest(new FriendBackendController.GetFriendsRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = ""
		}, new Action<FriendBackendController.GetFriendsResponse>(this.GetFriendsComplete)));
	}

	// Token: 0x06003600 RID: 13824 RVA: 0x001059AA File Offset: 0x00103BAA
	private IEnumerator SendGetFriendsRequest(FriendBackendController.GetFriendsRequest data, Action<FriendBackendController.GetFriendsResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/GetFriendsV2", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			FriendBackendController.GetFriendsResponse getFriendsResponse = JsonConvert.DeserializeObject<FriendBackendController.GetFriendsResponse>(request.downloadHandler.text);
			callback(getFriendsResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.getFriendsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.getFriendsRetryCount + 1));
				this.getFriendsRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.GetFriendsInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum GetFriends retries attempted. Please check your network connection.", null);
				this.getFriendsRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.getFriendsInProgress = false;
		}
		yield break;
	}

	// Token: 0x06003601 RID: 13825 RVA: 0x001059C8 File Offset: 0x00103BC8
	private void GetFriendsComplete([CanBeNull] FriendBackendController.GetFriendsResponse response)
	{
		this.getFriendsInProgress = false;
		if (response != null)
		{
			this.lastGetFriendsResponse = response;
			if (this.lastGetFriendsResponse.Result != null)
			{
				this.lastPrivacyState = this.lastGetFriendsResponse.Result.MyPrivacyState;
				if (this.lastGetFriendsResponse.Result.Friends != null)
				{
					this.lastFriendsList.Clear();
					foreach (FriendBackendController.Friend friend in this.lastGetFriendsResponse.Result.Friends)
					{
						this.lastFriendsList.Add(friend);
					}
				}
			}
			Action<bool> onGetFriendsComplete = this.OnGetFriendsComplete;
			if (onGetFriendsComplete == null)
			{
				return;
			}
			onGetFriendsComplete(true);
			return;
		}
		else
		{
			Action<bool> onGetFriendsComplete2 = this.OnGetFriendsComplete;
			if (onGetFriendsComplete2 == null)
			{
				return;
			}
			onGetFriendsComplete2(false);
			return;
		}
	}

	// Token: 0x06003602 RID: 13826 RVA: 0x00105AA4 File Offset: 0x00103CA4
	private void SetPrivacyStateInternal()
	{
		base.StartCoroutine(this.SendSetPrivacyStateRequest(new FriendBackendController.SetPrivacyStateRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			PrivacyState = this.setPrivacyStateState.ToString()
		}, new Action<FriendBackendController.SetPrivacyStateResponse>(this.SetPrivacyStateComplete)));
	}

	// Token: 0x06003603 RID: 13827 RVA: 0x00105B0A File Offset: 0x00103D0A
	private IEnumerator SendSetPrivacyStateRequest(FriendBackendController.SetPrivacyStateRequest data, Action<FriendBackendController.SetPrivacyStateResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/SetPrivacyState", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			FriendBackendController.SetPrivacyStateResponse setPrivacyStateResponse = JsonConvert.DeserializeObject<FriendBackendController.SetPrivacyStateResponse>(request.downloadHandler.text);
			callback(setPrivacyStateResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.setPrivacyStateRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.setPrivacyStateRetryCount + 1));
				this.setPrivacyStateRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.SetPrivacyStateInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum SetPrivacyState retries attempted. Please check your network connection.", null);
				this.setPrivacyStateRetryCount = 0;
				callback(null);
			}
		}
		else
		{
			this.setPrivacyStateInProgress = false;
		}
		yield break;
	}

	// Token: 0x06003604 RID: 13828 RVA: 0x00105B28 File Offset: 0x00103D28
	private void SetPrivacyStateComplete([CanBeNull] FriendBackendController.SetPrivacyStateResponse response)
	{
		this.setPrivacyStateInProgress = false;
		if (response != null)
		{
			this.lastPrivacyStateResponse = response;
			Action<bool> onSetPrivacyStateComplete = this.OnSetPrivacyStateComplete;
			if (onSetPrivacyStateComplete != null)
			{
				onSetPrivacyStateComplete(true);
			}
		}
		else
		{
			Action<bool> onSetPrivacyStateComplete2 = this.OnSetPrivacyStateComplete;
			if (onSetPrivacyStateComplete2 != null)
			{
				onSetPrivacyStateComplete2(false);
			}
		}
		if (this.setPrivacyStateQueue.Count > 0)
		{
			FriendBackendController.PrivacyState privacyState = this.setPrivacyStateQueue.Dequeue();
			this.SetPrivacyState(privacyState);
		}
	}

	// Token: 0x06003605 RID: 13829 RVA: 0x00105B90 File Offset: 0x00103D90
	private void AddFriendInternal()
	{
		base.StartCoroutine(this.SendAddFriendRequest(new FriendBackendController.FriendRequestRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			MothershipId = "",
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipToken = "",
			MyFriendLinkId = NetworkSystem.Instance.LocalPlayer.UserId,
			FriendFriendLinkId = this.addFriendTargetPlayer.UserId
		}, new Action<bool>(this.AddFriendComplete)));
	}

	// Token: 0x06003606 RID: 13830 RVA: 0x00105C1B File Offset: 0x00103E1B
	private IEnumerator SendAddFriendRequest(FriendBackendController.FriendRequestRequest data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/RequestFriend", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			callback(true);
		}
		else
		{
			if (request.responseCode == 409L)
			{
				flag = false;
			}
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.addFriendRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.addFriendRetryCount + 1));
				this.addFriendRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.AddFriendInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum AddFriend retries attempted. Please check your network connection.", null);
				this.addFriendRetryCount = 0;
				callback(false);
			}
		}
		else
		{
			this.addFriendInProgress = false;
		}
		yield break;
	}

	// Token: 0x06003607 RID: 13831 RVA: 0x00105C38 File Offset: 0x00103E38
	private void AddFriendComplete([CanBeNull] bool success)
	{
		if (success)
		{
			Action<NetPlayer, bool> onAddFriendComplete = this.OnAddFriendComplete;
			if (onAddFriendComplete != null)
			{
				onAddFriendComplete(this.addFriendTargetPlayer, true);
			}
		}
		else
		{
			Action<NetPlayer, bool> onAddFriendComplete2 = this.OnAddFriendComplete;
			if (onAddFriendComplete2 != null)
			{
				onAddFriendComplete2(this.addFriendTargetPlayer, false);
			}
		}
		this.addFriendInProgress = false;
		this.addFriendTargetIdHash = 0;
		this.addFriendTargetPlayer = null;
		if (this.addFriendRequestQueue.Count > 0)
		{
			ValueTuple<int, NetPlayer> valueTuple = this.addFriendRequestQueue.Dequeue();
			this.AddFriend(valueTuple.Item2);
		}
	}

	// Token: 0x06003608 RID: 13832 RVA: 0x00105CB8 File Offset: 0x00103EB8
	private void RemoveFriendInternal()
	{
		base.StartCoroutine(this.SendRemoveFriendRequest(new FriendBackendController.RemoveFriendRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			MothershipId = "",
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MyFriendLinkId = NetworkSystem.Instance.LocalPlayer.UserId,
			FriendFriendLinkId = this.removeFriendTarget.Presence.FriendLinkId
		}, new Action<bool>(this.RemoveFriendComplete)));
	}

	// Token: 0x06003609 RID: 13833 RVA: 0x00105D3D File Offset: 0x00103F3D
	private IEnumerator SendRemoveFriendRequest(FriendBackendController.RemoveFriendRequest data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/RemoveFriend", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == UnityWebRequest.Result.Success)
		{
			callback(true);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.removeFriendRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.removeFriendRetryCount + 1));
				this.removeFriendRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.AddFriendInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum AddFriend retries attempted. Please check your network connection.", null);
				this.removeFriendRetryCount = 0;
				callback(false);
			}
		}
		else
		{
			this.removeFriendInProgress = false;
		}
		yield break;
	}

	// Token: 0x0600360A RID: 13834 RVA: 0x00105D5C File Offset: 0x00103F5C
	private void RemoveFriendComplete([CanBeNull] bool success)
	{
		if (success)
		{
			Action<FriendBackendController.Friend, bool> onRemoveFriendComplete = this.OnRemoveFriendComplete;
			if (onRemoveFriendComplete != null)
			{
				onRemoveFriendComplete(this.removeFriendTarget, true);
			}
		}
		else
		{
			Action<FriendBackendController.Friend, bool> onRemoveFriendComplete2 = this.OnRemoveFriendComplete;
			if (onRemoveFriendComplete2 != null)
			{
				onRemoveFriendComplete2(this.removeFriendTarget, false);
			}
		}
		this.removeFriendInProgress = false;
		this.removeFriendTargetIdHash = 0;
		this.removeFriendTarget = null;
		if (this.removeFriendRequestQueue.Count > 0)
		{
			ValueTuple<int, FriendBackendController.Friend> valueTuple = this.removeFriendRequestQueue.Dequeue();
			this.RemoveFriend(valueTuple.Item2);
		}
	}

	// Token: 0x0600360B RID: 13835 RVA: 0x00105DDC File Offset: 0x00103FDC
	private void LogNetPlayersInRoom()
	{
		Debug.Log("Local Player PlayfabId: " + PlayFabAuthenticator.instance.GetPlayFabPlayerId());
		int num = 0;
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			Debug.Log(string.Format("[{0}] Player: {1}, ActorNumber: {2}, UserID: {3}, IsMasterClient: {4}", new object[] { num, netPlayer.NickName, netPlayer.ActorNumber, netPlayer.UserId, netPlayer.IsMasterClient }));
			num++;
		}
	}

	// Token: 0x0600360C RID: 13836 RVA: 0x00105E74 File Offset: 0x00104074
	private void TestAddFriend()
	{
		this.OnAddFriendComplete -= this.TestAddFriendCompleteCallback;
		this.OnAddFriendComplete += this.TestAddFriendCompleteCallback;
		NetPlayer netPlayer = null;
		if (this.netPlayerIndexToAddFriend >= 0 && this.netPlayerIndexToAddFriend < NetworkSystem.Instance.AllNetPlayers.Length)
		{
			netPlayer = NetworkSystem.Instance.AllNetPlayers[this.netPlayerIndexToAddFriend];
		}
		this.AddFriend(netPlayer);
	}

	// Token: 0x0600360D RID: 13837 RVA: 0x00105EDD File Offset: 0x001040DD
	private void TestAddFriendCompleteCallback(NetPlayer player, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = false");
	}

	// Token: 0x0600360E RID: 13838 RVA: 0x00105EF8 File Offset: 0x001040F8
	private void TestRemoveFriend()
	{
		this.OnRemoveFriendComplete -= this.TestRemoveFriendCompleteCallback;
		this.OnRemoveFriendComplete += this.TestRemoveFriendCompleteCallback;
		FriendBackendController.Friend friend = null;
		if (this.friendListIndexToRemoveFriend >= 0 && this.friendListIndexToRemoveFriend < this.FriendsList.Count)
		{
			friend = this.FriendsList[this.friendListIndexToRemoveFriend];
		}
		this.RemoveFriend(friend);
	}

	// Token: 0x0600360F RID: 13839 RVA: 0x00105F60 File Offset: 0x00104160
	private void TestRemoveFriendCompleteCallback(FriendBackendController.Friend friend, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = false");
	}

	// Token: 0x06003610 RID: 13840 RVA: 0x00105F7A File Offset: 0x0010417A
	private void TestGetFriends()
	{
		this.OnGetFriendsComplete -= this.TestGetFriendsCompleteCallback;
		this.OnGetFriendsComplete += this.TestGetFriendsCompleteCallback;
		this.GetFriends();
	}

	// Token: 0x06003611 RID: 13841 RVA: 0x00105FA8 File Offset: 0x001041A8
	private void TestGetFriendsCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestGetFriendsCompleteCallback returned with success = true");
			if (this.FriendsList != null)
			{
				string text = string.Format("Friend Count: {0} Friends: \n", this.FriendsList.Count);
				for (int i = 0; i < this.FriendsList.Count; i++)
				{
					if (this.FriendsList[i] != null && this.FriendsList[i].Presence != null)
					{
						text = string.Concat(new string[]
						{
							text,
							this.FriendsList[i].Presence.UserName,
							", ",
							this.FriendsList[i].Presence.FriendLinkId,
							", ",
							this.FriendsList[i].Presence.RoomId,
							", ",
							this.FriendsList[i].Presence.Region,
							", ",
							this.FriendsList[i].Presence.Zone,
							"\n"
						});
					}
					else
					{
						text += "null friend\n";
					}
				}
				Debug.Log(text);
				return;
			}
		}
		else
		{
			Debug.Log("FriendBackend: TestGetFriendsCompleteCallback returned with success = false");
		}
	}

	// Token: 0x06003612 RID: 13842 RVA: 0x00106105 File Offset: 0x00104305
	private void TestSetPrivacyState()
	{
		this.OnSetPrivacyStateComplete -= this.TestSetPrivacyStateCompleteCallback;
		this.OnSetPrivacyStateComplete += this.TestSetPrivacyStateCompleteCallback;
		this.SetPrivacyState(this.privacyStateToSet);
	}

	// Token: 0x06003613 RID: 13843 RVA: 0x00106138 File Offset: 0x00104338
	private void TestSetPrivacyStateCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log(string.Format("SetPrivacyState Success: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
			return;
		}
		Debug.Log(string.Format("SetPrivacyState Failed: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
	}

	// Token: 0x04003BC3 RID: 15299
	[OnEnterPlay_SetNull]
	public static volatile FriendBackendController Instance;

	// Token: 0x04003BC8 RID: 15304
	private int maxRetriesOnFail = 3;

	// Token: 0x04003BC9 RID: 15305
	private int getFriendsRetryCount;

	// Token: 0x04003BCA RID: 15306
	private int setPrivacyStateRetryCount;

	// Token: 0x04003BCB RID: 15307
	private int addFriendRetryCount;

	// Token: 0x04003BCC RID: 15308
	private int removeFriendRetryCount;

	// Token: 0x04003BCD RID: 15309
	private bool getFriendsInProgress;

	// Token: 0x04003BCE RID: 15310
	private FriendBackendController.GetFriendsResponse lastGetFriendsResponse;

	// Token: 0x04003BCF RID: 15311
	private List<FriendBackendController.Friend> lastFriendsList = new List<FriendBackendController.Friend>();

	// Token: 0x04003BD0 RID: 15312
	private bool setPrivacyStateInProgress;

	// Token: 0x04003BD1 RID: 15313
	private FriendBackendController.PrivacyState setPrivacyStateState;

	// Token: 0x04003BD2 RID: 15314
	private FriendBackendController.SetPrivacyStateResponse lastPrivacyStateResponse;

	// Token: 0x04003BD3 RID: 15315
	private Queue<FriendBackendController.PrivacyState> setPrivacyStateQueue = new Queue<FriendBackendController.PrivacyState>();

	// Token: 0x04003BD4 RID: 15316
	private FriendBackendController.PrivacyState lastPrivacyState;

	// Token: 0x04003BD5 RID: 15317
	private bool addFriendInProgress;

	// Token: 0x04003BD6 RID: 15318
	private int addFriendTargetIdHash;

	// Token: 0x04003BD7 RID: 15319
	private NetPlayer addFriendTargetPlayer;

	// Token: 0x04003BD8 RID: 15320
	private Queue<ValueTuple<int, NetPlayer>> addFriendRequestQueue = new Queue<ValueTuple<int, NetPlayer>>();

	// Token: 0x04003BD9 RID: 15321
	private bool removeFriendInProgress;

	// Token: 0x04003BDA RID: 15322
	private int removeFriendTargetIdHash;

	// Token: 0x04003BDB RID: 15323
	private FriendBackendController.Friend removeFriendTarget;

	// Token: 0x04003BDC RID: 15324
	private Queue<ValueTuple<int, FriendBackendController.Friend>> removeFriendRequestQueue = new Queue<ValueTuple<int, FriendBackendController.Friend>>();

	// Token: 0x04003BDD RID: 15325
	[SerializeField]
	private int netPlayerIndexToAddFriend;

	// Token: 0x04003BDE RID: 15326
	[SerializeField]
	private int friendListIndexToRemoveFriend;

	// Token: 0x04003BDF RID: 15327
	[SerializeField]
	private FriendBackendController.PrivacyState privacyStateToSet;

	// Token: 0x020008B7 RID: 2231
	public class Friend
	{
		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06003615 RID: 13845 RVA: 0x001061D8 File Offset: 0x001043D8
		// (set) Token: 0x06003616 RID: 13846 RVA: 0x001061E0 File Offset: 0x001043E0
		public FriendBackendController.FriendPresence Presence { get; set; }

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06003617 RID: 13847 RVA: 0x001061E9 File Offset: 0x001043E9
		// (set) Token: 0x06003618 RID: 13848 RVA: 0x001061F1 File Offset: 0x001043F1
		public DateTime Created { get; set; }
	}

	// Token: 0x020008B8 RID: 2232
	public class FriendPresence
	{
		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x0600361A RID: 13850 RVA: 0x001061FA File Offset: 0x001043FA
		// (set) Token: 0x0600361B RID: 13851 RVA: 0x00106202 File Offset: 0x00104402
		public string FriendLinkId { get; set; }

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x0600361C RID: 13852 RVA: 0x0010620B File Offset: 0x0010440B
		// (set) Token: 0x0600361D RID: 13853 RVA: 0x00106213 File Offset: 0x00104413
		public string UserName { get; set; }

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x0600361E RID: 13854 RVA: 0x0010621C File Offset: 0x0010441C
		// (set) Token: 0x0600361F RID: 13855 RVA: 0x00106224 File Offset: 0x00104424
		public string RoomId { get; set; }

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06003620 RID: 13856 RVA: 0x0010622D File Offset: 0x0010442D
		// (set) Token: 0x06003621 RID: 13857 RVA: 0x00106235 File Offset: 0x00104435
		public string Zone { get; set; }

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06003622 RID: 13858 RVA: 0x0010623E File Offset: 0x0010443E
		// (set) Token: 0x06003623 RID: 13859 RVA: 0x00106246 File Offset: 0x00104446
		public string Region { get; set; }

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06003624 RID: 13860 RVA: 0x0010624F File Offset: 0x0010444F
		// (set) Token: 0x06003625 RID: 13861 RVA: 0x00106257 File Offset: 0x00104457
		public bool? IsPublic { get; set; }
	}

	// Token: 0x020008B9 RID: 2233
	public class FriendLink
	{
		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06003627 RID: 13863 RVA: 0x00106260 File Offset: 0x00104460
		// (set) Token: 0x06003628 RID: 13864 RVA: 0x00106268 File Offset: 0x00104468
		public string my_playfab_id { get; set; }

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06003629 RID: 13865 RVA: 0x00106271 File Offset: 0x00104471
		// (set) Token: 0x0600362A RID: 13866 RVA: 0x00106279 File Offset: 0x00104479
		public string my_mothership_id { get; set; }

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x0600362B RID: 13867 RVA: 0x00106282 File Offset: 0x00104482
		// (set) Token: 0x0600362C RID: 13868 RVA: 0x0010628A File Offset: 0x0010448A
		public string my_friendlink_id { get; set; }

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x0600362D RID: 13869 RVA: 0x00106293 File Offset: 0x00104493
		// (set) Token: 0x0600362E RID: 13870 RVA: 0x0010629B File Offset: 0x0010449B
		public string friend_playfab_id { get; set; }

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x0600362F RID: 13871 RVA: 0x001062A4 File Offset: 0x001044A4
		// (set) Token: 0x06003630 RID: 13872 RVA: 0x001062AC File Offset: 0x001044AC
		public string friend_mothership_id { get; set; }

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06003631 RID: 13873 RVA: 0x001062B5 File Offset: 0x001044B5
		// (set) Token: 0x06003632 RID: 13874 RVA: 0x001062BD File Offset: 0x001044BD
		public string friend_friendlink_id { get; set; }

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06003633 RID: 13875 RVA: 0x001062C6 File Offset: 0x001044C6
		// (set) Token: 0x06003634 RID: 13876 RVA: 0x001062CE File Offset: 0x001044CE
		public DateTime created { get; set; }
	}

	// Token: 0x020008BA RID: 2234
	[NullableContext(2)]
	[Nullable(0)]
	public class FriendIdResponse
	{
		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06003636 RID: 13878 RVA: 0x001062D7 File Offset: 0x001044D7
		// (set) Token: 0x06003637 RID: 13879 RVA: 0x001062DF File Offset: 0x001044DF
		public string PlayFabId { get; set; }

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06003638 RID: 13880 RVA: 0x001062E8 File Offset: 0x001044E8
		// (set) Token: 0x06003639 RID: 13881 RVA: 0x001062F0 File Offset: 0x001044F0
		public string MothershipId { get; set; } = "";
	}

	// Token: 0x020008BB RID: 2235
	public class FriendRequestRequest
	{
		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x0600363B RID: 13883 RVA: 0x0010630C File Offset: 0x0010450C
		// (set) Token: 0x0600363C RID: 13884 RVA: 0x00106314 File Offset: 0x00104514
		public string PlayFabId { get; set; }

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x0600363D RID: 13885 RVA: 0x0010631D File Offset: 0x0010451D
		// (set) Token: 0x0600363E RID: 13886 RVA: 0x00106325 File Offset: 0x00104525
		public string MothershipId { get; set; } = "";

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x0600363F RID: 13887 RVA: 0x0010632E File Offset: 0x0010452E
		// (set) Token: 0x06003640 RID: 13888 RVA: 0x00106336 File Offset: 0x00104536
		public string PlayFabTicket { get; set; }

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06003641 RID: 13889 RVA: 0x0010633F File Offset: 0x0010453F
		// (set) Token: 0x06003642 RID: 13890 RVA: 0x00106347 File Offset: 0x00104547
		public string MothershipToken { get; set; }

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06003643 RID: 13891 RVA: 0x00106350 File Offset: 0x00104550
		// (set) Token: 0x06003644 RID: 13892 RVA: 0x00106358 File Offset: 0x00104558
		public string MyFriendLinkId { get; set; }

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06003645 RID: 13893 RVA: 0x00106361 File Offset: 0x00104561
		// (set) Token: 0x06003646 RID: 13894 RVA: 0x00106369 File Offset: 0x00104569
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x020008BC RID: 2236
	public class GetFriendsRequest
	{
		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06003648 RID: 13896 RVA: 0x00106385 File Offset: 0x00104585
		// (set) Token: 0x06003649 RID: 13897 RVA: 0x0010638D File Offset: 0x0010458D
		public string PlayFabId { get; set; }

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x0600364A RID: 13898 RVA: 0x00106396 File Offset: 0x00104596
		// (set) Token: 0x0600364B RID: 13899 RVA: 0x0010639E File Offset: 0x0010459E
		public string MothershipId { get; set; } = "";

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x0600364C RID: 13900 RVA: 0x001063A7 File Offset: 0x001045A7
		// (set) Token: 0x0600364D RID: 13901 RVA: 0x001063AF File Offset: 0x001045AF
		public string MothershipToken { get; set; }

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x0600364E RID: 13902 RVA: 0x001063B8 File Offset: 0x001045B8
		// (set) Token: 0x0600364F RID: 13903 RVA: 0x001063C0 File Offset: 0x001045C0
		public string PlayFabTicket { get; set; }
	}

	// Token: 0x020008BD RID: 2237
	public class GetFriendsResponse
	{
		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06003651 RID: 13905 RVA: 0x001063DC File Offset: 0x001045DC
		// (set) Token: 0x06003652 RID: 13906 RVA: 0x001063E4 File Offset: 0x001045E4
		[CanBeNull]
		public FriendBackendController.GetFriendsResult Result { get; set; }

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x06003653 RID: 13907 RVA: 0x001063ED File Offset: 0x001045ED
		// (set) Token: 0x06003654 RID: 13908 RVA: 0x001063F5 File Offset: 0x001045F5
		public int StatusCode { get; set; }

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06003655 RID: 13909 RVA: 0x001063FE File Offset: 0x001045FE
		// (set) Token: 0x06003656 RID: 13910 RVA: 0x00106406 File Offset: 0x00104606
		[Nullable(2)]
		public string Error
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}
	}

	// Token: 0x020008BE RID: 2238
	public class GetFriendsResult
	{
		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x06003658 RID: 13912 RVA: 0x0010640F File Offset: 0x0010460F
		// (set) Token: 0x06003659 RID: 13913 RVA: 0x00106417 File Offset: 0x00104617
		public List<FriendBackendController.Friend> Friends { get; set; }

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x0600365A RID: 13914 RVA: 0x00106420 File Offset: 0x00104620
		// (set) Token: 0x0600365B RID: 13915 RVA: 0x00106428 File Offset: 0x00104628
		public FriendBackendController.PrivacyState MyPrivacyState { get; set; }
	}

	// Token: 0x020008BF RID: 2239
	public class SetPrivacyStateRequest
	{
		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x0600365D RID: 13917 RVA: 0x00106431 File Offset: 0x00104631
		// (set) Token: 0x0600365E RID: 13918 RVA: 0x00106439 File Offset: 0x00104639
		public string PlayFabId { get; set; }

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x0600365F RID: 13919 RVA: 0x00106442 File Offset: 0x00104642
		// (set) Token: 0x06003660 RID: 13920 RVA: 0x0010644A File Offset: 0x0010464A
		public string PlayFabTicket { get; set; }

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06003661 RID: 13921 RVA: 0x00106453 File Offset: 0x00104653
		// (set) Token: 0x06003662 RID: 13922 RVA: 0x0010645B File Offset: 0x0010465B
		public string PrivacyState { get; set; }
	}

	// Token: 0x020008C0 RID: 2240
	[NullableContext(2)]
	[Nullable(0)]
	public class SetPrivacyStateResponse
	{
		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06003664 RID: 13924 RVA: 0x00106464 File Offset: 0x00104664
		// (set) Token: 0x06003665 RID: 13925 RVA: 0x0010646C File Offset: 0x0010466C
		public int StatusCode { get; set; }

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06003666 RID: 13926 RVA: 0x00106475 File Offset: 0x00104675
		// (set) Token: 0x06003667 RID: 13927 RVA: 0x0010647D File Offset: 0x0010467D
		public string Error { get; set; }
	}

	// Token: 0x020008C1 RID: 2241
	public class RemoveFriendRequest
	{
		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06003669 RID: 13929 RVA: 0x00106486 File Offset: 0x00104686
		// (set) Token: 0x0600366A RID: 13930 RVA: 0x0010648E File Offset: 0x0010468E
		public string PlayFabId { get; set; }

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x0600366B RID: 13931 RVA: 0x00106497 File Offset: 0x00104697
		// (set) Token: 0x0600366C RID: 13932 RVA: 0x0010649F File Offset: 0x0010469F
		public string MothershipId { get; set; } = "";

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x0600366D RID: 13933 RVA: 0x001064A8 File Offset: 0x001046A8
		// (set) Token: 0x0600366E RID: 13934 RVA: 0x001064B0 File Offset: 0x001046B0
		public string PlayFabTicket { get; set; }

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x0600366F RID: 13935 RVA: 0x001064B9 File Offset: 0x001046B9
		// (set) Token: 0x06003670 RID: 13936 RVA: 0x001064C1 File Offset: 0x001046C1
		public string MothershipToken { get; set; }

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06003671 RID: 13937 RVA: 0x001064CA File Offset: 0x001046CA
		// (set) Token: 0x06003672 RID: 13938 RVA: 0x001064D2 File Offset: 0x001046D2
		public string MyFriendLinkId { get; set; }

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06003673 RID: 13939 RVA: 0x001064DB File Offset: 0x001046DB
		// (set) Token: 0x06003674 RID: 13940 RVA: 0x001064E3 File Offset: 0x001046E3
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x020008C2 RID: 2242
	public enum PendingRequestStatus
	{
		// Token: 0x04003C0C RID: 15372
		I_REQUESTED,
		// Token: 0x04003C0D RID: 15373
		THEY_REQUESTED,
		// Token: 0x04003C0E RID: 15374
		CONFIRMED,
		// Token: 0x04003C0F RID: 15375
		NOT_FOUND
	}

	// Token: 0x020008C3 RID: 2243
	public enum PrivacyState
	{
		// Token: 0x04003C11 RID: 15377
		VISIBLE,
		// Token: 0x04003C12 RID: 15378
		PUBLIC_ONLY,
		// Token: 0x04003C13 RID: 15379
		HIDDEN
	}
}
