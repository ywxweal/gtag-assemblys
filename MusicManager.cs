using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200069A RID: 1690
public class MusicManager : MonoBehaviour
{
	// Token: 0x06002A49 RID: 10825 RVA: 0x000D0CFF File Offset: 0x000CEEFF
	private void Awake()
	{
		if (MusicManager.Instance == null)
		{
			MusicManager.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06002A4A RID: 10826 RVA: 0x000D0D1F File Offset: 0x000CEF1F
	public void RegisterMusicSource(MusicSource musicSource)
	{
		if (!this.activeSources.Contains(musicSource))
		{
			this.activeSources.Add(musicSource);
		}
	}

	// Token: 0x06002A4B RID: 10827 RVA: 0x000D0D3C File Offset: 0x000CEF3C
	public void UnregisterMusicSource(MusicSource musicSource)
	{
		if (this.activeSources.Contains(musicSource))
		{
			this.activeSources.Remove(musicSource);
			musicSource.UnsetVolumeOverride();
		}
	}

	// Token: 0x06002A4C RID: 10828 RVA: 0x000D0D60 File Offset: 0x000CEF60
	public void FadeOutMusic(float duration = 3f)
	{
		base.StopAllCoroutines();
		if (duration > 0f)
		{
			base.StartCoroutine(this.FadeOutVolumeCoroutine(duration));
			return;
		}
		foreach (MusicSource musicSource in this.activeSources)
		{
			musicSource.SetVolumeOverride(0f);
		}
	}

	// Token: 0x06002A4D RID: 10829 RVA: 0x000D0DD4 File Offset: 0x000CEFD4
	public void FadeInMusic(float duration = 3f)
	{
		base.StopAllCoroutines();
		if (duration > 0f)
		{
			base.StartCoroutine(this.FadeInVolumeCoroutine(duration));
			return;
		}
		foreach (MusicSource musicSource in this.activeSources)
		{
			musicSource.UnsetVolumeOverride();
		}
	}

	// Token: 0x06002A4E RID: 10830 RVA: 0x000D0E44 File Offset: 0x000CF044
	private IEnumerator FadeInVolumeCoroutine(float duration)
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			float deltaTime = Time.deltaTime;
			foreach (MusicSource musicSource in this.activeSources)
			{
				float num = musicSource.DefaultVolume / duration;
				float num2 = Mathf.MoveTowards(musicSource.AudioSource.volume, musicSource.DefaultVolume, num * deltaTime);
				musicSource.SetVolumeOverride(num2);
				if (musicSource.AudioSource.volume != musicSource.DefaultVolume)
				{
					complete = false;
				}
			}
			yield return null;
		}
		using (HashSet<MusicSource>.Enumerator enumerator = this.activeSources.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MusicSource musicSource2 = enumerator.Current;
				musicSource2.UnsetVolumeOverride();
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x06002A4F RID: 10831 RVA: 0x000D0E5A File Offset: 0x000CF05A
	private IEnumerator FadeOutVolumeCoroutine(float duration)
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			float deltaTime = Time.deltaTime;
			foreach (MusicSource musicSource in this.activeSources)
			{
				float num = musicSource.DefaultVolume / duration;
				float num2 = Mathf.MoveTowards(musicSource.AudioSource.volume, 0f, num * deltaTime);
				musicSource.SetVolumeOverride(num2);
				if (musicSource.AudioSource.volume != 0f)
				{
					complete = false;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04002F42 RID: 12098
	[OnEnterPlay_SetNull]
	public static volatile MusicManager Instance;

	// Token: 0x04002F43 RID: 12099
	private HashSet<MusicSource> activeSources = new HashSet<MusicSource>();
}
