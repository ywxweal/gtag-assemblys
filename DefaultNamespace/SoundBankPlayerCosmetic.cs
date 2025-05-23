using System;
using UnityEngine;

namespace DefaultNamespace
{
	// Token: 0x02000ABF RID: 2751
	[RequireComponent(typeof(SoundBankPlayer))]
	public class SoundBankPlayerCosmetic : MonoBehaviour
	{
		// Token: 0x0600425E RID: 16990 RVA: 0x00132790 File Offset: 0x00130990
		private void Awake()
		{
			this.playAudioLoop = false;
		}

		// Token: 0x0600425F RID: 16991 RVA: 0x0013279C File Offset: 0x0013099C
		public void Update()
		{
			if (!this.playAudioLoop)
			{
				return;
			}
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null && !this.soundBankPlayer.audioSource.isPlaying)
			{
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06004260 RID: 16992 RVA: 0x00132804 File Offset: 0x00130A04
		public void PlayAudio()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06004261 RID: 16993 RVA: 0x00132850 File Offset: 0x00130A50
		public void PlayAudioLoop()
		{
			this.playAudioLoop = true;
		}

		// Token: 0x06004262 RID: 16994 RVA: 0x0013285C File Offset: 0x00130A5C
		public void PlayAudioNonInterrupting()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				if (this.soundBankPlayer.audioSource.isPlaying)
				{
					return;
				}
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06004263 RID: 16995 RVA: 0x001328BC File Offset: 0x00130ABC
		public void PlayAudioWithTunableVolume(bool leftHand, float fingerValue)
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				float num = Mathf.Clamp01(fingerValue);
				this.soundBankPlayer.audioSource.volume = num;
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06004264 RID: 16996 RVA: 0x00132920 File Offset: 0x00130B20
		public void StopAudio()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				this.soundBankPlayer.audioSource.Stop();
			}
			this.playAudioLoop = false;
		}

		// Token: 0x040044BD RID: 17597
		[SerializeField]
		private SoundBankPlayer soundBankPlayer;

		// Token: 0x040044BE RID: 17598
		private bool playAudioLoop;
	}
}
