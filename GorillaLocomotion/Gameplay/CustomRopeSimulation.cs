using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CD8 RID: 3288
	public class CustomRopeSimulation : MonoBehaviour
	{
		// Token: 0x0600518E RID: 20878 RVA: 0x0018BCC4 File Offset: 0x00189EC4
		private void Start()
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.nodeCount; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.ropeNodePrefab);
				gameObject.transform.parent = base.transform;
				gameObject.transform.position = position;
				this.nodes.Add(gameObject.transform);
				position.y -= this.nodeDistance;
			}
			this.nodes[this.nodes.Count - 1].GetComponentInChildren<Renderer>().enabled = false;
			this.burstNodes = new NativeArray<BurstRopeNode>(this.nodes.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}

		// Token: 0x0600518F RID: 20879 RVA: 0x0018BD74 File Offset: 0x00189F74
		private void OnDestroy()
		{
			this.burstNodes.Dispose();
		}

		// Token: 0x06005190 RID: 20880 RVA: 0x0018BD84 File Offset: 0x00189F84
		private void Update()
		{
			new SolveRopeJob
			{
				fixedDeltaTime = Time.deltaTime,
				gravity = this.gravity,
				nodes = this.burstNodes,
				nodeDistance = this.nodeDistance,
				rootPos = base.transform.position
			}.Run<SolveRopeJob>();
			for (int i = 0; i < this.burstNodes.Length; i++)
			{
				this.nodes[i].position = this.burstNodes[i].curPos;
				if (i > 0)
				{
					Vector3 vector = this.burstNodes[i - 1].curPos - this.burstNodes[i].curPos;
					this.nodes[i].up = -vector;
				}
			}
		}

		// Token: 0x040055AE RID: 21934
		private List<Transform> nodes = new List<Transform>();

		// Token: 0x040055AF RID: 21935
		[SerializeField]
		private GameObject ropeNodePrefab;

		// Token: 0x040055B0 RID: 21936
		[SerializeField]
		private int nodeCount = 10;

		// Token: 0x040055B1 RID: 21937
		[SerializeField]
		private float nodeDistance = 0.4f;

		// Token: 0x040055B2 RID: 21938
		[SerializeField]
		private Vector3 gravity = new Vector3(0f, -9.81f, 0f);

		// Token: 0x040055B3 RID: 21939
		private NativeArray<BurstRopeNode> burstNodes;
	}
}
