using System;
using UnityEngine;

// Token: 0x02000694 RID: 1684
public class XfToXfLine : MonoBehaviour
{
	// Token: 0x06002A2A RID: 10794 RVA: 0x000D0712 File Offset: 0x000CE912
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06002A2B RID: 10795 RVA: 0x000D0720 File Offset: 0x000CE920
	private void Update()
	{
		this.lineRenderer.SetPosition(0, this.pt0.transform.position);
		this.lineRenderer.SetPosition(1, this.pt1.transform.position);
	}

	// Token: 0x04002F2E RID: 12078
	public Transform pt0;

	// Token: 0x04002F2F RID: 12079
	public Transform pt1;

	// Token: 0x04002F30 RID: 12080
	private LineRenderer lineRenderer;
}
