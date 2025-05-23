using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.GuidedRefs;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000D37 RID: 3383
	public class VolcanoEffects : BaseGuidedRefTargetMono
	{
		// Token: 0x060054BD RID: 21693 RVA: 0x0019D06C File Offset: 0x0019B26C
		protected override void Awake()
		{
			base.Awake();
			if (this.RemoveNullsFromArray<ParticleSystem>(ref this.lavaSpewParticleSystems))
			{
				this.LogNullsFoundInArray("lavaSpewParticleSystems");
			}
			if (this.RemoveNullsFromArray<ParticleSystem>(ref this.smokeParticleSystems))
			{
				this.LogNullsFoundInArray("smokeParticleSystems");
			}
			this.hasVolcanoAudioSrc = this.volcanoAudioSource != null;
			this.hasForestSpeakerAudioSrc = this.forestSpeakerAudioSrc != null;
			this.lavaSpewEmissionModules = new ParticleSystem.EmissionModule[this.lavaSpewParticleSystems.Length];
			this.lavaSpewEmissionDefaultRateMultipliers = new float[this.lavaSpewParticleSystems.Length];
			this.lavaSpewDefaultEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
			this.lavaSpewAdjustedEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
			for (int i = 0; i < this.lavaSpewParticleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.lavaSpewParticleSystems[i].emission;
				this.lavaSpewEmissionDefaultRateMultipliers[i] = emission.rateOverTimeMultiplier;
				this.lavaSpewDefaultEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				this.lavaSpewAdjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				for (int j = 0; j < emission.burstCount; j++)
				{
					ParticleSystem.Burst burst = emission.GetBurst(j);
					this.lavaSpewDefaultEmitBursts[i][j] = burst;
					this.lavaSpewAdjustedEmitBursts[i][j] = new ParticleSystem.Burst(burst.time, burst.minCount, burst.maxCount, burst.cycleCount, burst.repeatInterval);
					this.lavaSpewAdjustedEmitBursts[i][j].count = burst.count;
				}
				this.lavaSpewEmissionModules[i] = emission;
			}
			this.smokeMainModules = new ParticleSystem.MainModule[this.smokeParticleSystems.Length];
			this.smokeEmissionModules = new ParticleSystem.EmissionModule[this.smokeParticleSystems.Length];
			this.smokeEmissionDefaultRateMultipliers = new float[this.smokeParticleSystems.Length];
			for (int k = 0; k < this.smokeParticleSystems.Length; k++)
			{
				this.smokeMainModules[k] = this.smokeParticleSystems[k].main;
				this.smokeEmissionModules[k] = this.smokeParticleSystems[k].emission;
				this.smokeEmissionDefaultRateMultipliers[k] = this.smokeEmissionModules[k].rateOverTimeMultiplier;
			}
			this.InitState(this.drainedStateFX);
			this.InitState(this.eruptingStateFX);
			this.InitState(this.risingStateFX);
			this.InitState(this.fullStateFX);
			this.InitState(this.drainingStateFX);
			this.currentStateFX = this.drainedStateFX;
			this.UpdateDrainedState(0f);
		}

		// Token: 0x060054BE RID: 21694 RVA: 0x0019D2FC File Offset: 0x0019B4FC
		public void OnVolcanoBellyEmpty()
		{
			if (!this.hasForestSpeakerAudioSrc)
			{
				return;
			}
			if (Time.time - this.timeVolcanoBellyWasLastEmpty < this.warnVolcanoBellyEmptied.length)
			{
				return;
			}
			this.forestSpeakerAudioSrc.gameObject.SetActive(true);
			this.forestSpeakerAudioSrc.GTPlayOneShot(this.warnVolcanoBellyEmptied, 1f);
		}

		// Token: 0x060054BF RID: 21695 RVA: 0x0019D354 File Offset: 0x0019B554
		public void OnStoneAccepted(double activationProgress)
		{
			if (!this.hasVolcanoAudioSrc)
			{
				return;
			}
			this.volcanoAudioSource.gameObject.SetActive(true);
			if (activationProgress > 1.0)
			{
				this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptLastStone, 1f);
				return;
			}
			this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptStone, 1f);
		}

		// Token: 0x060054C0 RID: 21696 RVA: 0x0019D3B4 File Offset: 0x0019B5B4
		private void InitState(VolcanoEffects.LavaStateFX fx)
		{
			fx.startSoundExists = fx.startSound != null;
			fx.endSoundExists = fx.endSound != null;
			fx.loop1Exists = fx.loop1AudioSrc != null;
			fx.loop2Exists = fx.loop2AudioSrc != null;
			if (fx.loop1Exists)
			{
				fx.loop1DefaultVolume = fx.loop1AudioSrc.volume;
				fx.loop1AudioSrc.volume = 0f;
			}
			if (fx.loop2Exists)
			{
				fx.loop2DefaultVolume = fx.loop2AudioSrc.volume;
				fx.loop2AudioSrc.volume = 0f;
			}
		}

		// Token: 0x060054C1 RID: 21697 RVA: 0x0019D45C File Offset: 0x0019B65C
		private void SetLavaAudioEnabled(bool toEnable)
		{
			AudioSource[] array = this.lavaSurfaceAudioSrcs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(toEnable);
			}
		}

		// Token: 0x060054C2 RID: 21698 RVA: 0x0019D48C File Offset: 0x0019B68C
		private void SetLavaAudioEnabled(bool toEnable, float volume)
		{
			foreach (AudioSource audioSource in this.lavaSurfaceAudioSrcs)
			{
				audioSource.volume = volume;
				audioSource.gameObject.SetActive(toEnable);
			}
		}

		// Token: 0x060054C3 RID: 21699 RVA: 0x0019D4C4 File Offset: 0x0019B6C4
		private void ResetState()
		{
			if (this.currentStateFX == null)
			{
				return;
			}
			this.currentStateFX.startSoundPlayed = false;
			this.currentStateFX.endSoundPlayed = false;
			if (this.currentStateFX.startSoundExists)
			{
				this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(false);
			}
			if (this.currentStateFX.endSoundExists)
			{
				this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(false);
			}
			if (this.currentStateFX.loop1Exists)
			{
				this.currentStateFX.loop1AudioSrc.gameObject.SetActive(false);
			}
			if (this.currentStateFX.loop2Exists)
			{
				this.currentStateFX.loop2AudioSrc.gameObject.SetActive(false);
			}
		}

		// Token: 0x060054C4 RID: 21700 RVA: 0x0019D580 File Offset: 0x0019B780
		private void UpdateState(float time, float timeRemaining, float progress)
		{
			if (this.currentStateFX == null)
			{
				return;
			}
			if (this.currentStateFX.startSoundExists && !this.currentStateFX.startSoundPlayed && time >= this.currentStateFX.startSoundDelay)
			{
				this.currentStateFX.startSoundPlayed = true;
				this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(true);
				this.currentStateFX.startSoundAudioSrc.GTPlayOneShot(this.currentStateFX.startSound, this.currentStateFX.startSoundVol);
			}
			if (this.currentStateFX.endSoundExists && !this.currentStateFX.endSoundPlayed && timeRemaining <= this.currentStateFX.endSound.length + this.currentStateFX.endSoundPadTime)
			{
				this.currentStateFX.endSoundPlayed = true;
				this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(true);
				this.currentStateFX.endSoundAudioSrc.GTPlayOneShot(this.currentStateFX.endSound, this.currentStateFX.endSoundVol);
			}
			if (this.currentStateFX.loop1Exists)
			{
				this.currentStateFX.loop1AudioSrc.volume = this.currentStateFX.loop1VolAnim.Evaluate(progress) * this.currentStateFX.loop1DefaultVolume;
				if (!this.currentStateFX.loop1AudioSrc.isPlaying)
				{
					this.currentStateFX.loop1AudioSrc.gameObject.SetActive(true);
					this.currentStateFX.loop1AudioSrc.GTPlay();
				}
			}
			if (this.currentStateFX.loop2Exists)
			{
				this.currentStateFX.loop2AudioSrc.volume = this.currentStateFX.loop2VolAnim.Evaluate(progress) * this.currentStateFX.loop2DefaultVolume;
				if (!this.currentStateFX.loop2AudioSrc.isPlaying)
				{
					this.currentStateFX.loop2AudioSrc.gameObject.SetActive(true);
					this.currentStateFX.loop2AudioSrc.GTPlay();
				}
			}
			for (int i = 0; i < this.smokeMainModules.Length; i++)
			{
				this.smokeMainModules[i].startColor = this.currentStateFX.smokeStartColorAnim.Evaluate(progress);
				this.smokeEmissionModules[i].rateOverTimeMultiplier = this.currentStateFX.smokeEmissionAnim.Evaluate(progress) * this.smokeEmissionDefaultRateMultipliers[i];
			}
			this.SetParticleEmissionRateAndBurst(this.currentStateFX.lavaSpewEmissionAnim.Evaluate(progress), this.lavaSpewEmissionModules, this.lavaSpewEmissionDefaultRateMultipliers, this.lavaSpewDefaultEmitBursts, this.lavaSpewAdjustedEmitBursts);
			if (this.applyShaderGlobals)
			{
				Shader.SetGlobalColor(this.shaderProp_ZoneLiquidLightColor, this.currentStateFX.lavaLightColor.Evaluate(progress) * this.currentStateFX.lavaLightIntensityAnim.Evaluate(progress));
				Shader.SetGlobalFloat(this.shaderProp_ZoneLiquidLightDistScale, this.currentStateFX.lavaLightAttenuationAnim.Evaluate(progress));
			}
		}

		// Token: 0x060054C5 RID: 21701 RVA: 0x0019D855 File Offset: 0x0019BA55
		public void SetDrainedState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(false);
			this.currentStateFX = this.drainedStateFX;
		}

		// Token: 0x060054C6 RID: 21702 RVA: 0x0019D870 File Offset: 0x0019BA70
		public void UpdateDrainedState(float time)
		{
			this.ResetState();
			this.UpdateState(time, float.MaxValue, float.MinValue);
		}

		// Token: 0x060054C7 RID: 21703 RVA: 0x0019D889 File Offset: 0x0019BA89
		public void SetEruptingState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(false, 0f);
			this.currentStateFX = this.eruptingStateFX;
		}

		// Token: 0x060054C8 RID: 21704 RVA: 0x0019D8A9 File Offset: 0x0019BAA9
		public void UpdateEruptingState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
		}

		// Token: 0x060054C9 RID: 21705 RVA: 0x0019D8B4 File Offset: 0x0019BAB4
		public void SetRisingState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(true, 0f);
			this.currentStateFX = this.risingStateFX;
		}

		// Token: 0x060054CA RID: 21706 RVA: 0x0019D8D4 File Offset: 0x0019BAD4
		public void UpdateRisingState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
			AudioSource[] array = this.lavaSurfaceAudioSrcs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].volume = Mathf.Lerp(0f, 1f, Mathf.Clamp01(time));
			}
		}

		// Token: 0x060054CB RID: 21707 RVA: 0x0019D91C File Offset: 0x0019BB1C
		public void SetFullState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(true, 1f);
			this.currentStateFX = this.fullStateFX;
		}

		// Token: 0x060054CC RID: 21708 RVA: 0x0019D8A9 File Offset: 0x0019BAA9
		public void UpdateFullState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
		}

		// Token: 0x060054CD RID: 21709 RVA: 0x0019D93C File Offset: 0x0019BB3C
		public void SetDrainingState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(true, 1f);
			this.currentStateFX = this.drainingStateFX;
		}

		// Token: 0x060054CE RID: 21710 RVA: 0x0019D95C File Offset: 0x0019BB5C
		public void UpdateDrainingState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
			AudioSource[] array = this.lavaSurfaceAudioSrcs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].volume = Mathf.Lerp(1f, 0f, progress);
			}
		}

		// Token: 0x060054CF RID: 21711 RVA: 0x0019D9A0 File Offset: 0x0019BBA0
		private void SetParticleEmissionRateAndBurst(float multiplier, ParticleSystem.EmissionModule[] emissionModules, float[] defaultRateMultipliers, ParticleSystem.Burst[][] defaultEmitBursts, ParticleSystem.Burst[][] adjustedEmitBursts)
		{
			for (int i = 0; i < emissionModules.Length; i++)
			{
				emissionModules[i].rateOverTimeMultiplier = multiplier * defaultRateMultipliers[i];
				int num = Mathf.Min(emissionModules[i].burstCount, defaultEmitBursts[i].Length);
				for (int j = 0; j < num; j++)
				{
					adjustedEmitBursts[i][j].probability = defaultEmitBursts[i][j].probability * multiplier;
				}
				emissionModules[i].SetBursts(adjustedEmitBursts[i]);
			}
		}

		// Token: 0x060054D0 RID: 21712 RVA: 0x0019DA20 File Offset: 0x0019BC20
		private bool RemoveNullsFromArray<T>(ref T[] array) where T : Object
		{
			List<T> list = new List<T>(array.Length);
			foreach (T t in array)
			{
				if (t != null)
				{
					list.Add(t);
				}
			}
			int num = array.Length;
			array = list.ToArray();
			return num != array.Length;
		}

		// Token: 0x060054D1 RID: 21713 RVA: 0x0019DA7A File Offset: 0x0019BC7A
		private void LogNullsFoundInArray(string nameOfArray)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Null reference found in ",
				nameOfArray,
				" array of component: \"",
				this.GetComponentPath(int.MaxValue),
				"\""
			}), this);
		}

		// Token: 0x04005805 RID: 22533
		[Tooltip("Only one VolcanoEffects should change shader globals in the scene (lava color, lava light) at a time.")]
		[SerializeField]
		private bool applyShaderGlobals = true;

		// Token: 0x04005806 RID: 22534
		[Tooltip("Game trigger notification sounds will play through this.")]
		[SerializeField]
		private AudioSource forestSpeakerAudioSrc;

		// Token: 0x04005807 RID: 22535
		[Tooltip("The accumulator value of rocks being thrown into the volcano has been reset.")]
		[SerializeField]
		private AudioClip warnVolcanoBellyEmptied;

		// Token: 0x04005808 RID: 22536
		[Tooltip("Accept stone sounds will play through here.")]
		[SerializeField]
		private AudioSource volcanoAudioSource;

		// Token: 0x04005809 RID: 22537
		[Tooltip("volcano ate rock but needs more.")]
		[SerializeField]
		private AudioClip volcanoAcceptStone;

		// Token: 0x0400580A RID: 22538
		[Tooltip("volcano ate last needed rock.")]
		[SerializeField]
		private AudioClip volcanoAcceptLastStone;

		// Token: 0x0400580B RID: 22539
		[Tooltip("This will be faded in while lava is rising.")]
		[SerializeField]
		private AudioSource[] lavaSurfaceAudioSrcs;

		// Token: 0x0400580C RID: 22540
		[Tooltip("Emission will be adjusted for these particles during eruption.")]
		[SerializeField]
		private ParticleSystem[] lavaSpewParticleSystems;

		// Token: 0x0400580D RID: 22541
		[Tooltip("Smoke emits during all states but it's intensity and color will change when erupting/idling.")]
		[SerializeField]
		private ParticleSystem[] smokeParticleSystems;

		// Token: 0x0400580E RID: 22542
		[SerializeField]
		private VolcanoEffects.LavaStateFX drainedStateFX;

		// Token: 0x0400580F RID: 22543
		[SerializeField]
		private VolcanoEffects.LavaStateFX eruptingStateFX;

		// Token: 0x04005810 RID: 22544
		[SerializeField]
		private VolcanoEffects.LavaStateFX risingStateFX;

		// Token: 0x04005811 RID: 22545
		[SerializeField]
		private VolcanoEffects.LavaStateFX fullStateFX;

		// Token: 0x04005812 RID: 22546
		[SerializeField]
		private VolcanoEffects.LavaStateFX drainingStateFX;

		// Token: 0x04005813 RID: 22547
		private VolcanoEffects.LavaStateFX currentStateFX;

		// Token: 0x04005814 RID: 22548
		private ParticleSystem.EmissionModule[] lavaSpewEmissionModules;

		// Token: 0x04005815 RID: 22549
		private float[] lavaSpewEmissionDefaultRateMultipliers;

		// Token: 0x04005816 RID: 22550
		private ParticleSystem.Burst[][] lavaSpewDefaultEmitBursts;

		// Token: 0x04005817 RID: 22551
		private ParticleSystem.Burst[][] lavaSpewAdjustedEmitBursts;

		// Token: 0x04005818 RID: 22552
		private ParticleSystem.MainModule[] smokeMainModules;

		// Token: 0x04005819 RID: 22553
		private ParticleSystem.EmissionModule[] smokeEmissionModules;

		// Token: 0x0400581A RID: 22554
		private float[] smokeEmissionDefaultRateMultipliers;

		// Token: 0x0400581B RID: 22555
		private int shaderProp_ZoneLiquidLightColor = Shader.PropertyToID("_ZoneLiquidLightColor");

		// Token: 0x0400581C RID: 22556
		private int shaderProp_ZoneLiquidLightDistScale = Shader.PropertyToID("_ZoneLiquidLightDistScale");

		// Token: 0x0400581D RID: 22557
		private float timeVolcanoBellyWasLastEmpty;

		// Token: 0x0400581E RID: 22558
		private bool hasVolcanoAudioSrc;

		// Token: 0x0400581F RID: 22559
		private bool hasForestSpeakerAudioSrc;

		// Token: 0x02000D38 RID: 3384
		[Serializable]
		public class LavaStateFX
		{
			// Token: 0x04005820 RID: 22560
			public AudioClip startSound;

			// Token: 0x04005821 RID: 22561
			public AudioSource startSoundAudioSrc;

			// Token: 0x04005822 RID: 22562
			[Tooltip("Multiplied by the AudioSource's volume.")]
			public float startSoundVol = 1f;

			// Token: 0x04005823 RID: 22563
			[FormerlySerializedAs("startSoundPad")]
			public float startSoundDelay;

			// Token: 0x04005824 RID: 22564
			public AudioClip endSound;

			// Token: 0x04005825 RID: 22565
			public AudioSource endSoundAudioSrc;

			// Token: 0x04005826 RID: 22566
			[Tooltip("Multiplied by the AudioSource's volume.")]
			public float endSoundVol = 1f;

			// Token: 0x04005827 RID: 22567
			[Tooltip("How much time should there be between the end of the clip playing and the end of the state.")]
			public float endSoundPadTime;

			// Token: 0x04005828 RID: 22568
			public AudioSource loop1AudioSrc;

			// Token: 0x04005829 RID: 22569
			public AnimationCurve loop1VolAnim;

			// Token: 0x0400582A RID: 22570
			public AudioSource loop2AudioSrc;

			// Token: 0x0400582B RID: 22571
			public AnimationCurve loop2VolAnim;

			// Token: 0x0400582C RID: 22572
			public AnimationCurve lavaSpewEmissionAnim;

			// Token: 0x0400582D RID: 22573
			public AnimationCurve smokeEmissionAnim;

			// Token: 0x0400582E RID: 22574
			public Gradient smokeStartColorAnim;

			// Token: 0x0400582F RID: 22575
			public Gradient lavaLightColor;

			// Token: 0x04005830 RID: 22576
			public AnimationCurve lavaLightIntensityAnim = AnimationCurve.Constant(0f, 1f, 60f);

			// Token: 0x04005831 RID: 22577
			public AnimationCurve lavaLightAttenuationAnim = AnimationCurve.Constant(0f, 1f, 0.1f);

			// Token: 0x04005832 RID: 22578
			[NonSerialized]
			public bool startSoundExists;

			// Token: 0x04005833 RID: 22579
			[NonSerialized]
			public bool startSoundPlayed;

			// Token: 0x04005834 RID: 22580
			[NonSerialized]
			public bool endSoundExists;

			// Token: 0x04005835 RID: 22581
			[NonSerialized]
			public bool endSoundPlayed;

			// Token: 0x04005836 RID: 22582
			[NonSerialized]
			public bool loop1Exists;

			// Token: 0x04005837 RID: 22583
			[NonSerialized]
			public float loop1DefaultVolume;

			// Token: 0x04005838 RID: 22584
			[NonSerialized]
			public bool loop2Exists;

			// Token: 0x04005839 RID: 22585
			[NonSerialized]
			public float loop2DefaultVolume;
		}
	}
}
