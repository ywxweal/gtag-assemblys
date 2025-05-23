using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000AD1 RID: 2769
	public class BucketThrowable : MonoBehaviour
	{
		// Token: 0x060042F0 RID: 17136 RVA: 0x001356A0 File Offset: 0x001338A0
		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Gorilla Head"))
			{
				if (this.audioSource && this.triggerClip)
				{
					this.audioSource.GTPlayOneShot(this.triggerClip, 1f);
				}
				base.Invoke("TriggerEvent", 0.25f);
			}
		}

		// Token: 0x060042F1 RID: 17137 RVA: 0x00135704 File Offset: 0x00133904
		private void TriggerEvent()
		{
			UnityAction<bool> onTriggerEntered = this.OnTriggerEntered;
			if (onTriggerEntered == null)
			{
				return;
			}
			onTriggerEntered(false);
		}

		// Token: 0x04004576 RID: 17782
		public GameObject projectilePrefab;

		// Token: 0x04004577 RID: 17783
		[Range(0f, 1f)]
		public float weightedChance = 1f;

		// Token: 0x04004578 RID: 17784
		public AudioSource audioSource;

		// Token: 0x04004579 RID: 17785
		public AudioClip triggerClip;

		// Token: 0x0400457A RID: 17786
		public bool destroyAfterRelease;

		// Token: 0x0400457B RID: 17787
		public float destroyAfterSeconds = -1f;

		// Token: 0x0400457C RID: 17788
		public UnityAction<bool> OnTriggerEntered;
	}
}
