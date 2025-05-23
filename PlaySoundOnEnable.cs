using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class PlaySoundOnEnable : MonoBehaviour
{
	// Token: 0x060008E4 RID: 2276 RVA: 0x000301A5 File Offset: 0x0002E3A5
	private void Reset()
	{
		this._source = base.GetComponent<AudioSource>();
		if (this._source)
		{
			this._source.playOnAwake = false;
		}
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x000301CC File Offset: 0x0002E3CC
	private void OnEnable()
	{
		this.Play();
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x000301D4 File Offset: 0x0002E3D4
	private void OnDisable()
	{
		this.Stop();
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x000301DC File Offset: 0x0002E3DC
	public void Play()
	{
		if (this._loop && this._clips.Length == 1 && this._loopDelay == Vector2.zero)
		{
			this._source.clip = this._clips[0];
			this._source.loop = true;
			this._source.GTPlay();
			return;
		}
		this._source.loop = false;
		if (this._loop)
		{
			base.StartCoroutine(this.DoLoop());
			return;
		}
		this._source.clip = this._clips[Random.Range(0, this._clips.Length)];
		this._source.GTPlay();
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x00030286 File Offset: 0x0002E486
	private IEnumerator DoLoop()
	{
		while (base.enabled)
		{
			this._source.clip = this._clips[Random.Range(0, this._clips.Length)];
			this._source.GTPlay();
			while (this._source.isPlaying)
			{
				yield return null;
			}
			float num = Random.Range(this._loopDelay.x, this._loopDelay.y);
			if (num > 0f)
			{
				float waitEndTime = Time.time + num;
				while (Time.time < waitEndTime)
				{
					yield return null;
				}
			}
		}
		yield break;
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00030295 File Offset: 0x0002E495
	public void Stop()
	{
		this._source.GTStop();
		this._source.loop = false;
	}

	// Token: 0x04000A7D RID: 2685
	[SerializeField]
	private AudioSource _source;

	// Token: 0x04000A7E RID: 2686
	[SerializeField]
	private AudioClip[] _clips;

	// Token: 0x04000A7F RID: 2687
	[SerializeField]
	private bool _loop;

	// Token: 0x04000A80 RID: 2688
	[SerializeField]
	private Vector2 _loopDelay;
}
