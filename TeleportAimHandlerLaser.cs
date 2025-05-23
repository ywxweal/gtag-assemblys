using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F3 RID: 755
public class TeleportAimHandlerLaser : TeleportAimHandler
{
	// Token: 0x0600122B RID: 4651 RVA: 0x000565CC File Offset: 0x000547CC
	public override void GetPoints(List<Vector3> points)
	{
		Ray ray;
		base.LocomotionTeleport.InputHandler.GetAimData(out ray);
		points.Add(ray.origin);
		points.Add(ray.origin + ray.direction * this.Range);
	}

	// Token: 0x04001446 RID: 5190
	[Tooltip("Maximum range for aiming.")]
	public float Range = 100f;
}
