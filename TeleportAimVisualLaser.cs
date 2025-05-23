using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public class TeleportAimVisualLaser : TeleportSupport
{
	// Token: 0x0600122F RID: 4655 RVA: 0x00056723 File Offset: 0x00054923
	public TeleportAimVisualLaser()
	{
		this._enterAimStateAction = new Action(this.EnterAimState);
		this._exitAimStateAction = new Action(this.ExitAimState);
		this._updateAimDataAction = new Action<LocomotionTeleport.AimData>(this.UpdateAimData);
	}

	// Token: 0x06001230 RID: 4656 RVA: 0x00056761 File Offset: 0x00054961
	private void EnterAimState()
	{
		this._lineRenderer.gameObject.SetActive(true);
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x00056774 File Offset: 0x00054974
	private void ExitAimState()
	{
		this._lineRenderer.gameObject.SetActive(false);
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x00056787 File Offset: 0x00054987
	private void Awake()
	{
		this.LaserPrefab.gameObject.SetActive(false);
		this._lineRenderer = Object.Instantiate<LineRenderer>(this.LaserPrefab);
	}

	// Token: 0x06001233 RID: 4659 RVA: 0x000567AB File Offset: 0x000549AB
	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._enterAimStateAction;
		base.LocomotionTeleport.ExitStateAim += this._exitAimStateAction;
		base.LocomotionTeleport.UpdateAimData += this._updateAimDataAction;
	}

	// Token: 0x06001234 RID: 4660 RVA: 0x000567E6 File Offset: 0x000549E6
	protected override void RemoveEventHandlers()
	{
		base.LocomotionTeleport.EnterStateAim -= this._enterAimStateAction;
		base.LocomotionTeleport.ExitStateAim -= this._exitAimStateAction;
		base.LocomotionTeleport.UpdateAimData -= this._updateAimDataAction;
		base.RemoveEventHandlers();
	}

	// Token: 0x06001235 RID: 4661 RVA: 0x00056824 File Offset: 0x00054A24
	private void UpdateAimData(LocomotionTeleport.AimData obj)
	{
		this._lineRenderer.sharedMaterial.color = (obj.TargetValid ? Color.green : Color.red);
		List<Vector3> points = obj.Points;
		this._lineRenderer.positionCount = points.Count;
		for (int i = 0; i < points.Count; i++)
		{
			this._lineRenderer.SetPosition(i, points[i]);
		}
	}

	// Token: 0x0400144C RID: 5196
	[Tooltip("This prefab will be instantiated when the aim visual is awakened, and will be set active when the user is aiming, and deactivated when they are done aiming.")]
	public LineRenderer LaserPrefab;

	// Token: 0x0400144D RID: 5197
	private readonly Action _enterAimStateAction;

	// Token: 0x0400144E RID: 5198
	private readonly Action _exitAimStateAction;

	// Token: 0x0400144F RID: 5199
	private readonly Action<LocomotionTeleport.AimData> _updateAimDataAction;

	// Token: 0x04001450 RID: 5200
	private LineRenderer _lineRenderer;

	// Token: 0x04001451 RID: 5201
	private Vector3[] _linePoints;
}
