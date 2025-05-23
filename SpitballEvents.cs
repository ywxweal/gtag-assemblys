using System;
using UnityEngine;

// Token: 0x02000469 RID: 1129
public class SpitballEvents : SubEmitterListener
{
	// Token: 0x06001BBB RID: 7099 RVA: 0x00088249 File Offset: 0x00086449
	protected override void OnSubEmit()
	{
		base.OnSubEmit();
		if (this._audioSource && this._sfxHit)
		{
			this._audioSource.GTPlayOneShot(this._sfxHit, 1f);
		}
	}

	// Token: 0x04001EC9 RID: 7881
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x04001ECA RID: 7882
	[SerializeField]
	private AudioClip _sfxHit;
}
