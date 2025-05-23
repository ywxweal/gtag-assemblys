using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000914 RID: 2324
[Serializable]
public class PhotonSignal<T1, T2, T3, T4> : PhotonSignal
{
	// Token: 0x1700058E RID: 1422
	// (get) Token: 0x06003897 RID: 14487 RVA: 0x00110FDF File Offset: 0x0010F1DF
	public override int argCount
	{
		get
		{
			return 4;
		}
	}

	// Token: 0x14000068 RID: 104
	// (add) Token: 0x06003898 RID: 14488 RVA: 0x00110FE2 File Offset: 0x0010F1E2
	// (remove) Token: 0x06003899 RID: 14489 RVA: 0x00111016 File Offset: 0x0010F216
	public new event OnSignalReceived<T1, T2, T3, T4> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x0600389A RID: 14490 RVA: 0x00110B93 File Offset: 0x0010ED93
	public PhotonSignal(string signalID)
		: base(signalID)
	{
	}

	// Token: 0x0600389B RID: 14491 RVA: 0x00110B9C File Offset: 0x0010ED9C
	public PhotonSignal(int signalID)
		: base(signalID)
	{
	}

	// Token: 0x0600389C RID: 14492 RVA: 0x00111033 File Offset: 0x0010F233
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x0600389D RID: 14493 RVA: 0x00111042 File Offset: 0x0010F242
	public void Raise(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		this.Raise(this._receivers, arg1, arg2, arg3, arg4);
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x00111058 File Offset: 0x0010F258
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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
		array[5] = arg4;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo photonSignalInfo = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, photonSignalInfo);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x0600389F RID: 14495 RVA: 0x00111114 File Offset: 0x0010F314
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 t;
		T2 t2;
		T3 t3;
		T4 t4;
		if (!args.TryParseArgs(2, out t, out t2, out t3, out t4))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3, T4>(this._callbacks, t, t2, t3, t4, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3, T4>(this._callbacks, t, t2, t3, t4, info);
	}

	// Token: 0x060038A0 RID: 14496 RVA: 0x0011115C File Offset: 0x0010F35C
	public new static implicit operator PhotonSignal<T1, T2, T3, T4>(string s)
	{
		return new PhotonSignal<T1, T2, T3, T4>(s);
	}

	// Token: 0x060038A1 RID: 14497 RVA: 0x00111164 File Offset: 0x0010F364
	public new static explicit operator PhotonSignal<T1, T2, T3, T4>(int i)
	{
		return new PhotonSignal<T1, T2, T3, T4>(i);
	}

	// Token: 0x04003DF5 RID: 15861
	private OnSignalReceived<T1, T2, T3, T4> _callbacks;

	// Token: 0x04003DF6 RID: 15862
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3, T4>).FullName.GetStaticHash();
}
