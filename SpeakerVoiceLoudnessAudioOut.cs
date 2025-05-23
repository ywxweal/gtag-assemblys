using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200021B RID: 539
public class SpeakerVoiceLoudnessAudioOut : UnityAudioOut
{
	// Token: 0x06000C8A RID: 3210 RVA: 0x00041A6C File Offset: 0x0003FC6C
	public SpeakerVoiceLoudnessAudioOut(SpeakerVoiceToLoudness speaker, AudioSource audioSource, AudioOutDelayControl.PlayDelayConfig playDelayConfig, Photon.Voice.ILogger logger, string logPrefix, bool debugInfo)
		: base(audioSource, playDelayConfig, logger, logPrefix, debugInfo)
	{
		this.voiceToLoudness = speaker;
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x00041A84 File Offset: 0x0003FC84
	public override void OutWrite(float[] data, int offsetSamples)
	{
		float num = 0f;
		for (int i = 0; i < data.Length; i++)
		{
			float num2 = data[i];
			if (!float.IsFinite(num2))
			{
				num2 = 0f;
				data[i] = num2;
			}
			else if (num2 > 1f)
			{
				num2 = 1f;
				data[i] = num2;
			}
			else if (num2 < -1f)
			{
				num2 = -1f;
				data[i] = num2;
			}
			num += Mathf.Abs(num2);
		}
		if (num > 0f)
		{
			this.voiceToLoudness.loudness = num / (float)data.Length;
		}
		base.OutWrite(data, offsetSamples);
	}

	// Token: 0x04000F17 RID: 3863
	private SpeakerVoiceToLoudness voiceToLoudness;
}
