﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E80 RID: 3712
	internal static class BoingWorkSynchronous
	{
		// Token: 0x06005CBA RID: 23738 RVA: 0x001CA2B4 File Offset: 0x001C84B4
		internal static void ExecuteBehaviors(Dictionary<int, BoingBehavior> behaviorMap, BoingManager.UpdateMode updateMode)
		{
			float deltaTime = Time.deltaTime;
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in behaviorMap)
			{
				BoingBehavior value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					BoingManager.UpdateMode updateMode2 = value.UpdateMode;
					if (updateMode2 != BoingManager.UpdateMode.FixedUpdate)
					{
						if (updateMode2 - BoingManager.UpdateMode.EarlyUpdate <= 1)
						{
							value.Execute(deltaTime);
						}
					}
					else
					{
						value.Execute(BoingManager.FixedDeltaTime);
					}
				}
			}
		}

		// Token: 0x06005CBB RID: 23739 RVA: 0x001CA340 File Offset: 0x001C8540
		internal static void ExecuteReactors(BoingEffector.Params[] aEffectorParams, Dictionary<int, BoingReactor> reactorMap, Dictionary<int, BoingReactorField> fieldMap, Dictionary<int, BoingReactorFieldCPUSampler> cpuSamplerMap, BoingManager.UpdateMode updateMode)
		{
			float deltaTime = BoingManager.DeltaTime;
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in reactorMap)
			{
				BoingReactor value = keyValuePair.Value;
				if (value.UpdateMode == updateMode)
				{
					value.PrepareExecute();
					if (aEffectorParams != null)
					{
						for (int i = 0; i < aEffectorParams.Length; i++)
						{
							value.Params.AccumulateTarget(ref aEffectorParams[i], deltaTime);
						}
					}
					value.Params.EndAccumulateTargets();
					BoingManager.UpdateMode updateMode2 = value.UpdateMode;
					if (updateMode2 != BoingManager.UpdateMode.FixedUpdate)
					{
						if (updateMode2 - BoingManager.UpdateMode.EarlyUpdate <= 1)
						{
							value.Execute(deltaTime);
						}
					}
					else
					{
						value.Execute(BoingManager.FixedDeltaTime);
					}
				}
			}
			foreach (KeyValuePair<int, BoingReactorField> keyValuePair2 in fieldMap)
			{
				BoingReactorField value2 = keyValuePair2.Value;
				if (value2.HardwareMode == BoingReactorField.HardwareModeEnum.CPU)
				{
					value2.ExecuteCpu(deltaTime);
				}
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair3 in cpuSamplerMap)
			{
				BoingReactorFieldCPUSampler value3 = keyValuePair3.Value;
			}
		}

		// Token: 0x06005CBC RID: 23740 RVA: 0x001CA498 File Offset: 0x001C8698
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

		// Token: 0x06005CBD RID: 23741 RVA: 0x001CA564 File Offset: 0x001C8764
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
	}
}
