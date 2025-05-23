using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BED RID: 3053
	public class CowController : MonoBehaviour
	{
		// Token: 0x06004B6D RID: 19309 RVA: 0x000023F4 File Offset: 0x000005F4
		private void Start()
		{
		}

		// Token: 0x06004B6E RID: 19310 RVA: 0x00165D14 File Offset: 0x00163F14
		public void PlayMooSound()
		{
			this._mooCowAudioSource.timeSamples = 0;
			this._mooCowAudioSource.GTPlay();
		}

		// Token: 0x06004B6F RID: 19311 RVA: 0x00165D2D File Offset: 0x00163F2D
		public void GoMooCowGo()
		{
			this._cowAnimation.Rewind();
			this._cowAnimation.Play();
		}

		// Token: 0x04004DFC RID: 19964
		[SerializeField]
		private Animation _cowAnimation;

		// Token: 0x04004DFD RID: 19965
		[SerializeField]
		private AudioSource _mooCowAudioSource;
	}
}
