using System;
using UnityEngine;

// Token: 0x020004EC RID: 1260
public class BuilderLaserSight : MonoBehaviour
{
	// Token: 0x06001E8B RID: 7819 RVA: 0x000954C1 File Offset: 0x000936C1
	public void Awake()
	{
		if (this.lineRenderer == null)
		{
			this.lineRenderer = base.GetComponentInChildren<LineRenderer>();
		}
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = false;
		}
	}

	// Token: 0x06001E8C RID: 7820 RVA: 0x000954F7 File Offset: 0x000936F7
	public void SetPoints(Vector3 start, Vector3 end)
	{
		this.lineRenderer.positionCount = 2;
		this.lineRenderer.SetPosition(0, start);
		this.lineRenderer.SetPosition(1, end);
	}

	// Token: 0x06001E8D RID: 7821 RVA: 0x0009551F File Offset: 0x0009371F
	public void Show(bool show)
	{
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = show;
		}
	}

	// Token: 0x040021E4 RID: 8676
	public LineRenderer lineRenderer;
}
