using System;
using UnityEngine;

// Token: 0x02000530 RID: 1328
public abstract class CosmeticCritterCatcher : CosmeticCritterHoldable
{
	// Token: 0x0600203B RID: 8251 RVA: 0x000A24AF File Offset: 0x000A06AF
	public CosmeticCritterSpawner GetLinkedSpawner()
	{
		return this.optionalLinkedSpawner;
	}

	// Token: 0x0600203C RID: 8252
	public abstract CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter);

	// Token: 0x0600203D RID: 8253 RVA: 0x000A24B7 File Offset: 0x000A06B7
	public virtual bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		return this.callLimiter.CheckCallServerTime(serverTime);
	}

	// Token: 0x0600203E RID: 8254
	public abstract void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime);

	// Token: 0x0600203F RID: 8255 RVA: 0x000A24C5 File Offset: 0x000A06C5
	protected override void OnEnable()
	{
		base.OnEnable();
		CosmeticCritterManager.Instance.RegisterCatcher(this);
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x000A24D8 File Offset: 0x000A06D8
	protected override void OnDisable()
	{
		base.OnDisable();
		CosmeticCritterManager.Instance.UnregisterCatcher(this);
	}

	// Token: 0x04002448 RID: 9288
	[SerializeField]
	[Tooltip("If this catcher is capable of spawning immediately after catching, the linked spawner must be assigned here.")]
	protected CosmeticCritterSpawner optionalLinkedSpawner;
}
