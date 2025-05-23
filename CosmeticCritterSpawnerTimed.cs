using System;
using UnityEngine;

// Token: 0x02000536 RID: 1334
public abstract class CosmeticCritterSpawnerTimed : CosmeticCritterSpawnerIndependent
{
	// Token: 0x0600206E RID: 8302 RVA: 0x000A2F5E File Offset: 0x000A115E
	protected override CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(5, this.spawnIntervalMinMax.x, 0.5f);
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x000A2F76 File Offset: 0x000A1176
	public override bool CanSpawnLocal()
	{
		if (Time.time >= this.nextLocalSpawnTime)
		{
			this.nextLocalSpawnTime = Time.time + Random.Range(this.spawnIntervalMinMax.x, this.spawnIntervalMinMax.y);
			return base.CanSpawnLocal();
		}
		return false;
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x000A2FB4 File Offset: 0x000A11B4
	public override bool CanSpawnRemote(double serverTime)
	{
		return base.CanSpawnRemote(serverTime);
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x000A2FBD File Offset: 0x000A11BD
	protected override void OnEnable()
	{
		base.OnEnable();
		if (base.IsLocal)
		{
			this.nextLocalSpawnTime = Time.time + Random.Range(this.spawnIntervalMinMax.x, this.spawnIntervalMinMax.y);
		}
	}

	// Token: 0x06002072 RID: 8306 RVA: 0x000A2FF4 File Offset: 0x000A11F4
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x04002464 RID: 9316
	[Tooltip("The minimum and maximum time to wait between spawn attempts.")]
	[SerializeField]
	private Vector2 spawnIntervalMinMax = new Vector2(2f, 5f);

	// Token: 0x04002465 RID: 9317
	[Tooltip("Currently does nothing.")]
	[SerializeField]
	[Range(0f, 1f)]
	private float spawnChance = 1f;
}
