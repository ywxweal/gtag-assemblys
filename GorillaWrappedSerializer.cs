using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005DE RID: 1502
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaWrappedSerializer : NetworkBehaviour, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x17000373 RID: 883
	// (get) Token: 0x060024B0 RID: 9392 RVA: 0x000B8504 File Offset: 0x000B6704
	public NetworkView NetView
	{
		get
		{
			return this.netView;
		}
	}

	// Token: 0x17000374 RID: 884
	// (get) Token: 0x060024B1 RID: 9393 RVA: 0x000B850C File Offset: 0x000B670C
	// (set) Token: 0x060024B2 RID: 9394 RVA: 0x000B8514 File Offset: 0x000B6714
	protected virtual object data { get; set; }

	// Token: 0x17000375 RID: 885
	// (get) Token: 0x060024B3 RID: 9395 RVA: 0x000B851D File Offset: 0x000B671D
	public bool IsLocallyOwned
	{
		get
		{
			return this.netView.IsMine;
		}
	}

	// Token: 0x17000376 RID: 886
	// (get) Token: 0x060024B4 RID: 9396 RVA: 0x000B852A File Offset: 0x000B672A
	public bool IsValid
	{
		get
		{
			return this.netView.IsValid;
		}
	}

	// Token: 0x060024B5 RID: 9397 RVA: 0x000B8537 File Offset: 0x000B6737
	private void Awake()
	{
		if (this.netView == null)
		{
			this.netView = base.GetComponent<NetworkView>();
		}
	}

	// Token: 0x060024B6 RID: 9398 RVA: 0x000B8554 File Offset: 0x000B6754
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.netView == null || !this.netView.IsValid)
		{
			return;
		}
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		this.ProcessSpawn(photonMessageInfoWrapped);
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x000B858C File Offset: 0x000B678C
	public override void Spawned()
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(base.Object.StateAuthority.PlayerId, base.Runner.Tick.Raw);
		this.ProcessSpawn(photonMessageInfoWrapped);
	}

	// Token: 0x060024B8 RID: 9400 RVA: 0x000B85CC File Offset: 0x000B67CC
	private void ProcessSpawn(PhotonMessageInfoWrapped wrappedInfo)
	{
		this.successfullInstantiate = this.OnSpawnSetupCheck(wrappedInfo, out this.targetObject, out this.targetType);
		if (this.successfullInstantiate)
		{
			GameObject gameObject = this.targetObject;
			IWrappedSerializable wrappedSerializable = ((gameObject != null) ? gameObject.GetComponent(this.targetType) : null) as IWrappedSerializable;
			if (wrappedSerializable != null)
			{
				this.serializeTarget = wrappedSerializable;
			}
			if (this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccesfullySpawned(wrappedInfo);
			return;
		}
		this.FailedToSpawn();
	}

	// Token: 0x060024B9 RID: 9401 RVA: 0x000B8647 File Offset: 0x000B6847
	protected virtual bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IWrappedSerializable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x060024BA RID: 9402
	protected abstract void OnSuccesfullySpawned(PhotonMessageInfoWrapped info);

	// Token: 0x060024BB RID: 9403 RVA: 0x000B8660 File Offset: 0x000B6860
	private void FailedToSpawn()
	{
		Debug.LogError("Failed to network instantiate");
		if (this.netView.IsMine)
		{
			PhotonNetwork.Destroy(this.netView.GetView);
			return;
		}
		this.netView.GetView.ObservedComponents.Remove(this);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060024BC RID: 9404
	protected abstract void OnFailedSpawn();

	// Token: 0x060024BD RID: 9405 RVA: 0x000B830D File Offset: 0x000B650D
	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x000B86B8 File Offset: 0x000B68B8
	public override void FixedUpdateNetwork()
	{
		this.data = this.serializeTarget.OnSerializeWrite();
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x000B86CB File Offset: 0x000B68CB
	public override void Render()
	{
		if (!base.Object.HasStateAuthority)
		{
			this.serializeTarget.OnSerializeRead(this.data);
		}
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x000B86EC File Offset: 0x000B68EC
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || info.Sender != info.photonView.Owner || this.serializeTarget == null)
		{
			return;
		}
		if (stream.IsWriting)
		{
			this.serializeTarget.OnSerializeWrite(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeRead(stream, info);
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x000B8740 File Offset: 0x000B6940
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x060024C2 RID: 9410 RVA: 0x000B8740 File Offset: 0x000B6940
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x060024C3 RID: 9411
	protected abstract void OnBeforeDespawn();

	// Token: 0x060024C4 RID: 9412 RVA: 0x000B8748 File Offset: 0x000B6948
	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T t = base.gameObject.AddComponent<T>();
		this.netView.GetView.RefreshRpcMonoBehaviourCache();
		t.SetClassTarget(this.serializeTarget, this);
		return t;
	}

	// Token: 0x060024C5 RID: 9413 RVA: 0x000B8778 File Offset: 0x000B6978
	public void SendRPC(string rpcName, bool targetOthers, params object[] data)
	{
		RpcTarget rpcTarget = (targetOthers ? RpcTarget.Others : RpcTarget.MasterClient);
		this.netView.SendRPC(rpcName, rpcTarget, data);
	}

	// Token: 0x060024C6 RID: 9414 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void FusionDataRPC(string method, RpcTarget target, params object[] parameters)
	{
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void FusionDataRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
	}

	// Token: 0x060024C8 RID: 9416 RVA: 0x000B879B File Offset: 0x000B699B
	public void SendRPC(string rpcName, NetPlayer targetPlayer, params object[] data)
	{
		this.netView.GetView.RPC(rpcName, ((PunNetPlayer)targetPlayer).PlayerRef, data);
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x000023F4 File Offset: 0x000005F4
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x000023F4 File Offset: 0x000005F4
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x040029B8 RID: 10680
	protected bool successfullInstantiate;

	// Token: 0x040029B9 RID: 10681
	protected IWrappedSerializable serializeTarget;

	// Token: 0x040029BA RID: 10682
	private Type targetType;

	// Token: 0x040029BB RID: 10683
	protected GameObject targetObject;

	// Token: 0x040029BC RID: 10684
	[SerializeField]
	protected NetworkView netView;
}
