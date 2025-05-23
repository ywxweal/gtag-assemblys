using System;
using System.Collections.Generic;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Audio
{
	// Token: 0x02000D92 RID: 3474
	public class GTSpeaker : Speaker
	{
		// Token: 0x06005639 RID: 22073 RVA: 0x001A3D2C File Offset: 0x001A1F2C
		public void Start()
		{
			LoudSpeakerNetwork componentInChildren = base.transform.root.GetComponentInChildren<LoudSpeakerNetwork>();
			if (componentInChildren != null)
			{
				this.AddExternalAudioSources(componentInChildren.SpeakerSources);
			}
		}

		// Token: 0x0600563A RID: 22074 RVA: 0x001A3D5F File Offset: 0x001A1F5F
		public void AddExternalAudioSources(AudioSource[] audioSources)
		{
			if (this._initializedExternalAudioSources)
			{
				return;
			}
			this._externalAudioSources = audioSources;
			this.InitializeExternalAudioSources();
			if (this._audioOutputStarted)
			{
				this.ExternalAudioOutputStart(this._frequency, this._channels, this._frameSamplesPerChannel);
			}
		}

		// Token: 0x0600563B RID: 22075 RVA: 0x001A3D97 File Offset: 0x001A1F97
		protected override void Initialize()
		{
			if (base.IsInitialized)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Already initialized.", Array.Empty<object>());
				}
				return;
			}
			base.Initialize();
		}

		// Token: 0x0600563C RID: 22076 RVA: 0x001A3DCC File Offset: 0x001A1FCC
		private void InitializeExternalAudioSources()
		{
			this._initializedExternalAudioSources = true;
			this._externalAudioOutputs = new List<IAudioOut<float>>();
			AudioOutDelayControl.PlayDelayConfig playDelayConfig = new AudioOutDelayControl.PlayDelayConfig
			{
				Low = this.playbackDelaySettings.MinDelaySoft,
				High = this.playbackDelaySettings.MaxDelaySoft,
				Max = this.playbackDelaySettings.MaxDelayHard
			};
			foreach (AudioSource audioSource in this._externalAudioSources)
			{
				this._externalAudioOutputs.Add(this.GetAudioOutFactoryFromSource(audioSource, playDelayConfig)());
			}
		}

		// Token: 0x0600563D RID: 22077 RVA: 0x001A3E55 File Offset: 0x001A2055
		private Func<IAudioOut<float>> GetAudioOutFactoryFromSource(AudioSource source, AudioOutDelayControl.PlayDelayConfig pdc)
		{
			return () => new UnityAudioOut(source, pdc, this.Logger, string.Empty, this.Logger.IsDebugEnabled);
		}

		// Token: 0x0600563E RID: 22078 RVA: 0x001A3E7C File Offset: 0x001A207C
		protected override void OnAudioFrame(FrameOut<float> frame)
		{
			base.OnAudioFrame(frame);
			if (this.BroadcastExternal)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					audioOut.Push(frame.Buf);
					if (frame.EndOfStream)
					{
						audioOut.Flush();
					}
				}
			}
		}

		// Token: 0x0600563F RID: 22079 RVA: 0x001A3EF4 File Offset: 0x001A20F4
		protected override void AudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
		{
			this._audioOutputStarted = true;
			this._frequency = frequency;
			this._channels = channels;
			this._frameSamplesPerChannel = frameSamplesPerChannel;
			base.AudioOutputStart(frequency, channels, frameSamplesPerChannel);
			this.ExternalAudioOutputStart(frequency, channels, frameSamplesPerChannel);
		}

		// Token: 0x06005640 RID: 22080 RVA: 0x001A3F24 File Offset: 0x001A2124
		private void ExternalAudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
		{
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					if (!audioOut.IsPlaying)
					{
						audioOut.Start(frequency, channels, frameSamplesPerChannel);
						audioOut.ToggleAudioSource(false);
					}
				}
			}
		}

		// Token: 0x06005641 RID: 22081 RVA: 0x001A3F90 File Offset: 0x001A2190
		protected override void AudioOutputStop()
		{
			this._audioOutputStarted = false;
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					audioOut.Stop();
				}
			}
			base.AudioOutputStop();
		}

		// Token: 0x06005642 RID: 22082 RVA: 0x001A3FF8 File Offset: 0x001A21F8
		protected override void AudioOutputService()
		{
			base.AudioOutputService();
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					if (!audioOut.IsPlaying)
					{
						audioOut.Service();
					}
				}
			}
		}

		// Token: 0x06005643 RID: 22083 RVA: 0x001A4060 File Offset: 0x001A2260
		public void ToggleAudioSource(bool toggle)
		{
			if (this._externalAudioOutputs == null)
			{
				return;
			}
			foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
			{
				audioOut.ToggleAudioSource(toggle);
			}
		}

		// Token: 0x04005A0C RID: 23052
		[FormerlySerializedAs("UseExternalAudioSources")]
		public bool BroadcastExternal;

		// Token: 0x04005A0D RID: 23053
		[SerializeField]
		private AudioSource[] _externalAudioSources;

		// Token: 0x04005A0E RID: 23054
		private List<IAudioOut<float>> _externalAudioOutputs;

		// Token: 0x04005A0F RID: 23055
		private int _frequency;

		// Token: 0x04005A10 RID: 23056
		private int _channels;

		// Token: 0x04005A11 RID: 23057
		private int _frameSamplesPerChannel;

		// Token: 0x04005A12 RID: 23058
		private bool _initializedExternalAudioSources;

		// Token: 0x04005A13 RID: 23059
		private bool _audioOutputStarted;
	}
}
