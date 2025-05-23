using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Scripting;

// Token: 0x020000CF RID: 207
[NetworkBehaviourWeaved(11)]
public class GhostLabReliableState : NetworkComponent
{
	// Token: 0x17000061 RID: 97
	// (get) Token: 0x0600050C RID: 1292 RVA: 0x0001CF94 File Offset: 0x0001B194
	// (set) Token: 0x0600050D RID: 1293 RVA: 0x0001CFBE File Offset: 0x0001B1BE
	[Networked]
	[NetworkedWeaved(0, 11)]
	private unsafe GhostLabData NetData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GhostLabReliableState.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GhostLabData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GhostLabReliableState.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GhostLabData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0001CFE9 File Offset: 0x0001B1E9
	protected override void Awake()
	{
		base.Awake();
		this.singleDoorOpen = new bool[this.singleDoorCount];
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001D002 File Offset: 0x0001B202
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		Player localPlayer = PhotonNetwork.LocalPlayer;
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0001D014 File Offset: 0x0001B214
	public override void WriteDataFusion()
	{
		this.NetData = new GhostLabData((int)this.doorState, this.singleDoorOpen);
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0001D030 File Offset: 0x0001B230
	public override void ReadDataFusion()
	{
		this.doorState = (GhostLab.EntranceDoorsState)this.NetData.DoorState;
		for (int i = 0; i < this.singleDoorCount; i++)
		{
			this.singleDoorOpen[i] = this.NetData.OpenDoors[i];
		}
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x0001D088 File Offset: 0x0001B288
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.doorState);
		for (int i = 0; i < this.singleDoorOpen.Length; i++)
		{
			stream.SendNext(this.singleDoorOpen[i]);
		}
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x0001D0E4 File Offset: 0x0001B2E4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		this.doorState = (GhostLab.EntranceDoorsState)stream.ReceiveNext();
		for (int i = 0; i < this.singleDoorOpen.Length; i++)
		{
			this.singleDoorOpen[i] = (bool)stream.ReceiveNext();
		}
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0001D140 File Offset: 0x0001B340
	public void UpdateEntranceDoorsState(GhostLab.EntranceDoorsState newState)
	{
		if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
		{
			this.doorState = newState;
			return;
		}
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.IsMasterClient)
		{
			base.SendRPC("RemoteEntranceDoorState", RpcTarget.MasterClient, new object[] { newState });
		}
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x0001D1A0 File Offset: 0x0001B3A0
	public void UpdateSingleDoorState(int singleDoorIndex)
	{
		if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
		{
			this.singleDoorOpen[singleDoorIndex] = !this.singleDoorOpen[singleDoorIndex];
			return;
		}
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.IsMasterClient)
		{
			base.SendRPC("RemoteSingleDoorState", RpcTarget.MasterClient, new object[] { singleDoorIndex });
		}
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x0001D20C File Offset: 0x0001B40C
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPC_RemoteEntranceDoorState(GhostLab.EntranceDoorsState newState, RpcInfo info = default(RpcInfo))
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
							num += 4;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* data = SimulationMessage.GetData(ptr);
							int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
							*(GhostLab.EntranceDoorsState*)(data + num2) = newState;
							num2 += 4;
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GhostLabReliableState::RPC_RemoteEntranceDoorState(GhostLab/EntranceDoorsState,Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		GorillaNot.IncrementRPCCall(info, "RPC_RemoteEntranceDoorState");
		if (!base.IsMine)
		{
			return;
		}
		this.doorState = newState;
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0001D370 File Offset: 0x0001B570
	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPC_RemoteSingleDoorState(int doorIndex, RpcInfo info = default(RpcInfo))
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
							num += 4;
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* data = SimulationMessage.GetData(ptr);
							int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2), data);
							*(int*)(data + num2) = doorIndex;
							num2 += 4;
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GhostLabReliableState::RPC_RemoteSingleDoorState(System.Int32,Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		GorillaNot.IncrementRPCCall(info, "RPC_RemoteSingleDoorState");
		if (!base.IsMine)
		{
			return;
		}
		if (doorIndex >= this.singleDoorCount)
		{
			return;
		}
		this.singleDoorOpen[doorIndex] = !this.singleDoorOpen[doorIndex];
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0001D4E5 File Offset: 0x0001B6E5
	[PunRPC]
	public void RemoteEntranceDoorState(GhostLab.EntranceDoorsState newState, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemoteEntranceDoorState");
		if (!base.IsMine)
		{
			return;
		}
		this.doorState = newState;
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0001D502 File Offset: 0x0001B702
	[PunRPC]
	public void RemoteSingleDoorState(int doorIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemoteSingleDoorState");
		if (!base.IsMine)
		{
			return;
		}
		if (doorIndex >= this.singleDoorCount)
		{
			return;
		}
		this.singleDoorOpen[doorIndex] = !this.singleDoorOpen[doorIndex];
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x0001D535 File Offset: 0x0001B735
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.NetData = this._NetData;
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x0001D54D File Offset: 0x0001B74D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._NetData = this.NetData;
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0001D564 File Offset: 0x0001B764
	[NetworkRpcWeavedInvoker(1, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemoteEntranceDoorState@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		GhostLab.EntranceDoorsState entranceDoorsState = *(GhostLab.EntranceDoorsState*)(data + num);
		num += 4;
		GhostLab.EntranceDoorsState entranceDoorsState2 = entranceDoorsState;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((GhostLabReliableState)behaviour).RPC_RemoteEntranceDoorState(entranceDoorsState2, rpcInfo);
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x0001D5D8 File Offset: 0x0001B7D8
	[NetworkRpcWeavedInvoker(2, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemoteSingleDoorState@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int num3 = num2;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((GhostLabReliableState)behaviour).RPC_RemoteSingleDoorState(num3, rpcInfo);
	}

	// Token: 0x040005EE RID: 1518
	public GhostLab.EntranceDoorsState doorState;

	// Token: 0x040005EF RID: 1519
	public int singleDoorCount;

	// Token: 0x040005F0 RID: 1520
	public bool[] singleDoorOpen;

	// Token: 0x040005F1 RID: 1521
	[WeaverGenerated]
	[DefaultForProperty("NetData", 0, 11)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private GhostLabData _NetData;
}
