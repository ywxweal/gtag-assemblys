using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002F7 RID: 759
public abstract class TeleportInputHandler : TeleportSupport
{
	// Token: 0x06001241 RID: 4673 RVA: 0x00056AA9 File Offset: 0x00054CA9
	protected TeleportInputHandler()
	{
		this._startReadyAction = delegate
		{
			base.StartCoroutine(this.TeleportReadyCoroutine());
		};
		this._startAimAction = delegate
		{
			base.StartCoroutine(this.TeleportAimCoroutine());
		};
	}

	// Token: 0x06001242 RID: 4674 RVA: 0x00056AD5 File Offset: 0x00054CD5
	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.InputHandler = this;
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateReady += this._startReadyAction;
		base.LocomotionTeleport.EnterStateAim += this._startAimAction;
	}

	// Token: 0x06001243 RID: 4675 RVA: 0x00056B0C File Offset: 0x00054D0C
	protected override void RemoveEventHandlers()
	{
		if (base.LocomotionTeleport.InputHandler == this)
		{
			base.LocomotionTeleport.InputHandler = null;
		}
		base.LocomotionTeleport.EnterStateReady -= this._startReadyAction;
		base.LocomotionTeleport.EnterStateAim -= this._startAimAction;
		base.RemoveEventHandlers();
	}

	// Token: 0x06001244 RID: 4676 RVA: 0x00056B60 File Offset: 0x00054D60
	private IEnumerator TeleportReadyCoroutine()
	{
		while (this.GetIntention() != LocomotionTeleport.TeleportIntentions.Aim)
		{
			yield return null;
		}
		base.LocomotionTeleport.CurrentIntention = LocomotionTeleport.TeleportIntentions.Aim;
		yield break;
	}

	// Token: 0x06001245 RID: 4677 RVA: 0x00056B6F File Offset: 0x00054D6F
	private IEnumerator TeleportAimCoroutine()
	{
		LocomotionTeleport.TeleportIntentions teleportIntentions = this.GetIntention();
		while (teleportIntentions == LocomotionTeleport.TeleportIntentions.Aim || teleportIntentions == LocomotionTeleport.TeleportIntentions.PreTeleport)
		{
			base.LocomotionTeleport.CurrentIntention = teleportIntentions;
			yield return null;
			teleportIntentions = this.GetIntention();
		}
		base.LocomotionTeleport.CurrentIntention = teleportIntentions;
		yield break;
	}

	// Token: 0x06001246 RID: 4678
	public abstract LocomotionTeleport.TeleportIntentions GetIntention();

	// Token: 0x06001247 RID: 4679
	public abstract void GetAimData(out Ray aimRay);

	// Token: 0x0400145B RID: 5211
	private readonly Action _startReadyAction;

	// Token: 0x0400145C RID: 5212
	private readonly Action _startAimAction;
}
