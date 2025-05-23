using System;
using UnityEngine;

// Token: 0x020009BB RID: 2491
[CreateAssetMenu(menuName = "Gorilla Tag/SoundBankSO")]
public class SoundBankSO : ScriptableObject
{
	// Token: 0x04003FFD RID: 16381
	public AudioClip[] sounds;

	// Token: 0x04003FFE RID: 16382
	public Vector2 volumeRange = new Vector2(0.5f, 0.5f);

	// Token: 0x04003FFF RID: 16383
	public Vector2 pitchRange = new Vector2(1f, 1f);
}
