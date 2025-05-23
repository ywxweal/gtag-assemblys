using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020005AF RID: 1455
public class GRHazardousMaterial : MonoBehaviour
{
	// Token: 0x0600238E RID: 9102 RVA: 0x000B2FB4 File Offset: 0x000B11B4
	public void OnLocalPlayerOverlap()
	{
		GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
		if (component != null && component.State == GRPlayer.GRPlayerState.Alive)
		{
			GhostReactorManager.instance.RequestPlayerStateChange(component, GRPlayer.GRPlayerState.Ghost);
		}
	}

	// Token: 0x0600238F RID: 9103 RVA: 0x000B2FE9 File Offset: 0x000B11E9
	private void OnTriggerEnter(Collider collider)
	{
		if (collider == GTPlayer.Instance.headCollider || collider == GTPlayer.Instance.bodyCollider)
		{
			this.OnLocalPlayerOverlap();
		}
	}

	// Token: 0x06002390 RID: 9104 RVA: 0x000B3015 File Offset: 0x000B1215
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider == GTPlayer.Instance.headCollider || collision.collider == GTPlayer.Instance.bodyCollider)
		{
			this.OnLocalPlayerOverlap();
		}
	}
}
