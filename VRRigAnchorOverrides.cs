using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000431 RID: 1073
public class VRRigAnchorOverrides : MonoBehaviour
{
	// Token: 0x170002DA RID: 730
	// (get) Token: 0x06001A7A RID: 6778 RVA: 0x00081FC1 File Offset: 0x000801C1
	// (set) Token: 0x06001A7B RID: 6779 RVA: 0x00081FCC File Offset: 0x000801CC
	public Transform CurrentBadgeTransform
	{
		get
		{
			return this.currentBadgeTransform;
		}
		set
		{
			if (value != this.currentBadgeTransform)
			{
				this.ResetBadge();
				this.currentBadgeTransform = value;
				this.badgeDefaultRot = this.currentBadgeTransform.localRotation;
				this.badgeDefaultPos = this.currentBadgeTransform.localPosition;
				this.UpdateBadge();
			}
		}
	}

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x06001A7C RID: 6780 RVA: 0x0008201C File Offset: 0x0008021C
	public Transform HuntDefaultAnchor
	{
		get
		{
			return this.huntComputerDefaultAnchor;
		}
	}

	// Token: 0x170002DC RID: 732
	// (get) Token: 0x06001A7D RID: 6781 RVA: 0x00082024 File Offset: 0x00080224
	public Transform HuntComputer
	{
		get
		{
			return this.huntComputer;
		}
	}

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x06001A7E RID: 6782 RVA: 0x0008202C File Offset: 0x0008022C
	public Transform BuilderWatchAnchor
	{
		get
		{
			return this.builderResizeButtonDefaultAnchor;
		}
	}

	// Token: 0x170002DE RID: 734
	// (get) Token: 0x06001A7F RID: 6783 RVA: 0x00082034 File Offset: 0x00080234
	public Transform BuilderWatch
	{
		get
		{
			return this.builderResizeButton;
		}
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x0008203C File Offset: 0x0008023C
	private void Awake()
	{
		for (int i = 0; i < 8; i++)
		{
			this.overrideAnchors[i] = null;
		}
		int num = this.MapPositionToIndex(TransferrableObject.PositionState.OnChest);
		this.overrideAnchors[num] = this.chestDefaultTransform;
		this.huntDefaultTransform = this.huntComputer;
		this.builderResizeButtonDefaultTransform = this.builderResizeButton;
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x00082090 File Offset: 0x00080290
	private void OnEnable()
	{
		if (this.nameDefaultAnchor && this.nameDefaultAnchor.parent)
		{
			this.nameTransform.parent = this.nameDefaultAnchor.parent;
		}
		else
		{
			Debug.LogError("VRRigAnchorOverrides: could not set parent `nameTransform` because `nameDefaultAnchor` or its parent was null!" + base.transform.GetPathQ(), this);
		}
		this.huntComputer = this.huntDefaultTransform;
		if (this.huntComputerDefaultAnchor && this.huntComputerDefaultAnchor.parent)
		{
			this.huntComputer.parent = this.huntComputerDefaultAnchor.parent;
		}
		else
		{
			Debug.LogError("VRRigAnchorOverrides: could not set parent `huntComputer` because `huntComputerDefaultAnchor` or its parent was null!" + base.transform.GetPathQ(), this);
		}
		this.builderResizeButton = this.builderResizeButtonDefaultTransform;
		if (this.builderResizeButtonDefaultAnchor && this.builderResizeButtonDefaultAnchor.parent)
		{
			this.builderResizeButton.parent = this.builderResizeButtonDefaultAnchor.parent;
			return;
		}
		Debug.LogError("VRRigAnchorOverrides: could not set parent `builderResizeButton` because `builderResizeButtonDefaultAnchor` or its parent was null! Path: " + base.transform.GetPathQ(), this);
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x000821AC File Offset: 0x000803AC
	private int MapPositionToIndex(TransferrableObject.PositionState pos)
	{
		int num = (int)pos;
		int num2 = 0;
		while ((num >>= 1) != 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x000821CC File Offset: 0x000803CC
	public void OverrideAnchor(TransferrableObject.PositionState pos, Transform anchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (this.overrideAnchors[num])
		{
			foreach (object obj in this.overrideAnchors[num])
			{
				((Transform)obj).parent = null;
			}
		}
		this.overrideAnchors[num] = anchor;
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x00082248 File Offset: 0x00080448
	public Transform AnchorOverride(TransferrableObject.PositionState pos, Transform fallback)
	{
		int num = this.MapPositionToIndex(pos);
		Transform transform = this.overrideAnchors[num];
		if (transform != null)
		{
			return transform;
		}
		return fallback;
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x0008226C File Offset: 0x0008046C
	public void UpdateNameAnchor(GameObject nameAnchor, CosmeticsController.CosmeticSlots slot)
	{
		if (slot != CosmeticsController.CosmeticSlots.Badge)
		{
			switch (slot)
			{
			case CosmeticsController.CosmeticSlots.Shirt:
				this.nameAnchors[0] = nameAnchor;
				break;
			case CosmeticsController.CosmeticSlots.Pants:
				this.nameAnchors[1] = nameAnchor;
				break;
			case CosmeticsController.CosmeticSlots.Back:
				this.nameAnchors[2] = nameAnchor;
				break;
			}
		}
		else
		{
			this.nameAnchors[3] = nameAnchor;
		}
		this.UpdateName();
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x000822C4 File Offset: 0x000804C4
	private void UpdateName()
	{
		foreach (GameObject gameObject in this.nameAnchors)
		{
			if (gameObject)
			{
				this.nameTransform.parent = gameObject.transform;
				this.nameTransform.localRotation = Quaternion.identity;
				this.nameTransform.localPosition = Vector3.zero;
				return;
			}
		}
		if (this.nameDefaultAnchor)
		{
			this.nameTransform.parent = this.nameDefaultAnchor;
			this.nameTransform.localRotation = Quaternion.identity;
			this.nameTransform.localPosition = Vector3.zero;
			return;
		}
		Debug.LogError("VRRigAnchorOverrides: could not set parent for `nameTransform` because `nameDefaultAnchor` or its parent was null! Path: " + base.transform.GetPathQ(), this);
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x0008237E File Offset: 0x0008057E
	public void UpdateBadgeAnchor(GameObject badgeAnchor, CosmeticsController.CosmeticSlots slot)
	{
		switch (slot)
		{
		case CosmeticsController.CosmeticSlots.Shirt:
			this.badgeAnchors[0] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Pants:
			this.badgeAnchors[1] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Back:
			this.badgeAnchors[2] = badgeAnchor;
			break;
		}
		this.UpdateBadge();
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x000823BC File Offset: 0x000805BC
	private void UpdateBadge()
	{
		if (!this.currentBadgeTransform)
		{
			return;
		}
		foreach (GameObject gameObject in this.badgeAnchors)
		{
			if (gameObject)
			{
				this.currentBadgeTransform.localRotation = gameObject.transform.localRotation;
				this.currentBadgeTransform.localPosition = gameObject.transform.localPosition;
				return;
			}
		}
		this.ResetBadge();
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x0008242B File Offset: 0x0008062B
	private void ResetBadge()
	{
		if (!this.currentBadgeTransform)
		{
			return;
		}
		this.currentBadgeTransform.localRotation = this.badgeDefaultRot;
		this.currentBadgeTransform.localPosition = this.badgeDefaultPos;
	}

	// Token: 0x04001D91 RID: 7569
	[SerializeField]
	internal Transform nameDefaultAnchor;

	// Token: 0x04001D92 RID: 7570
	[SerializeField]
	internal Transform nameTransform;

	// Token: 0x04001D93 RID: 7571
	[SerializeField]
	internal Transform chestDefaultTransform;

	// Token: 0x04001D94 RID: 7572
	[SerializeField]
	internal Transform huntComputer;

	// Token: 0x04001D95 RID: 7573
	[SerializeField]
	internal Transform huntComputerDefaultAnchor;

	// Token: 0x04001D96 RID: 7574
	private Transform huntDefaultTransform;

	// Token: 0x04001D97 RID: 7575
	[SerializeField]
	protected Transform builderResizeButton;

	// Token: 0x04001D98 RID: 7576
	[SerializeField]
	protected Transform builderResizeButtonDefaultAnchor;

	// Token: 0x04001D99 RID: 7577
	private Transform builderResizeButtonDefaultTransform;

	// Token: 0x04001D9A RID: 7578
	private readonly Transform[] overrideAnchors = new Transform[8];

	// Token: 0x04001D9B RID: 7579
	private GameObject nameLastObjectToAttach;

	// Token: 0x04001D9C RID: 7580
	private Transform currentBadgeTransform;

	// Token: 0x04001D9D RID: 7581
	private Vector3 badgeDefaultPos;

	// Token: 0x04001D9E RID: 7582
	private Quaternion badgeDefaultRot;

	// Token: 0x04001D9F RID: 7583
	private GameObject[] badgeAnchors = new GameObject[3];

	// Token: 0x04001DA0 RID: 7584
	private GameObject[] nameAnchors = new GameObject[4];

	// Token: 0x04001DA1 RID: 7585
	[SerializeField]
	public Transform friendshipBraceletLeftDefaultAnchor;

	// Token: 0x04001DA2 RID: 7586
	public Transform friendshipBraceletLeftAnchor;

	// Token: 0x04001DA3 RID: 7587
	[SerializeField]
	public Transform friendshipBraceletRightDefaultAnchor;

	// Token: 0x04001DA4 RID: 7588
	public Transform friendshipBraceletRightAnchor;
}
