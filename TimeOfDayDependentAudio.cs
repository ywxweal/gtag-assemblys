using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class TimeOfDayDependentAudio : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x06000CBA RID: 3258 RVA: 0x000425C4 File Offset: 0x000407C4
	private void Awake()
	{
		this.stepTime = 1f;
		if (this.myParticleSystem != null)
		{
			this.myEmissionModule = this.myParticleSystem.emission;
			this.startingEmissionRate = this.myEmissionModule.rateOverTime.constant;
		}
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x00042614 File Offset: 0x00040814
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
		base.StopAllCoroutines();
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x00042623 File Offset: 0x00040823
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
		base.StartCoroutine(this.UpdateTimeOfDay());
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x00042639 File Offset: 0x00040839
	public void SliceUpdate()
	{
		this.isModified = false;
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00042642 File Offset: 0x00040842
	private IEnumerator UpdateTimeOfDay()
	{
		yield return 0;
		for (;;)
		{
			if (BetterDayNightManager.instance != null)
			{
				if (this.isModified)
				{
					this.positionMultiplier = this.positionMultiplierSet;
				}
				else
				{
					this.positionMultiplier = 1f;
				}
				if (this.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.CurrentWeather() == this.myWeather || BetterDayNightManager.instance.NextWeather() == this.myWeather)
				{
					if (!this.dependentStuff.activeSelf && (!this.includesAudio || this.dependentStuff != this.timeOfDayDependent))
					{
						this.dependentStuff.SetActive(true);
					}
					if (this.includesAudio)
					{
						if (this.timeOfDayDependent != null)
						{
							if (this.volumes[BetterDayNightManager.instance.currentTimeIndex] == 0f)
							{
								if (this.timeOfDayDependent.activeSelf)
								{
									this.timeOfDayDependent.SetActive(false);
								}
							}
							else if (!this.timeOfDayDependent.activeSelf)
							{
								this.timeOfDayDependent.SetActive(true);
							}
						}
						if (this.volumes[BetterDayNightManager.instance.currentTimeIndex] != this.audioSources[0].volume)
						{
							if (BetterDayNightManager.instance.currentLerp < 0.05f)
							{
								this.currentVolume = Mathf.Lerp(this.currentVolume, this.volumes[BetterDayNightManager.instance.currentTimeIndex], BetterDayNightManager.instance.currentLerp * 20f);
							}
							else
							{
								this.currentVolume = this.volumes[BetterDayNightManager.instance.currentTimeIndex];
							}
						}
					}
					if (this.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.CurrentWeather() == this.myWeather)
					{
						if (this.myWeather == BetterDayNightManager.WeatherType.All || BetterDayNightManager.instance.NextWeather() == this.myWeather)
						{
							if (this.myParticleSystem != null)
							{
								this.newRate = this.startingEmissionRate;
							}
							if (this.includesAudio && this.myParticleSystem != null)
							{
								this.currentVolume = Mathf.Lerp(this.volumes[BetterDayNightManager.instance.currentTimeIndex], this.volumes[(BetterDayNightManager.instance.currentTimeIndex + 1) % this.volumes.Length], BetterDayNightManager.instance.currentLerp);
							}
							else if (this.includesAudio)
							{
								if (BetterDayNightManager.instance.currentLerp < 0.05f)
								{
									this.currentVolume = Mathf.Lerp(this.currentVolume, this.volumes[BetterDayNightManager.instance.currentTimeIndex], BetterDayNightManager.instance.currentLerp * 20f);
								}
								else
								{
									this.currentVolume = this.volumes[BetterDayNightManager.instance.currentTimeIndex];
								}
							}
						}
						else
						{
							if (this.myParticleSystem != null)
							{
								this.newRate = ((BetterDayNightManager.instance.currentLerp < 0.5f) ? Mathf.Lerp(this.startingEmissionRate, 0f, BetterDayNightManager.instance.currentLerp * 2f) : 0f);
							}
							if (this.includesAudio)
							{
								this.currentVolume = ((BetterDayNightManager.instance.currentLerp < 0.5f) ? Mathf.Lerp(this.volumes[BetterDayNightManager.instance.currentTimeIndex], 0f, BetterDayNightManager.instance.currentLerp * 2f) : 0f);
							}
						}
					}
					else
					{
						if (this.myParticleSystem != null)
						{
							this.newRate = ((BetterDayNightManager.instance.currentLerp > 0.5f) ? Mathf.Lerp(0f, this.startingEmissionRate, (BetterDayNightManager.instance.currentLerp - 0.5f) * 2f) : 0f);
						}
						if (this.includesAudio)
						{
							this.currentVolume = ((BetterDayNightManager.instance.currentLerp > 0.5f) ? Mathf.Lerp(0f, this.volumes[(BetterDayNightManager.instance.currentTimeIndex + 1) % this.volumes.Length], (BetterDayNightManager.instance.currentLerp - 0.5f) * 2f) : 0f);
						}
					}
					if (this.myParticleSystem != null)
					{
						this.myEmissionModule = this.myParticleSystem.emission;
						this.myEmissionModule.rateOverTime = this.newRate;
					}
					if (this.includesAudio)
					{
						for (int i = 0; i < this.audioSources.Length; i++)
						{
							MusicSource component = this.audioSources[i].gameObject.GetComponent<MusicSource>();
							if (!(component != null) || !component.VolumeOverridden)
							{
								this.audioSources[i].volume = this.currentVolume * this.positionMultiplier;
								this.audioSources[i].enabled = this.currentVolume != 0f;
							}
						}
					}
				}
				else if (this.dependentStuff.activeSelf)
				{
					this.dependentStuff.SetActive(false);
				}
			}
			yield return new WaitForSeconds(this.stepTime);
		}
		yield break;
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x00042654 File Offset: 0x00040854
	public bool BuildValidationCheck()
	{
		for (int i = 0; i < this.audioSources.Length; i++)
		{
			if (this.audioSources[i] == null)
			{
				Debug.LogError("audio source array contains null references", this);
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04000F35 RID: 3893
	public AudioSource[] audioSources;

	// Token: 0x04000F36 RID: 3894
	public float[] volumes;

	// Token: 0x04000F37 RID: 3895
	public float currentVolume;

	// Token: 0x04000F38 RID: 3896
	public float stepTime;

	// Token: 0x04000F39 RID: 3897
	public BetterDayNightManager.WeatherType myWeather;

	// Token: 0x04000F3A RID: 3898
	public GameObject dependentStuff;

	// Token: 0x04000F3B RID: 3899
	public GameObject timeOfDayDependent;

	// Token: 0x04000F3C RID: 3900
	public bool includesAudio;

	// Token: 0x04000F3D RID: 3901
	public ParticleSystem myParticleSystem;

	// Token: 0x04000F3E RID: 3902
	private float startingEmissionRate;

	// Token: 0x04000F3F RID: 3903
	private int lastEmission;

	// Token: 0x04000F40 RID: 3904
	private int nextEmission;

	// Token: 0x04000F41 RID: 3905
	private ParticleSystem.MinMaxCurve newCurve;

	// Token: 0x04000F42 RID: 3906
	private ParticleSystem.EmissionModule myEmissionModule;

	// Token: 0x04000F43 RID: 3907
	private float newRate;

	// Token: 0x04000F44 RID: 3908
	public float positionMultiplierSet;

	// Token: 0x04000F45 RID: 3909
	public float positionMultiplier = 1f;

	// Token: 0x04000F46 RID: 3910
	public bool isModified;
}
