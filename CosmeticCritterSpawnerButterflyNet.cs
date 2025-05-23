using System;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class CosmeticCritterSpawnerButterflyNet : CosmeticCritterSpawnerTimed
{
	// Token: 0x0600041E RID: 1054 RVA: 0x0001811C File Offset: 0x0001631C
	public override void SetRandomVariables(CosmeticCritter critter)
	{
		Vector3 vector = base.transform.position + Random.onUnitSphere * this.spawnRadius;
		(critter as CosmeticCritterButterfly).SetStartPos(vector);
	}

	// Token: 0x04000495 RID: 1173
	[Tooltip("Spawn a butterfly on the surface of a sphere with this radius, and with a center on this object.")]
	[SerializeField]
	private float spawnRadius = 1f;
}
