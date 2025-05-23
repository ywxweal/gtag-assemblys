using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200027C RID: 636
public class FusionCallbackHandler : SimulationBehaviour, INetworkRunnerCallbacks
{
	// Token: 0x06000E9C RID: 3740 RVA: 0x000498D4 File Offset: 0x00047AD4
	public void Setup(NetworkSystemFusion parentController)
	{
		this.parent = parentController;
		this.parent.runner.AddCallbacks(new INetworkRunnerCallbacks[] { this });
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x000498F7 File Offset: 0x00047AF7
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.RemoveCallbacks();
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x00049908 File Offset: 0x00047B08
	private async void RemoveCallbacks()
	{
		await Task.Delay(500);
		this.parent.runner.RemoveCallbacks(new INetworkRunnerCallbacks[] { this });
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x0004993F File Offset: 0x00047B3F
	public void OnConnectedToServer(NetworkRunner runner)
	{
		this.parent.OnJoinedSession();
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x0004994C File Offset: 0x00047B4C
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		this.parent.OnJoinFailed(reason);
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x0004995C File Offset: 0x00047B5C
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
		this.parent.CustomAuthenticationResponse(data);
		Debug.Log("Received custom auth response:");
		foreach (KeyValuePair<string, object> keyValuePair in data)
		{
			Debug.Log(keyValuePair.Key + ":" + (keyValuePair.Value as string));
		}
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x000499DC File Offset: 0x00047BDC
	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		this.parent.OnDisconnectedFromSession();
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x000499E9 File Offset: 0x00047BE9
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.parent.MigrateHost(runner, hostMigrationToken);
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x000499F8 File Offset: 0x00047BF8
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		NetworkedInput input2 = NetInput.GetInput();
		input.Set<NetworkedInput>(input2);
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x00049A14 File Offset: 0x00047C14
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerJoined(player);
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x00049A22 File Offset: 0x00047C22
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerLeft(player);
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x00049A30 File Offset: 0x00047C30
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		this.parent.OnRunnerShutDown();
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x00049A40 File Offset: 0x00047C40
	[Rpc(Channel = RpcChannel.Reliable)]
	public unsafe static void RPC_OnEventRaisedReliable(NetworkRunner runner, byte eventCode, byte[] byteData, bool hasOps, byte[] netOptsData, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			if (runner.HasAnyActiveConnections())
			{
				int num = 8;
				num += 4;
				num += (byteData.Length * 1 + 4 + 3) & -4;
				num += 4;
				num += (netOptsData.Length * 1 + 4 + 3) & -4;
				SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")), data);
				data[num2] = eventCode;
				num2 += (1 + 3) & -4;
				*(int*)(data + num2) = byteData.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray<byte>((void*)(data + num2), byteData) + 3) & -4) + num2;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), hasOps);
				num2 += 4;
				*(int*)(data + num2) = netOptsData.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray<byte>((void*)(data + num2), netOptsData) + 3) & -4) + num2;
				ptr->Offset = num2 * 8;
				ptr->SetStatic();
				runner.SendRpc(ptr);
			}
			info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
		}
		object obj = byteData.ByteDeserialize();
		NetEventOptions netEventOptions = null;
		if (hasOps)
		{
			netEventOptions = (NetEventOptions)netOptsData.ByteDeserialize();
		}
		if (!FusionCallbackHandler.CanRecieveEvent(runner, netEventOptions, info))
		{
			return;
		}
		NetworkSystem.Instance.RaiseEvent(eventCode, obj, info.Source.PlayerId);
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x00049C44 File Offset: 0x00047E44
	[Rpc(Channel = RpcChannel.Unreliable)]
	public unsafe static void RPC_OnEventRaisedUnreliable(NetworkRunner runner, byte eventCode, byte[] byteData, bool hasOps, byte[] netOptsData, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			if (runner.HasAnyActiveConnections())
			{
				int num = 8;
				num += 4;
				num += (byteData.Length * 1 + 4 + 3) & -4;
				num += 4;
				num += (netOptsData.Length * 1 + 4 + 3) & -4;
				SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")), data);
				data[num2] = eventCode;
				num2 += (1 + 3) & -4;
				*(int*)(data + num2) = byteData.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray<byte>((void*)(data + num2), byteData) + 3) & -4) + num2;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), hasOps);
				num2 += 4;
				*(int*)(data + num2) = netOptsData.Length;
				num2 += 4;
				num2 = ((Native.CopyFromArray<byte>((void*)(data + num2), netOptsData) + 3) & -4) + num2;
				ptr->Offset = num2 * 8;
				ptr->SetUnreliable();
				ptr->SetStatic();
				runner.SendRpc(ptr);
			}
			info = RpcInfo.FromLocal(runner, RpcChannel.Unreliable, RpcHostMode.SourceIsServer);
		}
		object obj = byteData.ByteDeserialize();
		NetEventOptions netEventOptions = null;
		if (hasOps)
		{
			netEventOptions = (NetEventOptions)netOptsData.ByteDeserialize();
		}
		if (!FusionCallbackHandler.CanRecieveEvent(runner, netEventOptions, info))
		{
			return;
		}
		NetworkSystem.Instance.RaiseEvent(eventCode, obj, info.Source.PlayerId);
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x00049E50 File Offset: 0x00048050
	private static bool CanRecieveEvent(NetworkRunner runner, NetEventOptions opts, RpcInfo info)
	{
		if (opts != null)
		{
			if (opts.Reciever != NetEventOptions.RecieverTarget.all)
			{
				if (opts.Reciever == NetEventOptions.RecieverTarget.master && !NetworkSystem.Instance.IsMasterClient)
				{
					return false;
				}
				if (info.Source == runner.LocalPlayer)
				{
					return false;
				}
			}
			if (opts.TargetActors != null && !opts.TargetActors.Contains(runner.LocalPlayer.PlayerId))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x00049EC4 File Offset: 0x000480C4
	[NetworkRpcStaticWeavedInvoker("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnEventRaisedReliable@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte b = data[num];
		num += (1 + 3) & -4;
		byte b2 = b;
		byte[] array = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray<byte>(array, (void*)(data + num)) + 3) & -4) + num;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool flag2 = flag;
		byte[] array2 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray<byte>(array2, (void*)(data + num)) + 3) & -4) + num;
		RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionCallbackHandler.RPC_OnEventRaisedReliable(runner, b2, array, flag2, array2, rpcInfo);
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x00049FE0 File Offset: 0x000481E0
	[NetworkRpcStaticWeavedInvoker("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnEventRaisedUnreliable@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte b = data[num];
		num += (1 + 3) & -4;
		byte b2 = b;
		byte[] array = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray<byte>(array, (void*)(data + num)) + 3) & -4) + num;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool flag2 = flag;
		byte[] array2 = new byte[*(int*)(data + num)];
		num += 4;
		num = ((Native.CopyToArray<byte>(array2, (void*)(data + num)) + 3) & -4) + num;
		RpcInfo rpcInfo = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(runner, b2, array, flag2, array2, rpcInfo);
	}

	// Token: 0x040011CB RID: 4555
	private NetworkSystemFusion parent;
}
