using System;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DFD RID: 3581
	public class VelocityBasedAudioTriggerCosmetic : MonoBehaviour
	{
		// Token: 0x060058A2 RID: 22690 RVA: 0x001B40D8 File Offset: 0x001B22D8
		private void Awake()
		{
			this.audioSource.clip = this.audioClip;
		}

		// Token: 0x060058A3 RID: 22691 RVA: 0x001B40EC File Offset: 0x001B22EC
		private void Update()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (averageVelocity.magnitude < this.minVelocityThreshold)
			{
				return;
			}
			float num = Mathf.InverseLerp(this.minOutputVolume, this.maxOutputVolume, averageVelocity.magnitude);
			this.audioSource.volume = num;
			if (this.audioSource != null && !this.audioSource.isPlaying)
			{
				this.audioSource.clip = this.audioClip;
				this.audioSource.GTPlay();
			}
		}

		// Token: 0x04005DFE RID: 24062
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04005DFF RID: 24063
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005E00 RID: 24064
		[SerializeField]
		private AudioClip audioClip;

		// Token: 0x04005E01 RID: 24065
		[Tooltip(" Minimum velocity to trigger audio")]
		[SerializeField]
		private float minVelocityThreshold;

		// Token: 0x04005E02 RID: 24066
		[SerializeField]
		private float minOutputVolume;

		// Token: 0x04005E03 RID: 24067
		[SerializeField]
		private float maxOutputVolume = 1f;
	}
}
