using System;
using UnityEngine;

// Token: 0x020002FA RID: 762
public class TeleportInputHandlerHMD : TeleportInputHandler
{
	// Token: 0x1700020F RID: 527
	// (get) Token: 0x06001256 RID: 4694 RVA: 0x00056C9A File Offset: 0x00054E9A
	// (set) Token: 0x06001257 RID: 4695 RVA: 0x00056CA2 File Offset: 0x00054EA2
	public Transform Pointer { get; private set; }

	// Token: 0x06001258 RID: 4696 RVA: 0x00056CAC File Offset: 0x00054EAC
	public override LocomotionTeleport.TeleportIntentions GetIntention()
	{
		if (!base.isActiveAndEnabled)
		{
			return LocomotionTeleport.TeleportIntentions.None;
		}
		if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && OVRInput.GetDown(this.TeleportButton, OVRInput.Controller.Active))
		{
			if (!this.FastTeleport)
			{
				return LocomotionTeleport.TeleportIntentions.PreTeleport;
			}
			return LocomotionTeleport.TeleportIntentions.Teleport;
		}
		else if (base.LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.PreTeleport)
		{
			if (OVRInput.GetUp(this.TeleportButton, OVRInput.Controller.Active))
			{
				return LocomotionTeleport.TeleportIntentions.Teleport;
			}
			return LocomotionTeleport.TeleportIntentions.PreTeleport;
		}
		else
		{
			if (OVRInput.Get(this.AimButton, OVRInput.Controller.Active))
			{
				return LocomotionTeleport.TeleportIntentions.Aim;
			}
			if (this.AimButton == this.TeleportButton)
			{
				return LocomotionTeleport.TeleportIntentions.Teleport;
			}
			return LocomotionTeleport.TeleportIntentions.None;
		}
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x00056D38 File Offset: 0x00054F38
	public override void GetAimData(out Ray aimRay)
	{
		Transform centerEyeAnchor = base.LocomotionTeleport.LocomotionController.CameraRig.centerEyeAnchor;
		aimRay = new Ray(centerEyeAnchor.position, centerEyeAnchor.forward);
	}

	// Token: 0x04001464 RID: 5220
	[Tooltip("The button used to begin aiming for a teleport.")]
	public OVRInput.RawButton AimButton;

	// Token: 0x04001465 RID: 5221
	[Tooltip("The button used to trigger the teleport after aiming. It can be the same button as the AimButton, however you cannot abort a teleport if it is.")]
	public OVRInput.RawButton TeleportButton;

	// Token: 0x04001466 RID: 5222
	[Tooltip("When true, the system will not use the PreTeleport intention which will allow a teleport to occur on a button downpress. When false, the button downpress will trigger the PreTeleport intention and the Teleport intention when the button is released.")]
	public bool FastTeleport;
}
