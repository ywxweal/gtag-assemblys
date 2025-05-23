using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020009B9 RID: 2489
public class SoundBankPlayer : MonoBehaviour
{
	// Token: 0x170005DD RID: 1501
	// (get) Token: 0x06003B7C RID: 15228 RVA: 0x0011B850 File Offset: 0x00119A50
	public bool isPlaying
	{
		get
		{
			return Time.realtimeSinceStartup < this.playEndTime;
		}
	}

	// Token: 0x170005DE RID: 1502
	// (get) Token: 0x06003B7D RID: 15229 RVA: 0x0011B85F File Offset: 0x00119A5F
	public float NormalizedTime
	{
		get
		{
			if (this.clipDuration != 0f)
			{
				return Mathf.Clamp01(this.CurrentTime / this.clipDuration);
			}
			return 1f;
		}
	}

	// Token: 0x170005DF RID: 1503
	// (get) Token: 0x06003B7E RID: 15230 RVA: 0x0011B886 File Offset: 0x00119A86
	public float CurrentTime
	{
		get
		{
			return Time.realtimeSinceStartup - this.playStartTime;
		}
	}

	// Token: 0x06003B7F RID: 15231 RVA: 0x0011B894 File Offset: 0x00119A94
	protected void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.gameObject.AddComponent<AudioSource>();
			this.audioSource.outputAudioMixerGroup = this.outputAudioMixerGroup;
			this.audioSource.spatialize = this.spatialize;
			this.audioSource.spatializePostEffects = this.spatializePostEffects;
			this.audioSource.bypassEffects = this.bypassEffects;
			this.audioSource.bypassListenerEffects = this.bypassListenerEffects;
			this.audioSource.bypassReverbZones = this.bypassReverbZones;
			this.audioSource.priority = this.priority;
			this.audioSource.spatialBlend = this.spatialBlend;
			this.audioSource.dopplerLevel = this.dopplerLevel;
			this.audioSource.spread = this.spread;
			this.audioSource.rolloffMode = this.rolloffMode;
			this.audioSource.minDistance = this.minDistance;
			this.audioSource.maxDistance = this.maxDistance;
			this.audioSource.reverbZoneMix = this.reverbZoneMix;
		}
		this.audioSource.volume = 1f;
		this.audioSource.playOnAwake = false;
		if (this.shuffleOrder)
		{
			int[] array = new int[this.soundBank.sounds.Length / 2];
			this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
			for (int i = 0; i < this.playlist.Length; i++)
			{
				int num = 0;
				for (int j = 0; j < 100; j++)
				{
					num = Random.Range(0, this.soundBank.sounds.Length);
					if (Array.IndexOf<int>(array, num) == -1)
					{
						break;
					}
				}
				if (array.Length != 0)
				{
					array[i % array.Length] = num;
				}
				this.playlist[i] = new SoundBankPlayer.PlaylistEntry
				{
					index = num,
					volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
					pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
				};
			}
			return;
		}
		this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
		for (int k = 0; k < this.playlist.Length; k++)
		{
			this.playlist[k] = new SoundBankPlayer.PlaylistEntry
			{
				index = k % this.soundBank.sounds.Length,
				volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
				pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
			};
		}
	}

	// Token: 0x06003B80 RID: 15232 RVA: 0x0011BB8D File Offset: 0x00119D8D
	protected void OnEnable()
	{
		if (this.playOnEnable)
		{
			this.Play();
		}
	}

	// Token: 0x06003B81 RID: 15233 RVA: 0x0011BBA0 File Offset: 0x00119DA0
	public void Play()
	{
		this.Play(null, null);
	}

	// Token: 0x06003B82 RID: 15234 RVA: 0x0011BBC8 File Offset: 0x00119DC8
	public void Play(float? volumeOverride = null, float? pitchOverride = null)
	{
		if (!base.enabled || this.soundBank.sounds.Length == 0)
		{
			return;
		}
		SoundBankPlayer.PlaylistEntry playlistEntry = this.playlist[this.nextIndex];
		this.audioSource.pitch = ((pitchOverride != null) ? pitchOverride.Value : playlistEntry.pitch);
		AudioClip audioClip = this.soundBank.sounds[playlistEntry.index];
		if (audioClip != null)
		{
			this.audioSource.GTPlayOneShot(audioClip, (volumeOverride != null) ? volumeOverride.Value : playlistEntry.volume);
			this.clipDuration = audioClip.length;
			this.playStartTime = Time.realtimeSinceStartup;
			this.playEndTime = Mathf.Max(this.playEndTime, this.playStartTime + audioClip.length);
			this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
			return;
		}
		if (this.missingSoundsAreOk)
		{
			this.clipDuration = 0f;
			this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
			return;
		}
		Debug.LogErrorFormat("Sounds bank {0} is missing a clip at {1}", new object[]
		{
			base.gameObject.name,
			playlistEntry.index
		});
	}

	// Token: 0x06003B83 RID: 15235 RVA: 0x0011BD05 File Offset: 0x00119F05
	public void RestartSequence()
	{
		this.nextIndex = 0;
	}

	// Token: 0x04003FE0 RID: 16352
	[Tooltip("Optional. AudioSource Settings will be used if this is not defined.")]
	public AudioSource audioSource;

	// Token: 0x04003FE1 RID: 16353
	public bool playOnEnable = true;

	// Token: 0x04003FE2 RID: 16354
	public bool shuffleOrder = true;

	// Token: 0x04003FE3 RID: 16355
	public bool missingSoundsAreOk;

	// Token: 0x04003FE4 RID: 16356
	public SoundBankSO soundBank;

	// Token: 0x04003FE5 RID: 16357
	public AudioMixerGroup outputAudioMixerGroup;

	// Token: 0x04003FE6 RID: 16358
	public bool spatialize;

	// Token: 0x04003FE7 RID: 16359
	public bool spatializePostEffects;

	// Token: 0x04003FE8 RID: 16360
	public bool bypassEffects;

	// Token: 0x04003FE9 RID: 16361
	public bool bypassListenerEffects;

	// Token: 0x04003FEA RID: 16362
	public bool bypassReverbZones;

	// Token: 0x04003FEB RID: 16363
	public int priority = 128;

	// Token: 0x04003FEC RID: 16364
	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	// Token: 0x04003FED RID: 16365
	public float reverbZoneMix = 1f;

	// Token: 0x04003FEE RID: 16366
	public float dopplerLevel = 1f;

	// Token: 0x04003FEF RID: 16367
	public float spread;

	// Token: 0x04003FF0 RID: 16368
	public AudioRolloffMode rolloffMode;

	// Token: 0x04003FF1 RID: 16369
	public float minDistance = 1f;

	// Token: 0x04003FF2 RID: 16370
	public float maxDistance = 100f;

	// Token: 0x04003FF3 RID: 16371
	public AnimationCurve customRolloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04003FF4 RID: 16372
	private int nextIndex;

	// Token: 0x04003FF5 RID: 16373
	private float playStartTime;

	// Token: 0x04003FF6 RID: 16374
	private float playEndTime;

	// Token: 0x04003FF7 RID: 16375
	private float clipDuration;

	// Token: 0x04003FF8 RID: 16376
	private SoundBankPlayer.PlaylistEntry[] playlist;

	// Token: 0x020009BA RID: 2490
	private struct PlaylistEntry
	{
		// Token: 0x04003FF9 RID: 16377
		public int index;

		// Token: 0x04003FFA RID: 16378
		public float volume;

		// Token: 0x04003FFB RID: 16379
		public float pitch;
	}
}
