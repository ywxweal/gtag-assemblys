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
	// (add) Token: 0x060035EF RID: 13807 RVA: 0x00105560 File Offset: 0x00103760
	// (remove) Token: 0x060035F0 RID: 13808 RVA: 0x00105598 File Offset: 0x00103798
	public event Action<bool> OnGetFriendsComplete;

	// Token: 0x1400005F RID: 95
	// (add) Token: 0x060035F1 RID: 13809 RVA: 0x001055D0 File Offset: 0x001037D0
	// (remove) Token: 0x060035F2 RID: 13810 RVA: 0x00105608 File Offset: 0x00103808
	public event Action<bool> OnSetPrivacyStateComplete;

	// Token: 0x14000060 RID: 96
	// (add) Token: 0x060035F3 RID: 13811 RVA: 0x00105640 File Offset: 0x00103840
	// (remove) Token: 0x060035F4 RID: 13812 RVA: 0x00105678 File Offset: 0x00103878
	public event Action<NetPlayer, bool> OnAddFriendComplete;

	// Token: 0x14000061 RID: 97
	// (add) Token: 0x060035F5 RID: 13813 RVA: 0x001056B0 File Offset: 0x001038B0
	// (remove) Token: 0x060035F6 RID: 13814 RVA: 0x001056E8 File Offset: 0x001038E8
	public event Action<FriendBackendController.Friend, bool> OnRemoveFriendComplete;

	// Token: 0x17000537 RID: 1335
	// (get) Token: 0x060035F7 RID: 13815 RVA: 0x0010571D File Offset: 0x0010391D
	public List<FriendBackendController.Friend> FriendsList
	{
		get
		{
			return this.lastFriendsList;
		}
	}

	// Token: 0x17000538 RID: 1336
	// (get) Token: 0x060035F8 RID: 13816 RVA: 0x00105725 File Offset: 0x00103925
	public FriendBackendController.PrivacyState MyPrivacyState
	{
		get
		{
			return this.lastPrivacyState;
		}
	}

	// Token: 0x060035F9 RID: 13817 RVA: 0x0010572D File Offset: 0x0010392D
	public void GetFriends()
	{
		if (!this.getFriendsInProgress)
		{
			this.getFriendsInProgress = true;
			this.GetFriendsInternal();
		}
	}

	// Token: 0x060035FA RID: 13818 RVA: 0x00105744 File Offset: 0x00103944
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

	// Token: 0x060035FB RID: 13819 RVA: 0x00105770 File Offset: 0x00103970
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

	// Token: 0x060035FC RID: 13820 RVA: 0x001057E0 File Offset: 0x001039E0
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

	// Token: 0x060035FD RID: 13821 RVA: 0x00105855 File Offset: 0x00103A55
	private void Awake()
	{
		if (FriendBackendController.Instance == null)
		{
			FriendBackendController.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060035FE RID: 13822 RVA: 0x00105878 File Offset: 0x00103A78
	private void GetFriendsInternal()
	{
		base.StartCoroutine(this.SendGetFriendsRequest(new FriendBackendController.GetFriendsRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = ""
		}, new Action<FriendBackendController.GetFriendsResponse>(this.GetFriendsComplete)));
	}

	// Token: 0x060035FF RID: 13823 RVA: 0x001058D2 File Offset: 0x00103AD2
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

	// Token: 0x06003600 RID: 13824 RVA: 0x001058F0 File Offset: 0x00103AF0
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

	// Token: 0x06003601 RID: 13825 RVA: 0x001059CC File Offset: 0x00103BCC
	private void SetPrivacyStateInternal()
	{
		base.StartCoroutine(this.SendSetPrivacyStateRequest(new FriendBackendController.SetPrivacyStateRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			PrivacyState = this.setPrivacyStateState.ToString()
		}, new Action<FriendBackendController.SetPrivacyStateResponse>(this.SetPrivacyStateComplete)));
	}

	// Token: 0x06003602 RID: 13826 RVA: 0x00105A32 File Offset: 0x00103C32
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

	// Token: 0x06003603 RID: 13827 RVA: 0x00105A50 File Offset: 0x00103C50
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

	// Token: 0x06003604 RID: 13828 RVA: 0x00105AB8 File Offset: 0x00103CB8
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

	// Token: 0x06003605 RID: 13829 RVA: 0x00105B43 File Offset: 0x00103D43
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

	// Token: 0x06003606 RID: 13830 RVA: 0x00105B60 File Offset: 0x00103D60
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

	// Token: 0x06003607 RID: 13831 RVA: 0x00105BE0 File Offset: 0x00103DE0
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

	// Token: 0x06003608 RID: 13832 RVA: 0x00105C65 File Offset: 0x00103E65
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

	// Token: 0x06003609 RID: 13833 RVA: 0x00105C84 File Offset: 0x00103E84
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

	// Token: 0x0600360A RID: 13834 RVA: 0x00105D04 File Offset: 0x00103F04
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

	// Token: 0x0600360B RID: 13835 RVA: 0x00105D9C File Offset: 0x00103F9C
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

	// Token: 0x0600360C RID: 13836 RVA: 0x00105E05 File Offset: 0x00104005
	private void TestAddFriendCompleteCallback(NetPlayer player, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = false");
	}

	// Token: 0x0600360D RID: 13837 RVA: 0x00105E20 File Offset: 0x00104020
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

	// Token: 0x0600360E RID: 13838 RVA: 0x00105E88 File Offset: 0x00104088
	private void TestRemoveFriendCompleteCallback(FriendBackendController.Friend friend, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = false");
	}

	// Token: 0x0600360F RID: 13839 RVA: 0x00105EA2 File Offset: 0x001040A2
	private void TestGetFriends()
	{
		this.OnGetFriendsComplete -= this.TestGetFriendsCompleteCallback;
		this.OnGetFriendsComplete += this.TestGetFriendsCompleteCallback;
		this.GetFriends();
	}

	// Token: 0x06003610 RID: 13840 RVA: 0x00105ED0 File Offset: 0x001040D0
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

	// Token: 0x06003611 RID: 13841 RVA: 0x0010602D File Offset: 0x0010422D
	private void TestSetPrivacyState()
	{
		this.OnSetPrivacyStateComplete -= this.TestSetPrivacyStateCompleteCallback;
		this.OnSetPrivacyStateComplete += this.TestSetPrivacyStateCompleteCallback;
		this.SetPrivacyState(this.privacyStateToSet);
	}

	// Token: 0x06003612 RID: 13842 RVA: 0x00106060 File Offset: 0x00104260
	private void TestSetPrivacyStateCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log(string.Format("SetPrivacyState Success: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
			return;
		}
		Debug.Log(string.Format("SetPrivacyState Failed: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
	}

	// Token: 0x04003BC2 RID: 15298
	[OnEnterPlay_SetNull]
	public static volatile FriendBackendController Instance;

	// Token: 0x04003BC7 RID: 15303
	private int maxRetriesOnFail = 3;

	// Token: 0x04003BC8 RID: 15304
	private int getFriendsRetryCount;

	// Token: 0x04003BC9 RID: 15305
	private int setPrivacyStateRetryCount;

	// Token: 0x04003BCA RID: 15306
	private int addFriendRetryCount;

	// Token: 0x04003BCB RID: 15307
	private int removeFriendRetryCount;

	// Token: 0x04003BCC RID: 15308
	private bool getFriendsInProgress;

	// Token: 0x04003BCD RID: 15309
	private FriendBackendController.GetFriendsResponse lastGetFriendsResponse;

	// Token: 0x04003BCE RID: 15310
	private List<FriendBackendController.Friend> lastFriendsList = new List<FriendBackendController.Friend>();

	// Token: 0x04003BCF RID: 15311
	private bool setPrivacyStateInProgress;

	// Token: 0x04003BD0 RID: 15312
	private FriendBackendController.PrivacyState setPrivacyStateState;

	// Token: 0x04003BD1 RID: 15313
	private FriendBackendController.SetPrivacyStateResponse lastPrivacyStateResponse;

	// Token: 0x04003BD2 RID: 15314
	private Queue<FriendBackendController.PrivacyState> setPrivacyStateQueue = new Queue<FriendBackendController.PrivacyState>();

	// Token: 0x04003BD3 RID: 15315
	private FriendBackendController.PrivacyState lastPrivacyState;

	// Token: 0x04003BD4 RID: 15316
	private bool addFriendInProgress;

	// Token: 0x04003BD5 RID: 15317
	private int addFriendTargetIdHash;

	// Token: 0x04003BD6 RID: 15318
	private NetPlayer addFriendTargetPlayer;

	// Token: 0x04003BD7 RID: 15319
	private Queue<ValueTuple<int, NetPlayer>> addFriendRequestQueue = new Queue<ValueTuple<int, NetPlayer>>();

	// Token: 0x04003BD8 RID: 15320
	private bool removeFriendInProgress;

	// Token: 0x04003BD9 RID: 15321
	private int removeFriendTargetIdHash;

	// Token: 0x04003BDA RID: 15322
	private FriendBackendController.Friend removeFriendTarget;

	// Token: 0x04003BDB RID: 15323
	private Queue<ValueTuple<int, FriendBackendController.Friend>> removeFriendRequestQueue = new Queue<ValueTuple<int, FriendBackendController.Friend>>();

	// Token: 0x04003BDC RID: 15324
	[SerializeField]
	private int netPlayerIndexToAddFriend;

	// Token: 0x04003BDD RID: 15325
	[SerializeField]
	private int friendListIndexToRemoveFriend;

	// Token: 0x04003BDE RID: 15326
	[SerializeField]
	private FriendBackendController.PrivacyState privacyStateToSet;

	// Token: 0x020008B7 RID: 2231
	public class Friend
	{
		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06003614 RID: 13844 RVA: 0x00106100 File Offset: 0x00104300
		// (set) Token: 0x06003615 RID: 13845 RVA: 0x00106108 File Offset: 0x00104308
		public FriendBackendController.FriendPresence Presence { get; set; }

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06003616 RID: 13846 RVA: 0x00106111 File Offset: 0x00104311
		// (set) Token: 0x06003617 RID: 13847 RVA: 0x00106119 File Offset: 0x00104319
		public DateTime Created { get; set; }
	}

	// Token: 0x020008B8 RID: 2232
	public class FriendPresence
	{
		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06003619 RID: 13849 RVA: 0x00106122 File Offset: 0x00104322
		// (set) Token: 0x0600361A RID: 13850 RVA: 0x0010612A File Offset: 0x0010432A
		public string FriendLinkId { get; set; }

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x0600361B RID: 13851 RVA: 0x00106133 File Offset: 0x00104333
		// (set) Token: 0x0600361C RID: 13852 RVA: 0x0010613B File Offset: 0x0010433B
		public string UserName { get; set; }

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x0600361D RID: 13853 RVA: 0x00106144 File Offset: 0x00104344
		// (set) Token: 0x0600361E RID: 13854 RVA: 0x0010614C File Offset: 0x0010434C
		public string RoomId { get; set; }

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x0600361F RID: 13855 RVA: 0x00106155 File Offset: 0x00104355
		// (set) Token: 0x06003620 RID: 13856 RVA: 0x0010615D File Offset: 0x0010435D
		public string Zone { get; set; }

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06003621 RID: 13857 RVA: 0x00106166 File Offset: 0x00104366
		// (set) Token: 0x06003622 RID: 13858 RVA: 0x0010616E File Offset: 0x0010436E
		public string Region { get; set; }

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06003623 RID: 13859 RVA: 0x00106177 File Offset: 0x00104377
		// (set) Token: 0x06003624 RID: 13860 RVA: 0x0010617F File Offset: 0x0010437F
		public bool? IsPublic { get; set; }
	}

	// Token: 0x020008B9 RID: 2233
	public class FriendLink
	{
		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06003626 RID: 13862 RVA: 0x00106188 File Offset: 0x00104388
		// (set) Token: 0x06003627 RID: 13863 RVA: 0x00106190 File Offset: 0x00104390
		public string my_playfab_id { get; set; }

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06003628 RID: 13864 RVA: 0x00106199 File Offset: 0x00104399
		// (set) Token: 0x06003629 RID: 13865 RVA: 0x001061A1 File Offset: 0x001043A1
		public string my_mothership_id { get; set; }

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x0600362A RID: 13866 RVA: 0x001061AA File Offset: 0x001043AA
		// (set) Token: 0x0600362B RID: 13867 RVA: 0x001061B2 File Offset: 0x001043B2
		public string my_friendlink_id { get; set; }

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x0600362C RID: 13868 RVA: 0x001061BB File Offset: 0x001043BB
		// (set) Token: 0x0600362D RID: 13869 RVA: 0x001061C3 File Offset: 0x001043C3
		public string friend_playfab_id { get; set; }

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x0600362E RID: 13870 RVA: 0x001061CC File Offset: 0x001043CC
		// (set) Token: 0x0600362F RID: 13871 RVA: 0x001061D4 File Offset: 0x001043D4
		public string friend_mothership_id { get; set; }

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06003630 RID: 13872 RVA: 0x001061DD File Offset: 0x001043DD
		// (set) Token: 0x06003631 RID: 13873 RVA: 0x001061E5 File Offset: 0x001043E5
		public string friend_friendlink_id { get; set; }

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06003632 RID: 13874 RVA: 0x001061EE File Offset: 0x001043EE
		// (set) Token: 0x06003633 RID: 13875 RVA: 0x001061F6 File Offset: 0x001043F6
		public DateTime created { get; set; }
	}

	// Token: 0x020008BA RID: 2234
	[NullableContext(2)]
	[Nullable(0)]
	public class FriendIdResponse
	{
		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06003635 RID: 13877 RVA: 0x001061FF File Offset: 0x001043FF
		// (set) Token: 0x06003636 RID: 13878 RVA: 0x00106207 File Offset: 0x00104407
		public string PlayFabId { get; set; }

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06003637 RID: 13879 RVA: 0x00106210 File Offset: 0x00104410
		// (set) Token: 0x06003638 RID: 13880 RVA: 0x00106218 File Offset: 0x00104418
		public string MothershipId { get; set; } = "";
	}

	// Token: 0x020008BB RID: 2235
	public class FriendRequestRequest
	{
		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x0600363A RID: 13882 RVA: 0x00106234 File Offset: 0x00104434
		// (set) Token: 0x0600363B RID: 13883 RVA: 0x0010623C File Offset: 0x0010443C
		public string PlayFabId { get; set; }

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x0600363C RID: 13884 RVA: 0x00106245 File Offset: 0x00104445
		// (set) Token: 0x0600363D RID: 13885 RVA: 0x0010624D File Offset: 0x0010444D
		public string MothershipId { get; set; } = "";

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x0600363E RID: 13886 RVA: 0x00106256 File Offset: 0x00104456
		// (set) Token: 0x0600363F RID: 13887 RVA: 0x0010625E File Offset: 0x0010445E
		public string PlayFabTicket { get; set; }

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06003640 RID: 13888 RVA: 0x00106267 File Offset: 0x00104467
		// (set) Token: 0x06003641 RID: 13889 RVA: 0x0010626F File Offset: 0x0010446F
		public string MothershipToken { get; set; }

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06003642 RID: 13890 RVA: 0x00106278 File Offset: 0x00104478
		// (set) Token: 0x06003643 RID: 13891 RVA: 0x00106280 File Offset: 0x00104480
		public string MyFriendLinkId { get; set; }

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06003644 RID: 13892 RVA: 0x00106289 File Offset: 0x00104489
		// (set) Token: 0x06003645 RID: 13893 RVA: 0x00106291 File Offset: 0x00104491
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x020008BC RID: 2236
	public class GetFriendsRequest
	{
		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06003647 RID: 13895 RVA: 0x001062AD File Offset: 0x001044AD
		// (set) Token: 0x06003648 RID: 13896 RVA: 0x001062B5 File Offset: 0x001044B5
		public string PlayFabId { get; set; }

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06003649 RID: 13897 RVA: 0x001062BE File Offset: 0x001044BE
		// (set) Token: 0x0600364A RID: 13898 RVA: 0x001062C6 File Offset: 0x001044C6
		public string MothershipId { get; set; } = "";

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x0600364B RID: 13899 RVA: 0x001062CF File Offset: 0x001044CF
		// (set) Token: 0x0600364C RID: 13900 RVA: 0x001062D7 File Offset: 0x001044D7
		public string MothershipToken { get; set; }

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x0600364D RID: 13901 RVA: 0x001062E0 File Offset: 0x001044E0
		// (set) Token: 0x0600364E RID: 13902 RVA: 0x001062E8 File Offset: 0x001044E8
		public string PlayFabTicket { get; set; }
	}

	// Token: 0x020008BD RID: 2237
	public class GetFriendsResponse
	{
		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06003650 RID: 13904 RVA: 0x00106304 File Offset: 0x00104504
		// (set) Token: 0x06003651 RID: 13905 RVA: 0x0010630C File Offset: 0x0010450C
		[CanBeNull]
		public FriendBackendController.GetFriendsResult Result { get; set; }

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x06003652 RID: 13906 RVA: 0x00106315 File Offset: 0x00104515
		// (set) Token: 0x06003653 RID: 13907 RVA: 0x0010631D File Offset: 0x0010451D
		public int StatusCode { get; set; }

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06003654 RID: 13908 RVA: 0x00106326 File Offset: 0x00104526
		// (set) Token: 0x06003655 RID: 13909 RVA: 0x0010632E File Offset: 0x0010452E
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
		// (get) Token: 0x06003657 RID: 13911 RVA: 0x00106337 File Offset: 0x00104537
		// (set) Token: 0x06003658 RID: 13912 RVA: 0x0010633F File Offset: 0x0010453F
		public List<FriendBackendController.Friend> Friends { get; set; }

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06003659 RID: 13913 RVA: 0x00106348 File Offset: 0x00104548
		// (set) Token: 0x0600365A RID: 13914 RVA: 0x00106350 File Offset: 0x00104550
		public FriendBackendController.PrivacyState MyPrivacyState { get; set; }
	}

	// Token: 0x020008BF RID: 2239
	public class SetPrivacyStateRequest
	{
		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x0600365C RID: 13916 RVA: 0x00106359 File Offset: 0x00104559
		// (set) Token: 0x0600365D RID: 13917 RVA: 0x00106361 File Offset: 0x00104561
		public string PlayFabId { get; set; }

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x0600365E RID: 13918 RVA: 0x0010636A File Offset: 0x0010456A
		// (set) Token: 0x0600365F RID: 13919 RVA: 0x00106372 File Offset: 0x00104572
		public string PlayFabTicket { get; set; }

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06003660 RID: 13920 RVA: 0x0010637B File Offset: 0x0010457B
		// (set) Token: 0x06003661 RID: 13921 RVA: 0x00106383 File Offset: 0x00104583
		public string PrivacyState { get; set; }
	}

	// Token: 0x020008C0 RID: 2240
	[NullableContext(2)]
	[Nullable(0)]
	public class SetPrivacyStateResponse
	{
		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06003663 RID: 13923 RVA: 0x0010638C File Offset: 0x0010458C
		// (set) Token: 0x06003664 RID: 13924 RVA: 0x00106394 File Offset: 0x00104594
		public int StatusCode { get; set; }

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06003665 RID: 13925 RVA: 0x0010639D File Offset: 0x0010459D
		// (set) Token: 0x06003666 RID: 13926 RVA: 0x001063A5 File Offset: 0x001045A5
		public string Error { get; set; }
	}

	// Token: 0x020008C1 RID: 2241
	public class RemoveFriendRequest
	{
		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06003668 RID: 13928 RVA: 0x001063AE File Offset: 0x001045AE
		// (set) Token: 0x06003669 RID: 13929 RVA: 0x001063B6 File Offset: 0x001045B6
		public string PlayFabId { get; set; }

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x0600366A RID: 13930 RVA: 0x001063BF File Offset: 0x001045BF
		// (set) Token: 0x0600366B RID: 13931 RVA: 0x001063C7 File Offset: 0x001045C7
		public string MothershipId { get; set; } = "";

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x0600366C RID: 13932 RVA: 0x001063D0 File Offset: 0x001045D0
		// (set) Token: 0x0600366D RID: 13933 RVA: 0x001063D8 File Offset: 0x001045D8
		public string PlayFabTicket { get; set; }

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x0600366E RID: 13934 RVA: 0x001063E1 File Offset: 0x001045E1
		// (set) Token: 0x0600366F RID: 13935 RVA: 0x001063E9 File Offset: 0x001045E9
		public string MothershipToken { get; set; }

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06003670 RID: 13936 RVA: 0x001063F2 File Offset: 0x001045F2
		// (set) Token: 0x06003671 RID: 13937 RVA: 0x001063FA File Offset: 0x001045FA
		public string MyFriendLinkId { get; set; }

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06003672 RID: 13938 RVA: 0x00106403 File Offset: 0x00104603
		// (set) Token: 0x06003673 RID: 13939 RVA: 0x0010640B File Offset: 0x0010460B
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x020008C2 RID: 2242
	public enum PendingRequestStatus
	{
		// Token: 0x04003C0B RID: 15371
		I_REQUESTED,
		// Token: 0x04003C0C RID: 15372
		THEY_REQUESTED,
		// Token: 0x04003C0D RID: 15373
		CONFIRMED,
		// Token: 0x04003C0E RID: 15374
		NOT_FOUND
	}

	// Token: 0x020008C3 RID: 2243
	public enum PrivacyState
	{
		// Token: 0x04003C10 RID: 15376
		VISIBLE,
		// Token: 0x04003C11 RID: 15377
		PUBLIC_ONLY,
		// Token: 0x04003C12 RID: 15378
		HIDDEN
	}
}
