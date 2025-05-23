using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000BF RID: 191
public class SkyboxController : MonoBehaviour
{
	// Token: 0x060004CB RID: 1227 RVA: 0x0001BDE0 File Offset: 0x00019FE0
	private void Start()
	{
		if (this._dayNightManager.AsNull<BetterDayNightManager>() == null)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (this._dayNightManager.AsNull<BetterDayNightManager>() == null)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0001BE6E File Offset: 0x0001A06E
	private void Update()
	{
		if (!this.lastUpdate.HasElapsed(1f, true))
		{
			return;
		}
		this.UpdateTime();
		this.UpdateSky();
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0001BE90 File Offset: 0x0001A090
	private void OnValidate()
	{
		this.UpdateSky();
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0001BE98 File Offset: 0x0001A098
	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0001BED0 File Offset: 0x0001A0D0
	private void UpdateSky()
	{
		if (this.skyMaterials == null || this.skyMaterials.Length == 0)
		{
			return;
		}
		int num = this.skyMaterials.Length;
		float num2 = Mathf.Clamp(this._currentTime, 0f, 1f);
		float num3 = 1f / (float)num;
		int num4 = (int)(num2 / num3);
		float num5 = (num2 - (float)num4 * num3) / num3;
		this._currentSky = this.skyMaterials[num4];
		this._nextSky = this.skyMaterials[(num4 + 1) % num];
		this.skyFront.sharedMaterial = this._currentSky;
		this.skyBack.sharedMaterial = this._nextSky;
		if (this._currentSky.renderQueue != 3000)
		{
			this.SetFrontToTransparent();
		}
		if (this._nextSky.renderQueue == 3000)
		{
			this.SetBackToOpaque();
		}
		this._currentSky.SetFloat(SkyboxController._SkyAlpha, 1f - num5);
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0001BFB4 File Offset: 0x0001A1B4
	private void SetFrontToTransparent()
	{
		bool flag = false;
		bool flag2 = false;
		string text = "Transparent";
		int num = 3000;
		BlendMode blendMode = BlendMode.SrcAlpha;
		BlendMode blendMode2 = BlendMode.OneMinusSrcAlpha;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.OneMinusSrcAlpha;
		Material sharedMaterial = this.skyFront.sharedMaterial;
		sharedMaterial.SetFloat("_ZWrite", flag ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag);
		sharedMaterial.SetFloat("_AlphaToMask", flag2 ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", text);
		sharedMaterial.renderQueue = num;
		sharedMaterial.SetFloat("_SrcBlend", (float)blendMode);
		sharedMaterial.SetFloat("_DstBlend", (float)blendMode2);
		sharedMaterial.SetFloat("_SrcBlendAlpha", (float)blendMode3);
		sharedMaterial.SetFloat("_DstBlendAlpha", (float)blendMode4);
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0001C074 File Offset: 0x0001A274
	private void SetFrontToOpaque()
	{
		bool flag = false;
		bool flag2 = true;
		string text = "Opaque";
		int num = 2000;
		BlendMode blendMode = BlendMode.One;
		BlendMode blendMode2 = BlendMode.Zero;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.Zero;
		Material sharedMaterial = this.skyFront.sharedMaterial;
		sharedMaterial.SetFloat("_ZWrite", flag2 ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag2);
		sharedMaterial.SetFloat("_AlphaToMask", flag ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", text);
		sharedMaterial.renderQueue = num;
		sharedMaterial.SetFloat("_SrcBlend", (float)blendMode);
		sharedMaterial.SetFloat("_DstBlend", (float)blendMode2);
		sharedMaterial.SetFloat("_SrcBlendAlpha", (float)blendMode3);
		sharedMaterial.SetFloat("_DstBlendAlpha", (float)blendMode4);
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0001C134 File Offset: 0x0001A334
	private void SetBackToOpaque()
	{
		bool flag = false;
		bool flag2 = true;
		string text = "Opaque";
		int num = 2000;
		BlendMode blendMode = BlendMode.One;
		BlendMode blendMode2 = BlendMode.Zero;
		BlendMode blendMode3 = BlendMode.One;
		BlendMode blendMode4 = BlendMode.Zero;
		Material sharedMaterial = this.skyBack.sharedMaterial;
		sharedMaterial.SetFloat("_ZWrite", flag2 ? 1f : 0f);
		sharedMaterial.SetShaderPassEnabled("DepthOnly", flag2);
		sharedMaterial.SetFloat("_AlphaToMask", flag ? 1f : 0f);
		sharedMaterial.SetOverrideTag("RenderType", text);
		sharedMaterial.renderQueue = num;
		sharedMaterial.SetFloat("_SrcBlend", (float)blendMode);
		sharedMaterial.SetFloat("_DstBlend", (float)blendMode2);
		sharedMaterial.SetFloat("_SrcBlendAlpha", (float)blendMode3);
		sharedMaterial.SetFloat("_DstBlendAlpha", (float)blendMode4);
	}

	// Token: 0x04000598 RID: 1432
	public MeshRenderer skyFront;

	// Token: 0x04000599 RID: 1433
	public MeshRenderer skyBack;

	// Token: 0x0400059A RID: 1434
	public Material[] skyMaterials = new Material[0];

	// Token: 0x0400059B RID: 1435
	[Range(0f, 1f)]
	public float lerpValue;

	// Token: 0x0400059C RID: 1436
	[NonSerialized]
	private Material _currentSky;

	// Token: 0x0400059D RID: 1437
	[NonSerialized]
	private Material _nextSky;

	// Token: 0x0400059E RID: 1438
	private TimeSince lastUpdate = TimeSince.Now();

	// Token: 0x0400059F RID: 1439
	[Space]
	private BetterDayNightManager _dayNightManager;

	// Token: 0x040005A0 RID: 1440
	private double _currentSeconds = -1.0;

	// Token: 0x040005A1 RID: 1441
	private double _totalSecondsInRange = -1.0;

	// Token: 0x040005A2 RID: 1442
	private float _currentTime = -1f;

	// Token: 0x040005A3 RID: 1443
	private static ShaderHashId _SkyAlpha = "_SkyAlpha";
}
