using System;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class MonkeyeProjectileTarget : MonoBehaviour
{
	// Token: 0x06000E74 RID: 3700 RVA: 0x00049079 File Offset: 0x00047279
	private void Awake()
	{
		this.monkeyeAI = base.GetComponent<MonkeyeAI>();
		this.notifier = base.GetComponentInChildren<SlingshotProjectileHitNotifier>();
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x00049093 File Offset: 0x00047293
	private void OnEnable()
	{
		if (this.notifier != null)
		{
			this.notifier.OnProjectileHit += this.Notifier_OnProjectileHit;
			this.notifier.OnPaperPlaneHit += this.Notifier_OnPaperPlaneHit;
		}
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x000490D1 File Offset: 0x000472D1
	private void OnDisable()
	{
		if (this.notifier != null)
		{
			this.notifier.OnProjectileHit -= this.Notifier_OnProjectileHit;
			this.notifier.OnPaperPlaneHit -= this.Notifier_OnPaperPlaneHit;
		}
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x0004910F File Offset: 0x0004730F
	private void Notifier_OnProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		this.monkeyeAI.SetSleep();
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0004910F File Offset: 0x0004730F
	private void Notifier_OnPaperPlaneHit(PaperPlaneProjectile projectile, Collider collider)
	{
		this.monkeyeAI.SetSleep();
	}

	// Token: 0x040011AD RID: 4525
	private MonkeyeAI monkeyeAI;

	// Token: 0x040011AE RID: 4526
	private SlingshotProjectileHitNotifier notifier;
}
