using System;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class CloudUmbrellaCloud : MonoBehaviour
{
	// Token: 0x060005FF RID: 1535 RVA: 0x00022AA8 File Offset: 0x00020CA8
	protected void Awake()
	{
		this.umbrellaXform = this.umbrella.transform;
		this.cloudScaleXform = this.cloudRenderer.transform;
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x00022ACC File Offset: 0x00020CCC
	protected void LateUpdate()
	{
		float num = Vector3.Dot(this.umbrellaXform.up, Vector3.up);
		float num2 = Mathf.Clamp01(this.scaleCurve.Evaluate(num));
		this.rendererOn = ((num2 > 0.09f && num2 < 0.1f) ? this.rendererOn : (num2 > 0.1f));
		this.cloudRenderer.enabled = this.rendererOn;
		this.cloudScaleXform.localScale = new Vector3(num2, num2, num2);
		this.cloudRotateXform.up = Vector3.up;
	}

	// Token: 0x040006FA RID: 1786
	public UmbrellaItem umbrella;

	// Token: 0x040006FB RID: 1787
	public Transform cloudRotateXform;

	// Token: 0x040006FC RID: 1788
	public Renderer cloudRenderer;

	// Token: 0x040006FD RID: 1789
	public AnimationCurve scaleCurve;

	// Token: 0x040006FE RID: 1790
	private const float kHideAtScale = 0.1f;

	// Token: 0x040006FF RID: 1791
	private const float kHideAtScaleTolerance = 0.01f;

	// Token: 0x04000700 RID: 1792
	private bool rendererOn;

	// Token: 0x04000701 RID: 1793
	private Transform umbrellaXform;

	// Token: 0x04000702 RID: 1794
	private Transform cloudScaleXform;
}
