using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020006CA RID: 1738
public class BetterDayNightManager : MonoBehaviour, IGorillaSliceableSimple, ITimeOfDaySystem
{
	// Token: 0x06002B54 RID: 11092 RVA: 0x000D56C0 File Offset: 0x000D38C0
	public static void Register(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Add(data);
	}

	// Token: 0x06002B55 RID: 11093 RVA: 0x000D56CD File Offset: 0x000D38CD
	public static void Unregister(PerSceneRenderData data)
	{
		BetterDayNightManager.allScenesRenderData.Remove(data);
	}

	// Token: 0x17000440 RID: 1088
	// (get) Token: 0x06002B56 RID: 11094 RVA: 0x000D56DB File Offset: 0x000D38DB
	// (set) Token: 0x06002B57 RID: 11095 RVA: 0x000D56E3 File Offset: 0x000D38E3
	public string currentTimeOfDay { get; private set; }

	// Token: 0x17000441 RID: 1089
	// (get) Token: 0x06002B58 RID: 11096 RVA: 0x000D56EC File Offset: 0x000D38EC
	public float NormalizedTimeOfDay
	{
		get
		{
			return Mathf.Clamp01((float)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds / this.totalSeconds));
		}
	}

	// Token: 0x17000442 RID: 1090
	// (get) Token: 0x06002B59 RID: 11097 RVA: 0x000D5716 File Offset: 0x000D3916
	double ITimeOfDaySystem.currentTimeInSeconds
	{
		get
		{
			return this.currentTime;
		}
	}

	// Token: 0x17000443 RID: 1091
	// (get) Token: 0x06002B5A RID: 11098 RVA: 0x000D571E File Offset: 0x000D391E
	double ITimeOfDaySystem.totalTimeInSeconds
	{
		get
		{
			return this.totalSeconds;
		}
	}

	// Token: 0x06002B5B RID: 11099 RVA: 0x000D5728 File Offset: 0x000D3928
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (BetterDayNightManager.instance == null)
		{
			BetterDayNightManager.instance = this;
		}
		else if (BetterDayNightManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.currentLerp = 0f;
		this.totalHours = 0.0;
		for (int i = 0; i < this.timeOfDayRange.Length; i++)
		{
			this.totalHours += this.timeOfDayRange[i];
		}
		this.totalSeconds = this.totalHours * 60.0 * 60.0;
		this.currentTimeIndex = 0;
		this.baseSeconds = 0.0;
		this.computerInit = false;
		this.randomNumberGenerator = new Random(this.mySeed);
		this.GenerateWeatherEventTimes();
		this.ChangeMaps(0, 1);
		base.StartCoroutine(this.UpdateTimeOfDay());
	}

	// Token: 0x06002B5C RID: 11100 RVA: 0x000D581A File Offset: 0x000D3A1A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		base.StopAllCoroutines();
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x000023F4 File Offset: 0x000005F4
	protected void OnDestroy()
	{
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x000D582C File Offset: 0x000D3A2C
	private Vector4 MaterialColorCorrection(Vector4 color)
	{
		if (color.x < 0.5f)
		{
			color.x += 3E-08f;
		}
		if (color.y < 0.5f)
		{
			color.y += 3E-08f;
		}
		if (color.z < 0.5f)
		{
			color.z += 3E-08f;
		}
		if (color.w < 0.5f)
		{
			color.w += 3E-08f;
		}
		return color;
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x000D58AE File Offset: 0x000D3AAE
	private IEnumerator UpdateTimeOfDay()
	{
		yield return 0;
		for (;;)
		{
			if (this.animatingLightFlash != null)
			{
				yield return new WaitForSeconds(this.currentTimestep);
			}
			else
			{
				try
				{
					if (!this.computerInit && GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
					{
						this.computerInit = true;
						this.initialDayCycles = (long)(TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier / this.totalSeconds);
						this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
						this.baseSeconds = TimeSpan.FromMilliseconds((double)GorillaComputer.instance.startupMillis).TotalSeconds * this.timeMultiplier % this.totalSeconds;
						this.currentTime = (this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds;
						this.currentIndexSeconds = 0.0;
						for (int i = 0; i < this.timeOfDayRange.Length; i++)
						{
							this.currentIndexSeconds += this.timeOfDayRange[i] * 3600.0;
							if (this.currentIndexSeconds > this.currentTime)
							{
								this.currentTimeIndex = i;
								break;
							}
						}
						this.currentWeatherIndex += this.currentTimeIndex;
					}
					else if (!this.computerInit && this.baseSeconds == 0.0)
					{
						this.initialDayCycles = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier / this.totalSeconds);
						this.currentWeatherIndex = (int)(this.initialDayCycles * (long)this.dayNightLightmapNames.Length) % this.weatherCycle.Length;
						this.baseSeconds = TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalSeconds * this.timeMultiplier % this.totalSeconds;
						this.currentTime = this.baseSeconds % this.totalSeconds;
						this.currentIndexSeconds = 0.0;
						for (int j = 0; j < this.timeOfDayRange.Length; j++)
						{
							this.currentIndexSeconds += this.timeOfDayRange[j] * 3600.0;
							if (this.currentIndexSeconds > this.currentTime)
							{
								this.currentTimeIndex = j;
								break;
							}
						}
						this.currentWeatherIndex += this.currentTimeIndex - 1;
						if (this.currentWeatherIndex < 0)
						{
							this.currentWeatherIndex = this.weatherCycle.Length - 1;
						}
					}
					this.currentTime = ((this.currentSetting == TimeSettings.Normal) ? ((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) % this.totalSeconds) : this.currentTime);
					this.currentIndexSeconds = 0.0;
					for (int k = 0; k < this.timeOfDayRange.Length; k++)
					{
						this.currentIndexSeconds += this.timeOfDayRange[k] * 3600.0;
						if (this.currentIndexSeconds > this.currentTime)
						{
							this.currentTimeIndex = k;
							break;
						}
					}
					if (this.timeIndexOverrideFunc != null)
					{
						this.currentTimeIndex = this.timeIndexOverrideFunc(this.currentTimeIndex);
					}
					if (this.currentTimeIndex != this.lastIndex)
					{
						this.currentWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
						this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
					}
					this.currentLerp = (float)(1.0 - (this.currentIndexSeconds - this.currentTime) / (this.timeOfDayRange[this.currentTimeIndex] * 3600.0));
					this.ChangeLerps(this.currentLerp);
					this.lastIndex = this.currentTimeIndex;
					this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
				}
				catch (Exception ex)
				{
					string text = "Error in BetterDayNightManager: ";
					Exception ex2 = ex;
					Debug.LogError(text + ((ex2 != null) ? ex2.ToString() : null), this);
				}
				this.gameEpochDay = (long)((this.baseSeconds + (double)Time.realtimeSinceStartup * this.timeMultiplier) / this.totalSeconds + (double)this.initialDayCycles);
				foreach (BetterDayNightManager.ScheduledEvent scheduledEvent in BetterDayNightManager.scheduledEvents.Values)
				{
					if (scheduledEvent.lastDayCalled != this.gameEpochDay && scheduledEvent.hour == this.currentTimeIndex)
					{
						scheduledEvent.lastDayCalled = this.gameEpochDay;
						scheduledEvent.action();
					}
				}
				yield return new WaitForSeconds(this.currentTimestep);
			}
		}
		yield break;
	}

	// Token: 0x06002B60 RID: 11104 RVA: 0x000D58C0 File Offset: 0x000D3AC0
	private void ChangeLerps(float newLerp)
	{
		Shader.SetGlobalFloat(this._GlobalDayNightLerpValue, newLerp);
		for (int i = 0; i < this.standardMaterialsUnlit.Length; i++)
		{
			this.tempLerp = Mathf.Lerp(this.colorFrom, this.colorTo, newLerp);
			this.standardMaterialsUnlit[i].color = new Color(this.tempLerp, this.tempLerp, this.tempLerp);
		}
		for (int j = 0; j < this.standardMaterialsUnlitDarker.Length; j++)
		{
			this.tempLerp = Mathf.Lerp(this.colorFromDarker, this.colorToDarker, newLerp);
			Color.RGBToHSV(this.standardMaterialsUnlitDarker[j].color, out this.h, out this.s, out this.v);
			this.standardMaterialsUnlitDarker[j].color = Color.HSVToRGB(this.h, this.s, this.tempLerp);
		}
	}

	// Token: 0x06002B61 RID: 11105 RVA: 0x000D59A0 File Offset: 0x000D3BA0
	private void ChangeMaps(int fromIndex, int toIndex)
	{
		this.fromWeatherIndex = this.currentWeatherIndex;
		this.toWeatherIndex = (this.currentWeatherIndex + 1) % this.weatherCycle.Length;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.fromSky = this.dayNightWeatherSkyboxTextures[fromIndex];
		}
		else
		{
			this.fromSky = this.dayNightSkyboxTextures[fromIndex];
		}
		this.fromSky2 = this.cloudsDayNightSkyboxTextures[fromIndex];
		this.fromSky3 = this.beachDayNightSkyboxTextures[fromIndex];
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			this.toSky = this.dayNightWeatherSkyboxTextures[toIndex];
		}
		else
		{
			this.toSky = this.dayNightSkyboxTextures[toIndex];
		}
		this.toSky2 = this.cloudsDayNightSkyboxTextures[toIndex];
		this.toSky3 = this.beachDayNightSkyboxTextures[toIndex];
		this.PopulateAllLightmaps(fromIndex, toIndex);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex1, this.fromSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSkyTex2, this.toSky);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex1, this.fromSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky2Tex2, this.toSky2);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex1, this.fromSky3);
		Shader.SetGlobalTexture(this._GlobalDayNightSky3Tex2, this.toSky3);
		this.colorFrom = this.standardUnlitColor[fromIndex];
		this.colorTo = this.standardUnlitColor[toIndex];
		this.colorFromDarker = this.standardUnlitColorWithPremadeColorDarker[fromIndex];
		this.colorToDarker = this.standardUnlitColorWithPremadeColorDarker[toIndex];
	}

	// Token: 0x06002B62 RID: 11106 RVA: 0x000D5B28 File Offset: 0x000D3D28
	public void SliceUpdate()
	{
		if (!this.shouldRepopulate)
		{
			using (List<PerSceneRenderData>.Enumerator enumerator = BetterDayNightManager.allScenesRenderData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CheckShouldRepopulate())
					{
						this.shouldRepopulate = true;
					}
				}
			}
		}
		if (this.shouldRepopulate)
		{
			this.PopulateAllLightmaps();
			this.shouldRepopulate = false;
		}
	}

	// Token: 0x06002B63 RID: 11107 RVA: 0x000D5BA0 File Offset: 0x000D3DA0
	public void RequestRepopulateLightmaps()
	{
		this.shouldRepopulate = true;
	}

	// Token: 0x06002B64 RID: 11108 RVA: 0x000D5BA9 File Offset: 0x000D3DA9
	public void PopulateAllLightmaps()
	{
		this.PopulateAllLightmaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x06002B65 RID: 11109 RVA: 0x000D5BC8 File Offset: 0x000D3DC8
	public void PopulateAllLightmaps(int fromIndex, int toIndex)
	{
		string text;
		if (this.weatherCycle[this.fromWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			text = this.dayNightWeatherLightmapNames[fromIndex];
		}
		else
		{
			text = this.dayNightLightmapNames[fromIndex];
		}
		string text2;
		if (this.weatherCycle[this.toWeatherIndex] == BetterDayNightManager.WeatherType.Raining)
		{
			text2 = this.dayNightWeatherLightmapNames[toIndex];
		}
		else
		{
			text2 = this.dayNightLightmapNames[toIndex];
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		foreach (PerSceneRenderData perSceneRenderData in BetterDayNightManager.allScenesRenderData)
		{
			perSceneRenderData.PopulateLightmaps(text, text2, lightmaps);
		}
		LightmapSettings.lightmaps = lightmaps;
	}

	// Token: 0x06002B66 RID: 11110 RVA: 0x000D5C70 File Offset: 0x000D3E70
	public BetterDayNightManager.WeatherType CurrentWeather()
	{
		return this.weatherCycle[this.currentWeatherIndex];
	}

	// Token: 0x06002B67 RID: 11111 RVA: 0x000D5C7F File Offset: 0x000D3E7F
	public BetterDayNightManager.WeatherType NextWeather()
	{
		return this.weatherCycle[(this.currentWeatherIndex + 1) % this.weatherCycle.Length];
	}

	// Token: 0x06002B68 RID: 11112 RVA: 0x000D5C99 File Offset: 0x000D3E99
	public BetterDayNightManager.WeatherType LastWeather()
	{
		return this.weatherCycle[(this.currentWeatherIndex - 1) % this.weatherCycle.Length];
	}

	// Token: 0x06002B69 RID: 11113 RVA: 0x000D5CB4 File Offset: 0x000D3EB4
	private void GenerateWeatherEventTimes()
	{
		this.weatherCycle = new BetterDayNightManager.WeatherType[100 * this.dayNightLightmapNames.Length];
		this.rainChance = this.rainChance * 2f / (float)this.maxRainDuration;
		for (int i = 1; i < this.weatherCycle.Length; i++)
		{
			this.weatherCycle[i] = (((float)this.randomNumberGenerator.Next(100) < this.rainChance * 100f) ? BetterDayNightManager.WeatherType.Raining : BetterDayNightManager.WeatherType.None);
			if (this.weatherCycle[i] == BetterDayNightManager.WeatherType.Raining)
			{
				this.rainDuration = this.randomNumberGenerator.Next(1, this.maxRainDuration + 1);
				for (int j = 1; j < this.rainDuration; j++)
				{
					if (i + j < this.weatherCycle.Length)
					{
						this.weatherCycle[i + j] = BetterDayNightManager.WeatherType.Raining;
					}
				}
				i += this.rainDuration - 1;
			}
		}
	}

	// Token: 0x06002B6A RID: 11114 RVA: 0x000D5D8C File Offset: 0x000D3F8C
	public static int RegisterScheduledEvent(int hour, Action action)
	{
		int num = (int)(DateTime.Now.Ticks % 2147483647L);
		while (BetterDayNightManager.scheduledEvents.ContainsKey(num))
		{
			num++;
		}
		BetterDayNightManager.scheduledEvents.Add(num, new BetterDayNightManager.ScheduledEvent
		{
			lastDayCalled = -1L,
			hour = hour,
			action = action
		});
		return num;
	}

	// Token: 0x06002B6B RID: 11115 RVA: 0x000D5DE9 File Offset: 0x000D3FE9
	public static void UnregisterScheduledEvent(int id)
	{
		BetterDayNightManager.scheduledEvents.Remove(id);
	}

	// Token: 0x06002B6C RID: 11116 RVA: 0x000D5DF7 File Offset: 0x000D3FF7
	public void SetTimeIndexOverrideFunction(Func<int, int> overrideFunction)
	{
		this.timeIndexOverrideFunc = overrideFunction;
	}

	// Token: 0x06002B6D RID: 11117 RVA: 0x000D5E00 File Offset: 0x000D4000
	public void UnsetTimeIndexOverrideFunction()
	{
		this.timeIndexOverrideFunc = null;
	}

	// Token: 0x06002B6E RID: 11118 RVA: 0x000D5E0C File Offset: 0x000D400C
	public void SetOverrideIndex(int index)
	{
		this.overrideIndex = index;
		this.currentWeatherIndex = this.overrideIndex;
		this.currentTimeIndex = this.overrideIndex;
		this.currentTimeOfDay = this.dayNightLightmapNames[this.currentTimeIndex];
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
	}

	// Token: 0x06002B6F RID: 11119 RVA: 0x000D5E68 File Offset: 0x000D4068
	public void AnimateLightFlash(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		if (this.animatingLightFlash != null)
		{
			base.StopCoroutine(this.animatingLightFlash);
		}
		this.animatingLightFlash = base.StartCoroutine(this.AnimateLightFlashCo(index, fadeInDuration, holdDuration, fadeOutDuration));
	}

	// Token: 0x06002B70 RID: 11120 RVA: 0x000D5E95 File Offset: 0x000D4095
	private IEnumerator AnimateLightFlashCo(int index, float fadeInDuration, float holdDuration, float fadeOutDuration)
	{
		int startMap = ((this.currentLerp < 0.5f) ? this.currentTimeIndex : ((this.currentTimeIndex + 1) % this.timeOfDayRange.Length));
		this.ChangeMaps(startMap, index);
		float endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(index, index);
		this.ChangeLerps(0f);
		endTimestamp = Time.time + fadeInDuration;
		while (Time.time < endTimestamp)
		{
			yield return null;
		}
		this.ChangeMaps(index, startMap);
		endTimestamp = Time.time + fadeOutDuration;
		while (Time.time < endTimestamp)
		{
			this.ChangeLerps(1f - (endTimestamp - Time.time) / fadeInDuration);
			yield return null;
		}
		this.ChangeMaps(this.currentTimeIndex, (this.currentTimeIndex + 1) % this.timeOfDayRange.Length);
		this.ChangeLerps(this.currentLerp);
		this.animatingLightFlash = null;
		yield break;
	}

	// Token: 0x06002B71 RID: 11121 RVA: 0x000D5EBC File Offset: 0x000D40BC
	public void SetTimeOfDay(int timeIndex)
	{
		double num = 0.0;
		for (int i = 0; i < timeIndex; i++)
		{
			num += this.timeOfDayRange[i];
		}
		this.currentTime = num * 3600.0;
		this.currentSetting = TimeSettings.Static;
	}

	// Token: 0x06002B72 RID: 11122 RVA: 0x000D5F02 File Offset: 0x000D4102
	public void FastForward(float seconds)
	{
		this.baseSeconds += (double)seconds;
	}

	// Token: 0x06002B75 RID: 11125 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04003104 RID: 12548
	[OnEnterPlay_SetNull]
	public static volatile BetterDayNightManager instance;

	// Token: 0x04003105 RID: 12549
	[OnEnterPlay_Clear]
	public static List<PerSceneRenderData> allScenesRenderData = new List<PerSceneRenderData>();

	// Token: 0x04003106 RID: 12550
	public Shader standard;

	// Token: 0x04003107 RID: 12551
	public Shader standardCutout;

	// Token: 0x04003108 RID: 12552
	public Shader gorillaUnlit;

	// Token: 0x04003109 RID: 12553
	public Shader gorillaUnlitCutout;

	// Token: 0x0400310A RID: 12554
	public Material[] standardMaterialsUnlit;

	// Token: 0x0400310B RID: 12555
	public Material[] standardMaterialsUnlitDarker;

	// Token: 0x0400310C RID: 12556
	public Material[] dayNightSupportedMaterials;

	// Token: 0x0400310D RID: 12557
	public Material[] dayNightSupportedMaterialsCutout;

	// Token: 0x0400310E RID: 12558
	public string[] dayNightLightmapNames;

	// Token: 0x0400310F RID: 12559
	public string[] dayNightWeatherLightmapNames;

	// Token: 0x04003110 RID: 12560
	public Texture2D[] dayNightSkyboxTextures;

	// Token: 0x04003111 RID: 12561
	public Texture2D[] cloudsDayNightSkyboxTextures;

	// Token: 0x04003112 RID: 12562
	public Texture2D[] beachDayNightSkyboxTextures;

	// Token: 0x04003113 RID: 12563
	public Texture2D[] dayNightWeatherSkyboxTextures;

	// Token: 0x04003114 RID: 12564
	public float[] standardUnlitColor;

	// Token: 0x04003115 RID: 12565
	public float[] standardUnlitColorWithPremadeColorDarker;

	// Token: 0x04003116 RID: 12566
	public float currentLerp;

	// Token: 0x04003117 RID: 12567
	public float currentTimestep;

	// Token: 0x04003118 RID: 12568
	public double[] timeOfDayRange;

	// Token: 0x04003119 RID: 12569
	public double timeMultiplier;

	// Token: 0x0400311A RID: 12570
	private float lastTime;

	// Token: 0x0400311B RID: 12571
	private double currentTime;

	// Token: 0x0400311C RID: 12572
	private double totalHours;

	// Token: 0x0400311D RID: 12573
	private double totalSeconds;

	// Token: 0x0400311E RID: 12574
	private float colorFrom;

	// Token: 0x0400311F RID: 12575
	private float colorTo;

	// Token: 0x04003120 RID: 12576
	private float colorFromDarker;

	// Token: 0x04003121 RID: 12577
	private float colorToDarker;

	// Token: 0x04003122 RID: 12578
	public int currentTimeIndex;

	// Token: 0x04003123 RID: 12579
	public int currentWeatherIndex;

	// Token: 0x04003124 RID: 12580
	private int lastIndex;

	// Token: 0x04003125 RID: 12581
	private double currentIndexSeconds;

	// Token: 0x04003126 RID: 12582
	private float tempLerp;

	// Token: 0x04003127 RID: 12583
	private double baseSeconds;

	// Token: 0x04003128 RID: 12584
	private bool computerInit;

	// Token: 0x04003129 RID: 12585
	private float h;

	// Token: 0x0400312A RID: 12586
	private float s;

	// Token: 0x0400312B RID: 12587
	private float v;

	// Token: 0x0400312C RID: 12588
	public int mySeed;

	// Token: 0x0400312D RID: 12589
	public Random randomNumberGenerator = new Random();

	// Token: 0x0400312E RID: 12590
	public BetterDayNightManager.WeatherType[] weatherCycle;

	// Token: 0x04003130 RID: 12592
	public float rainChance = 0.3f;

	// Token: 0x04003131 RID: 12593
	public int maxRainDuration = 5;

	// Token: 0x04003132 RID: 12594
	private int rainDuration;

	// Token: 0x04003133 RID: 12595
	private float remainingSeconds;

	// Token: 0x04003134 RID: 12596
	private long initialDayCycles;

	// Token: 0x04003135 RID: 12597
	private long gameEpochDay;

	// Token: 0x04003136 RID: 12598
	private int currentWeatherCycle;

	// Token: 0x04003137 RID: 12599
	private int fromWeatherIndex;

	// Token: 0x04003138 RID: 12600
	private int toWeatherIndex;

	// Token: 0x04003139 RID: 12601
	private Texture2D fromSky;

	// Token: 0x0400313A RID: 12602
	private Texture2D fromSky2;

	// Token: 0x0400313B RID: 12603
	private Texture2D fromSky3;

	// Token: 0x0400313C RID: 12604
	private Texture2D toSky;

	// Token: 0x0400313D RID: 12605
	private Texture2D toSky2;

	// Token: 0x0400313E RID: 12606
	private Texture2D toSky3;

	// Token: 0x0400313F RID: 12607
	public AddCollidersToParticleSystemTriggers[] weatherSystems;

	// Token: 0x04003140 RID: 12608
	public List<Collider> collidersToAddToWeatherSystems = new List<Collider>();

	// Token: 0x04003141 RID: 12609
	private Func<int, int> timeIndexOverrideFunc;

	// Token: 0x04003142 RID: 12610
	public int overrideIndex = -1;

	// Token: 0x04003143 RID: 12611
	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, BetterDayNightManager.ScheduledEvent> scheduledEvents = new Dictionary<int, BetterDayNightManager.ScheduledEvent>(256);

	// Token: 0x04003144 RID: 12612
	public TimeSettings currentSetting;

	// Token: 0x04003145 RID: 12613
	private ShaderHashId _Color = "_Color";

	// Token: 0x04003146 RID: 12614
	private ShaderHashId _GlobalDayNightLerpValue = "_GlobalDayNightLerpValue";

	// Token: 0x04003147 RID: 12615
	private ShaderHashId _GlobalDayNightSkyTex1 = "_GlobalDayNightSkyTex1";

	// Token: 0x04003148 RID: 12616
	private ShaderHashId _GlobalDayNightSkyTex2 = "_GlobalDayNightSkyTex2";

	// Token: 0x04003149 RID: 12617
	private ShaderHashId _GlobalDayNightSky2Tex1 = "_GlobalDayNightSky2Tex1";

	// Token: 0x0400314A RID: 12618
	private ShaderHashId _GlobalDayNightSky2Tex2 = "_GlobalDayNightSky2Tex2";

	// Token: 0x0400314B RID: 12619
	private ShaderHashId _GlobalDayNightSky3Tex1 = "_GlobalDayNightSky3Tex1";

	// Token: 0x0400314C RID: 12620
	private ShaderHashId _GlobalDayNightSky3Tex2 = "_GlobalDayNightSky3Tex2";

	// Token: 0x0400314D RID: 12621
	private bool shouldRepopulate;

	// Token: 0x0400314E RID: 12622
	private Coroutine animatingLightFlash;

	// Token: 0x020006CB RID: 1739
	public enum WeatherType
	{
		// Token: 0x04003150 RID: 12624
		None,
		// Token: 0x04003151 RID: 12625
		Raining,
		// Token: 0x04003152 RID: 12626
		All
	}

	// Token: 0x020006CC RID: 1740
	private class ScheduledEvent
	{
		// Token: 0x04003153 RID: 12627
		public long lastDayCalled;

		// Token: 0x04003154 RID: 12628
		public int hour;

		// Token: 0x04003155 RID: 12629
		public Action action;
	}
}
