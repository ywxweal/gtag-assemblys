using System;
using UnityEngine;

// Token: 0x02000218 RID: 536
public class SoundOnCollisionTagSpecific : MonoBehaviour
{
	// Token: 0x06000C83 RID: 3203 RVA: 0x000418E0 File Offset: 0x0003FAE0
	private void OnTriggerEnter(Collider collider)
	{
		if (Time.time > this.nextSound && collider.gameObject.CompareTag(this.tagName))
		{
			this.nextSound = Time.time + this.noiseCooldown;
			this.audioSource.GTPlayOneShot(this.collisionSounds[Random.Range(0, this.collisionSounds.Length)], 0.5f);
		}
	}

	// Token: 0x04000F0D RID: 3853
	public string tagName;

	// Token: 0x04000F0E RID: 3854
	public float noiseCooldown = 1f;

	// Token: 0x04000F0F RID: 3855
	private float nextSound;

	// Token: 0x04000F10 RID: 3856
	public AudioSource audioSource;

	// Token: 0x04000F11 RID: 3857
	public AudioClip[] collisionSounds;
}
