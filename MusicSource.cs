using System;
using UnityEngine;

// Token: 0x0200069D RID: 1693
[RequireComponent(typeof(AudioSource))]
public class MusicSource : MonoBehaviour
{
	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x06002A5E RID: 10846 RVA: 0x000D1188 File Offset: 0x000CF388
	public AudioSource AudioSource
	{
		get
		{
			return this.audioSource;
		}
	}

	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x06002A5F RID: 10847 RVA: 0x000D1190 File Offset: 0x000CF390
	public float DefaultVolume
	{
		get
		{
			return this.defaultVolume;
		}
	}

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x06002A60 RID: 10848 RVA: 0x000D1198 File Offset: 0x000CF398
	public bool VolumeOverridden
	{
		get
		{
			return this.volumeOverride != null;
		}
	}

	// Token: 0x06002A61 RID: 10849 RVA: 0x000D11A5 File Offset: 0x000CF3A5
	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		if (this.setDefaultVolumeFromAudioSourceOnAwake)
		{
			this.defaultVolume = this.audioSource.volume;
		}
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x000D11DA File Offset: 0x000CF3DA
	private void OnEnable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.RegisterMusicSource(this);
		}
	}

	// Token: 0x06002A63 RID: 10851 RVA: 0x000D11F8 File Offset: 0x000CF3F8
	private void OnDisable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.UnregisterMusicSource(this);
		}
	}

	// Token: 0x06002A64 RID: 10852 RVA: 0x000D1216 File Offset: 0x000CF416
	public void SetVolumeOverride(float volume)
	{
		this.volumeOverride = new float?(volume);
		this.audioSource.volume = this.volumeOverride.Value;
	}

	// Token: 0x06002A65 RID: 10853 RVA: 0x000D123A File Offset: 0x000CF43A
	public void UnsetVolumeOverride()
	{
		this.volumeOverride = null;
		this.audioSource.volume = this.defaultVolume;
	}

	// Token: 0x04002F50 RID: 12112
	[SerializeField]
	private float defaultVolume = 1f;

	// Token: 0x04002F51 RID: 12113
	[SerializeField]
	private bool setDefaultVolumeFromAudioSourceOnAwake = true;

	// Token: 0x04002F52 RID: 12114
	private AudioSource audioSource;

	// Token: 0x04002F53 RID: 12115
	private float? volumeOverride;
}
