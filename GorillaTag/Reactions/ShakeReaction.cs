using System;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x02000D53 RID: 3411
	public class ShakeReaction : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x0600554A RID: 21834 RVA: 0x0019F96A File Offset: 0x0019DB6A
		private float loopSoundTotalDuration
		{
			get
			{
				return this.loopSoundFadeInDuration + this.loopSoundSustainDuration + this.loopSoundFadeOutDuration;
			}
		}

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x0600554B RID: 21835 RVA: 0x0019F980 File Offset: 0x0019DB80
		// (set) Token: 0x0600554C RID: 21836 RVA: 0x0019F988 File Offset: 0x0019DB88
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x0600554D RID: 21837 RVA: 0x0019F994 File Offset: 0x0019DB94
		protected void Awake()
		{
			this.sampleHistoryPos = new Vector3[256];
			this.sampleHistoryTime = new float[256];
			this.sampleHistoryVel = new Vector3[256];
			if (this.particles != null)
			{
				this.maxEmissionRate = this.particles.emission.rateOverTime.constant;
			}
			Application.quitting += this.HandleApplicationQuitting;
		}

		// Token: 0x0600554E RID: 21838 RVA: 0x0019FA14 File Offset: 0x0019DC14
		protected void OnEnable()
		{
			float unscaledTime = Time.unscaledTime;
			Vector3 position = this.shakeXform.position;
			for (int i = 0; i < 256; i++)
			{
				this.sampleHistoryTime[i] = unscaledTime;
				this.sampleHistoryPos[i] = position;
				this.sampleHistoryVel[i] = Vector3.zero;
			}
			if (this.loopSoundAudioSource != null)
			{
				this.loopSoundAudioSource.loop = true;
				this.loopSoundAudioSource.GTPlay();
			}
			this.hasLoopSound = this.loopSoundAudioSource != null;
			this.hasShakeSound = this.shakeSoundBankPlayer != null;
			this.hasParticleSystem = this.particles != null;
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x0600554F RID: 21839 RVA: 0x0019FACB File Offset: 0x0019DCCB
		protected void OnDisable()
		{
			if (this.loopSoundAudioSource != null)
			{
				this.loopSoundAudioSource.GTStop();
			}
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06005550 RID: 21840 RVA: 0x000D1CE3 File Offset: 0x000CFEE3
		private void HandleApplicationQuitting()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06005551 RID: 21841 RVA: 0x0019FAEC File Offset: 0x0019DCEC
		void ITickSystemPost.PostTick()
		{
			float unscaledTime = Time.unscaledTime;
			Vector3 position = this.shakeXform.position;
			int num = (this.currentIndex - 1 + 256) % 256;
			this.currentIndex = (this.currentIndex + 1) % 256;
			this.sampleHistoryTime[this.currentIndex] = unscaledTime;
			float num2 = unscaledTime - this.sampleHistoryTime[num];
			this.sampleHistoryPos[this.currentIndex] = position;
			if (num2 > 0f)
			{
				Vector3 vector = position - this.sampleHistoryPos[num];
				this.sampleHistoryVel[this.currentIndex] = vector / num2;
			}
			else
			{
				this.sampleHistoryVel[this.currentIndex] = Vector3.zero;
			}
			float sqrMagnitude = (this.sampleHistoryVel[num] - this.sampleHistoryVel[this.currentIndex]).sqrMagnitude;
			this.poopVelocity = Mathf.Round(Mathf.Sqrt(sqrMagnitude) * 1000f) / 1000f;
			float num3 = this.shakeXform.lossyScale.x * this.velocityThreshold * this.velocityThreshold;
			if (sqrMagnitude >= num3)
			{
				this.lastShakeTime = unscaledTime;
			}
			float num4 = unscaledTime - this.lastShakeTime;
			float num5 = Mathf.Clamp01(num4 / this.particleDuration);
			if (this.hasParticleSystem)
			{
				this.particles.emission.rateOverTime = this.emissionCurve.Evaluate(num5) * this.maxEmissionRate;
			}
			if (this.hasShakeSound && this.lastShakeTime - this.lastShakeSoundTime > this.shakeSoundCooldown)
			{
				this.shakeSoundBankPlayer.Play();
				this.lastShakeSoundTime = unscaledTime;
			}
			if (this.hasLoopSound)
			{
				if (num4 < this.loopSoundFadeInDuration)
				{
					this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeInCurve.Evaluate(Mathf.Clamp01(num4 / this.loopSoundFadeInDuration));
					return;
				}
				if (num4 < this.loopSoundFadeInDuration + this.loopSoundSustainDuration)
				{
					this.loopSoundAudioSource.volume = this.loopSoundBaseVolume;
					return;
				}
				this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeOutCurve.Evaluate(Mathf.Clamp01((num4 - this.loopSoundFadeInDuration - this.loopSoundSustainDuration) / this.loopSoundFadeOutDuration));
			}
		}

		// Token: 0x040058A6 RID: 22694
		[SerializeField]
		private Transform shakeXform;

		// Token: 0x040058A7 RID: 22695
		[SerializeField]
		private float velocityThreshold = 5f;

		// Token: 0x040058A8 RID: 22696
		[SerializeField]
		private SoundBankPlayer shakeSoundBankPlayer;

		// Token: 0x040058A9 RID: 22697
		[SerializeField]
		private float shakeSoundCooldown = 1f;

		// Token: 0x040058AA RID: 22698
		[SerializeField]
		private AudioSource loopSoundAudioSource;

		// Token: 0x040058AB RID: 22699
		[SerializeField]
		private float loopSoundBaseVolume = 1f;

		// Token: 0x040058AC RID: 22700
		[SerializeField]
		private float loopSoundSustainDuration = 1f;

		// Token: 0x040058AD RID: 22701
		[SerializeField]
		private float loopSoundFadeInDuration = 1f;

		// Token: 0x040058AE RID: 22702
		[SerializeField]
		private AnimationCurve loopSoundFadeInCurve;

		// Token: 0x040058AF RID: 22703
		[SerializeField]
		private float loopSoundFadeOutDuration = 1f;

		// Token: 0x040058B0 RID: 22704
		[SerializeField]
		private AnimationCurve loopSoundFadeOutCurve;

		// Token: 0x040058B1 RID: 22705
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x040058B2 RID: 22706
		[SerializeField]
		private AnimationCurve emissionCurve;

		// Token: 0x040058B3 RID: 22707
		[SerializeField]
		private float particleDuration = 5f;

		// Token: 0x040058B5 RID: 22709
		private const int sampleHistorySize = 256;

		// Token: 0x040058B6 RID: 22710
		private float[] sampleHistoryTime;

		// Token: 0x040058B7 RID: 22711
		private Vector3[] sampleHistoryPos;

		// Token: 0x040058B8 RID: 22712
		private Vector3[] sampleHistoryVel;

		// Token: 0x040058B9 RID: 22713
		private int currentIndex;

		// Token: 0x040058BA RID: 22714
		private float lastShakeSoundTime = float.MinValue;

		// Token: 0x040058BB RID: 22715
		private float lastShakeTime = float.MinValue;

		// Token: 0x040058BC RID: 22716
		private float maxEmissionRate;

		// Token: 0x040058BD RID: 22717
		private bool hasLoopSound;

		// Token: 0x040058BE RID: 22718
		private bool hasShakeSound;

		// Token: 0x040058BF RID: 22719
		private bool hasParticleSystem;

		// Token: 0x040058C0 RID: 22720
		[DebugReadout]
		private float poopVelocity;
	}
}
