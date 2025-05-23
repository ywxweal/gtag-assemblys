using System;
using UnityEngine;

// Token: 0x02000694 RID: 1684
public class XfToXfLine : MonoBehaviour
{
	// Token: 0x06002A2B RID: 10795 RVA: 0x000D07B6 File Offset: 0x000CE9B6
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06002A2C RID: 10796 RVA: 0x000D07C4 File Offset: 0x000CE9C4
	private void Update()
	{
		this.lineRenderer.SetPosition(0, this.pt0.transform.position);
		this.lineRenderer.SetPosition(1, this.pt1.transform.position);
	}

	// Token: 0x04002F30 RID: 12080
	public Transform pt0;

	// Token: 0x04002F31 RID: 12081
	public Transform pt1;

	// Token: 0x04002F32 RID: 12082
	private LineRenderer lineRenderer;
}
