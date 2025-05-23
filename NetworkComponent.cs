using System;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020002A3 RID: 675
[NetworkBehaviourWeaved(0)]
public abstract class NetworkComponent : NetworkView, IPunObservable, IStateAuthorityChanged, IOnPhotonViewOwnerChange, IPhotonViewCallback, IInRoomCallbacks, IPunInstantiateMagicCallback
{
	// Token: 0x06000FC0 RID: 4032 RVA: 0x0004EF85 File Offset: 0x0004D185
	internal virtual void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		this.AddToNetwork();
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x0004EF93 File Offset: 0x0004D193
	internal virtual void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x0004EFA1 File Offset: 0x0004D1A1
	protected override void Start()
	{
		base.Start();
		this.AddToNetwork();
	}

	// Token: 0x06000FC3 RID: 4035 RVA: 0x0004EFAF File Offset: 0x0004D1AF
	private void AddToNetwork()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x06000FC4 RID: 4036 RVA: 0x0004EFB7 File Offset: 0x0004D1B7
	public override void Spawned()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnSpawned();
		}
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x0004EFCB File Offset: 0x0004D1CB
	public override void FixedUpdateNetwork()
	{
		this.WriteDataFusion();
	}

	// Token: 0x06000FC6 RID: 4038 RVA: 0x0004EFD3 File Offset: 0x0004D1D3
	public override void Render()
	{
		if (!base.HasStateAuthority)
		{
			this.ReadDataFusion();
		}
	}

	// Token: 0x06000FC7 RID: 4039
	public abstract void WriteDataFusion();

	// Token: 0x06000FC8 RID: 4040
	public abstract void ReadDataFusion();

	// Token: 0x06000FC9 RID: 4041 RVA: 0x0004EFE3 File Offset: 0x0004D1E3
	public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		this.OnSpawned();
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x0004EFEB File Offset: 0x0004D1EB
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			this.WriteDataPUN(stream, info);
			return;
		}
		if (stream.IsReading)
		{
			this.ReadDataPUN(stream, info);
		}
	}

	// Token: 0x06000FCB RID: 4043
	protected abstract void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06000FCC RID: 4044
	protected abstract void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06000FCD RID: 4045 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnSpawned()
	{
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnOwnerSwitched(NetPlayer newOwningPlayer)
	{
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x0004F00E File Offset: 0x0004D20E
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		this.OnOwnerSwitched(NetworkSystem.Instance.GetPlayer(newMasterClient));
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x0004F024 File Offset: 0x0004D224
	public override void StateAuthorityChanged()
	{
		base.StateAuthorityChanged();
		if (base.Object == null)
		{
			return;
		}
		if (base.Object.StateAuthority == default(PlayerRef))
		{
			return;
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnOwnerSwitched(NetworkSystem.Instance.GetPlayer(base.Object.StateAuthority));
			return;
		}
		this.OnOwnerSwitched(NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x0004F09A File Offset: 0x0004D29A
	public void OnMasterClientSwitch(NetPlayer newMaster)
	{
		this.StateAuthorityChanged();
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnOwnerChange(Player newOwner, Player previousOwner)
	{
	}

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x06000FD7 RID: 4055 RVA: 0x0004F0A2 File Offset: 0x0004D2A2
	public bool IsLocallyOwned
	{
		get
		{
			return base.IsMine;
		}
	}

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x06000FD8 RID: 4056 RVA: 0x0004F0AA File Offset: 0x0004D2AA
	public bool ShouldWriteObjectData
	{
		get
		{
			return NetworkSystem.Instance.ShouldWriteObjectData(base.gameObject);
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x06000FD9 RID: 4057 RVA: 0x0004F0BC File Offset: 0x0004D2BC
	public bool ShouldUpdateobject
	{
		get
		{
			return NetworkSystem.Instance.ShouldUpdateObject(base.gameObject);
		}
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x06000FDA RID: 4058 RVA: 0x0004F0CE File Offset: 0x0004D2CE
	public int OwnerID
	{
		get
		{
			return NetworkSystem.Instance.GetOwningPlayerID(base.gameObject);
		}
	}

	// Token: 0x06000FDC RID: 4060 RVA: 0x0004F0E8 File Offset: 0x0004D2E8
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x0004F0F4 File Offset: 0x0004D2F4
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
