using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Sockets;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x0200020E RID: 526
internal class RequestableOwnershipGaurdHandler : IPunOwnershipCallbacks, IInRoomCallbacks, INetworkRunnerCallbacks
{
	// Token: 0x06000C23 RID: 3107 RVA: 0x000402F8 File Offset: 0x0003E4F8
	static RequestableOwnershipGaurdHandler()
	{
		PhotonNetwork.AddCallbackTarget(RequestableOwnershipGaurdHandler.callbackInstance);
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x00040322 File Offset: 0x0003E522
	internal static void RegisterView(NetworkView view, RequestableOwnershipGuard guard)
	{
		if (view == null || RequestableOwnershipGaurdHandler.gaurdedViews.Contains(view))
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Add(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Add(view, guard);
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x00040353 File Offset: 0x0003E553
	internal static void RemoveView(NetworkView view)
	{
		if (view == null)
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Remove(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Remove(view);
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x00040378 File Offset: 0x0003E578
	internal static void RegisterViews(NetworkView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RegisterView(views[i], guard);
		}
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x000403A0 File Offset: 0x0003E5A0
	public static void RemoveViews(NetworkView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RemoveView(views[i]);
		}
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x000403C8 File Offset: 0x0003E5C8
	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		NetworkView networkView = RequestableOwnershipGaurdHandler.gaurdedViews.FirstOrDefault((NetworkView p) => p.GetView == targetView);
		RequestableOwnershipGuard requestableOwnershipGuard;
		if (networkView.IsNull() || !RequestableOwnershipGaurdHandler.guardingLookup.TryGetValue(networkView, out requestableOwnershipGuard) || requestableOwnershipGuard.IsNull())
		{
			return;
		}
		NetPlayer currentOwner = requestableOwnershipGuard.currentOwner;
		Player player = ((currentOwner != null) ? currentOwner.GetPlayerRef() : null);
		int num = ((player != null) ? player.ActorNumber : 0);
		if (num == 0 || previousOwner != player)
		{
			GTDev.LogError<string>("Ownership transferred but the previous owner didn't initiate the request, Switching back", null);
			targetView.OwnerActorNr = num;
			targetView.ControllerActorNr = num;
		}
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x00040467 File Offset: 0x0003E667
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		this.OnHostChangedShared();
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x00040467 File Offset: 0x0003E667
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.OnHostChangedShared();
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x00040470 File Offset: 0x0003E670
	private void OnHostChangedShared()
	{
		foreach (NetworkView networkView in RequestableOwnershipGaurdHandler.gaurdedViews)
		{
			RequestableOwnershipGuard requestableOwnershipGuard;
			if (!RequestableOwnershipGaurdHandler.guardingLookup.TryGetValue(networkView, out requestableOwnershipGuard))
			{
				break;
			}
			if (networkView.Owner != null && requestableOwnershipGuard.currentOwner != null && !object.Equals(networkView.Owner, requestableOwnershipGuard.currentOwner))
			{
				networkView.OwnerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
				networkView.ControllerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
			}
		}
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x000023F4 File Offset: 0x000005F4
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x000023F4 File Offset: 0x000005F4
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnConnectedToServer(NetworkRunner runner)
	{
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x04000EE4 RID: 3812
	private static HashSet<NetworkView> gaurdedViews = new HashSet<NetworkView>();

	// Token: 0x04000EE5 RID: 3813
	private static readonly RequestableOwnershipGaurdHandler callbackInstance = new RequestableOwnershipGaurdHandler();

	// Token: 0x04000EE6 RID: 3814
	private static Dictionary<NetworkView, RequestableOwnershipGuard> guardingLookup = new Dictionary<NetworkView, RequestableOwnershipGuard>();
}
