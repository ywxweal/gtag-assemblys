using System;
using GorillaExtensions;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000CB3 RID: 3251
	public class HandEffectsTrigger : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x0600505C RID: 20572 RVA: 0x0017FFAF File Offset: 0x0017E1AF
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x0600505D RID: 20573 RVA: 0x0017FFB8 File Offset: 0x0017E1B8
		public bool FingersDown
		{
			get
			{
				return !(this.rig == null) && ((this.rightHand && this.rig.IsMakingFistRight()) || (!this.rightHand && this.rig.IsMakingFistLeft()));
			}
		}

		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x0600505E RID: 20574 RVA: 0x00180004 File Offset: 0x0017E204
		public bool FingersUp
		{
			get
			{
				return !(this.rig == null) && ((this.rightHand && this.rig.IsMakingFiveRight()) || (!this.rightHand && this.rig.IsMakingFiveLeft()));
			}
		}

		// Token: 0x170007F7 RID: 2039
		// (get) Token: 0x0600505F RID: 20575 RVA: 0x00180050 File Offset: 0x0017E250
		public Vector3 Velocity
		{
			get
			{
				if (this.velocityEstimator != null && this.rig != null && this.rig.scaleFactor > 0.001f)
				{
					return this.velocityEstimator.linearVelocity / this.rig.scaleFactor;
				}
				return Vector3.zero;
			}
		}

		// Token: 0x170007F8 RID: 2040
		// (get) Token: 0x06005060 RID: 20576 RVA: 0x001800AC File Offset: 0x0017E2AC
		bool IHandEffectsTrigger.RightHand
		{
			get
			{
				return this.rightHand;
			}
		}

		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x06005061 RID: 20577 RVA: 0x001800B4 File Offset: 0x0017E2B4
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x06005062 RID: 20578 RVA: 0x00045F89 File Offset: 0x00044189
		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x170007FB RID: 2043
		// (get) Token: 0x06005063 RID: 20579 RVA: 0x001800BC File Offset: 0x0017E2BC
		public VRRig Rig
		{
			get
			{
				return this.rig;
			}
		}

		// Token: 0x170007FC RID: 2044
		// (get) Token: 0x06005064 RID: 20580 RVA: 0x001800C4 File Offset: 0x0017E2C4
		public TagEffectPack CosmeticEffectPack
		{
			get
			{
				if (this.rig == null)
				{
					return null;
				}
				return this.rig.CosmeticEffectPack;
			}
		}

		// Token: 0x06005065 RID: 20581 RVA: 0x001800E4 File Offset: 0x0017E2E4
		private void Awake()
		{
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.velocityEstimator == null)
			{
				this.velocityEstimator = base.GetComponentInParent<GorillaVelocityEstimator>();
			}
			for (int i = 0; i < this.debugVisuals.Length; i++)
			{
				this.debugVisuals[i].SetActive(TagEffectsLibrary.DebugMode);
			}
		}

		// Token: 0x06005066 RID: 20582 RVA: 0x00045FDF File Offset: 0x000441DF
		private void OnEnable()
		{
			if (!HandEffectsTriggerRegistry.HasInstance)
			{
				HandEffectsTriggerRegistry.FindInstance();
			}
			HandEffectsTriggerRegistry.Instance.Register(this);
		}

		// Token: 0x06005067 RID: 20583 RVA: 0x00045FF8 File Offset: 0x000441F8
		private void OnDisable()
		{
			HandEffectsTriggerRegistry.Instance.Unregister(this);
		}

		// Token: 0x06005068 RID: 20584 RVA: 0x0018013C File Offset: 0x0017E33C
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
			if (this.rig == other.Rig)
			{
				return;
			}
			if (this.FingersDown && other.FingersDown && (other.Static || (Vector3.Dot(Vector3.Dot(this.Velocity, base.transform.up) * base.transform.up - Vector3.Dot(other.Velocity, other.Transform.up) * other.Transform.up, -other.Transform.up) > TagEffectsLibrary.FistBumpSpeedThreshold && Vector3.Dot(base.transform.up, other.Transform.up) < -0.01f)))
			{
				this.PlayHandEffects(TagEffectsLibrary.EffectType.FIST_BUMP, other);
			}
			if (this.FingersUp && other.FingersUp && (other.Static || Mathf.Abs(Vector3.Dot(Vector3.Dot(this.Velocity, base.transform.right) * base.transform.right - Vector3.Dot(other.Velocity, other.Transform.right) * other.Transform.right, other.Transform.right)) > TagEffectsLibrary.HighFiveSpeedThreshold))
			{
				this.PlayHandEffects(TagEffectsLibrary.EffectType.HIGH_FIVE, other);
			}
		}

		// Token: 0x06005069 RID: 20585 RVA: 0x001802A8 File Offset: 0x0017E4A8
		private void PlayHandEffects(TagEffectsLibrary.EffectType effectType, IHandEffectsTrigger other)
		{
			bool flag = false;
			if (this.rig.isOfflineVRRig)
			{
				PlayerGameEvents.TriggerHandEffect(effectType.ToString());
			}
			HandEffectsOverrideCosmetic handEffectsOverrideCosmetic = null;
			HandEffectsOverrideCosmetic handEffectsOverrideCosmetic2 = null;
			foreach (HandEffectsOverrideCosmetic handEffectsOverrideCosmetic3 in (this.rightHand ? this.rig.CosmeticHandEffectsOverride_Right : this.rig.CosmeticHandEffectsOverride_Left))
			{
				if (handEffectsOverrideCosmetic3.handEffectType == this.MapEnum(effectType))
				{
					handEffectsOverrideCosmetic2 = handEffectsOverrideCosmetic3;
					break;
				}
			}
			if (this.rig != null && this.rig.isOfflineVRRig && GorillaTagger.Instance != null)
			{
				if (other.Rig)
				{
					foreach (HandEffectsOverrideCosmetic handEffectsOverrideCosmetic4 in ((other.Rig.CosmeticHandEffectsOverride_Right != null) ? other.Rig.CosmeticHandEffectsOverride_Right : other.Rig.CosmeticHandEffectsOverride_Left))
					{
						if (handEffectsOverrideCosmetic4.handEffectType == this.MapEnum(effectType))
						{
							handEffectsOverrideCosmetic = handEffectsOverrideCosmetic4;
							break;
						}
					}
					if (handEffectsOverrideCosmetic && handEffectsOverrideCosmetic.handEffectType == this.MapEnum(effectType) && ((!handEffectsOverrideCosmetic.isLeftHand && other.RightHand) || (handEffectsOverrideCosmetic.isLeftHand && !other.RightHand)))
					{
						if (handEffectsOverrideCosmetic.thirdPerson.playHaptics)
						{
							GorillaTagger.Instance.StartVibration(!this.rightHand, handEffectsOverrideCosmetic.thirdPerson.hapticStrength, handEffectsOverrideCosmetic.thirdPerson.hapticDuration);
						}
						TagEffectsLibrary.placeEffects(handEffectsOverrideCosmetic.thirdPerson.effectVFX, base.transform, this.rig.scaleFactor, false, handEffectsOverrideCosmetic.thirdPerson.parentEffect, base.transform.rotation);
						flag = true;
					}
				}
				if (handEffectsOverrideCosmetic2 && handEffectsOverrideCosmetic2.handEffectType == this.MapEnum(effectType) && ((handEffectsOverrideCosmetic2.isLeftHand && !this.rightHand) || (!handEffectsOverrideCosmetic2.isLeftHand && this.rightHand)))
				{
					if (handEffectsOverrideCosmetic2.firstPerson.playHaptics)
					{
						GorillaTagger.Instance.StartVibration(!this.rightHand, handEffectsOverrideCosmetic2.firstPerson.hapticStrength, handEffectsOverrideCosmetic2.firstPerson.hapticDuration);
					}
					TagEffectsLibrary.placeEffects(handEffectsOverrideCosmetic2.firstPerson.effectVFX, other.Transform, this.rig.scaleFactor, false, handEffectsOverrideCosmetic2.firstPerson.parentEffect, other.Transform.rotation);
					flag = true;
				}
				if (!flag)
				{
					GorillaTagger.Instance.StartVibration(!this.rightHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
					TagEffectsLibrary.PlayEffect(base.transform, !this.rightHand, this.rig.scaleFactor, effectType, this.CosmeticEffectPack, other.CosmeticEffectPack, base.transform.rotation);
				}
			}
		}

		// Token: 0x0600506A RID: 20586 RVA: 0x001805C4 File Offset: 0x0017E7C4
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return (base.transform.position - t.Transform.position).IsShorterThan(this.triggerRadius * this.rig.scaleFactor);
		}

		// Token: 0x0600506B RID: 20587 RVA: 0x001805F8 File Offset: 0x0017E7F8
		private HandEffectsOverrideCosmetic.HandEffectType MapEnum(TagEffectsLibrary.EffectType oldEnum)
		{
			return HandEffectsTrigger.mappingArray[(int)oldEnum];
		}

		// Token: 0x04005386 RID: 21382
		[SerializeField]
		private float triggerRadius = 0.07f;

		// Token: 0x04005387 RID: 21383
		[SerializeField]
		private bool rightHand;

		// Token: 0x04005388 RID: 21384
		[SerializeField]
		private bool isStatic;

		// Token: 0x04005389 RID: 21385
		private VRRig rig;

		// Token: 0x0400538A RID: 21386
		public GorillaVelocityEstimator velocityEstimator;

		// Token: 0x0400538B RID: 21387
		[SerializeField]
		private GameObject[] debugVisuals;

		// Token: 0x0400538D RID: 21389
		private static HandEffectsOverrideCosmetic.HandEffectType[] mappingArray = new HandEffectsOverrideCosmetic.HandEffectType[]
		{
			HandEffectsOverrideCosmetic.HandEffectType.None,
			HandEffectsOverrideCosmetic.HandEffectType.None,
			HandEffectsOverrideCosmetic.HandEffectType.HighFive,
			HandEffectsOverrideCosmetic.HandEffectType.FistBump
		};
	}
}
