using System;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020005D8 RID: 1496
[NetworkBehaviourWeaved(1)]
internal class GameModeSerializer : GorillaSerializerMasterOnly, IStateAuthorityChanged
{
	// Token: 0x1700036E RID: 878
	// (get) Token: 0x06002471 RID: 9329 RVA: 0x000B7328 File Offset: 0x000B5528
	// (set) Token: 0x06002472 RID: 9330 RVA: 0x000B734E File Offset: 0x000B554E
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe int gameModeKeyInt
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameModeSerializer.gameModeKeyInt. Networked properties can only be accessed when Spawned() has been called.");
			}
			return this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameModeSerializer.gameModeKeyInt. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = value;
		}
	}

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x06002473 RID: 9331 RVA: 0x000B7375 File Offset: 0x000B5575
	public GorillaGameManager GameModeInstance
	{
		get
		{
			return this.gameModeInstance;
		}
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x000B7380 File Offset: 0x000B5580
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
		if (player != null)
		{
			GorillaNot.IncrementRPCCall(wrappedInfo, "OnSpawnSetupCheck");
		}
		GameModeSerializer activeNetworkHandler = global::GorillaGameModes.GameMode.ActiveNetworkHandler;
		if (player != null && player.InRoom)
		{
			if (!player.IsMasterClient)
			{
				GTDev.LogError<string>("SPAWN FAIL NOT MASTER :" + player.UserId + player.NickName, null);
				GorillaNot.instance.SendReport("trying to inappropriately create game managers", player.UserId, player.NickName);
				return false;
			}
			if (!this.netView.IsRoomView)
			{
				GTDev.LogError<string>("SPAWN FAIL ROOM VIEW" + player.UserId + player.NickName, null);
				GorillaNot.instance.SendReport("creating game manager as player object", player.UserId, player.NickName);
				return false;
			}
			if (activeNetworkHandler.IsNotNull() && activeNetworkHandler != this)
			{
				GTDev.LogError<string>("DUPLICATE CHECK" + player.UserId + player.NickName, null);
				GorillaNot.instance.SendReport("trying to create multiple game managers", player.UserId, player.NickName);
				return false;
			}
		}
		else if ((activeNetworkHandler.IsNotNull() && activeNetworkHandler != this) || !this.netView.IsRoomView)
		{
			GTDev.LogError<string>("ACTIVE HANDLER CHECK FAIL" + ((player != null) ? player.UserId : null) + ((player != null) ? player.NickName : null), null);
			GTDev.LogError<string>("existing game manager! destroying newly created manager", null);
			return false;
		}
		object[] instantiationData = wrappedInfo.punInfo.photonView.InstantiationData;
		if (instantiationData != null && instantiationData.Length >= 1)
		{
			object obj = instantiationData[0];
			if (obj is int)
			{
				int num = (int)obj;
				this.gameModeKey = (GameModeType)num;
				this.gameModeInstance = global::GorillaGameModes.GameMode.GetGameModeInstance(this.gameModeKey);
				if (this.gameModeInstance.IsNull() || !this.gameModeInstance.ValidGameMode())
				{
					return false;
				}
				this.serializeTarget = this.gameModeInstance;
				base.transform.parent = VRRigCache.Instance.NetworkParent;
				return true;
			}
		}
		GTDev.LogError<string>("missing instantiation data", null);
		return false;
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000B758E File Offset: 0x000B578E
	internal void Init(int gameModeType)
	{
		Debug.Log("<color=red>Init called</color>");
		this.gameModeKeyInt = gameModeType;
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x000B75A1 File Offset: 0x000B57A1
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
		this.netView.GetView.AddCallbackTarget(this);
		global::GorillaGameModes.GameMode.SetupGameModeRemote(this);
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x000B75BA File Offset: 0x000B57BA
	protected override void OnBeforeDespawn()
	{
		global::GorillaGameModes.GameMode.RemoveNetworkLink(this);
	}

	// Token: 0x06002478 RID: 9336 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x06002479 RID: 9337 RVA: 0x000B75C2 File Offset: 0x000B57C2
	[PunRPC]
	internal void RPC_ReportTag(int taggedPlayer, PhotonMessageInfo info)
	{
		this.ReportTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x000B75DB File Offset: 0x000B57DB
	[PunRPC]
	internal void RPC_ReportHit(PhotonMessageInfo info)
	{
		this.ReportHit(new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x000B75EC File Offset: 0x000B57EC
	[Rpc(RpcSources.All, RpcTargets.All)]
	internal unsafe void RPC_ReportTag(int taggedPlayer, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameModeSerializer::RPC_ReportTag(System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					if (base.Runner.HasAnyActiveConnections())
					{
						int num = 8;
						num += 4;
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1), data);
						*(int*)(data + num2) = taggedPlayer;
						num2 += 4;
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
		this.ReportTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x000B7738 File Offset: 0x000B5938
	[Rpc(RpcSources.All, RpcTargets.All)]
	internal unsafe void RPC_ReportHit(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameModeSerializer::RPC_ReportHit(Fusion.RpcInfo)", base.Object, 7);
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
		this.ReportHit(new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x000B7858 File Offset: 0x000B5A58
	private void ReportTag(NetPlayer taggedPlayer, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTag");
		NetPlayer sender = info.Sender;
		this.gameModeInstance.ReportTag(taggedPlayer, sender);
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x000B7888 File Offset: 0x000B5A88
	private void ReportHit(PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportContactWithLavaRPC");
		NetPlayer sender = info.Sender;
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.customMaps);
		bool flag2 = false;
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			InfectionLavaController instance = InfectionLavaController.Instance;
			flag2 = instance != null && instance.LavaCurrentlyActivated && (instance.SurfaceCenter - rigContainer.Rig.syncPos).sqrMagnitude < 2500f && instance.LavaPlane.GetDistanceToPoint(rigContainer.Rig.syncPos) < 5f;
		}
		if (flag || flag2)
		{
			this.GameModeInstance.HitPlayer(info.Sender);
		}
	}

	// Token: 0x0600247F RID: 9343 RVA: 0x000B7942 File Offset: 0x000B5B42
	[PunRPC]
	internal void RPC_BroadcastRoundComplete(PhotonMessageInfo info)
	{
		this.BroadcastRoundComplete(info);
	}

	// Token: 0x06002480 RID: 9344 RVA: 0x000B7950 File Offset: 0x000B5B50
	private void BroadcastRoundComplete(PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "BroadcastRoundComplete");
		if (info.Sender.IsMasterClient)
		{
			this.gameModeInstance.HandleRoundComplete();
		}
	}

	// Token: 0x06002481 RID: 9345 RVA: 0x000B7976 File Offset: 0x000B5B76
	protected override void FusionDataRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
		Debug.Log(this.gameModeData.GetType().Name);
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x000B798D File Offset: 0x000B5B8D
	protected override void FusionDataRPC(string method, RpcTarget target, params object[] parameters)
	{
		base.FusionDataRPC(method, target, parameters);
	}

	// Token: 0x06002483 RID: 9347 RVA: 0x000B7998 File Offset: 0x000B5B98
	void IStateAuthorityChanged.StateAuthorityChanged()
	{
		GameModeSerializer.FusionGameModeOwnerChanged(NetworkSystem.Instance.GetPlayer(base.Object.StateAuthority));
	}

	// Token: 0x06002485 RID: 9349 RVA: 0x000B79C1 File Offset: 0x000B5BC1
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.gameModeKeyInt = this._gameModeKeyInt;
	}

	// Token: 0x06002486 RID: 9350 RVA: 0x000B79D9 File Offset: 0x000B5BD9
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._gameModeKeyInt = this.gameModeKeyInt;
	}

	// Token: 0x06002487 RID: 9351 RVA: 0x000B79F0 File Offset: 0x000B5BF0
	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportTag@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int num3 = num2;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((GameModeSerializer)behaviour).RPC_ReportTag(num3, rpcInfo);
	}

	// Token: 0x06002488 RID: 9352 RVA: 0x000B7A60 File Offset: 0x000B5C60
	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((GameModeSerializer)behaviour).RPC_ReportHit(rpcInfo);
	}

	// Token: 0x04002991 RID: 10641
	[WeaverGenerated]
	[DefaultForProperty("gameModeKeyInt", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private int _gameModeKeyInt;

	// Token: 0x04002992 RID: 10642
	private GameModeType gameModeKey;

	// Token: 0x04002993 RID: 10643
	private GorillaGameManager gameModeInstance;

	// Token: 0x04002994 RID: 10644
	private FusionGameModeData gameModeData;

	// Token: 0x04002995 RID: 10645
	private Type currentGameDataType;

	// Token: 0x04002996 RID: 10646
	public static Action<NetPlayer> FusionGameModeOwnerChanged;
}
