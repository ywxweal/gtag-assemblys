using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200059D RID: 1437
public class GRCurrencyDepositor : MonoBehaviour
{
	// Token: 0x06002318 RID: 8984 RVA: 0x000AF8C0 File Offset: 0x000ADAC0
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody != null)
		{
			GRCollectible component = other.attachedRigidbody.GetComponent<GRCollectible>();
			if (component != null)
			{
				if (GameEntityManager.instance.IsAuthority())
				{
					GhostReactorManager.instance.RequestDepositCollectible(component.entity.id);
				}
				this.collectibleDepositedEffect.Play();
				this.audioSource.volume = this.collectibleDepositedClipVolume;
				this.audioSource.PlayOneShot(this.collectibleDepositedClip);
				if (component.entity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					if (GamePlayerLocal.instance.gamePlayer.GetGameEntityId(0) == component.entity.id)
					{
						GorillaTagger.Instance.StartVibration(true, 0.5f, 0.15f);
						return;
					}
					if (GamePlayerLocal.instance.gamePlayer.GetGameEntityId(1) == component.entity.id)
					{
						GorillaTagger.Instance.StartVibration(false, 0.5f, 0.15f);
					}
				}
			}
		}
	}

	// Token: 0x0400274F RID: 10063
	public Transform depositingChargePoint;

	// Token: 0x04002750 RID: 10064
	[SerializeField]
	private ParticleSystem collectibleDepositedEffect;

	// Token: 0x04002751 RID: 10065
	[SerializeField]
	private AudioClip collectibleDepositedClip;

	// Token: 0x04002752 RID: 10066
	[SerializeField]
	private float collectibleDepositedClipVolume;

	// Token: 0x04002753 RID: 10067
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002754 RID: 10068
	[NonSerialized]
	public GhostReactorManager grManager;

	// Token: 0x04002755 RID: 10069
	private const float hapticStrength = 0.5f;

	// Token: 0x04002756 RID: 10070
	private const float hapticDuration = 0.15f;
}
