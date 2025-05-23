using System;
using UnityEngine;

// Token: 0x0200030A RID: 778
public class TeleportTargetHandlerPhysical : TeleportTargetHandler
{
	// Token: 0x06001299 RID: 4761 RVA: 0x00057AD4 File Offset: 0x00055CD4
	protected override bool ConsiderTeleport(Vector3 start, ref Vector3 end)
	{
		if (base.LocomotionTeleport.AimCollisionTest(start, end, this.AimCollisionLayerMask, out this.AimData.TargetHitInfo))
		{
			Vector3 normalized = (end - start).normalized;
			end = start + normalized * this.AimData.TargetHitInfo.distance;
			return true;
		}
		return false;
	}
}
