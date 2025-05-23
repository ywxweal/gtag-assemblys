using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D8E RID: 3470
	public class DuplicateAudioSource : MonoBehaviour
	{
		// Token: 0x06005621 RID: 22049 RVA: 0x001A3256 File Offset: 0x001A1456
		public void SetTargetAudioSource(AudioSource target)
		{
			this.TargetAudioSource = target;
			this.StartDuplicating();
		}

		// Token: 0x06005622 RID: 22050 RVA: 0x001A3268 File Offset: 0x001A1468
		[ContextMenu("Start Duplicating")]
		public void StartDuplicating()
		{
			this._isDuplicating = true;
			this._audioSource.loop = this.TargetAudioSource.loop;
			this._audioSource.clip = this.TargetAudioSource.clip;
			if (this.TargetAudioSource.isPlaying)
			{
				this._audioSource.Play();
			}
		}

		// Token: 0x06005623 RID: 22051 RVA: 0x001A32C0 File Offset: 0x001A14C0
		[ContextMenu("Stop Duplicating")]
		public void StopDuplicating()
		{
			this._isDuplicating = false;
			this._audioSource.Stop();
		}

		// Token: 0x06005624 RID: 22052 RVA: 0x001A32D4 File Offset: 0x001A14D4
		public void LateUpdate()
		{
			if (this._isDuplicating)
			{
				if (this.TargetAudioSource.isPlaying && !this._audioSource.isPlaying)
				{
					this._audioSource.Play();
					return;
				}
				if (!this.TargetAudioSource.isPlaying && this._audioSource.isPlaying)
				{
					this._audioSource.Stop();
				}
			}
		}

		// Token: 0x040059EF RID: 23023
		public AudioSource TargetAudioSource;

		// Token: 0x040059F0 RID: 23024
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040059F1 RID: 23025
		[SerializeField]
		private bool _isDuplicating;
	}
}
