using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaTag.Rendering
{
	// Token: 0x02000D98 RID: 3480
	public class WaterBubbleParticleVolumeCollector : MonoBehaviour
	{
		// Token: 0x06005662 RID: 22114 RVA: 0x001A49C0 File Offset: 0x001A2BC0
		protected void Awake()
		{
			List<WaterVolume> componentsInHierarchy = SceneManager.GetActiveScene().GetComponentsInHierarchy(true, 64);
			List<Collider> list = new List<Collider>(componentsInHierarchy.Count * 4);
			foreach (WaterVolume waterVolume in componentsInHierarchy)
			{
				if (!(waterVolume.Parameters != null) || waterVolume.Parameters.allowBubblesInVolume)
				{
					foreach (Collider collider in waterVolume.volumeColliders)
					{
						if (!(collider == null))
						{
							list.Add(collider);
						}
					}
				}
			}
			this.bubbleableVolumeColliders = list.ToArray();
			this.particleTriggerModules = new ParticleSystem.TriggerModule[this.particleSystems.Length];
			this.particleEmissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleTriggerModules[i] = this.particleSystems[i].trigger;
				this.particleEmissionModules[i] = this.particleSystems[i].emission;
			}
			for (int j = 0; j < this.particleSystems.Length; j++)
			{
				ParticleSystem.TriggerModule triggerModule = this.particleTriggerModules[j];
				for (int k = 0; k < list.Count; k++)
				{
					triggerModule.SetCollider(k, this.bubbleableVolumeColliders[k]);
				}
			}
			this.SetEmissionState(false);
		}

		// Token: 0x06005663 RID: 22115 RVA: 0x001A4B60 File Offset: 0x001A2D60
		protected void LateUpdate()
		{
			bool headInWater = GTPlayer.Instance.HeadInWater;
			if (headInWater && !this.emissionEnabled)
			{
				this.SetEmissionState(true);
				return;
			}
			if (!headInWater && this.emissionEnabled)
			{
				this.SetEmissionState(false);
			}
		}

		// Token: 0x06005664 RID: 22116 RVA: 0x001A4BA0 File Offset: 0x001A2DA0
		private void SetEmissionState(bool setEnabled)
		{
			float num = (setEnabled ? 1f : 0f);
			for (int i = 0; i < this.particleEmissionModules.Length; i++)
			{
				this.particleEmissionModules[i].rateOverTimeMultiplier = num;
			}
			this.emissionEnabled = setEnabled;
		}

		// Token: 0x04005A29 RID: 23081
		public ParticleSystem[] particleSystems;

		// Token: 0x04005A2A RID: 23082
		private ParticleSystem.TriggerModule[] particleTriggerModules;

		// Token: 0x04005A2B RID: 23083
		private ParticleSystem.EmissionModule[] particleEmissionModules;

		// Token: 0x04005A2C RID: 23084
		private Collider[] bubbleableVolumeColliders;

		// Token: 0x04005A2D RID: 23085
		private bool emissionEnabled;
	}
}
