using System;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class AudioFader : MonoBehaviour
{
	// Token: 0x060008F1 RID: 2289 RVA: 0x000303B7 File Offset: 0x0002E5B7
	private void Start()
	{
		this.fadeInSpeed = this.maxVolume / this.fadeInDuration;
		this.fadeOutSpeed = this.maxVolume / this.fadeOutDuration;
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x000303E0 File Offset: 0x0002E5E0
	public void FadeIn()
	{
		this.targetVolume = this.maxVolume;
		if (this.fadeInDuration > 0f)
		{
			base.enabled = true;
			this.currentFadeSpeed = this.fadeInSpeed;
		}
		else
		{
			this.currentVolume = this.maxVolume;
		}
		this.audioToFade.volume = this.currentVolume;
		if (!this.audioToFade.isPlaying)
		{
			this.audioToFade.Play();
		}
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00030450 File Offset: 0x0002E650
	public void FadeOut()
	{
		this.targetVolume = 0f;
		if (this.fadeOutDuration > 0f)
		{
			base.enabled = true;
			this.currentFadeSpeed = this.fadeOutSpeed;
		}
		else
		{
			this.currentVolume = 0f;
			if (this.audioToFade.isPlaying)
			{
				this.audioToFade.Stop();
			}
		}
		if (this.outro != null && this.currentVolume > 0f)
		{
			this.outro.volume = this.currentVolume;
			this.outro.Play();
		}
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x000304E4 File Offset: 0x0002E6E4
	private void Update()
	{
		this.currentVolume = Mathf.MoveTowards(this.currentVolume, this.targetVolume, this.currentFadeSpeed * Time.deltaTime);
		this.audioToFade.volume = this.currentVolume;
		if (this.currentVolume == this.targetVolume)
		{
			base.enabled = false;
			if (this.currentVolume == 0f && this.audioToFade.isPlaying)
			{
				this.audioToFade.Stop();
			}
		}
	}

	// Token: 0x04000A85 RID: 2693
	[SerializeField]
	private AudioSource audioToFade;

	// Token: 0x04000A86 RID: 2694
	[SerializeField]
	private AudioSource outro;

	// Token: 0x04000A87 RID: 2695
	[SerializeField]
	private float fadeInDuration = 0.3f;

	// Token: 0x04000A88 RID: 2696
	[SerializeField]
	private float fadeOutDuration = 0.3f;

	// Token: 0x04000A89 RID: 2697
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x04000A8A RID: 2698
	private float currentVolume;

	// Token: 0x04000A8B RID: 2699
	private float targetVolume;

	// Token: 0x04000A8C RID: 2700
	private float currentFadeSpeed;

	// Token: 0x04000A8D RID: 2701
	private float fadeInSpeed;

	// Token: 0x04000A8E RID: 2702
	private float fadeOutSpeed;
}
