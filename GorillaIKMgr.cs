using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000617 RID: 1559
public class GorillaIKMgr : MonoBehaviour
{
	// Token: 0x170003AC RID: 940
	// (get) Token: 0x060026CA RID: 9930 RVA: 0x000C0738 File Offset: 0x000BE938
	public static GorillaIKMgr Instance
	{
		get
		{
			return GorillaIKMgr._instance;
		}
	}

	// Token: 0x060026CB RID: 9931 RVA: 0x000C0740 File Offset: 0x000BE940
	private void Awake()
	{
		GorillaIKMgr._instance = this;
		this.firstFrame = true;
		this.tAA = new TransformAccessArray(0, -1);
		this.transformList = new List<Transform>();
		this.job = new GorillaIKMgr.IKJob
		{
			constantInput = new NativeArray<GorillaIKMgr.IKConstantInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			input = new NativeArray<GorillaIKMgr.IKInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			output = new NativeArray<GorillaIKMgr.IKOutput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory)
		};
		this.jobXform = new GorillaIKMgr.IKTransformJob
		{
			transformRotations = new NativeArray<Quaternion>(140, Allocator.Persistent, NativeArrayOptions.ClearMemory)
		};
	}

	// Token: 0x060026CC RID: 9932 RVA: 0x000C07D4 File Offset: 0x000BE9D4
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.jobXformHandle.Complete();
		this.jobXform.transformRotations.Dispose();
		this.tAA.Dispose();
		this.job.input.Dispose();
		this.job.constantInput.Dispose();
		this.job.output.Dispose();
	}

	// Token: 0x060026CD RID: 9933 RVA: 0x000C0844 File Offset: 0x000BEA44
	public void RegisterIK(GorillaIK ik)
	{
		this.ikList.Add(ik);
		this.actualListSz += 2;
		this.updatedSinceLastRun = true;
		if (this.job.constantInput.IsCreated)
		{
			this.SetConstantData(ik, this.actualListSz - 2);
		}
	}

	// Token: 0x060026CE RID: 9934 RVA: 0x000C0894 File Offset: 0x000BEA94
	public void DeregisterIK(GorillaIK ik)
	{
		int num = this.ikList.FindIndex((GorillaIK curr) => curr == ik);
		this.updatedSinceLastRun = true;
		this.ikList.RemoveAt(num);
		this.actualListSz -= 2;
		if (this.job.constantInput.IsCreated)
		{
			for (int i = num; i < this.actualListSz; i++)
			{
				this.job.constantInput[i] = this.job.constantInput[i + 2];
			}
		}
	}

	// Token: 0x060026CF RID: 9935 RVA: 0x000C0930 File Offset: 0x000BEB30
	private void SetConstantData(GorillaIK ik, int index)
	{
		this.job.constantInput[index] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerLeft,
			initRotUpper = ik.initialUpperLeft
		};
		this.job.constantInput[index + 1] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerRight,
			initRotUpper = ik.initialUpperRight
		};
	}

	// Token: 0x060026D0 RID: 9936 RVA: 0x000C09A8 File Offset: 0x000BEBA8
	private void CopyInput()
	{
		int num = 0;
		int i = 0;
		while (i < this.actualListSz)
		{
			GorillaIK gorillaIK = this.ikList[i / 2];
			this.job.input[i] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.leftUpperArm.parent.InverseTransformPoint(gorillaIK.targetLeft.position)
			};
			this.job.input[i + 1] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.rightUpperArm.parent.InverseTransformPoint(gorillaIK.targetRight.position)
			};
			i += 2;
			num++;
		}
	}

	// Token: 0x060026D1 RID: 9937 RVA: 0x000C0A5C File Offset: 0x000BEC5C
	private void CopyOutput()
	{
		bool flag = false;
		if (this.updatedSinceLastRun || this.tAA.length != this.ikList.Count * 7)
		{
			flag = true;
			this.tAA.Dispose();
			this.transformList.Clear();
		}
		for (int i = 0; i < this.ikList.Count; i++)
		{
			GorillaIK gorillaIK = this.ikList[i];
			if (flag || this.updatedSinceLastRun)
			{
				this.transformList.Add(gorillaIK.leftUpperArm);
				this.transformList.Add(gorillaIK.leftLowerArm);
				this.transformList.Add(gorillaIK.rightUpperArm);
				this.transformList.Add(gorillaIK.rightLowerArm);
				this.transformList.Add(gorillaIK.headBone);
				this.transformList.Add(gorillaIK.leftHand);
				this.transformList.Add(gorillaIK.rightHand);
			}
			this.jobXform.transformRotations[this.tFormCount * i] = this.job.output[i * 2].upperArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 1] = this.job.output[i * 2].lowerArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 2] = this.job.output[i * 2 + 1].upperArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 3] = this.job.output[i * 2 + 1].lowerArmLocalRot;
			this.jobXform.transformRotations[this.tFormCount * i + 4] = gorillaIK.targetHead.rotation;
			this.jobXform.transformRotations[this.tFormCount * i + 5] = gorillaIK.targetLeft.rotation;
			this.jobXform.transformRotations[this.tFormCount * i + 6] = gorillaIK.targetRight.rotation;
		}
		if (flag)
		{
			this.tAA = new TransformAccessArray(this.transformList.ToArray(), -1);
		}
		this.updatedSinceLastRun = false;
	}

	// Token: 0x060026D2 RID: 9938 RVA: 0x000C0CAC File Offset: 0x000BEEAC
	public void LateUpdate()
	{
		if (!this.firstFrame)
		{
			this.jobXformHandle.Complete();
		}
		this.CopyInput();
		this.jobHandle = this.job.Schedule(this.actualListSz, 20, default(JobHandle));
		this.jobHandle.Complete();
		this.CopyOutput();
		this.jobXformHandle = this.jobXform.Schedule(this.tAA, default(JobHandle));
		this.firstFrame = false;
	}

	// Token: 0x04002B3F RID: 11071
	[OnEnterPlay_SetNull]
	private static GorillaIKMgr _instance;

	// Token: 0x04002B40 RID: 11072
	private const int MaxSize = 20;

	// Token: 0x04002B41 RID: 11073
	private List<GorillaIK> ikList = new List<GorillaIK>(20);

	// Token: 0x04002B42 RID: 11074
	private int actualListSz;

	// Token: 0x04002B43 RID: 11075
	private JobHandle jobHandle;

	// Token: 0x04002B44 RID: 11076
	private JobHandle jobXformHandle;

	// Token: 0x04002B45 RID: 11077
	private bool firstFrame = true;

	// Token: 0x04002B46 RID: 11078
	private TransformAccessArray tAA;

	// Token: 0x04002B47 RID: 11079
	private List<Transform> transformList;

	// Token: 0x04002B48 RID: 11080
	private bool updatedSinceLastRun;

	// Token: 0x04002B49 RID: 11081
	private int tFormCount = 7;

	// Token: 0x04002B4A RID: 11082
	private GorillaIKMgr.IKJob job;

	// Token: 0x04002B4B RID: 11083
	private GorillaIKMgr.IKTransformJob jobXform;

	// Token: 0x02000618 RID: 1560
	private struct IKConstantInput
	{
		// Token: 0x04002B4C RID: 11084
		public Quaternion initRotLower;

		// Token: 0x04002B4D RID: 11085
		public Quaternion initRotUpper;
	}

	// Token: 0x02000619 RID: 1561
	private struct IKInput
	{
		// Token: 0x04002B4E RID: 11086
		public Vector3 targetPos;
	}

	// Token: 0x0200061A RID: 1562
	private struct IKOutput
	{
		// Token: 0x060026D4 RID: 9940 RVA: 0x000C0D4F File Offset: 0x000BEF4F
		public IKOutput(Quaternion upperArmLocalRot_, Quaternion lowerArmLocalRot_)
		{
			this.upperArmLocalRot = upperArmLocalRot_;
			this.lowerArmLocalRot = lowerArmLocalRot_;
		}

		// Token: 0x04002B4F RID: 11087
		public Quaternion upperArmLocalRot;

		// Token: 0x04002B50 RID: 11088
		public Quaternion lowerArmLocalRot;
	}

	// Token: 0x0200061B RID: 1563
	[BurstCompile]
	private struct IKJob : IJobParallelFor
	{
		// Token: 0x060026D5 RID: 9941 RVA: 0x000C0D60 File Offset: 0x000BEF60
		public void Execute(int i)
		{
			Quaternion initRotUpper = this.constantInput[i].initRotUpper;
			Vector3 vector = GorillaIKMgr.IKJob.upperArmLocalPos;
			Quaternion quaternion = initRotUpper * this.constantInput[i].initRotLower;
			Vector3 vector2 = vector + initRotUpper * GorillaIKMgr.IKJob.forearmLocalPos;
			Vector3 vector3 = vector2 + quaternion * GorillaIKMgr.IKJob.handLocalPos;
			float num = 0f;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (vector2 - vector3).magnitude;
			float num2 = magnitude + magnitude2 - num;
			Vector3 normalized = (vector3 - vector).normalized;
			Vector3 normalized2 = (vector2 - vector).normalized;
			Vector3 normalized3 = (vector3 - vector2).normalized;
			Vector3 normalized4 = (this.input[i].targetPos - vector).normalized;
			float num3 = Mathf.Clamp((this.input[i].targetPos - vector).magnitude, num, num2);
			float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized2), -1f, 1f));
			float num5 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(-normalized2, normalized3), -1f, 1f));
			float num6 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized4), -1f, 1f));
			float num7 = Mathf.Acos(Mathf.Clamp((magnitude2 * magnitude2 - magnitude * magnitude - num3 * num3) / (-2f * magnitude * num3), -1f, 1f));
			float num8 = Mathf.Acos(Mathf.Clamp((num3 * num3 - magnitude * magnitude - magnitude2 * magnitude2) / (-2f * magnitude * magnitude2), -1f, 1f));
			Vector3 normalized5 = Vector3.Cross(normalized, normalized2).normalized;
			Vector3 normalized6 = Vector3.Cross(normalized, normalized4).normalized;
			Quaternion quaternion2 = Quaternion.AngleAxis((num7 - num4) * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized5);
			Quaternion quaternion3 = Quaternion.AngleAxis((num8 - num5) * 57.29578f, Quaternion.Inverse(quaternion) * normalized5);
			Quaternion quaternion4 = Quaternion.AngleAxis(num6 * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized6);
			Quaternion quaternion5 = this.constantInput[i].initRotUpper * quaternion4 * quaternion2;
			Quaternion quaternion6 = this.constantInput[i].initRotLower * quaternion3;
			this.output[i] = new GorillaIKMgr.IKOutput(quaternion5, quaternion6);
		}

		// Token: 0x04002B51 RID: 11089
		public NativeArray<GorillaIKMgr.IKConstantInput> constantInput;

		// Token: 0x04002B52 RID: 11090
		public NativeArray<GorillaIKMgr.IKInput> input;

		// Token: 0x04002B53 RID: 11091
		public NativeArray<GorillaIKMgr.IKOutput> output;

		// Token: 0x04002B54 RID: 11092
		private static readonly Vector3 upperArmLocalPos = new Vector3(-0.0002577677f, 0.1454885f, -0.02598158f);

		// Token: 0x04002B55 RID: 11093
		private static readonly Vector3 forearmLocalPos = new Vector3(4.204223E-06f, 0.4061671f, -1.043081E-06f);

		// Token: 0x04002B56 RID: 11094
		private static readonly Vector3 handLocalPos = new Vector3(3.073364E-08f, 0.3816895f, 1.117587E-08f);
	}

	// Token: 0x0200061C RID: 1564
	[BurstCompile]
	private struct IKTransformJob : IJobParallelForTransform
	{
		// Token: 0x060026D7 RID: 9943 RVA: 0x000C1068 File Offset: 0x000BF268
		public void Execute(int index, TransformAccess xform)
		{
			if (index % 7 <= 3)
			{
				xform.localRotation = this.transformRotations[index];
				return;
			}
			xform.rotation = this.transformRotations[index];
		}

		// Token: 0x04002B57 RID: 11095
		public NativeArray<Quaternion> transformRotations;
	}
}
