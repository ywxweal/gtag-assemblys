using System;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class LaserPointer : OVRCursor
{
	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x060011A5 RID: 4517 RVA: 0x00054B83 File Offset: 0x00052D83
	// (set) Token: 0x060011A4 RID: 4516 RVA: 0x00054B4F File Offset: 0x00052D4F
	public LaserPointer.LaserBeamBehavior laserBeamBehavior
	{
		get
		{
			return this._laserBeamBehavior;
		}
		set
		{
			this._laserBeamBehavior = value;
			if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.Off || this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.OnWhenHitTarget)
			{
				this.lineRenderer.enabled = false;
				return;
			}
			this.lineRenderer.enabled = true;
		}
	}

	// Token: 0x060011A6 RID: 4518 RVA: 0x00054B8B File Offset: 0x00052D8B
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x060011A7 RID: 4519 RVA: 0x00054B99 File Offset: 0x00052D99
	private void Start()
	{
		if (this.cursorVisual)
		{
			this.cursorVisual.SetActive(false);
		}
		OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
		OVRManager.InputFocusLost += this.OnInputFocusLost;
	}

	// Token: 0x060011A8 RID: 4520 RVA: 0x00054BD6 File Offset: 0x00052DD6
	public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
	{
		this._startPoint = start;
		this._endPoint = dest;
		this._hitTarget = true;
	}

	// Token: 0x060011A9 RID: 4521 RVA: 0x00054BED File Offset: 0x00052DED
	public override void SetCursorRay(Transform t)
	{
		this._startPoint = t.position;
		this._forward = t.forward;
		this._hitTarget = false;
	}

	// Token: 0x060011AA RID: 4522 RVA: 0x00054C10 File Offset: 0x00052E10
	private void LateUpdate()
	{
		this.lineRenderer.SetPosition(0, this._startPoint);
		if (this._hitTarget)
		{
			this.lineRenderer.SetPosition(1, this._endPoint);
			this.UpdateLaserBeam(this._startPoint, this._endPoint);
			if (this.cursorVisual)
			{
				this.cursorVisual.transform.position = this._endPoint;
				this.cursorVisual.SetActive(true);
				return;
			}
		}
		else
		{
			this.UpdateLaserBeam(this._startPoint, this._startPoint + this.maxLength * this._forward);
			this.lineRenderer.SetPosition(1, this._startPoint + this.maxLength * this._forward);
			if (this.cursorVisual)
			{
				this.cursorVisual.SetActive(false);
			}
		}
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x00054CF8 File Offset: 0x00052EF8
	private void UpdateLaserBeam(Vector3 start, Vector3 end)
	{
		if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.Off)
		{
			return;
		}
		if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.On)
		{
			this.lineRenderer.SetPosition(0, start);
			this.lineRenderer.SetPosition(1, end);
			return;
		}
		if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.OnWhenHitTarget)
		{
			if (this._hitTarget)
			{
				if (!this.lineRenderer.enabled)
				{
					this.lineRenderer.enabled = true;
					this.lineRenderer.SetPosition(0, start);
					this.lineRenderer.SetPosition(1, end);
					return;
				}
			}
			else if (this.lineRenderer.enabled)
			{
				this.lineRenderer.enabled = false;
			}
		}
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x00054D90 File Offset: 0x00052F90
	private void OnDisable()
	{
		if (this.cursorVisual)
		{
			this.cursorVisual.SetActive(false);
		}
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x00054DAB File Offset: 0x00052FAB
	public void OnInputFocusLost()
	{
		if (base.gameObject && base.gameObject.activeInHierarchy)
		{
			this.m_restoreOnInputAcquired = true;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x00054DDA File Offset: 0x00052FDA
	public void OnInputFocusAcquired()
	{
		if (this.m_restoreOnInputAcquired && base.gameObject)
		{
			this.m_restoreOnInputAcquired = false;
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x00054E04 File Offset: 0x00053004
	private void OnDestroy()
	{
		OVRManager.InputFocusAcquired -= this.OnInputFocusAcquired;
		OVRManager.InputFocusLost -= this.OnInputFocusLost;
	}

	// Token: 0x040013D4 RID: 5076
	public GameObject cursorVisual;

	// Token: 0x040013D5 RID: 5077
	public float maxLength = 10f;

	// Token: 0x040013D6 RID: 5078
	private LaserPointer.LaserBeamBehavior _laserBeamBehavior;

	// Token: 0x040013D7 RID: 5079
	private bool m_restoreOnInputAcquired;

	// Token: 0x040013D8 RID: 5080
	private Vector3 _startPoint;

	// Token: 0x040013D9 RID: 5081
	private Vector3 _forward;

	// Token: 0x040013DA RID: 5082
	private Vector3 _endPoint;

	// Token: 0x040013DB RID: 5083
	private bool _hitTarget;

	// Token: 0x040013DC RID: 5084
	private LineRenderer lineRenderer;

	// Token: 0x020002E2 RID: 738
	public enum LaserBeamBehavior
	{
		// Token: 0x040013DE RID: 5086
		On,
		// Token: 0x040013DF RID: 5087
		Off,
		// Token: 0x040013E0 RID: 5088
		OnWhenHitTarget
	}
}
