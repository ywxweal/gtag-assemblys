using System;
using System.Collections;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000CB7 RID: 3255
	public class TagEffectsLibrary : MonoBehaviour
	{
		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x0600507B RID: 20603 RVA: 0x00180717 File Offset: 0x0017E917
		public static float FistBumpSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.fistBumpSpeedThreshold;
			}
		}

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x0600507C RID: 20604 RVA: 0x00180723 File Offset: 0x0017E923
		public static float HighFiveSpeedThreshold
		{
			get
			{
				return TagEffectsLibrary._instance.highFiveSpeedThreshold;
			}
		}

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x0600507D RID: 20605 RVA: 0x0018072F File Offset: 0x0017E92F
		public static bool DebugMode
		{
			get
			{
				return TagEffectsLibrary._instance.debugMode;
			}
		}

		// Token: 0x0600507E RID: 20606 RVA: 0x0018073B File Offset: 0x0017E93B
		private void Awake()
		{
			if (TagEffectsLibrary._instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			TagEffectsLibrary._instance = this;
			this.tagEffectsPool = new Dictionary<string, Queue<GameObjectOnDisableDispatcher>>();
			this.tagEffectsComboLookUp = new Dictionary<TagEffectsCombo, TagEffectPack[]>();
		}

		// Token: 0x0600507F RID: 20607 RVA: 0x00180774 File Offset: 0x0017E974
		public static void PlayEffect(Transform target, bool isLeftHand, float rigScale, TagEffectsLibrary.EffectType effectType, TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack, Quaternion rotation)
		{
			if (TagEffectsLibrary._instance == null)
			{
				return;
			}
			ModeTagEffect modeTagEffect = null;
			TagEffectPack tagEffectPack = null;
			GameModeType gameModeType = ((GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual);
			for (int i = 0; i < TagEffectsLibrary._instance.defaultTagEffects.Length; i++)
			{
				if (TagEffectsLibrary._instance.defaultTagEffects[i] != null && TagEffectsLibrary._instance.defaultTagEffects[i].Modes.Contains(gameModeType))
				{
					modeTagEffect = TagEffectsLibrary._instance.defaultTagEffects[i];
					tagEffectPack = modeTagEffect.tagEffect;
					break;
				}
			}
			if (tagEffectPack == null)
			{
				return;
			}
			GameObject gameObject = tagEffectPack.firstPerson;
			GameObject gameObject2 = tagEffectPack.thirdPerson;
			GameObject gameObject3 = tagEffectPack.fistBump;
			GameObject gameObject4 = tagEffectPack.highFive;
			bool flag = tagEffectPack.firstPersonParentEffect;
			bool flag2 = tagEffectPack.thirdPersonParentEffect;
			bool flag3 = tagEffectPack.fistBumpParentEffect;
			bool flag4 = tagEffectPack.highFiveParentEffect;
			if (playerCosmeticTagEffectPack != null)
			{
				TagEffectPack tagEffectPack2 = TagEffectsLibrary.comboLookup(playerCosmeticTagEffectPack, otherPlayerCosmeticTagEffectPack);
				if (!modeTagEffect.blockFistBumpOverride && playerCosmeticTagEffectPack.fistBump != null)
				{
					gameObject3 = tagEffectPack2.fistBump;
					flag3 = tagEffectPack2.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockHiveFiveOverride && playerCosmeticTagEffectPack.highFive != null)
				{
					gameObject4 = tagEffectPack2.highFive;
					flag4 = tagEffectPack2.highFiveParentEffect;
				}
			}
			if (otherPlayerCosmeticTagEffectPack != null)
			{
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.firstPerson != null)
				{
					gameObject = otherPlayerCosmeticTagEffectPack.firstPerson;
					flag = otherPlayerCosmeticTagEffectPack.firstPersonParentEffect;
				}
				if (!modeTagEffect.blockTagOverride && otherPlayerCosmeticTagEffectPack.thirdPerson != null)
				{
					gameObject2 = otherPlayerCosmeticTagEffectPack.thirdPerson;
					flag2 = otherPlayerCosmeticTagEffectPack.thirdPersonParentEffect;
				}
			}
			switch (effectType)
			{
			case TagEffectsLibrary.EffectType.FIRST_PERSON:
				TagEffectsLibrary.placeEffects(gameObject, target, flag ? 1f : rigScale, false, flag, rotation);
				return;
			case TagEffectsLibrary.EffectType.THIRD_PERSON:
				TagEffectsLibrary.placeEffects(gameObject2, target, flag2 ? 1f : rigScale, false, flag2, rotation);
				return;
			case TagEffectsLibrary.EffectType.HIGH_FIVE:
				TagEffectsLibrary.placeEffects(gameObject4, target, flag4 ? 1f : rigScale, isLeftHand, flag4, rotation);
				return;
			case TagEffectsLibrary.EffectType.FIST_BUMP:
				TagEffectsLibrary.placeEffects(gameObject3, target, flag3 ? 1f : rigScale, isLeftHand, flag3, rotation);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005080 RID: 20608 RVA: 0x00180994 File Offset: 0x0017EB94
		private static TagEffectPack comboLookup(TagEffectPack playerCosmeticTagEffectPack, TagEffectPack otherPlayerCosmeticTagEffectPack)
		{
			if (otherPlayerCosmeticTagEffectPack == null)
			{
				return playerCosmeticTagEffectPack;
			}
			TagEffectsCombo tagEffectsCombo = new TagEffectsCombo();
			tagEffectsCombo.inputA = playerCosmeticTagEffectPack;
			tagEffectsCombo.inputB = otherPlayerCosmeticTagEffectPack;
			TagEffectPack[] array;
			if (!TagEffectsLibrary._instance.tagEffectsComboLookUp.TryGetValue(tagEffectsCombo, out array))
			{
				return playerCosmeticTagEffectPack;
			}
			int num = 0;
			if (GorillaComputer.instance != null)
			{
				num = GorillaComputer.instance.GetServerTime().Second;
			}
			return array[num % array.Length];
		}

		// Token: 0x06005081 RID: 20609 RVA: 0x00180A04 File Offset: 0x0017EC04
		public static void placeEffects(GameObject prefab, Transform target, float scale, bool flipZAxis, bool parentEffect, Quaternion rotation)
		{
			if (prefab == null)
			{
				return;
			}
			Queue<GameObjectOnDisableDispatcher> queue;
			if (!TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(prefab.name, out queue))
			{
				queue = new Queue<GameObjectOnDisableDispatcher>();
				TagEffectsLibrary._instance.tagEffectsPool.Add(prefab.name, queue);
			}
			if (queue.Count == 0 || (queue.Peek().gameObject.activeInHierarchy && queue.Count < 12))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(prefab, target.transform.position, rotation, parentEffect ? target : TagEffectsLibrary._instance.transform);
				gameObject.name = prefab.name;
				gameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
				GameObjectOnDisableDispatcher gameObjectOnDisableDispatcher;
				if (!gameObject.TryGetComponent<GameObjectOnDisableDispatcher>(out gameObjectOnDisableDispatcher))
				{
					gameObjectOnDisableDispatcher = gameObject.AddComponent<GameObjectOnDisableDispatcher>();
				}
				gameObjectOnDisableDispatcher.OnDisabled += TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				gameObject.SetActive(true);
				queue.Enqueue(gameObjectOnDisableDispatcher);
				return;
			}
			GameObjectOnDisableDispatcher gameObjectOnDisableDispatcher2 = queue.Dequeue();
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.RecycleGameObject(gameObjectOnDisableDispatcher2, target, scale, flipZAxis, parentEffect));
		}

		// Token: 0x06005082 RID: 20610 RVA: 0x00180B23 File Offset: 0x0017ED23
		private static void NewGameObjectOnDisableDispatcher_OnDisabled(GameObjectOnDisableDispatcher goodd)
		{
			TagEffectsLibrary._instance.StartCoroutine(TagEffectsLibrary._instance.ReclaimDisabled(goodd.transform));
		}

		// Token: 0x06005083 RID: 20611 RVA: 0x00180B40 File Offset: 0x0017ED40
		private IEnumerator RecycleGameObject(GameObjectOnDisableDispatcher recycledGameObject, Transform target, float scale, bool flipZAxis, bool parentEffect)
		{
			if (recycledGameObject.gameObject.activeInHierarchy)
			{
				recycledGameObject.gameObject.SetActive(false);
				recycledGameObject.OnDisabled -= TagEffectsLibrary.NewGameObjectOnDisableDispatcher_OnDisabled;
				yield return null;
			}
			recycledGameObject.transform.position = target.transform.position;
			recycledGameObject.transform.rotation = target.transform.rotation;
			recycledGameObject.transform.localScale = (flipZAxis ? new Vector3(scale, scale, -scale) : (Vector3.one * scale));
			recycledGameObject.transform.parent = (parentEffect ? target : TagEffectsLibrary._instance.transform);
			Queue<GameObjectOnDisableDispatcher> queue;
			if (TagEffectsLibrary._instance.tagEffectsPool.TryGetValue(recycledGameObject.gameObject.name, out queue))
			{
				recycledGameObject.gameObject.SetActive(true);
				queue.Enqueue(recycledGameObject);
			}
			yield break;
		}

		// Token: 0x06005084 RID: 20612 RVA: 0x00180B6D File Offset: 0x0017ED6D
		private IEnumerator ReclaimDisabled(Transform transform)
		{
			yield return null;
			transform.parent = TagEffectsLibrary._instance.transform;
			yield break;
		}

		// Token: 0x0400539E RID: 21406
		private const int OBJECT_QUEUE_LIMIT = 12;

		// Token: 0x0400539F RID: 21407
		[OnEnterPlay_SetNull]
		private static TagEffectsLibrary _instance;

		// Token: 0x040053A0 RID: 21408
		[SerializeField]
		private float fistBumpSpeedThreshold = 1f;

		// Token: 0x040053A1 RID: 21409
		[SerializeField]
		private float highFiveSpeedThreshold = 1f;

		// Token: 0x040053A2 RID: 21410
		[SerializeField]
		private ModeTagEffect[] defaultTagEffects;

		// Token: 0x040053A3 RID: 21411
		[SerializeField]
		private TagEffectsComboResult[] tagEffectsCombos;

		// Token: 0x040053A4 RID: 21412
		[SerializeField]
		private bool debugMode;

		// Token: 0x040053A5 RID: 21413
		private Dictionary<string, Queue<GameObjectOnDisableDispatcher>> tagEffectsPool;

		// Token: 0x040053A6 RID: 21414
		private Dictionary<TagEffectsCombo, TagEffectPack[]> tagEffectsComboLookUp;

		// Token: 0x02000CB8 RID: 3256
		public enum EffectType
		{
			// Token: 0x040053A8 RID: 21416
			FIRST_PERSON,
			// Token: 0x040053A9 RID: 21417
			THIRD_PERSON,
			// Token: 0x040053AA RID: 21418
			HIGH_FIVE,
			// Token: 0x040053AB RID: 21419
			FIST_BUMP
		}
	}
}
