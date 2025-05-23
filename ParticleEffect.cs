using System;
using UnityEngine;

// Token: 0x02000192 RID: 402
[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffect : MonoBehaviour
{
	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x060009E7 RID: 2535 RVA: 0x00034A02 File Offset: 0x00032C02
	public long effectID
	{
		get
		{
			return this._effectID;
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x060009E8 RID: 2536 RVA: 0x00034A0A File Offset: 0x00032C0A
	public bool isPlaying
	{
		get
		{
			return this.system && this.system.isPlaying;
		}
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x00034A26 File Offset: 0x00032C26
	public virtual void Play()
	{
		base.gameObject.SetActive(true);
		this.system.Play(true);
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00034A40 File Offset: 0x00032C40
	public virtual void Stop()
	{
		this.system.Stop(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00034A5A File Offset: 0x00032C5A
	private void OnParticleSystemStopped()
	{
		base.gameObject.SetActive(false);
		if (this.pool)
		{
			this.pool.Return(this);
		}
	}

	// Token: 0x04000C13 RID: 3091
	public ParticleSystem system;

	// Token: 0x04000C14 RID: 3092
	[SerializeField]
	private long _effectID;

	// Token: 0x04000C15 RID: 3093
	public ParticleEffectsPool pool;

	// Token: 0x04000C16 RID: 3094
	[NonSerialized]
	public int poolIndex = -1;
}
