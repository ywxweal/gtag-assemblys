using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000B97 RID: 2967
	public class AIEntity : MonoBehaviour
	{
		// Token: 0x06004997 RID: 18839 RVA: 0x0015FA74 File Offset: 0x0015DC74
		protected void Awake()
		{
			this.navMeshAgent = base.gameObject.GetComponent<NavMeshAgent>();
			this.animator = base.gameObject.GetComponent<Animator>();
			if (this.waypointsContainer != null)
			{
				foreach (Transform transform in this.waypointsContainer.GetComponentsInChildren<Transform>())
				{
					this.waypoints.Add(transform);
				}
			}
		}

		// Token: 0x06004998 RID: 18840 RVA: 0x0015FADC File Offset: 0x0015DCDC
		protected void ChooseRandomTarget()
		{
			int randomTarget = Random.Range(0, GorillaParent.instance.vrrigs.Count);
			int num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == GorillaParent.instance.vrrigs[randomTarget].creator);
			if (num == -1)
			{
				num = Random.Range(0, GorillaParent.instance.vrrigs.Count);
			}
			if (num < GorillaParent.instance.vrrigs.Count)
			{
				this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
				this.followTarget = GorillaParent.instance.vrrigs[num].head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		// Token: 0x06004999 RID: 18841 RVA: 0x0015FBCC File Offset: 0x0015DDCC
		protected void ChooseClosestTarget()
		{
			VRRig vrrig = null;
			float num = float.MaxValue;
			foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
			{
				if (vrrig2.head != null && !(vrrig2.head.rigTarget == null))
				{
					float sqrMagnitude = (base.transform.position - vrrig2.head.rigTarget.transform.position).sqrMagnitude;
					if (sqrMagnitude < this.minChaseRange * this.minChaseRange && sqrMagnitude < num)
					{
						num = sqrMagnitude;
						vrrig = vrrig2;
					}
				}
			}
			if (vrrig != null)
			{
				this.targetPlayer = vrrig.creator;
				this.followTarget = vrrig.head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		// Token: 0x04004C6B RID: 19563
		public GameObject waypointsContainer;

		// Token: 0x04004C6C RID: 19564
		public Transform circleCenter;

		// Token: 0x04004C6D RID: 19565
		public float circleRadius;

		// Token: 0x04004C6E RID: 19566
		public float angularSpeed;

		// Token: 0x04004C6F RID: 19567
		public float patrolSpeed;

		// Token: 0x04004C70 RID: 19568
		public float fleeSpeed;

		// Token: 0x04004C71 RID: 19569
		public NavMeshAgent navMeshAgent;

		// Token: 0x04004C72 RID: 19570
		public Animator animator;

		// Token: 0x04004C73 RID: 19571
		public float fleeRang;

		// Token: 0x04004C74 RID: 19572
		public float fleeSpeedMult;

		// Token: 0x04004C75 RID: 19573
		public float minChaseRange;

		// Token: 0x04004C76 RID: 19574
		public float attackDistance;

		// Token: 0x04004C77 RID: 19575
		public float navMeshSampleRange = 5f;

		// Token: 0x04004C78 RID: 19576
		internal readonly List<Transform> waypoints = new List<Transform>();

		// Token: 0x04004C79 RID: 19577
		internal float defaultSpeed;

		// Token: 0x04004C7A RID: 19578
		public Transform followTarget;

		// Token: 0x04004C7B RID: 19579
		public NetPlayer targetPlayer;

		// Token: 0x04004C7C RID: 19580
		public bool targetIsOnNavMesh;
	}
}
