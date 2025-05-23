using System;
using UnityEngine;

// Token: 0x020005BB RID: 1467
public class GRReviveStation : MonoBehaviour
{
	// Token: 0x17000369 RID: 873
	// (get) Token: 0x060023B6 RID: 9142 RVA: 0x000B3D30 File Offset: 0x000B1F30
	// (set) Token: 0x060023B7 RID: 9143 RVA: 0x000B3D38 File Offset: 0x000B1F38
	public int Index { get; set; }

	// Token: 0x060023B8 RID: 9144 RVA: 0x000B3D44 File Offset: 0x000B1F44
	public void RevivePlayer(GRPlayer player)
	{
		if (player.State != GRPlayer.GRPlayerState.Alive)
		{
			player.ChangePlayerState(GRPlayer.GRPlayerState.Alive);
			this.audioSource.Play();
			if (this.particleEffects != null)
			{
				for (int i = 0; i < this.particleEffects.Length; i++)
				{
					this.particleEffects[i].Play();
				}
			}
		}
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000B3D94 File Offset: 0x000B1F94
	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			VRRig component = attachedRigidbody.GetComponent<VRRig>();
			if (component != null)
			{
				GRPlayer component2 = component.GetComponent<GRPlayer>();
				if (component2 != null && component2.State != GRPlayer.GRPlayerState.Alive)
				{
					if (!NetworkSystem.Instance.InRoom && component == VRRig.LocalRig)
					{
						this.RevivePlayer(component2);
					}
					GhostReactorManager.instance.RequestPlayerRevive(this, component2);
				}
			}
		}
	}

	// Token: 0x0400288A RID: 10378
	public AudioSource audioSource;

	// Token: 0x0400288B RID: 10379
	public ParticleSystem[] particleEffects;
}
