using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DC0 RID: 3520
	public class CosmeticEffectsOnPlayers : MonoBehaviour, ISpawnable
	{
		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x06005735 RID: 22325 RVA: 0x001AC283 File Offset: 0x001AA483
		private HashSet<GameModeType> Modes
		{
			get
			{
				if (this.modesHash == null)
				{
					this.modesHash = new HashSet<GameModeType>(this.supportingGameModes);
				}
				return this.modesHash;
			}
		}

		// Token: 0x06005736 RID: 22326 RVA: 0x001AC2A4 File Offset: 0x001AA4A4
		private bool IsMyItem()
		{
			return this.myRig != null && this.myRig.isOfflineVRRig;
		}

		// Token: 0x06005737 RID: 22327 RVA: 0x001AC2C4 File Offset: 0x001AA4C4
		private void Awake()
		{
			foreach (CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect in this.allEffects)
			{
				this.allEffectsDict.TryAdd(cosmeticEffect.effectType, cosmeticEffect);
			}
		}

		// Token: 0x06005738 RID: 22328 RVA: 0x001AC2FD File Offset: 0x001AA4FD
		public void ApplyAllEffects()
		{
			if (!this.CheckGameMode())
			{
				return;
			}
			this.ApplyAllEffectsByDistance(base.transform.position);
		}

		// Token: 0x06005739 RID: 22329 RVA: 0x001AC31C File Offset: 0x001AA51C
		public void ApplyAllEffectsByDistance(Vector3 position)
		{
			if (!this.CheckGameMode())
			{
				return;
			}
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.allEffectsDict)
			{
				if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.SKIN)
				{
					this.ApplySkinByDistance(keyValuePair, position);
				}
				else if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.FPV)
				{
					this.ApplyFPVEffectsByDistance(keyValuePair, position);
				}
				else if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.KNOCKBACK)
				{
					this.ApplyTagWithKnockbackByDistance(keyValuePair, position);
				}
			}
		}

		// Token: 0x0600573A RID: 22330 RVA: 0x001AC3AC File Offset: 0x001AA5AC
		public void ApplyAllEffectsForRig(VRRig rig)
		{
			if (!this.CheckGameMode())
			{
				return;
			}
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.allEffectsDict)
			{
				if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.SKIN)
				{
					this.ApplySkinForRig(keyValuePair, rig);
				}
				else if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.FPV)
				{
					this.ApplyFPVEffectsForRig(keyValuePair, rig);
				}
				else if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.KNOCKBACK)
				{
					this.ApplyTagWithKnockbackForRig(keyValuePair, rig);
				}
				else if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.VOICEOVERRIDE)
				{
					this.ApplyVOForRig(keyValuePair, rig);
				}
			}
		}

		// Token: 0x0600573B RID: 22331 RVA: 0x001AC450 File Offset: 0x001AA650
		private void ApplyFPVEffectsByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!this.CheckGameMode())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (!PhotonNetwork.InRoom)
			{
				if ((GorillaTagger.Instance.offlineVRRig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					effect.Value.effectDuration = effect.Value.effectDurationOwner;
					GorillaTagger.Instance.offlineVRRig.SpawnFPVEffects(effect, true);
					return;
				}
			}
			else
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (this.myRig == vrrig && this.IsMyItem())
					{
						effect.Value.effectDuration = effect.Value.effectDurationOwner;
					}
					if ((vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
					{
						vrrig.SpawnFPVEffects(effect, true);
					}
				}
			}
		}

		// Token: 0x0600573C RID: 22332 RVA: 0x001AC578 File Offset: 0x001AA778
		private void ApplyFPVEffectsForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig rig)
		{
			if (!this.CheckGameMode())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (rig == this.myRig && this.IsMyItem())
			{
				effect.Value.effectDuration = effect.Value.effectDurationOwner;
			}
			rig.SpawnFPVEffects(effect, true);
		}

		// Token: 0x0600573D RID: 22333 RVA: 0x001AC5D8 File Offset: 0x001AA7D8
		private void ApplySkinByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!this.CheckGameMode())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (!PhotonNetwork.InRoom)
			{
				if ((GorillaTagger.Instance.offlineVRRig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					effect.Value.effectDuration = effect.Value.effectDurationOwner;
					GorillaTagger.Instance.offlineVRRig.SpawnSkinEffects(effect);
					return;
				}
			}
			else
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (this.myRig == vrrig && this.IsMyItem())
					{
						effect.Value.effectDuration = effect.Value.effectDurationOwner;
					}
					if ((vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
					{
						vrrig.SpawnSkinEffects(effect);
					}
				}
			}
		}

		// Token: 0x0600573E RID: 22334 RVA: 0x001AC700 File Offset: 0x001AA900
		private void ApplySkinForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!this.CheckGameMode())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig && this.IsMyItem())
			{
				effect.Value.effectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.SpawnSkinEffects(effect);
		}

		// Token: 0x0600573F RID: 22335 RVA: 0x001AC75C File Offset: 0x001AA95C
		private void ApplyTagWithKnockbackForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!this.CheckGameMode())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig && this.IsMyItem())
			{
				effect.Value.effectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.EnableHitWithKnockBack(effect);
		}

		// Token: 0x06005740 RID: 22336 RVA: 0x001AC7B8 File Offset: 0x001AA9B8
		private void ApplyTagWithKnockbackByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!this.CheckGameMode())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (!PhotonNetwork.InRoom)
			{
				if ((GorillaTagger.Instance.offlineVRRig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					effect.Value.effectDuration = effect.Value.effectDurationOwner;
					GorillaTagger.Instance.offlineVRRig.EnableHitWithKnockBack(effect);
					return;
				}
			}
			else
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (this.myRig == vrrig && this.IsMyItem())
					{
						effect.Value.effectDuration = effect.Value.effectDurationOwner;
					}
					if ((vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
					{
						vrrig.EnableHitWithKnockBack(effect);
					}
				}
			}
		}

		// Token: 0x06005741 RID: 22337 RVA: 0x001AC8E0 File Offset: 0x001AAAE0
		private void ApplyVOForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig rig)
		{
			if (!this.CheckGameMode())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (rig == this.myRig && this.IsMyItem())
			{
				effect.Value.effectDuration = effect.Value.effectDurationOwner;
			}
			rig.ActivateVOEffect(effect);
		}

		// Token: 0x06005742 RID: 22338 RVA: 0x001AC93C File Offset: 0x001AAB3C
		private bool CheckGameMode()
		{
			GameModeType gameModeType = ((GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual);
			return this.Modes.Contains(gameModeType);
		}

		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x06005743 RID: 22339 RVA: 0x001AC975 File Offset: 0x001AAB75
		// (set) Token: 0x06005744 RID: 22340 RVA: 0x001AC97D File Offset: 0x001AAB7D
		public bool IsSpawned { get; set; }

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x06005745 RID: 22341 RVA: 0x001AC986 File Offset: 0x001AAB86
		// (set) Token: 0x06005746 RID: 22342 RVA: 0x001AC98E File Offset: 0x001AAB8E
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06005747 RID: 22343 RVA: 0x001AC997 File Offset: 0x001AAB97
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06005748 RID: 22344 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnDespawn()
		{
		}

		// Token: 0x04005BC4 RID: 23492
		public CosmeticEffectsOnPlayers.CosmeticEffect[] allEffects = new CosmeticEffectsOnPlayers.CosmeticEffect[0];

		// Token: 0x04005BC5 RID: 23493
		[SerializeField]
		private GameModeType[] supportingGameModes;

		// Token: 0x04005BC6 RID: 23494
		private VRRig myRig;

		// Token: 0x04005BC7 RID: 23495
		private HashSet<GameModeType> modesHash;

		// Token: 0x04005BC8 RID: 23496
		private Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> allEffectsDict = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

		// Token: 0x02000DC1 RID: 3521
		[Serializable]
		public class CosmeticEffect
		{
			// Token: 0x170008B8 RID: 2232
			// (get) Token: 0x0600574A RID: 22346 RVA: 0x001AC9BF File Offset: 0x001AABBF
			// (set) Token: 0x0600574B RID: 22347 RVA: 0x001AC9C7 File Offset: 0x001AABC7
			public float effectDuration
			{
				get
				{
					return this.effectDurationOthers;
				}
				set
				{
					this.effectDurationOthers = value;
				}
			}

			// Token: 0x170008B9 RID: 2233
			// (get) Token: 0x0600574C RID: 22348 RVA: 0x001AC9D0 File Offset: 0x001AABD0
			// (set) Token: 0x0600574D RID: 22349 RVA: 0x001AC9D8 File Offset: 0x001AABD8
			public float EffectStartedTime { get; set; }

			// Token: 0x0600574E RID: 22350 RVA: 0x001AC9E1 File Offset: 0x001AABE1
			private bool IsSkin()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.SKIN;
			}

			// Token: 0x0600574F RID: 22351 RVA: 0x001AC9EC File Offset: 0x001AABEC
			private bool IsFPV()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.FPV;
			}

			// Token: 0x06005750 RID: 22352 RVA: 0x001AC9F7 File Offset: 0x001AABF7
			private bool IsKnockback()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.KNOCKBACK;
			}

			// Token: 0x06005751 RID: 22353 RVA: 0x001ACA02 File Offset: 0x001AAC02
			private bool IsVO()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.VOICEOVERRIDE;
			}

			// Token: 0x04005BCB RID: 23499
			public CosmeticEffectsOnPlayers.EFFECTTYPE effectType;

			// Token: 0x04005BCC RID: 23500
			public float effectDistanceRadius;

			// Token: 0x04005BCD RID: 23501
			public float effectDurationOthers;

			// Token: 0x04005BCE RID: 23502
			public float effectDurationOwner;

			// Token: 0x04005BCF RID: 23503
			public GorillaSkin newSkin;

			// Token: 0x04005BD0 RID: 23504
			[Tooltip("Spawn effects for the FPV - Use object pools")]
			public GameObject FPVEffect;

			// Token: 0x04005BD1 RID: 23505
			public float knockbackStrengthMultiplier;

			// Token: 0x04005BD2 RID: 23506
			[Tooltip("Use object pools")]
			public GameObject knockbackVFX;

			// Token: 0x04005BD3 RID: 23507
			public AudioClip[] voiceOverrideNormalClips;

			// Token: 0x04005BD4 RID: 23508
			public AudioClip[] voiceOverrideLoudClips;

			// Token: 0x04005BD5 RID: 23509
			public float voiceOverrideNormalVolume = 0.5f;

			// Token: 0x04005BD6 RID: 23510
			public float voiceOverrideLoudVolume = 0.8f;

			// Token: 0x04005BD7 RID: 23511
			public float voiceOverrideLoudThreshold = 0.175f;
		}

		// Token: 0x02000DC2 RID: 3522
		public enum EFFECTTYPE
		{
			// Token: 0x04005BDA RID: 23514
			SKIN,
			// Token: 0x04005BDB RID: 23515
			FPV,
			// Token: 0x04005BDC RID: 23516
			KNOCKBACK,
			// Token: 0x04005BDD RID: 23517
			VOICEOVERRIDE
		}
	}
}
