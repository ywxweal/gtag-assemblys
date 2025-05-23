using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000471 RID: 1137
public class SoundEffects : MonoBehaviour
{
	// Token: 0x17000304 RID: 772
	// (get) Token: 0x06001BF5 RID: 7157 RVA: 0x000898F2 File Offset: 0x00087AF2
	public bool isPlaying
	{
		get
		{
			return this._lastClipIndex >= 0 && this._lastClipLength >= 0.0 && this._lastClipElapsedTime < this._lastClipLength;
		}
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x00089925 File Offset: 0x00087B25
	public void Clear()
	{
		this.audioClips.Clear();
		this._lastClipIndex = -1;
		this._lastClipLength = -1.0;
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x00089948 File Offset: 0x00087B48
	public void Stop()
	{
		if (this.source)
		{
			this.source.GTStop();
		}
		this._lastClipLength = -1.0;
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x00089974 File Offset: 0x00087B74
	public void PlayNext(float delayMin, float delayMax, float volMin, float volMax)
	{
		float num = this._rnd.NextFloat(delayMin, delayMax);
		float num2 = this._rnd.NextFloat(volMin, volMax);
		this.PlayNext(num, num2);
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x000899A8 File Offset: 0x00087BA8
	public void PlayNext(float delay = 0f, float volume = 1f)
	{
		if (!this.source)
		{
			return;
		}
		if (this.audioClips == null || this.audioClips.Count == 0)
		{
			return;
		}
		if (this.source.isPlaying)
		{
			this.source.GTStop();
		}
		int num = this._rnd.NextInt(this.audioClips.Count);
		while (this.distinct && this._lastClipIndex == num)
		{
			num = this._rnd.NextInt(this.audioClips.Count);
		}
		AudioClip audioClip = this.audioClips[num];
		this._lastClipIndex = num;
		this._lastClipLength = (double)audioClip.length;
		float num2 = delay;
		if (num2 < this._minDelay)
		{
			num2 = this._minDelay;
		}
		if (num2 < 0.0001f)
		{
			this.source.GTPlayOneShot(audioClip, volume);
			this._lastClipElapsedTime = 0f;
			return;
		}
		this.source.clip = audioClip;
		this.source.volume = volume;
		this.source.GTPlayDelayed(num2);
		this._lastClipElapsedTime = -num2;
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x00089ABC File Offset: 0x00087CBC
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(this.seed))
		{
			this.seed = "0x1337C0D3";
		}
		this._rnd = new SRand(this.seed);
		if (this.audioClips == null)
		{
			this.audioClips = new List<AudioClip>();
		}
	}

	// Token: 0x04001F16 RID: 7958
	public AudioSource source;

	// Token: 0x04001F17 RID: 7959
	[Space]
	public List<AudioClip> audioClips = new List<AudioClip>();

	// Token: 0x04001F18 RID: 7960
	public string seed = "0x1337C0D3";

	// Token: 0x04001F19 RID: 7961
	[Space]
	public bool distinct = true;

	// Token: 0x04001F1A RID: 7962
	[SerializeField]
	private float _minDelay;

	// Token: 0x04001F1B RID: 7963
	[Space]
	[SerializeField]
	private SRand _rnd;

	// Token: 0x04001F1C RID: 7964
	[NonSerialized]
	private int _lastClipIndex = -1;

	// Token: 0x04001F1D RID: 7965
	[NonSerialized]
	private double _lastClipLength = -1.0;

	// Token: 0x04001F1E RID: 7966
	[NonSerialized]
	private TimeSince _lastClipElapsedTime;
}
