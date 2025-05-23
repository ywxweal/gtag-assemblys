using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using Fusion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000629 RID: 1577
public class GorillaNot : MonoBehaviour
{
	// Token: 0x170003AF RID: 943
	// (get) Token: 0x06002706 RID: 9990 RVA: 0x0004A489 File Offset: 0x00048689
	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x06002707 RID: 9991 RVA: 0x000C18E6 File Offset: 0x000BFAE6
	// (set) Token: 0x06002708 RID: 9992 RVA: 0x000C18EE File Offset: 0x000BFAEE
	private bool sendReport
	{
		get
		{
			return this._sendReport;
		}
		set
		{
			if (!this._sendReport)
			{
				this._sendReport = true;
			}
		}
	}

	// Token: 0x170003B1 RID: 945
	// (get) Token: 0x06002709 RID: 9993 RVA: 0x000C18FF File Offset: 0x000BFAFF
	// (set) Token: 0x0600270A RID: 9994 RVA: 0x000C1907 File Offset: 0x000BFB07
	private string suspiciousPlayerId
	{
		get
		{
			return this._suspiciousPlayerId;
		}
		set
		{
			if (this._suspiciousPlayerId == "")
			{
				this._suspiciousPlayerId = value;
			}
		}
	}

	// Token: 0x170003B2 RID: 946
	// (get) Token: 0x0600270B RID: 9995 RVA: 0x000C1922 File Offset: 0x000BFB22
	// (set) Token: 0x0600270C RID: 9996 RVA: 0x000C192A File Offset: 0x000BFB2A
	private string suspiciousPlayerName
	{
		get
		{
			return this._suspiciousPlayerName;
		}
		set
		{
			if (this._suspiciousPlayerName == "")
			{
				this._suspiciousPlayerName = value;
			}
		}
	}

	// Token: 0x170003B3 RID: 947
	// (get) Token: 0x0600270D RID: 9997 RVA: 0x000C1945 File Offset: 0x000BFB45
	// (set) Token: 0x0600270E RID: 9998 RVA: 0x000C194D File Offset: 0x000BFB4D
	private string suspiciousReason
	{
		get
		{
			return this._suspiciousReason;
		}
		set
		{
			if (this._suspiciousReason == "")
			{
				this._suspiciousReason = value;
			}
		}
	}

	// Token: 0x0600270F RID: 9999 RVA: 0x000C1968 File Offset: 0x000BFB68
	private void Start()
	{
		if (GorillaNot.instance == null)
		{
			GorillaNot.instance = this;
		}
		else if (GorillaNot.instance != this)
		{
			Object.Destroy(this);
		}
		RoomSystem.PlayerJoinedEvent = (Action<NetPlayer>)Delegate.Combine(RoomSystem.PlayerJoinedEvent, new Action<NetPlayer>(this.OnPlayerEnteredRoom));
		RoomSystem.PlayerLeftEvent = (Action<NetPlayer>)Delegate.Combine(RoomSystem.PlayerLeftEvent, new Action<NetPlayer>(this.OnPlayerLeftRoom));
		RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(delegate
		{
			this.cachedPlayerList = NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0];
		}));
		base.StartCoroutine(this.CheckReports());
		this.logErrorCount = 0;
		Application.logMessageReceived += this.LogErrorCount;
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x000C1A28 File Offset: 0x000BFC28
	private void OnApplicationPause(bool paused)
	{
		if (paused || !RoomSystem.JoinedRoom)
		{
			return;
		}
		this.lastServerTimestamp = NetworkSystem.Instance.SimTick;
		this.RefreshRPCs();
	}

	// Token: 0x06002711 RID: 10001 RVA: 0x000C1A4C File Offset: 0x000BFC4C
	public void LogErrorCount(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error)
		{
			this.logErrorCount++;
			this.stringIndex = logString.LastIndexOf("Sender is ");
			if (logString.Contains("RPC") && this.stringIndex >= 0)
			{
				this.playerID = logString.Substring(this.stringIndex + 10);
				this.tempPlayer = null;
				for (int i = 0; i < this.cachedPlayerList.Length; i++)
				{
					if (this.cachedPlayerList[i].UserId == this.playerID)
					{
						this.tempPlayer = this.cachedPlayerList[i];
						break;
					}
				}
				string text = "invalid RPC stuff";
				if (!this.IncrementRPCTracker(in this.tempPlayer, in text, in this.rpcErrorMax))
				{
					this.SendReport("invalid RPC stuff", this.tempPlayer.UserId, this.tempPlayer.NickName);
				}
				this.tempPlayer = null;
			}
			if (this.logErrorCount > this.logErrorMax)
			{
				Debug.unityLogger.logEnabled = false;
			}
		}
	}

	// Token: 0x06002712 RID: 10002 RVA: 0x000C1B50 File Offset: 0x000BFD50
	public void SendReport(string susReason, string susId, string susNick)
	{
		this.suspiciousReason = susReason;
		this.suspiciousPlayerId = susId;
		this.suspiciousPlayerName = susNick;
		this.sendReport = true;
	}

	// Token: 0x06002713 RID: 10003 RVA: 0x000C1B70 File Offset: 0x000BFD70
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void DispatchReport()
	{
		if ((this.sendReport || this.testAssault) && this.suspiciousPlayerId != "" && this.reportedPlayers.IndexOf(this.suspiciousPlayerId) == -1)
		{
			if (this._suspiciousPlayerName.Length > 12)
			{
				this._suspiciousPlayerName = this._suspiciousPlayerName.Remove(12);
			}
			this.reportedPlayers.Add(this.suspiciousPlayerId);
			this.testAssault = false;
			WebFlags webFlags = new WebFlags(1);
			NetEventOptions netEventOptions = new NetEventOptions
			{
				TargetActors = GorillaNot.targetActors,
				Reciever = NetEventOptions.RecieverTarget.master,
				Flags = webFlags
			};
			string[] array = new string[this.cachedPlayerList.Length];
			int num = 0;
			foreach (NetPlayer netPlayer in this.cachedPlayerList)
			{
				array[num] = netPlayer.UserId;
				num++;
			}
			object[] array3 = new object[]
			{
				NetworkSystem.Instance.RoomStringStripped(),
				array,
				NetworkSystem.Instance.MasterClient.UserId,
				this.suspiciousPlayerId,
				this.suspiciousPlayerName,
				this.suspiciousReason,
				NetworkSystemConfig.AppVersion
			};
			NetworkSystemRaiseEvent.RaiseEvent(8, array3, netEventOptions, true);
			if (this.ShouldDisconnectFromRoom())
			{
				base.StartCoroutine(this.QuitDelay());
			}
		}
		this._sendReport = false;
		this._suspiciousPlayerId = "";
		this._suspiciousPlayerName = "";
		this._suspiciousReason = "";
	}

	// Token: 0x06002714 RID: 10004 RVA: 0x000C1CF1 File Offset: 0x000BFEF1
	private IEnumerator CheckReports()
	{
		for (;;)
		{
			try
			{
				this.logErrorCount = 0;
				if (NetworkSystem.Instance.InRoom)
				{
					this.lastCheck = Time.time;
					this.lastServerTimestamp = NetworkSystem.Instance.SimTick;
					if (!PhotonNetwork.CurrentRoom.PublishUserId)
					{
						this.sendReport = true;
						this.suspiciousReason = "missing player ids";
						this.SetToRoomCreatorIfHere();
						this.CloseInvalidRoom();
						Debug.Log("publish user id's is off");
					}
					if (this.cachedPlayerList.Length > (int)PhotonNetworkController.Instance.GetRoomSize(PhotonNetworkController.Instance.currentGameType))
					{
						this.sendReport = true;
						this.suspiciousReason = "too many players";
						this.SetToRoomCreatorIfHere();
						this.CloseInvalidRoom();
					}
					if (this.currentMasterClient != NetworkSystem.Instance.MasterClient || this.LowestActorNumber() != NetworkSystem.Instance.MasterClient.ActorNumber)
					{
						foreach (NetPlayer netPlayer in this.cachedPlayerList)
						{
							if (this.currentMasterClient == netPlayer)
							{
								this.sendReport = true;
								this.suspiciousReason = "room host force changed";
								this.suspiciousPlayerId = NetworkSystem.Instance.MasterClient.UserId;
								this.suspiciousPlayerName = NetworkSystem.Instance.MasterClient.NickName;
							}
						}
						this.currentMasterClient = NetworkSystem.Instance.MasterClient;
					}
					this.RefreshRPCs();
					this.DispatchReport();
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1.01f);
		}
		yield break;
	}

	// Token: 0x06002715 RID: 10005 RVA: 0x000C1D00 File Offset: 0x000BFF00
	private void RefreshRPCs()
	{
		foreach (Dictionary<string, GorillaNot.RPCCallTracker> dictionary in this.userRPCCalls.Values)
		{
			foreach (GorillaNot.RPCCallTracker rpccallTracker in dictionary.Values)
			{
				rpccallTracker.RPCCalls = 0;
			}
		}
	}

	// Token: 0x06002716 RID: 10006 RVA: 0x000C1D90 File Offset: 0x000BFF90
	private int LowestActorNumber()
	{
		this.lowestActorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		foreach (NetPlayer netPlayer in this.cachedPlayerList)
		{
			if (netPlayer.ActorNumber < this.lowestActorNumber)
			{
				this.lowestActorNumber = netPlayer.ActorNumber;
			}
		}
		return this.lowestActorNumber;
	}

	// Token: 0x06002717 RID: 10007 RVA: 0x000C1DEB File Offset: 0x000BFFEB
	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		this.cachedPlayerList = NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0];
	}

	// Token: 0x06002718 RID: 10008 RVA: 0x000C1E08 File Offset: 0x000C0008
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.cachedPlayerList = NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0];
		Dictionary<string, GorillaNot.RPCCallTracker> dictionary;
		if (this.userRPCCalls.TryGetValue(otherPlayer.UserId, out dictionary))
		{
			this.userRPCCalls.Remove(otherPlayer.UserId);
		}
	}

	// Token: 0x06002719 RID: 10009 RVA: 0x000C1E56 File Offset: 0x000C0056
	public static void IncrementRPCCall(PhotonMessageInfo info, [CallerMemberName] string callingMethod = "")
	{
		GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(info), callingMethod);
	}

	// Token: 0x0600271A RID: 10010 RVA: 0x000C1E64 File Offset: 0x000C0064
	public static void IncrementRPCCall(PhotonMessageInfoWrapped infoWrapped, [CallerMemberName] string callingMethod = "")
	{
		GorillaNot.instance.IncrementRPCCallLocal(infoWrapped, callingMethod);
	}

	// Token: 0x0600271B RID: 10011 RVA: 0x000C1E74 File Offset: 0x000C0074
	private void IncrementRPCCallLocal(PhotonMessageInfoWrapped infoWrapped, string rpcFunction)
	{
		if (infoWrapped.sentTick < this.lastServerTimestamp)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(infoWrapped.senderID);
		if (player == null)
		{
			return;
		}
		string userId = player.UserId;
		if (!this.IncrementRPCTracker(in userId, in rpcFunction, in this.rpcCallLimit))
		{
			this.SendReport("too many rpc calls! " + rpcFunction, player.UserId, player.NickName);
			return;
		}
	}

	// Token: 0x0600271C RID: 10012 RVA: 0x000C1EDC File Offset: 0x000C00DC
	private bool IncrementRPCTracker(in NetPlayer sender, in string rpcFunction, in int callLimit)
	{
		string userId = sender.UserId;
		return this.IncrementRPCTracker(in userId, in rpcFunction, in callLimit);
	}

	// Token: 0x0600271D RID: 10013 RVA: 0x000C1EFC File Offset: 0x000C00FC
	private bool IncrementRPCTracker(in Player sender, in string rpcFunction, in int callLimit)
	{
		string userId = sender.UserId;
		return this.IncrementRPCTracker(in userId, in rpcFunction, in callLimit);
	}

	// Token: 0x0600271E RID: 10014 RVA: 0x000C1F1C File Offset: 0x000C011C
	private bool IncrementRPCTracker(in string userId, in string rpcFunction, in int callLimit)
	{
		GorillaNot.RPCCallTracker rpccallTracker = this.GetRPCCallTracker(userId, rpcFunction);
		if (rpccallTracker == null)
		{
			return true;
		}
		rpccallTracker.RPCCalls++;
		if (rpccallTracker.RPCCalls > rpccallTracker.RPCCallsMax)
		{
			rpccallTracker.RPCCallsMax = rpccallTracker.RPCCalls;
		}
		return rpccallTracker.RPCCalls <= callLimit;
	}

	// Token: 0x0600271F RID: 10015 RVA: 0x000C1F70 File Offset: 0x000C0170
	private GorillaNot.RPCCallTracker GetRPCCallTracker(string userID, string rpcFunction)
	{
		if (userID == null)
		{
			return null;
		}
		GorillaNot.RPCCallTracker rpccallTracker = null;
		Dictionary<string, GorillaNot.RPCCallTracker> dictionary;
		if (!this.userRPCCalls.TryGetValue(userID, out dictionary))
		{
			rpccallTracker = new GorillaNot.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			Dictionary<string, GorillaNot.RPCCallTracker> dictionary2 = new Dictionary<string, GorillaNot.RPCCallTracker>();
			dictionary2.Add(rpcFunction, rpccallTracker);
			this.userRPCCalls.Add(userID, dictionary2);
		}
		else if (!dictionary.TryGetValue(rpcFunction, out rpccallTracker))
		{
			rpccallTracker = new GorillaNot.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			dictionary.Add(rpcFunction, rpccallTracker);
		}
		return rpccallTracker;
	}

	// Token: 0x06002720 RID: 10016 RVA: 0x000C1FED File Offset: 0x000C01ED
	private IEnumerator QuitDelay()
	{
		yield return new WaitForSeconds(1f);
		NetworkSystem.Instance.ReturnToSinglePlayer();
		yield break;
	}

	// Token: 0x06002721 RID: 10017 RVA: 0x000C1FF8 File Offset: 0x000C01F8
	private void SetToRoomCreatorIfHere()
	{
		this.tempPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1, false);
		if (this.tempPlayer != null)
		{
			this.suspiciousPlayerId = this.tempPlayer.UserId;
			this.suspiciousPlayerName = this.tempPlayer.NickName;
			return;
		}
		this.suspiciousPlayerId = "n/a";
		this.suspiciousPlayerName = "n/a";
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x000C2060 File Offset: 0x000C0260
	private bool ShouldDisconnectFromRoom()
	{
		return this._suspiciousReason.Contains("too many players") || this._suspiciousReason.Contains("invalid room name") || this._suspiciousReason.Contains("invalid game mode") || this._suspiciousReason.Contains("missing player ids");
	}

	// Token: 0x06002723 RID: 10019 RVA: 0x000C20B5 File Offset: 0x000C02B5
	private void CloseInvalidRoom()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.CurrentRoom.MaxPlayers = PhotonNetworkController.Instance.GetRoomSize(PhotonNetworkController.Instance.currentGameType);
	}

	// Token: 0x04002B98 RID: 11160
	[OnEnterPlay_SetNull]
	public static volatile GorillaNot instance;

	// Token: 0x04002B99 RID: 11161
	private bool _sendReport;

	// Token: 0x04002B9A RID: 11162
	private string _suspiciousPlayerId = "";

	// Token: 0x04002B9B RID: 11163
	private string _suspiciousPlayerName = "";

	// Token: 0x04002B9C RID: 11164
	private string _suspiciousReason = "";

	// Token: 0x04002B9D RID: 11165
	internal List<string> reportedPlayers = new List<string>();

	// Token: 0x04002B9E RID: 11166
	public byte roomSize;

	// Token: 0x04002B9F RID: 11167
	public float lastCheck;

	// Token: 0x04002BA0 RID: 11168
	public float checkCooldown = 3f;

	// Token: 0x04002BA1 RID: 11169
	public float userDecayTime = 15f;

	// Token: 0x04002BA2 RID: 11170
	public NetPlayer currentMasterClient;

	// Token: 0x04002BA3 RID: 11171
	public bool testAssault;

	// Token: 0x04002BA4 RID: 11172
	private const byte ReportAssault = 8;

	// Token: 0x04002BA5 RID: 11173
	private int lowestActorNumber;

	// Token: 0x04002BA6 RID: 11174
	private int calls;

	// Token: 0x04002BA7 RID: 11175
	public int rpcCallLimit = 50;

	// Token: 0x04002BA8 RID: 11176
	public int logErrorMax = 50;

	// Token: 0x04002BA9 RID: 11177
	public int rpcErrorMax = 10;

	// Token: 0x04002BAA RID: 11178
	private object outObj;

	// Token: 0x04002BAB RID: 11179
	private NetPlayer tempPlayer;

	// Token: 0x04002BAC RID: 11180
	private int logErrorCount;

	// Token: 0x04002BAD RID: 11181
	private int stringIndex;

	// Token: 0x04002BAE RID: 11182
	private string playerID;

	// Token: 0x04002BAF RID: 11183
	private string playerNick;

	// Token: 0x04002BB0 RID: 11184
	private int lastServerTimestamp;

	// Token: 0x04002BB1 RID: 11185
	private const string InvalidRPC = "invalid RPC stuff";

	// Token: 0x04002BB2 RID: 11186
	public NetPlayer[] cachedPlayerList;

	// Token: 0x04002BB3 RID: 11187
	private static int[] targetActors = new int[] { -1 };

	// Token: 0x04002BB4 RID: 11188
	private Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>> userRPCCalls = new Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>>();

	// Token: 0x04002BB5 RID: 11189
	private Hashtable hashTable;

	// Token: 0x0200062A RID: 1578
	private class RPCCallTracker
	{
		// Token: 0x04002BB6 RID: 11190
		public int RPCCalls;

		// Token: 0x04002BB7 RID: 11191
		public int RPCCallsMax;
	}
}
