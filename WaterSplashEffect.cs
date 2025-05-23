using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x02000231 RID: 561
public class WaterSplashEffect : MonoBehaviour
{
	// Token: 0x06000CF7 RID: 3319 RVA: 0x000445DB File Offset: 0x000427DB
	private void OnEnable()
	{
		this.startTime = Time.time;
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x000445E8 File Offset: 0x000427E8
	public void Destroy()
	{
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.DeactivateParticleSystems(this.smallSplashParticleSystems);
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x0004461C File Offset: 0x0004281C
	public void PlayEffect(bool isBigSplash, bool isEntry, float scale, WaterVolume volume = null)
	{
		this.waterVolume = volume;
		if (isBigSplash)
		{
			this.DeactivateParticleSystems(this.smallSplashParticleSystems);
			this.SetParticleEffectParameters(this.bigSplashParticleSystems, scale, this.bigSplashBaseGravityMultiplier, this.bigSplashBaseStartSpeed, this.bigSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.bigSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.bigSplashAudioClips, ref WaterSplashEffect.lastPlayedBigSplashAudioClipIndex);
			return;
		}
		if (isEntry)
		{
			this.DeactivateParticleSystems(this.bigSplashParticleSystems);
			this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.smallSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.smallSplashEntryAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashEntryAudioClipIndex);
			return;
		}
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
		this.PlayParticleEffects(this.smallSplashParticleSystems);
		this.PlayRandomAudioClipWithoutRepeats(this.smallSplashExitAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashExitAudioClipIndex);
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x00044724 File Offset: 0x00042924
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 vector = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - vector;
		}
		if ((Time.time - this.startTime) / this.lifeTime >= 1f)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x000447EC File Offset: 0x000429EC
	private void DeactivateParticleSystems(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x00044818 File Offset: 0x00042A18
	private void PlayParticleEffects(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(true);
				particleSystems[i].Play();
			}
		}
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x0004484C File Offset: 0x00042A4C
	private void SetParticleEffectParameters(ParticleSystem[] particleSystems, float scale, float baseGravMultiplier, float baseStartSpeed, float baseSimulationSpeed, WaterVolume waterVolume = null)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				ParticleSystem.MainModule main = particleSystems[i].main;
				main.startSpeed = baseStartSpeed;
				main.gravityModifier = baseGravMultiplier;
				if (scale < 0.99f)
				{
					main.startSpeed = baseStartSpeed * scale * 2f;
					main.gravityModifier = baseGravMultiplier * scale * 0.5f;
				}
				if (waterVolume != null && waterVolume.Parameters != null)
				{
					particleSystems[i].colorBySpeed.color = waterVolume.Parameters.splashColorBySpeedGradient;
				}
			}
		}
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x00044904 File Offset: 0x00042B04
	private void PlayRandomAudioClipWithoutRepeats(AudioClip[] audioClips, ref int lastPlayedAudioClipIndex)
	{
		if (this.audioSource != null && audioClips != null && audioClips.Length != 0)
		{
			int num = 0;
			if (audioClips.Length > 1)
			{
				int num2 = Random.Range(0, audioClips.Length);
				if (num2 == lastPlayedAudioClipIndex)
				{
					num2 = ((Random.Range(0f, 1f) > 0.5f) ? ((num2 + 1) % audioClips.Length) : (num2 - 1));
					if (num2 < 0)
					{
						num2 = audioClips.Length - 1;
					}
				}
				num = num2;
			}
			lastPlayedAudioClipIndex = num;
			this.audioSource.clip = audioClips[num];
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x0400105E RID: 4190
	private static int lastPlayedBigSplashAudioClipIndex = -1;

	// Token: 0x0400105F RID: 4191
	private static int lastPlayedSmallSplashEntryAudioClipIndex = -1;

	// Token: 0x04001060 RID: 4192
	private static int lastPlayedSmallSplashExitAudioClipIndex = -1;

	// Token: 0x04001061 RID: 4193
	public ParticleSystem[] bigSplashParticleSystems;

	// Token: 0x04001062 RID: 4194
	public ParticleSystem[] smallSplashParticleSystems;

	// Token: 0x04001063 RID: 4195
	public float bigSplashBaseGravityMultiplier = 0.9f;

	// Token: 0x04001064 RID: 4196
	public float bigSplashBaseStartSpeed = 1.9f;

	// Token: 0x04001065 RID: 4197
	public float bigSplashBaseSimulationSpeed = 0.9f;

	// Token: 0x04001066 RID: 4198
	public float smallSplashBaseGravityMultiplier = 0.6f;

	// Token: 0x04001067 RID: 4199
	public float smallSplashBaseStartSpeed = 0.6f;

	// Token: 0x04001068 RID: 4200
	public float smallSplashBaseSimulationSpeed = 0.6f;

	// Token: 0x04001069 RID: 4201
	public float lifeTime = 1f;

	// Token: 0x0400106A RID: 4202
	private float startTime = -1f;

	// Token: 0x0400106B RID: 4203
	public AudioSource audioSource;

	// Token: 0x0400106C RID: 4204
	public AudioClip[] bigSplashAudioClips;

	// Token: 0x0400106D RID: 4205
	public AudioClip[] smallSplashEntryAudioClips;

	// Token: 0x0400106E RID: 4206
	public AudioClip[] smallSplashExitAudioClips;

	// Token: 0x0400106F RID: 4207
	private WaterVolume waterVolume;
}
