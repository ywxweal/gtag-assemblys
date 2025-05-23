using System;
using UnityEngine;

// Token: 0x020009BF RID: 2495
public class BezierCurve : MonoBehaviour
{
	// Token: 0x06003BA5 RID: 15269 RVA: 0x0011CFDC File Offset: 0x0011B1DC
	public Vector3 GetPoint(float t)
	{
		return base.transform.TransformPoint(Bezier.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], t));
	}

	// Token: 0x06003BA6 RID: 15270 RVA: 0x0011D02C File Offset: 0x0011B22C
	public Vector3 GetVelocity(float t)
	{
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], this.points[3], t)) - base.transform.position;
	}

	// Token: 0x06003BA7 RID: 15271 RVA: 0x0011D08C File Offset: 0x0011B28C
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x06003BA8 RID: 15272 RVA: 0x0011D0A8 File Offset: 0x0011B2A8
	public void Reset()
	{
		this.points = new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
	}

	// Token: 0x04004006 RID: 16390
	public Vector3[] points;
}
