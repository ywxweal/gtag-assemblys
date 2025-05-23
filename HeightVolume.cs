using System;
using UnityEngine;

// Token: 0x02000662 RID: 1634
public class HeightVolume : MonoBehaviour
{
	// Token: 0x060028D8 RID: 10456 RVA: 0x000CB5D1 File Offset: 0x000C97D1
	private void Awake()
	{
		if (this.targetTransform == null)
		{
			this.targetTransform = Camera.main.transform;
		}
		this.musicSource = this.audioSource.gameObject.GetComponent<MusicSource>();
	}

	// Token: 0x060028D9 RID: 10457 RVA: 0x000CB608 File Offset: 0x000C9808
	private void Update()
	{
		if (this.audioSource.gameObject.activeSelf && (!(this.musicSource != null) || !this.musicSource.VolumeOverridden))
		{
			if (this.targetTransform.position.y > this.heightTop.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.baseVolume : this.minVolume);
				return;
			}
			if (this.targetTransform.position.y < this.heightBottom.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.minVolume : this.baseVolume);
				return;
			}
			this.audioSource.volume = ((!this.invertHeightVol) ? ((this.targetTransform.position.y - this.heightBottom.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume) : ((this.heightTop.position.y - this.targetTransform.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume));
		}
	}

	// Token: 0x04002DD7 RID: 11735
	public Transform heightTop;

	// Token: 0x04002DD8 RID: 11736
	public Transform heightBottom;

	// Token: 0x04002DD9 RID: 11737
	public AudioSource audioSource;

	// Token: 0x04002DDA RID: 11738
	public float baseVolume;

	// Token: 0x04002DDB RID: 11739
	public float minVolume;

	// Token: 0x04002DDC RID: 11740
	public Transform targetTransform;

	// Token: 0x04002DDD RID: 11741
	public bool invertHeightVol;

	// Token: 0x04002DDE RID: 11742
	private MusicSource musicSource;
}
