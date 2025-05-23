using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E7D RID: 3709
	public static class BoingWorkAsynchronous
	{
		// Token: 0x06005CB1 RID: 23729 RVA: 0x001C9B28 File Offset: 0x001C7D28
		internal static void PostUnregisterBehaviorCleanUp()
		{
			if (BoingWorkAsynchronous.s_behaviorJobNeedsGather)
			{
				BoingWorkAsynchronous.s_hBehaviorJob.Complete();
				BoingWorkAsynchronous.s_aBehaviorParams.Dispose();
				BoingWorkAsynchronous.s_aBehaviorOutput.Dispose();
				BoingWorkAsynchronous.s_behaviorJobNeedsGather = false;
			}
		}

		// Token: 0x06005CB2 RID: 23730 RVA: 0x001C9B55 File Offset: 0x001C7D55
		internal static void PostUnregisterEffectorReactorCleanUp()
		{
			if (BoingWorkAsynchronous.s_reactorJobNeedsGather)
			{
				BoingWorkAsynchronous.s_hReactorJob.Complete();
				BoingWorkAsynchronous.s_aEffectors.Dispose();
				BoingWorkAsynchronous.s_aReactorExecParams.Dispose();
				BoingWorkAsynchronous.s_aReactorExecOutput.Dispose();
				BoingWorkAsynchronous.s_reactorJobNeedsGather = false;
			}
		}

		// Token: 0x06005CB3 RID: 23731 RVA: 0x001C9B8C File Offset: 0x001C7D8C
		internal static void ExecuteBehaviors(Dictionary<int, BoingBehavior> behaviorMap, BoingManager.UpdateMode updateMode)
		{
			int num = 0;
			BoingWorkAsynchronous.s_aBehaviorParams = new NativeArray<BoingWork.Params>(behaviorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			BoingWorkAsynchronous.s_aBehaviorOutput = new NativeArray<BoingWork.Output>(behaviorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in behaviorMap)
			{
				BoingBehavior value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					BoingWorkAsynchronous.s_aBehaviorParams[num++] = value.Params;
				}
			}
			if (num > 0)
			{
				BoingWorkAsynchronous.BehaviorJob behaviorJob = new BoingWorkAsynchronous.BehaviorJob
				{
					Params = BoingWorkAsynchronous.s_aBehaviorParams,
					Output = BoingWorkAsynchronous.s_aBehaviorOutput,
					DeltaTime = BoingManager.DeltaTime,
					FixedDeltaTime = BoingManager.FixedDeltaTime
				};
				int num2 = (int)Mathf.Ceil((float)num / (float)Environment.ProcessorCount);
				BoingWorkAsynchronous.s_hBehaviorJob = behaviorJob.Schedule(num, num2, default(JobHandle));
				JobHandle.ScheduleBatchedJobs();
			}
			BoingWorkAsynchronous.s_behaviorJobNeedsGather = true;
			if (BoingWorkAsynchronous.s_behaviorJobNeedsGather)
			{
				if (num > 0)
				{
					BoingWorkAsynchronous.s_hBehaviorJob.Complete();
					for (int i = 0; i < num; i++)
					{
						BoingWorkAsynchronous.s_aBehaviorOutput[i].GatherOutput(behaviorMap, updateMode);
					}
				}
				BoingWorkAsynchronous.s_aBehaviorParams.Dispose();
				BoingWorkAsynchronous.s_aBehaviorOutput.Dispose();
				BoingWorkAsynchronous.s_behaviorJobNeedsGather = false;
			}
		}

		// Token: 0x06005CB4 RID: 23732 RVA: 0x001C9CEC File Offset: 0x001C7EEC
		internal static void ExecuteReactors(Dictionary<int, BoingEffector> effectorMap, Dictionary<int, BoingReactor> reactorMap, Dictionary<int, BoingReactorField> fieldMap, Dictionary<int, BoingReactorFieldCPUSampler> cpuSamplerMap, BoingManager.UpdateMode updateMode)
		{
			int num = 0;
			BoingWorkAsynchronous.s_aEffectors = new NativeArray<BoingEffector.Params>(effectorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			BoingWorkAsynchronous.s_aReactorExecParams = new NativeArray<BoingWork.Params>(reactorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			BoingWorkAsynchronous.s_aReactorExecOutput = new NativeArray<BoingWork.Output>(reactorMap.Count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in reactorMap)
			{
				BoingReactor value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					BoingWorkAsynchronous.s_aReactorExecParams[num++] = value.Params;
				}
			}
			if (num > 0)
			{
				int num2 = 0;
				BoingEffector.Params @params = default(BoingEffector.Params);
				foreach (KeyValuePair<int, BoingEffector> keyValuePair2 in effectorMap)
				{
					BoingEffector value2 = keyValuePair2.Value;
					@params.Fill(keyValuePair2.Value);
					BoingWorkAsynchronous.s_aEffectors[num2++] = @params;
				}
			}
			if (num > 0)
			{
				BoingWorkAsynchronous.s_hReactorJob = new BoingWorkAsynchronous.ReactorJob
				{
					Effectors = BoingWorkAsynchronous.s_aEffectors,
					Params = BoingWorkAsynchronous.s_aReactorExecParams,
					Output = BoingWorkAsynchronous.s_aReactorExecOutput,
					DeltaTime = BoingManager.DeltaTime,
					FixedDeltaTime = BoingManager.FixedDeltaTime
				}.Schedule(num, 32, default(JobHandle));
				JobHandle.ScheduleBatchedJobs();
			}
			foreach (KeyValuePair<int, BoingReactorField> keyValuePair3 in fieldMap)
			{
				BoingReactorField value3 = keyValuePair3.Value;
				if (value3.HardwareMode == BoingReactorField.HardwareModeEnum.CPU)
				{
					value3.ExecuteCpu(BoingManager.DeltaTime);
				}
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair4 in cpuSamplerMap)
			{
				BoingReactorFieldCPUSampler value4 = keyValuePair4.Value;
			}
			BoingWorkAsynchronous.s_reactorJobNeedsGather = true;
			if (BoingWorkAsynchronous.s_reactorJobNeedsGather)
			{
				if (num > 0)
				{
					BoingWorkAsynchronous.s_hReactorJob.Complete();
					for (int i = 0; i < num; i++)
					{
						BoingWorkAsynchronous.s_aReactorExecOutput[i].GatherOutput(reactorMap, updateMode);
					}
				}
				BoingWorkAsynchronous.s_aEffectors.Dispose();
				BoingWorkAsynchronous.s_aReactorExecParams.Dispose();
				BoingWorkAsynchronous.s_aReactorExecOutput.Dispose();
				BoingWorkAsynchronous.s_reactorJobNeedsGather = false;
			}
		}

		// Token: 0x06005CB5 RID: 23733 RVA: 0x001C9F6C File Offset: 0x001C816C
		internal static void ExecuteBones(BoingEffector.Params[] aEffectorParams, Dictionary<int, BoingBones> bonesMap, BoingManager.UpdateMode updateMode)
		{
			float deltaTime = BoingManager.DeltaTime;
			foreach (KeyValuePair<int, BoingBones> keyValuePair in bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					if (aEffectorParams != null)
					{
						for (int i = 0; i < aEffectorParams.Length; i++)
						{
							value.AccumulateTarget(ref aEffectorParams[i], deltaTime);
						}
					}
					value.EndAccumulateTargets();
					BoingManager.UpdateMode updateMode2 = value.UpdateMode;
					if (updateMode2 != BoingManager.UpdateMode.FixedUpdate)
					{
						if (updateMode2 - BoingManager.UpdateMode.EarlyUpdate <= 1)
						{
							value.Params.Execute(value, BoingManager.DeltaTime);
						}
					}
					else
					{
						value.Params.Execute(value, BoingManager.FixedDeltaTime);
					}
				}
			}
		}

		// Token: 0x06005CB6 RID: 23734 RVA: 0x001CA038 File Offset: 0x001C8238
		internal static void PullBonesResults(BoingEffector.Params[] aEffectorParams, Dictionary<int, BoingBones> bonesMap, BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingBones> keyValuePair in bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.Params.PullResults(value);
				}
			}
		}

		// Token: 0x040060FD RID: 24829
		private static bool s_behaviorJobNeedsGather;

		// Token: 0x040060FE RID: 24830
		private static JobHandle s_hBehaviorJob;

		// Token: 0x040060FF RID: 24831
		private static NativeArray<BoingWork.Params> s_aBehaviorParams;

		// Token: 0x04006100 RID: 24832
		private static NativeArray<BoingWork.Output> s_aBehaviorOutput;

		// Token: 0x04006101 RID: 24833
		private static bool s_reactorJobNeedsGather;

		// Token: 0x04006102 RID: 24834
		private static JobHandle s_hReactorJob;

		// Token: 0x04006103 RID: 24835
		private static NativeArray<BoingEffector.Params> s_aEffectors;

		// Token: 0x04006104 RID: 24836
		private static NativeArray<BoingWork.Params> s_aReactorExecParams;

		// Token: 0x04006105 RID: 24837
		private static NativeArray<BoingWork.Output> s_aReactorExecOutput;

		// Token: 0x02000E7E RID: 3710
		private struct BehaviorJob : IJobParallelFor
		{
			// Token: 0x06005CB7 RID: 23735 RVA: 0x001CA09C File Offset: 0x001C829C
			public void Execute(int index)
			{
				BoingWork.Params @params = this.Params[index];
				if (@params.Bits.IsBitSet(9))
				{
					@params.Execute(this.FixedDeltaTime);
				}
				else
				{
					@params.Execute(this.DeltaTime);
				}
				this.Output[index] = new BoingWork.Output(@params.InstanceID, ref @params.Instance.PositionSpring, ref @params.Instance.RotationSpring, ref @params.Instance.ScaleSpring);
			}

			// Token: 0x04006106 RID: 24838
			public NativeArray<BoingWork.Params> Params;

			// Token: 0x04006107 RID: 24839
			public NativeArray<BoingWork.Output> Output;

			// Token: 0x04006108 RID: 24840
			public float DeltaTime;

			// Token: 0x04006109 RID: 24841
			public float FixedDeltaTime;
		}

		// Token: 0x02000E7F RID: 3711
		private struct ReactorJob : IJobParallelFor
		{
			// Token: 0x06005CB8 RID: 23736 RVA: 0x001CA120 File Offset: 0x001C8320
			public void Execute(int index)
			{
				BoingWork.Params @params = this.Params[index];
				int i = 0;
				int length = this.Effectors.Length;
				while (i < length)
				{
					BoingEffector.Params params2 = this.Effectors[i];
					@params.AccumulateTarget(ref params2, this.DeltaTime);
					i++;
				}
				@params.EndAccumulateTargets();
				if (@params.Bits.IsBitSet(9))
				{
					@params.Execute(this.FixedDeltaTime);
				}
				else
				{
					@params.Execute(BoingManager.DeltaTime);
				}
				this.Output[index] = new BoingWork.Output(@params.InstanceID, ref @params.Instance.PositionSpring, ref @params.Instance.RotationSpring, ref @params.Instance.ScaleSpring);
			}

			// Token: 0x0400610A RID: 24842
			[ReadOnly]
			public NativeArray<BoingEffector.Params> Effectors;

			// Token: 0x0400610B RID: 24843
			public NativeArray<BoingWork.Params> Params;

			// Token: 0x0400610C RID: 24844
			public NativeArray<BoingWork.Output> Output;

			// Token: 0x0400610D RID: 24845
			public float DeltaTime;

			// Token: 0x0400610E RID: 24846
			public float FixedDeltaTime;
		}
	}
}
