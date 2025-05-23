using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Audio;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

// Token: 0x020002BC RID: 700
[RequireComponent(typeof(PUNCallbackNotifier))]
public class NetworkSystemPUN : NetworkSystem
{
	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x060010CD RID: 4301 RVA: 0x0005078A File Offset: 0x0004E98A
	public override NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.m_allNetPlayers;
		}
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x060010CE RID: 4302 RVA: 0x00050792 File Offset: 0x0004E992
	public override NetPlayer[] PlayerListOthers
	{
		get
		{
			return this.m_otherNetPlayers;
		}
	}

	// Token: 0x170001DA RID: 474
	// (get) Token: 0x060010CF RID: 4303 RVA: 0x0005079A File Offset: 0x0004E99A
	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.punVoice;
		}
	}

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x060010D0 RID: 4304 RVA: 0x000507A4 File Offset: 0x0004E9A4
	private int lowestPingRegionIndex
	{
		get
		{
			int num = 9999;
			int num2 = -1;
			for (int i = 0; i < this.regionData.Length; i++)
			{
				if (this.regionData[i].pingToRegion < num)
				{
					num = this.regionData[i].pingToRegion;
					num2 = i;
				}
			}
			return num2;
		}
	}

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x060010D1 RID: 4305 RVA: 0x000507ED File Offset: 0x0004E9ED
	// (set) Token: 0x060010D2 RID: 4306 RVA: 0x000507F5 File Offset: 0x0004E9F5
	private NetworkSystemPUN.InternalState internalState
	{
		get
		{
			return this.currentState;
		}
		set
		{
			this.currentState = value;
		}
	}

	// Token: 0x170001DD RID: 477
	// (get) Token: 0x060010D3 RID: 4307 RVA: 0x000507FE File Offset: 0x0004E9FE
	public override string CurrentPhotonBackend
	{
		get
		{
			return "PUN";
		}
	}

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060010D4 RID: 4308 RVA: 0x00050805 File Offset: 0x0004EA05
	public override bool IsOnline
	{
		get
		{
			return this.InRoom;
		}
	}

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x060010D5 RID: 4309 RVA: 0x0005080D File Offset: 0x0004EA0D
	public override bool InRoom
	{
		get
		{
			return PhotonNetwork.InRoom;
		}
	}

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x060010D6 RID: 4310 RVA: 0x00050814 File Offset: 0x0004EA14
	public override string RoomName
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return ((currentRoom != null) ? currentRoom.Name : null) ?? string.Empty;
		}
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x00050830 File Offset: 0x0004EA30
	public override string RoomStringStripped()
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		NetworkSystem.reusableSB.Clear();
		NetworkSystem.reusableSB.AppendFormat("Room: '{0}' ", (currentRoom.Name.Length < 20) ? currentRoom.Name : currentRoom.Name.Remove(20));
		NetworkSystem.reusableSB.AppendFormat("{0},{1} {3}/{2} players.", new object[]
		{
			currentRoom.IsVisible ? "visible" : "hidden",
			currentRoom.IsOpen ? "open" : "closed",
			currentRoom.MaxPlayers,
			currentRoom.PlayerCount
		});
		NetworkSystem.reusableSB.Append("\ncustomProps: {");
		NetworkSystem.reusableSB.AppendFormat("joinedGameMode={0}, ", (RoomSystem.RoomGameMode.Length < 50) ? RoomSystem.RoomGameMode : RoomSystem.RoomGameMode.Remove(50));
		IDictionary customProperties = currentRoom.CustomProperties;
		if (customProperties.Contains("gameMode"))
		{
			object obj = customProperties["gameMode"];
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

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x060010D8 RID: 4312 RVA: 0x000509B8 File Offset: 0x0004EBB8
	public override string GameModeString
	{
		get
		{
			object obj;
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj);
			if (obj != null)
			{
				return obj.ToString();
			}
			return null;
		}
	}

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x060010D9 RID: 4313 RVA: 0x000509E7 File Offset: 0x0004EBE7
	public override string CurrentRegion
	{
		get
		{
			return PhotonNetwork.CloudRegion;
		}
	}

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x060010DA RID: 4314 RVA: 0x000509EE File Offset: 0x0004EBEE
	public override bool SessionIsPrivate
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && !currentRoom.IsVisible;
		}
	}

	// Token: 0x170001E4 RID: 484
	// (get) Token: 0x060010DB RID: 4315 RVA: 0x00050A03 File Offset: 0x0004EC03
	public override int LocalPlayerID
	{
		get
		{
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x060010DC RID: 4316 RVA: 0x00050A0F File Offset: 0x0004EC0F
	public override int ServerTimestamp
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x060010DD RID: 4317 RVA: 0x00050A16 File Offset: 0x0004EC16
	public override double SimTime
	{
		get
		{
			return PhotonNetwork.Time;
		}
	}

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x060010DE RID: 4318 RVA: 0x00050A1D File Offset: 0x0004EC1D
	public override float SimDeltaTime
	{
		get
		{
			return Time.deltaTime;
		}
	}

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x060010DF RID: 4319 RVA: 0x00050A0F File Offset: 0x0004EC0F
	public override int SimTick
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x060010E0 RID: 4320 RVA: 0x00050A24 File Offset: 0x0004EC24
	public override int TickRate
	{
		get
		{
			return PhotonNetwork.SerializationRate;
		}
	}

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x060010E1 RID: 4321 RVA: 0x00050A2B File Offset: 0x0004EC2B
	public override int RoomPlayerCount
	{
		get
		{
			return (int)PhotonNetwork.CurrentRoom.PlayerCount;
		}
	}

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x060010E2 RID: 4322 RVA: 0x00050A37 File Offset: 0x0004EC37
	public override bool IsMasterClient
	{
		get
		{
			return PhotonNetwork.IsMasterClient;
		}
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x00050A40 File Offset: 0x0004EC40
	public override async void Initialise()
	{
		base.Initialise();
		base.netState = NetSystemState.Initialization;
		PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = NetworkSystemConfig.AppVersion;
		PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
		PhotonNetwork.EnableCloseConnection = false;
		PhotonNetwork.AutomaticallySyncScene = false;
		string playerName = PlayerPrefs.GetString("playerName", "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0'));
		this.playerPool = new ObjectPool<PunNetPlayer>(10);
		base.UpdatePlayers();
		await this.CacheRegionInfo();
		base.UpdatePlayers();
		this.SetMyNickName(playerName);
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x00050A78 File Offset: 0x0004EC78
	private async Task CacheRegionInfo()
	{
		if (!this.isWrongVersion)
		{
			this.regionData = new NetworkRegionInfo[this.regionNames.Length];
			for (int i = 0; i < this.regionData.Length; i++)
			{
				this.regionData[i] = new NetworkRegionInfo();
			}
			int tryingRegionIndex = 0;
			TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.Authenticated }).GetAwaiter();
			TaskAwaiter<bool> taskAwaiter2;
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (taskAwaiter.GetResult())
			{
				base.netState = NetSystemState.PingRecon;
				while (tryingRegionIndex < this.regionNames.Length)
				{
					this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
					PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[tryingRegionIndex];
					this.currentRegionIndex = tryingRegionIndex;
					PhotonNetwork.ConnectUsingSettings();
					taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.ConnectedToMaster }).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						return;
					}
					this.regionData[this.currentRegionIndex].playersInRegion = PhotonNetwork.CountOfPlayers;
					this.regionData[this.currentRegionIndex].pingToRegion = PhotonNetwork.GetPing();
					Utils.Log("Ping for " + PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion.ToString() + " is " + PhotonNetwork.GetPing().ToString());
					this.internalState = NetworkSystemPUN.InternalState.PingGathering;
					PhotonNetwork.Disconnect();
					taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.Internal_Disconnected }).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						return;
					}
					tryingRegionIndex++;
				}
				this.internalState = NetworkSystemPUN.InternalState.Idle;
				base.netState = NetSystemState.Idle;
			}
		}
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x00050ABB File Offset: 0x0004ECBB
	public override AuthenticationValues GetAuthenticationValues()
	{
		return PhotonNetwork.AuthValues;
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x00050AC2 File Offset: 0x0004ECC2
	public override void SetAuthenticationValues(AuthenticationValues authValues)
	{
		PhotonNetwork.AuthValues = authValues;
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x00050ACA File Offset: 0x0004ECCA
	public override void FinishAuthenticating()
	{
		this.internalState = NetworkSystemPUN.InternalState.Authenticated;
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x00050AD4 File Offset: 0x0004ECD4
	private async Task WaitForState(CancellationToken ct, params NetworkSystemPUN.InternalState[] desiredStates)
	{
		float timeoutTime = Time.time + 10f;
		while (!desiredStates.Contains(this.internalState))
		{
			if (ct.IsCancellationRequested)
			{
				string text = "";
				foreach (NetworkSystemPUN.InternalState internalState in desiredStates)
				{
					text += string.Format("- {0}", internalState);
				}
				Debug.LogError("Got cancelation token while waiting for states " + text);
				this.internalState = NetworkSystemPUN.InternalState.StateCheckFailed;
				break;
			}
			if (timeoutTime < Time.time)
			{
				string text2 = "";
				foreach (NetworkSystemPUN.InternalState internalState2 in desiredStates)
				{
					text2 += string.Format("- {0}", internalState2);
				}
				Debug.LogError("Got stuck waiting for states " + text2);
				this.internalState = NetworkSystemPUN.InternalState.StateCheckFailed;
				break;
			}
			await Task.Yield();
		}
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x00050B28 File Offset: 0x0004ED28
	private Task<bool> WaitForStateCheck(params NetworkSystemPUN.InternalState[] desiredStates)
	{
		NetworkSystemPUN.<WaitForStateCheck>d__59 <WaitForStateCheck>d__;
		<WaitForStateCheck>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForStateCheck>d__.<>4__this = this;
		<WaitForStateCheck>d__.desiredStates = desiredStates;
		<WaitForStateCheck>d__.<>1__state = -1;
		<WaitForStateCheck>d__.<>t__builder.Start<NetworkSystemPUN.<WaitForStateCheck>d__59>(ref <WaitForStateCheck>d__);
		return <WaitForStateCheck>d__.<>t__builder.Task;
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x00050B74 File Offset: 0x0004ED74
	private async Task<NetJoinResult> MakeOrFindRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		if (this.InRoom)
		{
			await this.InternalDisconnect();
		}
		this.currentRegionIndex = 0;
		bool flag = ((regionIndex >= 0) ? (await this.TryJoinRoomInRegion(roomName, opts, regionIndex)) : (await this.TryJoinRoom(roomName, opts)));
		NetJoinResult netJoinResult;
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_JoinFailed_Full)
		{
			netJoinResult = NetJoinResult.Failed_Full;
		}
		else if (!flag)
		{
			netJoinResult = await this.TryCreateRoom(roomName, opts);
		}
		else
		{
			netJoinResult = NetJoinResult.Success;
		}
		return netJoinResult;
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x00050BD0 File Offset: 0x0004EDD0
	private async Task<bool> TryJoinRoom(string roomName, RoomConfig opts)
	{
		while (this.currentRegionIndex < this.regionNames.Length)
		{
			TaskAwaiter<bool> taskAwaiter = this.TryJoinRoomInRegion(roomName, opts, this.currentRegionIndex).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (taskAwaiter.GetResult())
			{
				return true;
			}
			this.currentRegionIndex++;
		}
		return false;
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x00050C24 File Offset: 0x0004EE24
	private async Task<bool> TryJoinRoomInRegion(string roomName, RoomConfig opts, int regionIndex)
	{
		this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
		string text = this.regionNames[regionIndex];
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = text;
		this.currentRegionIndex = regionIndex;
		this.UpdateZoneInfo(opts.isPublic, null);
		PhotonNetwork.ConnectUsingSettings();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.ConnectedToMaster }).GetAwaiter();
		TaskAwaiter<bool> taskAwaiter2;
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		bool flag;
		if (!taskAwaiter.GetResult())
		{
			flag = false;
		}
		else
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joining;
			PhotonNetwork.JoinRoom(roomName, null);
			taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
			{
				NetworkSystemPUN.InternalState.Searching_Joined,
				NetworkSystemPUN.InternalState.Searching_JoinFailed,
				NetworkSystemPUN.InternalState.Searching_JoinFailed_Full
			}).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				flag = false;
			}
			else if (this.internalState == NetworkSystemPUN.InternalState.Searching_JoinFailed_Full)
			{
				flag = false;
			}
			else
			{
				bool foundRoom = this.internalState == NetworkSystemPUN.InternalState.Searching_Joined;
				if (!foundRoom)
				{
					PhotonNetwork.Disconnect();
					this.internalState = NetworkSystemPUN.InternalState.Searching_Disconnecting;
					taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.Searching_Disconnected }).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						return false;
					}
				}
				flag = foundRoom;
			}
		}
		return flag;
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x00050C80 File Offset: 0x0004EE80
	private async Task<NetJoinResult> TryCreateRoom(string roomName, RoomConfig opts)
	{
		this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
		this.currentRegionIndex = this.lowestPingRegionIndex;
		this.UpdateZoneInfo(opts.isPublic, null);
		PhotonNetwork.ConnectUsingSettings();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.ConnectedToMaster }).GetAwaiter();
		TaskAwaiter<bool> taskAwaiter2;
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		NetJoinResult netJoinResult;
		if (!taskAwaiter.GetResult())
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Creating;
			PhotonNetwork.CreateRoom(roomName, opts.ToPUNOpts(), null, null);
			taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
			{
				NetworkSystemPUN.InternalState.Searching_Created,
				NetworkSystemPUN.InternalState.Searching_CreateFailed
			}).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else if (this.internalState == NetworkSystemPUN.InternalState.Searching_CreateFailed)
			{
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else
			{
				netJoinResult = NetJoinResult.FallbackCreated;
			}
		}
		return netJoinResult;
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x00050CD4 File Offset: 0x0004EED4
	private async Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		if (this.InRoom)
		{
			await this.InternalDisconnect();
		}
		this.internalState = NetworkSystemPUN.InternalState.ConnectingToMaster;
		object obj;
		if (!this.firstRoomJoin && opts.CustomProps.TryGetValue("gameMode", out obj) && !obj.ToString().StartsWith("city"))
		{
			this.firstRoomJoin = true;
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
			this.currentRegionIndex = this.lowestPingRegionIndex;
		}
		else if (!opts.IsJoiningWithFriends)
		{
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.GetRandomWeightedRegion();
			this.currentRegionIndex = Array.IndexOf<string>(this.regionNames, PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion);
		}
		this.UpdateZoneInfo(true, PhotonNetworkController.Instance.currentJoinTrigger.zone.GetName<GTZone>());
		PhotonNetwork.ConnectUsingSettings();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.ConnectedToMaster }).GetAwaiter();
		TaskAwaiter<bool> taskAwaiter2;
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		NetJoinResult netJoinResult;
		if (!taskAwaiter.GetResult())
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joining;
			if (opts.IsJoiningWithFriends)
			{
				PhotonNetwork.JoinRandomRoom(opts.CustomProps, opts.MaxPlayers, MatchmakingMode.RandomMatching, null, null, opts.joinFriendIDs.ToArray<string>());
			}
			else
			{
				PhotonNetwork.JoinRandomRoom(opts.CustomProps, opts.MaxPlayers, MatchmakingMode.FillRoom, null, null, null);
			}
			taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
			{
				NetworkSystemPUN.InternalState.Searching_Joined,
				NetworkSystemPUN.InternalState.Searching_JoinFailed
			}).GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult())
			{
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else if (this.internalState == NetworkSystemPUN.InternalState.Searching_JoinFailed)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_Creating;
				if (opts.IsJoiningWithFriends)
				{
					PhotonNetwork.CreateRoom(NetworkSystem.GetRandomRoomName(), opts.ToPUNOpts(), null, opts.joinFriendIDs);
				}
				else
				{
					PhotonNetwork.CreateRoom(NetworkSystem.GetRandomRoomName(), opts.ToPUNOpts(), null, null);
				}
				taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[]
				{
					NetworkSystemPUN.InternalState.Searching_Created,
					NetworkSystemPUN.InternalState.Searching_CreateFailed
				}).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					netJoinResult = NetJoinResult.Failed_Other;
				}
				else if (this.internalState == NetworkSystemPUN.InternalState.Searching_CreateFailed)
				{
					netJoinResult = NetJoinResult.Failed_Other;
				}
				else
				{
					netJoinResult = NetJoinResult.FallbackCreated;
				}
			}
			else
			{
				netJoinResult = NetJoinResult.Success;
			}
		}
		return netJoinResult;
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x00050D20 File Offset: 0x0004EF20
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
		else if (this.roomTask != null && !this.roomTask.IsCompleted)
		{
			netJoinResult = NetJoinResult.Failed_Other;
		}
		else
		{
			base.netState = NetSystemState.Connecting;
			NetJoinResult netJoinResult2;
			if (roomName != null)
			{
				this.roomTask = this.MakeOrFindRoom(roomName, opts, regionIndex);
				netJoinResult2 = await this.roomTask;
				this.roomTask = null;
			}
			else
			{
				this.roomTask = this.JoinRandomPublicRoom(opts);
				netJoinResult2 = await this.roomTask;
				this.roomTask = null;
			}
			if (netJoinResult2 == NetJoinResult.Failed_Full)
			{
				GorillaComputer.instance.roomFull = true;
				GorillaComputer.instance.UpdateScreen();
				this.ResetSystem();
				this.roomTask = null;
				netJoinResult = netJoinResult2;
			}
			else if (netJoinResult2 == NetJoinResult.Failed_Other)
			{
				this.ResetSystem();
				this.roomTask = null;
				netJoinResult = netJoinResult2;
			}
			else if (netJoinResult2 == NetJoinResult.AlreadyInRoom)
			{
				base.netState = NetSystemState.InGame;
				this.roomTask = null;
				netJoinResult = netJoinResult2;
			}
			else if (!this.InRoom)
			{
				GTDev.LogError<string>("NetworkSystem: room joined success but we have disconnected", null);
				netJoinResult = NetJoinResult.Failed_Other;
			}
			else
			{
				base.netState = NetSystemState.InGame;
				base.PlayerJoined(base.LocalPlayer);
				this.localRecorder.StartRecording();
				netJoinResult = netJoinResult2;
			}
		}
		return netJoinResult;
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x00050D7C File Offset: 0x0004EF7C
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
				NetworkSystemPUN.<>c__DisplayClass66_0 CS$<>8__locals1 = new NetworkSystemPUN.<>c__DisplayClass66_0();
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
								Player player;
								if (this.InRoom && PhotonNetwork.CurrentRoom.Players.TryGetValue(actorIDToFollow, out player) && player != null)
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

	// Token: 0x060010F1 RID: 4337 RVA: 0x00002628 File Offset: 0x00000828
	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060010F2 RID: 4338 RVA: 0x00050DE0 File Offset: 0x0004EFE0
	public override string GetRandomWeightedRegion()
	{
		float value = Random.value;
		int num = 0;
		for (int i = 0; i < this.regionData.Length; i++)
		{
			num += this.regionData[i].playersInRegion;
		}
		float num2 = 0f;
		int num3 = -1;
		while (num2 < value && num3 < this.regionData.Length - 1)
		{
			num3++;
			num2 += (float)this.regionData[num3].playersInRegion / (float)num;
		}
		return this.regionNames[num3];
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x00050E58 File Offset: 0x0004F058
	public override async Task ReturnToSinglePlayer()
	{
		if (base.netState == NetSystemState.InGame || base.netState == NetSystemState.Connecting)
		{
			base.netState = NetSystemState.Disconnecting;
			this._taskCancelTokens.ForEach(delegate(CancellationTokenSource cts)
			{
				cts.Cancel();
				cts.Dispose();
			});
			this._taskCancelTokens.Clear();
			await this.InternalDisconnect();
			base.netState = NetSystemState.Idle;
		}
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x00050E9C File Offset: 0x0004F09C
	private async Task InternalDisconnect()
	{
		this.internalState = NetworkSystemPUN.InternalState.Internal_Disconnecting;
		PhotonNetwork.Disconnect();
		TaskAwaiter<bool> taskAwaiter = this.WaitForStateCheck(new NetworkSystemPUN.InternalState[] { NetworkSystemPUN.InternalState.Internal_Disconnected }).GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<bool> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		if (!taskAwaiter.GetResult())
		{
			Debug.LogError("Failed to achieve internal disconnected state");
		}
		Object.Destroy(this.VoiceNetworkObject);
		base.UpdatePlayers();
		base.SinglePlayerStarted();
	}

	// Token: 0x060010F5 RID: 4341 RVA: 0x00050EDF File Offset: 0x0004F0DF
	private void AddVoice()
	{
		this.SetupVoice();
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x00050EE8 File Offset: 0x0004F0E8
	private void SetupVoice()
	{
		this.punVoice = PhotonVoiceNetwork.Instance;
		this.VoiceNetworkObject = this.punVoice.gameObject;
		this.VoiceNetworkObject.name = "VoiceNetworkObject";
		this.VoiceNetworkObject.transform.parent = base.transform;
		this.VoiceNetworkObject.transform.localPosition = Vector3.zero;
		this.punVoice.LogLevel = this.VoiceSettings.LogLevel;
		this.punVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
		this.punVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
		this.punVoice.AutoConnectAndJoin = this.VoiceSettings.AutoConnectAndJoin;
		this.punVoice.AutoLeaveAndDisconnect = this.VoiceSettings.AutoLeaveAndDisconnect;
		this.punVoice.WorkInOfflineMode = this.VoiceSettings.WorkInOfflineMode;
		this.punVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
		AppSettings appSettings = new AppSettings();
		appSettings.AppIdRealtime = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
		appSettings.AppIdVoice = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice;
		this.punVoice.Settings = appSettings;
		this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
		{
			this.punVoice.RemoteVoiceAdded += callback;
		});
		this.localRecorder = this.VoiceNetworkObject.GetComponent<GTRecorder>();
		if (this.localRecorder == null)
		{
			this.localRecorder = this.VoiceNetworkObject.AddComponent<GTRecorder>();
			if (VRRigCache.Instance != null && VRRigCache.Instance.localRig != null)
			{
				LoudSpeakerActivator componentInChildren = VRRigCache.Instance.localRig.GetComponentInChildren<LoudSpeakerActivator>();
				if (componentInChildren != null)
				{
					componentInChildren.SetRecorder((GTRecorder)this.localRecorder);
				}
			}
		}
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
		this.punVoice.PrimaryRecorder = this.localRecorder;
		this.VoiceNetworkObject.AddComponent<VoiceToLoudness>();
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x0004B9CB File Offset: 0x00049BCB
	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x0005125B File Offset: 0x0004F45B
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return Object.Instantiate<GameObject>(prefab, position, rotation);
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, 0, null);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, 0, null);
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x00051290 File Offset: 0x0004F490
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, rotation, isRoomObject);
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x0005129D File Offset: 0x0004F49D
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return Object.Instantiate<GameObject>(prefab, position, rotation);
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, group, data);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, group, data);
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x000512D8 File Offset: 0x0004F4D8
	public override void NetDestroy(GameObject instance)
	{
		PhotonView photonView;
		if (instance.TryGetComponent<PhotonView>(out photonView) && photonView.AmOwner)
		{
			PhotonNetwork.Destroy(instance);
			return;
		}
		Object.Destroy(instance);
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x00051304 File Offset: 0x0004F504
	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		RpcTarget rpcTarget = (sendToSelf ? RpcTarget.All : RpcTarget.Others);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[] { NetworkSystem.EmptyArgs });
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x00051340 File Offset: 0x0004F540
	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		RpcTarget rpcTarget = (sendToSelf ? RpcTarget.All : RpcTarget.Others);
		(ref args).SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[] { args.Data });
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x00051384 File Offset: 0x0004F584
	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		RpcTarget rpcTarget = (sendToSelf ? RpcTarget.All : RpcTarget.Others);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[] { message });
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x000513BC File Offset: 0x0004F5BC
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[] { NetworkSystem.EmptyArgs });
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x000513FC File Offset: 0x0004F5FC
	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		(ref args).SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[] { args.Data });
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x00051444 File Offset: 0x0004F644
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[] { message });
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x00051480 File Offset: 0x0004F680
	public override async Task AwaitSceneReady()
	{
		while (PhotonNetwork.LevelLoadingProgress < 1f)
		{
			await Task.Yield();
		}
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x000514BC File Offset: 0x0004F6BC
	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0)
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
		Debug.LogError("Somehow no local net players found. This shouldn't happen");
		return null;
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x00051534 File Offset: 0x0004F734
	public override NetPlayer GetPlayer(int PlayerID)
	{
		if (this.InRoom && !PhotonNetwork.CurrentRoom.Players.ContainsKey(PlayerID))
		{
			return null;
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ActorNumber == PlayerID)
			{
				return netPlayer;
			}
		}
		base.UpdatePlayers();
		foreach (NetPlayer netPlayer2 in this.netPlayerCache)
		{
			if (netPlayer2.ActorNumber == PlayerID)
			{
				return netPlayer2;
			}
		}
		GTDev.LogWarning<string>("There is no NetPlayer with this ID currently in game. Passed ID: " + PlayerID.ToString(), null);
		return null;
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x00051614 File Offset: 0x0004F814
	public override void SetMyNickName(string id)
	{
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags) && !id.StartsWith("gorilla"))
		{
			Debug.Log("[KID] Trying to set custom nickname but that permission has been disallowed");
			PhotonNetwork.LocalPlayer.NickName = "gorilla";
			return;
		}
		PlayerPrefs.SetString("playerName", id);
		PhotonNetwork.LocalPlayer.NickName = id;
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x00051666 File Offset: 0x0004F866
	public override string GetMyNickName()
	{
		return PhotonNetwork.LocalPlayer.NickName;
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x00051672 File Offset: 0x0004F872
	public override string GetMyDefaultName()
	{
		return PhotonNetwork.LocalPlayer.DefaultName;
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x00051680 File Offset: 0x0004F880
	public override string GetNickName(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player != null)
		{
			return player.NickName;
		}
		return null;
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x000516A0 File Offset: 0x0004F8A0
	public override string GetNickName(NetPlayer player)
	{
		return player.NickName;
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x000516A8 File Offset: 0x0004F8A8
	public override void SetMyTutorialComplete()
	{
		bool flag = PlayerPrefs.GetString("didTutorial", "nope") == "done";
		if (!flag)
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("didTutorial", flag);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x0004C8B2 File Offset: 0x0004AAB2
	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x0005170C File Offset: 0x0004F90C
	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			return false;
		}
		Player player2 = PhotonNetwork.CurrentRoom.GetPlayer(player.ActorNumber, false);
		if (player2 == null)
		{
			return false;
		}
		object obj;
		if (player2.CustomProperties.TryGetValue("didTutorial", out obj))
		{
			bool flag;
			bool flag2;
			if (obj is bool)
			{
				flag = (bool)obj;
				flag2 = 1 == 0;
			}
			else
			{
				flag2 = true;
			}
			return flag2 || flag;
		}
		return false;
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x0005176B File Offset: 0x0004F96B
	public override string GetMyUserID()
	{
		return PhotonNetwork.LocalPlayer.UserId;
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x00051778 File Offset: 0x0004F978
	public override string GetUserID(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player != null)
		{
			return player.UserId;
		}
		return null;
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x00051798 File Offset: 0x0004F998
	public override string GetUserID(NetPlayer netPlayer)
	{
		Player playerRef = ((PunNetPlayer)netPlayer).PlayerRef;
		if (playerRef != null)
		{
			return playerRef.UserId;
		}
		return null;
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x000517BC File Offset: 0x0004F9BC
	public override int GlobalPlayerCount()
	{
		int num = 0;
		foreach (NetworkRegionInfo networkRegionInfo in this.regionData)
		{
			num += networkRegionInfo.playersInRegion;
		}
		return num;
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x000517F0 File Offset: 0x0004F9F0
	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		PhotonView photonView;
		return !this.IsOnline || !obj.TryGetComponent<PhotonView>(out photonView) || photonView.IsMine;
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x0005181C File Offset: 0x0004FA1C
	protected override void UpdateNetPlayerList()
	{
		if (!this.IsOnline)
		{
			bool flag = false;
			PunNetPlayer punNetPlayer = null;
			if (this.netPlayerCache.Count > 0)
			{
				for (int i = 0; i < this.netPlayerCache.Count; i++)
				{
					NetPlayer netPlayer = this.netPlayerCache[i];
					if (netPlayer.IsLocal)
					{
						punNetPlayer = (PunNetPlayer)netPlayer;
						flag = true;
					}
					else
					{
						this.playerPool.Return((PunNetPlayer)netPlayer);
					}
				}
				this.netPlayerCache.Clear();
			}
			if (!flag)
			{
				punNetPlayer = this.playerPool.Take();
				punNetPlayer.InitPlayer(PhotonNetwork.LocalPlayer);
			}
			this.netPlayerCache.Add(punNetPlayer);
		}
		else
		{
			Dictionary<int, Player>.ValueCollection values = PhotonNetwork.CurrentRoom.Players.Values;
			foreach (Player player in values)
			{
				bool flag2 = false;
				for (int j = 0; j < this.netPlayerCache.Count; j++)
				{
					if (player == ((PunNetPlayer)this.netPlayerCache[j]).PlayerRef)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					PunNetPlayer punNetPlayer2 = this.playerPool.Take();
					punNetPlayer2.InitPlayer(player);
					this.netPlayerCache.Add(punNetPlayer2);
				}
			}
			for (int k = 0; k < this.netPlayerCache.Count; k++)
			{
				PunNetPlayer punNetPlayer3 = (PunNetPlayer)this.netPlayerCache[k];
				bool flag3 = false;
				using (Dictionary<int, Player>.ValueCollection.Enumerator enumerator = values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == punNetPlayer3.PlayerRef)
						{
							flag3 = true;
							break;
						}
					}
				}
				if (!flag3)
				{
					this.playerPool.Return(punNetPlayer3);
					this.netPlayerCache.Remove(punNetPlayer3);
				}
			}
		}
		this.m_allNetPlayers = this.netPlayerCache.ToArray();
		this.m_otherNetPlayers = new NetPlayer[this.m_allNetPlayers.Length - 1];
		int num = 0;
		for (int l = 0; l < this.m_allNetPlayers.Length; l++)
		{
			NetPlayer netPlayer2 = this.m_allNetPlayers[l];
			if (netPlayer2.IsLocal)
			{
				num++;
			}
			else
			{
				int num2 = l - num;
				if (num2 == this.m_otherNetPlayers.Length)
				{
					break;
				}
				this.m_otherNetPlayers[num2] = netPlayer2;
			}
		}
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x00051A88 File Offset: 0x0004FC88
	public override bool IsObjectRoomObject(GameObject obj)
	{
		PhotonView component = obj.GetComponent<PhotonView>();
		if (component == null)
		{
			Debug.LogError("No photonview found on this Object, this shouldn't happen");
			return false;
		}
		return component.IsRoomView;
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x00051AB7 File Offset: 0x0004FCB7
	public override bool ShouldUpdateObject(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x00051AB7 File Offset: 0x0004FCB7
	public override bool ShouldWriteObjectData(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x00051AC0 File Offset: 0x0004FCC0
	public override int GetOwningPlayerID(GameObject obj)
	{
		PhotonView photonView;
		if (obj.TryGetComponent<PhotonView>(out photonView) && photonView.Owner != null)
		{
			return photonView.Owner.ActorNumber;
		}
		return -1;
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x00051AEC File Offset: 0x0004FCEC
	public override bool ShouldSpawnLocally(int playerID)
	{
		return this.LocalPlayerID == playerID || (playerID == -1 && PhotonNetwork.MasterClient.IsLocal);
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsTotalAuthority()
	{
		return false;
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x00051B09 File Offset: 0x0004FD09
	public void OnConnectedtoMaster()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.ConnectingToMaster)
		{
			this.internalState = NetworkSystemPUN.InternalState.ConnectedToMaster;
		}
		base.UpdatePlayers();
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x00051B21 File Offset: 0x0004FD21
	public void OnJoinedRoom()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joined;
		}
		else if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Created;
		}
		this.AddVoice();
		base.UpdatePlayers();
		base.JoinedNetworkRoom();
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x00051B5B File Offset: 0x0004FD5B
	public void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log("onJoinRoomFailed " + returnCode.ToString() + message);
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			if (returnCode == 32765)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed_Full;
				return;
			}
			this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed;
		}
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x00051B97 File Offset: 0x0004FD97
	public void OnCreateRoomFailed(short returnCode, string message)
	{
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_CreateFailed;
		}
	}

	// Token: 0x0600111E RID: 4382 RVA: 0x00051BAC File Offset: 0x0004FDAC
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.UpdatePlayers();
		NetPlayer player = base.GetPlayer(newPlayer);
		base.PlayerJoined(player);
	}

	// Token: 0x0600111F RID: 4383 RVA: 0x00051BD0 File Offset: 0x0004FDD0
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		NetPlayer player = base.GetPlayer(otherPlayer);
		base.UpdatePlayers();
		base.PlayerLeft(player);
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x00051BF4 File Offset: 0x0004FDF4
	public async void OnDisconnected(DisconnectCause cause)
	{
		await base.RefreshNonce();
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Disconnecting)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Disconnected;
		}
		else if (this.internalState == NetworkSystemPUN.InternalState.PingGathering)
		{
			this.internalState = NetworkSystemPUN.InternalState.Internal_Disconnected;
		}
		else if (this.internalState == NetworkSystemPUN.InternalState.Internal_Disconnecting)
		{
			this.internalState = NetworkSystemPUN.InternalState.Internal_Disconnected;
		}
		else
		{
			base.UpdatePlayers();
			base.SinglePlayerStarted();
		}
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x00051C2B File Offset: 0x0004FE2B
	public void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitchedCallback(newMasterClient);
	}

	// Token: 0x06001122 RID: 4386 RVA: 0x00051C3C File Offset: 0x0004FE3C
	private ValueTuple<CancellationTokenSource, CancellationToken> GetCancellationToken()
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		CancellationToken token = cancellationTokenSource.Token;
		this._taskCancelTokens.Add(cancellationTokenSource);
		return new ValueTuple<CancellationTokenSource, CancellationToken>(cancellationTokenSource, token);
	}

	// Token: 0x06001123 RID: 4387 RVA: 0x00051C6C File Offset: 0x0004FE6C
	public void ResetSystem()
	{
		if (this.VoiceNetworkObject)
		{
			Object.Destroy(this.VoiceNetworkObject);
		}
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
		this.currentRegionIndex = this.lowestPingRegionIndex;
		PhotonNetwork.Disconnect();
		this._taskCancelTokens.ForEach(delegate(CancellationTokenSource token)
		{
			token.Cancel();
			token.Dispose();
		});
		this._taskCancelTokens.Clear();
		this.internalState = NetworkSystemPUN.InternalState.Idle;
		base.netState = NetSystemState.Idle;
	}

	// Token: 0x06001124 RID: 4388 RVA: 0x00051D04 File Offset: 0x0004FF04
	private void UpdateZoneInfo(bool roomIsPublic, string zoneName = null)
	{
		AuthenticationValues authenticationValues = this.GetAuthenticationValues();
		Dictionary<string, object> dictionary = ((authenticationValues != null) ? authenticationValues.AuthPostData : null) as Dictionary<string, object>;
		if (dictionary != null)
		{
			dictionary["Zone"] = ((zoneName != null) ? zoneName : ((ZoneManagement.instance.activeZones.Count > 0) ? ZoneManagement.instance.activeZones.First<GTZone>().GetName<GTZone>() : ""));
			dictionary["SubZone"] = GTSubZone.none.GetName<GTSubZone>();
			dictionary["IsPublic"] = roomIsPublic;
			authenticationValues.SetAuthPostData(dictionary);
			this.SetAuthenticationValues(authenticationValues);
		}
	}

	// Token: 0x04001315 RID: 4885
	private NetworkRegionInfo[] regionData;

	// Token: 0x04001316 RID: 4886
	private Task<NetJoinResult> roomTask;

	// Token: 0x04001317 RID: 4887
	private ObjectPool<PunNetPlayer> playerPool;

	// Token: 0x04001318 RID: 4888
	private NetPlayer[] m_allNetPlayers = new NetPlayer[0];

	// Token: 0x04001319 RID: 4889
	private NetPlayer[] m_otherNetPlayers = new NetPlayer[0];

	// Token: 0x0400131A RID: 4890
	private List<CancellationTokenSource> _taskCancelTokens = new List<CancellationTokenSource>();

	// Token: 0x0400131B RID: 4891
	private PhotonVoiceNetwork punVoice;

	// Token: 0x0400131C RID: 4892
	private GameObject VoiceNetworkObject;

	// Token: 0x0400131D RID: 4893
	private NetworkSystemPUN.InternalState currentState;

	// Token: 0x0400131E RID: 4894
	private bool firstRoomJoin;

	// Token: 0x020002BD RID: 701
	private enum InternalState
	{
		// Token: 0x04001320 RID: 4896
		AwaitingAuth,
		// Token: 0x04001321 RID: 4897
		Authenticated,
		// Token: 0x04001322 RID: 4898
		PingGathering,
		// Token: 0x04001323 RID: 4899
		StateCheckFailed,
		// Token: 0x04001324 RID: 4900
		ConnectingToMaster,
		// Token: 0x04001325 RID: 4901
		ConnectedToMaster,
		// Token: 0x04001326 RID: 4902
		Idle,
		// Token: 0x04001327 RID: 4903
		Internal_Disconnecting,
		// Token: 0x04001328 RID: 4904
		Internal_Disconnected,
		// Token: 0x04001329 RID: 4905
		Searching_Connecting,
		// Token: 0x0400132A RID: 4906
		Searching_Connected,
		// Token: 0x0400132B RID: 4907
		Searching_Joining,
		// Token: 0x0400132C RID: 4908
		Searching_Joined,
		// Token: 0x0400132D RID: 4909
		Searching_JoinFailed,
		// Token: 0x0400132E RID: 4910
		Searching_JoinFailed_Full,
		// Token: 0x0400132F RID: 4911
		Searching_Creating,
		// Token: 0x04001330 RID: 4912
		Searching_Created,
		// Token: 0x04001331 RID: 4913
		Searching_CreateFailed,
		// Token: 0x04001332 RID: 4914
		Searching_Disconnecting,
		// Token: 0x04001333 RID: 4915
		Searching_Disconnected
	}
}
