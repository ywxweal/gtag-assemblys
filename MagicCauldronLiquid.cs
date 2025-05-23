using System;
using UnityEngine;

// Token: 0x0200067C RID: 1660
public class MagicCauldronLiquid : MonoBehaviour
{
	// Token: 0x06002976 RID: 10614 RVA: 0x000CDE7B File Offset: 0x000CC07B
	private void Test()
	{
		this._animProgress = 0f;
		this._animating = true;
		base.enabled = true;
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x000CDE96 File Offset: 0x000CC096
	public void AnimateColorFromTo(Color a, Color b, float length = 1f)
	{
		this._colorStart = a;
		this._colorEnd = b;
		this._animProgress = 0f;
		this._animating = true;
		this.animLength = length;
		base.enabled = true;
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x000CDEC6 File Offset: 0x000CC0C6
	private void ApplyColor(Color color)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetColor("_BaseColor", color);
		this._applyMaterial.Apply();
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x000CDEF4 File Offset: 0x000CC0F4
	private void ApplyWaveParams(float amplitude, float frequency, float scale, float rotation)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetFloat("_WaveAmplitude", amplitude);
		this._applyMaterial.SetFloat("_WaveFrequency", frequency);
		this._applyMaterial.SetFloat("_WaveScale", scale);
		this._applyMaterial.Apply();
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x000CDF4D File Offset: 0x000CC14D
	private void OnEnable()
	{
		if (this._applyMaterial)
		{
			this._applyMaterial.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x000CDF68 File Offset: 0x000CC168
	private void OnDisable()
	{
		this._animating = false;
		this._animProgress = 0f;
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x000CDF7C File Offset: 0x000CC17C
	private void Update()
	{
		if (!this._animating)
		{
			return;
		}
		float num = this._animationCurve.Evaluate(this._animProgress / this.animLength);
		float num2 = this._waveCurve.Evaluate(this._animProgress / this.animLength);
		if (num >= 1f)
		{
			this.ApplyColor(this._colorEnd);
			this._animating = false;
			base.enabled = false;
			return;
		}
		Color color = Color.Lerp(this._colorStart, this._colorEnd, num);
		Mathf.Lerp(this.waveNormal.frequency, this.waveAnimating.frequency, num2);
		Mathf.Lerp(this.waveNormal.amplitude, this.waveAnimating.amplitude, num2);
		Mathf.Lerp(this.waveNormal.scale, this.waveAnimating.scale, num2);
		Mathf.Lerp(this.waveNormal.rotation, this.waveAnimating.rotation, num2);
		this.ApplyColor(color);
		this._animProgress += Time.deltaTime;
	}

	// Token: 0x04002E80 RID: 11904
	[SerializeField]
	private ApplyMaterialProperty _applyMaterial;

	// Token: 0x04002E81 RID: 11905
	[SerializeField]
	private Color _colorStart;

	// Token: 0x04002E82 RID: 11906
	[SerializeField]
	private Color _colorEnd;

	// Token: 0x04002E83 RID: 11907
	[SerializeField]
	private bool _animating;

	// Token: 0x04002E84 RID: 11908
	[SerializeField]
	private float _animProgress;

	// Token: 0x04002E85 RID: 11909
	[SerializeField]
	private AnimationCurve _animationCurve = AnimationCurves.EaseOutCubic;

	// Token: 0x04002E86 RID: 11910
	[SerializeField]
	private AnimationCurve _waveCurve = AnimationCurves.EaseInElastic;

	// Token: 0x04002E87 RID: 11911
	public float animLength = 1f;

	// Token: 0x04002E88 RID: 11912
	public MagicCauldronLiquid.WaveParams waveNormal;

	// Token: 0x04002E89 RID: 11913
	public MagicCauldronLiquid.WaveParams waveAnimating;

	// Token: 0x0200067D RID: 1661
	[Serializable]
	public struct WaveParams
	{
		// Token: 0x04002E8A RID: 11914
		public float amplitude;

		// Token: 0x04002E8B RID: 11915
		public float frequency;

		// Token: 0x04002E8C RID: 11916
		public float scale;

		// Token: 0x04002E8D RID: 11917
		public float rotation;
	}
}
