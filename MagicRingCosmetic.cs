using System;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class MagicRingCosmetic : MonoBehaviour
{
	// Token: 0x06000617 RID: 1559 RVA: 0x000230B7 File Offset: 0x000212B7
	protected void Awake()
	{
		this.materialPropertyBlock = new MaterialPropertyBlock();
		this.defaultEmissiveColor = this.ringRenderer.sharedMaterial.GetColor(this.emissionColorShaderPropID);
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x000230E0 File Offset: 0x000212E0
	protected void LateUpdate()
	{
		float celsius = this.thermalReceiver.celsius;
		if (celsius >= this.fadeInTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedIn)
		{
			this.fadeInSounds.Play();
			this.fadeState = MagicRingCosmetic.FadeState.FadedIn;
		}
		else if (celsius <= this.fadeOutTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedOut)
		{
			this.fadeOutSounds.Play();
			this.fadeState = MagicRingCosmetic.FadeState.FadedOut;
		}
		this.emissiveAmount = Mathf.MoveTowards(this.emissiveAmount, (this.fadeState == MagicRingCosmetic.FadeState.FadedIn) ? 1f : 0f, Time.deltaTime / this.fadeTime);
		this.ringRenderer.GetPropertyBlock(this.materialPropertyBlock);
		this.materialPropertyBlock.SetColor(this.emissionColorShaderPropID, new Color(this.defaultEmissiveColor.r, this.defaultEmissiveColor.g, this.defaultEmissiveColor.b, this.emissiveAmount));
		this.ringRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x04000721 RID: 1825
	[Tooltip("The ring will fade in the emissive texture based on temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x04000722 RID: 1826
	public Renderer ringRenderer;

	// Token: 0x04000723 RID: 1827
	public float fadeInTemperatureThreshold = 200f;

	// Token: 0x04000724 RID: 1828
	public float fadeOutTemperatureThreshold = 190f;

	// Token: 0x04000725 RID: 1829
	public float fadeTime = 1.5f;

	// Token: 0x04000726 RID: 1830
	public SoundBankPlayer fadeInSounds;

	// Token: 0x04000727 RID: 1831
	public SoundBankPlayer fadeOutSounds;

	// Token: 0x04000728 RID: 1832
	private MagicRingCosmetic.FadeState fadeState;

	// Token: 0x04000729 RID: 1833
	private int emissionColorShaderPropID = Shader.PropertyToID("_EmissionColor");

	// Token: 0x0400072A RID: 1834
	private Color defaultEmissiveColor;

	// Token: 0x0400072B RID: 1835
	private float emissiveAmount;

	// Token: 0x0400072C RID: 1836
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x020000F2 RID: 242
	private enum FadeState
	{
		// Token: 0x0400072E RID: 1838
		FadedOut,
		// Token: 0x0400072F RID: 1839
		FadedIn
	}
}
