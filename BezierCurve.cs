using System;
using UnityEngine;

// Token: 0x020009BF RID: 2495
public class BezierCurve : MonoBehaviour
{
	// Token: 0x06003BA4 RID: 15268 RVA: 0x0011CF04 File Offset: 0x0011B104
	public Vector3 GetPoint(float t)
	{
		return base.transform.TransformPoint(Bezier.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], t));
	}

	// Token: 0x06003BA5 RID: 15269 RVA: 0x0011CF54 File Offset: 0x0011B154
	public Vector3 GetVelocity(float t)
	{
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], this.points[3], t)) - base.transform.position;
	}

	// Token: 0x06003BA6 RID: 15270 RVA: 0x0011CFB4 File Offset: 0x0011B1B4
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x06003BA7 RID: 15271 RVA: 0x0011CFD0 File Offset: 0x0011B1D0
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

	// Token: 0x04004005 RID: 16389
	public Vector3[] points;
}
