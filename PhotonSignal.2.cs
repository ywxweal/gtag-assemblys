using System;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000911 RID: 2321
[Serializable]
public class PhotonSignal<T1> : PhotonSignal
{
	// Token: 0x1700058B RID: 1419
	// (get) Token: 0x06003873 RID: 14451 RVA: 0x00047642 File Offset: 0x00045842
	public override int argCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x14000065 RID: 101
	// (add) Token: 0x06003874 RID: 14452 RVA: 0x00110B42 File Offset: 0x0010ED42
	// (remove) Token: 0x06003875 RID: 14453 RVA: 0x00110B76 File Offset: 0x0010ED76
	public new event OnSignalReceived<T1> OnSignal
	{
		add
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1>)Delegate.Remove(this._callbacks, value);
			this._callbacks = (OnSignalReceived<T1>)Delegate.Combine(this._callbacks, value);
		}
		remove
		{
			if (value == null)
			{
				return;
			}
			this._callbacks = (OnSignalReceived<T1>)Delegate.Remove(this._callbacks, value);
		}
	}

	// Token: 0x06003876 RID: 14454 RVA: 0x00110B93 File Offset: 0x0010ED93
	public PhotonSignal(string signalID)
		: base(signalID)
	{
	}

	// Token: 0x06003877 RID: 14455 RVA: 0x00110B9C File Offset: 0x0010ED9C
	public PhotonSignal(int signalID)
		: base(signalID)
	{
	}

	// Token: 0x06003878 RID: 14456 RVA: 0x00110BA5 File Offset: 0x0010EDA5
	public override void ClearListeners()
	{
		this._callbacks = null;
		base.ClearListeners();
	}

	// Token: 0x06003879 RID: 14457 RVA: 0x00110BB4 File Offset: 0x0010EDB4
	public void Raise(T1 arg1)
	{
		this.Raise(this._receivers, arg1);
	}

	// Token: 0x0600387A RID: 14458 RVA: 0x00110BC4 File Offset: 0x0010EDC4
	public void Raise(ReceiverGroup receivers, T1 arg1)
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
		if (this._localOnly || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
		{
			PhotonSignalInfo photonSignalInfo = new PhotonSignalInfo(PhotonUtils.LocalNetPlayer, serverTimestamp);
			this._Relay(array, photonSignalInfo);
			return;
		}
		PhotonNetwork.RaiseEvent(177, array, raiseEventOptions, PhotonSignal.gSendReliable);
	}

	// Token: 0x0600387B RID: 14459 RVA: 0x00110C64 File Offset: 0x0010EE64
	protected override void _Relay(object[] args, PhotonSignalInfo info)
	{
		T1 t;
		if (!args.TryParseArgs(2, out t))
		{
			return;
		}
		if (!this._safeInvoke)
		{
			PhotonSignal._Invoke<T1>(this._callbacks, t, info);
			return;
		}
		PhotonSignal._SafeInvoke<T1>(this._callbacks, t, info);
	}

	// Token: 0x0600387C RID: 14460 RVA: 0x00110CA0 File Offset: 0x0010EEA0
	public new static implicit operator PhotonSignal<T1>(string s)
	{
		return new PhotonSignal<T1>(s);
	}

	// Token: 0x0600387D RID: 14461 RVA: 0x00110CA8 File Offset: 0x0010EEA8
	public new static explicit operator PhotonSignal<T1>(int i)
	{
		return new PhotonSignal<T1>(i);
	}

	// Token: 0x04003DEF RID: 15855
	private OnSignalReceived<T1> _callbacks;

	// Token: 0x04003DF0 RID: 15856
	private static readonly int kSignature = typeof(PhotonSignal<T1>).FullName.GetStaticHash();
}
