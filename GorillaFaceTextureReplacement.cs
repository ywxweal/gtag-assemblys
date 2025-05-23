using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class GorillaFaceTextureReplacement : MonoBehaviour, ISpawnable
{
	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000966 RID: 2406 RVA: 0x000329AF File Offset: 0x00030BAF
	// (set) Token: 0x06000967 RID: 2407 RVA: 0x000329B7 File Offset: 0x00030BB7
	public bool IsSpawned { get; set; }

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000968 RID: 2408 RVA: 0x000329C0 File Offset: 0x00030BC0
	// (set) Token: 0x06000969 RID: 2409 RVA: 0x000329C8 File Offset: 0x00030BC8
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x0600096A RID: 2410 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnDespawn()
	{
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x000329D1 File Offset: 0x00030BD1
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x000329DA File Offset: 0x00030BDA
	private void OnEnable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().SetFaceMaterialReplacement(this.newFaceMaterial);
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x000329F2 File Offset: 0x00030BF2
	private void OnDisable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().ClearFaceMaterialReplacement();
	}

	// Token: 0x04000B4E RID: 2894
	[SerializeField]
	private Material newFaceMaterial;

	// Token: 0x04000B4F RID: 2895
	private VRRig myRig;
}
