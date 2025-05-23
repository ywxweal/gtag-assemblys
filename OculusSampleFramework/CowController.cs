using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BED RID: 3053
	public class CowController : MonoBehaviour
	{
		// Token: 0x06004B6E RID: 19310 RVA: 0x000023F4 File Offset: 0x000005F4
		private void Start()
		{
		}

		// Token: 0x06004B6F RID: 19311 RVA: 0x00165DEC File Offset: 0x00163FEC
		public void PlayMooSound()
		{
			this._mooCowAudioSource.timeSamples = 0;
			this._mooCowAudioSource.GTPlay();
		}

		// Token: 0x06004B70 RID: 19312 RVA: 0x00165E05 File Offset: 0x00164005
		public void GoMooCowGo()
		{
			this._cowAnimation.Rewind();
			this._cowAnimation.Play();
		}

		// Token: 0x04004DFD RID: 19965
		[SerializeField]
		private Animation _cowAnimation;

		// Token: 0x04004DFE RID: 19966
		[SerializeField]
		private AudioSource _mooCowAudioSource;
	}
}
