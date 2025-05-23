using System;

// Token: 0x0200030E RID: 782
public class TeleportTransitionInstant : TeleportTransition
{
	// Token: 0x060012A8 RID: 4776 RVA: 0x00057D06 File Offset: 0x00055F06
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.LocomotionTeleport.DoTeleport();
	}
}
