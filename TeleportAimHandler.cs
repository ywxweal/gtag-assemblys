using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F2 RID: 754
public abstract class TeleportAimHandler : TeleportSupport
{
	// Token: 0x06001227 RID: 4647 RVA: 0x00056588 File Offset: 0x00054788
	protected override void OnEnable()
	{
		base.OnEnable();
		base.LocomotionTeleport.AimHandler = this;
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x0005659C File Offset: 0x0005479C
	protected override void OnDisable()
	{
		if (base.LocomotionTeleport.AimHandler == this)
		{
			base.LocomotionTeleport.AimHandler = null;
		}
		base.OnDisable();
	}

	// Token: 0x06001229 RID: 4649
	public abstract void GetPoints(List<Vector3> points);
}
