using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Reactions
{
	// Token: 0x02000D4A RID: 3402
	public class FireInstance : MonoBehaviour
	{
		// Token: 0x06005524 RID: 21796 RVA: 0x0019E749 File Offset: 0x0019C949
		protected void Awake()
		{
			FireManager.Register(this);
		}

		// Token: 0x06005525 RID: 21797 RVA: 0x0019E751 File Offset: 0x0019C951
		protected void OnDestroy()
		{
			FireManager.Unregister(this);
		}

		// Token: 0x06005526 RID: 21798 RVA: 0x0019E759 File Offset: 0x0019C959
		protected void OnEnable()
		{
			FireManager.OnEnable(this);
		}

		// Token: 0x06005527 RID: 21799 RVA: 0x0019E761 File Offset: 0x0019C961
		protected void OnDisable()
		{
			FireManager.OnDisable(this);
		}

		// Token: 0x06005528 RID: 21800 RVA: 0x0019E769 File Offset: 0x0019C969
		protected void OnTriggerEnter(Collider other)
		{
			FireManager.OnTriggerEnter(this, other);
		}

		// Token: 0x04005863 RID: 22627
		[Header("Scene References")]
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[SerializeField]
		internal Collider _collider;

		// Token: 0x04005864 RID: 22628
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[FormerlySerializedAs("_thermalSourceVolume")]
		[SerializeField]
		internal ThermalSourceVolume _thermalVolume;

		// Token: 0x04005865 RID: 22629
		[SerializeField]
		internal ParticleSystem _particleSystem;

		// Token: 0x04005866 RID: 22630
		[FormerlySerializedAs("_audioSource")]
		[SerializeField]
		internal AudioSource _loopingAudioSource;

		// Token: 0x04005867 RID: 22631
		[Tooltip("The emissive color will be darkened on the materials of these renderers as the fire is extinguished.")]
		[SerializeField]
		internal Renderer[] _emissiveRenderers;

		// Token: 0x04005868 RID: 22632
		[Header("Asset References")]
		[SerializeField]
		internal GTDirectAssetRef<AudioClip> _extinguishSound;

		// Token: 0x04005869 RID: 22633
		[SerializeField]
		internal float _extinguishSoundVolume = 1f;

		// Token: 0x0400586A RID: 22634
		[SerializeField]
		internal GTDirectAssetRef<AudioClip> _igniteSound;

		// Token: 0x0400586B RID: 22635
		[SerializeField]
		internal float _igniteSoundVolume = 1f;

		// Token: 0x0400586C RID: 22636
		[Header("Values")]
		[SerializeField]
		internal bool _despawnOnExtinguish = true;

		// Token: 0x0400586D RID: 22637
		[SerializeField]
		internal float _maxLifetime = 10f;

		// Token: 0x0400586E RID: 22638
		[Tooltip("How long it should take to reheat to it's default temperature.")]
		[SerializeField]
		internal float _reheatSpeed = 1f;

		// Token: 0x0400586F RID: 22639
		[Tooltip("If you completely extinguish the object, how long should it stay extinguished?")]
		[SerializeField]
		internal float _stayExtinguishedDuration = 1f;

		// Token: 0x04005870 RID: 22640
		internal float _defaultTemperature;

		// Token: 0x04005871 RID: 22641
		internal float _timeSinceExtinguished;

		// Token: 0x04005872 RID: 22642
		internal float _timeSinceDyingStart;

		// Token: 0x04005873 RID: 22643
		internal float _timeAlive;

		// Token: 0x04005874 RID: 22644
		internal float _psDefaultEmissionRate;

		// Token: 0x04005875 RID: 22645
		internal ParticleSystem.EmissionModule _psEmissionModule;

		// Token: 0x04005876 RID: 22646
		internal Vector3Int _spatialGridPosition;

		// Token: 0x04005877 RID: 22647
		internal bool _isDespawning;

		// Token: 0x04005878 RID: 22648
		internal float _deathStateDuration;

		// Token: 0x04005879 RID: 22649
		internal MaterialPropertyBlock[] _emiRenderers_matPropBlocks;

		// Token: 0x0400587A RID: 22650
		internal Color[] _emiRenderers_defaultColors;
	}
}
