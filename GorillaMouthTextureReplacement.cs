using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class GorillaMouthTextureReplacement : MonoBehaviour, ISpawnable
{
	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000992 RID: 2450 RVA: 0x0003357D File Offset: 0x0003177D
	// (set) Token: 0x06000993 RID: 2451 RVA: 0x00033585 File Offset: 0x00031785
	public bool IsSpawned { get; set; }

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000994 RID: 2452 RVA: 0x0003358E File Offset: 0x0003178E
	// (set) Token: 0x06000995 RID: 2453 RVA: 0x00033596 File Offset: 0x00031796
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000996 RID: 2454 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnDespawn()
	{
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0003359F File Offset: 0x0003179F
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x000335A8 File Offset: 0x000317A8
	private void OnEnable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().SetMouthTextureReplacement(this.newMouthAtlas);
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x000335C0 File Offset: 0x000317C0
	private void OnDisable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().ClearMouthTextureReplacement();
	}

	// Token: 0x04000BA2 RID: 2978
	[SerializeField]
	private Texture2D newMouthAtlas;

	// Token: 0x04000BA3 RID: 2979
	private VRRig myRig;
}
