using System;
using UnityEngine;

// Token: 0x0200096D RID: 2413
[RequireComponent(typeof(AudioSource))]
public class AudioLooper : MonoBehaviour
{
	// Token: 0x06003A32 RID: 14898 RVA: 0x00116EEB File Offset: 0x001150EB
	protected virtual void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06003A33 RID: 14899 RVA: 0x00116EFC File Offset: 0x001150FC
	private void Update()
	{
		if (!this.audioSource.isPlaying)
		{
			if (this.audioSource.clip == this.loopClip && this.interjectionClips.Length != 0 && Random.value < this.interjectionLikelyhood)
			{
				this.audioSource.clip = this.interjectionClips[Random.Range(0, this.interjectionClips.Length)];
			}
			else
			{
				this.audioSource.clip = this.loopClip;
			}
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x04003F36 RID: 16182
	private AudioSource audioSource;

	// Token: 0x04003F37 RID: 16183
	[SerializeField]
	private AudioClip loopClip;

	// Token: 0x04003F38 RID: 16184
	[SerializeField]
	private AudioClip[] interjectionClips;

	// Token: 0x04003F39 RID: 16185
	[SerializeField]
	private float interjectionLikelyhood = 0.5f;
}
