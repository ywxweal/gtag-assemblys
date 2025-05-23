using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003BC RID: 956
public class TransferrableObjectHoldablePart : HoldableObject
{
	// Token: 0x0600163D RID: 5693 RVA: 0x0006BD48 File Offset: 0x00069F48
	private void Update()
	{
		VRRig vrrig;
		if (!this.transferrableParentObject.IsLocalObject())
		{
			vrrig = this.transferrableParentObject.myOnlineRig;
			this.isHeld = (this.transferrableParentObject.itemState & this.heldBit) > (TransferrableObject.ItemStates)0;
			TransferrableObject.PositionState currentState = this.transferrableParentObject.currentState;
			if (currentState == TransferrableObject.PositionState.OnRightArm || currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.isHeldLeftHand = this.isHeld;
			}
			else
			{
				this.isHeldLeftHand = false;
			}
		}
		else
		{
			vrrig = VRRig.LocalRig;
		}
		if (this.isHeld)
		{
			if (this.transferrableParentObject.InHand())
			{
				this.UpdateHeld(vrrig, this.isHeldLeftHand);
				return;
			}
			if (this.transferrableParentObject.IsLocalObject())
			{
				this.OnRelease(null, this.isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			}
		}
	}

	// Token: 0x0600163E RID: 5694 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
	}

	// Token: 0x0600163F RID: 5695 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x0006BE14 File Offset: 0x0006A014
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.isHeld = true;
		this.isHeldLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
		this.transferrableParentObject.itemState |= this.heldBit;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		UnityEvent unityEvent = this.onGrab;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x0006BE7B File Offset: 0x0006A07B
	public override void DropItemCleanup()
	{
		this.isHeld = false;
		this.isHeldLeftHand = false;
		this.transferrableParentObject.itemState &= ~this.heldBit;
	}

	// Token: 0x06001642 RID: 5698 RVA: 0x0006BEA4 File Offset: 0x0006A0A4
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
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		this.isHeld = false;
		this.isHeldLeftHand = false;
		this.transferrableParentObject.itemState &= ~this.heldBit;
		UnityEvent unityEvent = this.onRelease;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		return true;
	}

	// Token: 0x040018B9 RID: 6329
	[SerializeField]
	protected TransferrableObject transferrableParentObject;

	// Token: 0x040018BA RID: 6330
	[SerializeField]
	private TransferrableObject.ItemStates heldBit = TransferrableObject.ItemStates.Part0Held;

	// Token: 0x040018BB RID: 6331
	private bool isHeld;

	// Token: 0x040018BC RID: 6332
	protected bool isHeldLeftHand;

	// Token: 0x040018BD RID: 6333
	public UnityEvent onGrab;

	// Token: 0x040018BE RID: 6334
	public UnityEvent onRelease;

	// Token: 0x040018BF RID: 6335
	public UnityEvent onDrop;
}
