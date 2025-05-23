using System;
using UnityEngine;

// Token: 0x0200026A RID: 618
public class LineRendererDraw : MonoBehaviour
{
	// Token: 0x06000E35 RID: 3637 RVA: 0x000487E6 File Offset: 0x000469E6
	public void SetUpLine(Transform[] points)
	{
		this.lr.positionCount = points.Length;
		this.points = points;
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x00048800 File Offset: 0x00046A00
	private void LateUpdate()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			this.lr.SetPosition(i, this.points[i].position);
		}
	}

	// Token: 0x06000E37 RID: 3639 RVA: 0x00048839 File Offset: 0x00046A39
	public void Enable(bool enable)
	{
		this.lr.enabled = enable;
	}

	// Token: 0x0400118D RID: 4493
	public LineRenderer lr;

	// Token: 0x0400118E RID: 4494
	public Transform[] points;
}
