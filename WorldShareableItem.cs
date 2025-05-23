using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020003C1 RID: 961
[NetworkBehaviourWeaved(0)]
public class WorldShareableItem : NetworkComponent, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001651 RID: 5713 RVA: 0x0006C4AC File Offset: 0x0006A6AC
	// (set) Token: 0x06001652 RID: 5714 RVA: 0x0006C4B4 File Offset: 0x0006A6B4
	[DevInspectorShow]
	public TransferrableObject.PositionState transferableObjectState { get; set; }

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06001653 RID: 5715 RVA: 0x0006C4BD File Offset: 0x0006A6BD
	// (set) Token: 0x06001654 RID: 5716 RVA: 0x0006C4C5 File Offset: 0x0006A6C5
	public TransferrableObject.ItemStates transferableObjectItemState { get; set; }

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06001655 RID: 5717 RVA: 0x0006C4CE File Offset: 0x0006A6CE
	// (set) Token: 0x06001656 RID: 5718 RVA: 0x0006C4D6 File Offset: 0x0006A6D6
	public TransferrableObject.PositionState transferableObjectStateNetworked { get; set; }

	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06001657 RID: 5719 RVA: 0x0006C4DF File Offset: 0x0006A6DF
	// (set) Token: 0x06001658 RID: 5720 RVA: 0x0006C4E7 File Offset: 0x0006A6E7
	public TransferrableObject.ItemStates transferableObjectItemStateNetworked { get; set; }

	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06001659 RID: 5721 RVA: 0x0006C4F0 File Offset: 0x0006A6F0
	// (set) Token: 0x0600165A RID: 5722 RVA: 0x0006C4F8 File Offset: 0x0006A6F8
	[DevInspectorShow]
	public WorldTargetItem target
	{
		get
		{
			return this._target;
		}
		set
		{
			this._target = value;
		}
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x0006C501 File Offset: 0x0006A701
	protected override void Awake()
	{
		base.Awake();
		this.guard = base.GetComponent<RequestableOwnershipGuard>();
		this.teleportSerializer = base.GetComponent<TransformViewTeleportSerializer>();
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x0600165C RID: 5724 RVA: 0x0006C531 File Offset: 0x0006A731
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		if (GTAppState.isQuitting)
		{
			return;
		}
		base.OnEnable();
		this.guard.AddCallbackTarget(this);
		WorldShareableItemManager.Register(this);
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x0006C56C File Offset: 0x0006A76C
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		if (this.target == null || !this.target.transferrableObject.isSceneObject)
		{
			return;
		}
		PhotonView[] components = base.GetComponents<PhotonView>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ViewID = 0;
		}
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
		this.guard.RemoveCallbackTarget(this);
		this.rpcCallBack = null;
		this.onOwnerChangeCb = null;
		WorldShareableItemManager.Unregister(this);
	}

	// Token: 0x0600165E RID: 5726 RVA: 0x0006C5EC File Offset: 0x0006A7EC
	public void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		WorldShareableItemManager.Unregister(this);
	}

	// Token: 0x0600165F RID: 5727 RVA: 0x0006C5FC File Offset: 0x0006A7FC
	public void SetupSharableViewIDs(NetPlayer player, int slotID)
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		int num = player.ActorNumber * 1000 + 990 + slotID * 2;
		this.guard.giveCreatorAbsoluteAuthority = true;
		if (num != photonView.ViewID)
		{
			photonView.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2;
			photonView2.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2 + 1;
			this.guard.SetCreator(player);
		}
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x0006C688 File Offset: 0x0006A888
	public void ResetViews()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		photonView.ViewID = 0;
		photonView2.ViewID = 0;
	}

	// Token: 0x06001661 RID: 5729 RVA: 0x0006C6B8 File Offset: 0x0006A8B8
	public void SetupSharableObject(int itemIDx, NetPlayer owner, Transform targetXform)
	{
		this.target = WorldTargetItem.GenerateTargetFromPlayerAndID(owner, itemIDx);
		if (this.target.targetObject != targetXform)
		{
			Debug.LogError(string.Format("The target object found a transform that does not match the target transform, this should never happen. owner: {0} itemIDx: {1} targetXformPath: {2}, target.targetObject: {3}", new object[]
			{
				owner,
				itemIDx,
				targetXform.GetPath(),
				this.target.targetObject.GetPath()
			}));
		}
		TransferrableObject component = this.target.targetObject.GetComponent<TransferrableObject>();
		this.validShareable = component.canDrop || component.shareable || component.allowWorldSharableInstance;
		if (!this.validShareable)
		{
			Debug.LogError(string.Format("tried to setup an invalid shareable {0} {1} {2}", owner, itemIDx, targetXform.GetPath()));
			base.gameObject.SetActive(false);
			this.Invalidate();
			return;
		}
		this.guard.AddCallbackTarget(component);
		this.guard.giveCreatorAbsoluteAuthority = true;
		component.SetWorldShareableItem(this);
	}

	// Token: 0x06001662 RID: 5730 RVA: 0x0006C7A9 File Offset: 0x0006A9A9
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		base.OnPhotonInstantiate(info);
	}

	// Token: 0x06001663 RID: 5731 RVA: 0x0006C7B4 File Offset: 0x0006A9B4
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (this.onOwnerChangeCb != null)
		{
			NetPlayer player = NetworkSystem.Instance.GetPlayer(newOwner);
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(previousOwner);
			this.onOwnerChangeCb(player, player2);
		}
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06001664 RID: 5732 RVA: 0x0006C7EE File Offset: 0x0006A9EE
	// (set) Token: 0x06001665 RID: 5733 RVA: 0x0006C7F6 File Offset: 0x0006A9F6
	[DevInspectorShow]
	public bool EnableRemoteSync
	{
		get
		{
			return this.enableRemoteSync;
		}
		set
		{
			this.enableRemoteSync = value;
		}
	}

	// Token: 0x06001666 RID: 5734 RVA: 0x0006C800 File Offset: 0x0006AA00
	public void TriggeredUpdate()
	{
		if (!this.IsTargetValid())
		{
			return;
		}
		if (this.guard.isTrulyMine)
		{
			if (this.target.transferrableObject)
			{
				this.target.transferrableObject.worldShareableInstance != this;
			}
			base.transform.position = this.target.targetObject.position;
			base.transform.rotation = this.target.targetObject.rotation;
			return;
		}
		if (!base.IsMine && this.EnableRemoteSync)
		{
			this.target.targetObject.position = base.transform.position;
			this.target.targetObject.rotation = base.transform.rotation;
		}
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x0006C8C9 File Offset: 0x0006AAC9
	public void SyncToSceneObject(TransferrableObject transferrableObject)
	{
		this.target = WorldTargetItem.GenerateTargetFromWorldSharableItem(null, -2, transferrableObject.transform);
		base.transform.parent = null;
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x0006C8EB File Offset: 0x0006AAEB
	public void SetupSceneObjectOnNetwork(NetPlayer owner)
	{
		this.guard.SetOwnership(owner, false, false);
	}

	// Token: 0x06001669 RID: 5737 RVA: 0x0006C8FB File Offset: 0x0006AAFB
	public bool IsTargetValid()
	{
		return this.target != null;
	}

	// Token: 0x0600166A RID: 5738 RVA: 0x0006C906 File Offset: 0x0006AB06
	public void Invalidate()
	{
		this.target = null;
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x0600166B RID: 5739 RVA: 0x0006C920 File Offset: 0x0006AB20
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == null)
		{
			return;
		}
		WorldShareableItem.CachedData cachedData;
		if (this.cachedDatas.TryGetValue(toPlayer, out cachedData))
		{
			this.transferableObjectState = cachedData.cachedTransferableObjectState;
			this.transferableObjectItemState = cachedData.cachedTransferableObjectItemState;
			this.cachedDatas.Remove(toPlayer);
		}
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x0006C966 File Offset: 0x0006AB66
	public override void WriteDataFusion()
	{
		this.transferableObjectItemStateNetworked = this.transferableObjectItemState;
		this.transferableObjectStateNetworked = this.transferableObjectState;
	}

	// Token: 0x0600166D RID: 5741 RVA: 0x0006C980 File Offset: 0x0006AB80
	public override void ReadDataFusion()
	{
		this.transferableObjectItemState = this.transferableObjectItemStateNetworked;
		this.transferableObjectState = this.transferableObjectStateNetworked;
	}

	// Token: 0x0600166E RID: 5742 RVA: 0x0006C99A File Offset: 0x0006AB9A
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.transferableObjectState);
		stream.SendNext(this.transferableObjectItemState);
	}

	// Token: 0x0600166F RID: 5743 RVA: 0x0006C9C0 File Offset: 0x0006ABC0
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (player != this.guard.actualOwner)
		{
			Debug.Log("Blocking info from non owner");
			this.cachedDatas.AddOrUpdate(player, new WorldShareableItem.CachedData
			{
				cachedTransferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext(),
				cachedTransferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext()
			});
			return;
		}
		this.transferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext();
		this.transferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext();
	}

	// Token: 0x06001670 RID: 5744 RVA: 0x0006CA52 File Offset: 0x0006AC52
	[PunRPC]
	internal void RPCWorldShareable(PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		GorillaNot.IncrementRPCCall(info, "RPCWorldShareable");
		if (this.rpcCallBack == null)
		{
			return;
		}
		this.rpcCallBack();
	}

	// Token: 0x06001671 RID: 5745 RVA: 0x00047642 File Offset: 0x00045842
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return true;
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x00047642 File Offset: 0x00045842
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return true;
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06001675 RID: 5749 RVA: 0x0006CA84 File Offset: 0x0006AC84
	public void SetWillTeleport()
	{
		this.teleportSerializer.SetWillTeleport();
	}

	// Token: 0x06001677 RID: 5751 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001678 RID: 5752 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040018DC RID: 6364
	private bool validShareable = true;

	// Token: 0x040018DD RID: 6365
	public RequestableOwnershipGuard guard;

	// Token: 0x040018DE RID: 6366
	private TransformViewTeleportSerializer teleportSerializer;

	// Token: 0x040018DF RID: 6367
	[DevInspectorShow]
	[CanBeNull]
	private WorldTargetItem _target;

	// Token: 0x040018E0 RID: 6368
	public WorldShareableItem.OnOwnerChangeDelegate onOwnerChangeCb;

	// Token: 0x040018E1 RID: 6369
	public Action rpcCallBack;

	// Token: 0x040018E2 RID: 6370
	private bool enableRemoteSync = true;

	// Token: 0x040018E3 RID: 6371
	public Dictionary<NetPlayer, WorldShareableItem.CachedData> cachedDatas = new Dictionary<NetPlayer, WorldShareableItem.CachedData>();

	// Token: 0x020003C2 RID: 962
	// (Invoke) Token: 0x0600167A RID: 5754
	public delegate void Delegate();

	// Token: 0x020003C3 RID: 963
	// (Invoke) Token: 0x0600167E RID: 5758
	public delegate void OnOwnerChangeDelegate(NetPlayer newOwner, NetPlayer prevOwner);

	// Token: 0x020003C4 RID: 964
	public struct CachedData
	{
		// Token: 0x040018E4 RID: 6372
		public TransferrableObject.PositionState cachedTransferableObjectState;

		// Token: 0x040018E5 RID: 6373
		public TransferrableObject.ItemStates cachedTransferableObjectItemState;
	}
}
