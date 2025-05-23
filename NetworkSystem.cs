using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using GorillaNetworking;
using Photon.Realtime;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;

// Token: 0x020002AA RID: 682
public abstract class NetworkSystem : MonoBehaviour
{
	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06000FF7 RID: 4087 RVA: 0x0004F304 File Offset: 0x0004D504
	// (set) Token: 0x06000FF8 RID: 4088 RVA: 0x0004F30C File Offset: 0x0004D50C
	public bool groupJoinInProgress { get; protected set; }

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x0004F315 File Offset: 0x0004D515
	// (set) Token: 0x06000FFA RID: 4090 RVA: 0x0004F31D File Offset: 0x0004D51D
	public NetSystemState netState
	{
		get
		{
			return this.testState;
		}
		protected set
		{
			Debug.Log("netstate set to:" + value.ToString());
			this.testState = value;
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06000FFB RID: 4091 RVA: 0x0004F342 File Offset: 0x0004D542
	public NetPlayer LocalPlayer
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsLocal);
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x06000FFC RID: 4092 RVA: 0x0004F36E File Offset: 0x0004D56E
	public virtual bool IsMasterClient { get; }

	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x06000FFD RID: 4093 RVA: 0x0004F376 File Offset: 0x0004D576
	public virtual NetPlayer MasterClient
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsMasterClient);
		}
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x06000FFE RID: 4094 RVA: 0x0004F3A2 File Offset: 0x0004D5A2
	public Recorder LocalRecorder
	{
		get
		{
			return this.localRecorder;
		}
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x06000FFF RID: 4095 RVA: 0x0004F3AA File Offset: 0x0004D5AA
	public Speaker LocalSpeaker
	{
		get
		{
			return this.localSpeaker;
		}
	}

	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06001000 RID: 4096 RVA: 0x0004F3B4 File Offset: 0x0004D5B4
	// (remove) Token: 0x06001001 RID: 4097 RVA: 0x0004F3EC File Offset: 0x0004D5EC
	public event Action OnJoinedRoomEvent;

	// Token: 0x06001002 RID: 4098 RVA: 0x0004F421 File Offset: 0x0004D621
	protected void JoinedNetworkRoom()
	{
		VRRigCache.Instance.OnJoinedRoom();
		Action onJoinedRoomEvent = this.OnJoinedRoomEvent;
		if (onJoinedRoomEvent == null)
		{
			return;
		}
		onJoinedRoomEvent();
	}

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x06001003 RID: 4099 RVA: 0x0004F440 File Offset: 0x0004D640
	// (remove) Token: 0x06001004 RID: 4100 RVA: 0x0004F478 File Offset: 0x0004D678
	public event Action OnMultiplayerStarted;

	// Token: 0x06001005 RID: 4101 RVA: 0x0004F4AD File Offset: 0x0004D6AD
	internal void MultiplayerStarted()
	{
		Action onMultiplayerStarted = this.OnMultiplayerStarted;
		if (onMultiplayerStarted == null)
		{
			return;
		}
		onMultiplayerStarted();
	}

	// Token: 0x1400002B RID: 43
	// (add) Token: 0x06001006 RID: 4102 RVA: 0x0004F4C0 File Offset: 0x0004D6C0
	// (remove) Token: 0x06001007 RID: 4103 RVA: 0x0004F4F8 File Offset: 0x0004D6F8
	public event Action OnReturnedToSinglePlayer;

	// Token: 0x06001008 RID: 4104 RVA: 0x0004F530 File Offset: 0x0004D730
	protected void SinglePlayerStarted()
	{
		try
		{
			Action onReturnedToSinglePlayer = this.OnReturnedToSinglePlayer;
			if (onReturnedToSinglePlayer != null)
			{
				onReturnedToSinglePlayer();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		VRRigCache.Instance.OnLeftRoom();
	}

	// Token: 0x1400002C RID: 44
	// (add) Token: 0x06001009 RID: 4105 RVA: 0x0004F574 File Offset: 0x0004D774
	// (remove) Token: 0x0600100A RID: 4106 RVA: 0x0004F5AC File Offset: 0x0004D7AC
	public event Action<NetPlayer> OnPlayerJoined;

	// Token: 0x0600100B RID: 4107 RVA: 0x0004F5E1 File Offset: 0x0004D7E1
	protected void PlayerJoined(NetPlayer netPlayer)
	{
		if (this.IsOnline)
		{
			VRRigCache.Instance.OnPlayerEnteredRoom(netPlayer);
			Action<NetPlayer> onPlayerJoined = this.OnPlayerJoined;
			if (onPlayerJoined == null)
			{
				return;
			}
			onPlayerJoined(netPlayer);
		}
	}

	// Token: 0x1400002D RID: 45
	// (add) Token: 0x0600100C RID: 4108 RVA: 0x0004F608 File Offset: 0x0004D808
	// (remove) Token: 0x0600100D RID: 4109 RVA: 0x0004F640 File Offset: 0x0004D840
	public event Action<NetPlayer> OnPlayerLeft;

	// Token: 0x0600100E RID: 4110 RVA: 0x0004F678 File Offset: 0x0004D878
	protected void PlayerLeft(NetPlayer netPlayer)
	{
		try
		{
			Action<NetPlayer> onPlayerLeft = this.OnPlayerLeft;
			if (onPlayerLeft != null)
			{
				onPlayerLeft(netPlayer);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		VRRigCache.Instance.OnPlayerLeftRoom(netPlayer);
	}

	// Token: 0x1400002E RID: 46
	// (add) Token: 0x0600100F RID: 4111 RVA: 0x0004F6BC File Offset: 0x0004D8BC
	// (remove) Token: 0x06001010 RID: 4112 RVA: 0x0004F6F4 File Offset: 0x0004D8F4
	internal event Action<NetPlayer> OnMasterClientSwitchedEvent;

	// Token: 0x06001011 RID: 4113 RVA: 0x0004F729 File Offset: 0x0004D929
	protected void OnMasterClientSwitchedCallback(NetPlayer nMaster)
	{
		Action<NetPlayer> onMasterClientSwitchedEvent = this.OnMasterClientSwitchedEvent;
		if (onMasterClientSwitchedEvent == null)
		{
			return;
		}
		onMasterClientSwitchedEvent(nMaster);
	}

	// Token: 0x1400002F RID: 47
	// (add) Token: 0x06001012 RID: 4114 RVA: 0x0004F73C File Offset: 0x0004D93C
	// (remove) Token: 0x06001013 RID: 4115 RVA: 0x0004F774 File Offset: 0x0004D974
	public event Action<byte, object, int> OnRaiseEvent;

	// Token: 0x06001014 RID: 4116 RVA: 0x0004F7A9 File Offset: 0x0004D9A9
	internal void RaiseEvent(byte eventCode, object data, int source)
	{
		Action<byte, object, int> onRaiseEvent = this.OnRaiseEvent;
		if (onRaiseEvent == null)
		{
			return;
		}
		onRaiseEvent(eventCode, data, source);
	}

	// Token: 0x14000030 RID: 48
	// (add) Token: 0x06001015 RID: 4117 RVA: 0x0004F7C0 File Offset: 0x0004D9C0
	// (remove) Token: 0x06001016 RID: 4118 RVA: 0x0004F7F8 File Offset: 0x0004D9F8
	public event Action<Dictionary<string, object>> OnCustomAuthenticationResponse;

	// Token: 0x06001017 RID: 4119 RVA: 0x0004F82D File Offset: 0x0004DA2D
	internal void CustomAuthenticationResponse(Dictionary<string, object> response)
	{
		Action<Dictionary<string, object>> onCustomAuthenticationResponse = this.OnCustomAuthenticationResponse;
		if (onCustomAuthenticationResponse == null)
		{
			return;
		}
		onCustomAuthenticationResponse(response);
	}

	// Token: 0x06001018 RID: 4120 RVA: 0x0004F840 File Offset: 0x0004DA40
	public virtual void Initialise()
	{
		Debug.Log("INITIALISING NETWORKSYSTEMS");
		if (NetworkSystem.Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		NetworkSystem.Instance = this;
		NetCrossoverUtils.Prewarm();
	}

	// Token: 0x06001019 RID: 4121 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void Update()
	{
	}

	// Token: 0x0600101A RID: 4122 RVA: 0x0004F86F File Offset: 0x0004DA6F
	public void RegisterSceneNetworkItem(GameObject item)
	{
		if (!this.SceneObjectsToAttach.Contains(item))
		{
			this.SceneObjectsToAttach.Add(item);
		}
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x0004F88B File Offset: 0x0004DA8B
	public virtual void AttachObjectInGame(GameObject item)
	{
		this.RegisterSceneNetworkItem(item);
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void DetatchSceneObjectInGame(GameObject item)
	{
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x0004F894 File Offset: 0x0004DA94
	public virtual AuthenticationValues GetAuthenticationValues()
	{
		Debug.LogWarning("NetworkSystem.GetAuthenticationValues should be overridden");
		return new AuthenticationValues();
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x0004F8A5 File Offset: 0x0004DAA5
	public virtual void SetAuthenticationValues(AuthenticationValues authValues)
	{
		Debug.LogWarning("NetworkSystem.SetAuthenticationValues should be overridden");
	}

	// Token: 0x0600101F RID: 4127
	public abstract void FinishAuthenticating();

	// Token: 0x06001020 RID: 4128
	public abstract Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1);

	// Token: 0x06001021 RID: 4129
	public abstract Task JoinFriendsRoom(string userID, int actorID, string keyToFollow, string shufflerToFollow);

	// Token: 0x06001022 RID: 4130
	public abstract Task ReturnToSinglePlayer();

	// Token: 0x06001023 RID: 4131
	public abstract void JoinPubWithFriends();

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x06001024 RID: 4132 RVA: 0x0004F8B1 File Offset: 0x0004DAB1
	public bool WrongVersion
	{
		get
		{
			return this.isWrongVersion;
		}
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x0004F8B9 File Offset: 0x0004DAB9
	public void SetWrongVersion()
	{
		this.isWrongVersion = true;
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x0004F8C2 File Offset: 0x0004DAC2
	public GameObject NetInstantiate(GameObject prefab, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, false);
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x0004F8D6 File Offset: 0x0004DAD6
	public GameObject NetInstantiate(GameObject prefab, Vector3 position, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, Quaternion.identity, false);
	}

	// Token: 0x06001028 RID: 4136
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false);

	// Token: 0x06001029 RID: 4137
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false);

	// Token: 0x0600102A RID: 4138
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null);

	// Token: 0x0600102B RID: 4139
	public abstract void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null);

	// Token: 0x0600102C RID: 4140
	public abstract void NetDestroy(GameObject instance);

	// Token: 0x0600102D RID: 4141
	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true);

	// Token: 0x0600102E RID: 4142
	public abstract void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true) where T : struct;

	// Token: 0x0600102F RID: 4143
	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true);

	// Token: 0x06001030 RID: 4144
	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod);

	// Token: 0x06001031 RID: 4145
	public abstract void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args) where T : struct;

	// Token: 0x06001032 RID: 4146
	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message);

	// Token: 0x06001033 RID: 4147 RVA: 0x0004F8E8 File Offset: 0x0004DAE8
	public static string GetRandomRoomName()
	{
		string text = "";
		for (int i = 0; i < 4; i++)
		{
			text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
		}
		if (GorillaComputer.instance.IsPlayerInVirtualStump())
		{
			text = GorillaComputer.instance.VStumpRoomPrepend + text;
		}
		if (GorillaComputer.instance.CheckAutoBanListForName(text))
		{
			return text;
		}
		return NetworkSystem.GetRandomRoomName();
	}

	// Token: 0x06001034 RID: 4148
	public abstract string GetRandomWeightedRegion();

	// Token: 0x06001035 RID: 4149 RVA: 0x0004F960 File Offset: 0x0004DB60
	protected async Task RefreshNonce()
	{
		Debug.Log("Refreshing Nonce Token.");
		this.nonceRefreshed = false;
		PlayFabAuthenticator.instance.RefreshSteamAuthTicketForPhoton(new Action<string>(this.GetSteamAuthTicketSuccessCallback), new Action<EResult>(this.GetSteamAuthTicketFailureCallback));
		while (!this.nonceRefreshed)
		{
			await Task.Yield();
		}
		Debug.Log("New Nonce Token acquired");
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x0004F9A4 File Offset: 0x0004DBA4
	private void GetSteamAuthTicketSuccessCallback(string ticket)
	{
		AuthenticationValues authenticationValues = this.GetAuthenticationValues();
		Dictionary<string, object> dictionary = ((authenticationValues != null) ? authenticationValues.AuthPostData : null) as Dictionary<string, object>;
		if (dictionary != null)
		{
			dictionary["Nonce"] = ticket;
			authenticationValues.SetAuthPostData(dictionary);
			this.SetAuthenticationValues(authenticationValues);
			this.nonceRefreshed = true;
		}
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x0004F9EE File Offset: 0x0004DBEE
	private void GetSteamAuthTicketFailureCallback(EResult result)
	{
		base.StartCoroutine(this.ReGetNonce());
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x0004F9FD File Offset: 0x0004DBFD
	private IEnumerator ReGetNonce()
	{
		yield return new WaitForSeconds(3f);
		PlayFabAuthenticator.instance.RefreshSteamAuthTicketForPhoton(new Action<string>(this.GetSteamAuthTicketSuccessCallback), new Action<EResult>(this.GetSteamAuthTicketFailureCallback));
		yield return null;
		yield break;
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x0004FA0C File Offset: 0x0004DC0C
	public void BroadcastMyRoom(bool create, string key, string shuffler)
	{
		string text = NetworkSystem.ShuffleRoomName(NetworkSystem.Instance.RoomName, shuffler.Substring(2, 8), true) + "|" + NetworkSystem.ShuffleRoomName("ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(NetworkSystem.Instance.currentRegionIndex, 1), shuffler.Substring(0, 2), true);
		Debug.Log(string.Format("Broadcasting room {0} region {1}({2}). Create: {3} key: {4} (shuffler {5}) shuffled: {6}", new object[]
		{
			NetworkSystem.Instance.RoomName,
			NetworkSystem.Instance.currentRegionIndex,
			NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex],
			create,
			key,
			shuffler,
			text
		}));
		GorillaServer instance = GorillaServer.Instance;
		BroadcastMyRoomRequest broadcastMyRoomRequest = new BroadcastMyRoomRequest();
		broadcastMyRoomRequest.KeyToFollow = key;
		broadcastMyRoomRequest.RoomToJoin = text;
		broadcastMyRoomRequest.Set = create;
		instance.BroadcastMyRoom(broadcastMyRoomRequest, delegate(ExecuteFunctionResult result)
		{
		}, delegate(PlayFabError error)
		{
		});
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x0004FB24 File Offset: 0x0004DD24
	public bool InstantCheckGroupData(string userID, string keyToFollow)
	{
		bool success = false;
		global::PlayFab.ClientModels.GetSharedGroupDataRequest getSharedGroupDataRequest = new global::PlayFab.ClientModels.GetSharedGroupDataRequest();
		getSharedGroupDataRequest.Keys = new List<string> { keyToFollow };
		getSharedGroupDataRequest.SharedGroupId = userID;
		PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
		{
			Debug.Log("Get Shared Group Data returned a success");
			Debug.Log(result.Data.ToStringFull());
			if (result.Data.Count > 0)
			{
				success = true;
				return;
			}
			Debug.Log("RESULT returned but no DATA");
		}, delegate(PlayFabError error)
		{
			Debug.Log("ERROR - no group data found");
		}, null, null);
		return success;
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x0004FB94 File Offset: 0x0004DD94
	public NetPlayer GetNetPlayerByID(int playerActorNumber)
	{
		return this.netPlayerCache.Find((NetPlayer a) => a.ActorNumber == playerActorNumber);
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void NetRaiseEventReliable(byte eventCode, object data)
	{
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void NetRaiseEventUnreliable(byte eventCode, object data)
	{
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void NetRaiseEventReliable(byte eventCode, object data, NetEventOptions options)
	{
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void NetRaiseEventUnreliable(byte eventCode, object data, NetEventOptions options)
	{
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x0004FBC8 File Offset: 0x0004DDC8
	public static string ShuffleRoomName(string room, string shuffle, bool encode)
	{
		NetworkSystem.shuffleStringBuilder.Clear();
		int num;
		if (!int.TryParse(shuffle, out num))
		{
			Debug.Log("Shuffle room failed");
			return "";
		}
		for (int i = 0; i < room.Length; i++)
		{
			int num2 = int.Parse(shuffle.Substring(i * 2 % (shuffle.Length - 1), 2));
			int num3 = NetworkSystem.mod("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".IndexOf(room[i]) + (encode ? num2 : (-num2)), "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Length);
			NetworkSystem.shuffleStringBuilder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"[num3]);
		}
		return NetworkSystem.shuffleStringBuilder.ToString();
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x00023459 File Offset: 0x00021659
	public static int mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	// Token: 0x06001042 RID: 4162
	public abstract Task AwaitSceneReady();

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x06001043 RID: 4163
	public abstract string CurrentPhotonBackend { get; }

	// Token: 0x06001044 RID: 4164
	public abstract NetPlayer GetLocalPlayer();

	// Token: 0x06001045 RID: 4165
	public abstract NetPlayer GetPlayer(int PlayerID);

	// Token: 0x06001046 RID: 4166 RVA: 0x0004FC70 File Offset: 0x0004DE70
	public NetPlayer GetPlayer(Player punPlayer)
	{
		if (punPlayer == null)
		{
			return null;
		}
		NetPlayer netPlayer = this.FindPlayer(punPlayer);
		if (netPlayer == null)
		{
			this.UpdatePlayers();
			netPlayer = this.FindPlayer(punPlayer);
			if (netPlayer == null)
			{
				Debug.LogError(string.Format("There is no NetPlayer with this ID currently in game. Passed ID: {0} nickname {1}", punPlayer.ActorNumber, punPlayer.NickName));
				return null;
			}
		}
		return netPlayer;
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x0004FCC4 File Offset: 0x0004DEC4
	private NetPlayer FindPlayer(Player punPlayer)
	{
		for (int i = 0; i < this.netPlayerCache.Count; i++)
		{
			if (this.netPlayerCache[i].GetPlayerRef() == punPlayer)
			{
				return this.netPlayerCache[i];
			}
		}
		return null;
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x00045F91 File Offset: 0x00044191
	public NetPlayer GetPlayer(PlayerRef playerRef)
	{
		return null;
	}

	// Token: 0x06001049 RID: 4169
	public abstract void SetMyNickName(string name);

	// Token: 0x0600104A RID: 4170
	public abstract string GetMyNickName();

	// Token: 0x0600104B RID: 4171
	public abstract string GetMyDefaultName();

	// Token: 0x0600104C RID: 4172
	public abstract string GetNickName(int playerID);

	// Token: 0x0600104D RID: 4173
	public abstract string GetNickName(NetPlayer player);

	// Token: 0x0600104E RID: 4174
	public abstract string GetMyUserID();

	// Token: 0x0600104F RID: 4175
	public abstract string GetUserID(int playerID);

	// Token: 0x06001050 RID: 4176
	public abstract string GetUserID(NetPlayer player);

	// Token: 0x06001051 RID: 4177
	public abstract void SetMyTutorialComplete();

	// Token: 0x06001052 RID: 4178
	public abstract bool GetMyTutorialCompletion();

	// Token: 0x06001053 RID: 4179
	public abstract bool GetPlayerTutorialCompletion(int playerID);

	// Token: 0x06001054 RID: 4180 RVA: 0x0004FD09 File Offset: 0x0004DF09
	public void AddVoiceSettings(SO_NetworkVoiceSettings settings)
	{
		this.VoiceSettings = settings;
	}

	// Token: 0x06001055 RID: 4181
	public abstract void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback);

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x06001056 RID: 4182
	public abstract VoiceConnection VoiceConnection { get; }

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06001057 RID: 4183
	public abstract bool IsOnline { get; }

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06001058 RID: 4184
	public abstract bool InRoom { get; }

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x06001059 RID: 4185
	public abstract string RoomName { get; }

	// Token: 0x0600105A RID: 4186
	public abstract string RoomStringStripped();

	// Token: 0x0600105B RID: 4187 RVA: 0x0004FD14 File Offset: 0x0004DF14
	public string RoomString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
		{
			this.RoomName,
			this.CurrentRoom.isPublic ? "visible" : "hidden",
			this.CurrentRoom.isJoinable ? "open" : "closed",
			this.CurrentRoom.MaxPlayers,
			this.RoomPlayerCount,
			this.CurrentRoom.CustomProps.ToStringFull()
		});
	}

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x0600105C RID: 4188
	public abstract string GameModeString { get; }

	// Token: 0x170001BE RID: 446
	// (get) Token: 0x0600105D RID: 4189
	public abstract string CurrentRegion { get; }

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x0600105E RID: 4190
	public abstract bool SessionIsPrivate { get; }

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x0600105F RID: 4191
	public abstract int LocalPlayerID { get; }

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x06001060 RID: 4192 RVA: 0x0004FDA6 File Offset: 0x0004DFA6
	public virtual NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.netPlayerCache.ToArray();
		}
	}

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06001061 RID: 4193 RVA: 0x0004FDB3 File Offset: 0x0004DFB3
	public virtual NetPlayer[] PlayerListOthers
	{
		get
		{
			return this.netPlayerCache.FindAll((NetPlayer p) => !p.IsLocal).ToArray();
		}
	}

	// Token: 0x06001062 RID: 4194
	protected abstract void UpdateNetPlayerList();

	// Token: 0x06001063 RID: 4195 RVA: 0x0004FDE4 File Offset: 0x0004DFE4
	public void UpdatePlayers()
	{
		this.UpdateNetPlayerList();
	}

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06001064 RID: 4196
	public abstract double SimTime { get; }

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x06001065 RID: 4197
	public abstract float SimDeltaTime { get; }

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x06001066 RID: 4198
	public abstract int SimTick { get; }

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x06001067 RID: 4199
	public abstract int TickRate { get; }

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06001068 RID: 4200
	public abstract int ServerTimestamp { get; }

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06001069 RID: 4201
	public abstract int RoomPlayerCount { get; }

	// Token: 0x0600106A RID: 4202
	public abstract int GlobalPlayerCount();

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x0600106B RID: 4203 RVA: 0x0004FDEC File Offset: 0x0004DFEC
	// (set) Token: 0x0600106C RID: 4204 RVA: 0x0004FDF4 File Offset: 0x0004DFF4
	public RoomConfig CurrentRoom { get; protected set; }

	// Token: 0x0600106D RID: 4205
	public abstract bool IsObjectLocallyOwned(GameObject obj);

	// Token: 0x0600106E RID: 4206
	public abstract bool IsObjectRoomObject(GameObject obj);

	// Token: 0x0600106F RID: 4207
	public abstract bool ShouldUpdateObject(GameObject obj);

	// Token: 0x06001070 RID: 4208
	public abstract bool ShouldWriteObjectData(GameObject obj);

	// Token: 0x06001071 RID: 4209
	public abstract int GetOwningPlayerID(GameObject obj);

	// Token: 0x06001072 RID: 4210
	public abstract bool ShouldSpawnLocally(int playerID);

	// Token: 0x06001073 RID: 4211
	public abstract bool IsTotalAuthority();

	// Token: 0x040012BC RID: 4796
	public static NetworkSystem Instance;

	// Token: 0x040012BD RID: 4797
	public NetworkSystemConfig config;

	// Token: 0x040012BE RID: 4798
	public bool changingSceneManually;

	// Token: 0x040012BF RID: 4799
	public string[] regionNames;

	// Token: 0x040012C0 RID: 4800
	public int currentRegionIndex;

	// Token: 0x040012C2 RID: 4802
	private bool nonceRefreshed;

	// Token: 0x040012C3 RID: 4803
	protected bool isWrongVersion;

	// Token: 0x040012C4 RID: 4804
	private NetSystemState testState;

	// Token: 0x040012C5 RID: 4805
	protected List<NetPlayer> netPlayerCache = new List<NetPlayer>();

	// Token: 0x040012C6 RID: 4806
	protected Recorder localRecorder;

	// Token: 0x040012C7 RID: 4807
	protected Speaker localSpeaker;

	// Token: 0x040012C9 RID: 4809
	public List<GameObject> SceneObjectsToAttach = new List<GameObject>();

	// Token: 0x040012CA RID: 4810
	protected SO_NetworkVoiceSettings VoiceSettings;

	// Token: 0x040012CB RID: 4811
	protected List<Action<RemoteVoiceLink>> remoteVoiceAddedCallbacks = new List<Action<RemoteVoiceLink>>();

	// Token: 0x040012D4 RID: 4820
	protected static readonly byte[] EmptyArgs = new byte[0];

	// Token: 0x040012D5 RID: 4821
	public const string roomCharacters = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";

	// Token: 0x040012D6 RID: 4822
	public const string shuffleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

	// Token: 0x040012D7 RID: 4823
	private static StringBuilder shuffleStringBuilder = new StringBuilder(4);

	// Token: 0x040012D8 RID: 4824
	protected static StringBuilder reusableSB = new StringBuilder();

	// Token: 0x020002AB RID: 683
	// (Invoke) Token: 0x06001077 RID: 4215
	public delegate void RPC(byte[] data);

	// Token: 0x020002AC RID: 684
	// (Invoke) Token: 0x0600107B RID: 4219
	public delegate void StringRPC(string message);

	// Token: 0x020002AD RID: 685
	// (Invoke) Token: 0x0600107F RID: 4223
	public delegate void StaticRPC(byte[] data);

	// Token: 0x020002AE RID: 686
	// (Invoke) Token: 0x06001083 RID: 4227
	public delegate void StaticRPCPlaceholder(byte[] args);
}
