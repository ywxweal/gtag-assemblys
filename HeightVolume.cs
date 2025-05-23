using System;
using UnityEngine;

// Token: 0x02000662 RID: 1634
public class HeightVolume : MonoBehaviour
{
	// Token: 0x060028D9 RID: 10457 RVA: 0x000CB675 File Offset: 0x000C9875
	private void Awake()
	{
		if (this.targetTransform == null)
		{
			this.targetTransform = Camera.main.transform;
		}
		this.musicSource = this.audioSource.gameObject.GetComponent<MusicSource>();
	}

	// Token: 0x060028DA RID: 10458 RVA: 0x000CB6AC File Offset: 0x000C98AC
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

	// Token: 0x04002DD9 RID: 11737
	public Transform heightTop;

	// Token: 0x04002DDA RID: 11738
	public Transform heightBottom;

	// Token: 0x04002DDB RID: 11739
	public AudioSource audioSource;

	// Token: 0x04002DDC RID: 11740
	public float baseVolume;

	// Token: 0x04002DDD RID: 11741
	public float minVolume;

	// Token: 0x04002DDE RID: 11742
	public Transform targetTransform;

	// Token: 0x04002DDF RID: 11743
	public bool invertHeightVol;

	// Token: 0x04002DE0 RID: 11744
	private MusicSource musicSource;
}
