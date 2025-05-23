using System;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000D6 RID: 214
[NetworkBehaviourWeaved(11)]
public class SecondLookSkeletonSynchValues : NetworkComponent
{
	// Token: 0x17000068 RID: 104
	// (get) Token: 0x06000553 RID: 1363 RVA: 0x0001EE22 File Offset: 0x0001D022
	// (set) Token: 0x06000554 RID: 1364 RVA: 0x0001EE4C File Offset: 0x0001D04C
	[Networked]
	[NetworkedWeaved(0, 11)]
	public unsafe SkeletonNetData NetData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing SecondLookSkeletonSynchValues.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(SkeletonNetData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing SecondLookSkeletonSynchValues.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(SkeletonNetData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0001EE77 File Offset: 0x0001D077
	protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
	{
		base.OnOwnerSwitched(newOwningPlayer);
		if (newOwningPlayer.IsLocal)
		{
			this.mySkeleton.SetNodes();
			if (this.mySkeleton.currentState != this.currentState)
			{
				this.mySkeleton.ChangeState(this.currentState);
			}
		}
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0001EEB7 File Offset: 0x0001D0B7
	public override void WriteDataFusion()
	{
		this.NetData = new SkeletonNetData((int)this.currentState, this.position, this.rotation, this.currentNode, this.nextNode, this.angerPoint);
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001EEE8 File Offset: 0x0001D0E8
	public override void ReadDataFusion()
	{
		this.currentState = (SecondLookSkeleton.GhostState)this.NetData.CurrentState;
		Vector3 vector = this.NetData.Position;
		(ref this.position).SetValueSafe(in vector);
		Quaternion quaternion = this.NetData.Rotation;
		(ref this.rotation).SetValueSafe(in quaternion);
		this.currentNode = this.NetData.CurrentNode;
		this.nextNode = this.NetData.NextNode;
		this.angerPoint = this.NetData.AngerPoint;
		if (this.mySkeleton.tapped && this.currentState != this.mySkeleton.currentState)
		{
			this.mySkeleton.ChangeState(this.currentState);
		}
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0001EFB0 File Offset: 0x0001D1B0
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.mySkeleton.currentState);
		stream.SendNext(this.mySkeleton.spookyGhost.transform.position);
		stream.SendNext(this.mySkeleton.spookyGhost.transform.rotation);
		stream.SendNext(this.currentNode);
		stream.SendNext(this.nextNode);
		stream.SendNext(this.angerPoint);
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001F05C File Offset: 0x0001D25C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		this.currentState = (SecondLookSkeleton.GhostState)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		(ref this.position).SetValueSafe(in vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		(ref this.rotation).SetValueSafe(in quaternion);
		this.currentNode = (int)stream.ReceiveNext();
		this.nextNode = (int)stream.ReceiveNext();
		this.angerPoint = (int)stream.ReceiveNext();
		if (this.mySkeleton.tapped && this.currentState != this.mySkeleton.currentState)
		{
			this.mySkeleton.ChangeState(this.currentState);
		}
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0001F128 File Offset: 0x0001D328
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPC_RemoteActiveGhost(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) != 0)
				{
					if ((localAuthorityMask & 1) != 1)
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							int num = 8;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* data = SimulationMessage.GetData(ptr);
							int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
							ptr->Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 1) == 0)
						{
							return;
						}
					}
					info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
					goto IL_0012;
				}
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void SecondLookSkeletonSynchValues::RPC_RemoteActiveGhost(Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		GorillaNot.IncrementRPCCall(info, "RPC_RemoteActiveGhost");
		if (!base.IsMine)
		{
			return;
		}
		this.mySkeleton.RemoteActivateGhost();
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0001F26C File Offset: 0x0001D46C
	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPC_RemotePlayerSeen(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void SecondLookSkeletonSynchValues::RPC_RemotePlayerSeen(Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					if (base.Runner.HasAnyActiveConnections())
					{
						int num = 8;
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2), data);
						ptr->Offset = num2 * 8;
						base.Runner.SendRpc(ptr);
					}
					if ((localAuthorityMask & 7) != 0)
					{
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_0012;
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		GorillaNot.IncrementRPCCall(info, "RPC_RemotePlayerSeen");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Source);
		if (!this.mySkeleton.playersSeen.Contains(player))
		{
			this.mySkeleton.RemotePlayerSeen(player);
		}
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0001F3C0 File Offset: 0x0001D5C0
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPC_RemotePlayerCaught(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) != 0)
				{
					if ((localAuthorityMask & 1) != 1)
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							int num = 8;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* data = SimulationMessage.GetData(ptr);
							int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 3), data);
							ptr->Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 1) == 0)
						{
							return;
						}
					}
					info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
					goto IL_0012;
				}
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void SecondLookSkeletonSynchValues::RPC_RemotePlayerCaught(Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		GorillaNot.IncrementRPCCall(info, "RPC_RemotePlayerCaught");
		if (!base.IsMine)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Source);
		if (this.mySkeleton.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			this.mySkeleton.RemotePlayerCaught(player);
		}
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0001F521 File Offset: 0x0001D721
	[PunRPC]
	public void RemoteActivateGhost(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemoteActivateGhost");
		if (!base.IsMine)
		{
			return;
		}
		this.mySkeleton.RemoteActivateGhost();
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x0001F544 File Offset: 0x0001D744
	[PunRPC]
	public void RemotePlayerSeen(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemotePlayerSeen");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (!this.mySkeleton.playersSeen.Contains(player))
		{
			this.mySkeleton.RemotePlayerSeen(player);
		}
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x0001F58C File Offset: 0x0001D78C
	[PunRPC]
	public void RemotePlayerCaught(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemotePlayerCaught");
		if (!base.IsMine)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.mySkeleton.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			this.mySkeleton.RemotePlayerCaught(player);
		}
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0001F5D8 File Offset: 0x0001D7D8
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.NetData = this._NetData;
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0001F5F0 File Offset: 0x0001D7F0
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._NetData = this.NetData;
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x0001F604 File Offset: 0x0001D804
	[NetworkRpcWeavedInvoker(1, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemoteActiveGhost@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((SecondLookSkeletonSynchValues)behaviour).RPC_RemoteActiveGhost(rpcInfo);
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x0001F658 File Offset: 0x0001D858
	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemotePlayerSeen@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((SecondLookSkeletonSynchValues)behaviour).RPC_RemotePlayerSeen(rpcInfo);
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x0001F6AC File Offset: 0x0001D8AC
	[NetworkRpcWeavedInvoker(3, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemotePlayerCaught@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((SecondLookSkeletonSynchValues)behaviour).RPC_RemotePlayerCaught(rpcInfo);
	}

	// Token: 0x04000641 RID: 1601
	public SecondLookSkeleton.GhostState currentState;

	// Token: 0x04000642 RID: 1602
	public Vector3 position;

	// Token: 0x04000643 RID: 1603
	public Quaternion rotation;

	// Token: 0x04000644 RID: 1604
	public SecondLookSkeleton mySkeleton;

	// Token: 0x04000645 RID: 1605
	public int currentNode;

	// Token: 0x04000646 RID: 1606
	public int nextNode;

	// Token: 0x04000647 RID: 1607
	public int angerPoint;

	// Token: 0x04000648 RID: 1608
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("NetData", 0, 11)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private SkeletonNetData _NetData;
}
