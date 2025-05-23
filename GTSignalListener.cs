using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000655 RID: 1621
public class GTSignalListener : MonoBehaviour
{
	// Token: 0x170003DB RID: 987
	// (get) Token: 0x0600287A RID: 10362 RVA: 0x000C987B File Offset: 0x000C7A7B
	// (set) Token: 0x0600287B RID: 10363 RVA: 0x000C9883 File Offset: 0x000C7A83
	public int rigActorID { get; private set; } = -1;

	// Token: 0x0600287C RID: 10364 RVA: 0x000C988C File Offset: 0x000C7A8C
	private void Awake()
	{
		this.OnListenerAwake();
	}

	// Token: 0x0600287D RID: 10365 RVA: 0x000C9894 File Offset: 0x000C7A94
	private void OnEnable()
	{
		this.RefreshActorID();
		this.OnListenerEnable();
		GTSignalRelay.Register(this);
	}

	// Token: 0x0600287E RID: 10366 RVA: 0x000C98A8 File Offset: 0x000C7AA8
	private void OnDisable()
	{
		GTSignalRelay.Unregister(this);
		this.OnListenerDisable();
	}

	// Token: 0x0600287F RID: 10367 RVA: 0x000C98B6 File Offset: 0x000C7AB6
	private void RefreshActorID()
	{
		this.rig = base.GetComponentInParent<VRRig>(true);
		int num;
		if (!(this.rig == null))
		{
			NetPlayer owningNetPlayer = this.rig.OwningNetPlayer;
			num = ((owningNetPlayer != null) ? owningNetPlayer.ActorNumber : (-1));
		}
		else
		{
			num = -1;
		}
		this.rigActorID = num;
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x000C98F3 File Offset: 0x000C7AF3
	public virtual bool IsReady()
	{
		return this._callLimits.CheckCallTime(Time.time);
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnListenerAwake()
	{
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnListenerEnable()
	{
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnListenerDisable()
	{
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void HandleSignalReceived(int sender, object[] args)
	{
	}

	// Token: 0x04002D4E RID: 11598
	[Space]
	public GTSignalID signal;

	// Token: 0x04002D4F RID: 11599
	[Space]
	public VRRig rig;

	// Token: 0x04002D51 RID: 11601
	[Space]
	public bool deafen;

	// Token: 0x04002D52 RID: 11602
	[FormerlySerializedAs("listenToRigOnly")]
	public bool listenToSelfOnly;

	// Token: 0x04002D53 RID: 11603
	public bool ignoreSelf;

	// Token: 0x04002D54 RID: 11604
	[Space]
	public bool callUnityEvent = true;

	// Token: 0x04002D55 RID: 11605
	[Space]
	[SerializeField]
	private CallLimiter _callLimits = new CallLimiter(10, 0.25f, 0.5f);

	// Token: 0x04002D56 RID: 11606
	[Space]
	public UnityEvent onSignalReceived;
}
