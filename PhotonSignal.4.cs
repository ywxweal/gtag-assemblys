using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000913 RID: 2323
[Serializable]
public class PhotonSignal<T1, T2, T3> : PhotonSignal
{
	// Token: 0x1700058D RID: 1421
	// (get) Token: 0x0600388A RID: 14474 RVA: 0x000C23EE File Offset: 0x000C05EE
	public override int argCount
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x14000067 RID: 103
	// (add) Token: 0x0600388B RID: 14475 RVA: 0x00110D77 File Offset: 0x0010EF77
	// (remove) Token: 0x0600388C RID: 14476 RVA: 0x00110DAB File Offset: 0x0010EFAB
	public new event OnSignalReceived<T1, T2, T3> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x0600388D RID: 14477 RVA: 0x00110ABB File Offset: 0x0010ECBB
	public PhotonSignal(string signalID)
		: base(signalID)
	{
	}

	// Token: 0x0600388E RID: 14478 RVA: 0x00110AC4 File Offset: 0x0010ECC4
	public PhotonSignal(int signalID)
		: base(signalID)
	{
	}

	// Token: 0x0600388F RID: 14479 RVA: 0x00110DC8 File Offset: 0x0010EFC8
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06003890 RID: 14480 RVA: 0x00110DD7 File Offset: 0x0010EFD7
	public void Raise(T1 arg1, T2 arg2, T3 arg3)
	{
		this.Raise(this._receivers, arg1, arg2, arg3);
	}

	// Token: 0x06003891 RID: 14481 RVA: 0x00110DE8 File Offset: 0x0010EFE8
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!this._enabled)
		{
			return;
		}
		if (this._mute)
		{
			return;
		}
		RaiseEventOptions raiseEventOptions = PhotonSignal.gGroupToOptions[receivers];
		object[] array = PhotonUtils.FetchScratchArray(2 + this.argCount);
		int serverTimestamp = PhotonNetwork.ServerTimestamp;
		array[0] = this._signalID;
		array[1] = serverTimestamp;
		array[2] = arg1;
		array[3] = arg2;
		array[4] = arg3;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo photonSignalInfo = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, photonSignalInfo);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06003892 RID: 14482 RVA: 0x00110E98 File Offset: 0x0010F098
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 t;
		T2 t2;
		T3 t3;
		if (!args.TryParseArgs(2, out t, out t2, out t3))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3>(this._callbacks, t, t2, t3, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3>(this._callbacks, t, t2, t3, info);
	}

	// Token: 0x06003893 RID: 14483 RVA: 0x00110EDC File Offset: 0x0010F0DC
	public new static implicit operator PhotonSignal<T1, T2, T3>(string s)
	{
		return new PhotonSignal<T1, T2, T3>(s);
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x00110EE4 File Offset: 0x0010F0E4
	public new static explicit operator PhotonSignal<T1, T2, T3>(int i)
	{
		return new PhotonSignal<T1, T2, T3>(i);
	}

	// Token: 0x04003DF2 RID: 15858
	private OnSignalReceived<T1, T2, T3> _callbacks;

	// Token: 0x04003DF3 RID: 15859
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3>).FullName.GetStaticHash();
}
