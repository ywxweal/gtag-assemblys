using System;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class CosmeticCritterCatcherButterflyNet : CosmeticCritterCatcher
{
	// Token: 0x0600041A RID: 1050 RVA: 0x00017FE4 File Offset: 0x000161E4
	public override CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter)
	{
		if (!(critter is CosmeticCritterButterfly) || (critter.transform.position - this.velocityEstimator.transform.position).sqrMagnitude > this.maxCatchRadius * this.maxCatchRadius || this.velocityEstimator.linearVelocity.sqrMagnitude < this.minCatchSpeed * this.minCatchSpeed)
		{
			return CosmeticCritterAction.None;
		}
		return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn;
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00018058 File Offset: 0x00016258
	public override bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		return base.ValidateRemoteCatchAction(critter, catchAction, serverTime) && critter is CosmeticCritterButterfly && (critter.transform.position - this.velocityEstimator.transform.position).sqrMagnitude <= this.maxCatchRadius * this.maxCatchRadius + 1f && this.velocityEstimator.linearVelocity.sqrMagnitude >= this.minCatchSpeed * this.minCatchSpeed - 1f && catchAction == (CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn);
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x000180E3 File Offset: 0x000162E3
	public override void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		this.caughtButterflyParticleSystem.Emit((critter as CosmeticCritterButterfly).GetEmitParams, 1);
		this.catchFX.Play();
		this.catchSFX.Play();
	}

	// Token: 0x0400048F RID: 1167
	[Tooltip("Use this for calculating the catch position and velocity.")]
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000490 RID: 1168
	[Tooltip("Catch the Butterfly if it is within this radius.")]
	[SerializeField]
	private float maxCatchRadius;

	// Token: 0x04000491 RID: 1169
	[Tooltip("Only catch the Butterfly if the net is moving faster than this speed.")]
	[SerializeField]
	private float minCatchSpeed;

	// Token: 0x04000492 RID: 1170
	[Tooltip("Spawn a particle inside the net representing the caught Butterfly.")]
	[SerializeField]
	private ParticleSystem caughtButterflyParticleSystem;

	// Token: 0x04000493 RID: 1171
	[Tooltip("Play this particle effect when catching a Butterfly.")]
	[SerializeField]
	private ParticleSystem catchFX;

	// Token: 0x04000494 RID: 1172
	[Tooltip("Play this sound when catching a Butterfly.")]
	[SerializeField]
	private AudioSource catchSFX;
}
