using System;
using UnityEngine;

// Token: 0x0200026B RID: 619
[RequireComponent(typeof(LineRenderer))]
public class LineRenderVelocityMapper : MonoBehaviour
{
	// Token: 0x06000E39 RID: 3641 RVA: 0x00048847 File Offset: 0x00046A47
	private void Awake()
	{
		this._lr = base.GetComponent<LineRenderer>();
		this._lr.useWorldSpace = true;
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x00048864 File Offset: 0x00046A64
	private void LateUpdate()
	{
		if (this.velocityEstimator == null)
		{
			return;
		}
		this._lr.SetPosition(0, this.velocityEstimator.transform.position);
		if (this.velocityEstimator.linearVelocity.sqrMagnitude > 0.1f)
		{
			this._lr.SetPosition(1, this.velocityEstimator.transform.position + this.velocityEstimator.linearVelocity.normalized * 0.2f);
			return;
		}
		this._lr.SetPosition(1, this.velocityEstimator.transform.position);
	}

	// Token: 0x0400118F RID: 4495
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001190 RID: 4496
	private LineRenderer _lr;
}
