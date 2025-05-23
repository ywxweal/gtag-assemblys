using System;
using UnityEngine;

// Token: 0x020005AA RID: 1450
public class GREntityDestroyTrigger : MonoBehaviour
{
	// Token: 0x06002381 RID: 9089 RVA: 0x000B2A42 File Offset: 0x000B0C42
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.GetComponent<GameEntity>() != null && GameEntityManager.instance.IsAuthority())
		{
			GameEntityManager.instance.RequestDestroyItem(other.attachedRigidbody.GetComponent<GameEntity>().id);
		}
	}
}
