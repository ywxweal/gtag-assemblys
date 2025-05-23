using System;
using GorillaTagScripts.ModIO;
using ModIO;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200073C RID: 1852
internal class VirtualStumpSerializer : GorillaSerializer
{
	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x06002E3D RID: 11837 RVA: 0x000B83A1 File Offset: 0x000B65A1
	internal bool HasAuthority
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x06002E3E RID: 11838 RVA: 0x000E6EDD File Offset: 0x000E50DD
	protected void Start()
	{
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
	}

	// Token: 0x06002E3F RID: 11839 RVA: 0x000E6F0C File Offset: 0x000E510C
	private void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		int driverID = CustomMapsTerminal.GetDriverID();
		if (leavingPlayer.ActorNumber == driverID)
		{
			CustomMapsTerminal.SetTerminalControlStatus(false, -2, true);
		}
	}

	// Token: 0x06002E40 RID: 11840 RVA: 0x000E6F3E File Offset: 0x000E513E
	private void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		this.waitingForRoomInitialization = true;
		base.SendRPC("RequestRoomInitialization_RPC", false, Array.Empty<object>());
	}

	// Token: 0x06002E41 RID: 11841 RVA: 0x000E6F68 File Offset: 0x000E5168
	[PunRPC]
	private void RequestRoomInitialization_RPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestRoomInitialization_RPC");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestRoomInitialization))
		{
			return;
		}
		player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestRoomInitialization);
		bool flag = CustomMapsTerminal.GetDriverID() != -2;
		base.SendRPC("InitializeRoom_RPC", info.Sender, new object[]
		{
			flag,
			CustomMapsTerminal.GetDriverID(),
			CustomMapsTerminal.UpdateAndRetrieveLocalStatus().PackData(true),
			CustomMapLoader.LoadedMapModId
		});
	}

	// Token: 0x06002E42 RID: 11842 RVA: 0x000E7004 File Offset: 0x000E5204
	[PunRPC]
	private void InitializeRoom_RPC(bool locked, int driverID, long[] statusData, long loadedMapModID, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "InitializeRoom_RPC");
		if (!info.Sender.IsMasterClient || !this.waitingForRoomInitialization)
		{
			return;
		}
		if (statusData.IsNullOrEmpty<long>())
		{
			return;
		}
		if (driverID != -2 && NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		CustomMapsTerminal.ClearTags();
		CustomMapsTerminal.UpdateStatusFromDriver(statusData, driverID, false);
		if (loadedMapModID > 0L)
		{
			CustomMapManager.SetRoomMod(loadedMapModID);
		}
		this.waitingForRoomInitialization = false;
	}

	// Token: 0x06002E43 RID: 11843 RVA: 0x000E7074 File Offset: 0x000E5274
	public void LoadModSynced(long modId)
	{
		CustomMapManager.SetRoomMod(modId);
		CustomMapManager.LoadMod(new ModId(modId));
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			base.SendRPC("SetRoomMap_RPC", true, new object[] { modId });
		}
	}

	// Token: 0x06002E44 RID: 11844 RVA: 0x000E70C5 File Offset: 0x000E52C5
	public void UnloadModSynced()
	{
		CustomMapManager.UnloadMod(true);
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			base.SendRPC("UnloadMod_RPC", true, Array.Empty<object>());
		}
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x000E70F8 File Offset: 0x000E52F8
	[PunRPC]
	private void SetRoomMap_RPC(long modId, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SetRoomMap_RPC");
		if (modId <= 0L)
		{
			return;
		}
		if (info.Sender.ActorNumber != this.photonView.OwnerActorNr && info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		if (modId != this.detailsScreen.currentModProfile.id.id)
		{
			return;
		}
		CustomMapManager.SetRoomMod(modId);
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x000E7160 File Offset: 0x000E5360
	[PunRPC]
	private void UnloadMod_RPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "UnloadMod_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		if (!CustomMapManager.AreAllPlayersInVirtualStump())
		{
			return;
		}
		CustomMapManager.UnloadMod(true);
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x000E718F File Offset: 0x000E538F
	public void RequestTerminalControlStatusChange(bool lockedStatus)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RequestTerminalControlStatusChange_RPC", false, new object[] { lockedStatus });
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x000E71BC File Offset: 0x000E53BC
	[PunRPC]
	private void RequestTerminalControlStatusChange_RPC(bool lockedStatus, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestTerminalControlStatusChange_RPC");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[19].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (!player.IsNull && CustomMapManager.IsRemotePlayerInVirtualStump(info.Sender.UserId))
		{
			CustomMapsTerminal.HandleTerminalControlStatusChangeRequest(lockedStatus, info.Sender.ActorNumber);
		}
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x000E7251 File Offset: 0x000E5451
	public void SetTerminalControlStatus(bool locked, int playerID)
	{
		if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		base.SendRPC("SetTerminalControlStatus_RPC", true, new object[] { locked, playerID });
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x000E7290 File Offset: 0x000E5490
	[PunRPC]
	private void SetTerminalControlStatus_RPC(bool locked, int driverID, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SetTerminalControlStatus_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (driverID != -2 && NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[16].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.SetTerminalControlStatus(locked, driverID, false);
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x000E7318 File Offset: 0x000E5518
	public void SendTerminalStatus(bool sendFullModList = false, bool forceSearch = false)
	{
		if (!NetworkSystem.Instance.InRoom || !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		base.SendRPC("UpdateScreen_RPC", true, new object[]
		{
			CustomMapsTerminal.UpdateAndRetrieveLocalStatus().PackData(sendFullModList),
			forceSearch,
			CustomMapsTerminal.GetDriverID()
		});
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x000E7370 File Offset: 0x000E5570
	[PunRPC]
	private void UpdateScreen_RPC(long[] statusData, bool forceNewSearch, int driverID, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "UpdateScreen_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID() || !CustomMapManager.IsRemotePlayerInVirtualStump(info.Sender.UserId))
		{
			return;
		}
		if (statusData.IsNullOrEmpty<long>())
		{
			return;
		}
		if (NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[17].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.UpdateStatusFromDriver(statusData, driverID, forceNewSearch);
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x000E7415 File Offset: 0x000E5615
	public void RefreshDriverNickName()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RefreshDriverNickName_RPC", true, Array.Empty<object>());
	}

	// Token: 0x06002E4E RID: 11854 RVA: 0x000E7438 File Offset: 0x000E5638
	[PunRPC]
	private void RefreshDriverNickName_RPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RefreshDriverNickName_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[18].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
	}

	// Token: 0x040034D0 RID: 13520
	[SerializeField]
	private VirtualStumpBarrierSFX barrierSFX;

	// Token: 0x040034D1 RID: 13521
	[SerializeField]
	private CustomMapsDetailsScreen detailsScreen;

	// Token: 0x040034D2 RID: 13522
	private bool waitingForRoomInitialization;
}
