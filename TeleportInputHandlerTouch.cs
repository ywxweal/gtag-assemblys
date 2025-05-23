using System;
using UnityEngine;

// Token: 0x020002FB RID: 763
public class TeleportInputHandlerTouch : TeleportInputHandlerHMD
{
	// Token: 0x0600125B RID: 4699 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Start()
	{
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x00056D7C File Offset: 0x00054F7C
	public override LocomotionTeleport.TeleportIntentions GetIntention()
	{
		if (!base.isActiveAndEnabled)
		{
			return LocomotionTeleport.TeleportIntentions.None;
		}
		if (this.InputMode == TeleportInputHandlerTouch.InputModes.SeparateButtonsForAimAndTeleport)
		{
			return base.GetIntention();
		}
		if (this.InputMode == TeleportInputHandlerTouch.InputModes.ThumbstickTeleport || this.InputMode == TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly)
		{
			Vector2 vector = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.Active);
			Vector2 vector2 = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Active);
			bool flag = OVRInput.Get(OVRInput.RawTouch.LThumbstick, OVRInput.Controller.Active);
			bool flag2 = OVRInput.Get(OVRInput.RawTouch.RThumbstick, OVRInput.Controller.Active);
			float num;
			float num2;
			if (this.InputMode == TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly && base.LocomotionTeleport.CurrentIntention != LocomotionTeleport.TeleportIntentions.Aim)
			{
				num = Mathf.Abs(Vector2.Dot(vector, Vector2.up));
				num2 = Mathf.Abs(Vector2.Dot(vector2, Vector2.up));
			}
			else
			{
				num = vector.magnitude;
				num2 = vector2.magnitude;
			}
			float num3;
			OVRInput.Controller controller;
			if (this.AimingController == OVRInput.Controller.LTouch)
			{
				num3 = num;
				controller = OVRInput.Controller.LTouch;
			}
			else if (this.AimingController == OVRInput.Controller.RTouch)
			{
				num3 = num2;
				controller = OVRInput.Controller.RTouch;
			}
			else if (num > num2)
			{
				num3 = num;
				controller = OVRInput.Controller.LTouch;
			}
			else
			{
				num3 = num2;
				controller = OVRInput.Controller.RTouch;
			}
			if (num3 <= this.ThumbstickTeleportThreshold && (this.AimingController != OVRInput.Controller.Touch || (!flag && !flag2)) && (this.AimingController != OVRInput.Controller.LTouch || !flag) && (this.AimingController != OVRInput.Controller.RTouch || !flag2))
			{
				if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
				{
					if (!this.FastTeleport)
					{
						return LocomotionTeleport.TeleportIntentions.PreTeleport;
					}
					return LocomotionTeleport.TeleportIntentions.Teleport;
				}
				else if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
				{
					return LocomotionTeleport.TeleportIntentions.Teleport;
				}
			}
			else if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim)
			{
				return LocomotionTeleport.TeleportIntentions.Aim;
			}
			if (num3 > this.ThumbstickTeleportThreshold)
			{
				this.InitiatingController = controller;
				return LocomotionTeleport.TeleportIntentions.Aim;
			}
			return LocomotionTeleport.TeleportIntentions.None;
		}
		else
		{
			OVRInput.RawButton rawButton = this._rawButtons[(int)this.CapacitiveAimAndTeleportButton];
			if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && OVRInput.GetDown(rawButton, OVRInput.Controller.Active))
			{
				if (!this.FastTeleport)
				{
					return LocomotionTeleport.TeleportIntentions.PreTeleport;
				}
				return LocomotionTeleport.TeleportIntentions.Teleport;
			}
			else if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
			{
				if (this.FastTeleport || OVRInput.GetUp(rawButton, OVRInput.Controller.Active))
				{
					return LocomotionTeleport.TeleportIntentions.Teleport;
				}
				return LocomotionTeleport.TeleportIntentions.PreTeleport;
			}
			else
			{
				if (OVRInput.GetDown(this._rawTouch[(int)this.CapacitiveAimAndTeleportButton], OVRInput.Controller.Active))
				{
					return LocomotionTeleport.TeleportIntentions.Aim;
				}
				if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && !OVRInput.GetUp(this._rawTouch[(int)this.CapacitiveAimAndTeleportButton], OVRInput.Controller.Active))
				{
					return LocomotionTeleport.TeleportIntentions.Aim;
				}
				return LocomotionTeleport.TeleportIntentions.None;
			}
		}
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x00056FB4 File Offset: 0x000551B4
	public override void GetAimData(out Ray aimRay)
	{
		OVRInput.Controller controller = this.AimingController;
		if (controller == OVRInput.Controller.Touch)
		{
			controller = this.InitiatingController;
		}
		Transform transform = ((controller == OVRInput.Controller.LTouch) ? this.LeftHand : this.RightHand);
		aimRay = new Ray(transform.position, transform.forward);
	}

	// Token: 0x04001467 RID: 5223
	public Transform LeftHand;

	// Token: 0x04001468 RID: 5224
	public Transform RightHand;

	// Token: 0x04001469 RID: 5225
	[Tooltip("CapacitiveButtonForAimAndTeleport=Activate aiming via cap touch detection, press the same button to teleport.\nSeparateButtonsForAimAndTeleport=Use one button to begin aiming, and another to trigger the teleport.\nThumbstickTeleport=Push a thumbstick to begin aiming, release to teleport.")]
	public TeleportInputHandlerTouch.InputModes InputMode;

	// Token: 0x0400146A RID: 5226
	private readonly OVRInput.RawButton[] _rawButtons = new OVRInput.RawButton[]
	{
		OVRInput.RawButton.A,
		OVRInput.RawButton.B,
		OVRInput.RawButton.LIndexTrigger,
		OVRInput.RawButton.LThumbstick,
		OVRInput.RawButton.RIndexTrigger,
		OVRInput.RawButton.RThumbstick,
		OVRInput.RawButton.X,
		OVRInput.RawButton.Y
	};

	// Token: 0x0400146B RID: 5227
	private readonly OVRInput.RawTouch[] _rawTouch = new OVRInput.RawTouch[]
	{
		OVRInput.RawTouch.A,
		OVRInput.RawTouch.B,
		OVRInput.RawTouch.LIndexTrigger,
		OVRInput.RawTouch.LThumbstick,
		OVRInput.RawTouch.RIndexTrigger,
		OVRInput.RawTouch.RThumbstick,
		OVRInput.RawTouch.X,
		OVRInput.RawTouch.Y
	};

	// Token: 0x0400146C RID: 5228
	[Tooltip("Select the controller to be used for aiming. Supports LTouch, RTouch, or Touch for either.")]
	public OVRInput.Controller AimingController;

	// Token: 0x0400146D RID: 5229
	private OVRInput.Controller InitiatingController;

	// Token: 0x0400146E RID: 5230
	[Tooltip("Select the button to use for triggering aim and teleport when InputMode==CapacitiveButtonForAimAndTeleport")]
	public TeleportInputHandlerTouch.AimCapTouchButtons CapacitiveAimAndTeleportButton;

	// Token: 0x0400146F RID: 5231
	[Tooltip("The thumbstick magnitude required to trigger aiming and teleports when InputMode==InputModes.ThumbstickTeleport")]
	public float ThumbstickTeleportThreshold = 0.5f;

	// Token: 0x020002FC RID: 764
	public enum InputModes
	{
		// Token: 0x04001471 RID: 5233
		CapacitiveButtonForAimAndTeleport,
		// Token: 0x04001472 RID: 5234
		SeparateButtonsForAimAndTeleport,
		// Token: 0x04001473 RID: 5235
		ThumbstickTeleport,
		// Token: 0x04001474 RID: 5236
		ThumbstickTeleportForwardBackOnly
	}

	// Token: 0x020002FD RID: 765
	public enum AimCapTouchButtons
	{
		// Token: 0x04001476 RID: 5238
		A,
		// Token: 0x04001477 RID: 5239
		B,
		// Token: 0x04001478 RID: 5240
		LeftTrigger,
		// Token: 0x04001479 RID: 5241
		LeftThumbstick,
		// Token: 0x0400147A RID: 5242
		RightTrigger,
		// Token: 0x0400147B RID: 5243
		RightThumbstick,
		// Token: 0x0400147C RID: 5244
		X,
		// Token: 0x0400147D RID: 5245
		Y
	}
}
