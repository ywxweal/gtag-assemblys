using System;
using UnityEngine;

// Token: 0x0200092A RID: 2346
public interface IFXEffectContextObject
{
	// Token: 0x1700059A RID: 1434
	// (get) Token: 0x06003916 RID: 14614
	int[] PrefabPoolIds { get; }

	// Token: 0x1700059B RID: 1435
	// (get) Token: 0x06003917 RID: 14615
	Vector3 Positon { get; }

	// Token: 0x1700059C RID: 1436
	// (get) Token: 0x06003918 RID: 14616
	Quaternion Rotation { get; }

	// Token: 0x1700059D RID: 1437
	// (get) Token: 0x06003919 RID: 14617
	AudioSource SoundSource { get; }

	// Token: 0x1700059E RID: 1438
	// (get) Token: 0x0600391A RID: 14618
	AudioClip Sound { get; }

	// Token: 0x1700059F RID: 1439
	// (get) Token: 0x0600391B RID: 14619
	float Volume { get; }

	// Token: 0x0600391C RID: 14620
	void OnPlayVisualFX(int effectID, GameObject effect);

	// Token: 0x0600391D RID: 14621
	void OnPlaySoundFX(AudioSource audioSource);
}
