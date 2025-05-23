using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200066C RID: 1644
public class HoverboardHandle : HoldableObject
{
	// Token: 0x06002913 RID: 10515 RVA: 0x000CC954 File Offset: 0x000CAB54
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		if (Time.frameCount > this.noHapticsUntilFrame)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.noHapticsUntilFrame = Time.frameCount + 1;
	}

	// Token: 0x06002914 RID: 10516 RVA: 0x000CC9C4 File Offset: 0x000CABC4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		Transform transform = (flag ? VRRig.LocalRig.leftHand.rigTarget : VRRig.LocalRig.rightHand.rigTarget);
		Quaternion quaternion;
		Vector3 vector;
		if (!this.parentVisual.IsHeld)
		{
			quaternion = (flag ? this.defaultHoldAngleLeft : this.defaultHoldAngleRight);
			vector = (flag ? this.defaultHoldPosLeft : this.defaultHoldPosRight);
		}
		else
		{
			quaternion = transform.InverseTransformRotation(this.parentVisual.transform.rotation);
			vector = transform.InverseTransformPoint(this.parentVisual.transform.position);
		}
		this.parentVisual.SetIsHeld(flag, vector, quaternion, this.parentVisual.boardColor);
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x000CCA9D File Offset: 0x000CAC9D
	public override void DropItemCleanup()
	{
		if (this.parentVisual.gameObject.activeSelf)
		{
			this.parentVisual.DropFreeBoard();
		}
		this.parentVisual.SetNotHeld();
	}

	// Token: 0x06002916 RID: 10518 RVA: 0x000CCAC8 File Offset: 0x000CACC8
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
		{
			return false;
		}
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
		{
			return false;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.parentVisual.IsLeftHanded);
		this.parentVisual.SetNotHeld();
		return true;
	}

	// Token: 0x04002E22 RID: 11810
	[SerializeField]
	private HoverboardVisual parentVisual;

	// Token: 0x04002E23 RID: 11811
	[SerializeField]
	private Quaternion defaultHoldAngleLeft;

	// Token: 0x04002E24 RID: 11812
	[SerializeField]
	private Quaternion defaultHoldAngleRight;

	// Token: 0x04002E25 RID: 11813
	[SerializeField]
	private Vector3 defaultHoldPosLeft;

	// Token: 0x04002E26 RID: 11814
	[SerializeField]
	private Vector3 defaultHoldPosRight;

	// Token: 0x04002E27 RID: 11815
	private int noHapticsUntilFrame = -1;
}
