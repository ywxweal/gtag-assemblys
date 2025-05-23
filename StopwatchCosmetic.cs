using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200046A RID: 1130
public class StopwatchCosmetic : TransferrableObject
{
	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06001BBD RID: 7101 RVA: 0x000882A9 File Offset: 0x000864A9
	public bool isActivating
	{
		get
		{
			return this._isActivating;
		}
	}

	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06001BBE RID: 7102 RVA: 0x000882B1 File Offset: 0x000864B1
	public float activeTimeElapsed
	{
		get
		{
			return this._activeTimeElapsed;
		}
	}

	// Token: 0x06001BBF RID: 7103 RVA: 0x000882BC File Offset: 0x000864BC
	protected override void Awake()
	{
		base.Awake();
		if (StopwatchCosmetic.gWatchToggleRPC == null)
		{
			StopwatchCosmetic.gWatchToggleRPC = new PhotonEvent(StaticHash.Compute("StopwatchCosmetic", "WatchToggle"));
		}
		if (StopwatchCosmetic.gWatchResetRPC == null)
		{
			StopwatchCosmetic.gWatchResetRPC = new PhotonEvent(StaticHash.Compute("StopwatchCosmetic", "WatchReset"));
		}
		this._watchToggle = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchToggle);
		this._watchReset = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchReset);
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x00088340 File Offset: 0x00086540
	internal override void OnEnable()
	{
		base.OnEnable();
		int num;
		if (!this.FetchMyViewID(out num))
		{
			this._photonID = -1;
			return;
		}
		StopwatchCosmetic.gWatchResetRPC += this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC += this._watchToggle;
		this._photonID = num.GetStaticHash();
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x0008839B File Offset: 0x0008659B
	internal override void OnDisable()
	{
		base.OnDisable();
		StopwatchCosmetic.gWatchResetRPC -= this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC -= this._watchToggle;
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x000883D0 File Offset: 0x000865D0
	private void OnWatchToggle(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnWatchToggle");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		bool flag = (bool)args[1];
		int num = (int)args[2];
		this._watchFace.SetMillisElapsed(num, true);
		this._watchFace.WatchToggle();
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x00088450 File Offset: 0x00086650
	private void OnWatchReset(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnWatchReset");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		this._watchFace.WatchReset();
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x000884B0 File Offset: 0x000866B0
	private bool FetchMyViewID(out int viewID)
	{
		viewID = -1;
		NetPlayer netPlayer = ((base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null));
		if (netPlayer == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
		{
			return false;
		}
		if (rigContainer.Rig.netView == null)
		{
			return false;
		}
		viewID = rigContainer.Rig.netView.ViewID;
		return true;
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x0008854F File Offset: 0x0008674F
	public bool PollActivated()
	{
		if (!this._activated)
		{
			return false;
		}
		this._activated = false;
		return true;
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x00088564 File Offset: 0x00086764
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (this._isActivating)
		{
			this._activeTimeElapsed += Time.deltaTime;
		}
		if (this._isActivating && this._activeTimeElapsed > 1f)
		{
			this._isActivating = false;
			this._watchFace.WatchReset(true);
			StopwatchCosmetic.gWatchResetRPC.RaiseOthers(new object[] { this._photonID });
		}
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x000885D7 File Offset: 0x000867D7
	public override void OnActivate()
	{
		if (!this.CanActivate())
		{
			return;
		}
		base.OnActivate();
		if (this.IsMyItem())
		{
			this._activeTimeElapsed = 0f;
			this._isActivating = true;
		}
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x00088604 File Offset: 0x00086804
	public override void OnDeactivate()
	{
		if (!this.CanDeactivate())
		{
			return;
		}
		base.OnDeactivate();
		if (!this.IsMyItem())
		{
			return;
		}
		this._isActivating = false;
		this._activated = true;
		this._watchFace.WatchToggle();
		StopwatchCosmetic.gWatchToggleRPC.RaiseOthers(new object[]
		{
			this._photonID,
			this._watchFace.watchActive,
			this._watchFace.millisElapsed
		});
		this._activated = false;
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x0008868D File Offset: 0x0008688D
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x00088698 File Offset: 0x00086898
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04001ECB RID: 7883
	[SerializeField]
	private StopwatchFace _watchFace;

	// Token: 0x04001ECC RID: 7884
	[Space]
	[NonSerialized]
	private bool _isActivating;

	// Token: 0x04001ECD RID: 7885
	[NonSerialized]
	private float _activeTimeElapsed;

	// Token: 0x04001ECE RID: 7886
	[NonSerialized]
	private bool _activated;

	// Token: 0x04001ECF RID: 7887
	[Space]
	[NonSerialized]
	private int _photonID = -1;

	// Token: 0x04001ED0 RID: 7888
	private static PhotonEvent gWatchToggleRPC;

	// Token: 0x04001ED1 RID: 7889
	private static PhotonEvent gWatchResetRPC;

	// Token: 0x04001ED2 RID: 7890
	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchToggle;

	// Token: 0x04001ED3 RID: 7891
	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchReset;

	// Token: 0x04001ED4 RID: 7892
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04001ED5 RID: 7893
	[DebugOption]
	public bool disableDeactivation;
}
