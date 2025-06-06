﻿using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000912 RID: 2322
[Serializable]
public class PhotonSignal<T1, T2> : PhotonSignal
{
	// Token: 0x1700058C RID: 1420
	// (get) Token: 0x0600387F RID: 14463 RVA: 0x000121FB File Offset: 0x000103FB
	public override int argCount
	{
		get
		{
			return 2;
		}
	}

	// Token: 0x14000066 RID: 102
	// (add) Token: 0x06003880 RID: 14464 RVA: 0x00110CCB File Offset: 0x0010EECB
	// (remove) Token: 0x06003881 RID: 14465 RVA: 0x00110CFF File Offset: 0x0010EEFF
	public new event OnSignalReceived<T1, T2> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1, T2>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06003882 RID: 14466 RVA: 0x00110B93 File Offset: 0x0010ED93
	public PhotonSignal(string signalID)
		: base(signalID)
	{
	}

	// Token: 0x06003883 RID: 14467 RVA: 0x00110B9C File Offset: 0x0010ED9C
	public PhotonSignal(int signalID)
		: base(signalID)
	{
	}

	// Token: 0x06003884 RID: 14468 RVA: 0x00110D1C File Offset: 0x0010EF1C
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06003885 RID: 14469 RVA: 0x00110D2B File Offset: 0x0010EF2B
	public void Raise(T1 arg1, T2 arg2)
	{
		this.Raise(this._receivers, arg1, arg2);
	}

	// Token: 0x06003886 RID: 14470 RVA: 0x00110D3C File Offset: 0x0010EF3C
	public void Raise(ReceiverGroup receivers, T1 arg1, T2 arg2)
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
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo photonSignalInfo = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, photonSignalInfo);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x06003887 RID: 14471 RVA: 0x00110DE4 File Offset: 0x0010EFE4
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 t;
		T2 t2;
		if (!args.TryParseArgs(2, out t, out t2))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1, T2>(this._callbacks, t, t2, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1, T2>(this._callbacks, t, t2, info);
	}

	// Token: 0x06003888 RID: 14472 RVA: 0x00110E24 File Offset: 0x0010F024
	public new static implicit operator PhotonSignal<T1, T2>(string s)
	{
		return new PhotonSignal<T1, T2>(s);
	}

	// Token: 0x06003889 RID: 14473 RVA: 0x00110E2C File Offset: 0x0010F02C
	public new static explicit operator PhotonSignal<T1, T2>(int i)
	{
		return new PhotonSignal<T1, T2>(i);
	}

	// Token: 0x04003DF1 RID: 15857
	private OnSignalReceived<T1, T2> _callbacks;

	// Token: 0x04003DF2 RID: 15858
	private static readonly int kSignature = typeof(PhotonSignal<T1, T2>).FullName.GetStaticHash();
}
