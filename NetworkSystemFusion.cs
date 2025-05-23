using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Audio;
using Photon.Realtime;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000289 RID: 649
public class NetworkSystemFusion : NetworkSystem
{
	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06000F12 RID: 3858 RVA: 0x0004AFDA File Offset: 0x000491DA
	// (set) Token: 0x06000F13 RID: 3859 RVA: 0x0004AFE2 File Offset: 0x000491E2
	public NetworkRunner runner { get; private set; }

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06000F14 RID: 3860 RVA: 0x0004AFEB File Offset: 0x000491EB
	public override bool IsOnline
	{
		get
		{
			return this.runner != null && !this.runner.IsSinglePlayer;
		}
	}

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06000F15 RID: 3861 RVA: 0x0004B00B File Offset: 0x0004920B
	public override bool InRoom
	{
		get
		{
			return this.runner != null && this.runner.State != NetworkRunner.States.Shutdown && !this.runner.IsSinglePlayer && this.runner.IsConnectedToServer;
		}
	}

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x06000F16 RID: 3862 RVA: 0x0004B043 File Offset: 0x00049243
	public override string RoomName
	{
		get
		{
			SessionInfo sessionInfo = this.runner.SessionInfo;
			if (sessionInfo == null)
			{
				return null;
			}
			return sessionInfo.Name;
		}
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x0004B05C File Offset: 0x0004925C
	public override string RoomStringStripped()
	{
		SessionInfo sessionInfo = this.runner.SessionInfo;
		NetworkSystem.reusableSB.Clear();
		NetworkSystem.reusableSB.AppendFormat("Room: '{0}' ", (sessionInfo.Name.Length < 20) ? sessionInfo.Name : sessionInfo.Name.Remove(20));
		NetworkSystem.reusableSB.AppendFormat("{0},{1} {3}/{2} players.", new object[]
		{
			sessionInfo.IsVisible ? "visible" : "hidden",
			sessionInfo.IsOpen ? "open" : "closed",
			sessionInfo.MaxPlayers,
			sessionInfo.PlayerCount
		});
		NetworkSystem.reusableSB.Append("\ncustomProps: {");
		NetworkSystem.reusableSB.AppendFormat("joinedGameMode={0}, ", (RoomSystem.RoomGameMode.Length < 50) ? RoomSystem.RoomGameMode : RoomSystem.RoomGameMode.Remove(50));
		IDictionary properties = sessionInfo.Properties;
		Debug.Log(RoomSystem.RoomGameMode.ToString());
		if (properties.Contains("gameMode"))
		{
			object obj = properties["gameMode"];
			if (obj == null)
			{
				NetworkSystem.reusableSB.AppendFormat("gameMode=null}", Array.Empty<object>());
			}
			else
			{
				string text = obj as string;
				if (text != null)
				{
					NetworkSystem.reusableSB.AppendFormat("gameMode={0}", (text.Length < 50) ? text : text.Remove(50));
				}
			}
		}
		NetworkSystem.reusableSB.Append("}");
		Debug.Log(NetworkSystem.reusableSB.ToString());
		return NetworkSystem.reusableSB.ToString();
	}

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06000F18 RID: 3864 RVA: 0x0004B1F8 File Offset: 0x000493F8
	public override string GameModeString
	{
		get
		{
			SessionProperty sessionProperty;
			this.runner.SessionInfo.Properties.TryGetValue("gameMode", out sessionProperty);
			if (sessionProperty != null)
			{
				return (string)sessionProperty.PropertyValue;
			}
			return null;
		}
	}

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06000F19 RID: 3865 RVA: 0x0004B232 File Offset: 0x00049432
	public override string CurrentRegion
	{
		get
		{
			SessionInfo sessionInfo = this.runner.SessionInfo;
			if (sessionInfo == null)
			{
				return null;
			}
			return sessionInfo.Region;
		}
	}

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06000F1A RID: 3866 RVA: 0x0004B24C File Offset: 0x0004944C
	public override bool SessionIsPrivate
	{
		get
		{
			NetworkRunner runner = this.runner;
			bool? flag;
			if (runner == null)
			{
				flag = null;
			}
			else
			{
				SessionInfo sessionInfo = runner.SessionInfo;
				flag = ((sessionInfo != null) ? new bool?(!sessionInfo.IsVisible) : null);
			}
			bool? flag2 = flag;
			return flag2.GetValueOrDefault();
		}
	}

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06000F1B RID: 3867 RVA: 0x0004B298 File Offset: 0x00049498
	public override int LocalPlayerID
	{
		get
		{
			return this.runner.LocalPlayer.PlayerId;
		}
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x06000F1C RID: 3868 RVA: 0x0004B2B8 File Offset: 0x000494B8
	public override string CurrentPhotonBackend
	{
		get
		{
			return "Fusion";
		}
	}

	// Token: 0x1700018D RID: 397
	// (get) Token: 0x06000F1D RID: 3869 RVA: 0x0004B2BF File Offset: 0x000494BF
	public override double SimTime
	{
		get
		{
			return (double)this.runner.SimulationTime;
		}
	}

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x06000F1E RID: 3870 RVA: 0x0004B2CD File Offset: 0x000494CD
	public override float SimDeltaTime
	{
		get
		{
			return this.runner.DeltaTime;
		}
	}

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x06000F1F RID: 3871 RVA: 0x0004B2DA File Offset: 0x000494DA
	public override int SimTick
	{
		get
		{
			return this.runner.Tick.Raw;
		}
	}

	// Token: 0x17000190 RID: 400
	// (get) Token: 0x06000F20 RID: 3872 RVA: 0x0004B2EC File Offset: 0x000494EC
	public override int TickRate
	{
		get
		{
			return this.runner.TickRate;
		}
	}

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x06000F21 RID: 3873 RVA: 0x0004B2DA File Offset: 0x000494DA
	public override int ServerTimestamp
	{
		get
		{
			return this.runner.Tick.Raw;
		}
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x06000F22 RID: 3874 RVA: 0x0004B2F9 File Offset: 0x000494F9
	public override int RoomPlayerCount
	{
		get
		{
			return this.runner.SessionInfo.PlayerCount;
		}
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x06000F23 RID: 3875 RVA: 0x0004B30B File Offset: 0x0004950B
	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.FusionVoice;
		}
	}

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x06000F24 RID: 3876 RVA: 0x0004B313 File Offset: 0x00049513
	public override bool IsMasterClient
	{
		get
		{
			NetworkRunner runner = this.runner;
			return runner == null || runner.IsSharedModeMasterClient;
		}
	}

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x06000F25 RID: 3877 RVA: 0x0004B328 File Offset: 0x00049528
	public override NetPlayer MasterClient
	{
		get
		{
			if (this.runner != null && this.runner.IsSharedModeMasterClient)
			{
				return base.GetPlayer(this.runner.LocalPlayer);
			}
			if (!(global::GorillaGameModes.GameMode.ActiveNetworkHandler != null))
			{
				return null;
			}
			return base.GetPlayer(global::GorillaGameModes.GameMode.ActiveNetworkHandler.Object.StateAuthority);
		}
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x0004B388 File Offset: 0x00049588
	public override async void Initialise()
	{
		base.Initialise();
		this.myObjectProvider = new CustomObjectProvider();
		base.netState = NetSystemState.Initialization;
		this.internalState = NetworkSystemFusion.InternalState.Idle;
		await this.ReturnToSinglePlayer();
		this.AwaitAuth();
		this.CreateRegionCrawler();
		GameModeSerializer.FusionGameModeOwnerChanged = (Action<NetPlayer>)Delegate.Combine(GameModeSerializer.FusionGameModeOwnerChanged, new Action<NetPlayer>(base.OnMasterClientSwitchedCallback));
		base.OnMasterClientSwitchedEvent += this.OnMasterSwitch;
		base.netState = NetSystemState.Idle;
		this.playerPool = new ObjectPool<FusionNetPlayer>(10);
		base.UpdatePlayers();
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x0004B3C0 File Offset: 0x000495C0
	private void CreateRegionCrawler()
	{
		GameObject gameObject = new GameObject("[Network Crawler]");
		gameObject.transform.SetParent(base.transform);
		this.regionCrawler = gameObject.AddComponent<FusionRegionCrawler>();
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x0004B3F8 File Offset: 0x000495F8
	private async Task AwaitAuth()
	{
		this.internalState = NetworkSystemFusion.InternalState.AwaitingAuth;
		while (this.cachedPlayfabAuth == null)
		{
			await Task.Yield();
		}
		this.internalState = NetworkSystemFusion.InternalState.Idle;
		base.netState = NetSystemState.Idle;
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x0004B43B File Offset: 0x0004963B
	public override void FinishAuthenticating()
	{
		if (this.cachedPlayfabAuth != null)
		{
			Debug.Log("AUTHED");
			return;
		}
		Debug.LogError("Authentication Failed");
	}

	// Token: 0x06000F2A RID: 3882 RVA: 0x0004B45C File Offset: 0x0004965C
	public override async Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetJoinResult netJoinResult;
		if (this.isWrongVersion)
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else if (base.netState != NetSystemState.Idle && base.netState != NetSystemState.InGame)
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else if (this.InRoom && roomName == this.RoomName)
		{
			netJoinResult = NetJoinResult.AlreadyInRoom;
		}
		else
		{
			base.netState = NetSystemState.Connecting;
			Utils.Log("Connecting to:" + (string.IsNullOrEmpty(roomName) ? "random room" : roomName));
			NetJoinResult netJoinResult2;
			if (!string.IsNullOrEmpty(roomName))
			{
				Task<NetJoinResult> makeOrJoinTask = this.MakeOrJoinRoom(roomName, opts);
				await makeOrJoinTask;
				netJoinResult2 = makeOrJoinTask.Result;
				makeOrJoinTask = null;
			}
			else
			{
				Task<NetJoinResult> makeOrJoinTask = this.JoinRandomPublicRoom(opts);
				await makeOrJoinTask;
				netJoinResult2 = makeOrJoinTask.Result;
				makeOrJoinTask = null;
			}
			if (netJoinResult2 == NetJoinResult.Failed_Full || netJoinResult2 == NetJoinResult.Failed_Other)
			{
				this.ResetSystem();
				netJoinResult = netJoinResult2;
			}
			else if (netJoinResult2 == NetJoinResult.AlreadyInRoom)
			{
				base.netState = NetSystemState.InGame;
				netJoinResult = netJoinResult2;
			}
			else
			{
				base.UpdatePlayers();
				base.netState = NetSystemState.InGame;
				Utils.Log("Connect to room result: " + netJoinResult2.ToString());
				netJoinResult = netJoinResult2;
			}
		}
		return netJoinResult;
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x0004B4B0 File Offset: 0x000496B0
	private async Task<bool> Connect(global::Fusion.GameMode mode, string targetSessionName, RoomConfig opts)
	{
		if (this.runner != null)
		{
			bool goingBetweenRooms = this.InRoom && mode != global::Fusion.GameMode.Single;
			await this.CloseRunner(ShutdownReason.Ok);
			await Task.Yield();
			if (goingBetweenRooms)
			{
				base.SinglePlayerStarted();
				await Task.Yield();
			}
		}
		if (this.volatileNetObj)
		{
			Debug.LogError("Volatile net obj should not exist - destroying and recreating");
			Object.Destroy(this.volatileNetObj);
		}
		this.volatileNetObj = new GameObject("VolatileFusionObj");
		this.volatileNetObj.transform.parent = base.transform;
		this.runner = this.volatileNetObj.AddComponent<NetworkRunner>();
		this.internalRPCProvider = this.runner.AddBehaviour<FusionInternalRPCs>();
		this.callbackHandler = this.volatileNetObj.AddComponent<FusionCallbackHandler>();
		this.callbackHandler.Setup(this);
		this.AttachCallbackTargets();
		this.lastConnectAttempt_WasFull = false;
		this.internalState = NetworkSystemFusion.InternalState.ConnectingToRoom;
		Hashtable customProps = opts.CustomProps;
		Dictionary<string, SessionProperty> dictionary = ((customProps != null) ? customProps.ToPropDict() : null);
		this.myObjectProvider.SceneObjects = this.SceneObjectsToAttach;
		NetworkSceneManagerDefault networkSceneManagerDefault = this.volatileNetObj.AddComponent<NetworkSceneManagerDefault>();
		Task<global::Fusion.StartGameResult> startupTask = this.runner.StartGame(new StartGameArgs
		{
			IsVisible = new bool?(opts.isPublic),
			IsOpen = new bool?(opts.isJoinable),
			GameMode = mode,
			SessionName = targetSessionName,
			PlayerCount = new int?((int)opts.MaxPlayers),
			SceneManager = networkSceneManagerDefault,
			AuthValues = this.cachedPlayfabAuth,
			SessionProperties = dictionary,
			EnableClientSessionCreation = new bool?(opts.createIfMissing),
			ObjectProvider = this.myObjectProvider
		});
		await startupTask;
		Utils.Log("Startuptask finished : " + startupTask.Result.ToString());
		bool flag;
		if (!startupTask.Result.Ok)
		{
			base.CurrentRoom = null;
			flag = startupTask.Result.Ok;
		}
		else
		{
			if (this.cachedNetSceneObjects.Count > 0)
			{
				foreach (NetworkObject networkObject in this.cachedNetSceneObjects)
				{
					this.registrationQueue.Enqueue(networkObject);
				}
			}
			this.AttachSceneObjects(false);
			this.AddVoice();
			base.CurrentRoom = opts;
			if (this.IsTotalAuthority() || this.runner.IsSharedModeMasterClient)
			{
				opts.SetFusionOpts(this.runner);
			}
			this.SetMyNickName(GorillaComputer.instance.savedName);
			flag = startupTask.Result.Ok;
		}
		return flag;
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x0004B50C File Offset: 0x0004970C
	private async Task<NetJoinResult> MakeOrJoinRoom(string roomName, RoomConfig opts)
	{
		int currentRegionIndex = 0;
		bool flag = false;
		opts.createIfMissing = false;
		Task<bool> connectTask;
		while (currentRegionIndex < this.regionNames.Length && !flag)
		{
			try
			{
				PhotonAppSettings.Global.AppSettings.FixedRegion = this.regionNames[currentRegionIndex];
				this.internalState = NetworkSystemFusion.InternalState.Searching_Joining;
				connectTask = this.Connect(global::Fusion.GameMode.Shared, roomName, opts);
				await connectTask;
				flag = connectTask.Result;
				if (!flag)
				{
					if (this.lastConnectAttempt_WasFull)
					{
						Utils.Log("Found room but it was full");
						break;
					}
					Utils.Log("Region incrimenting");
					currentRegionIndex++;
				}
				connectTask = null;
			}
			catch (Exception ex)
			{
				Debug.LogError("MakeOrJoinRoom - message: " + ex.Message + "\nStacktrace : " + ex.StackTrace);
				return NetJoinResult.Failed_Other;
			}
		}
		if (this.lastConnectAttempt_WasFull)
		{
			PhotonAppSettings.Global.AppSettings.FixedRegion = "";
			return NetJoinResult.Failed_Full;
		}
		if (flag)
		{
			return NetJoinResult.Success;
		}
		PhotonAppSettings.Global.AppSettings.FixedRegion = "";
		opts.createIfMissing = true;
		connectTask = this.Connect(global::Fusion.GameMode.Shared, roomName, opts);
		await connectTask;
		Utils.Log("made room?");
		if (!connectTask.Result)
		{
			Debug.LogError("NS-FUS] Failed to create private room");
			return NetJoinResult.Failed_Other;
		}
		while (!this.runner.SessionInfo.IsValid)
		{
			await Task.Yield();
		}
		return NetJoinResult.FallbackCreated;
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x0004B560 File Offset: 0x00049760
	private async Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		bool shouldCreateIfNone = opts.createIfMissing;
		PhotonAppSettings.Global.AppSettings.FixedRegion = "";
		this.internalState = NetworkSystemFusion.InternalState.Searching_Joining;
		opts.createIfMissing = false;
		Task<bool> connectTask = this.Connect(global::Fusion.GameMode.Shared, null, opts);
		await connectTask;
		NetJoinResult netJoinResult;
		if (!connectTask.Result && shouldCreateIfNone)
		{
			opts.createIfMissing = shouldCreateIfNone;
			Task<bool> createTask = this.Connect(global::Fusion.GameMode.Shared, NetworkSystem.GetRandomRoomName(), opts);
			await createTask;
			if (!createTask.Result)
			{
				Debug.LogError("NS-FUS] Failed to create public room");
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else
			{
				opts.SetFusionOpts(this.runner);
				netJoinResult = NetJoinResult.FallbackCreated;
			}
		}
		else
		{
			netJoinResult = NetJoinResult.Success;
		}
		return netJoinResult;
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x0004B5AC File Offset: 0x000497AC
	public override async Task JoinFriendsRoom(string userID, int actorIDToFollow, string keyToFollow, string shufflerToFollow)
	{
		bool foundFriend = false;
		float searchStartTime = Time.time;
		float timeToSpendSearching = 15f;
		Dictionary<string, global::PlayFab.ClientModels.SharedGroupDataRecord> dummyData = new Dictionary<string, global::PlayFab.ClientModels.SharedGroupDataRecord>();
		try
		{
			base.groupJoinInProgress = true;
			while (!foundFriend && searchStartTime + timeToSpendSearching > Time.time)
			{
				NetworkSystemFusion.<>c__DisplayClass62_0 CS$<>8__locals1 = new NetworkSystemFusion.<>c__DisplayClass62_0();
				CS$<>8__locals1.data = dummyData;
				CS$<>8__locals1.callbackFinished = false;
				PlayFabClientAPI.GetSharedGroupData(new global::PlayFab.ClientModels.GetSharedGroupDataRequest
				{
					Keys = new List<string> { keyToFollow },
					SharedGroupId = userID
				}, delegate(GetSharedGroupDataResult result)
				{
					CS$<>8__locals1.data = result.Data;
					Debug.Log(string.Format("Got friend follow data, {0} entries", CS$<>8__locals1.data.Count));
					CS$<>8__locals1.callbackFinished = true;
				}, delegate(PlayFabError error)
				{
					Debug.Log(string.Format("GetSharedGroupData returns error: {0}", error));
					CS$<>8__locals1.callbackFinished = true;
				}, null, null);
				while (!CS$<>8__locals1.callbackFinished)
				{
					await Task.Yield();
				}
				foreach (KeyValuePair<string, global::PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in CS$<>8__locals1.data)
				{
					if (keyValuePair.Key == keyToFollow)
					{
						string[] array = keyValuePair.Value.Value.Split("|", StringSplitOptions.None);
						if (array.Length == 2)
						{
							string roomID = NetworkSystem.ShuffleRoomName(array[0], shufflerToFollow.Substring(2, 8), false);
							int regionIndex = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".IndexOf(NetworkSystem.ShuffleRoomName(array[1], shufflerToFollow.Substring(0, 2), false));
							if (regionIndex >= 0 && regionIndex < NetworkSystem.Instance.regionNames.Length)
							{
								foundFriend = true;
								NetPlayer player = this.GetPlayer(actorIDToFollow);
								if (this.InRoom && this.GetPlayer(actorIDToFollow) != null)
								{
									GorillaNot.instance.SendReport("possible kick attempt", player.UserId, player.NickName);
								}
								else if (this.RoomName != roomID)
								{
									await this.ReturnToSinglePlayer();
									Task<NetJoinResult> ConnectToRoomTask = this.ConnectToRoom(roomID, new RoomConfig
									{
										createIfMissing = false,
										isPublic = true,
										isJoinable = true
									}, regionIndex);
									await ConnectToRoomTask;
									NetJoinResult result2 = ConnectToRoomTask.Result;
									ConnectToRoomTask = null;
								}
								roomID = null;
							}
						}
					}
				}
				Dictionary<string, global::PlayFab.ClientModels.SharedGroupDataRecord>.Enumerator enumerator = default(Dictionary<string, global::PlayFab.ClientModels.SharedGroupDataRecord>.Enumerator);
				await Task.Delay(500);
				CS$<>8__locals1 = null;
			}
		}
		finally
		{
			base.groupJoinInProgress = false;
		}
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x00002628 File Offset: 0x00000828
	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x0004B610 File Offset: 0x00049810
	public override async Task ReturnToSinglePlayer()
	{
		if (base.netState == NetSystemState.InGame || base.netState == NetSystemState.Initialization)
		{
			base.netState = NetSystemState.Disconnecting;
			Utils.Log("Returning to single player");
			if (this.runner)
			{
				await this.CloseRunner(ShutdownReason.Ok);
				await Task.Yield();
				Utils.Log("Connect in return to single player");
			}
			base.netState = NetSystemState.Idle;
			this.internalState = NetworkSystemFusion.InternalState.Idle;
			base.SinglePlayerStarted();
		}
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x0004B654 File Offset: 0x00049854
	private async Task CloseRunner(ShutdownReason reason = ShutdownReason.Ok)
	{
		this.internalState = NetworkSystemFusion.InternalState.Disconnecting;
		try
		{
			await this.runner.Shutdown(true, reason, false);
		}
		catch (Exception ex)
		{
			StackFrame frame = new StackTrace(ex, true).GetFrame(0);
			int fileLineNumber = frame.GetFileLineNumber();
			Debug.LogError(string.Concat(new string[]
			{
				ex.Message,
				" File:",
				frame.GetFileName(),
				" line: ",
				fileLineNumber.ToString()
			}));
		}
		if (Application.isPlaying)
		{
			Object.Destroy(this.volatileNetObj);
		}
		else
		{
			Object.DestroyImmediate(this.volatileNetObj);
		}
		this.internalState = NetworkSystemFusion.InternalState.Disconnected;
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x0004B6A0 File Offset: 0x000498A0
	public async void MigrateHost(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		Utils.Log("HOSTTEST : MigrateHostTriggered, returning to single player!");
		await this.ReturnToSinglePlayer();
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x0004B6D8 File Offset: 0x000498D8
	public async void ResetSystem()
	{
		if (Application.isPlaying)
		{
			base.StopAllCoroutines();
			await this.Connect(global::Fusion.GameMode.Single, "--", RoomConfig.SPConfig());
			Utils.Log("Connect in return to single player");
			base.netState = NetSystemState.Idle;
			this.internalState = NetworkSystemFusion.InternalState.Idle;
		}
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x0004B70F File Offset: 0x0004990F
	private void AddVoice()
	{
		this.SetupVoice();
		this.FusionVoiceBridge = this.volatileNetObj.AddComponent<FusionVoiceBridge>();
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0004B728 File Offset: 0x00049928
	private void SetupVoice()
	{
		Utils.Log("<color=orange>Adding Voice Stuff</color>");
		this.FusionVoice = this.volatileNetObj.AddComponent<VoiceConnection>();
		this.FusionVoice.LogLevel = this.VoiceSettings.LogLevel;
		this.FusionVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
		this.FusionVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
		this.FusionVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
		Photon.Realtime.AppSettings appSettings = new Photon.Realtime.AppSettings();
		appSettings.AppIdFusion = PhotonAppSettings.Global.AppSettings.AppIdFusion;
		appSettings.AppIdVoice = PhotonAppSettings.Global.AppSettings.AppIdVoice;
		this.FusionVoice.Settings = appSettings;
		this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
		{
			this.FusionVoice.RemoteVoiceAdded += callback;
		});
		this.localRecorder = this.volatileNetObj.AddComponent<Recorder>();
		this.localRecorder.LogLevel = this.VoiceSettings.LogLevel;
		this.localRecorder.RecordOnlyWhenEnabled = this.VoiceSettings.RecordOnlyWhenEnabled;
		this.localRecorder.RecordOnlyWhenJoined = this.VoiceSettings.RecordOnlyWhenJoined;
		this.localRecorder.StopRecordingWhenPaused = this.VoiceSettings.StopRecordingWhenPaused;
		this.localRecorder.TransmitEnabled = this.VoiceSettings.TransmitEnabled;
		this.localRecorder.AutoStart = this.VoiceSettings.AutoStart;
		this.localRecorder.Encrypt = this.VoiceSettings.Encrypt;
		this.localRecorder.FrameDuration = this.VoiceSettings.FrameDuration;
		this.localRecorder.SamplingRate = this.VoiceSettings.SamplingRate;
		this.localRecorder.InterestGroup = this.VoiceSettings.InterestGroup;
		this.localRecorder.SourceType = this.VoiceSettings.InputSourceType;
		this.localRecorder.MicrophoneType = this.VoiceSettings.MicrophoneType;
		this.localRecorder.UseMicrophoneTypeFallback = this.VoiceSettings.UseFallback;
		this.localRecorder.VoiceDetection = this.VoiceSettings.Detect;
		this.localRecorder.VoiceDetectionThreshold = this.VoiceSettings.Threshold;
		this.localRecorder.Bitrate = this.VoiceSettings.Bitrate;
		this.localRecorder.VoiceDetectionDelayMs = this.VoiceSettings.Delay;
		this.localRecorder.DebugEchoMode = this.VoiceSettings.DebugEcho;
		this.localRecorder.UserData = this.runner.UserId;
		this.FusionVoice.PrimaryRecorder = this.localRecorder;
		this.volatileNetObj.AddComponent<VoiceToLoudness>();
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x0004B9CB File Offset: 0x00049BCB
	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x0004B9D9 File Offset: 0x00049BD9
	private void AttachCallbackTargets()
	{
		this.runner.AddCallbacks(this.objectsThatNeedCallbacks.ToArray());
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x0004B9F1 File Offset: 0x00049BF1
	public void RegisterForNetworkCallbacks(INetworkRunnerCallbacks callbacks)
	{
		if (!this.objectsThatNeedCallbacks.Contains(callbacks))
		{
			this.objectsThatNeedCallbacks.Add(callbacks);
		}
		if (this.runner != null)
		{
			this.runner.AddCallbacks(new INetworkRunnerCallbacks[] { callbacks });
		}
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x0004BA30 File Offset: 0x00049C30
	private async void AttachSceneObjects(bool onlyCached = false)
	{
		if (!onlyCached)
		{
			this.SceneObjectsToAttach.ForEach(delegate(GameObject obj)
			{
				if (!this.cachedNetSceneObjects.Exists((NetworkObject o) => o.gameObject == obj.gameObject))
				{
					NetworkObject component = obj.GetComponent<NetworkObject>();
					if (component == null)
					{
						Debug.LogWarning("no network object on scene item - " + obj.name);
						return;
					}
					this.cachedNetSceneObjects.Add(component);
					this.registrationQueue.Enqueue(component);
				}
			});
		}
		await Task.Delay(5);
		this.ProcessRegistrationQueue();
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x0004BA70 File Offset: 0x00049C70
	public override void AttachObjectInGame(GameObject item)
	{
		base.AttachObjectInGame(item);
		NetworkObject component = item.GetComponent<NetworkObject>();
		if ((component != null && !this.cachedNetSceneObjects.Contains(component)) || !component.IsValid)
		{
			this.cachedNetSceneObjects.AddIfNew(component);
			this.registrationQueue.Enqueue(component);
			this.ProcessRegistrationQueue();
		}
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x0004BAC8 File Offset: 0x00049CC8
	private void ProcessRegistrationQueue()
	{
		if (this.isProcessingQueue)
		{
			Debug.LogError("Queue is still processing");
			return;
		}
		this.isProcessingQueue = true;
		List<NetworkObject> list = new List<NetworkObject>();
		SceneRef sceneRef = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
		while (this.registrationQueue.Count > 0)
		{
			NetworkObject networkObject = this.registrationQueue.Dequeue();
			if (this.InRoom && !networkObject.IsValid && !networkObject.Id.IsValid && networkObject.Runner == null)
			{
				try
				{
					list.Add(networkObject);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					this.isProcessingQueue = false;
					this.runner.RegisterSceneObjects(sceneRef, list.ToArray(), default(NetworkSceneLoadId));
					this.ProcessRegistrationQueue();
					break;
				}
			}
		}
		this.runner.RegisterSceneObjects(sceneRef, list.ToArray(), default(NetworkSceneLoadId));
		this.isProcessingQueue = false;
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x0004BBC4 File Offset: 0x00049DC4
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		Utils.Log("Net instantiate Fusion: " + prefab.name);
		try
		{
			return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(this.runner.LocalPlayer), null, (NetworkSpawnFlags)0).gameObject;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
		return null;
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x0004BC34 File Offset: 0x00049E34
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (playerRef.PlayerId == playerAuthID)
			{
				Utils.Log("Net instantiate Fusion: " + prefab.name);
				return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(playerRef), null, (NetworkSpawnFlags)0).gameObject;
			}
		}
		Debug.LogError(string.Format("Couldn't find player with ID: {0}, cancelling requested spawn...", playerAuthID));
		return null;
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x0004BCE0 File Offset: 0x00049EE0
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null)
	{
		Utils.Log("Net instantiate Fusion: " + prefab.name);
		return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(this.runner.LocalPlayer), callback, (NetworkSpawnFlags)0).gameObject;
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x0004BD34 File Offset: 0x00049F34
	public override void NetDestroy(GameObject instance)
	{
		NetworkObject networkObject;
		if (instance.TryGetComponent<NetworkObject>(out networkObject))
		{
			this.runner.Despawn(networkObject);
			return;
		}
		Object.Destroy(instance);
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x0004BD60 File Offset: 0x00049F60
	public override bool ShouldSpawnLocally(int playerID)
	{
		if (this.runner.GameMode == global::Fusion.GameMode.Shared)
		{
			return this.runner.LocalPlayer.PlayerId == playerID || (playerID == -1 && this.runner.IsSharedModeMasterClient);
		}
		return this.runner.GameMode != global::Fusion.GameMode.Client;
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x0004BDB8 File Offset: 0x00049FB8
	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x0004BE30 File Offset: 0x0004A030
	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
		(ref args).SerializeToRPCData<T>();
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x0004BEB0 File Offset: 0x0004A0B0
	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x0004BF14 File Offset: 0x0004A114
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		this.GetPlayerRef(targetPlayerID);
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x0004BF33 File Offset: 0x0004A133
	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Utils.Log(rpcMethod.GetDelegateName() + "RPC called!");
		this.GetPlayerRef(targetPlayerID);
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x0004BF52 File Offset: 0x0004A152
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		this.GetPlayerRef(targetPlayerID);
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x0004BF5C File Offset: 0x0004A15C
	public override void NetRaiseEventReliable(byte eventCode, object data)
	{
		byte[] array = data.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedReliable(this.runner, eventCode, array, false, null, default(RpcInfo));
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x0004BF88 File Offset: 0x0004A188
	public override void NetRaiseEventUnreliable(byte eventCode, object data)
	{
		byte[] array = data.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(this.runner, eventCode, array, false, null, default(RpcInfo));
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x0004BFB4 File Offset: 0x0004A1B4
	public override void NetRaiseEventReliable(byte eventCode, object data, NetEventOptions opts)
	{
		byte[] array = data.ByteSerialize();
		byte[] array2 = opts.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedReliable(this.runner, eventCode, array, true, array2, default(RpcInfo));
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x0004BFE8 File Offset: 0x0004A1E8
	public override void NetRaiseEventUnreliable(byte eventCode, object data, NetEventOptions opts)
	{
		byte[] array = data.ByteSerialize();
		byte[] array2 = opts.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(this.runner, eventCode, array, true, array2, default(RpcInfo));
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x00002628 File Offset: 0x00000828
	public override string GetRandomWeightedRegion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x0004C01C File Offset: 0x0004A21C
	public override async Task AwaitSceneReady()
	{
		while (this.runner.SceneManager.IsBusy)
		{
			await Task.Yield();
		}
		for (float counter = 0f; counter < 0.5f; counter += Time.deltaTime)
		{
			await Task.Yield();
		}
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnJoinedSession()
	{
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x0004C05F File Offset: 0x0004A25F
	public void OnJoinFailed(NetConnectFailedReason reason)
	{
		switch (reason)
		{
		case NetConnectFailedReason.Timeout:
		case NetConnectFailedReason.ServerRefused:
			break;
		case NetConnectFailedReason.ServerFull:
			this.lastConnectAttempt_WasFull = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x0004C07D File Offset: 0x0004A27D
	public void OnDisconnectedFromSession()
	{
		Utils.Log("On Disconnected");
		this.internalState = NetworkSystemFusion.InternalState.Disconnected;
		base.UpdatePlayers();
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x0004C097 File Offset: 0x0004A297
	public void OnRunnerShutDown()
	{
		Utils.Log("Runner shutdown callback");
		if (this.internalState == NetworkSystemFusion.InternalState.Disconnecting)
		{
			this.internalState = NetworkSystemFusion.InternalState.Disconnected;
		}
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x0004C0B5 File Offset: 0x0004A2B5
	public void OnFusionPlayerJoined(PlayerRef player)
	{
		this.AwaitJoiningPlayerClientReady(player);
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x0004C0C0 File Offset: 0x0004A2C0
	private async Task AwaitJoiningPlayerClientReady(PlayerRef player)
	{
		base.UpdatePlayers();
		if (this.runner != null && player == this.runner.LocalPlayer && !this.runner.IsSinglePlayer)
		{
			Utils.Log("JoinedNetworkRoom");
			await Task.Delay(8);
			base.JoinedNetworkRoom();
		}
		if (this.runner != null && player == this.runner.LocalPlayer && this.runner.IsSinglePlayer)
		{
			base.SinglePlayerStarted();
		}
		await Task.Delay(200);
		NetPlayer joiningPlayer = base.GetPlayer(player);
		if (joiningPlayer == null)
		{
			Debug.LogError("Joining player doesnt have a NetPlayer somehow, this shouldnt happen");
		}
		while (joiningPlayer.NickName.IsNullOrEmpty())
		{
			await Task.Delay(1);
		}
		base.PlayerJoined(joiningPlayer);
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x0004C10C File Offset: 0x0004A30C
	public void OnFusionPlayerLeft(PlayerRef player)
	{
		if (this.IsTotalAuthority())
		{
			NetworkObject playerObject = this.runner.GetPlayerObject(player);
			if (playerObject != null)
			{
				Utils.Log("Destroying player object for leaving player!");
				this.NetDestroy(playerObject.gameObject);
			}
			else
			{
				Utils.Log("Player left without destroying an avatar for it somehow?");
			}
		}
		NetPlayer player2 = base.GetPlayer(player);
		if (player2 == null)
		{
			Debug.LogError("Joining player doesnt have a NetPlayer somehow, this shouldnt happen");
		}
		base.PlayerLeft(player2);
		base.UpdatePlayers();
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x0004C17C File Offset: 0x0004A37C
	protected override void UpdateNetPlayerList()
	{
		if (this.runner == null)
		{
			if (this.netPlayerCache.Count <= 1)
			{
				if (this.netPlayerCache.Exists((NetPlayer p) => p.IsLocal))
				{
					goto IL_0084;
				}
			}
			this.netPlayerCache.ForEach(delegate(NetPlayer p)
			{
				this.playerPool.Return((FusionNetPlayer)p);
			});
			this.netPlayerCache.Clear();
			this.netPlayerCache.Add(new FusionNetPlayer(default(PlayerRef)));
			return;
		}
		IL_0084:
		NetPlayer[] array;
		if (this.runner.IsSinglePlayer)
		{
			if (this.netPlayerCache.Count == 1 && this.netPlayerCache[0].IsLocal)
			{
				return;
			}
			bool flag = false;
			array = this.netPlayerCache.ToArray();
			if (this.netPlayerCache.Count > 0)
			{
				foreach (NetPlayer netPlayer in array)
				{
					if (((FusionNetPlayer)netPlayer).PlayerRef == this.runner.LocalPlayer)
					{
						flag = true;
					}
					else
					{
						this.playerPool.Return((FusionNetPlayer)netPlayer);
						this.netPlayerCache.Remove(netPlayer);
					}
				}
			}
			if (!flag)
			{
				FusionNetPlayer fusionNetPlayer = this.playerPool.Take();
				fusionNetPlayer.InitPlayer(this.runner.LocalPlayer);
				this.netPlayerCache.Add(fusionNetPlayer);
			}
		}
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			bool flag2 = false;
			for (int j = 0; j < this.netPlayerCache.Count; j++)
			{
				if (playerRef == ((FusionNetPlayer)this.netPlayerCache[j]).PlayerRef)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				FusionNetPlayer fusionNetPlayer2 = this.playerPool.Take();
				fusionNetPlayer2.InitPlayer(playerRef);
				this.netPlayerCache.Add(fusionNetPlayer2);
			}
		}
		array = this.netPlayerCache.ToArray();
		foreach (NetPlayer netPlayer2 in array)
		{
			bool flag3 = false;
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == ((FusionNetPlayer)netPlayer2).PlayerRef)
					{
						flag3 = true;
					}
				}
			}
			if (!flag3)
			{
				this.playerPool.Return((FusionNetPlayer)netPlayer2);
				this.netPlayerCache.Remove(netPlayer2);
			}
		}
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x0004C438 File Offset: 0x0004A638
	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
		PlayerRef playerRef = this.runner.LocalPlayer;
		if (owningPlayerID != null)
		{
			playerRef = this.GetPlayerRef(owningPlayerID.Value);
		}
		this.runner.SetPlayerObject(playerRef, playerInstance.GetComponent<NetworkObject>());
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x0004C47C File Offset: 0x0004A67C
	private PlayerRef GetPlayerRef(int playerID)
	{
		if (this.runner == null)
		{
			Debug.LogWarning("There is no runner yet - returning default player ref");
			return default(PlayerRef);
		}
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (playerRef.PlayerId == playerID)
			{
				return playerRef;
			}
		}
		Debug.LogWarning(string.Format("GetPlayerRef - Couldn't find active player with ID #{0}", playerID));
		return default(PlayerRef);
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x0004C518 File Offset: 0x0004A718
	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != this.runner.SessionInfo.PlayerCount)
		{
			base.UpdatePlayers();
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.IsLocal)
			{
				return netPlayer;
			}
		}
		Debug.LogError("Somehow there is no local NetPlayer. This shoulnd't happen.");
		return null;
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x0004C5B0 File Offset: 0x0004A7B0
	public override NetPlayer GetPlayer(int PlayerID)
	{
		if (PlayerID == -1)
		{
			Debug.LogWarning("Attempting to get NetPlayer for local -1 ID.");
			return null;
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ActorNumber == PlayerID)
			{
				return netPlayer;
			}
		}
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != this.runner.SessionInfo.PlayerCount)
		{
			base.UpdatePlayers();
			foreach (NetPlayer netPlayer2 in this.netPlayerCache)
			{
				if (netPlayer2.ActorNumber == PlayerID)
				{
					return netPlayer2;
				}
			}
		}
		Debug.LogError("Failed to find the player, before and after resyncing the player cache, this probably shoulnd't happen...");
		return null;
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x0004C6A4 File Offset: 0x0004A8A4
	public override void SetMyNickName(string name)
	{
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags) && !name.StartsWith("gorilla"))
		{
			Debug.Log("[KID] Trying to set custom nickname but that permission has been disallowed");
			if (this.InRoom && GorillaTagger.Instance.rigSerializer != null)
			{
				GorillaTagger.Instance.rigSerializer.nickName = "gorilla";
			}
			return;
		}
		PlayerPrefs.SetString("playerName", name);
		if (this.InRoom && GorillaTagger.Instance.rigSerializer != null)
		{
			GorillaTagger.Instance.rigSerializer.nickName = name;
		}
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0004C73E File Offset: 0x0004A93E
	public override string GetMyNickName()
	{
		return PlayerPrefs.GetString("playerName");
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0004C74C File Offset: 0x0004A94C
	public override string GetMyDefaultName()
	{
		return "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x0004C780 File Offset: 0x0004A980
	public override string GetNickName(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		return this.GetNickName(player);
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x0004C79C File Offset: 0x0004A99C
	public override string GetNickName(NetPlayer player)
	{
		if (player == null)
		{
			Debug.LogError("Cant get nick name as playerID doesnt have a NetPlayer...");
			return "";
		}
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
		{
			return rigContainer.Rig.rigSerializer.defaultName.Value ?? "";
		}
		return rigContainer.Rig.rigSerializer.nickName.Value ?? "";
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x0004C815 File Offset: 0x0004AA15
	public override string GetMyUserID()
	{
		return this.runner.GetPlayerUserId(this.runner.LocalPlayer);
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x0004C82D File Offset: 0x0004AA2D
	public override string GetUserID(int playerID)
	{
		if (this.runner == null)
		{
			return string.Empty;
		}
		return this.runner.GetPlayerUserId(this.GetPlayerRef(playerID));
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x0004C855 File Offset: 0x0004AA55
	public override string GetUserID(NetPlayer player)
	{
		if (this.runner == null)
		{
			return string.Empty;
		}
		return this.runner.GetPlayerUserId(((FusionNetPlayer)player).PlayerRef);
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x0004C881 File Offset: 0x0004AA81
	public override void SetMyTutorialComplete()
	{
		if (!(PlayerPrefs.GetString("didTutorial", "nope") == "done"))
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x0004C8B2 File Offset: 0x0004AAB2
	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x0004C8D0 File Offset: 0x0004AAD0
	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			Debug.LogError("Player not found");
			return false;
		}
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer == null)
		{
			Debug.LogError("VRRig not found for player");
			return false;
		}
		if (rigContainer.Rig.rigSerializer == null)
		{
			Debug.LogWarning("Vr rig serializer is not set up on the rig yet");
			return false;
		}
		return rigContainer.Rig.rigSerializer.tutorialComplete;
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0004C946 File Offset: 0x0004AB46
	public override int GlobalPlayerCount()
	{
		if (this.regionCrawler == null)
		{
			return 0;
		}
		return this.regionCrawler.PlayerCountGlobal;
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x0004C964 File Offset: 0x0004AB64
	public override int GetOwningPlayerID(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return -1;
		}
		if (this.runner.GameMode == global::Fusion.GameMode.Shared)
		{
			return networkObject.StateAuthority.PlayerId;
		}
		return networkObject.InputAuthority.PlayerId;
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x0004C9A8 File Offset: 0x0004ABA8
	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return false;
		}
		if (this.runner.GameMode == global::Fusion.GameMode.Shared)
		{
			return networkObject.StateAuthority == this.runner.LocalPlayer;
		}
		return networkObject.InputAuthority == this.runner.LocalPlayer;
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x0004C9FC File Offset: 0x0004ABFC
	public override bool IsTotalAuthority()
	{
		return this.runner.Mode == SimulationModes.Server || this.runner.Mode == SimulationModes.Host || this.runner.GameMode == global::Fusion.GameMode.Single || this.runner.IsSharedModeMasterClient;
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x0004CA38 File Offset: 0x0004AC38
	public override bool ShouldWriteObjectData(GameObject obj)
	{
		NetworkObject networkObject;
		return obj.TryGetComponent<NetworkObject>(out networkObject) && networkObject.HasStateAuthority;
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x0004CA58 File Offset: 0x0004AC58
	public override bool ShouldUpdateObject(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			return true;
		}
		if (this.IsTotalAuthority())
		{
			return true;
		}
		if (networkObject.InputAuthority.IsRealPlayer && !networkObject.InputAuthority.IsRealPlayer)
		{
			return networkObject.InputAuthority == this.runner.LocalPlayer;
		}
		return this.runner.IsSharedModeMasterClient;
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x0004CAC0 File Offset: 0x0004ACC0
	public override bool IsObjectRoomObject(GameObject obj)
	{
		NetworkObject networkObject;
		if (obj.TryGetComponent<NetworkObject>(out networkObject))
		{
			Debug.LogWarning("Fusion currently automatically passes false for roomobject check.");
			return false;
		}
		return false;
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x0004CAE4 File Offset: 0x0004ACE4
	private void OnMasterSwitch(NetPlayer player)
	{
		if (this.runner.IsSharedModeMasterClient)
		{
			Dictionary<string, SessionProperty> dictionary = new Dictionary<string, SessionProperty> { 
			{
				"MasterClient",
				base.LocalPlayer.ActorNumber
			} };
			this.runner.SessionInfo.UpdateCustomProperties(dictionary);
		}
	}

	// Token: 0x040011FB RID: 4603
	private NetworkSystemFusion.InternalState internalState;

	// Token: 0x040011FC RID: 4604
	private FusionInternalRPCs internalRPCProvider;

	// Token: 0x040011FD RID: 4605
	private FusionCallbackHandler callbackHandler;

	// Token: 0x040011FE RID: 4606
	private FusionRegionCrawler regionCrawler;

	// Token: 0x040011FF RID: 4607
	private GameObject volatileNetObj;

	// Token: 0x04001200 RID: 4608
	private global::Fusion.Photon.Realtime.AuthenticationValues cachedPlayfabAuth;

	// Token: 0x04001201 RID: 4609
	private const string playerPropertiesPath = "P_FusionProperties";

	// Token: 0x04001202 RID: 4610
	private bool lastConnectAttempt_WasFull;

	// Token: 0x04001203 RID: 4611
	private FusionVoiceBridge FusionVoiceBridge;

	// Token: 0x04001204 RID: 4612
	private VoiceConnection FusionVoice;

	// Token: 0x04001205 RID: 4613
	private CustomObjectProvider myObjectProvider;

	// Token: 0x04001206 RID: 4614
	private ObjectPool<FusionNetPlayer> playerPool;

	// Token: 0x04001207 RID: 4615
	public List<NetworkObject> cachedNetSceneObjects = new List<NetworkObject>();

	// Token: 0x04001208 RID: 4616
	private List<INetworkRunnerCallbacks> objectsThatNeedCallbacks = new List<INetworkRunnerCallbacks>();

	// Token: 0x04001209 RID: 4617
	private Queue<NetworkObject> registrationQueue = new Queue<NetworkObject>();

	// Token: 0x0400120A RID: 4618
	private bool isProcessingQueue;

	// Token: 0x0200028A RID: 650
	private enum InternalState
	{
		// Token: 0x0400120C RID: 4620
		AwaitingAuth,
		// Token: 0x0400120D RID: 4621
		Idle,
		// Token: 0x0400120E RID: 4622
		Searching_Joining,
		// Token: 0x0400120F RID: 4623
		Searching_Joined,
		// Token: 0x04001210 RID: 4624
		Searching_JoinFailed,
		// Token: 0x04001211 RID: 4625
		Searching_Disconnecting,
		// Token: 0x04001212 RID: 4626
		Searching_Disconnected,
		// Token: 0x04001213 RID: 4627
		ConnectingToRoom,
		// Token: 0x04001214 RID: 4628
		ConnectedToRoom,
		// Token: 0x04001215 RID: 4629
		JoinRoomFailed,
		// Token: 0x04001216 RID: 4630
		Disconnecting,
		// Token: 0x04001217 RID: 4631
		Disconnected,
		// Token: 0x04001218 RID: 4632
		StateCheckFailed
	}
}
