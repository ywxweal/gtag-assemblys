using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020008FF RID: 2303
[Serializable]
public class PhotonEvent : IOnEventCallback, IEquatable<PhotonEvent>
{
	// Token: 0x17000585 RID: 1413
	// (get) Token: 0x060037E9 RID: 14313 RVA: 0x0010FB45 File Offset: 0x0010DD45
	// (set) Token: 0x060037EA RID: 14314 RVA: 0x0010FB4D File Offset: 0x0010DD4D
	public bool reliable
	{
		get
		{
			return this._reliable;
		}
		set
		{
			this._reliable = value;
		}
	}

	// Token: 0x17000586 RID: 1414
	// (get) Token: 0x060037EB RID: 14315 RVA: 0x0010FB56 File Offset: 0x0010DD56
	// (set) Token: 0x060037EC RID: 14316 RVA: 0x0010FB5E File Offset: 0x0010DD5E
	public bool failSilent
	{
		get
		{
			return this._failSilent;
		}
		set
		{
			this._failSilent = value;
		}
	}

	// Token: 0x060037ED RID: 14317 RVA: 0x0010FB67 File Offset: 0x0010DD67
	private PhotonEvent()
	{
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x0010FB76 File Offset: 0x0010DD76
	public PhotonEvent(int eventId)
	{
		if (eventId == -1)
		{
			throw new Exception(string.Format("<{0}> cannot be {1}.", "eventId", -1));
		}
		this._eventId = eventId;
		this.Enable();
	}

	// Token: 0x060037EF RID: 14319 RVA: 0x0010FBB1 File Offset: 0x0010DDB1
	public PhotonEvent(string eventId)
		: this(StaticHash.Compute(eventId))
	{
	}

	// Token: 0x060037F0 RID: 14320 RVA: 0x0010FBBF File Offset: 0x0010DDBF
	public PhotonEvent(int eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
		: this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x060037F1 RID: 14321 RVA: 0x0010FBCF File Offset: 0x0010DDCF
	public PhotonEvent(string eventId, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
		: this(eventId)
	{
		this.AddCallback(callback);
	}

	// Token: 0x060037F2 RID: 14322 RVA: 0x0010FBE0 File Offset: 0x0010DDE0
	~PhotonEvent()
	{
		this.Dispose();
	}

	// Token: 0x060037F3 RID: 14323 RVA: 0x0010FC0C File Offset: 0x0010DE0C
	public void AddCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		if (this._delegate != null)
		{
			foreach (Delegate @delegate in this._delegate.GetInvocationList())
			{
				if (@delegate != null && @delegate.Equals(callback))
				{
					return;
				}
			}
		}
		this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Combine(this._delegate, callback);
	}

	// Token: 0x060037F4 RID: 14324 RVA: 0x0010FC7A File Offset: 0x0010DE7A
	public void RemoveCallback(Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (this._disposed)
		{
			return;
		}
		if (callback != null)
		{
			this._delegate = (Action<int, int, object[], PhotonMessageInfoWrapped>)Delegate.Remove(this._delegate, callback);
		}
	}

	// Token: 0x060037F5 RID: 14325 RVA: 0x0010FC9F File Offset: 0x0010DE9F
	public void Enable()
	{
		if (this._disposed)
		{
			return;
		}
		if (this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
		this._enabled = true;
	}

	// Token: 0x060037F6 RID: 14326 RVA: 0x0010FCC7 File Offset: 0x0010DEC7
	public void Disable()
	{
		if (this._disposed)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
		this._enabled = false;
	}

	// Token: 0x060037F7 RID: 14327 RVA: 0x0010FCEF File Offset: 0x0010DEEF
	public void Dispose()
	{
		this._delegate = null;
		if (this._enabled)
		{
			this._enabled = false;
			if (Application.isPlaying)
			{
				PhotonNetwork.RemoveCallbackTarget(this);
			}
		}
		this._eventId = -1;
		this._disposed = true;
	}

	// Token: 0x14000063 RID: 99
	// (add) Token: 0x060037F8 RID: 14328 RVA: 0x0010FD24 File Offset: 0x0010DF24
	// (remove) Token: 0x060037F9 RID: 14329 RVA: 0x0010FD58 File Offset: 0x0010DF58
	public static event Action<PhotonEvent, EventData, Exception> OnError;

	// Token: 0x060037FA RID: 14330 RVA: 0x0010FD8C File Offset: 0x0010DF8C
	void IOnEventCallback.OnEvent(EventData ev)
	{
		if (ev.Code != 176)
		{
			return;
		}
		if (this._disposed)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		try
		{
			object[] array = (object[])ev.CustomData;
			if (array.Length == 0)
			{
				throw new Exception("Invalid/missing event data!");
			}
			int num = (int)array[0];
			int eventId = this._eventId;
			if (num == -1)
			{
				throw new Exception(string.Format("Invalid {0} ID! ({1})", "sender", -1));
			}
			if (eventId == -1)
			{
				throw new Exception(string.Format("Invalid {0} ID! ({1})", "receiver", -1));
			}
			object[] array2 = ((array.Length == 1) ? Array.Empty<object>() : array.Skip(1).ToArray<object>());
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(ev.Sender, PhotonNetwork.ServerTimestamp);
			this.InvokeDelegate(num, eventId, array2, photonMessageInfoWrapped);
		}
		catch (Exception ex)
		{
			Action<PhotonEvent, EventData, Exception> onError = PhotonEvent.OnError;
			if (onError != null)
			{
				onError(this, ev, ex);
			}
		}
	}

	// Token: 0x060037FB RID: 14331 RVA: 0x0010FE84 File Offset: 0x0010E084
	private void InvokeDelegate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		Action<int, int, object[], PhotonMessageInfoWrapped> @delegate = this._delegate;
		if (@delegate == null)
		{
			return;
		}
		@delegate(sender, target, args, info);
	}

	// Token: 0x060037FC RID: 14332 RVA: 0x0010FE9B File Offset: 0x0010E09B
	public void RaiseLocal(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.Local, args);
	}

	// Token: 0x060037FD RID: 14333 RVA: 0x0010FEA5 File Offset: 0x0010E0A5
	public void RaiseOthers(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteOthers, args);
	}

	// Token: 0x060037FE RID: 14334 RVA: 0x0010FEAF File Offset: 0x0010E0AF
	public void RaiseAll(params object[] args)
	{
		this.Raise(PhotonEvent.RaiseMode.RemoteAll, args);
	}

	// Token: 0x060037FF RID: 14335 RVA: 0x0010FEBC File Offset: 0x0010E0BC
	private void Raise(PhotonEvent.RaiseMode mode, params object[] args)
	{
		if (this._disposed)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (!this._enabled)
		{
			return;
		}
		SendOptions sendOptions = (this._reliable ? PhotonEvent.gSendReliable : PhotonEvent.gSendUnreliable);
		switch (mode)
		{
		case PhotonEvent.RaiseMode.Local:
			this.InvokeDelegate(this._eventId, this._eventId, args, new PhotonMessageInfoWrapped(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.ServerTimestamp));
			return;
		case PhotonEvent.RaiseMode.RemoteOthers:
		{
			object[] array = args.Prepend(this._eventId).ToArray<object>();
			PhotonNetwork.RaiseEvent(176, array, PhotonEvent.gReceiversOthers, sendOptions);
			return;
		}
		case PhotonEvent.RaiseMode.RemoteAll:
		{
			object[] array2 = args.Prepend(this._eventId).ToArray<object>();
			PhotonNetwork.RaiseEvent(176, array2, PhotonEvent.gReceiversAll, sendOptions);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06003800 RID: 14336 RVA: 0x0010FF88 File Offset: 0x0010E188
	public bool Equals(PhotonEvent other)
	{
		return !(other == null) && (this._eventId == other._eventId && this._enabled == other._enabled && this._reliable == other._reliable && this._failSilent == other._failSilent) && this._disposed == other._disposed;
	}

	// Token: 0x06003801 RID: 14337 RVA: 0x0010FFE8 File Offset: 0x0010E1E8
	public override bool Equals(object obj)
	{
		PhotonEvent photonEvent = obj as PhotonEvent;
		return photonEvent != null && this.Equals(photonEvent);
	}

	// Token: 0x06003802 RID: 14338 RVA: 0x00110008 File Offset: 0x0010E208
	public override int GetHashCode()
	{
		int staticHash = this._eventId.GetStaticHash();
		int num = StaticHash.Compute(this._enabled, this._reliable, this._failSilent, this._disposed);
		return StaticHash.Compute(staticHash, num);
	}

	// Token: 0x06003803 RID: 14339 RVA: 0x00110044 File Offset: 0x0010E244
	public static PhotonEvent operator +(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.AddCallback(callback);
		return photonEvent;
	}

	// Token: 0x06003804 RID: 14340 RVA: 0x00110062 File Offset: 0x0010E262
	public static PhotonEvent operator -(PhotonEvent photonEvent, Action<int, int, object[], PhotonMessageInfoWrapped> callback)
	{
		if (photonEvent == null)
		{
			throw new ArgumentNullException("photonEvent");
		}
		photonEvent.RemoveCallback(callback);
		return photonEvent;
	}

	// Token: 0x06003805 RID: 14341 RVA: 0x00110080 File Offset: 0x0010E280
	static PhotonEvent()
	{
		PhotonEvent.gSendUnreliable.Encrypt = true;
		PhotonEvent.gSendReliable = SendOptions.SendReliable;
		PhotonEvent.gSendReliable.Encrypt = true;
	}

	// Token: 0x06003806 RID: 14342 RVA: 0x001100D9 File Offset: 0x0010E2D9
	public static bool operator ==(PhotonEvent x, PhotonEvent y)
	{
		return EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x06003807 RID: 14343 RVA: 0x001100E7 File Offset: 0x0010E2E7
	public static bool operator !=(PhotonEvent x, PhotonEvent y)
	{
		return !EqualityComparer<PhotonEvent>.Default.Equals(x, y);
	}

	// Token: 0x04003DC9 RID: 15817
	private const int INVALID_ID = -1;

	// Token: 0x04003DCA RID: 15818
	[SerializeField]
	private int _eventId = -1;

	// Token: 0x04003DCB RID: 15819
	[SerializeField]
	private bool _enabled;

	// Token: 0x04003DCC RID: 15820
	[SerializeField]
	private bool _reliable;

	// Token: 0x04003DCD RID: 15821
	[SerializeField]
	private bool _failSilent;

	// Token: 0x04003DCE RID: 15822
	[NonSerialized]
	private bool _disposed;

	// Token: 0x04003DCF RID: 15823
	private Action<int, int, object[], PhotonMessageInfoWrapped> _delegate;

	// Token: 0x04003DD1 RID: 15825
	public const byte PHOTON_EVENT_CODE = 176;

	// Token: 0x04003DD2 RID: 15826
	private static readonly RaiseEventOptions gReceiversAll = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.All
	};

	// Token: 0x04003DD3 RID: 15827
	private static readonly RaiseEventOptions gReceiversOthers = new RaiseEventOptions
	{
		Receivers = ReceiverGroup.Others
	};

	// Token: 0x04003DD4 RID: 15828
	private static readonly SendOptions gSendReliable;

	// Token: 0x04003DD5 RID: 15829
	private static readonly SendOptions gSendUnreliable = SendOptions.SendUnreliable;

	// Token: 0x02000900 RID: 2304
	public enum RaiseMode
	{
		// Token: 0x04003DD7 RID: 15831
		Local,
		// Token: 0x04003DD8 RID: 15832
		RemoteOthers,
		// Token: 0x04003DD9 RID: 15833
		RemoteAll
	}
}
