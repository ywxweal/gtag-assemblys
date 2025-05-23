using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B68 RID: 2920
	public class BuilderScaleAudioRadius : MonoBehaviour
	{
		// Token: 0x06004844 RID: 18500 RVA: 0x00158A76 File Offset: 0x00156C76
		private void OnEnable()
		{
			if (this.useLossyScaleOnEnable)
			{
				this.setScaleNextFrame = true;
				this.enableFrame = Time.frameCount;
			}
		}

		// Token: 0x06004845 RID: 18501 RVA: 0x00158A92 File Offset: 0x00156C92
		private void OnDisable()
		{
			if (this.useLossyScaleOnEnable)
			{
				this.RevertScale();
			}
		}

		// Token: 0x06004846 RID: 18502 RVA: 0x00158AA2 File Offset: 0x00156CA2
		private void LateUpdate()
		{
			if (this.setScaleNextFrame && Time.frameCount > this.enableFrame)
			{
				if (this.useLossyScaleOnEnable)
				{
					this.SetScale(base.transform.lossyScale.x);
				}
				this.setScaleNextFrame = false;
			}
		}

		// Token: 0x06004847 RID: 18503 RVA: 0x00158ADE File Offset: 0x00156CDE
		private void PlaySound()
		{
			if (this.autoPlaySoundBank != null)
			{
				this.autoPlaySoundBank.Play();
				return;
			}
			if (this.audioSource.clip != null)
			{
				this.audioSource.Play();
			}
		}

		// Token: 0x06004848 RID: 18504 RVA: 0x00158B18 File Offset: 0x00156D18
		public void SetScale(float inScale)
		{
			if (Mathf.Approximately(inScale, this.scale))
			{
				if (this.autoPlay)
				{
					this.PlaySound();
				}
				return;
			}
			this.scale = inScale;
			this.RevertScale();
			if (Mathf.Approximately(this.scale, 1f))
			{
				if (this.autoPlay)
				{
					this.PlaySound();
				}
				return;
			}
			AudioRolloffMode rolloffMode = this.audioSource.rolloffMode;
			if (rolloffMode > AudioRolloffMode.Linear)
			{
				if (rolloffMode == AudioRolloffMode.Custom)
				{
					this.maxDist = this.audioSource.maxDistance;
					this.audioSource.maxDistance *= this.scale;
				}
			}
			else
			{
				this.minDist = this.audioSource.minDistance;
				this.maxDist = this.audioSource.maxDistance;
				this.audioSource.maxDistance *= this.scale;
				this.audioSource.minDistance *= this.scale;
			}
			if (this.autoPlay)
			{
				this.PlaySound();
			}
			this.shouldRevert = true;
		}

		// Token: 0x06004849 RID: 18505 RVA: 0x00158C18 File Offset: 0x00156E18
		public void RevertScale()
		{
			if (!this.shouldRevert)
			{
				return;
			}
			AudioRolloffMode rolloffMode = this.audioSource.rolloffMode;
			if (rolloffMode > AudioRolloffMode.Linear)
			{
				if (rolloffMode == AudioRolloffMode.Custom)
				{
					this.audioSource.maxDistance = this.maxDist;
				}
			}
			else
			{
				this.audioSource.minDistance = this.minDist;
				this.audioSource.maxDistance = this.maxDist;
			}
			this.scale = 1f;
			this.shouldRevert = false;
		}

		// Token: 0x04004ABD RID: 19133
		[Tooltip("Scale particles on enable using lossy scale")]
		[SerializeField]
		private bool useLossyScaleOnEnable;

		// Token: 0x04004ABE RID: 19134
		[Tooltip("Play sound after scaling")]
		[SerializeField]
		private bool autoPlay;

		// Token: 0x04004ABF RID: 19135
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04004AC0 RID: 19136
		[FormerlySerializedAs("soundBankToPlay")]
		[SerializeField]
		private SoundBankPlayer autoPlaySoundBank;

		// Token: 0x04004AC1 RID: 19137
		private float minDist;

		// Token: 0x04004AC2 RID: 19138
		private float maxDist = 1f;

		// Token: 0x04004AC3 RID: 19139
		private AnimationCurve customCurve;

		// Token: 0x04004AC4 RID: 19140
		private AnimationCurve scaledCurve = new AnimationCurve();

		// Token: 0x04004AC5 RID: 19141
		private float scale = 1f;

		// Token: 0x04004AC6 RID: 19142
		private bool shouldRevert;

		// Token: 0x04004AC7 RID: 19143
		private bool setScaleNextFrame;

		// Token: 0x04004AC8 RID: 19144
		private int enableFrame;
	}
}
