using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x02000219 RID: 537
[RequireComponent(typeof(Speaker))]
public class SpeakerVoiceToLoudness : MonoBehaviour
{
	// Token: 0x06000C85 RID: 3205 RVA: 0x00041958 File Offset: 0x0003FB58
	private void Awake()
	{
		Speaker component = base.GetComponent<Speaker>();
		component.CustomAudioOutFactory = this.GetVolumeTracking(component);
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x0004197C File Offset: 0x0003FB7C
	private Func<IAudioOut<float>> GetVolumeTracking(Speaker speaker)
	{
		AudioOutDelayControl.PlayDelayConfig pdc = new AudioOutDelayControl.PlayDelayConfig
		{
			Low = this.playbackDelaySettings.MinDelaySoft,
			High = this.playbackDelaySettings.MaxDelaySoft,
			Max = this.playbackDelaySettings.MaxDelayHard
		};
		return () => new SpeakerVoiceLoudnessAudioOut(this, speaker.GetComponent<AudioSource>(), pdc, speaker.Logger, string.Empty, speaker.Logger.IsDebugEnabled);
	}

	// Token: 0x04000F12 RID: 3858
	[SerializeField]
	private PlaybackDelaySettings playbackDelaySettings = new PlaybackDelaySettings
	{
		MinDelaySoft = 200,
		MaxDelaySoft = 400,
		MaxDelayHard = 1000
	};

	// Token: 0x04000F13 RID: 3859
	public float loudness;
}
