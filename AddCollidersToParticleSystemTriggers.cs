using System;
using UnityEngine;

// Token: 0x020004A8 RID: 1192
public class AddCollidersToParticleSystemTriggers : MonoBehaviour
{
	// Token: 0x06001CCD RID: 7373 RVA: 0x0008BED4 File Offset: 0x0008A0D4
	private void Update()
	{
		this.count = 0;
		while (this.count < 6)
		{
			this.index++;
			if (this.index >= this.collidersToAdd.Length)
			{
				if (BetterDayNightManager.instance.collidersToAddToWeatherSystems.Count >= this.index - this.collidersToAdd.Length)
				{
					this.index = 0;
				}
				else
				{
					this.particleSystemToUpdate.trigger.SetCollider(this.count, BetterDayNightManager.instance.collidersToAddToWeatherSystems[this.index - this.collidersToAdd.Length]);
				}
			}
			if (this.index < this.collidersToAdd.Length)
			{
				this.particleSystemToUpdate.trigger.SetCollider(this.count, this.collidersToAdd[this.index]);
			}
			this.count++;
		}
	}

	// Token: 0x04002009 RID: 8201
	public Collider[] collidersToAdd;

	// Token: 0x0400200A RID: 8202
	public ParticleSystem particleSystemToUpdate;

	// Token: 0x0400200B RID: 8203
	private int count;

	// Token: 0x0400200C RID: 8204
	private int index;
}
