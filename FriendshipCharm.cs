using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020003EF RID: 1007
public class FriendshipCharm : HoldableObject
{
	// Token: 0x0600182B RID: 6187 RVA: 0x00075807 File Offset: 0x00073A07
	private void Awake()
	{
		this.parent = base.transform.parent;
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x0007581C File Offset: 0x00073A1C
	private void LateUpdate()
	{
		if (!this.isBroken && (this.lineStart.transform.position - this.lineEnd.transform.position).IsLongerThan(this.breakBraceletLength * GTPlayer.Instance.scale))
		{
			this.DestroyBracelet();
		}
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x00075874 File Offset: 0x00073A74
	public void OnEnable()
	{
		this.interactionPoint.enabled = true;
		this.meshRenderer.enabled = true;
		this.isBroken = false;
		this.UpdatePosition();
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x0007589B File Offset: 0x00073A9B
	private void DestroyBracelet()
	{
		this.interactionPoint.enabled = false;
		this.isBroken = true;
		Debug.Log("LeaveGroup: bracelet destroyed");
		FriendshipGroupDetection.Instance.LeaveParty();
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x000758C4 File Offset: 0x00073AC4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 2f);
		base.transform.SetParent(flag ? this.leftHandHoldAnchor : this.rightHandHoldAnchor);
		base.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x0007594C File Offset: 0x00073B4C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(null, flag);
		this.UpdatePosition();
		return base.OnRelease(zoneReleased, releasingHand);
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x00075988 File Offset: 0x00073B88
	private void UpdatePosition()
	{
		base.transform.SetParent(this.parent);
		base.transform.localPosition = this.releasePosition.localPosition;
		base.transform.localRotation = this.releasePosition.localRotation;
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x000759C8 File Offset: 0x00073BC8
	private void OnCollisionEnter(Collision other)
	{
		if (!this.isBroken)
		{
			return;
		}
		if (this.breakItemLayerMask != (this.breakItemLayerMask | (1 << other.gameObject.layer)))
		{
			return;
		}
		this.meshRenderer.enabled = false;
		this.UpdatePosition();
	}

	// Token: 0x06001833 RID: 6195 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void DropItemCleanup()
	{
	}

	// Token: 0x04001B10 RID: 6928
	[SerializeField]
	private InteractionPoint interactionPoint;

	// Token: 0x04001B11 RID: 6929
	[SerializeField]
	private Transform rightHandHoldAnchor;

	// Token: 0x04001B12 RID: 6930
	[SerializeField]
	private Transform leftHandHoldAnchor;

	// Token: 0x04001B13 RID: 6931
	[SerializeField]
	private MeshRenderer meshRenderer;

	// Token: 0x04001B14 RID: 6932
	[SerializeField]
	private Transform lineStart;

	// Token: 0x04001B15 RID: 6933
	[SerializeField]
	private Transform lineEnd;

	// Token: 0x04001B16 RID: 6934
	[SerializeField]
	private Transform releasePosition;

	// Token: 0x04001B17 RID: 6935
	[SerializeField]
	private float breakBraceletLength;

	// Token: 0x04001B18 RID: 6936
	[SerializeField]
	private LayerMask breakItemLayerMask;

	// Token: 0x04001B19 RID: 6937
	private Transform parent;

	// Token: 0x04001B1A RID: 6938
	private bool isBroken;
}
