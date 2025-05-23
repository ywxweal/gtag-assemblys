using System;
using UnityEngine;

// Token: 0x02000302 RID: 770
public class TeleportOrientationHandlerHMD : TeleportOrientationHandler
{
	// Token: 0x06001271 RID: 4721 RVA: 0x00057208 File Offset: 0x00055408
	protected override void InitializeTeleportDestination()
	{
		this._initialRotation = Quaternion.identity;
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x00057218 File Offset: 0x00055418
	protected override void UpdateTeleportDestination()
	{
		if (this.AimData.Destination != null && (this.UpdateOrientationDuringAim || base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.PreTeleport))
		{
			Transform centerEyeAnchor = base.LocomotionTeleport.LocomotionController.CameraRig.centerEyeAnchor;
			Vector3 valueOrDefault = this.AimData.Destination.GetValueOrDefault();
			Plane plane = new Plane(Vector3.up, valueOrDefault);
			float num;
			if (plane.Raycast(new Ray(centerEyeAnchor.position, centerEyeAnchor.forward), out num))
			{
				Vector3 vector = centerEyeAnchor.position + centerEyeAnchor.forward * num - valueOrDefault;
				vector.y = 0f;
				float magnitude = vector.magnitude;
				if (magnitude > this.AimDistanceThreshold)
				{
					vector.Normalize();
					Quaternion quaternion = Quaternion.LookRotation(new Vector3(vector.x, 0f, vector.z), Vector3.up);
					this._initialRotation = quaternion;
					if (this.AimDistanceMaxRange > 0f && magnitude > this.AimDistanceMaxRange)
					{
						this.AimData.TargetValid = false;
					}
					base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, new Quaternion?(quaternion), new Quaternion?(base.GetLandingOrientation(this.OrientationMode, quaternion)));
					return;
				}
			}
		}
		base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, new Quaternion?(this._initialRotation), new Quaternion?(base.GetLandingOrientation(this.OrientationMode, this._initialRotation)));
	}

	// Token: 0x04001487 RID: 5255
	[Tooltip("HeadRelative=Character will orient to match the arrow. ForwardFacing=When user orients to match the arrow, they will be facing the sensors.")]
	public TeleportOrientationHandler.OrientationModes OrientationMode;

	// Token: 0x04001488 RID: 5256
	[Tooltip("Should the destination orientation be updated during the aim state in addition to the PreTeleport state?")]
	public bool UpdateOrientationDuringAim;

	// Token: 0x04001489 RID: 5257
	[Tooltip("How far from the destination must the HMD be pointing before using it for orientation")]
	public float AimDistanceThreshold;

	// Token: 0x0400148A RID: 5258
	[Tooltip("How far from the destination must the HMD be pointing before rejecting the teleport")]
	public float AimDistanceMaxRange;

	// Token: 0x0400148B RID: 5259
	private Quaternion _initialRotation;
}
