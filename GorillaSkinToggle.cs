using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000187 RID: 391
public class GorillaSkinToggle : MonoBehaviour, ISpawnable
{
	// Token: 0x170000EF RID: 239
	// (get) Token: 0x060009A8 RID: 2472 RVA: 0x00033845 File Offset: 0x00031A45
	public bool applied
	{
		get
		{
			return this._applied;
		}
	}

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x060009A9 RID: 2473 RVA: 0x0003384D File Offset: 0x00031A4D
	// (set) Token: 0x060009AA RID: 2474 RVA: 0x00033855 File Offset: 0x00031A55
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x060009AB RID: 2475 RVA: 0x0003385E File Offset: 0x00031A5E
	// (set) Token: 0x060009AC RID: 2476 RVA: 0x00033866 File Offset: 0x00031A66
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060009AD RID: 2477 RVA: 0x00033870 File Offset: 0x00031A70
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this._rig = base.GetComponentInParent<VRRig>(true);
		if (this.coloringRules.Length != 0)
		{
			this._activeSkin = GorillaSkin.CopyWithInstancedMaterials(this._skin);
			for (int i = 0; i < this.coloringRules.Length; i++)
			{
				this.coloringRules[i].Init();
			}
			return;
		}
		this._activeSkin = this._skin;
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060009AF RID: 2479 RVA: 0x000338D8 File Offset: 0x00031AD8
	private void OnPlayerColorChanged(Color playerColor)
	{
		foreach (GorillaSkinToggle.ColoringRule coloringRule in this.coloringRules)
		{
			coloringRule.Apply(this._activeSkin, playerColor);
		}
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x00033910 File Offset: 0x00031B10
	private void OnEnable()
	{
		if (this.coloringRules.Length != 0)
		{
			this._rig.OnColorChanged += this.OnPlayerColorChanged;
			this.OnPlayerColorChanged(this._rig.playerColor);
		}
		this.Apply();
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x00033949 File Offset: 0x00031B49
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
		if (this.coloringRules.Length != 0)
		{
			this._rig.OnColorChanged -= this.OnPlayerColorChanged;
		}
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x00033979 File Offset: 0x00031B79
	public void Apply()
	{
		GorillaSkin.ApplyToRig(this._rig, this._activeSkin, GorillaSkin.SkinType.cosmetic);
		this._applied = true;
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x00033994 File Offset: 0x00031B94
	public void ApplyToMannequin(GameObject mannequin)
	{
		if (this._skin.IsNull())
		{
			Debug.LogError("No skin set on GorillaSkinToggle");
			return;
		}
		if (mannequin.IsNull())
		{
			Debug.LogError("No mannequin set on GorillaSkinToggle");
			return;
		}
		this._skin.ApplySkinToMannequin(mannequin);
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x000339D0 File Offset: 0x00031BD0
	public void Remove()
	{
		GorillaSkin.ApplyToRig(this._rig, null, GorillaSkin.SkinType.cosmetic);
		float @float = PlayerPrefs.GetFloat("redValue", 0f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
		this._applied = false;
	}

	// Token: 0x04000BB9 RID: 3001
	private VRRig _rig;

	// Token: 0x04000BBA RID: 3002
	[SerializeField]
	private GorillaSkin _skin;

	// Token: 0x04000BBB RID: 3003
	private GorillaSkin _activeSkin;

	// Token: 0x04000BBC RID: 3004
	[SerializeField]
	private GorillaSkinToggle.ColoringRule[] coloringRules;

	// Token: 0x04000BBD RID: 3005
	[Space]
	[SerializeField]
	private bool _applied;

	// Token: 0x02000188 RID: 392
	[Serializable]
	private struct ColoringRule
	{
		// Token: 0x060009B6 RID: 2486 RVA: 0x00033A2E File Offset: 0x00031C2E
		public void Init()
		{
			if (this.shaderColorProperty == "")
			{
				this.shaderColorProperty = "_BaseColor";
			}
			this.shaderHashId = new ShaderHashId(this.shaderColorProperty);
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x00033A60 File Offset: 0x00031C60
		public void Apply(GorillaSkin skin, Color color)
		{
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Body))
			{
				skin.bodyMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Face))
			{
				skin.faceMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Chest))
			{
				skin.chestMaterial.SetColor(this.shaderHashId, color);
			}
			if (this.colorMaterials.HasFlag(GorillaSkinMaterials.Scoreboard))
			{
				skin.scoreboardMaterial.SetColor(this.shaderHashId, color);
			}
		}

		// Token: 0x04000BC0 RID: 3008
		public GorillaSkinMaterials colorMaterials;

		// Token: 0x04000BC1 RID: 3009
		public string shaderColorProperty;

		// Token: 0x04000BC2 RID: 3010
		private ShaderHashId shaderHashId;
	}
}
