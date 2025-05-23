using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000541 RID: 1345
public class EnclosedSpaceVolume : GorillaTriggerBox
{
	// Token: 0x06002095 RID: 8341 RVA: 0x000A35E2 File Offset: 0x000A17E2
	private void Awake()
	{
		this.audioSourceInside.volume = this.quietVolume;
		this.audioSourceOutside.volume = this.loudVolume;
	}

	// Token: 0x06002096 RID: 8342 RVA: 0x000A3606 File Offset: 0x000A1806
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<GTPlayer>() != null)
		{
			this.audioSourceInside.volume = this.loudVolume;
			this.audioSourceOutside.volume = this.quietVolume;
		}
	}

	// Token: 0x06002097 RID: 8343 RVA: 0x000A363D File Offset: 0x000A183D
	private void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<GTPlayer>() != null)
		{
			this.audioSourceInside.volume = this.quietVolume;
			this.audioSourceOutside.volume = this.loudVolume;
		}
	}

	// Token: 0x04002490 RID: 9360
	public AudioSource audioSourceInside;

	// Token: 0x04002491 RID: 9361
	public AudioSource audioSourceOutside;

	// Token: 0x04002492 RID: 9362
	public float loudVolume;

	// Token: 0x04002493 RID: 9363
	public float quietVolume;
}
