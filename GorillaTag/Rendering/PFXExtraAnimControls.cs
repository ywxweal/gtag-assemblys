using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000D9D RID: 3485
	public class PFXExtraAnimControls : MonoBehaviour
	{
		// Token: 0x06005671 RID: 22129 RVA: 0x001A4DB0 File Offset: 0x001A2FB0
		protected void Awake()
		{
			this.emissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			this.cachedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			this.adjustedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.particleSystems[i].emission;
				this.cachedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				this.adjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				for (int j = 0; j < emission.burstCount; j++)
				{
					this.cachedEmitBursts[i][j] = emission.GetBurst(j);
					this.adjustedEmitBursts[i][j] = emission.GetBurst(j);
				}
				this.emissionModules[i] = emission;
			}
		}

		// Token: 0x06005672 RID: 22130 RVA: 0x001A4E90 File Offset: 0x001A3090
		protected void LateUpdate()
		{
			for (int i = 0; i < this.emissionModules.Length; i++)
			{
				this.emissionModules[i].rateOverTimeMultiplier = this.emitRateMult;
				Mathf.Min(this.emissionModules[i].burstCount, this.cachedEmitBursts[i].Length);
				for (int j = 0; j < this.cachedEmitBursts[i].Length; j++)
				{
					this.adjustedEmitBursts[i][j].probability = this.cachedEmitBursts[i][j].probability * this.emitBurstProbabilityMult;
				}
				this.emissionModules[i].SetBursts(this.adjustedEmitBursts[i]);
			}
		}

		// Token: 0x04005A34 RID: 23092
		public float emitRateMult = 1f;

		// Token: 0x04005A35 RID: 23093
		public float emitBurstProbabilityMult = 1f;

		// Token: 0x04005A36 RID: 23094
		[SerializeField]
		private ParticleSystem[] particleSystems;

		// Token: 0x04005A37 RID: 23095
		private ParticleSystem.EmissionModule[] emissionModules;

		// Token: 0x04005A38 RID: 23096
		private ParticleSystem.Burst[][] cachedEmitBursts;

		// Token: 0x04005A39 RID: 23097
		private ParticleSystem.Burst[][] adjustedEmitBursts;
	}
}
