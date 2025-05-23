using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using TagEffects;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class TagEffectsPackToggle : MonoBehaviour, ISpawnable
{
	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000A3B RID: 2619 RVA: 0x00035B8D File Offset: 0x00033D8D
	// (set) Token: 0x06000A3C RID: 2620 RVA: 0x00035B95 File Offset: 0x00033D95
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000A3D RID: 2621 RVA: 0x00035B9E File Offset: 0x00033D9E
	// (set) Token: 0x06000A3E RID: 2622 RVA: 0x00035BA6 File Offset: 0x00033DA6
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06000A3F RID: 2623 RVA: 0x00035BAF File Offset: 0x00033DAF
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x00035BB8 File Offset: 0x00033DB8
	private void OnEnable()
	{
		this.Apply();
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x00035BC0 File Offset: 0x00033DC0
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x00035BD0 File Offset: 0x00033DD0
	public void Apply()
	{
		this._rig.CosmeticEffectPack = this.tagEffectPack;
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x00035BE3 File Offset: 0x00033DE3
	public void Remove()
	{
		this._rig.CosmeticEffectPack = null;
	}

	// Token: 0x04000C62 RID: 3170
	private VRRig _rig;

	// Token: 0x04000C63 RID: 3171
	[SerializeField]
	private TagEffectPack tagEffectPack;
}
