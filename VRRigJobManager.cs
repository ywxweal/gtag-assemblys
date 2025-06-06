﻿using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x0200073F RID: 1855
[DefaultExecutionOrder(0)]
public class VRRigJobManager : MonoBehaviour
{
	// Token: 0x17000489 RID: 1161
	// (get) Token: 0x06002E67 RID: 11879 RVA: 0x000E7C44 File Offset: 0x000E5E44
	public static VRRigJobManager Instance
	{
		get
		{
			return VRRigJobManager._instance;
		}
	}

	// Token: 0x06002E68 RID: 11880 RVA: 0x000E7C4B File Offset: 0x000E5E4B
	private void Awake()
	{
		VRRigJobManager._instance = this;
		this.cachedInput = new NativeArray<VRRigJobManager.VRRigTransformInput>(9, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.tAA = new TransformAccessArray(9, 2);
	}

	// Token: 0x06002E69 RID: 11881 RVA: 0x000E7C70 File Offset: 0x000E5E70
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.cachedInput.Dispose();
		this.tAA.Dispose();
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x000E7C93 File Offset: 0x000E5E93
	public void RegisterVRRig(VRRig rig)
	{
		this.rigList.Add(rig);
		this.tAA.Add(rig.transform);
		this.actualListSz++;
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x000E7CC0 File Offset: 0x000E5EC0
	public void DeregisterVRRig(VRRig rig)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.rigList.Remove(rig);
		for (int i = this.actualListSz - 1; i >= 0; i--)
		{
			if (this.tAA[i] == rig.transform)
			{
				this.tAA.RemoveAtSwapBack(i);
				break;
			}
		}
		this.actualListSz--;
	}

	// Token: 0x06002E6C RID: 11884 RVA: 0x000E7D2C File Offset: 0x000E5F2C
	private void CopyInput()
	{
		for (int i = 0; i < this.actualListSz; i++)
		{
			this.cachedInput[i] = new VRRigJobManager.VRRigTransformInput
			{
				rigPosition = this.rigList[i].jobPos,
				rigRotaton = this.rigList[i].jobRotation
			};
			this.tAA[i] = this.rigList[i].transform;
		}
	}

	// Token: 0x06002E6D RID: 11885 RVA: 0x000E7DAC File Offset: 0x000E5FAC
	public void Update()
	{
		this.jobHandle.Complete();
		for (int i = 0; i < this.rigList.Count; i++)
		{
			this.rigList[i].RemoteRigUpdate();
		}
		this.CopyInput();
		VRRigJobManager.VRRigTransformJob vrrigTransformJob = new VRRigJobManager.VRRigTransformJob
		{
			input = this.cachedInput
		};
		this.jobHandle = vrrigTransformJob.Schedule(this.tAA, default(JobHandle));
	}

	// Token: 0x040034E2 RID: 13538
	[OnEnterPlay_SetNull]
	private static VRRigJobManager _instance;

	// Token: 0x040034E3 RID: 13539
	private const int MaxSize = 9;

	// Token: 0x040034E4 RID: 13540
	private const int questJobThreads = 2;

	// Token: 0x040034E5 RID: 13541
	private List<VRRig> rigList = new List<VRRig>(9);

	// Token: 0x040034E6 RID: 13542
	private NativeArray<VRRigJobManager.VRRigTransformInput> cachedInput;

	// Token: 0x040034E7 RID: 13543
	private TransformAccessArray tAA;

	// Token: 0x040034E8 RID: 13544
	private int actualListSz;

	// Token: 0x040034E9 RID: 13545
	private JobHandle jobHandle;

	// Token: 0x02000740 RID: 1856
	private struct VRRigTransformInput
	{
		// Token: 0x040034EA RID: 13546
		public Vector3 rigPosition;

		// Token: 0x040034EB RID: 13547
		public Quaternion rigRotaton;
	}

	// Token: 0x02000741 RID: 1857
	[BurstCompile]
	private struct VRRigTransformJob : IJobParallelForTransform
	{
		// Token: 0x06002E6F RID: 11887 RVA: 0x000E7E38 File Offset: 0x000E6038
		public void Execute(int i, TransformAccess tA)
		{
			if (i < this.input.Length)
			{
				tA.position = this.input[i].rigPosition;
				tA.rotation = this.input[i].rigRotaton;
			}
		}

		// Token: 0x040034EC RID: 13548
		[ReadOnly]
		public NativeArray<VRRigJobManager.VRRigTransformInput> input;
	}
}
