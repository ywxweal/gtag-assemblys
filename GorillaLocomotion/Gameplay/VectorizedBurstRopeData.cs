using System;
using Unity.Collections;
using Unity.Mathematics;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CEA RID: 3306
	public struct VectorizedBurstRopeData
	{
		// Token: 0x0400563D RID: 22077
		public NativeArray<float4> posX;

		// Token: 0x0400563E RID: 22078
		public NativeArray<float4> posY;

		// Token: 0x0400563F RID: 22079
		public NativeArray<float4> posZ;

		// Token: 0x04005640 RID: 22080
		public NativeArray<int4> validNodes;

		// Token: 0x04005641 RID: 22081
		public NativeArray<float4> lastPosX;

		// Token: 0x04005642 RID: 22082
		public NativeArray<float4> lastPosY;

		// Token: 0x04005643 RID: 22083
		public NativeArray<float4> lastPosZ;

		// Token: 0x04005644 RID: 22084
		public NativeArray<float3> ropeRoots;

		// Token: 0x04005645 RID: 22085
		public NativeArray<float4> nodeMass;
	}
}
