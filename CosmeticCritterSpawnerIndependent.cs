using System;

// Token: 0x02000535 RID: 1333
public class CosmeticCritterSpawnerIndependent : CosmeticCritterSpawner
{
	// Token: 0x06002069 RID: 8297 RVA: 0x000A2F0A File Offset: 0x000A110A
	public virtual bool CanSpawnLocal()
	{
		return this.numCritters < this.maxCritters;
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x000A2F1A File Offset: 0x000A111A
	public virtual bool CanSpawnRemote(double serverTime)
	{
		return this.numCritters < this.maxCritters && this.callLimiter.CheckCallServerTime(serverTime);
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x000A2F38 File Offset: 0x000A1138
	protected override void OnEnable()
	{
		base.OnEnable();
		CosmeticCritterManager.Instance.RegisterIndependentSpawner(this);
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x000A2F4B File Offset: 0x000A114B
	protected override void OnDisable()
	{
		base.OnDisable();
		CosmeticCritterManager.Instance.UnregisterIndependentSpawner(this);
	}
}
