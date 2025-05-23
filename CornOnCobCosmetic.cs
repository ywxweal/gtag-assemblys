using System;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class CornOnCobCosmetic : MonoBehaviour
{
	// Token: 0x0600059D RID: 1437 RVA: 0x00020890 File Offset: 0x0001EA90
	protected void Awake()
	{
		this.emissionModule = this.particleSys.emission;
		this.maxBurstProbability = ((this.emissionModule.burstCount > 0) ? this.emissionModule.GetBurst(0).probability : 0.2f);
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x000208E0 File Offset: 0x0001EAE0
	protected void LateUpdate()
	{
		for (int i = 0; i < this.emissionModule.burstCount; i++)
		{
			ParticleSystem.Burst burst = this.emissionModule.GetBurst(i);
			burst.probability = this.maxBurstProbability * this.particleEmissionCurve.Evaluate(this.thermalReceiver.celsius);
			this.emissionModule.SetBurst(i, burst);
		}
		int particleCount = this.particleSys.particleCount;
		if (particleCount > this.previousParticleCount)
		{
			this.soundBankPlayer.Play();
		}
		this.previousParticleCount = particleCount;
	}

	// Token: 0x040006A3 RID: 1699
	[Tooltip("The corn will start popping based on the temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x040006A4 RID: 1700
	[Tooltip("The particle system that will be emitted when the heat source is hot enough.")]
	public ParticleSystem particleSys;

	// Token: 0x040006A5 RID: 1701
	[Tooltip("The curve that determines how many particles will be emitted based on the heat source's temperature.\n\nThe x-axis is the heat source's temperature and the y-axis is the number of particles to emit.")]
	public AnimationCurve particleEmissionCurve;

	// Token: 0x040006A6 RID: 1702
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x040006A7 RID: 1703
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x040006A8 RID: 1704
	private float maxBurstProbability;

	// Token: 0x040006A9 RID: 1705
	private int previousParticleCount;
}
