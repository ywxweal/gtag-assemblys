using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002FE RID: 766
public abstract class TeleportOrientationHandler : TeleportSupport
{
	// Token: 0x0600125F RID: 4703 RVA: 0x0005704C File Offset: 0x0005524C
	protected TeleportOrientationHandler()
	{
		this._updateOrientationAction = delegate
		{
			base.StartCoroutine(this.UpdateOrientationCoroutine());
		};
		this._updateAimDataAction = new Action<LocomotionTeleport.AimData>(this.UpdateAimData);
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x00057078 File Offset: 0x00055278
	private void UpdateAimData(LocomotionTeleport.AimData aimData)
	{
		this.AimData = aimData;
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x00057081 File Offset: 0x00055281
	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._updateOrientationAction;
		base.LocomotionTeleport.UpdateAimData += this._updateAimDataAction;
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x000570AB File Offset: 0x000552AB
	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= this._updateOrientationAction;
		base.LocomotionTeleport.UpdateAimData -= this._updateAimDataAction;
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x000570D5 File Offset: 0x000552D5
	private IEnumerator UpdateOrientationCoroutine()
	{
		this.InitializeTeleportDestination();
		while (base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim || base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.PreTeleport)
		{
			if (this.AimData != null)
			{
				this.UpdateTeleportDestination();
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06001264 RID: 4708
	protected abstract void InitializeTeleportDestination();

	// Token: 0x06001265 RID: 4709
	protected abstract void UpdateTeleportDestination();

	// Token: 0x06001266 RID: 4710 RVA: 0x000570E4 File Offset: 0x000552E4
	protected Quaternion GetLandingOrientation(TeleportOrientationHandler.OrientationModes mode, Quaternion rotation)
	{
		if (mode != TeleportOrientationHandler.OrientationModes.HeadRelative)
		{
			return rotation * Quaternion.Euler(0f, -base.LocomotionTeleport.LocomotionController.CameraRig.trackingSpace.localEulerAngles.y, 0f);
		}
		return rotation;
	}

	// Token: 0x0400147E RID: 5246
	private readonly Action _updateOrientationAction;

	// Token: 0x0400147F RID: 5247
	private readonly Action<LocomotionTeleport.AimData> _updateAimDataAction;

	// Token: 0x04001480 RID: 5248
	protected LocomotionTeleport.AimData AimData;

	// Token: 0x020002FF RID: 767
	public enum OrientationModes
	{
		// Token: 0x04001482 RID: 5250
		HeadRelative,
		// Token: 0x04001483 RID: 5251
		ForwardFacing
	}
}
