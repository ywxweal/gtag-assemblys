using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;

// Token: 0x02000283 RID: 643
public class FusionRegionCrawler : MonoBehaviour, INetworkRunnerCallbacks
{
	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000EE7 RID: 3815 RVA: 0x0004AB5C File Offset: 0x00048D5C
	public int PlayerCountGlobal
	{
		get
		{
			return this.globalPlayerCount;
		}
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x0004AB64 File Offset: 0x00048D64
	public void Start()
	{
		this.regionRunner = base.gameObject.AddComponent<NetworkRunner>();
		this.regionRunner.AddCallbacks(new INetworkRunnerCallbacks[] { this });
		base.StartCoroutine(this.OccasionalUpdate());
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x0004AB99 File Offset: 0x00048D99
	public IEnumerator OccasionalUpdate()
	{
		while (this.refreshPlayerCountAutomatically)
		{
			yield return this.UpdatePlayerCount();
			yield return new WaitForSeconds(this.UpdateFrequency);
		}
		yield break;
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x0004ABA8 File Offset: 0x00048DA8
	public IEnumerator UpdatePlayerCount()
	{
		int tempGlobalPlayerCount = 0;
		StartGameArgs startGameArgs = default(StartGameArgs);
		foreach (string text in NetworkSystem.Instance.regionNames)
		{
			startGameArgs.CustomPhotonAppSettings = new FusionAppSettings();
			startGameArgs.CustomPhotonAppSettings.FixedRegion = text;
			this.waitingForSessionListUpdate = true;
			this.regionRunner.JoinSessionLobby(SessionLobby.ClientServer, startGameArgs.CustomPhotonAppSettings.FixedRegion, null, null, new bool?(false), default(CancellationToken), false);
			while (this.waitingForSessionListUpdate)
			{
				yield return new WaitForEndOfFrame();
			}
			foreach (SessionInfo sessionInfo in this.sessionInfoCache)
			{
				tempGlobalPlayerCount += sessionInfo.PlayerCount;
			}
			tempGlobalPlayerCount += this.tempSessionPlayerCount;
		}
		string[] array = null;
		this.globalPlayerCount = tempGlobalPlayerCount;
		FusionRegionCrawler.PlayerCountUpdated onPlayerCountUpdated = this.OnPlayerCountUpdated;
		if (onPlayerCountUpdated != null)
		{
			onPlayerCountUpdated(this.globalPlayerCount);
		}
		yield break;
	}

	// Token: 0x06000EEB RID: 3819 RVA: 0x0004ABB7 File Offset: 0x00048DB7
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		if (this.waitingForSessionListUpdate)
		{
			this.sessionInfoCache = sessionList;
			this.waitingForSessionListUpdate = false;
		}
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x000023F4 File Offset: 0x000005F4
	void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x040011D7 RID: 4567
	public FusionRegionCrawler.PlayerCountUpdated OnPlayerCountUpdated;

	// Token: 0x040011D8 RID: 4568
	private NetworkRunner regionRunner;

	// Token: 0x040011D9 RID: 4569
	private List<SessionInfo> sessionInfoCache;

	// Token: 0x040011DA RID: 4570
	private bool waitingForSessionListUpdate;

	// Token: 0x040011DB RID: 4571
	private int globalPlayerCount;

	// Token: 0x040011DC RID: 4572
	private float UpdateFrequency = 10f;

	// Token: 0x040011DD RID: 4573
	private bool refreshPlayerCountAutomatically = true;

	// Token: 0x040011DE RID: 4574
	private int tempSessionPlayerCount;

	// Token: 0x02000284 RID: 644
	// (Invoke) Token: 0x06000F01 RID: 3841
	public delegate void PlayerCountUpdated(int playerCount);
}
