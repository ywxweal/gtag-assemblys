using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200040C RID: 1036
public class ManipulatableObject : HoldableObject
{
	// Token: 0x06001932 RID: 6450 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x00002076 File Offset: 0x00000276
	protected virtual bool ShouldHandDetach(GameObject hand)
	{
		return false;
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnHeldUpdate(GameObject hand)
	{
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnReleasedUpdate()
	{
	}

	// Token: 0x06001937 RID: 6455 RVA: 0x0007A140 File Offset: 0x00078340
	public virtual void LateUpdate()
	{
		if (this.isHeld)
		{
			if (this.holdingHand == null)
			{
				EquipmentInteractor.instance.ForceDropManipulatableObject(this);
				return;
			}
			this.OnHeldUpdate(this.holdingHand);
			if (this.ShouldHandDetach(this.holdingHand))
			{
				EquipmentInteractor.instance.ForceDropManipulatableObject(this);
				return;
			}
		}
		else
		{
			this.OnReleasedUpdate();
		}
	}

	// Token: 0x06001938 RID: 6456 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x0007A1A0 File Offset: 0x000783A0
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		this.isHeld = true;
		this.holdingHand = grabbingHand;
		this.OnStartManipulation(this.holdingHand);
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x0007A1E8 File Offset: 0x000783E8
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		Vector3 vector = Vector3.zero;
		if (flag)
		{
			vector = GTPlayer.Instance.leftHandCenterVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			EquipmentInteractor.instance.leftHandHeldEquipment = null;
		}
		else
		{
			vector = GTPlayer.Instance.rightHandCenterVelocityTracker.GetAverageVelocity(true, 0.15f, false);
			EquipmentInteractor.instance.rightHandHeldEquipment = null;
		}
		this.isHeld = false;
		this.holdingHand = null;
		this.OnStopManipulation(releasingHand, vector);
		return true;
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void DropItemCleanup()
	{
	}

	// Token: 0x04001C04 RID: 7172
	protected bool isHeld;

	// Token: 0x04001C05 RID: 7173
	protected GameObject holdingHand;
}
