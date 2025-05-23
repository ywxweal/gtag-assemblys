using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000915 RID: 2325
[Serializable]
public class PhotonSignal<T1, T2, T3, T4, T5> : PhotonSignal
{
	// Token: 0x1700058F RID: 1423
	// (get) Token: 0x060038A2 RID: 14498 RVA: 0x000A7C5D File Offset: 0x000A5E5D
	public override int argCount
	{
		get
		{
			return 5;
		}
	}

	// Token: 0x14000069 RID: 105
	// (add) Token: 0x060038A3 RID: 14499 RVA: 0x001110AF File Offset: 0x0010F2AF
	// (remove) Token: 0x060038A4 RID: 14500 RVA: 0x001110E3 File Offset: 0x0010F2E3
	public new event OnSignalReceived<T1, T2, T3, T4, T5> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2, T3, T4, T5>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x060038A5 RID: 14501 RVA: 0x00110ABB File Offset: 0x0010ECBB
	public PhotonSignal(string signalID)
		: base(signalID)
	{
	}

	// Token: 0x060038A6 RID: 14502 RVA: 0x00110AC4 File Offset: 0x0010ECC4
	public PhotonSignal(int signalID)
		: base(signalID)
	{
	}

	// Token: 0x060038A7 RID: 14503 RVA: 0x00111100 File Offset: 0x0010F300
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x060038A8 RID: 14504 RVA: 0x0011110F File Offset: 0x0010F30F
	public void Raise(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		this.Raise(this._receivers, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x060038A9 RID: 14505 RVA: 0x00111124 File Offset: 0x0010F324
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
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
		array[6] = arg5;
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo photonSignalInfo = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, photonSignalInfo);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x060038AA RID: 14506 RVA: 0x001111E8 File Offset: 0x0010F3E8
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 t;
		T2 t2;
		T3 t3;
		T4 t4;
		T5 t5;
		if (!args.TryParseArgs(2, out t, out t2, out t3, out t4, out t5))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2, T3, T4, T5>(this._callbacks, t, t2, t3, t4, t5, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2, T3, T4, T5>(this._callbacks, t, t2, t3, t4, t5, info);
	}

	// Token: 0x060038AB RID: 14507 RVA: 0x00111236 File Offset: 0x0010F436
	public new static implicit operator PhotonSignal<T1, T2, T3, T4, T5>(string s)
	{
		return new PhotonSignal<T1, T2, T3, T4, T5>(s);
	}

	// Token: 0x060038AC RID: 14508 RVA: 0x0011123E File Offset: 0x0010F43E
	public new static explicit operator PhotonSignal<T1, T2, T3, T4, T5>(int i)
	{
		return new PhotonSignal<T1, T2, T3, T4, T5>(i);
	}

	// Token: 0x04003DF6 RID: 15862
	private OnSignalReceived<T1, T2, T3, T4, T5> _callbacks;

	// Token: 0x04003DF7 RID: 15863
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2, T3, T4, T5>).FullName.GetStaticHash();
}
