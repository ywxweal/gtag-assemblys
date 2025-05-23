using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000473 RID: 1139
[RequireComponent(typeof(UseableObjectEvents))]
public class UseableObject : TransferrableObject
{
	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06001C02 RID: 7170 RVA: 0x00089C1B File Offset: 0x00087E1B
	public bool isMidUse
	{
		get
		{
			return this._isMidUse;
		}
	}

	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06001C03 RID: 7171 RVA: 0x00089C23 File Offset: 0x00087E23
	public float useTimeElapsed
	{
		get
		{
			return this._useTimeElapsed;
		}
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06001C04 RID: 7172 RVA: 0x00089C2B File Offset: 0x00087E2B
	public bool justUsed
	{
		get
		{
			if (!this._justUsed)
			{
				return false;
			}
			this._justUsed = false;
			return true;
		}
	}

	// Token: 0x06001C05 RID: 7173 RVA: 0x00089C3F File Offset: 0x00087E3F
	protected override void Awake()
	{
		base.Awake();
		this._events = base.gameObject.GetOrAddComponent<UseableObjectEvents>();
	}

	// Token: 0x06001C06 RID: 7174 RVA: 0x00089C58 File Offset: 0x00087E58
	internal override void OnEnable()
	{
		base.OnEnable();
		UseableObjectEvents events = this._events;
		VRRig myOnlineRig = base.myOnlineRig;
		NetPlayer netPlayer;
		if ((netPlayer = ((myOnlineRig != null) ? myOnlineRig.creator : null)) == null)
		{
			VRRig myRig = base.myRig;
			netPlayer = ((myRig != null) ? myRig.creator : null);
		}
		events.Init(netPlayer);
		this._events.Activate += this.OnObjectActivated;
		this._events.Deactivate += this.OnObjectDeactivated;
	}

	// Token: 0x06001C07 RID: 7175 RVA: 0x00089CE2 File Offset: 0x00087EE2
	internal override void OnDisable()
	{
		base.OnDisable();
		Object.Destroy(this._events);
	}

	// Token: 0x06001C08 RID: 7176 RVA: 0x00089CF5 File Offset: 0x00087EF5
	private void OnObjectActivated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x06001C09 RID: 7177 RVA: 0x00089CF5 File Offset: 0x00087EF5
	private void OnObjectDeactivated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x06001C0A RID: 7178 RVA: 0x00089CFB File Offset: 0x00087EFB
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (this._isMidUse)
		{
			this._useTimeElapsed += Time.deltaTime;
		}
	}

	// Token: 0x06001C0B RID: 7179 RVA: 0x00089D20 File Offset: 0x00087F20
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.IsMyItem())
		{
			UnityEvent unityEvent = this.onActivateLocal;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._useTimeElapsed = 0f;
			this._isMidUse = true;
		}
		if (this._raiseActivate)
		{
			UseableObjectEvents events = this._events;
			if (events == null)
			{
				return;
			}
			PhotonEvent activate = events.Activate;
			if (activate == null)
			{
				return;
			}
			activate.RaiseAll(Array.Empty<object>());
		}
	}

	// Token: 0x06001C0C RID: 7180 RVA: 0x00089D88 File Offset: 0x00087F88
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.IsMyItem())
		{
			UnityEvent unityEvent = this.onDeactivateLocal;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._isMidUse = false;
			this._justUsed = true;
		}
		if (this._raiseDeactivate)
		{
			UseableObjectEvents events = this._events;
			if (events == null)
			{
				return;
			}
			PhotonEvent deactivate = events.Deactivate;
			if (deactivate == null)
			{
				return;
			}
			deactivate.RaiseAll(Array.Empty<object>());
		}
	}

	// Token: 0x06001C0D RID: 7181 RVA: 0x00089DE9 File Offset: 0x00087FE9
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06001C0E RID: 7182 RVA: 0x00089DF4 File Offset: 0x00087FF4
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04001F23 RID: 7971
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04001F24 RID: 7972
	[DebugOption]
	public bool disableDeactivation;

	// Token: 0x04001F25 RID: 7973
	[SerializeField]
	private UseableObjectEvents _events;

	// Token: 0x04001F26 RID: 7974
	[SerializeField]
	private bool _raiseActivate = true;

	// Token: 0x04001F27 RID: 7975
	[SerializeField]
	private bool _raiseDeactivate = true;

	// Token: 0x04001F28 RID: 7976
	[NonSerialized]
	private DateTime _lastActivate;

	// Token: 0x04001F29 RID: 7977
	[NonSerialized]
	private DateTime _lastDeactivate;

	// Token: 0x04001F2A RID: 7978
	[NonSerialized]
	private bool _isMidUse;

	// Token: 0x04001F2B RID: 7979
	[NonSerialized]
	private float _useTimeElapsed;

	// Token: 0x04001F2C RID: 7980
	[NonSerialized]
	private bool _justUsed;

	// Token: 0x04001F2D RID: 7981
	[NonSerialized]
	private int tempHandPos;

	// Token: 0x04001F2E RID: 7982
	public UnityEvent onActivateLocal;

	// Token: 0x04001F2F RID: 7983
	public UnityEvent onDeactivateLocal;
}
