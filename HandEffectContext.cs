using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020003DC RID: 988
[Serializable]
internal class HandEffectContext : IFXEffectContextObject
{
	// Token: 0x1700029D RID: 669
	// (get) Token: 0x060017C9 RID: 6089 RVA: 0x000741A3 File Offset: 0x000723A3
	public int[] PrefabPoolIds
	{
		get
		{
			return this.prefabHashes;
		}
	}

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x060017CA RID: 6090 RVA: 0x000741AB File Offset: 0x000723AB
	public Vector3 Positon
	{
		get
		{
			return this.position;
		}
	}

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x060017CB RID: 6091 RVA: 0x000741B3 File Offset: 0x000723B3
	public Quaternion Rotation
	{
		get
		{
			return this.rotation;
		}
	}

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x060017CC RID: 6092 RVA: 0x000741BB File Offset: 0x000723BB
	public AudioSource SoundSource
	{
		get
		{
			return this.handSoundSource;
		}
	}

	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x060017CD RID: 6093 RVA: 0x000741C3 File Offset: 0x000723C3
	public AudioClip Sound
	{
		get
		{
			return this.soundFX;
		}
	}

	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x060017CE RID: 6094 RVA: 0x000741CB File Offset: 0x000723CB
	public float Volume
	{
		get
		{
			return this.clipVolume;
		}
	}

	// Token: 0x060017CF RID: 6095 RVA: 0x000741D4 File Offset: 0x000723D4
	public void OnPlayVisualFX(int fxID, GameObject fx)
	{
		FXModifier fxmodifier;
		if (fx.TryGetComponent<FXModifier>(out fxmodifier))
		{
			fxmodifier.UpdateScale(this.handSpeed * ((fxID == GorillaAmbushManager.HandEffectHash) ? GorillaAmbushManager.HandFXScaleModifier : 1f));
		}
	}

	// Token: 0x060017D0 RID: 6096 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPlaySoundFX(AudioSource audioSource)
	{
	}

	// Token: 0x04001AA1 RID: 6817
	internal const int MATERIAL_PREFAB = 0;

	// Token: 0x04001AA2 RID: 6818
	internal const int GAMEMODE_PREFAB = 1;

	// Token: 0x04001AA3 RID: 6819
	internal const int COSMETIC_PREFAB = 2;

	// Token: 0x04001AA4 RID: 6820
	internal int[] prefabHashes = new int[] { -1, -1, -1 };

	// Token: 0x04001AA5 RID: 6821
	internal Vector3 position;

	// Token: 0x04001AA6 RID: 6822
	internal Quaternion rotation;

	// Token: 0x04001AA7 RID: 6823
	[SerializeField]
	internal AudioSource handSoundSource;

	// Token: 0x04001AA8 RID: 6824
	internal AudioClip soundFX;

	// Token: 0x04001AA9 RID: 6825
	internal float clipVolume;

	// Token: 0x04001AAA RID: 6826
	internal float handSpeed;
}
