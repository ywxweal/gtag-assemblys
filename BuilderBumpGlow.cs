using System;
using UnityEngine;

// Token: 0x020004E0 RID: 1248
public class BuilderBumpGlow : MonoBehaviour
{
	// Token: 0x06001E1B RID: 7707 RVA: 0x00092580 File Offset: 0x00090780
	public void Awake()
	{
		this.blendIn = 1f;
		this.intensity = 0f;
		this.UpdateRender();
	}

	// Token: 0x06001E1C RID: 7708 RVA: 0x0009259E File Offset: 0x0009079E
	public void SetIntensity(float intensity)
	{
		this.intensity = intensity;
		this.UpdateRender();
	}

	// Token: 0x06001E1D RID: 7709 RVA: 0x000925AD File Offset: 0x000907AD
	public void SetBlendIn(float blendIn)
	{
		this.blendIn = blendIn;
		this.UpdateRender();
	}

	// Token: 0x06001E1E RID: 7710 RVA: 0x000023F4 File Offset: 0x000005F4
	private void UpdateRender()
	{
	}

	// Token: 0x04002164 RID: 8548
	public MeshRenderer glowRenderer;

	// Token: 0x04002165 RID: 8549
	private float blendIn;

	// Token: 0x04002166 RID: 8550
	private float intensity;
}
