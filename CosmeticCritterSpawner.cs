using System;
using UnityEngine;

// Token: 0x02000534 RID: 1332
public abstract class CosmeticCritterSpawner : CosmeticCritterHoldable
{
	// Token: 0x06002060 RID: 8288 RVA: 0x000A2E8C File Offset: 0x000A108C
	public GameObject GetCritterPrefab()
	{
		return this.critterPrefab;
	}

	// Token: 0x06002061 RID: 8289 RVA: 0x000A2E94 File Offset: 0x000A1094
	public CosmeticCritter GetCritter()
	{
		return this.cachedCritter;
	}

	// Token: 0x06002062 RID: 8290 RVA: 0x000A2E9C File Offset: 0x000A109C
	public Type GetCritterType()
	{
		return this.cachedType;
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void SetRandomVariables(CosmeticCritter critter)
	{
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x000A2EA4 File Offset: 0x000A10A4
	public virtual void OnSpawn(CosmeticCritter critter)
	{
		this.numCritters++;
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x000A2EB4 File Offset: 0x000A10B4
	public virtual void OnDespawn(CosmeticCritter critter)
	{
		this.numCritters = Math.Max(this.numCritters - 1, 0);
	}

	// Token: 0x06002066 RID: 8294 RVA: 0x000A2ECA File Offset: 0x000A10CA
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.cachedCritter == null)
		{
			this.cachedCritter = this.critterPrefab.GetComponent<CosmeticCritter>();
			this.cachedType = this.cachedCritter.GetType();
		}
	}

	// Token: 0x06002067 RID: 8295 RVA: 0x000A2F02 File Offset: 0x000A1102
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x0400245E RID: 9310
	[Tooltip("The critter prefab to spawn.")]
	[SerializeField]
	protected GameObject critterPrefab;

	// Token: 0x0400245F RID: 9311
	[Tooltip("The maximum number of critters that this spawner can have active at once.")]
	[SerializeField]
	protected int maxCritters;

	// Token: 0x04002460 RID: 9312
	protected CosmeticCritter cachedCritter;

	// Token: 0x04002461 RID: 9313
	protected Type cachedType;

	// Token: 0x04002462 RID: 9314
	protected int numCritters;

	// Token: 0x04002463 RID: 9315
	protected float nextLocalSpawnTime;
}
