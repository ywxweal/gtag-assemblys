using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DCB RID: 3531
	public class SquirtingFlowerBadgeCosmetic : MonoBehaviour, ISpawnable, IFingerFlexListener
	{
		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x06005786 RID: 22406 RVA: 0x001ADDB2 File Offset: 0x001ABFB2
		// (set) Token: 0x06005787 RID: 22407 RVA: 0x001ADDBA File Offset: 0x001ABFBA
		public VRRig MyRig { get; private set; }

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x06005788 RID: 22408 RVA: 0x001ADDC3 File Offset: 0x001ABFC3
		// (set) Token: 0x06005789 RID: 22409 RVA: 0x001ADDCB File Offset: 0x001ABFCB
		public bool IsSpawned { get; set; }

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x0600578A RID: 22410 RVA: 0x001ADDD4 File Offset: 0x001ABFD4
		// (set) Token: 0x0600578B RID: 22411 RVA: 0x001ADDDC File Offset: 0x001ABFDC
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x0600578C RID: 22412 RVA: 0x001ADDE5 File Offset: 0x001ABFE5
		public void OnSpawn(VRRig rig)
		{
			this.MyRig = rig;
		}

		// Token: 0x0600578D RID: 22413 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnDespawn()
		{
		}

		// Token: 0x0600578E RID: 22414 RVA: 0x001ADDEE File Offset: 0x001ABFEE
		private void Update()
		{
			if (!this.restartTimer && Time.time - this.triggeredTime >= this.coolDownTimer)
			{
				this.restartTimer = true;
			}
		}

		// Token: 0x0600578F RID: 22415 RVA: 0x001ADE14 File Offset: 0x001AC014
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

		// Token: 0x06005790 RID: 22416 RVA: 0x001ADE98 File Offset: 0x001AC098
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

		// Token: 0x06005791 RID: 22417 RVA: 0x001ADEC2 File Offset: 0x001AC0C2
		public void OnButtonReleased(bool isLeftHand, float value)
		{
			if (!this.FingerFlexValidation(isLeftHand))
			{
				return;
			}
			this.buttonReleased = true;
		}

		// Token: 0x06005792 RID: 22418 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnButtonPressStayed(bool isLeftHand, float value)
		{
		}

		// Token: 0x06005793 RID: 22419 RVA: 0x001ADED5 File Offset: 0x001AC0D5
		public bool FingerFlexValidation(bool isLeftHand)
		{
			return (!this.leftHand || isLeftHand) && (this.leftHand || !isLeftHand);
		}

		// Token: 0x04005C29 RID: 23593
		[SerializeField]
		private ParticleSystem particlesToPlay;

		// Token: 0x04005C2A RID: 23594
		[SerializeField]
		private GameObject objectToEnable;

		// Token: 0x04005C2B RID: 23595
		[SerializeField]
		private AudioClip audioToPlay;

		// Token: 0x04005C2C RID: 23596
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005C2D RID: 23597
		[SerializeField]
		private float coolDownTimer = 2f;

		// Token: 0x04005C2E RID: 23598
		[SerializeField]
		private bool leftHand;

		// Token: 0x04005C2F RID: 23599
		private float triggeredTime;

		// Token: 0x04005C30 RID: 23600
		private bool restartTimer;

		// Token: 0x04005C31 RID: 23601
		private bool buttonReleased = true;
	}
}
