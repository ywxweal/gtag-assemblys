using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E59 RID: 3673
	public static class BoingManager
	{
		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x06005BEB RID: 23531 RVA: 0x001C4638 File Offset: 0x001C2838
		public static IEnumerable<BoingBehavior> Behaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Values;
			}
		}

		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x06005BEC RID: 23532 RVA: 0x001C4644 File Offset: 0x001C2844
		public static IEnumerable<BoingReactor> Reactors
		{
			get
			{
				return BoingManager.s_reactorMap.Values;
			}
		}

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x06005BED RID: 23533 RVA: 0x001C4650 File Offset: 0x001C2850
		public static IEnumerable<BoingEffector> Effectors
		{
			get
			{
				return BoingManager.s_effectorMap.Values;
			}
		}

		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x06005BEE RID: 23534 RVA: 0x001C465C File Offset: 0x001C285C
		public static IEnumerable<BoingReactorField> ReactorFields
		{
			get
			{
				return BoingManager.s_fieldMap.Values;
			}
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x06005BEF RID: 23535 RVA: 0x001C4668 File Offset: 0x001C2868
		public static IEnumerable<BoingReactorFieldCPUSampler> ReactorFieldCPUSamlers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Values;
			}
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x06005BF0 RID: 23536 RVA: 0x001C4674 File Offset: 0x001C2874
		public static IEnumerable<BoingReactorFieldGPUSampler> ReactorFieldGPUSampler
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Values;
			}
		}

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x06005BF1 RID: 23537 RVA: 0x001C4680 File Offset: 0x001C2880
		public static float DeltaTime
		{
			get
			{
				return BoingManager.s_deltaTime;
			}
		}

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x06005BF2 RID: 23538 RVA: 0x001C4687 File Offset: 0x001C2887
		public static float FixedDeltaTime
		{
			get
			{
				return Time.fixedDeltaTime;
			}
		}

		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x06005BF3 RID: 23539 RVA: 0x001C468E File Offset: 0x001C288E
		internal static int NumBehaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Count;
			}
		}

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x06005BF4 RID: 23540 RVA: 0x001C469A File Offset: 0x001C289A
		internal static int NumEffectors
		{
			get
			{
				return BoingManager.s_effectorMap.Count;
			}
		}

		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x06005BF5 RID: 23541 RVA: 0x001C46A6 File Offset: 0x001C28A6
		internal static int NumReactors
		{
			get
			{
				return BoingManager.s_reactorMap.Count;
			}
		}

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x06005BF6 RID: 23542 RVA: 0x001C46B2 File Offset: 0x001C28B2
		internal static int NumFields
		{
			get
			{
				return BoingManager.s_fieldMap.Count;
			}
		}

		// Token: 0x17000904 RID: 2308
		// (get) Token: 0x06005BF7 RID: 23543 RVA: 0x001C46BE File Offset: 0x001C28BE
		internal static int NumCPUFieldSamplers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Count;
			}
		}

		// Token: 0x17000905 RID: 2309
		// (get) Token: 0x06005BF8 RID: 23544 RVA: 0x001C46CA File Offset: 0x001C28CA
		internal static int NumGPUFieldSamplers
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Count;
			}
		}

		// Token: 0x06005BF9 RID: 23545 RVA: 0x001C46D8 File Offset: 0x001C28D8
		private static void ValidateManager()
		{
			if (BoingManager.s_managerGo != null)
			{
				return;
			}
			BoingManager.s_managerGo = new GameObject("Boing Kit manager (don't delete)");
			BoingManager.s_managerGo.AddComponent<BoingManagerPreUpdatePump>();
			BoingManager.s_managerGo.AddComponent<BoingManagerPostUpdatePump>();
			Object.DontDestroyOnLoad(BoingManager.s_managerGo);
			BoingManager.s_managerGo.AddComponent<SphereCollider>().enabled = false;
		}

		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x06005BFA RID: 23546 RVA: 0x001C4732 File Offset: 0x001C2932
		internal static SphereCollider SharedSphereCollider
		{
			get
			{
				if (BoingManager.s_managerGo == null)
				{
					return null;
				}
				return BoingManager.s_managerGo.GetComponent<SphereCollider>();
			}
		}

		// Token: 0x06005BFB RID: 23547 RVA: 0x001C474D File Offset: 0x001C294D
		internal static void Register(BoingBehavior behavior)
		{
			BoingManager.PreRegisterBehavior();
			BoingManager.s_behaviorMap.Add(behavior.GetInstanceID(), behavior);
			if (BoingManager.OnBehaviorRegister != null)
			{
				BoingManager.OnBehaviorRegister(behavior);
			}
		}

		// Token: 0x06005BFC RID: 23548 RVA: 0x001C4777 File Offset: 0x001C2977
		internal static void Unregister(BoingBehavior behavior)
		{
			if (BoingManager.OnBehaviorUnregister != null)
			{
				BoingManager.OnBehaviorUnregister(behavior);
			}
			BoingManager.s_behaviorMap.Remove(behavior.GetInstanceID());
			BoingManager.PostUnregisterBehavior();
		}

		// Token: 0x06005BFD RID: 23549 RVA: 0x001C47A1 File Offset: 0x001C29A1
		internal static void Register(BoingEffector effector)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_effectorMap.Add(effector.GetInstanceID(), effector);
			if (BoingManager.OnEffectorRegister != null)
			{
				BoingManager.OnEffectorRegister(effector);
			}
		}

		// Token: 0x06005BFE RID: 23550 RVA: 0x001C47CB File Offset: 0x001C29CB
		internal static void Unregister(BoingEffector effector)
		{
			if (BoingManager.OnEffectorUnregister != null)
			{
				BoingManager.OnEffectorUnregister(effector);
			}
			BoingManager.s_effectorMap.Remove(effector.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06005BFF RID: 23551 RVA: 0x001C47F5 File Offset: 0x001C29F5
		internal static void Register(BoingReactor reactor)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_reactorMap.Add(reactor.GetInstanceID(), reactor);
			if (BoingManager.OnReactorRegister != null)
			{
				BoingManager.OnReactorRegister(reactor);
			}
		}

		// Token: 0x06005C00 RID: 23552 RVA: 0x001C481F File Offset: 0x001C2A1F
		internal static void Unregister(BoingReactor reactor)
		{
			if (BoingManager.OnReactorUnregister != null)
			{
				BoingManager.OnReactorUnregister(reactor);
			}
			BoingManager.s_reactorMap.Remove(reactor.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06005C01 RID: 23553 RVA: 0x001C4849 File Offset: 0x001C2A49
		internal static void Register(BoingReactorField field)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_fieldMap.Add(field.GetInstanceID(), field);
			if (BoingManager.OnReactorFieldRegister != null)
			{
				BoingManager.OnReactorFieldRegister(field);
			}
		}

		// Token: 0x06005C02 RID: 23554 RVA: 0x001C4873 File Offset: 0x001C2A73
		internal static void Unregister(BoingReactorField field)
		{
			if (BoingManager.OnReactorFieldUnregister != null)
			{
				BoingManager.OnReactorFieldUnregister(field);
			}
			BoingManager.s_fieldMap.Remove(field.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06005C03 RID: 23555 RVA: 0x001C489D File Offset: 0x001C2A9D
		internal static void Register(BoingReactorFieldCPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_cpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldCPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
		}

		// Token: 0x06005C04 RID: 23556 RVA: 0x001C48C7 File Offset: 0x001C2AC7
		internal static void Unregister(BoingReactorFieldCPUSampler sampler)
		{
			if (BoingManager.OnReactorFieldCPUSamplerUnregister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
			BoingManager.s_cpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06005C05 RID: 23557 RVA: 0x001C48F1 File Offset: 0x001C2AF1
		internal static void Register(BoingReactorFieldGPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_gpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldGPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldGPUSamplerRegister(sampler);
			}
		}

		// Token: 0x06005C06 RID: 23558 RVA: 0x001C491B File Offset: 0x001C2B1B
		internal static void Unregister(BoingReactorFieldGPUSampler sampler)
		{
			if (BoingManager.OnFieldGPUSamplerUnregister != null)
			{
				BoingManager.OnFieldGPUSamplerUnregister(sampler);
			}
			BoingManager.s_gpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06005C07 RID: 23559 RVA: 0x001C4945 File Offset: 0x001C2B45
		internal static void Register(BoingBones bones)
		{
			BoingManager.PreRegisterBones();
			BoingManager.s_bonesMap.Add(bones.GetInstanceID(), bones);
			if (BoingManager.OnBonesRegister != null)
			{
				BoingManager.OnBonesRegister(bones);
			}
		}

		// Token: 0x06005C08 RID: 23560 RVA: 0x001C496F File Offset: 0x001C2B6F
		internal static void Unregister(BoingBones bones)
		{
			if (BoingManager.OnBonesUnregister != null)
			{
				BoingManager.OnBonesUnregister(bones);
			}
			BoingManager.s_bonesMap.Remove(bones.GetInstanceID());
			BoingManager.PostUnregisterBones();
		}

		// Token: 0x06005C09 RID: 23561 RVA: 0x001C4999 File Offset: 0x001C2B99
		private static void PreRegisterBehavior()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x06005C0A RID: 23562 RVA: 0x001C49A0 File Offset: 0x001C2BA0
		private static void PostUnregisterBehavior()
		{
			if (BoingManager.s_behaviorMap.Count > 0)
			{
				return;
			}
			BoingWorkAsynchronous.PostUnregisterBehaviorCleanUp();
		}

		// Token: 0x06005C0B RID: 23563 RVA: 0x001C49B8 File Offset: 0x001C2BB8
		private static void PreRegisterEffectorReactor()
		{
			BoingManager.ValidateManager();
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				BoingManager.s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
			if (BoingManager.s_effectorMap.Count >= BoingManager.s_effectorParamsList.Capacity)
			{
				BoingManager.s_effectorParamsList.Capacity += BoingManager.kEffectorParamsIncrement;
				BoingManager.s_effectorParamsBuffer.Dispose();
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
		}

		// Token: 0x06005C0C RID: 23564 RVA: 0x001C4A48 File Offset: 0x001C2C48
		private static void PostUnregisterEffectorReactor()
		{
			if (BoingManager.s_effectorMap.Count > 0 || BoingManager.s_reactorMap.Count > 0 || BoingManager.s_fieldMap.Count > 0 || BoingManager.s_cpuSamplerMap.Count > 0 || BoingManager.s_gpuSamplerMap.Count > 0)
			{
				return;
			}
			BoingManager.s_effectorParamsList = null;
			BoingManager.s_effectorParamsBuffer.Dispose();
			BoingManager.s_effectorParamsBuffer = null;
			BoingWorkAsynchronous.PostUnregisterEffectorReactorCleanUp();
		}

		// Token: 0x06005C0D RID: 23565 RVA: 0x001C4999 File Offset: 0x001C2B99
		private static void PreRegisterBones()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x06005C0E RID: 23566 RVA: 0x000023F4 File Offset: 0x000005F4
		private static void PostUnregisterBones()
		{
		}

		// Token: 0x06005C0F RID: 23567 RVA: 0x001C4AB2 File Offset: 0x001C2CB2
		internal static void Execute(BoingManager.UpdateMode updateMode)
		{
			if (updateMode == BoingManager.UpdateMode.EarlyUpdate)
			{
				BoingManager.s_deltaTime = Time.deltaTime;
			}
			BoingManager.RefreshEffectorParams();
			BoingManager.ExecuteBones(updateMode);
			BoingManager.ExecuteBehaviors(updateMode);
			BoingManager.ExecuteReactors(updateMode);
		}

		// Token: 0x06005C10 RID: 23568 RVA: 0x001C4ADC File Offset: 0x001C2CDC
		internal static void ExecuteBehaviors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_behaviorMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				BoingBehavior value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
		}

		// Token: 0x06005C11 RID: 23569 RVA: 0x001C4B70 File Offset: 0x001C2D70
		internal static void PullBehaviorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
		}

		// Token: 0x06005C12 RID: 23570 RVA: 0x001C4BD8 File Offset: 0x001C2DD8
		internal static void RestoreBehaviors()
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x06005C13 RID: 23571 RVA: 0x001C4C30 File Offset: 0x001C2E30
		internal static void RefreshEffectorParams()
		{
			if (BoingManager.s_effectorParamsList == null)
			{
				return;
			}
			BoingManager.s_effectorParamsIndexMap.Clear();
			BoingManager.s_effectorParamsList.Clear();
			foreach (KeyValuePair<int, BoingEffector> keyValuePair in BoingManager.s_effectorMap)
			{
				BoingEffector value = keyValuePair.Value;
				BoingManager.s_effectorParamsIndexMap.Add(value.GetInstanceID(), BoingManager.s_effectorParamsList.Count);
				BoingManager.s_effectorParamsList.Add(new BoingEffector.Params(value));
			}
			if (BoingManager.s_aEffectorParams == null || BoingManager.s_aEffectorParams.Length != BoingManager.s_effectorParamsList.Count)
			{
				BoingManager.s_aEffectorParams = BoingManager.s_effectorParamsList.ToArray();
				return;
			}
			BoingManager.s_effectorParamsList.CopyTo(BoingManager.s_aEffectorParams);
		}

		// Token: 0x06005C14 RID: 23572 RVA: 0x001C4D04 File Offset: 0x001C2F04
		internal static void ExecuteReactors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_effectorMap.Count == 0 && BoingManager.s_reactorMap.Count == 0 && BoingManager.s_fieldMap.Count == 0 && BoingManager.s_cpuSamplerMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				BoingReactor value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteReactors(BoingManager.s_effectorMap, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteReactors(BoingManager.s_aEffectorParams, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
		}

		// Token: 0x06005C15 RID: 23573 RVA: 0x001C4DDC File Offset: 0x001C2FDC
		internal static void PullReactorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				if (keyValuePair2.Value.UpdateMode == updateMode)
				{
					keyValuePair2.Value.SampleFromField();
				}
			}
		}

		// Token: 0x06005C16 RID: 23574 RVA: 0x001C4E98 File Offset: 0x001C3098
		internal static void RestoreReactors()
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				keyValuePair.Value.Restore();
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				keyValuePair2.Value.Restore();
			}
		}

		// Token: 0x06005C17 RID: 23575 RVA: 0x001C4F38 File Offset: 0x001C3138
		internal static void DispatchReactorFieldCompute()
		{
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				return;
			}
			BoingManager.s_effectorParamsBuffer.SetData(BoingManager.s_aEffectorParams);
			float deltaTime = Time.deltaTime;
			foreach (KeyValuePair<int, BoingReactorField> keyValuePair in BoingManager.s_fieldMap)
			{
				BoingReactorField value = keyValuePair.Value;
				if (value.HardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					value.ExecuteGpu(deltaTime, BoingManager.s_effectorParamsBuffer, BoingManager.s_effectorParamsIndexMap);
				}
			}
		}

		// Token: 0x06005C18 RID: 23576 RVA: 0x001C4FC4 File Offset: 0x001C31C4
		internal static void ExecuteBones(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x06005C19 RID: 23577 RVA: 0x001C5064 File Offset: 0x001C3264
		internal static void PullBonesResults(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x06005C1A RID: 23578 RVA: 0x001C509C File Offset: 0x001C329C
		internal static void RestoreBones()
		{
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x0400601B RID: 24603
		public static BoingManager.BehaviorRegisterDelegate OnBehaviorRegister;

		// Token: 0x0400601C RID: 24604
		public static BoingManager.BehaviorUnregisterDelegate OnBehaviorUnregister;

		// Token: 0x0400601D RID: 24605
		public static BoingManager.EffectorRegisterDelegate OnEffectorRegister;

		// Token: 0x0400601E RID: 24606
		public static BoingManager.EffectorUnregisterDelegate OnEffectorUnregister;

		// Token: 0x0400601F RID: 24607
		public static BoingManager.ReactorRegisterDelegate OnReactorRegister;

		// Token: 0x04006020 RID: 24608
		public static BoingManager.ReactorUnregisterDelegate OnReactorUnregister;

		// Token: 0x04006021 RID: 24609
		public static BoingManager.ReactorFieldRegisterDelegate OnReactorFieldRegister;

		// Token: 0x04006022 RID: 24610
		public static BoingManager.ReactorFieldUnregisterDelegate OnReactorFieldUnregister;

		// Token: 0x04006023 RID: 24611
		public static BoingManager.ReactorFieldCPUSamplerRegisterDelegate OnReactorFieldCPUSamplerRegister;

		// Token: 0x04006024 RID: 24612
		public static BoingManager.ReactorFieldCPUSamplerUnregisterDelegate OnReactorFieldCPUSamplerUnregister;

		// Token: 0x04006025 RID: 24613
		public static BoingManager.ReactorFieldGPUSamplerRegisterDelegate OnReactorFieldGPUSamplerRegister;

		// Token: 0x04006026 RID: 24614
		public static BoingManager.ReactorFieldGPUSamplerUnregisterDelegate OnFieldGPUSamplerUnregister;

		// Token: 0x04006027 RID: 24615
		public static BoingManager.BonesRegisterDelegate OnBonesRegister;

		// Token: 0x04006028 RID: 24616
		public static BoingManager.BonesUnregisterDelegate OnBonesUnregister;

		// Token: 0x04006029 RID: 24617
		private static float s_deltaTime = 0f;

		// Token: 0x0400602A RID: 24618
		private static Dictionary<int, BoingBehavior> s_behaviorMap = new Dictionary<int, BoingBehavior>();

		// Token: 0x0400602B RID: 24619
		private static Dictionary<int, BoingEffector> s_effectorMap = new Dictionary<int, BoingEffector>();

		// Token: 0x0400602C RID: 24620
		private static Dictionary<int, BoingReactor> s_reactorMap = new Dictionary<int, BoingReactor>();

		// Token: 0x0400602D RID: 24621
		private static Dictionary<int, BoingReactorField> s_fieldMap = new Dictionary<int, BoingReactorField>();

		// Token: 0x0400602E RID: 24622
		private static Dictionary<int, BoingReactorFieldCPUSampler> s_cpuSamplerMap = new Dictionary<int, BoingReactorFieldCPUSampler>();

		// Token: 0x0400602F RID: 24623
		private static Dictionary<int, BoingReactorFieldGPUSampler> s_gpuSamplerMap = new Dictionary<int, BoingReactorFieldGPUSampler>();

		// Token: 0x04006030 RID: 24624
		private static Dictionary<int, BoingBones> s_bonesMap = new Dictionary<int, BoingBones>();

		// Token: 0x04006031 RID: 24625
		private static readonly int kEffectorParamsIncrement = 16;

		// Token: 0x04006032 RID: 24626
		private static List<BoingEffector.Params> s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);

		// Token: 0x04006033 RID: 24627
		private static BoingEffector.Params[] s_aEffectorParams;

		// Token: 0x04006034 RID: 24628
		private static ComputeBuffer s_effectorParamsBuffer;

		// Token: 0x04006035 RID: 24629
		private static Dictionary<int, int> s_effectorParamsIndexMap = new Dictionary<int, int>();

		// Token: 0x04006036 RID: 24630
		internal static readonly bool UseAsynchronousJobs = true;

		// Token: 0x04006037 RID: 24631
		internal static GameObject s_managerGo;

		// Token: 0x02000E5A RID: 3674
		public enum UpdateMode
		{
			// Token: 0x04006039 RID: 24633
			FixedUpdate,
			// Token: 0x0400603A RID: 24634
			EarlyUpdate,
			// Token: 0x0400603B RID: 24635
			LateUpdate
		}

		// Token: 0x02000E5B RID: 3675
		public enum TranslationLockSpace
		{
			// Token: 0x0400603D RID: 24637
			Global,
			// Token: 0x0400603E RID: 24638
			Local
		}

		// Token: 0x02000E5C RID: 3676
		// (Invoke) Token: 0x06005C1D RID: 23581
		public delegate void BehaviorRegisterDelegate(BoingBehavior behavior);

		// Token: 0x02000E5D RID: 3677
		// (Invoke) Token: 0x06005C21 RID: 23585
		public delegate void BehaviorUnregisterDelegate(BoingBehavior behavior);

		// Token: 0x02000E5E RID: 3678
		// (Invoke) Token: 0x06005C25 RID: 23589
		public delegate void EffectorRegisterDelegate(BoingEffector effector);

		// Token: 0x02000E5F RID: 3679
		// (Invoke) Token: 0x06005C29 RID: 23593
		public delegate void EffectorUnregisterDelegate(BoingEffector effector);

		// Token: 0x02000E60 RID: 3680
		// (Invoke) Token: 0x06005C2D RID: 23597
		public delegate void ReactorRegisterDelegate(BoingReactor reactor);

		// Token: 0x02000E61 RID: 3681
		// (Invoke) Token: 0x06005C31 RID: 23601
		public delegate void ReactorUnregisterDelegate(BoingReactor reactor);

		// Token: 0x02000E62 RID: 3682
		// (Invoke) Token: 0x06005C35 RID: 23605
		public delegate void ReactorFieldRegisterDelegate(BoingReactorField field);

		// Token: 0x02000E63 RID: 3683
		// (Invoke) Token: 0x06005C39 RID: 23609
		public delegate void ReactorFieldUnregisterDelegate(BoingReactorField field);

		// Token: 0x02000E64 RID: 3684
		// (Invoke) Token: 0x06005C3D RID: 23613
		public delegate void ReactorFieldCPUSamplerRegisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x02000E65 RID: 3685
		// (Invoke) Token: 0x06005C41 RID: 23617
		public delegate void ReactorFieldCPUSamplerUnregisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x02000E66 RID: 3686
		// (Invoke) Token: 0x06005C45 RID: 23621
		public delegate void ReactorFieldGPUSamplerRegisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x02000E67 RID: 3687
		// (Invoke) Token: 0x06005C49 RID: 23625
		public delegate void ReactorFieldGPUSamplerUnregisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x02000E68 RID: 3688
		// (Invoke) Token: 0x06005C4D RID: 23629
		public delegate void BonesRegisterDelegate(BoingBones bones);

		// Token: 0x02000E69 RID: 3689
		// (Invoke) Token: 0x06005C51 RID: 23633
		public delegate void BonesUnregisterDelegate(BoingBones bones);
	}
}
