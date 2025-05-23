using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag.Audio;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x02000D4B RID: 3403
	public class FireManager : ITickSystemPost
	{
		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x0600552A RID: 21802 RVA: 0x0019E7C5 File Offset: 0x0019C9C5
		// (set) Token: 0x0600552B RID: 21803 RVA: 0x0019E7CC File Offset: 0x0019C9CC
		[OnEnterPlay_SetNull]
		internal static FireManager instance { get; private set; }

		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x0600552C RID: 21804 RVA: 0x0019E7D4 File Offset: 0x0019C9D4
		// (set) Token: 0x0600552D RID: 21805 RVA: 0x0019E7DB File Offset: 0x0019C9DB
		[OnEnterPlay_Set(false)]
		internal static bool hasInstance { get; private set; }

		// Token: 0x0600552E RID: 21806 RVA: 0x0019E7E3 File Offset: 0x0019C9E3
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Initialize()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (FireManager.hasInstance)
			{
				return;
			}
			FireManager.instance = new FireManager();
			FireManager.hasInstance = true;
			TickSystem<object>.AddPostTickCallback(FireManager.instance);
		}

		// Token: 0x0600552F RID: 21807 RVA: 0x0019E810 File Offset: 0x0019CA10
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Register(FireInstance f)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			int instanceID = f.gameObject.GetInstanceID();
			if (!FireManager._kGObjInstId_to_fire.TryAdd(instanceID, f))
			{
				if (f == null)
				{
					Debug.LogError("FireManager: You tried to register null!", f);
					return;
				}
				Debug.LogError("FireManager: \"" + f.name + "\" was attempted to be registered more than once!", f);
			}
			f.GetComponentAndSetFieldIfNullElseLogAndDisable(ref f._collider, "_collider", "Collider", "Disabling.", "Register");
			f.GetComponentAndSetFieldIfNullElseLogAndDisable(ref f._thermalVolume, "_thermalVolume", "ThermalSourceVolume", "Disabling.", "Register");
			f.GetComponentAndSetFieldIfNullElseLogAndDisable(ref f._particleSystem, "_particleSystem", "ParticleSystem", "Disabling.", "Register");
			f.GetComponentAndSetFieldIfNullElseLogAndDisable(ref f._loopingAudioSource, "_loopingAudioSource", "AudioSource", "Disabling.", "Register");
			f.DisableIfNull(f._extinguishSound.obj, "_extinguishSound", "AudioClip", "Register");
			f.DisableIfNull(f._igniteSound.obj, "_igniteSound", "AudioClip", "Register");
			f._defaultTemperature = f._thermalVolume.celsius;
			f._timeSinceExtinguished = -f._stayExtinguishedDuration;
			f._psEmissionModule = f._particleSystem.emission;
			f._psDefaultEmissionRate = f._psEmissionModule.rateOverTime.constant;
			f._deathStateDuration = 0f;
			if (f._emissiveRenderers != null)
			{
				f._emiRenderers_matPropBlocks = new MaterialPropertyBlock[f._emissiveRenderers.Length];
				f._emiRenderers_defaultColors = new Color[f._emissiveRenderers.Length];
				for (int i = 0; i < f._emissiveRenderers.Length; i++)
				{
					f._emiRenderers_matPropBlocks[i] = new MaterialPropertyBlock();
					f._emissiveRenderers[i].GetPropertyBlock(f._emiRenderers_matPropBlocks[i]);
					f._emiRenderers_defaultColors[i] = f._emiRenderers_matPropBlocks[i].GetColor(FireManager.shaderProp_EmissionColor);
				}
			}
		}

		// Token: 0x06005530 RID: 21808 RVA: 0x0019EA0C File Offset: 0x0019CC0C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Unregister(FireInstance reactable)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			int instanceID = reactable.gameObject.GetInstanceID();
			FireManager._kGObjInstId_to_fire.Remove(instanceID);
		}

		// Token: 0x06005531 RID: 21809 RVA: 0x0019EA3C File Offset: 0x0019CC3C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3Int GetSpatialGridPos(Vector3 pos)
		{
			Vector3 vector = pos / 0.2f;
			return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
		}

		// Token: 0x06005532 RID: 21810 RVA: 0x0019EA70 File Offset: 0x0019CC70
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ResetFireValues(FireInstance f)
		{
			f._timeSinceExtinguished = Mathf.Min(f._timeSinceExtinguished, f._stayExtinguishedDuration);
			f._timeSinceDyingStart = 0f;
			f._isDespawning = false;
			f._timeAlive = 0f;
			f._thermalVolume.celsius = f._defaultTemperature;
		}

		// Token: 0x06005533 RID: 21811 RVA: 0x0019EAC4 File Offset: 0x0019CCC4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SpawnFire(SinglePool pool, Vector3 pos, Vector3 normal, float scale)
		{
			int num;
			if (FireManager._fireSpatialGrid.TryGetValue(FireManager.GetSpatialGridPos(pos), out num))
			{
				FireManager.ResetFireValues(FireManager._kGObjInstId_to_fire[num]);
				return;
			}
			GameObject gameObject = pool.Instantiate(false);
			gameObject.transform.position = pos;
			gameObject.transform.up = normal;
			gameObject.transform.localScale = Vector3.one * scale;
			gameObject.SetActive(true);
		}

		// Token: 0x06005534 RID: 21812 RVA: 0x0019EB34 File Offset: 0x0019CD34
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void OnEnable(FireInstance f)
		{
			if (ApplicationQuittingState.IsQuitting || ObjectPools.instance == null || !ObjectPools.instance.initialized)
			{
				return;
			}
			FireManager.ResetFireValues(f);
			f._spatialGridPosition = FireManager.GetSpatialGridPos(f.transform.position);
			FireManager._fireSpatialGrid.Add(f._spatialGridPosition, f.gameObject.GetInstanceID());
			FireManager._kEnabledReactions.Add(f);
			if (GTAudioOneShot.isInitialized && Time.realtimeSinceStartup > 10f)
			{
				GTAudioOneShot.Play(f._igniteSound, f.transform.position, f._igniteSoundVolume, 1f);
			}
			if (8 > FireManager._activeAudioSources)
			{
				FireManager._activeAudioSources++;
				f._loopingAudioSource.enabled = true;
				return;
			}
			f._loopingAudioSource.enabled = false;
		}

		// Token: 0x06005535 RID: 21813 RVA: 0x0019EC0C File Offset: 0x0019CE0C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void OnDisable(FireInstance f)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			FireManager._kEnabledReactions.Remove(f);
			FireManager._fireSpatialGrid.Remove(f._spatialGridPosition);
			FireManager._activeAudioSources = Mathf.Min(FireManager._activeAudioSources - (f._loopingAudioSource.enabled ? 1 : 0), 0);
		}

		// Token: 0x06005536 RID: 21814 RVA: 0x0019EC60 File Offset: 0x0019CE60
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void OnTriggerEnter(FireInstance f, Collider other)
		{
			if (f._isDespawning || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (other.gameObject.layer == 4)
			{
				FireManager.Extinguish(f.gameObject, float.MaxValue);
			}
		}

		// Token: 0x06005537 RID: 21815 RVA: 0x0019EC90 File Offset: 0x0019CE90
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void Extinguish(GameObject gObj, float extinguishAmount)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			FireInstance fireInstance;
			if (!FireManager._kGObjInstId_to_fire.TryGetValue(gObj.GetInstanceID(), out fireInstance))
			{
				return;
			}
			float num = fireInstance._thermalVolume.celsius - extinguishAmount;
			if (num <= 0f && fireInstance._thermalVolume.celsius > 0.001f)
			{
				fireInstance._thermalVolume.celsius = Mathf.Max(num, 0f);
				fireInstance._timeSinceExtinguished = 0f;
				GTAudioOneShot.Play(fireInstance._extinguishSound, fireInstance.transform.position, fireInstance._extinguishSoundVolume, 1f);
				if (fireInstance._despawnOnExtinguish)
				{
					fireInstance._isDespawning = true;
					fireInstance._timeSinceDyingStart = 0f;
				}
			}
		}

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x06005538 RID: 21816 RVA: 0x0019ED43 File Offset: 0x0019CF43
		// (set) Token: 0x06005539 RID: 21817 RVA: 0x0019ED4B File Offset: 0x0019CF4B
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x0600553A RID: 21818 RVA: 0x0019ED54 File Offset: 0x0019CF54
		void ITickSystemPost.PostTick()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			foreach (FireInstance fireInstance in FireManager._kEnabledReactions)
			{
				fireInstance._timeAlive += Time.unscaledDeltaTime;
				bool flag = fireInstance._timeSinceExtinguished < fireInstance._stayExtinguishedDuration;
				fireInstance._timeSinceExtinguished += Time.unscaledDeltaTime;
				bool flag2 = fireInstance._timeSinceExtinguished < fireInstance._stayExtinguishedDuration;
				if (fireInstance._isDespawning)
				{
					fireInstance._timeSinceDyingStart += Time.unscaledDeltaTime;
					if (fireInstance._timeSinceDyingStart >= fireInstance._deathStateDuration || fireInstance._thermalVolume.celsius < -9999f)
					{
						FireManager._kFiresToDespawn.Add(fireInstance);
					}
				}
				if (!fireInstance._isDespawning && fireInstance._despawnOnExtinguish && fireInstance._timeAlive > fireInstance._maxLifetime)
				{
					fireInstance._isDespawning = true;
					fireInstance._timeSinceDyingStart = 0f;
					GTAudioOneShot.Play(fireInstance._extinguishSound, fireInstance.transform.position, fireInstance._extinguishSoundVolume, 1f);
				}
				if (!fireInstance._isDespawning && flag != flag2)
				{
					if (flag2)
					{
						if (fireInstance._despawnOnExtinguish)
						{
							fireInstance._isDespawning = true;
							fireInstance._timeSinceDyingStart = 0f;
						}
						GTAudioOneShot.Play(fireInstance._extinguishSound, fireInstance.transform.position, fireInstance._extinguishSoundVolume, 1f);
					}
					else
					{
						GTAudioOneShot.Play(fireInstance._igniteSound, fireInstance.transform.position, fireInstance._igniteSoundVolume, 1f);
					}
				}
				float num = fireInstance._thermalVolume.celsius + fireInstance._reheatSpeed * Time.unscaledDeltaTime;
				if (fireInstance._isDespawning)
				{
					if (fireInstance._deathStateDuration <= 0f)
					{
						num = 0f;
					}
					else
					{
						num = Mathf.Lerp(fireInstance._thermalVolume.celsius, 0f, fireInstance._timeSinceDyingStart / fireInstance._deathStateDuration);
					}
				}
				num = ((num > fireInstance._defaultTemperature) ? fireInstance._defaultTemperature : num);
				fireInstance._thermalVolume.celsius = num;
				float num2 = num / fireInstance._defaultTemperature;
				fireInstance._loopingAudioSource.volume = num2;
				for (int i = 0; i < fireInstance._emissiveRenderers.Length; i++)
				{
					fireInstance._emiRenderers_matPropBlocks[i].SetColor(FireManager.shaderProp_EmissionColor, fireInstance._emiRenderers_defaultColors[i] * num2);
				}
			}
			foreach (FireInstance fireInstance2 in FireManager._kFiresToDespawn)
			{
				ObjectPools.instance.Destroy(fireInstance2.gameObject);
			}
			FireManager._kFiresToDespawn.Clear();
		}

		// Token: 0x0400587D RID: 22653
		[OnEnterPlay_Clear]
		private static readonly Dictionary<int, FireInstance> _kGObjInstId_to_fire = new Dictionary<int, FireInstance>(256);

		// Token: 0x0400587E RID: 22654
		[OnEnterPlay_Clear]
		private static readonly List<FireInstance> _kEnabledReactions = new List<FireInstance>(256);

		// Token: 0x0400587F RID: 22655
		[OnEnterPlay_Clear]
		private static readonly List<FireInstance> _kFiresToDespawn = new List<FireInstance>(256);

		// Token: 0x04005880 RID: 22656
		[OnEnterPlay_Clear]
		private static readonly Dictionary<Vector3Int, int> _fireSpatialGrid = new Dictionary<Vector3Int, int>(256);

		// Token: 0x04005881 RID: 22657
		private const float _kSpatialGridCellSize = 0.2f;

		// Token: 0x04005882 RID: 22658
		private const int _kMaxAudioSources = 8;

		// Token: 0x04005883 RID: 22659
		[OnEnterPlay_Set(0)]
		private static int _activeAudioSources;

		// Token: 0x04005884 RID: 22660
		private static readonly int shaderProp_EmissionColor = Shader.PropertyToID("_EmissionColor");
	}
}
