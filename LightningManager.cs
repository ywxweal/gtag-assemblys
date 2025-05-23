using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000852 RID: 2130
public class LightningManager : MonoBehaviour
{
	// Token: 0x060033C3 RID: 13251 RVA: 0x000FF9AA File Offset: 0x000FDBAA
	private void Start()
	{
		this.lightningAudio = base.GetComponent<AudioSource>();
		GorillaComputer instance = GorillaComputer.instance;
		instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
	}

	// Token: 0x060033C4 RID: 13252 RVA: 0x000FF9E0 File Offset: 0x000FDBE0
	private void OnTimeChanged()
	{
		this.InitializeRng();
		if (this.lightningRunner != null)
		{
			base.StopCoroutine(this.lightningRunner);
		}
		this.lightningRunner = base.StartCoroutine(this.LightningEffectRunner());
	}

	// Token: 0x060033C5 RID: 13253 RVA: 0x000FFA10 File Offset: 0x000FDC10
	private void GetHourStart(out long seed, out float timestampRealtime)
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		DateTime dateTime = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, 0, 0);
		timestampRealtime = Time.realtimeSinceStartup - (float)(serverTime - dateTime).TotalSeconds;
		seed = dateTime.Ticks;
	}

	// Token: 0x060033C6 RID: 13254 RVA: 0x000FFA70 File Offset: 0x000FDC70
	private void InitializeRng()
	{
		long num;
		float num2;
		this.GetHourStart(out num, out num2);
		this.currentHourlySeed = num;
		this.rng = new SRand(num);
		this.lightningTimestampsRealtime.Clear();
		this.nextLightningTimestampIndex = -1;
		float num3 = num2;
		float num4 = 0f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (num4 < 3600f)
		{
			float num5 = this.rng.NextFloat(this.minTimeBetweenFlashes, this.maxTimeBetweenFlashes);
			num4 += num5;
			num3 += num5;
			if (this.nextLightningTimestampIndex == -1 && num3 > realtimeSinceStartup)
			{
				this.nextLightningTimestampIndex = this.lightningTimestampsRealtime.Count;
			}
			this.lightningTimestampsRealtime.Add(num3);
		}
		this.lightningTimestampsRealtime[this.lightningTimestampsRealtime.Count - 1] = num2 + 3605f;
	}

	// Token: 0x060033C7 RID: 13255 RVA: 0x000FFB34 File Offset: 0x000FDD34
	private void DoLightningStrike()
	{
		BetterDayNightManager.instance.AnimateLightFlash(this.lightMapIndex, this.flashFadeInDuration, this.flashHoldDuration, this.flashFadeOutDuration);
		this.lightningAudio.clip = (ZoneManagement.IsInZone(GTZone.cave) ? this.muffledLightning : this.regularLightning);
		this.lightningAudio.GTPlay();
	}

	// Token: 0x060033C8 RID: 13256 RVA: 0x000FFB91 File Offset: 0x000FDD91
	private IEnumerator LightningEffectRunner()
	{
		for (;;)
		{
			if (this.lightningTimestampsRealtime.Count <= this.nextLightningTimestampIndex)
			{
				this.InitializeRng();
			}
			if (this.lightningTimestampsRealtime.Count > this.nextLightningTimestampIndex)
			{
				yield return new WaitForSecondsRealtime(this.lightningTimestampsRealtime[this.nextLightningTimestampIndex] - Time.realtimeSinceStartup);
				float num = this.lightningTimestampsRealtime[this.nextLightningTimestampIndex];
				this.nextLightningTimestampIndex++;
				if (Time.realtimeSinceStartup - num < 1f && this.lightningTimestampsRealtime.Count > this.nextLightningTimestampIndex)
				{
					this.DoLightningStrike();
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04003AE1 RID: 15073
	public int lightMapIndex;

	// Token: 0x04003AE2 RID: 15074
	public float minTimeBetweenFlashes;

	// Token: 0x04003AE3 RID: 15075
	public float maxTimeBetweenFlashes;

	// Token: 0x04003AE4 RID: 15076
	public float flashFadeInDuration;

	// Token: 0x04003AE5 RID: 15077
	public float flashHoldDuration;

	// Token: 0x04003AE6 RID: 15078
	public float flashFadeOutDuration;

	// Token: 0x04003AE7 RID: 15079
	private AudioSource lightningAudio;

	// Token: 0x04003AE8 RID: 15080
	private SRand rng;

	// Token: 0x04003AE9 RID: 15081
	private long currentHourlySeed;

	// Token: 0x04003AEA RID: 15082
	private List<float> lightningTimestampsRealtime = new List<float>();

	// Token: 0x04003AEB RID: 15083
	private int nextLightningTimestampIndex;

	// Token: 0x04003AEC RID: 15084
	public AudioClip regularLightning;

	// Token: 0x04003AED RID: 15085
	public AudioClip muffledLightning;

	// Token: 0x04003AEE RID: 15086
	private Coroutine lightningRunner;
}
