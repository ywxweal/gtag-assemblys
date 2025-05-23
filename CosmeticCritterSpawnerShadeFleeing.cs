using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class CosmeticCritterSpawnerShadeFleeing : CosmeticCritterSpawner
{
	// Token: 0x0600042C RID: 1068 RVA: 0x000185E7 File Offset: 0x000167E7
	public void SetSpawnPosition(Vector3 pos)
	{
		this.spawnPosition = pos;
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x000185F0 File Offset: 0x000167F0
	public override void OnSpawn(CosmeticCritter critter)
	{
		base.OnSpawn(critter);
		(critter as CosmeticCritterShadeFleeing).SetFleePosition(this.spawnPosition, base.transform.position);
	}

	// Token: 0x040004A2 RID: 1186
	private Vector3 spawnPosition;
}
