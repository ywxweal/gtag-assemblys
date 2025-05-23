using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200024E RID: 590
public class HandEffectsOverrideCosmetic : MonoBehaviour, ISpawnable
{
	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000D64 RID: 3428 RVA: 0x00045EDC File Offset: 0x000440DC
	// (set) Token: 0x06000D65 RID: 3429 RVA: 0x00045EE4 File Offset: 0x000440E4
	public bool IsSpawned { get; set; }

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000D66 RID: 3430 RVA: 0x00045EED File Offset: 0x000440ED
	// (set) Token: 0x06000D67 RID: 3431 RVA: 0x00045EF5 File Offset: 0x000440F5
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06000D68 RID: 3432 RVA: 0x00045EFE File Offset: 0x000440FE
	public void OnSpawn(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnDespawn()
	{
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x00045F07 File Offset: 0x00044107
	public void OnEnable()
	{
		if (!this.isLeftHand)
		{
			this._rig.CosmeticHandEffectsOverride_Right.Add(this);
			return;
		}
		this._rig.CosmeticHandEffectsOverride_Left.Add(this);
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x00045F34 File Offset: 0x00044134
	public void OnDisable()
	{
		if (!this.isLeftHand)
		{
			this._rig.CosmeticHandEffectsOverride_Right.Remove(this);
			return;
		}
		this._rig.CosmeticHandEffectsOverride_Left.Remove(this);
	}

	// Token: 0x040010FC RID: 4348
	public HandEffectsOverrideCosmetic.HandEffectType handEffectType;

	// Token: 0x040010FD RID: 4349
	public bool isLeftHand;

	// Token: 0x040010FE RID: 4350
	public HandEffectsOverrideCosmetic.EffectsOverride firstPerson;

	// Token: 0x040010FF RID: 4351
	public HandEffectsOverrideCosmetic.EffectsOverride thirdPerson;

	// Token: 0x04001100 RID: 4352
	private VRRig _rig;

	// Token: 0x0200024F RID: 591
	[Serializable]
	public class EffectsOverride
	{
		// Token: 0x04001103 RID: 4355
		public GameObject effectVFX;

		// Token: 0x04001104 RID: 4356
		public bool playHaptics;

		// Token: 0x04001105 RID: 4357
		public float hapticStrength = 0.5f;

		// Token: 0x04001106 RID: 4358
		public float hapticDuration = 0.5f;

		// Token: 0x04001107 RID: 4359
		public bool parentEffect;
	}

	// Token: 0x02000250 RID: 592
	public enum HandEffectType
	{
		// Token: 0x04001109 RID: 4361
		None,
		// Token: 0x0400110A RID: 4362
		FistBump,
		// Token: 0x0400110B RID: 4363
		HighFive
	}
}
