using System;

// Token: 0x02000301 RID: 769
public class TeleportOrientationHandler360 : TeleportOrientationHandler
{
	// Token: 0x0600126E RID: 4718 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void InitializeTeleportDestination()
	{
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x000571C0 File Offset: 0x000553C0
	protected override void UpdateTeleportDestination()
	{
		base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, null, null);
	}
}
