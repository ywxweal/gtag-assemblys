using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B29 RID: 2857
	public class UnMuteAudioSourceOnEnable : MonoBehaviour
	{
		// Token: 0x06004658 RID: 18008 RVA: 0x0014E727 File Offset: 0x0014C927
		public void Awake()
		{
			this.originalVolume = this.audioSource.volume;
		}

		// Token: 0x06004659 RID: 18009 RVA: 0x0014E73A File Offset: 0x0014C93A
		public void OnEnable()
		{
			this.audioSource.volume = this.originalVolume;
		}

		// Token: 0x0600465A RID: 18010 RVA: 0x0014E74D File Offset: 0x0014C94D
		public void OnDisable()
		{
			this.audioSource.volume = 0f;
		}

		// Token: 0x040048F1 RID: 18673
		public AudioSource audioSource;

		// Token: 0x040048F2 RID: 18674
		public float originalVolume;
	}
}
