using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005F2 RID: 1522
public class GorillaBodyRenderer : MonoBehaviour
{
	// Token: 0x17000397 RID: 919
	// (get) Token: 0x06002595 RID: 9621 RVA: 0x000BB33A File Offset: 0x000B953A
	// (set) Token: 0x06002596 RID: 9622 RVA: 0x000BB342 File Offset: 0x000B9542
	public GorillaBodyType bodyType
	{
		get
		{
			return this._bodyType;
		}
		set
		{
			this.SetBodyType(value);
		}
	}

	// Token: 0x17000398 RID: 920
	// (get) Token: 0x06002597 RID: 9623 RVA: 0x000BB34B File Offset: 0x000B954B
	public bool renderFace
	{
		get
		{
			return this._renderFace;
		}
	}

	// Token: 0x06002598 RID: 9624 RVA: 0x000BB354 File Offset: 0x000B9554
	public SkinnedMeshRenderer GetBody(GorillaBodyType type)
	{
		if (type < GorillaBodyType.Default || type >= (GorillaBodyType)this._renderersCache.Length)
		{
			return null;
		}
		return this._renderersCache[(int)type];
	}

	// Token: 0x17000399 RID: 921
	// (get) Token: 0x06002599 RID: 9625 RVA: 0x000BB37C File Offset: 0x000B957C
	public SkinnedMeshRenderer ActiveBody
	{
		get
		{
			return this.GetBody(this._bodyType);
		}
	}

	// Token: 0x0600259A RID: 9626 RVA: 0x000BB38C File Offset: 0x000B958C
	public static void SetAllSkeletons(bool allSkeletons)
	{
		GorillaBodyRenderer.oopsAllSkeletons = allSkeletons;
		GorillaTagger.Instance.offlineVRRig.bodyRenderer.Refresh();
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			vrrig.bodyRenderer.Refresh();
		}
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x000BB404 File Offset: 0x000B9604
	public void SetGameModeBodyType(GorillaBodyType bodyType)
	{
		this.gameModeBodyType = bodyType;
		this.Refresh();
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x000BB413 File Offset: 0x000B9613
	public void SetCosmeticBodyType(GorillaBodyType bodyType)
	{
		this.cosmeticBodyType = bodyType;
		this.Refresh();
	}

	// Token: 0x0600259D RID: 9629 RVA: 0x000BB422 File Offset: 0x000B9622
	public void SetDefaults()
	{
		this.gameModeBodyType = GorillaBodyType.Default;
		this.cosmeticBodyType = GorillaBodyType.Default;
		this.Refresh();
	}

	// Token: 0x0600259E RID: 9630 RVA: 0x000BB438 File Offset: 0x000B9638
	private void Refresh()
	{
		GorillaBodyType gorillaBodyType;
		if (GorillaBodyRenderer.oopsAllSkeletons)
		{
			gorillaBodyType = GorillaBodyType.Skeleton;
		}
		else if (this.gameModeBodyType != GorillaBodyType.Default)
		{
			gorillaBodyType = this.gameModeBodyType;
		}
		else
		{
			gorillaBodyType = this.cosmeticBodyType;
		}
		this.SetBodyType(gorillaBodyType);
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x000BB470 File Offset: 0x000B9670
	public void SetMaterialIndex(int materialIndex)
	{
		this.bodyDefault.sharedMaterial = this.rig.materialsToChangeTo[materialIndex];
		this.bodyNoHead.sharedMaterial = this.bodyDefault.sharedMaterial;
		this.rig.skeleton.SetMaterialIndex(materialIndex);
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x000BB4BC File Offset: 0x000B96BC
	public void SetSkinMaterials(Material bodyMat, Material chestMat)
	{
		Material[] sharedMaterials = this.bodyDefault.sharedMaterials;
		sharedMaterials[0] = bodyMat;
		sharedMaterials[1] = chestMat;
		this.bodyDefault.sharedMaterials = sharedMaterials;
		this.bodyNoHead.sharedMaterials = sharedMaterials;
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x000BB4F5 File Offset: 0x000B96F5
	public void SetupAsLocalPlayerBody()
	{
		this.faceRenderer.gameObject.layer = 22;
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x000BB50C File Offset: 0x000B970C
	private void SetBodyType(GorillaBodyType type)
	{
		if (this._bodyType == type)
		{
			return;
		}
		this.SetBodyEnabled(this._bodyType, false);
		this._bodyType = type;
		this.SetBodyEnabled(type, true);
		this._renderFace = this._bodyType != GorillaBodyType.NoHead && this._bodyType != GorillaBodyType.Skeleton && this._bodyType != GorillaBodyType.Invisible;
		if (this.faceRenderer != null)
		{
			this.faceRenderer.enabled = this._renderFace;
		}
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x000BB588 File Offset: 0x000B9788
	private void SetBodyEnabled(GorillaBodyType bodyType, bool enabled)
	{
		SkinnedMeshRenderer body = this.GetBody(bodyType);
		if (body == null)
		{
			return;
		}
		body.enabled = enabled;
		Transform[] bones = body.bones;
		for (int i = 0; i < bones.Length; i++)
		{
			bones[i].gameObject.SetActive(enabled);
		}
	}

	// Token: 0x060025A4 RID: 9636 RVA: 0x000BB5D1 File Offset: 0x000B97D1
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x060025A5 RID: 9637 RVA: 0x000BB5DC File Offset: 0x000B97DC
	private void Setup()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this._renderersCache = new SkinnedMeshRenderer[EnumData<GorillaBodyType>.Shared.Values.Length];
		this._renderersCache[0] = this.bodyDefault;
		this._renderersCache[1] = this.bodyNoHead;
		this._renderersCache[2] = this.bodySkeleton;
		this.SetBodyEnabled(GorillaBodyType.Default, true);
		this.SetBodyEnabled(GorillaBodyType.NoHead, false);
		this.SetBodyEnabled(GorillaBodyType.Skeleton, false);
		this.bodyDefault.GetSharedMaterials(this._cachedDefaultMats);
	}

	// Token: 0x04002A15 RID: 10773
	[SerializeField]
	private GorillaBodyType _bodyType;

	// Token: 0x04002A16 RID: 10774
	[SerializeField]
	private bool _renderFace = true;

	// Token: 0x04002A17 RID: 10775
	public MeshRenderer faceRenderer;

	// Token: 0x04002A18 RID: 10776
	public SkinnedMeshRenderer bodyDefault;

	// Token: 0x04002A19 RID: 10777
	public SkinnedMeshRenderer bodyNoHead;

	// Token: 0x04002A1A RID: 10778
	public SkinnedMeshRenderer bodySkeleton;

	// Token: 0x04002A1B RID: 10779
	public SkinnedMeshRenderer bodyCosmetic;

	// Token: 0x04002A1C RID: 10780
	private static bool oopsAllSkeletons;

	// Token: 0x04002A1D RID: 10781
	private GorillaBodyType gameModeBodyType;

	// Token: 0x04002A1E RID: 10782
	private GorillaBodyType cosmeticBodyType;

	// Token: 0x04002A1F RID: 10783
	[Space]
	[NonSerialized]
	private SkinnedMeshRenderer[] _renderersCache = new SkinnedMeshRenderer[0];

	// Token: 0x04002A20 RID: 10784
	[NonSerialized]
	private List<Material> _cachedDefaultMats = new List<Material>(2);

	// Token: 0x04002A21 RID: 10785
	private static readonly List<Material> gEmptyDefaultMats = new List<Material>();

	// Token: 0x04002A22 RID: 10786
	[Space]
	public VRRig rig;
}
