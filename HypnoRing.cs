using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000460 RID: 1120
public class HypnoRing : MonoBehaviour, ISpawnable
{
	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06001B7E RID: 7038 RVA: 0x000870CD File Offset: 0x000852CD
	// (set) Token: 0x06001B7F RID: 7039 RVA: 0x000870D5 File Offset: 0x000852D5
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06001B80 RID: 7040 RVA: 0x000870DE File Offset: 0x000852DE
	// (set) Token: 0x06001B81 RID: 7041 RVA: 0x000870E6 File Offset: 0x000852E6
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001B82 RID: 7042 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001B83 RID: 7043 RVA: 0x000870EF File Offset: 0x000852EF
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06001B84 RID: 7044 RVA: 0x000870F8 File Offset: 0x000852F8
	private void Update()
	{
		if ((this.attachedToLeftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT) > 0.5f)
		{
			base.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * this.rotationSpeed, Vector3.up);
			this.currentVolume = Mathf.MoveTowards(this.currentVolume, this.maxVolume, Time.deltaTime / this.fadeInDuration);
			this.audioSource.volume = this.currentVolume;
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.GTPlay();
				return;
			}
		}
		else
		{
			this.currentVolume = Mathf.MoveTowards(this.currentVolume, 0f, Time.deltaTime / this.fadeOutDuration);
			if (this.audioSource.isPlaying)
			{
				if (this.currentVolume == 0f)
				{
					this.audioSource.GTStop();
					return;
				}
				this.audioSource.volume = this.currentVolume;
			}
		}
	}

	// Token: 0x04001E79 RID: 7801
	[SerializeField]
	private bool attachedToLeftHand;

	// Token: 0x04001E7A RID: 7802
	private VRRig myRig;

	// Token: 0x04001E7B RID: 7803
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04001E7C RID: 7804
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001E7D RID: 7805
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x04001E7E RID: 7806
	[SerializeField]
	private float fadeInDuration;

	// Token: 0x04001E7F RID: 7807
	[SerializeField]
	private float fadeOutDuration;

	// Token: 0x04001E82 RID: 7810
	private float currentVolume;
}
