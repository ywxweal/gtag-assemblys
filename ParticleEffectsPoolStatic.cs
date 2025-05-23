using System;
using UnityEngine;

// Token: 0x02000195 RID: 405
public class ParticleEffectsPoolStatic<T> : ParticleEffectsPool where T : ParticleEffectsPool
{
	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000A02 RID: 2562 RVA: 0x00034D73 File Offset: 0x00032F73
	public static T Instance
	{
		get
		{
			return ParticleEffectsPoolStatic<T>.gInstance;
		}
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x00034D7A File Offset: 0x00032F7A
	protected override void OnPoolAwake()
	{
		if (ParticleEffectsPoolStatic<T>.gInstance && ParticleEffectsPoolStatic<T>.gInstance != this)
		{
			Object.Destroy(this);
			return;
		}
		ParticleEffectsPoolStatic<T>.gInstance = this as T;
	}

	// Token: 0x04000C21 RID: 3105
	protected static T gInstance;
}
