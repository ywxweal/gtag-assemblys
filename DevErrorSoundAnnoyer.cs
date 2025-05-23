using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B9 RID: 441
public class DevErrorSoundAnnoyer : MonoBehaviour
{
	// Token: 0x04000D1A RID: 3354
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x04000D1B RID: 3355
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000D1C RID: 3356
	[SerializeField]
	private Text errorUIText;

	// Token: 0x04000D1D RID: 3357
	[SerializeField]
	private Font errorFont;

	// Token: 0x04000D1E RID: 3358
	public string displayedText;
}
