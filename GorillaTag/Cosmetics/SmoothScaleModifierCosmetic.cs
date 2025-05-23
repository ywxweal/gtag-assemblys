using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DF5 RID: 3573
	[RequireComponent(typeof(TransferrableObject))]
	public class SmoothScaleModifierCosmetic : MonoBehaviour
	{
		// Token: 0x06005873 RID: 22643 RVA: 0x001B3301 File Offset: 0x001B1501
		private void Awake()
		{
			this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
			this.initialScale = this.objectPrefab.transform.localScale;
		}

		// Token: 0x06005874 RID: 22644 RVA: 0x001B3325 File Offset: 0x001B1525
		private void OnEnable()
		{
			this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
		}

		// Token: 0x06005875 RID: 22645 RVA: 0x001B3330 File Offset: 0x001B1530
		private void Update()
		{
			if (this.transferrableObject && !this.transferrableObject.InHand())
			{
				if (this.audioSource && this.audioSource.isPlaying)
				{
					this.audioSource.GTStop();
				}
				return;
			}
			switch (this.currentState)
			{
			case SmoothScaleModifierCosmetic.State.None:
				if (this.audioSource && this.normalSizeAudio && !this.audioSource.isPlaying)
				{
					this.audioSource.clip = this.normalSizeAudio;
					this.audioSource.volume = this.normalSizeAudioVolume;
					this.audioSource.GTPlay();
				}
				break;
			case SmoothScaleModifierCosmetic.State.Reset:
				this.SmoothScale(this.objectPrefab.transform.localScale, this.initialScale);
				if (Vector3.Distance(this.objectPrefab.transform.localScale, this.initialScale) < 0.01f)
				{
					this.objectPrefab.transform.localScale = this.initialScale;
					this.UpdateState(SmoothScaleModifierCosmetic.State.None);
					return;
				}
				break;
			case SmoothScaleModifierCosmetic.State.Scaling:
				this.SmoothScale(this.objectPrefab.transform.localScale, this.targetScale);
				if (Vector3.Distance(this.objectPrefab.transform.localScale, this.targetScale) < 0.01f)
				{
					this.objectPrefab.transform.localScale = this.targetScale;
					this.UpdateState(SmoothScaleModifierCosmetic.State.Scaled);
					return;
				}
				break;
			case SmoothScaleModifierCosmetic.State.Scaled:
				if (this.audioSource && this.scaledAudio && !this.audioSource.isPlaying)
				{
					this.audioSource.clip = this.scaledAudio;
					this.audioSource.volume = this.scaleAudioVolume;
					this.audioSource.GTPlay();
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06005876 RID: 22646 RVA: 0x001B350B File Offset: 0x001B170B
		private void SmoothScale(Vector3 initial, Vector3 target)
		{
			this.objectPrefab.transform.localScale = Vector3.MoveTowards(initial, target, this.speed * Time.deltaTime);
		}

		// Token: 0x06005877 RID: 22647 RVA: 0x001B3530 File Offset: 0x001B1730
		private void ApplyScaling(IFingerFlexListener.ComponentActivator activator)
		{
			if (this.audioSource)
			{
				this.audioSource.GTStop();
			}
			if (this.scaleOn == activator)
			{
				if (this.currentState != SmoothScaleModifierCosmetic.State.Scaled)
				{
					this.UpdateState(SmoothScaleModifierCosmetic.State.Scaling);
					return;
				}
			}
			else if (this.resetOn == activator && this.currentState != SmoothScaleModifierCosmetic.State.Reset)
			{
				this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
			}
		}

		// Token: 0x06005878 RID: 22648 RVA: 0x001B3588 File Offset: 0x001B1788
		private void UpdateState(SmoothScaleModifierCosmetic.State newState)
		{
			this.currentState = newState;
		}

		// Token: 0x06005879 RID: 22649 RVA: 0x001B3591 File Offset: 0x001B1791
		public void OnButtonPressed()
		{
			this.ApplyScaling(IFingerFlexListener.ComponentActivator.FingerFlexed);
		}

		// Token: 0x0600587A RID: 22650 RVA: 0x001B359A File Offset: 0x001B179A
		public void OnButtonReleased()
		{
			this.ApplyScaling(IFingerFlexListener.ComponentActivator.FingerReleased);
		}

		// Token: 0x0600587B RID: 22651 RVA: 0x001B35A3 File Offset: 0x001B17A3
		public void OnButtonPressStayed()
		{
			this.ApplyScaling(IFingerFlexListener.ComponentActivator.FingerStayed);
		}

		// Token: 0x04005DBA RID: 23994
		[SerializeField]
		private GameObject objectPrefab;

		// Token: 0x04005DBB RID: 23995
		[SerializeField]
		private Vector3 targetScale = new Vector3(2f, 2f, 2f);

		// Token: 0x04005DBC RID: 23996
		[SerializeField]
		private float speed = 2f;

		// Token: 0x04005DBD RID: 23997
		[SerializeField]
		private IFingerFlexListener.ComponentActivator scaleOn;

		// Token: 0x04005DBE RID: 23998
		[SerializeField]
		private IFingerFlexListener.ComponentActivator resetOn;

		// Token: 0x04005DBF RID: 23999
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005DC0 RID: 24000
		[SerializeField]
		private AudioClip scaledAudio;

		// Token: 0x04005DC1 RID: 24001
		[SerializeField]
		private float scaleAudioVolume = 0.1f;

		// Token: 0x04005DC2 RID: 24002
		[SerializeField]
		private AudioClip normalSizeAudio;

		// Token: 0x04005DC3 RID: 24003
		[SerializeField]
		private float normalSizeAudioVolume = 0.1f;

		// Token: 0x04005DC4 RID: 24004
		private SmoothScaleModifierCosmetic.State currentState;

		// Token: 0x04005DC5 RID: 24005
		private Vector3 initialScale;

		// Token: 0x04005DC6 RID: 24006
		private TransferrableObject transferrableObject;

		// Token: 0x02000DF6 RID: 3574
		private enum State
		{
			// Token: 0x04005DC8 RID: 24008
			None,
			// Token: 0x04005DC9 RID: 24009
			Reset,
			// Token: 0x04005DCA RID: 24010
			Scaling,
			// Token: 0x04005DCB RID: 24011
			Scaled
		}
	}
}
