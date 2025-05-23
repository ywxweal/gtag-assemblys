using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B29 RID: 2857
	public class UnMuteAudioSourceOnEnable : MonoBehaviour
	{
		// Token: 0x06004657 RID: 18007 RVA: 0x0014E64F File Offset: 0x0014C84F
		public void Awake()
		{
			this.originalVolume = this.audioSource.volume;
		}

		// Token: 0x06004658 RID: 18008 RVA: 0x0014E662 File Offset: 0x0014C862
		public void OnEnable()
		{
			this.audioSource.volume = this.originalVolume;
		}

		// Token: 0x06004659 RID: 18009 RVA: 0x0014E675 File Offset: 0x0014C875
		public void OnDisable()
		{
			this.audioSource.volume = 0f;
		}

		// Token: 0x040048F0 RID: 18672
		public AudioSource audioSource;

		// Token: 0x040048F1 RID: 18673
		public float originalVolume;
	}
}
