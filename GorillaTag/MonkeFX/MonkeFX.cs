using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.MonkeFX
{
	// Token: 0x02000D55 RID: 3413
	public class MonkeFX : ITickSystemPost
	{
		// Token: 0x06005557 RID: 21847 RVA: 0x0019FF64 File Offset: 0x0019E164
		private static void InitBonesArray()
		{
			MonkeFX._rigs = VRRigCache.Instance.GetAllRigs();
			MonkeFX._bones = new Transform[MonkeFX._rigs.Length * MonkeFX._boneNames.Length];
			for (int i = 0; i < MonkeFX._rigs.Length; i++)
			{
				if (MonkeFX._rigs[i] == null)
				{
					MonkeFX._errorLog_nullVRRigFromVRRigCache.AddOccurrence(i.ToString());
				}
				else
				{
					int num = i * MonkeFX._boneNames.Length;
					if (MonkeFX._rigs[i].mainSkin == null)
					{
						MonkeFX._errorLog_nullMainSkin.AddOccurrence(MonkeFX._rigs[i].transform.GetPath());
						Debug.LogError("(This should never happen) Skipping null `mainSkin` on `VRRig`! Scene path: \n- \"" + MonkeFX._rigs[i].transform.GetPath() + "\"");
					}
					else
					{
						for (int j = 0; j < MonkeFX._rigs[i].mainSkin.bones.Length; j++)
						{
							Transform transform = MonkeFX._rigs[i].mainSkin.bones[j];
							if (transform == null)
							{
								MonkeFX._errorLog_nullBone.AddOccurrence(j.ToString());
							}
							else
							{
								for (int k = 0; k < MonkeFX._boneNames.Length; k++)
								{
									if (MonkeFX._boneNames[k] == transform.name)
									{
										MonkeFX._bones[num + k] = transform;
									}
								}
							}
						}
					}
				}
			}
			MonkeFX._errorLog_nullVRRigFromVRRigCache.LogOccurrences(VRRigCache.Instance, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 106);
			MonkeFX._errorLog_nullMainSkin.LogOccurrences(null, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 107);
			MonkeFX._errorLog_nullBone.LogOccurrences(null, null, "InitBonesArray", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", 108);
		}

		// Token: 0x06005558 RID: 21848 RVA: 0x000023F4 File Offset: 0x000005F4
		private static void UpdateBones()
		{
		}

		// Token: 0x06005559 RID: 21849 RVA: 0x000023F4 File Offset: 0x000005F4
		private static void UpdateBone()
		{
		}

		// Token: 0x0600555A RID: 21850 RVA: 0x001A010C File Offset: 0x0019E30C
		public static void Register(MonkeFXSettingsSO settingsSO)
		{
			MonkeFX.EnsureInstance();
			if (settingsSO == null || !MonkeFX.instance._settingsSOs.Add(settingsSO))
			{
				return;
			}
			int num = MonkeFX.instance._srcMeshId_to_sourceMesh.Count;
			for (int i = 0; i < settingsSO.sourceMeshes.Length; i++)
			{
				Mesh obj = settingsSO.sourceMeshes[i].obj;
				if (!(obj == null) && MonkeFX.instance._srcMeshInst_to_meshId.TryAdd(obj.GetInstanceID(), num))
				{
					MonkeFX.instance._srcMeshId_to_sourceMesh.Add(obj);
					num++;
				}
			}
		}

		// Token: 0x0600555B RID: 21851 RVA: 0x001A01A4 File Offset: 0x0019E3A4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetScaleToFitInBounds(Mesh mesh)
		{
			Bounds bounds = mesh.bounds;
			float num = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
			if (num <= 0f)
			{
				return 0f;
			}
			return 1f / num;
		}

		// Token: 0x0600555C RID: 21852 RVA: 0x001A01FC File Offset: 0x0019E3FC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Pack0To1Floats(float x, float y)
		{
			return Mathf.Clamp01(x) * 65536f + Mathf.Clamp01(y);
		}

		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x0600555D RID: 21853 RVA: 0x001A0211 File Offset: 0x0019E411
		// (set) Token: 0x0600555E RID: 21854 RVA: 0x001A0218 File Offset: 0x0019E418
		public static MonkeFX instance { get; private set; }

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x0600555F RID: 21855 RVA: 0x001A0220 File Offset: 0x0019E420
		// (set) Token: 0x06005560 RID: 21856 RVA: 0x001A0227 File Offset: 0x0019E427
		public static bool hasInstance { get; private set; }

		// Token: 0x06005561 RID: 21857 RVA: 0x001A022F File Offset: 0x0019E42F
		private static void EnsureInstance()
		{
			if (MonkeFX.hasInstance)
			{
				return;
			}
			MonkeFX.instance = new MonkeFX();
			MonkeFX.hasInstance = true;
		}

		// Token: 0x06005562 RID: 21858 RVA: 0x001A0249 File Offset: 0x0019E449
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnAfterFirstSceneLoaded()
		{
			MonkeFX.EnsureInstance();
			TickSystem<object>.AddPostTickCallback(MonkeFX.instance);
		}

		// Token: 0x06005563 RID: 21859 RVA: 0x001A025A File Offset: 0x0019E45A
		void ITickSystemPost.PostTick()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			MonkeFX.UpdateBones();
		}

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x06005564 RID: 21860 RVA: 0x001A0269 File Offset: 0x0019E469
		// (set) Token: 0x06005565 RID: 21861 RVA: 0x001A0271 File Offset: 0x0019E471
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06005566 RID: 21862 RVA: 0x001A027A File Offset: 0x0019E47A
		private static void PauseTick()
		{
			if (!MonkeFX.hasInstance)
			{
				MonkeFX.instance = new MonkeFX();
			}
			TickSystem<object>.RemovePostTickCallback(MonkeFX.instance);
		}

		// Token: 0x06005567 RID: 21863 RVA: 0x001A0297 File Offset: 0x0019E497
		private static void ResumeTick()
		{
			if (!MonkeFX.hasInstance)
			{
				MonkeFX.instance = new MonkeFX();
			}
			TickSystem<object>.AddPostTickCallback(MonkeFX.instance);
		}

		// Token: 0x040058C7 RID: 22727
		private static readonly string[] _boneNames = new string[] { "body", "hand.L", "hand.R" };

		// Token: 0x040058C8 RID: 22728
		private static VRRig[] _rigs;

		// Token: 0x040058C9 RID: 22729
		private static Transform[] _bones;

		// Token: 0x040058CA RID: 22730
		private static int _rigsHash;

		// Token: 0x040058CB RID: 22731
		private static readonly GTLogErrorLimiter _errorLog_nullVRRigFromVRRigCache = new GTLogErrorLimiter("(This should never happen) Skipping null `VRRig` obtained from `VRRigCache`!", 10, "\n- ");

		// Token: 0x040058CC RID: 22732
		private static GTLogErrorLimiter _errorLog_nullMainSkin = new GTLogErrorLimiter("(This should never happen) Skipping null `mainSkin` on `VRRig`! Scene paths: \n", 10, "\n- ");

		// Token: 0x040058CD RID: 22733
		private static readonly GTLogErrorLimiter _errorLog_nullBone = new GTLogErrorLimiter("(This should never happen) Skipping null bone obtained from `VRRig.mainSkin.bones`! Index(es): ", 10, "\n- ");

		// Token: 0x040058CE RID: 22734
		private readonly HashSet<MonkeFXSettingsSO> _settingsSOs = new HashSet<MonkeFXSettingsSO>(8);

		// Token: 0x040058CF RID: 22735
		private readonly Dictionary<int, int> _srcMeshInst_to_meshId = new Dictionary<int, int>(8);

		// Token: 0x040058D0 RID: 22736
		private readonly List<Mesh> _srcMeshId_to_sourceMesh = new List<Mesh>(8);

		// Token: 0x040058D1 RID: 22737
		private readonly List<MonkeFX.ElementsRange> _srcMeshId_to_elemRange = new List<MonkeFX.ElementsRange>(8);

		// Token: 0x040058D2 RID: 22738
		private readonly Dictionary<int, List<MonkeFXSettingsSO>> _meshId_to_settingsUsers = new Dictionary<int, List<MonkeFXSettingsSO>>();

		// Token: 0x040058D3 RID: 22739
		private const float _k16BitFactor = 65536f;

		// Token: 0x02000D56 RID: 3414
		private struct ElementsRange
		{
			// Token: 0x040058D7 RID: 22743
			public int min;

			// Token: 0x040058D8 RID: 22744
			public int max;
		}
	}
}
