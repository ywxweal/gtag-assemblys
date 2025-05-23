using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020003FD RID: 1021
public class CosmeticAnchors : MonoBehaviour, ISpawnable
{
	// Token: 0x170002BD RID: 701
	// (get) Token: 0x060018A9 RID: 6313 RVA: 0x00077780 File Offset: 0x00075980
	// (set) Token: 0x060018AA RID: 6314 RVA: 0x00077788 File Offset: 0x00075988
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x060018AB RID: 6315 RVA: 0x00077791 File Offset: 0x00075991
	// (set) Token: 0x060018AC RID: 6316 RVA: 0x00077799 File Offset: 0x00075999
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060018AD RID: 6317 RVA: 0x000777A4 File Offset: 0x000759A4
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.anchorEnabled = false;
		this.vrRig = rig;
		if (!this.vrRig)
		{
			Debug.LogError("CosmeticAnchors.OnSpawn: Disabling! Could not find VRRig in parent! Path: " + base.transform.GetPathQ(), this);
			base.enabled = false;
			return;
		}
		this.anchorOverrides = this.vrRig.gameObject.GetComponent<VRRigAnchorOverrides>();
		this.AssignAnchorToPath(ref this.nameAnchor, this.nameAnchor_path);
		this.AssignAnchorToPath(ref this.leftArmAnchor, this.leftArmAnchor_path);
		this.AssignAnchorToPath(ref this.rightArmAnchor, this.rightArmAnchor_path);
		this.AssignAnchorToPath(ref this.chestAnchor, this.chestAnchor_path);
		this.AssignAnchorToPath(ref this.huntComputerAnchor, this.huntComputerAnchor_path);
		this.AssignAnchorToPath(ref this.badgeAnchor, this.badgeAnchor_path);
		this.AssignAnchorToPath(ref this.builderWatchAnchor, this.builderWatchAnchor_path);
		this.AssignAnchorToPath(ref this.friendshipBraceletLeftOverride, this.friendshipBraceletLeftOverride_path);
		this.AssignAnchorToPath(ref this.friendshipBraceletRightOverride, this.friendshipBraceletRightOverride_path);
	}

	// Token: 0x060018AE RID: 6318 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060018AF RID: 6319 RVA: 0x000778A8 File Offset: 0x00075AA8
	private void AssignAnchorToPath(ref GameObject anchorGObjRef, string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		Transform transform;
		if (!base.transform.TryFindByPath(path, out transform, false))
		{
			this.vrRig = base.GetComponentInParent<VRRig>(true);
			if (this.vrRig && this.vrRig.isOfflineVRRig)
			{
				Debug.LogError("CosmeticAnchors: Could not find path: \"" + path + "\".\nPath to this component: " + base.transform.GetPathQ(), this);
			}
			return;
		}
		anchorGObjRef = transform.gameObject;
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x00077920 File Offset: 0x00075B20
	private void OnEnable()
	{
		if (this.huntComputerAnchor || this.builderWatchAnchor)
		{
			CosmeticAnchorManager.RegisterCosmeticAnchor(this);
		}
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x00077942 File Offset: 0x00075B42
	private void OnDisable()
	{
		if (this.huntComputerAnchor || this.builderWatchAnchor)
		{
			CosmeticAnchorManager.UnregisterCosmeticAnchor(this);
		}
	}

	// Token: 0x060018B2 RID: 6322 RVA: 0x00077964 File Offset: 0x00075B64
	public void TryUpdate()
	{
		if (this.anchorEnabled && this.huntComputerAnchor && !GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf && this.anchorOverrides.HuntComputer.parent != this.anchorOverrides.HuntDefaultAnchor)
		{
			this.anchorOverrides.HuntComputer.parent = this.anchorOverrides.HuntDefaultAnchor;
			return;
		}
		if (this.anchorEnabled && this.huntComputerAnchor && GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf && this.anchorOverrides.HuntComputer.parent == this.anchorOverrides.HuntDefaultAnchor)
		{
			this.SetHuntComputerAnchor(this.anchorEnabled);
		}
		if (this.anchorEnabled && this.builderWatchAnchor && !GorillaTagger.Instance.offlineVRRig.builderResizeWatch.activeSelf && this.anchorOverrides.BuilderWatch.parent != this.anchorOverrides.BuilderWatchAnchor)
		{
			this.anchorOverrides.BuilderWatch.parent = this.anchorOverrides.BuilderWatchAnchor;
			return;
		}
		if (this.anchorEnabled && this.builderWatchAnchor && GorillaTagger.Instance.offlineVRRig.builderResizeWatch.activeSelf && this.anchorOverrides.BuilderWatch.parent == this.anchorOverrides.BuilderWatchAnchor)
		{
			this.SetBuilderWatchAnchor(this.anchorEnabled);
		}
	}

	// Token: 0x060018B3 RID: 6323 RVA: 0x00077AF8 File Offset: 0x00075CF8
	public void EnableAnchor(bool enable)
	{
		this.anchorEnabled = enable;
		if (this.anchorOverrides == null)
		{
			return;
		}
		if (this.leftArmAnchor)
		{
			this.anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnLeftArm, enable ? this.leftArmAnchor.transform : null);
		}
		if (this.rightArmAnchor)
		{
			this.anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnRightArm, enable ? this.rightArmAnchor.transform : null);
		}
		if (this.chestAnchor)
		{
			this.anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnChest, enable ? this.chestAnchor.transform : this.anchorOverrides.chestDefaultTransform);
		}
		this.anchorOverrides.UpdateNameAnchor(enable ? this.nameAnchor : null, this.slot);
		this.anchorOverrides.UpdateBadgeAnchor(enable ? this.badgeAnchor : null, this.slot);
		if (this.huntComputerAnchor)
		{
			this.SetHuntComputerAnchor(enable);
		}
		if (this.builderWatchAnchor)
		{
			this.SetBuilderWatchAnchor(enable);
		}
		this.SetCustomAnchor(this.anchorOverrides.friendshipBraceletLeftAnchor, enable, this.friendshipBraceletLeftOverride, this.anchorOverrides.friendshipBraceletLeftDefaultAnchor);
		this.SetCustomAnchor(this.anchorOverrides.friendshipBraceletRightAnchor, enable, this.friendshipBraceletRightOverride, this.anchorOverrides.friendshipBraceletRightDefaultAnchor);
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x00077C4C File Offset: 0x00075E4C
	private void SetHuntComputerAnchor(bool enable)
	{
		Transform huntComputer = this.anchorOverrides.HuntComputer;
		if (!GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf || !enable)
		{
			huntComputer.parent = this.anchorOverrides.HuntDefaultAnchor;
		}
		else
		{
			huntComputer.parent = this.huntComputerAnchor.transform;
		}
		huntComputer.transform.localPosition = Vector3.zero;
		huntComputer.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x060018B5 RID: 6325 RVA: 0x00077CC4 File Offset: 0x00075EC4
	private void SetBuilderWatchAnchor(bool enable)
	{
		Transform builderWatch = this.anchorOverrides.BuilderWatch;
		if (!GorillaTagger.Instance.offlineVRRig.builderResizeWatch.activeSelf || !enable)
		{
			builderWatch.parent = this.anchorOverrides.BuilderWatchAnchor;
		}
		else
		{
			builderWatch.parent = this.builderWatchAnchor.transform;
		}
		builderWatch.transform.localPosition = Vector3.zero;
		builderWatch.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x060018B6 RID: 6326 RVA: 0x00077D3C File Offset: 0x00075F3C
	private void SetCustomAnchor(Transform target, bool enable, GameObject overrideAnchor, Transform defaultAnchor)
	{
		Transform transform = ((enable && overrideAnchor != null) ? overrideAnchor.transform : defaultAnchor);
		if (target != null && target.parent != transform)
		{
			target.parent = transform;
			target.transform.localPosition = Vector3.zero;
			target.transform.localRotation = Quaternion.identity;
			target.transform.localScale = Vector3.one;
		}
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x00077DB0 File Offset: 0x00075FB0
	public Transform GetPositionAnchor(TransferrableObject.PositionState pos)
	{
		if (pos != TransferrableObject.PositionState.OnLeftArm)
		{
			if (pos != TransferrableObject.PositionState.OnRightArm)
			{
				if (pos != TransferrableObject.PositionState.OnChest)
				{
					return null;
				}
				if (!this.chestAnchor)
				{
					return null;
				}
				return this.chestAnchor.transform;
			}
			else
			{
				if (!this.rightArmAnchor)
				{
					return null;
				}
				return this.rightArmAnchor.transform;
			}
		}
		else
		{
			if (!this.leftArmAnchor)
			{
				return null;
			}
			return this.leftArmAnchor.transform;
		}
	}

	// Token: 0x060018B8 RID: 6328 RVA: 0x00077E1E File Offset: 0x0007601E
	public Transform GetNameAnchor()
	{
		if (!this.nameAnchor)
		{
			return null;
		}
		return this.nameAnchor.transform;
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x00077E3A File Offset: 0x0007603A
	public bool AffectedByHunt()
	{
		return this.huntComputerAnchor != null;
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x00077E48 File Offset: 0x00076048
	public bool AffectedByBuilder()
	{
		return this.builderWatchAnchor != null;
	}

	// Token: 0x04001B68 RID: 7016
	[SerializeField]
	protected GameObject nameAnchor;

	// Token: 0x04001B69 RID: 7017
	[Delayed]
	[SerializeField]
	protected string nameAnchor_path;

	// Token: 0x04001B6A RID: 7018
	[SerializeField]
	protected GameObject leftArmAnchor;

	// Token: 0x04001B6B RID: 7019
	[Delayed]
	[SerializeField]
	protected string leftArmAnchor_path;

	// Token: 0x04001B6C RID: 7020
	[SerializeField]
	protected GameObject rightArmAnchor;

	// Token: 0x04001B6D RID: 7021
	[Delayed]
	[SerializeField]
	protected string rightArmAnchor_path;

	// Token: 0x04001B6E RID: 7022
	[SerializeField]
	protected GameObject chestAnchor;

	// Token: 0x04001B6F RID: 7023
	[Delayed]
	[SerializeField]
	protected string chestAnchor_path;

	// Token: 0x04001B70 RID: 7024
	[SerializeField]
	protected GameObject huntComputerAnchor;

	// Token: 0x04001B71 RID: 7025
	[Delayed]
	[SerializeField]
	protected string huntComputerAnchor_path;

	// Token: 0x04001B72 RID: 7026
	[SerializeField]
	protected GameObject builderWatchAnchor;

	// Token: 0x04001B73 RID: 7027
	[Delayed]
	[SerializeField]
	protected string builderWatchAnchor_path;

	// Token: 0x04001B74 RID: 7028
	[SerializeField]
	protected GameObject friendshipBraceletLeftOverride;

	// Token: 0x04001B75 RID: 7029
	[Delayed]
	[SerializeField]
	protected string friendshipBraceletLeftOverride_path;

	// Token: 0x04001B76 RID: 7030
	[SerializeField]
	protected GameObject friendshipBraceletRightOverride;

	// Token: 0x04001B77 RID: 7031
	[Delayed]
	[SerializeField]
	protected string friendshipBraceletRightOverride_path;

	// Token: 0x04001B78 RID: 7032
	[SerializeField]
	protected GameObject badgeAnchor;

	// Token: 0x04001B79 RID: 7033
	[Delayed]
	[SerializeField]
	protected string badgeAnchor_path;

	// Token: 0x04001B7A RID: 7034
	[SerializeField]
	public CosmeticsController.CosmeticSlots slot;

	// Token: 0x04001B7B RID: 7035
	private VRRig vrRig;

	// Token: 0x04001B7C RID: 7036
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x04001B7D RID: 7037
	private bool anchorEnabled;

	// Token: 0x04001B7E RID: 7038
	private static GTLogErrorLimiter k_debugLogError_anchorOverridesNull = new GTLogErrorLimiter("The array `anchorOverrides` was null. Is the cosmetic getting initialized properly? ", 10, "\n- ");
}
