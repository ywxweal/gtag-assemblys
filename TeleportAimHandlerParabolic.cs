using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F4 RID: 756
public class TeleportAimHandlerParabolic : TeleportAimHandler
{
	// Token: 0x0600122D RID: 4653 RVA: 0x00056630 File Offset: 0x00054830
	public override void GetPoints(List<Vector3> points)
	{
		Ray ray;
		base.LocomotionTeleport.InputHandler.GetAimData(out ray);
		Vector3 vector = ray.origin;
		Vector3 vector2 = ray.direction * this.AimVelocity;
		float num = this.Range * this.Range;
		do
		{
			points.Add(vector);
			Vector3 vector3 = vector2;
			vector3.y += this.Gravity * 0.011111111f * this.AimStep;
			vector2 = vector3;
			vector += vector3 * this.AimStep;
		}
		while (vector.y - ray.origin.y > this.MinimumElevation && (ray.origin - vector).sqrMagnitude <= num);
	}

	// Token: 0x04001447 RID: 5191
	[Tooltip("Maximum range for aiming.")]
	public float Range;

	// Token: 0x04001448 RID: 5192
	[Tooltip("The MinimumElevation is relative to the AimPosition.")]
	public float MinimumElevation = -100f;

	// Token: 0x04001449 RID: 5193
	[Tooltip("The Gravity is used in conjunction with AimVelocity and the aim direction to simulate a projectile.")]
	public float Gravity = -9.8f;

	// Token: 0x0400144A RID: 5194
	[Tooltip("The AimVelocity is the initial speed of the faked projectile.")]
	[Range(0.001f, 50f)]
	public float AimVelocity = 1f;

	// Token: 0x0400144B RID: 5195
	[Tooltip("The AimStep is the how much to subdivide the iteration.")]
	[Range(0.001f, 1f)]
	public float AimStep = 1f;
}
