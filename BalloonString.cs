using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000397 RID: 919
public class BalloonString : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600155C RID: 5468 RVA: 0x00068704 File Offset: 0x00066904
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.vertices = new List<Vector3>(this.numSegments + 1);
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.vertices.Add(this.startPositionXf.position);
			int num = this.vertices.Count - 2;
			for (int i = 0; i < num; i++)
			{
				float num2 = (float)((i + 1) / (this.vertices.Count - 1));
				Vector3 vector = Vector3.Lerp(this.startPositionXf.position, this.endPositionXf.position, num2);
				this.vertices.Add(vector);
			}
			this.vertices.Add(this.endPositionXf.position);
		}
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x000687D4 File Offset: 0x000669D4
	private void UpdateDynamics()
	{
		this.vertices[0] = this.startPositionXf.position;
		this.vertices[this.vertices.Count - 1] = this.endPositionXf.position;
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x00068810 File Offset: 0x00066A10
	private void UpdateRenderPositions()
	{
		this.lineRenderer.SetPosition(0, this.startPositionXf.transform.position);
		this.lineRenderer.SetPosition(1, this.endPositionXf.transform.position);
	}

	// Token: 0x0600155F RID: 5471 RVA: 0x00010F2B File Offset: 0x0000F12B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x00010F34 File Offset: 0x0000F134
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x0006884A File Offset: 0x00066A4A
	public void SliceUpdate()
	{
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.UpdateDynamics();
			this.UpdateRenderPositions();
		}
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x040017C9 RID: 6089
	public Transform startPositionXf;

	// Token: 0x040017CA RID: 6090
	public Transform endPositionXf;

	// Token: 0x040017CB RID: 6091
	private List<Vector3> vertices;

	// Token: 0x040017CC RID: 6092
	public int numSegments = 1;

	// Token: 0x040017CD RID: 6093
	private LineRenderer lineRenderer;
}
