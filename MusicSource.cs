using System;
using UnityEngine;

// Token: 0x0200069D RID: 1693
[RequireComponent(typeof(AudioSource))]
public class MusicSource : MonoBehaviour
{
	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x06002A5D RID: 10845 RVA: 0x000D10E4 File Offset: 0x000CF2E4
	public AudioSource AudioSource
	{
		get
		{
			return this.audioSource;
		}
	}

	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x06002A5E RID: 10846 RVA: 0x000D10EC File Offset: 0x000CF2EC
	public float DefaultVolume
	{
		get
		{
			return this.defaultVolume;
		}
	}

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x06002A5F RID: 10847 RVA: 0x000D10F4 File Offset: 0x000CF2F4
	public bool VolumeOverridden
	{
		get
		{
			return this.volumeOverride != null;
		}
	}

	// Token: 0x06002A60 RID: 10848 RVA: 0x000D1101 File Offset: 0x000CF301
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

	// Token: 0x06002A61 RID: 10849 RVA: 0x000D1136 File Offset: 0x000CF336
	private void OnEnable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.RegisterMusicSource(this);
		}
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x000D1154 File Offset: 0x000CF354
	private void OnDisable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.UnregisterMusicSource(this);
		}
	}

	// Token: 0x06002A63 RID: 10851 RVA: 0x000D1172 File Offset: 0x000CF372
	public void SetVolumeOverride(float volume)
	{
		this.volumeOverride = new float?(volume);
		this.audioSource.volume = this.volumeOverride.Value;
	}

	// Token: 0x06002A64 RID: 10852 RVA: 0x000D1196 File Offset: 0x000CF396
	public void UnsetVolumeOverride()
	{
		this.volumeOverride = null;
		this.audioSource.volume = this.defaultVolume;
	}

	// Token: 0x04002F4E RID: 12110
	[SerializeField]
	private float defaultVolume = 1f;

	// Token: 0x04002F4F RID: 12111
	[SerializeField]
	private bool setDefaultVolumeFromAudioSourceOnAwake = true;

	// Token: 0x04002F50 RID: 12112
	private AudioSource audioSource;

	// Token: 0x04002F51 RID: 12113
	private float? volumeOverride;
}
