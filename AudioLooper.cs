using System;
using UnityEngine;

// Token: 0x0200096D RID: 2413
[RequireComponent(typeof(AudioSource))]
public class AudioLooper : MonoBehaviour
{
	// Token: 0x06003A33 RID: 14899 RVA: 0x00116FC3 File Offset: 0x001151C3
	protected virtual void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06003A34 RID: 14900 RVA: 0x00116FD4 File Offset: 0x001151D4
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

	// Token: 0x04003F37 RID: 16183
	private AudioSource audioSource;

	// Token: 0x04003F38 RID: 16184
	[SerializeField]
	private AudioClip loopClip;

	// Token: 0x04003F39 RID: 16185
	[SerializeField]
	private AudioClip[] interjectionClips;

	// Token: 0x04003F3A RID: 16186
	[SerializeField]
	private float interjectionLikelyhood = 0.5f;
}
