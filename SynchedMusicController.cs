using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x020006B6 RID: 1718
public class SynchedMusicController : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002AEB RID: 10987 RVA: 0x000D286C File Offset: 0x000D0A6C
	private void Start()
	{
		if (this.usingNewSyncedSongsCode)
		{
			this.New_Start();
			return;
		}
		this.totalLoopTime = 0L;
		AudioSource[] array = this.audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		}
		this.audioSource.mute = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		this.muteButton.isOn = this.audioSource.mute;
		this.muteButton.UpdateColor();
		for (int j = 0; j < this.muteButtons.Length; j++)
		{
			this.muteButtons[j].isOn = this.audioSource.mute;
			this.muteButtons[j].UpdateColor();
		}
		this.randomNumberGenerator = new Random(this.mySeed);
		this.GenerateSongStartRandomTimes();
		if (this.twoLayer)
		{
			array = this.audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].clip.LoadAudioData();
			}
		}
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x000D2988 File Offset: 0x000D0B88
	public void SliceUpdate()
	{
		if (this.usingNewSyncedSongsCode)
		{
			this.New_Update();
			return;
		}
		if (GorillaComputer.instance.startupMillis == 0L || this.totalLoopTime == 0L || this.songStartTimes.Length == 0)
		{
			return;
		}
		this.isPlayingCurrently = this.audioSource.isPlaying;
		if (this.testPlay)
		{
			this.testPlay = false;
			if (this.usingMultipleSources && this.usingMultipleSongs)
			{
				this.audioSource = this.audioSourceArray[Random.Range(0, this.audioSourceArray.Length)];
				this.audioSource.clip = this.songsArray[Random.Range(0, this.songsArray.Length)];
				this.audioSource.time = 0f;
			}
			if (this.twoLayer)
			{
				this.StartPlayingSongs(0L, 0L);
			}
			else if (this.audioSource.volume != 0f)
			{
				this.audioSource.GTPlay();
			}
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		this.currentTime = (GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) % this.totalLoopTime;
		if (!this.audioSource.isPlaying)
		{
			if (this.lastPlayIndex >= 0 && this.songStartTimes[this.lastPlayIndex % this.songStartTimes.Length] < this.currentTime && this.currentTime < this.songStartTimes[(this.lastPlayIndex + 1) % this.songStartTimes.Length])
			{
				if (this.twoLayer)
				{
					if (this.songStartTimes[this.lastPlayIndex] + (long)(this.audioSource.clip.length * 1000f) > this.currentTime)
					{
						this.StartPlayingSongs(this.songStartTimes[this.lastPlayIndex], this.currentTime);
						return;
					}
				}
				else if (this.usingMultipleSongs && this.usingMultipleSources)
				{
					if (this.songStartTimes[this.lastPlayIndex] + (long)(this.songsArray[this.audioClipsForPlaying[this.lastPlayIndex]].length * 1000f) > this.currentTime)
					{
						this.StartPlayingSong(this.songStartTimes[this.lastPlayIndex], this.currentTime, this.songsArray[this.audioClipsForPlaying[this.lastPlayIndex]], this.audioSourceArray[this.audioSourcesForPlaying[this.lastPlayIndex]]);
						return;
					}
				}
				else if (this.songStartTimes[this.lastPlayIndex] + (long)(this.audioSource.clip.length * 1000f) > this.currentTime)
				{
					this.StartPlayingSong(this.songStartTimes[this.lastPlayIndex], this.currentTime);
					return;
				}
			}
			else
			{
				for (int i = 0; i < this.songStartTimes.Length; i++)
				{
					if (this.songStartTimes[i] > this.currentTime)
					{
						this.lastPlayIndex = (i - 1) % this.songStartTimes.Length;
						return;
					}
				}
			}
		}
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x000D2C66 File Offset: 0x000D0E66
	private void StartPlayingSong(long timeStarted, long currentTime)
	{
		if (this.audioSource.volume != 0f)
		{
			this.audioSource.GTPlay();
		}
		this.audioSource.time = (float)(currentTime - timeStarted) / 1000f;
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x000D2C9C File Offset: 0x000D0E9C
	private void StartPlayingSongs(long timeStarted, long currentTime)
	{
		foreach (AudioSource audioSource in this.audioSourceArray)
		{
			if (audioSource.volume != 0f)
			{
				audioSource.GTPlay();
			}
			audioSource.time = (float)(currentTime - timeStarted) / 1000f;
		}
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x000D2CE8 File Offset: 0x000D0EE8
	private void StartPlayingSong(long timeStarted, long currentTime, AudioClip clipToPlay, AudioSource sourceToPlay)
	{
		this.audioSource = sourceToPlay;
		sourceToPlay.clip = clipToPlay;
		if (sourceToPlay.isActiveAndEnabled && sourceToPlay.volume != 0f)
		{
			sourceToPlay.GTPlay();
		}
		sourceToPlay.time = (float)(currentTime - timeStarted) / 1000f;
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x000D2D34 File Offset: 0x000D0F34
	private void GenerateSongStartRandomTimes()
	{
		this.songStartTimes = new long[500];
		this.audioSourcesForPlaying = new int[500];
		this.audioClipsForPlaying = new int[500];
		this.songStartTimes[0] = this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		for (int i = 1; i < this.songStartTimes.Length; i++)
		{
			this.songStartTimes[i] = this.songStartTimes[i - 1] + this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		}
		if (this.usingMultipleSources)
		{
			for (int j = 0; j < this.audioSourcesForPlaying.Length; j++)
			{
				this.audioSourcesForPlaying[j] = this.randomNumberGenerator.Next(this.audioSourceArray.Length);
			}
		}
		if (this.usingMultipleSongs)
		{
			for (int k = 0; k < this.audioClipsForPlaying.Length; k++)
			{
				this.audioClipsForPlaying[k] = this.randomNumberGenerator.Next(this.songsArray.Length);
			}
		}
		if (this.usingMultipleSongs)
		{
			this.totalLoopTime = this.songStartTimes[this.songStartTimes.Length - 1] + (long)(this.songsArray[this.audioClipsForPlaying[this.audioClipsForPlaying.Length - 1]].length * 1000f);
			return;
		}
		if (this.audioSource.clip != null)
		{
			this.totalLoopTime = this.songStartTimes[this.songStartTimes.Length - 1] + (long)(this.audioSource.clip.length * 1000f);
		}
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x000D2EC4 File Offset: 0x000D10C4
	public void MuteAudio(GorillaPressableButton pressedButton)
	{
		AudioSource[] array;
		if (this.audioSource.mute)
		{
			PlayerPrefs.SetInt(this.locationName + "Muted", 0);
			PlayerPrefs.Save();
			this.audioSource.mute = false;
			array = this.audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].mute = false;
			}
			pressedButton.isOn = false;
			pressedButton.UpdateColor();
			for (int j = 0; j < this.muteButtons.Length; j++)
			{
				if (this.muteButtons[j] != null)
				{
					this.muteButtons[j].isOn = false;
					this.muteButtons[j].UpdateColor();
				}
			}
			return;
		}
		PlayerPrefs.SetInt(this.locationName + "Muted", 1);
		PlayerPrefs.Save();
		this.audioSource.mute = true;
		array = this.audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = true;
		}
		pressedButton.isOn = true;
		pressedButton.UpdateColor();
		for (int k = 0; k < this.muteButtons.Length; k++)
		{
			if (this.muteButtons[k] != null)
			{
				this.muteButtons[k].isOn = true;
				this.muteButtons[k].UpdateColor();
			}
		}
	}

	// Token: 0x06002AF2 RID: 10994 RVA: 0x000D3004 File Offset: 0x000D1204
	protected void New_Start()
	{
		string text = this.New_Validate();
		if (text.Length > 0)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Disabling SynchedMusicController on \"",
				base.name,
				"\" due to invalid setup: ",
				text,
				" Path: ",
				base.transform.GetPathQ()
			}), this);
			base.enabled = false;
		}
		if (this.usingMultipleSources && this.audioSource == null)
		{
			this.audioSource = this.audioSourceArray[0];
		}
		this.totalLoopTime = 0L;
		bool flag = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		if (this.muteButton == null && this.muteButtons.Length >= 1 && this.muteButtons[0] != null)
		{
			this.muteButton = this.muteButtons[0];
		}
		if (this.audioSource != null)
		{
			this.audioSource.mute = flag;
			this.muteButton.isOn = this.audioSource.mute;
		}
		foreach (AudioSource audioSource in this.audioSourceArray)
		{
			audioSource.mute = flag;
			this.muteButton.isOn = audioSource.mute || this.muteButton.isOn;
		}
		for (int j = 0; j < this.muteButtons.Length; j++)
		{
			if (!(this.muteButtons[j] == null))
			{
				this.muteButtons[j].isOn = this.muteButton.isOn;
				this.muteButtons[j].UpdateColor();
			}
		}
		this.muteButton.UpdateColor();
		this.randomNumberGenerator = new Random(this.mySeed);
		this.New_GeneratePlaylistArrays();
		foreach (SynchedMusicController.SyncedSongInfo syncedSongInfo in this.syncedSongs)
		{
			if (syncedSongInfo.songLayers.Length > 1)
			{
				SynchedMusicController.SyncedSongLayerInfo[] songLayers = syncedSongInfo.songLayers;
				for (int k = 0; k < songLayers.Length; k++)
				{
					songLayers[k].audioClip.LoadAudioData();
				}
			}
		}
	}

	// Token: 0x06002AF3 RID: 10995 RVA: 0x000D322B File Offset: 0x000D142B
	public void OnEnable()
	{
		this.lastPlayIndex = -1;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002AF5 RID: 10997 RVA: 0x000D323C File Offset: 0x000D143C
	private void New_Update()
	{
		if (!GorillaComputer.hasInstance)
		{
			return;
		}
		if (GorillaComputer.instance.startupMillis == 0L || this.totalLoopTime <= 0L || this.songStartTimes.Length == 0)
		{
			return;
		}
		long startupMillis = GorillaComputer.instance.startupMillis;
		if (startupMillis <= 0L)
		{
			return;
		}
		long num = startupMillis + (long)(Time.realtimeSinceStartup * 1000f);
		long num2 = ((this.totalLoopTime > 0L) ? (num % this.totalLoopTime) : 0L);
		bool flag = false;
		if (this.lastPlayIndex < 0)
		{
			flag = true;
			for (int i = 1; i < 256; i++)
			{
				if (this.songStartTimes[i] > num2)
				{
					this.lastPlayIndex = (i - 1) % 256;
					break;
				}
			}
			if (this.lastPlayIndex < 0)
			{
				this.lastPlayIndex = 255;
			}
		}
		int num3 = (this.lastPlayIndex + 1) % 256;
		if (this.songStartTimes[num3] < num2)
		{
			this.lastPlayIndex = num3;
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		long num4 = this.songStartTimes[this.lastPlayIndex];
		SynchedMusicController.SyncedSongInfo syncedSongInfo = this.syncedSongs[this.audioClipsForPlaying[this.lastPlayIndex]];
		float length = syncedSongInfo.songLayers[0].audioClip.length;
		float num5 = (float)(num2 - num4) / 1000f;
		if (num5 < 0f || length < num5)
		{
			return;
		}
		for (int j = 0; j < syncedSongInfo.songLayers.Length; j++)
		{
			SynchedMusicController.SyncedSongLayerInfo syncedSongLayerInfo = syncedSongInfo.songLayers[j];
			if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.All)
			{
				foreach (AudioSource audioSource in this.audioSourceArray)
				{
					audioSource.clip = syncedSongLayerInfo.audioClip;
					if (audioSource.volume > 0f)
					{
						audioSource.GTPlay();
					}
					audioSource.time = num5;
				}
			}
			else if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Shuffle)
			{
				AudioSource audioSource2 = this.audioSourceArray[this.audioSourcesForPlaying[this.lastPlayIndex]];
				audioSource2.clip = syncedSongLayerInfo.audioClip;
				if (audioSource2.volume > 0f)
				{
					audioSource2.GTPlay();
				}
				audioSource2.time = num5;
			}
			else if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Specific)
			{
				foreach (AudioSource audioSource3 in syncedSongLayerInfo.audioSources)
				{
					audioSource3.clip = syncedSongLayerInfo.audioClip;
					if (audioSource3.volume > 0f)
					{
						audioSource3.GTPlay();
					}
					audioSource3.time = num5;
				}
			}
		}
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x000D34BC File Offset: 0x000D16BC
	private string New_Validate()
	{
		if (this.syncedSongs == null)
		{
			return "syncedSongs array cannot be null.";
		}
		if (this.syncedSongs.Length == 0)
		{
			return "syncedSongs array cannot be empty.";
		}
		for (int i = 0; i < this.syncedSongs.Length; i++)
		{
			SynchedMusicController.SyncedSongInfo syncedSongInfo = this.syncedSongs[i];
			if (syncedSongInfo.songLayers == null)
			{
				return string.Format("Song {0}'s songLayers array is null.", i);
			}
			if (syncedSongInfo.songLayers.Length == 0)
			{
				return string.Format("Song {0}'s songLayers array is empty.", i);
			}
			for (int j = 0; j < syncedSongInfo.songLayers.Length; j++)
			{
				SynchedMusicController.SyncedSongLayerInfo syncedSongLayerInfo = syncedSongInfo.songLayers[j];
				if (syncedSongLayerInfo.audioClip == null)
				{
					return string.Format("Song {0}'s song layer {1} does not have an audio clip.", i, j);
				}
				if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Specific)
				{
					if (syncedSongLayerInfo.audioSources == null || syncedSongLayerInfo.audioSources.Length == 0)
					{
						return string.Format("Song {0}'s song layer {1} has audioSourcePickMode set to {2} ", i, j, syncedSongLayerInfo.audioSourcePickMode) + "but layer's audioSources array is empty or null.";
					}
				}
				else if (this.audioSourceArray == null || this.audioSourceArray.Length == 0)
				{
					return string.Format("{0} is null or empty, while Song {1}'s song layer {2} has ", "audioSourceArray", i, j) + string.Format("audioSourcePickMode set to {0}, which uses the ", syncedSongLayerInfo.audioSourcePickMode) + "component's audioSourceArray.";
				}
			}
		}
		return string.Empty;
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x000D3624 File Offset: 0x000D1824
	private void New_GeneratePlaylistArrays()
	{
		if (this.syncedSongs == null || this.syncedSongs.Length == 0)
		{
			return;
		}
		this.songStartTimes = new long[256];
		this.songStartTimes[0] = this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		for (int i = 1; i < this.songStartTimes.Length; i++)
		{
			this.songStartTimes[i] = this.songStartTimes[i - 1] + this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		}
		this.audioSourcesForPlaying = new int[256];
		bool flag = false;
		SynchedMusicController.SyncedSongInfo[] array = this.syncedSongs;
		for (int j = 0; j < array.Length; j++)
		{
			SynchedMusicController.SyncedSongLayerInfo[] songLayers = array[j].songLayers;
			for (int k = 0; k < songLayers.Length; k++)
			{
				if (songLayers[k].audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Shuffle)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			for (int l = 0; l < this.audioSourcesForPlaying.Length; l++)
			{
				this.audioSourcesForPlaying[l] = this.randomNumberGenerator.Next(this.audioSourceArray.Length);
			}
		}
		this.audioClipsForPlaying = new int[256];
		for (int m = 0; m < this.audioClipsForPlaying.Length; m++)
		{
			if (this.shufflePlaylist)
			{
				this.audioClipsForPlaying[m] = this.randomNumberGenerator.Next(this.syncedSongs.Length);
			}
			else
			{
				this.audioClipsForPlaying[m] = this.syncedSongs.Length - 1;
			}
		}
		SynchedMusicController.SyncedSongInfo[] array2 = this.syncedSongs;
		int[] array3 = this.audioClipsForPlaying;
		long num = (long)array2[array3[array3.Length - 1]].songLayers[0].audioClip.length * 1000L;
		long[] array4 = this.songStartTimes;
		long num2 = array4[array4.Length - 1];
		this.totalLoopTime = num + num2;
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04002FD1 RID: 12241
	[SerializeField]
	private bool usingNewSyncedSongsCode;

	// Token: 0x04002FD2 RID: 12242
	[SerializeField]
	private bool shufflePlaylist = true;

	// Token: 0x04002FD3 RID: 12243
	[SerializeField]
	private SynchedMusicController.SyncedSongInfo[] syncedSongs;

	// Token: 0x04002FD4 RID: 12244
	[Tooltip("This should be unique per sound post. Sound posts that share the same seed and the same song count will play songs a the same times.")]
	public int mySeed;

	// Token: 0x04002FD5 RID: 12245
	private Random randomNumberGenerator = new Random();

	// Token: 0x04002FD6 RID: 12246
	[Tooltip("In milliseconds.")]
	public long minimumWait = 900000L;

	// Token: 0x04002FD7 RID: 12247
	[Tooltip("In milliseconds. A random value between 0 and this will be picked. The max wait time is randomInterval + minimumWait.")]
	public int randomInterval = 600000;

	// Token: 0x04002FD8 RID: 12248
	[DebugReadout]
	public long[] songStartTimes;

	// Token: 0x04002FD9 RID: 12249
	[DebugReadout]
	public int[] audioSourcesForPlaying;

	// Token: 0x04002FDA RID: 12250
	[DebugReadout]
	public int[] audioClipsForPlaying;

	// Token: 0x04002FDB RID: 12251
	public AudioSource audioSource;

	// Token: 0x04002FDC RID: 12252
	public AudioSource[] audioSourceArray;

	// Token: 0x04002FDD RID: 12253
	public AudioClip[] songsArray;

	// Token: 0x04002FDE RID: 12254
	[DebugReadout]
	public int lastPlayIndex;

	// Token: 0x04002FDF RID: 12255
	[DebugReadout]
	public long currentTime;

	// Token: 0x04002FE0 RID: 12256
	[DebugReadout]
	public long totalLoopTime;

	// Token: 0x04002FE1 RID: 12257
	public GorillaPressableButton muteButton;

	// Token: 0x04002FE2 RID: 12258
	public GorillaPressableButton[] muteButtons;

	// Token: 0x04002FE3 RID: 12259
	public bool usingMultipleSongs;

	// Token: 0x04002FE4 RID: 12260
	public bool usingMultipleSources;

	// Token: 0x04002FE5 RID: 12261
	[DebugReadout]
	public bool isPlayingCurrently;

	// Token: 0x04002FE6 RID: 12262
	[DebugReadout]
	public bool testPlay;

	// Token: 0x04002FE7 RID: 12263
	public bool twoLayer;

	// Token: 0x04002FE8 RID: 12264
	[Tooltip("Used to store the muted sound posts in player prefs.")]
	public string locationName;

	// Token: 0x04002FE9 RID: 12265
	private const int kPlaylistLength = 256;

	// Token: 0x020006B7 RID: 1719
	[Serializable]
	public struct SyncedSongInfo
	{
		// Token: 0x04002FEA RID: 12266
		[Tooltip("A layer for a song. For no layers, just add a single entry.")]
		[RequiredListLength(1, null)]
		public SynchedMusicController.SyncedSongLayerInfo[] songLayers;
	}

	// Token: 0x020006B8 RID: 1720
	[Serializable]
	public struct SyncedSongLayerInfo
	{
		// Token: 0x04002FEB RID: 12267
		[Tooltip("The clip that will be played.")]
		public AudioClip audioClip;

		// Token: 0x04002FEC RID: 12268
		public SynchedMusicController.AudioSourcePickMode audioSourcePickMode;

		// Token: 0x04002FED RID: 12269
		[Tooltip("The audio sources that should play the audio clip.")]
		public AudioSource[] audioSources;
	}

	// Token: 0x020006B9 RID: 1721
	public enum AudioSourcePickMode
	{
		// Token: 0x04002FEF RID: 12271
		All,
		// Token: 0x04002FF0 RID: 12272
		Shuffle,
		// Token: 0x04002FF1 RID: 12273
		Specific
	}
}
