using System;
using UnityEngine;

// Token: 0x02000989 RID: 2441
public class DelayedDestroyPooledObj : MonoBehaviour
{
	// Token: 0x06003AB5 RID: 15029 RVA: 0x0011910B File Offset: 0x0011730B
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x06003AB6 RID: 15030 RVA: 0x00119139 File Offset: 0x00117339
	protected void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x04003F97 RID: 16279
	[Tooltip("Return to the object pool after this many seconds.")]
	public float destroyDelay;

	// Token: 0x04003F98 RID: 16280
	private float timeToDie = -1f;
}
