using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public static class GTAudioSourceExtensions
{
	// Token: 0x06000AB4 RID: 2740 RVA: 0x0003A567 File Offset: 0x00038767
	public static void GTPlayOneShot(this AudioSource sound, IList<AudioClip> clips, float volumeScale = 1f)
	{
		sound.PlayOneShot(clips[Random.Range(0, clips.Count)], volumeScale);
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0003A582 File Offset: 0x00038782
	public static void GTPlayOneShot(this AudioSource audioSource, AudioClip clip, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clip, volumeScale);
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0003A58C File Offset: 0x0003878C
	public static void GTPlay(this AudioSource audioSource)
	{
		audioSource.Play();
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x0003A594 File Offset: 0x00038794
	public static void GTPlay(this AudioSource audioSource, ulong delay)
	{
		audioSource.Play(delay);
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x0003A59D File Offset: 0x0003879D
	public static void GTPause(this AudioSource audioSource)
	{
		audioSource.Pause();
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x0003A5A5 File Offset: 0x000387A5
	public static void GTUnPause(this AudioSource audioSource)
	{
		audioSource.UnPause();
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x0003A5AD File Offset: 0x000387AD
	public static void GTStop(this AudioSource audioSource)
	{
		audioSource.Stop();
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x0003A5B5 File Offset: 0x000387B5
	public static void GTPlayDelayed(this AudioSource audioSource, float delay)
	{
		audioSource.PlayDelayed(delay);
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x0003A5BE File Offset: 0x000387BE
	public static void GTPlayScheduled(this AudioSource audioSource, double time)
	{
		audioSource.PlayScheduled(time);
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0003A5C7 File Offset: 0x000387C7
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position)
	{
		AudioSource.PlayClipAtPoint(clip, position);
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x0003A5D0 File Offset: 0x000387D0
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
	{
		AudioSource.PlayClipAtPoint(clip, position, volume);
	}
}
