using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DCB RID: 3531
	public class SquirtingFlowerBadgeCosmetic : MonoBehaviour, ISpawnable, IFingerFlexListener
	{
		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x06005785 RID: 22405 RVA: 0x001ADCDA File Offset: 0x001ABEDA
		// (set) Token: 0x06005786 RID: 22406 RVA: 0x001ADCE2 File Offset: 0x001ABEE2
		public VRRig MyRig { get; private set; }

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x06005787 RID: 22407 RVA: 0x001ADCEB File Offset: 0x001ABEEB
		// (set) Token: 0x06005788 RID: 22408 RVA: 0x001ADCF3 File Offset: 0x001ABEF3
		public bool IsSpawned { get; set; }

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x06005789 RID: 22409 RVA: 0x001ADCFC File Offset: 0x001ABEFC
		// (set) Token: 0x0600578A RID: 22410 RVA: 0x001ADD04 File Offset: 0x001ABF04
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x0600578B RID: 22411 RVA: 0x001ADD0D File Offset: 0x001ABF0D
		public void OnSpawn(VRRig rig)
		{
			this.MyRig = rig;
		}

		// Token: 0x0600578C RID: 22412 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnDespawn()
		{
		}

		// Token: 0x0600578D RID: 22413 RVA: 0x001ADD16 File Offset: 0x001ABF16
		private void Update()
		{
			if (!this.restartTimer && Time.time - this.triggeredTime >= this.coolDownTimer)
			{
				this.restartTimer = true;
			}
		}

		// Token: 0x0600578E RID: 22414 RVA: 0x001ADD3C File Offset: 0x001ABF3C
		private void OnPlayEffectLocal()
		{
			if (this.particlesToPlay != null)
			{
				this.particlesToPlay.Play();
			}
			if (this.objectToEnable != null)
			{
				this.objectToEnable.SetActive(true);
			}
			if (this.audioSource != null && this.audioToPlay != null)
			{
				this.audioSource.GTPlayOneShot(this.audioToPlay, 1f);
			}
			this.restartTimer = false;
			this.triggeredTime = Time.time;
		}

		// Token: 0x0600578F RID: 22415 RVA: 0x001ADDC0 File Offset: 0x001ABFC0
		public void OnButtonPressed(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			if (!this.restartTimer || !this.buttonReleased)
			{
				return;
			}
			this.OnPlayEffectLocal();
			this.buttonReleased = false;
		}

		// Token: 0x06005790 RID: 22416 RVA: 0x001ADDEA File Offset: 0x001ABFEA
		public void OnButtonReleased(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			this.buttonReleased = true;
		}

		// Token: 0x06005791 RID: 22417 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnButtonPressStayed(bool isLeftHand, float value)
		{
		}

		// Token: 0x06005792 RID: 22418 RVA: 0x001ADDFD File Offset: 0x001ABFFD
		public bool FingerFlexValidation(bool isLeftHand)
		{
			return (!this.leftHand || isLeftHand) && (this.leftHand || !isLeftHand);
		}

		// Token: 0x04005C28 RID: 23592
		[SerializeField]
		private ParticleSystem particlesToPlay;

		// Token: 0x04005C29 RID: 23593
		[SerializeField]
		private GameObject objectToEnable;

		// Token: 0x04005C2A RID: 23594
		[SerializeField]
		private AudioClip audioToPlay;

		// Token: 0x04005C2B RID: 23595
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005C2C RID: 23596
		[SerializeField]
		private float coolDownTimer = 2f;

		// Token: 0x04005C2D RID: 23597
		[SerializeField]
		private bool leftHand;

		// Token: 0x04005C2E RID: 23598
		private float triggeredTime;

		// Token: 0x04005C2F RID: 23599
		private bool restartTimer;

		// Token: 0x04005C30 RID: 23600
		private bool buttonReleased = true;
	}
}
