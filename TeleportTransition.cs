using System;

// Token: 0x0200030B RID: 779
public abstract class TeleportTransition : TeleportSupport
{
	// Token: 0x0600129B RID: 4763 RVA: 0x00057B48 File Offset: 0x00055D48
	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting += this.LocomotionTeleportOnEnterStateTeleporting;
		base.AddEventHandlers();
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x00057B68 File Offset: 0x00055D68
	protected override void RemoveEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting -= this.LocomotionTeleportOnEnterStateTeleporting;
		base.RemoveEventHandlers();
	}

	// Token: 0x0600129D RID: 4765
	protected abstract void LocomotionTeleportOnEnterStateTeleporting();
}
