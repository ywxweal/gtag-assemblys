using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020002B7 RID: 695
[RequireComponent(typeof(PhotonView), typeof(NetworkObject))]
[NetworkBehaviourWeaved(0)]
public class NetworkView : NetworkBehaviour, IStateAuthorityChanged, IPunOwnershipCallbacks
{
	// Token: 0x170001CD RID: 461
	// (get) Token: 0x060010A0 RID: 4256 RVA: 0x0005018D File Offset: 0x0004E38D
	public bool IsMine
	{
		get
		{
			return this.punView != null && this.punView.IsMine;
		}
	}

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x060010A1 RID: 4257 RVA: 0x000501AA File Offset: 0x0004E3AA
	public bool IsValid
	{
		get
		{
			return this.punView != null;
		}
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x060010A2 RID: 4258 RVA: 0x000501AA File Offset: 0x0004E3AA
	public bool HasView
	{
		get
		{
			return this.punView != null;
		}
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x060010A3 RID: 4259 RVA: 0x000501B8 File Offset: 0x0004E3B8
	public bool IsRoomView
	{
		get
		{
			return this.punView.IsRoomView;
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x060010A4 RID: 4260 RVA: 0x000501C5 File Offset: 0x0004E3C5
	public PhotonView GetView
	{
		get
		{
			return this.punView;
		}
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x060010A5 RID: 4261 RVA: 0x000501CD File Offset: 0x0004E3CD
	public NetPlayer Owner
	{
		get
		{
			return NetworkSystem.Instance.GetPlayer(this.punView.Owner);
		}
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x060010A6 RID: 4262 RVA: 0x000501E4 File Offset: 0x0004E3E4
	public int ViewID
	{
		get
		{
			return this.punView.ViewID;
		}
	}

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x060010A7 RID: 4263 RVA: 0x000501F1 File Offset: 0x0004E3F1
	// (set) Token: 0x060010A8 RID: 4264 RVA: 0x000501FE File Offset: 0x0004E3FE
	internal OwnershipOption OwnershipTransfer
	{
		get
		{
			return this.punView.OwnershipTransfer;
		}
		set
		{
			this.punView.OwnershipTransfer = value;
			if (this.reliableView != null)
			{
				this.reliableView.OwnershipTransfer = value;
			}
		}
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x060010A9 RID: 4265 RVA: 0x00050226 File Offset: 0x0004E426
	// (set) Token: 0x060010AA RID: 4266 RVA: 0x00050233 File Offset: 0x0004E433
	public int OwnerActorNr
	{
		get
		{
			return this.punView.OwnerActorNr;
		}
		set
		{
			this.punView.OwnerActorNr = value;
			if (this.reliableView != null)
			{
				this.reliableView.OwnerActorNr = value;
			}
		}
	}

	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x060010AB RID: 4267 RVA: 0x0005025B File Offset: 0x0004E45B
	// (set) Token: 0x060010AC RID: 4268 RVA: 0x00050268 File Offset: 0x0004E468
	public int ControllerActorNr
	{
		get
		{
			return this.punView.ControllerActorNr;
		}
		set
		{
			this.punView.ControllerActorNr = value;
			if (this.reliableView != null)
			{
				this.reliableView.ControllerActorNr = value;
			}
		}
	}

	// Token: 0x060010AD RID: 4269 RVA: 0x00050290 File Offset: 0x0004E490
	private void GetViews()
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		if (components.Length > 1)
		{
			if (components[0].Synchronization == ViewSynchronization.UnreliableOnChange)
			{
				this.punView = components[0];
				this.reliableView = components[1];
			}
			else if (components[0].Synchronization == ViewSynchronization.ReliableDeltaCompressed)
			{
				this.reliableView = components[0];
				this.punView = components[1];
			}
		}
		else
		{
			this.punView = components[0];
		}
		if (this.punView == null)
		{
			this.punView = base.GetComponent<PhotonView>();
		}
		if (this.fusionView == null)
		{
			this.fusionView = base.GetComponent<NetworkObject>();
		}
	}

	// Token: 0x060010AE RID: 4270 RVA: 0x00050325 File Offset: 0x0004E525
	protected virtual void Awake()
	{
		this.GetViews();
	}

	// Token: 0x060010AF RID: 4271 RVA: 0x0005032D File Offset: 0x0004E52D
	protected virtual void Start()
	{
		if (this._sceneObject)
		{
			NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
		}
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x00050348 File Offset: 0x0004E548
	public void SendRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
		Player playerRef = (targetPlayer as PunNetPlayer).PlayerRef;
		this.punView.RPC(method, playerRef, parameters);
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x0005036F File Offset: 0x0004E56F
	public void SendRPC(string method, RpcTarget target, params object[] parameters)
	{
		this.punView.RPC(method, target, parameters);
	}

	// Token: 0x060010B2 RID: 4274 RVA: 0x00050380 File Offset: 0x0004E580
	public void SendRPC(string method, int target, params object[] parameters)
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom == null || !currentRoom.Players.ContainsKey(target))
		{
			return;
		}
		this.punView.RPC(method, currentRoom.Players[target], parameters);
	}

	// Token: 0x060010B3 RID: 4275 RVA: 0x000503BE File Offset: 0x0004E5BE
	public override void Spawned()
	{
		base.Spawned();
		this._spawned = true;
	}

	// Token: 0x060010B4 RID: 4276 RVA: 0x000503CD File Offset: 0x0004E5CD
	public void RequestOwnership()
	{
		this.GetView.RequestOwnership();
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x000503DA File Offset: 0x0004E5DA
	public void ReleaseOwnership()
	{
		this.changingStatAuth = true;
		base.Object.ReleaseStateAuthority();
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x000503EE File Offset: 0x0004E5EE
	public virtual void StateAuthorityChanged()
	{
		if (this.changingStatAuth)
		{
			this.changingStatAuth = false;
		}
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
	}

	// Token: 0x060010B9 RID: 4281 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x060010BB RID: 4283 RVA: 0x000023F4 File Offset: 0x000005F4
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x000023F4 File Offset: 0x000005F4
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x040012F5 RID: 4853
	[SerializeField]
	private PhotonView punView;

	// Token: 0x040012F6 RID: 4854
	[SerializeField]
	private PhotonView reliableView;

	// Token: 0x040012F7 RID: 4855
	[SerializeField]
	internal NetworkObject fusionView;

	// Token: 0x040012F8 RID: 4856
	[SerializeField]
	protected bool _sceneObject;

	// Token: 0x040012F9 RID: 4857
	private bool _spawned;

	// Token: 0x040012FA RID: 4858
	private bool changingStatAuth;
}
