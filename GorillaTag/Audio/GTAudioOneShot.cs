using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D8A RID: 3466
	internal static class GTAudioOneShot
	{
		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x06005614 RID: 22036 RVA: 0x001A3107 File Offset: 0x001A1307
		// (set) Token: 0x06005615 RID: 22037 RVA: 0x001A310E File Offset: 0x001A130E
		[OnEnterPlay_Set(false)]
		internal static bool isInitialized { get; private set; }

		// Token: 0x06005616 RID: 22038 RVA: 0x001A3118 File Offset: 0x001A1318
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (GTAudioOneShot.isInitialized)
			{
				return;
			}
			AudioSource audioSource = Resources.Load<AudioSource>("AudioSourceSingleton_Prefab");
			if (audioSource == null)
			{
				Debug.LogError("GTAudioOneShot: Failed to load AudioSourceSingleton_Prefab from resources!!!");
				return;
			}
			GTAudioOneShot.audioSource = Object.Instantiate<AudioSource>(audioSource);
			GTAudioOneShot.defaultCurve = GTAudioOneShot.audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
			Object.DontDestroyOnLoad(GTAudioOneShot.audioSource);
			GTAudioOneShot.isInitialized = true;
		}

		// Token: 0x06005617 RID: 22039 RVA: 0x001A3177 File Offset: 0x001A1377
		internal static void Play(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.pitch = pitch;
			GTAudioOneShot.audioSource.transform.position = position;
			GTAudioOneShot.audioSource.GTPlayOneShot(clip, volume);
		}

		// Token: 0x06005618 RID: 22040 RVA: 0x001A31AF File Offset: 0x001A13AF
		internal static void Play(AudioClip clip, Vector3 position, AnimationCurve curve, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
			GTAudioOneShot.Play(clip, position, volume, pitch);
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, GTAudioOneShot.defaultCurve);
		}

		// Token: 0x040059E7 RID: 23015
		[OnEnterPlay_SetNull]
		internal static AudioSource audioSource;

		// Token: 0x040059E8 RID: 23016
		[OnEnterPlay_SetNull]
		internal static AnimationCurve defaultCurve;
	}
}
