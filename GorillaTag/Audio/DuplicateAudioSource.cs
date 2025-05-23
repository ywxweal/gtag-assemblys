using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000D8E RID: 3470
	public class DuplicateAudioSource : MonoBehaviour
	{
		// Token: 0x06005622 RID: 22050 RVA: 0x001A332E File Offset: 0x001A152E
		public void SetTargetAudioSource(AudioSource target)
		{
			this.TargetAudioSource = target;
			this.StartDuplicating();
		}

		// Token: 0x06005623 RID: 22051 RVA: 0x001A3340 File Offset: 0x001A1540
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

		// Token: 0x06005624 RID: 22052 RVA: 0x001A3398 File Offset: 0x001A1598
		[ContextMenu("Stop Duplicating")]
		public void StopDuplicating()
		{
			this._isDuplicating = false;
			this._audioSource.Stop();
		}

		// Token: 0x06005625 RID: 22053 RVA: 0x001A33AC File Offset: 0x001A15AC
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

		// Token: 0x040059F0 RID: 23024
		public AudioSource TargetAudioSource;

		// Token: 0x040059F1 RID: 23025
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040059F2 RID: 23026
		[SerializeField]
		private bool _isDuplicating;
	}
}
