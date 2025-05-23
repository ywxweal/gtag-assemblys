using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000655 RID: 1621
public class GTSignalListener : MonoBehaviour
{
	// Token: 0x170003DB RID: 987
	// (get) Token: 0x06002879 RID: 10361 RVA: 0x000C97D7 File Offset: 0x000C79D7
	// (set) Token: 0x0600287A RID: 10362 RVA: 0x000C97DF File Offset: 0x000C79DF
	public int rigActorID { get; private set; } = -1;

	// Token: 0x0600287B RID: 10363 RVA: 0x000C97E8 File Offset: 0x000C79E8
	private void Awake()
	{
		this.OnListenerAwake();
	}

	// Token: 0x0600287C RID: 10364 RVA: 0x000C97F0 File Offset: 0x000C79F0
	private void OnEnable()
	{
		this.RefreshActorID();
		this.OnListenerEnable();
		GTSignalRelay.Register(this);
	}

	// Token: 0x0600287D RID: 10365 RVA: 0x000C9804 File Offset: 0x000C7A04
	private void OnDisable()
	{
		GTSignalRelay.Unregister(this);
		this.OnListenerDisable();
	}

	// Token: 0x0600287E RID: 10366 RVA: 0x000C9812 File Offset: 0x000C7A12
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

	// Token: 0x0600287F RID: 10367 RVA: 0x000C984F File Offset: 0x000C7A4F
	public virtual bool IsReady()
	{
		return this._callLimits.CheckCallTime(Time.time);
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnListenerAwake()
	{
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnListenerEnable()
	{
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnListenerDisable()
	{
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void HandleSignalReceived(int sender, object[] args)
	{
	}

	// Token: 0x04002D4C RID: 11596
	[Space]
	public GTSignalID signal;

	// Token: 0x04002D4D RID: 11597
	[Space]
	public VRRig rig;

	// Token: 0x04002D4F RID: 11599
	[Space]
	public bool deafen;

	// Token: 0x04002D50 RID: 11600
	[FormerlySerializedAs("listenToRigOnly")]
	public bool listenToSelfOnly;

	// Token: 0x04002D51 RID: 11601
	public bool ignoreSelf;

	// Token: 0x04002D52 RID: 11602
	[Space]
	public bool callUnityEvent = true;

	// Token: 0x04002D53 RID: 11603
	[Space]
	[SerializeField]
	private CallLimiter _callLimits = new CallLimiter(10, 0.25f, 0.5f);

	// Token: 0x04002D54 RID: 11604
	[Space]
	public UnityEvent onSignalReceived;
}
