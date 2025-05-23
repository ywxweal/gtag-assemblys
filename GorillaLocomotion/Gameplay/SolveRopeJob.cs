using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CDA RID: 3290
	[BurstCompile]
	public struct SolveRopeJob : IJob
	{
		// Token: 0x06005191 RID: 20881 RVA: 0x0018BDCC File Offset: 0x00189FCC
		public void Execute()
		{
			this.Simulate();
			for (int i = 0; i < 20; i++)
			{
				this.ApplyConstraint();
			}
		}

		// Token: 0x06005192 RID: 20882 RVA: 0x0018BDF4 File Offset: 0x00189FF4
		private void Simulate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				BurstRopeNode burstRopeNode = this.nodes[i];
				Vector3 vector = burstRopeNode.curPos - burstRopeNode.lastPos;
				burstRopeNode.lastPos = burstRopeNode.curPos;
				Vector3 vector2 = burstRopeNode.curPos + vector;
				vector2 += this.gravity * this.fixedDeltaTime;
				burstRopeNode.curPos = vector2;
				this.nodes[i] = burstRopeNode;
			}
		}

		// Token: 0x06005193 RID: 20883 RVA: 0x0018BE80 File Offset: 0x0018A080
		private void ApplyConstraint()
		{
			BurstRopeNode burstRopeNode = this.nodes[0];
			burstRopeNode.curPos = this.rootPos;
			this.nodes[0] = burstRopeNode;
			for (int i = 0; i < this.nodes.Length - 1; i++)
			{
				BurstRopeNode burstRopeNode2 = this.nodes[i];
				BurstRopeNode burstRopeNode3 = this.nodes[i + 1];
				float magnitude = (burstRopeNode2.curPos - burstRopeNode3.curPos).magnitude;
				float num = Mathf.Abs(magnitude - this.nodeDistance);
				Vector3 vector = Vector3.zero;
				if (magnitude > this.nodeDistance)
				{
					vector = (burstRopeNode2.curPos - burstRopeNode3.curPos).normalized;
				}
				else if (magnitude < this.nodeDistance)
				{
					vector = (burstRopeNode3.curPos - burstRopeNode2.curPos).normalized;
				}
				Vector3 vector2 = vector * num;
				burstRopeNode2.curPos -= vector2 * 0.5f;
				burstRopeNode3.curPos += vector2 * 0.5f;
				this.nodes[i] = burstRopeNode2;
				this.nodes[i + 1] = burstRopeNode3;
			}
		}

		// Token: 0x040055B5 RID: 21941
		[ReadOnly]
		public float fixedDeltaTime;

		// Token: 0x040055B6 RID: 21942
		[WriteOnly]
		public NativeArray<BurstRopeNode> nodes;

		// Token: 0x040055B7 RID: 21943
		[ReadOnly]
		public Vector3 gravity;

		// Token: 0x040055B8 RID: 21944
		[ReadOnly]
		public Vector3 rootPos;

		// Token: 0x040055B9 RID: 21945
		[ReadOnly]
		public float nodeDistance;
	}
}
