using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000665 RID: 1637
public class FreeHoverboardHandle : HoldableObject
{
	// Token: 0x060028E9 RID: 10473 RVA: 0x000CBDFC File Offset: 0x000C9FFC
	private void Awake()
	{
		this.hasParentBoard = this.parentFreeBoard != null;
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x000CBE10 File Offset: 0x000CA010
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

	// Token: 0x060028EB RID: 10475 RVA: 0x000CBE80 File Offset: 0x000CA080
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (this.hasParentBoard)
		{
			FreeHoverboardManager.instance.SendGrabBoardRPC(this.parentFreeBoard);
			Transform transform = (flag ? VRRig.LocalRig.leftHand.rigTarget : VRRig.LocalRig.rightHand.rigTarget);
			Quaternion quaternion = transform.InverseTransformRotation(base.transform.rotation);
			Vector3 vector = transform.InverseTransformPoint(base.transform.position);
			GTPlayer.Instance.GrabPersonalHoverboard(flag, vector, quaternion, this.parentFreeBoard.boardColor);
			return;
		}
		Quaternion quaternion2 = (flag ? this.defaultHoldAngleLeft : this.defaultHoldAngleRight);
		Vector3 vector2 = (flag ? this.defaultHoldPosLeft : this.defaultHoldPosRight);
		GTPlayer.Instance.GrabPersonalHoverboard(flag, vector2, quaternion2, VRRig.LocalRig.playerColor);
	}

	// Token: 0x060028EC RID: 10476 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void DropItemCleanup()
	{
	}

	// Token: 0x060028ED RID: 10477 RVA: 0x00002628 File Offset: 0x00000828
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		throw new NotImplementedException();
	}

	// Token: 0x04002DF7 RID: 11767
	[SerializeField]
	private FreeHoverboardInstance parentFreeBoard;

	// Token: 0x04002DF8 RID: 11768
	private bool hasParentBoard;

	// Token: 0x04002DF9 RID: 11769
	[SerializeField]
	private Vector3 defaultHoldPosLeft;

	// Token: 0x04002DFA RID: 11770
	[SerializeField]
	private Vector3 defaultHoldPosRight;

	// Token: 0x04002DFB RID: 11771
	[SerializeField]
	private Quaternion defaultHoldAngleLeft;

	// Token: 0x04002DFC RID: 11772
	[SerializeField]
	private Quaternion defaultHoldAngleRight;

	// Token: 0x04002DFD RID: 11773
	private int noHapticsUntilFrame = -1;
}
