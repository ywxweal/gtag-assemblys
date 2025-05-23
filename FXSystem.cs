using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200092C RID: 2348
public static class FXSystem
{
	// Token: 0x0600391F RID: 14623 RVA: 0x00112D40 File Offset: 0x00110F40
	public static void PlayFXForRig(FXType fxType, IFXContext context, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		FXSystemSettings settings = context.settings;
		if (settings.forLocalRig)
		{
			context.OnPlayFX();
			return;
		}
		if (FXSystem.CheckCallSpam(settings, (int)fxType, info.SentServerTime))
		{
			context.OnPlayFX();
		}
	}

	// Token: 0x06003920 RID: 14624 RVA: 0x00112D7C File Offset: 0x00110F7C
	public static void PlayFXForRigValidated(List<int> hashes, FXType fxType, IFXContext context, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		for (int i = 0; i < hashes.Count; i++)
		{
			if (!ObjectPools.instance.DoesPoolExist(hashes[i]))
			{
				return;
			}
		}
		FXSystem.PlayFXForRig(fxType, context, info);
	}

	// Token: 0x06003921 RID: 14625 RVA: 0x00112DB8 File Offset: 0x00110FB8
	public static void PlayFX<T>(FXType fxType, IFXContextParems<T> context, T args, PhotonMessageInfoWrapped info) where T : FXSArgs
	{
		FXSystemSettings settings = context.settings;
		if (settings.forLocalRig)
		{
			context.OnPlayFX(args);
			return;
		}
		if (FXSystem.CheckCallSpam(settings, (int)fxType, info.SentServerTime))
		{
			context.OnPlayFX(args);
		}
	}

	// Token: 0x06003922 RID: 14626 RVA: 0x00112DF4 File Offset: 0x00110FF4
	public static void PlayFXForRig<T>(FXType fxType, IFXEffectContext<T> context, PhotonMessageInfoWrapped info) where T : IFXEffectContextObject
	{
		FXSystemSettings settings = context.settings;
		if (!settings.forLocalRig && !FXSystem.CheckCallSpam(settings, (int)fxType, info.SentServerTime))
		{
			return;
		}
		FXSystem.PlayFX(context.effectContext);
	}

	// Token: 0x06003923 RID: 14627 RVA: 0x00112E34 File Offset: 0x00111034
	public static void PlayFX(IFXEffectContextObject effectContext)
	{
		int[] prefabPoolIds = effectContext.PrefabPoolIds;
		if (prefabPoolIds != null)
		{
			int num = prefabPoolIds.Length;
			for (int i = 0; i < num; i++)
			{
				int num2 = prefabPoolIds[i];
				if (num2 != -1)
				{
					GameObject gameObject = ObjectPools.instance.Instantiate(num2, effectContext.Positon, effectContext.Rotation, false);
					gameObject.SetActive(true);
					effectContext.OnPlayVisualFX(num2, gameObject);
				}
			}
		}
		AudioSource soundSource = effectContext.SoundSource;
		if (soundSource.IsNull())
		{
			return;
		}
		soundSource.volume = effectContext.Volume;
		AudioClip sound = effectContext.Sound;
		if (sound.IsNotNull())
		{
			soundSource.GTPlayOneShot(sound, 1f);
			effectContext.OnPlaySoundFX(soundSource);
		}
	}

	// Token: 0x06003924 RID: 14628 RVA: 0x00112ED8 File Offset: 0x001110D8
	public static bool CheckCallSpam(FXSystemSettings settings, int index, double serverTime)
	{
		CallLimitType<CallLimiter> callLimitType = settings.callSettings[index];
		if (!callLimitType.UseNetWorkTime)
		{
			return callLimitType.CallLimitSettings.CheckCallTime(Time.time);
		}
		return callLimitType.CallLimitSettings.CheckCallServerTime(serverTime);
	}
}
