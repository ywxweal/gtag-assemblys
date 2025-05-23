using System;
using UnityEngine;

// Token: 0x02000475 RID: 1141
public class XRaySkeleton : SyncToPlayerColor
{
	// Token: 0x06001C16 RID: 7190 RVA: 0x00089F7C File Offset: 0x0008817C
	protected override void Awake()
	{
		base.Awake();
		this.target = this.renderer.material;
		Material[] materialsToChangeTo = this.rig.materialsToChangeTo;
		this.tagMaterials = new Material[materialsToChangeTo.Length];
		this.tagMaterials[0] = new Material(this.target);
		for (int i = 1; i < materialsToChangeTo.Length; i++)
		{
			Material material = new Material(materialsToChangeTo[i]);
			this.tagMaterials[i] = material;
		}
	}

	// Token: 0x06001C17 RID: 7191 RVA: 0x00089FED File Offset: 0x000881ED
	public void SetMaterialIndex(int index)
	{
		this.renderer.sharedMaterial = this.tagMaterials[index];
		this._lastMatIndex = index;
	}

	// Token: 0x06001C18 RID: 7192 RVA: 0x0008A009 File Offset: 0x00088209
	private void Setup()
	{
		this.colorPropertiesToSync = new ShaderHashId[]
		{
			XRaySkeleton._BaseColor,
			XRaySkeleton._EmissionColor
		};
	}

	// Token: 0x06001C19 RID: 7193 RVA: 0x0008A030 File Offset: 0x00088230
	public override void UpdateColor(Color color)
	{
		if (this._lastMatIndex != 0)
		{
			return;
		}
		Material material = this.tagMaterials[0];
		float num;
		float num2;
		float num3;
		Color.RGBToHSV(color, out num, out num2, out num3);
		Color color2 = Color.HSVToRGB(num, num2, Mathf.Clamp(num3, this.baseValueMinMax.x, this.baseValueMinMax.y));
		material.SetColor(XRaySkeleton._BaseColor, color2);
		float num4;
		float num5;
		float num6;
		Color.RGBToHSV(color, out num4, out num5, out num6);
		Color color3 = Color.HSVToRGB(num4, 0.82f, 0.9f, true);
		color3 = new Color(color3.r * 1.4f, color3.g * 1.4f, color3.b * 1.4f);
		material.SetColor(XRaySkeleton._EmissionColor, ColorUtils.ComposeHDR(new Color32(36, 191, 136, byte.MaxValue), 2f));
		this.renderer.sharedMaterial = material;
	}

	// Token: 0x04001F34 RID: 7988
	public SkinnedMeshRenderer renderer;

	// Token: 0x04001F35 RID: 7989
	public Vector2 baseValueMinMax = new Vector2(0.69f, 1f);

	// Token: 0x04001F36 RID: 7990
	public Material[] tagMaterials = new Material[0];

	// Token: 0x04001F37 RID: 7991
	private int _lastMatIndex;

	// Token: 0x04001F38 RID: 7992
	private static readonly ShaderHashId _BaseColor = "_BaseColor";

	// Token: 0x04001F39 RID: 7993
	private static readonly ShaderHashId _EmissionColor = "_EmissionColor";
}
