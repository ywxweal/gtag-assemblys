using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000183 RID: 387
public class GorillaSkin : ScriptableObject
{
	// Token: 0x170000EA RID: 234
	// (get) Token: 0x0600099B RID: 2459 RVA: 0x000335D2 File Offset: 0x000317D2
	public Mesh bodyMesh
	{
		get
		{
			return this._bodyMesh;
		}
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x000335DC File Offset: 0x000317DC
	public static GorillaSkin CopyWithInstancedMaterials(GorillaSkin basis)
	{
		GorillaSkin gorillaSkin = ScriptableObject.CreateInstance<GorillaSkin>();
		gorillaSkin._faceMaterial = new Material(basis._faceMaterial);
		gorillaSkin._chestMaterial = new Material(basis._chestMaterial);
		gorillaSkin._bodyMaterial = new Material(basis._bodyMaterial);
		gorillaSkin._scoreboardMaterial = new Material(basis._scoreboardMaterial);
		gorillaSkin._bodyMesh = basis.bodyMesh;
		return gorillaSkin;
	}

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x0600099D RID: 2461 RVA: 0x0003363E File Offset: 0x0003183E
	public Material faceMaterial
	{
		get
		{
			return this._faceMaterial;
		}
	}

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x0600099E RID: 2462 RVA: 0x00033646 File Offset: 0x00031846
	public Material bodyMaterial
	{
		get
		{
			return this._bodyMaterial;
		}
	}

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x0600099F RID: 2463 RVA: 0x0003364E File Offset: 0x0003184E
	public Material chestMaterial
	{
		get
		{
			return this._chestMaterial;
		}
	}

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x060009A0 RID: 2464 RVA: 0x00033656 File Offset: 0x00031856
	public Material scoreboardMaterial
	{
		get
		{
			return this._scoreboardMaterial;
		}
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x00033660 File Offset: 0x00031860
	public static void ShowActiveSkin(VRRig rig)
	{
		bool flag;
		GorillaSkin activeSkin = GorillaSkin.GetActiveSkin(rig, out flag);
		GorillaSkin.ShowSkin(rig, activeSkin, flag);
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00033680 File Offset: 0x00031880
	public void ApplySkinToMannequin(GameObject mannequin)
	{
		SkinnedMeshRenderer skinnedMeshRenderer;
		if (mannequin.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
		{
			Material[] array = new Material[] { this.bodyMaterial, this.chestMaterial, this.faceMaterial };
			skinnedMeshRenderer.sharedMaterials = array;
			return;
		}
		MeshRenderer meshRenderer;
		if (mannequin.TryGetComponent<MeshRenderer>(out meshRenderer))
		{
			Material[] array2 = new Material[] { this.bodyMaterial, this.chestMaterial, this.faceMaterial };
			meshRenderer.sharedMaterials = array2;
		}
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x000336F4 File Offset: 0x000318F4
	public static GorillaSkin GetActiveSkin(VRRig rig, out bool useDefaultBodySkin)
	{
		if (rig.CurrentModeSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.CurrentModeSkin;
		}
		if (rig.TemporaryEffectSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.TemporaryEffectSkin;
		}
		if (rig.CurrentCosmeticSkin.IsNotNull())
		{
			useDefaultBodySkin = false;
			return rig.CurrentCosmeticSkin;
		}
		useDefaultBodySkin = true;
		return rig.defaultSkin;
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00033750 File Offset: 0x00031950
	public static void ShowSkin(VRRig rig, GorillaSkin skin, bool useDefaultBodySkin = false)
	{
		if (skin.bodyMesh != null)
		{
			rig.mainSkin.sharedMesh = skin.bodyMesh;
		}
		if (useDefaultBodySkin)
		{
			rig.materialsToChangeTo[0] = rig.myDefaultSkinMaterialInstance;
		}
		else
		{
			rig.materialsToChangeTo[0] = skin.bodyMaterial;
		}
		rig.bodyRenderer.SetSkinMaterials(rig.materialsToChangeTo[rig.setMatIndex], skin.chestMaterial);
		rig.scoreboardMaterial = skin.scoreboardMaterial;
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x000337C8 File Offset: 0x000319C8
	public static void ApplyToRig(VRRig rig, GorillaSkin skin, GorillaSkin.SkinType type)
	{
		bool flag;
		GorillaSkin activeSkin = GorillaSkin.GetActiveSkin(rig, out flag);
		switch (type)
		{
		case GorillaSkin.SkinType.cosmetic:
			rig.CurrentCosmeticSkin = skin;
			break;
		case GorillaSkin.SkinType.gameMode:
			rig.CurrentModeSkin = skin;
			break;
		case GorillaSkin.SkinType.temporaryEffect:
			rig.TemporaryEffectSkin = skin;
			break;
		default:
			Debug.LogError("Unknown skin slot");
			break;
		}
		bool flag2;
		GorillaSkin activeSkin2 = GorillaSkin.GetActiveSkin(rig, out flag2);
		if (activeSkin != activeSkin2)
		{
			GorillaSkin.ShowSkin(rig, activeSkin2, flag2);
		}
	}

	// Token: 0x04000BA6 RID: 2982
	[FormerlySerializedAs("faceMaterial")]
	[Space]
	[SerializeField]
	private Material _faceMaterial;

	// Token: 0x04000BA7 RID: 2983
	[FormerlySerializedAs("chestMaterial")]
	[FormerlySerializedAs("chestEarsMaterial")]
	[SerializeField]
	private Material _chestMaterial;

	// Token: 0x04000BA8 RID: 2984
	[FormerlySerializedAs("bodyMaterial")]
	[SerializeField]
	private Material _bodyMaterial;

	// Token: 0x04000BA9 RID: 2985
	[SerializeField]
	private Material _scoreboardMaterial;

	// Token: 0x04000BAA RID: 2986
	[Space]
	[SerializeField]
	private Mesh _bodyMesh;

	// Token: 0x04000BAB RID: 2987
	[Space]
	[NonSerialized]
	private Material _bodyRuntime;

	// Token: 0x04000BAC RID: 2988
	[NonSerialized]
	private Material _chestRuntime;

	// Token: 0x04000BAD RID: 2989
	[NonSerialized]
	private Material _faceRuntime;

	// Token: 0x04000BAE RID: 2990
	[NonSerialized]
	private Material _scoreRuntime;

	// Token: 0x02000184 RID: 388
	public enum SkinType
	{
		// Token: 0x04000BB0 RID: 2992
		cosmetic,
		// Token: 0x04000BB1 RID: 2993
		gameMode,
		// Token: 0x04000BB2 RID: 2994
		temporaryEffect
	}
}
