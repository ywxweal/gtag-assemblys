using System;
using GorillaTag;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x02000205 RID: 517
public class SpawnFireHandFX : FXModifier
{
	// Token: 0x06000C12 RID: 3090 RVA: 0x0003FDBC File Offset: 0x0003DFBC
	public override void UpdateScale(float scale)
	{
		if (this.firePool == null)
		{
			this.firePool = ObjectPools.instance.GetPoolByHash(in this.firePrefab);
		}
		FireManager.SpawnFire(this.firePool, base.transform.position, Vector3.up, scale * this.fireSize);
		ObjectPools.instance.Destroy(base.gameObject);
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000E95 RID: 3733
	[SerializeField]
	private HashWrapper firePrefab;

	// Token: 0x04000E96 RID: 3734
	[SerializeField]
	private float fireSize = 1f;

	// Token: 0x04000E97 RID: 3735
	private SinglePool firePool;
}
