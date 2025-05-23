using System;
using UnityEngine;

// Token: 0x02000989 RID: 2441
public class DelayedDestroyPooledObj : MonoBehaviour
{
	// Token: 0x06003AB6 RID: 15030 RVA: 0x001191E3 File Offset: 0x001173E3
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x06003AB7 RID: 15031 RVA: 0x00119211 File Offset: 0x00117411
	protected void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x04003F98 RID: 16280
	[Tooltip("Return to the object pool after this many seconds.")]
	public float destroyDelay;

	// Token: 0x04003F99 RID: 16281
	private float timeToDie = -1f;
}
