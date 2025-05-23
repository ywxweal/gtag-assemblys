using System;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020005DB RID: 1499
[RequireComponent(typeof(PhotonView))]
internal class GorillaSerializer : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
	// Token: 0x06002499 RID: 9369 RVA: 0x000B81D4 File Offset: 0x000B63D4
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || this.serializeTarget == null || !this.ValidOnSerialize(stream, in info))
		{
			return;
		}
		if (stream.IsReading)
		{
			this.serializeTarget.OnSerializeRead(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeWrite(stream, info);
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000B8220 File Offset: 0x000B6420
	public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.photonView == null)
		{
			return;
		}
		this.successfullInstantiate = this.OnInstantiateSetup(info, out this.targetObject, out this.targetType);
		if (this.successfullInstantiate)
		{
			if (this.targetType != null && this.targetObject.IsNotNull())
			{
				IGorillaSerializeable gorillaSerializeable = this.targetObject.GetComponent(this.targetType) as IGorillaSerializeable;
				if (gorillaSerializeable != null)
				{
					this.serializeTarget = gorillaSerializeable;
				}
			}
			if (this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccessfullInstantiate(info);
			return;
		}
		if (PhotonNetwork.InRoom && this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(this.photonView);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		this.photonView.ObservedComponents.Remove(this);
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x000B82F6 File Offset: 0x000B64F6
	protected virtual bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IGorillaSerializeable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x000B830D File Offset: 0x000B650D
	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	// Token: 0x0600249E RID: 9374 RVA: 0x000B8325 File Offset: 0x000B6525
	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T t = base.gameObject.AddComponent<T>();
		this.photonView.RefreshRpcMonoBehaviourCache();
		return t;
	}

	// Token: 0x0600249F RID: 9375 RVA: 0x000B8340 File Offset: 0x000B6540
	public void SendRPC(string rpcName, bool targetOthers, params object[] data)
	{
		RpcTarget rpcTarget = (targetOthers ? RpcTarget.Others : RpcTarget.MasterClient);
		this.photonView.RPC(rpcName, rpcTarget, data);
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x000B8363 File Offset: 0x000B6563
	public void SendRPC(string rpcName, Player targetPlayer, params object[] data)
	{
		this.photonView.RPC(rpcName, targetPlayer, data);
	}

	// Token: 0x040029AF RID: 10671
	protected bool successfullInstantiate;

	// Token: 0x040029B0 RID: 10672
	protected IGorillaSerializeable serializeTarget;

	// Token: 0x040029B1 RID: 10673
	private Type targetType;

	// Token: 0x040029B2 RID: 10674
	protected GameObject targetObject;

	// Token: 0x040029B3 RID: 10675
	[SerializeField]
	protected PhotonView photonView;
}
