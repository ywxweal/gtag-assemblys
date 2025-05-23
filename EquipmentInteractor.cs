using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000399 RID: 921
public class EquipmentInteractor : MonoBehaviour
{
	// Token: 0x17000258 RID: 600
	// (get) Token: 0x06001565 RID: 5477 RVA: 0x00068863 File Offset: 0x00066A63
	public GorillaHandClimber BodyClimber
	{
		get
		{
			return this.bodyClimber;
		}
	}

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06001566 RID: 5478 RVA: 0x0006886B File Offset: 0x00066A6B
	public GorillaHandClimber LeftClimber
	{
		get
		{
			return this.leftClimber;
		}
	}

	// Token: 0x1700025A RID: 602
	// (get) Token: 0x06001567 RID: 5479 RVA: 0x00068873 File Offset: 0x00066A73
	public GorillaHandClimber RightClimber
	{
		get
		{
			return this.rightClimber;
		}
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x0006887C File Offset: 0x00066A7C
	private void Awake()
	{
		if (EquipmentInteractor.instance == null)
		{
			EquipmentInteractor.instance = this;
			EquipmentInteractor.hasInstance = true;
		}
		else if (EquipmentInteractor.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.autoGrabLeft = true;
		this.autoGrabRight = true;
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x000688D0 File Offset: 0x00066AD0
	private void OnDestroy()
	{
		if (EquipmentInteractor.instance == this)
		{
			EquipmentInteractor.hasInstance = false;
			EquipmentInteractor.instance = null;
		}
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x000688EF File Offset: 0x00066AEF
	public void ReleaseRightHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		this.autoGrabRight = true;
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x0006892E File Offset: 0x00066B2E
	public void ReleaseLeftHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		this.autoGrabLeft = true;
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x0006896D File Offset: 0x00066B6D
	public void ForceStopClimbing()
	{
		this.bodyClimber.ForceStopClimbing(false, false);
		this.leftClimber.ForceStopClimbing(false, false);
		this.rightClimber.ForceStopClimbing(false, false);
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x00068996 File Offset: 0x00066B96
	public bool GetIsHolding(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return this.leftHandHeldEquipment != null;
		}
		return this.rightHandHeldEquipment != null;
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x000689B0 File Offset: 0x00066BB0
	public void InteractionPointDisabled(InteractionPoint interactionPoint)
	{
		if (this.iteratingInteractionPoints)
		{
			this.interactionPointsToRemove.Add(interactionPoint);
			return;
		}
		if (this.overlapInteractionPointsLeft != null)
		{
			this.overlapInteractionPointsLeft.Remove(interactionPoint);
		}
		if (this.overlapInteractionPointsRight != null)
		{
			this.overlapInteractionPointsRight.Remove(interactionPoint);
		}
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x000689FC File Offset: 0x00066BFC
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.CheckInputValue(true);
		this.isLeftGrabbing = (this.wasLeftGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasLeftGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis);
		if (this.leftClimber && this.leftClimber.isClimbing)
		{
			this.isLeftGrabbing = false;
		}
		this.CheckInputValue(false);
		this.isRightGrabbing = (this.wasRightGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasRightGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis);
		if (this.rightClimber && this.rightClimber.isClimbing)
		{
			this.isRightGrabbing = false;
		}
		BuilderPiece builderPiece = this.builderPieceInteractor.heldPiece[0];
		BuilderPiece builderPiece2 = this.builderPieceInteractor.heldPiece[1];
		this.FireHandInteractions(this.leftHand, true, builderPiece);
		this.FireHandInteractions(this.rightHand, false, builderPiece2);
		if (!this.isRightGrabbing && this.wasRightGrabPressed)
		{
			this.ReleaseRightHand();
		}
		if (!this.isLeftGrabbing && this.wasLeftGrabPressed)
		{
			this.ReleaseLeftHand();
		}
		this.builderPieceInteractor.OnLateUpdate();
		if (GameBallPlayerLocal.instance != null)
		{
			GameBallPlayerLocal.instance.OnUpdateInteract();
		}
		if (GamePlayerLocal.instance != null)
		{
			GamePlayerLocal.instance.OnUpdateInteract();
		}
		this.wasLeftGrabPressed = this.isLeftGrabbing;
		this.wasRightGrabPressed = this.isRightGrabbing;
	}

	// Token: 0x06001570 RID: 5488 RVA: 0x00068BB8 File Offset: 0x00066DB8
	private void FireHandInteractions(GameObject interactingHand, bool isLeftHand, BuilderPiece pieceInHand)
	{
		if (isLeftHand)
		{
			this.justGrabbed = (this.isLeftGrabbing && !this.wasLeftGrabPressed) || (this.isLeftGrabbing && this.autoGrabLeft);
			this.justReleased = this.leftHandHeldEquipment != null && !this.isLeftGrabbing && this.wasLeftGrabPressed;
		}
		else
		{
			this.justGrabbed = (this.isRightGrabbing && !this.wasRightGrabPressed) || (this.isRightGrabbing && this.autoGrabRight);
			this.justReleased = this.rightHandHeldEquipment != null && !this.isRightGrabbing && this.wasRightGrabPressed;
		}
		List<InteractionPoint> list = (isLeftHand ? this.overlapInteractionPointsLeft : this.overlapInteractionPointsRight);
		bool flag = (isLeftHand ? (this.leftHandHeldEquipment != null) : (this.rightHandHeldEquipment != null));
		bool flag2 = pieceInHand != null;
		bool flag3 = (isLeftHand ? this.disableLeftGrab : this.disableRightGrab);
		bool flag4 = !flag && !flag2 && !flag3;
		this.iteratingInteractionPoints = true;
		foreach (InteractionPoint interactionPoint in list)
		{
			if (flag4 && interactionPoint != null)
			{
				if (this.justGrabbed)
				{
					interactionPoint.Holdable.OnGrab(interactionPoint, interactingHand);
				}
				else
				{
					interactionPoint.Holdable.OnHover(interactionPoint, interactingHand);
				}
			}
			if (this.justReleased)
			{
				this.tempZone = interactionPoint.GetComponent<DropZone>();
				if (this.tempZone != null)
				{
					if (interactingHand == this.leftHand)
					{
						if (this.leftHandHeldEquipment != null)
						{
							this.leftHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
						}
					}
					else if (this.rightHandHeldEquipment != null)
					{
						this.rightHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
					}
				}
			}
		}
		this.iteratingInteractionPoints = false;
		foreach (InteractionPoint interactionPoint2 in this.interactionPointsToRemove)
		{
			if (this.overlapInteractionPointsLeft != null)
			{
				this.overlapInteractionPointsLeft.Remove(interactionPoint2);
			}
			if (this.overlapInteractionPointsRight != null)
			{
				this.overlapInteractionPointsRight.Remove(interactionPoint2);
			}
		}
		this.interactionPointsToRemove.Clear();
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x00068E10 File Offset: 0x00067010
	public void UpdateHandEquipment(IHoldableObject newEquipment, bool forLeftHand)
	{
		if (forLeftHand)
		{
			if (newEquipment != null && newEquipment == this.rightHandHeldEquipment && !newEquipment.TwoHanded)
			{
				this.rightHandHeldEquipment = null;
			}
			if (this.leftHandHeldEquipment != null)
			{
				this.leftHandHeldEquipment.DropItemCleanup();
			}
			this.leftHandHeldEquipment = newEquipment;
			this.autoGrabLeft = false;
			return;
		}
		if (newEquipment != null && newEquipment == this.leftHandHeldEquipment && !newEquipment.TwoHanded)
		{
			this.leftHandHeldEquipment = null;
		}
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.DropItemCleanup();
		}
		this.rightHandHeldEquipment = newEquipment;
		this.autoGrabRight = false;
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x00068E9C File Offset: 0x0006709C
	public void CheckInputValue(bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.grabValue = ControllerInputPoller.GripFloat(XRNode.LeftHand);
			this.tempValue = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
		}
		else
		{
			this.grabValue = ControllerInputPoller.GripFloat(XRNode.RightHand);
			this.tempValue = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
		}
		this.grabValue = Mathf.Max(this.grabValue, this.tempValue);
	}

	// Token: 0x06001573 RID: 5491 RVA: 0x00068EF5 File Offset: 0x000670F5
	public void ForceDropEquipment(IHoldableObject equipment)
	{
		if (this.rightHandHeldEquipment == equipment)
		{
			this.rightHandHeldEquipment = null;
		}
		if (this.leftHandHeldEquipment == equipment)
		{
			this.leftHandHeldEquipment = null;
		}
	}

	// Token: 0x06001574 RID: 5492 RVA: 0x00068F18 File Offset: 0x00067118
	public void ForceDropManipulatableObject(HoldableObject manipulatableObject)
	{
		if ((HoldableObject)this.rightHandHeldEquipment == manipulatableObject)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
			this.rightHandHeldEquipment = null;
			this.autoGrabRight = false;
		}
		if ((HoldableObject)this.leftHandHeldEquipment == manipulatableObject)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
			this.leftHandHeldEquipment = null;
			this.autoGrabLeft = false;
		}
	}

	// Token: 0x040017D1 RID: 6097
	[OnEnterPlay_SetNull]
	public static volatile EquipmentInteractor instance;

	// Token: 0x040017D2 RID: 6098
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x040017D3 RID: 6099
	public IHoldableObject leftHandHeldEquipment;

	// Token: 0x040017D4 RID: 6100
	public IHoldableObject rightHandHeldEquipment;

	// Token: 0x040017D5 RID: 6101
	public BuilderPieceInteractor builderPieceInteractor;

	// Token: 0x040017D6 RID: 6102
	public GameObject rightHand;

	// Token: 0x040017D7 RID: 6103
	public GameObject leftHand;

	// Token: 0x040017D8 RID: 6104
	public InputDevice leftHandDevice;

	// Token: 0x040017D9 RID: 6105
	public InputDevice rightHandDevice;

	// Token: 0x040017DA RID: 6106
	public List<InteractionPoint> overlapInteractionPointsLeft = new List<InteractionPoint>();

	// Token: 0x040017DB RID: 6107
	public List<InteractionPoint> overlapInteractionPointsRight = new List<InteractionPoint>();

	// Token: 0x040017DC RID: 6108
	public float grabRadius;

	// Token: 0x040017DD RID: 6109
	public float grabThreshold = 0.7f;

	// Token: 0x040017DE RID: 6110
	public float grabHysteresis = 0.05f;

	// Token: 0x040017DF RID: 6111
	public bool wasLeftGrabPressed;

	// Token: 0x040017E0 RID: 6112
	public bool wasRightGrabPressed;

	// Token: 0x040017E1 RID: 6113
	public bool isLeftGrabbing;

	// Token: 0x040017E2 RID: 6114
	public bool isRightGrabbing;

	// Token: 0x040017E3 RID: 6115
	public bool justReleased;

	// Token: 0x040017E4 RID: 6116
	public bool justGrabbed;

	// Token: 0x040017E5 RID: 6117
	public bool disableLeftGrab;

	// Token: 0x040017E6 RID: 6118
	public bool disableRightGrab;

	// Token: 0x040017E7 RID: 6119
	public bool autoGrabLeft;

	// Token: 0x040017E8 RID: 6120
	public bool autoGrabRight;

	// Token: 0x040017E9 RID: 6121
	private float grabValue;

	// Token: 0x040017EA RID: 6122
	private float tempValue;

	// Token: 0x040017EB RID: 6123
	private DropZone tempZone;

	// Token: 0x040017EC RID: 6124
	private bool iteratingInteractionPoints;

	// Token: 0x040017ED RID: 6125
	private List<InteractionPoint> interactionPointsToRemove = new List<InteractionPoint>();

	// Token: 0x040017EE RID: 6126
	[SerializeField]
	private GorillaHandClimber bodyClimber;

	// Token: 0x040017EF RID: 6127
	[SerializeField]
	private GorillaHandClimber leftClimber;

	// Token: 0x040017F0 RID: 6128
	[SerializeField]
	private GorillaHandClimber rightClimber;
}
