using System;
using UnityEngine;

// Token: 0x02000690 RID: 1680
public class TapInnerGlow : MonoBehaviour
{
	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x06002A04 RID: 10756 RVA: 0x000CFD08 File Offset: 0x000CDF08
	private Material targetMaterial
	{
		get
		{
			if (this._instance.AsNull<Material>() == null)
			{
				return this._instance = this._renderer.material;
			}
			return this._instance;
		}
	}

	// Token: 0x06002A05 RID: 10757 RVA: 0x000CFD44 File Offset: 0x000CDF44
	public void Tap()
	{
		if (!this._renderer)
		{
			return;
		}
		Material targetMaterial = this.targetMaterial;
		float num = this.tapLength;
		float time = GTShaderGlobals.Time;
		UberShader.InnerGlowSinePeriod.SetValue<float>(targetMaterial, num);
		UberShader.InnerGlowSinePhaseShift.SetValue<float>(targetMaterial, time);
	}

	// Token: 0x04002F23 RID: 12067
	public Renderer _renderer;

	// Token: 0x04002F24 RID: 12068
	public float tapLength = 1f;

	// Token: 0x04002F25 RID: 12069
	[Space]
	private Material _instance;
}
