using System;
using Fusion;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000483 RID: 1155
[NetworkBehaviourWeaved(31)]
public class BattleGameModeData : FusionGameModeData
{
	// Token: 0x1700030F RID: 783
	// (get) Token: 0x06001C44 RID: 7236 RVA: 0x0008AC33 File Offset: 0x00088E33
	// (set) Token: 0x06001C45 RID: 7237 RVA: 0x0008AC5D File Offset: 0x00088E5D
	[Networked]
	[NetworkedWeaved(0, 31)]
	private unsafe PaintbrawlData PaintbrawlData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BattleGameModeData.PaintbrawlData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(PaintbrawlData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BattleGameModeData.PaintbrawlData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(PaintbrawlData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x17000310 RID: 784
	// (get) Token: 0x06001C46 RID: 7238 RVA: 0x0008AC88 File Offset: 0x00088E88
	// (set) Token: 0x06001C47 RID: 7239 RVA: 0x0008AC95 File Offset: 0x00088E95
	public override object Data
	{
		get
		{
			return this.PaintbrawlData;
		}
		set
		{
			this.PaintbrawlData = (PaintbrawlData)value;
		}
	}

	// Token: 0x06001C48 RID: 7240 RVA: 0x0008ACA3 File Offset: 0x00088EA3
	public override void Spawned()
	{
		this.serializer = base.GetComponent<GameModeSerializer>();
		this.battleTarget = (GorillaPaintbrawlManager)this.serializer.GameModeInstance;
	}

	// Token: 0x06001C49 RID: 7241 RVA: 0x0008ACC8 File Offset: 0x00088EC8
	[Rpc]
	public unsafe void RPC_ReportSlinshotHit(int taggedPlayerID, Vector3 hitLocation, int projectileCount, RpcInfo rpcInfo = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void BattleGameModeData::RPC_ReportSlinshotHit(System.Int32,UnityEngine.Vector3,System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					if (base.Runner.HasAnyActiveConnections())
					{
						int num = 8;
						num += 4;
						num += 12;
						num += 4;
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
						*(int*)(data + num2) = taggedPlayerID;
						num2 += 4;
						*(Vector3*)(data + num2) = hitLocation;
						num2 += 12;
						*(int*)(data + num2) = projectileCount;
						num2 += 4;
						ptr->Offset = num2 * 8;
						base.Runner.SendRpc(ptr);
					}
					if ((localAuthorityMask & 7) != 0)
					{
						rpcInfo = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_0012;
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_0012:
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(rpcInfo);
		GorillaNot.IncrementRPCCall(photonMessageInfoWrapped, "RPC_ReportSlinshotHit");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(taggedPlayerID);
		this.battleTarget.ReportSlingshotHit(player, hitLocation, projectileCount, photonMessageInfoWrapped);
	}

	// Token: 0x06001C4B RID: 7243 RVA: 0x0008AE8F File Offset: 0x0008908F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.PaintbrawlData = this._PaintbrawlData;
	}

	// Token: 0x06001C4C RID: 7244 RVA: 0x0008AEA7 File Offset: 0x000890A7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._PaintbrawlData = this.PaintbrawlData;
	}

	// Token: 0x06001C4D RID: 7245 RVA: 0x0008AEBC File Offset: 0x000890BC
	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportSlinshotHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int num3 = num2;
		Vector3 vector = *(Vector3*)(data + num);
		num += 12;
		Vector3 vector2 = vector;
		int num4 = *(int*)(data + num);
		num += 4;
		int num5 = num4;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((BattleGameModeData)behaviour).RPC_ReportSlinshotHit(num3, vector2, num5, rpcInfo);
	}

	// Token: 0x04001F7C RID: 8060
	[WeaverGenerated]
	[DefaultForProperty("PaintbrawlData", 0, 31)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private PaintbrawlData _PaintbrawlData;

	// Token: 0x04001F7D RID: 8061
	private GorillaPaintbrawlManager battleTarget;

	// Token: 0x04001F7E RID: 8062
	private GameModeSerializer serializer;
}
