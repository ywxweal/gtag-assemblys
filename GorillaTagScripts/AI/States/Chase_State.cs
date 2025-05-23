using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000B9C RID: 2972
	public class Chase_State : IState
	{
		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x060049AC RID: 18860 RVA: 0x00160014 File Offset: 0x0015E214
		// (set) Token: 0x060049AD RID: 18861 RVA: 0x0016001C File Offset: 0x0015E21C
		public Transform FollowTarget { get; set; }

		// Token: 0x060049AE RID: 18862 RVA: 0x00160025 File Offset: 0x0015E225
		public Chase_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x060049AF RID: 18863 RVA: 0x00160045 File Offset: 0x0015E245
		public void Tick()
		{
			this.agent.SetDestination(this.FollowTarget.position);
			if (this.agent.remainingDistance < this.entity.attackDistance)
			{
				this.chaseOver = true;
			}
		}

		// Token: 0x060049B0 RID: 18864 RVA: 0x0016007D File Offset: 0x0015E27D
		public void OnEnter()
		{
			this.chaseOver = false;
			string text = "Current State: ";
			Type typeFromHandle = typeof(Chase_State);
			Debug.Log(text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
		}

		// Token: 0x060049B1 RID: 18865 RVA: 0x001600AB File Offset: 0x0015E2AB
		public void OnExit()
		{
			this.chaseOver = true;
		}

		// Token: 0x04004C86 RID: 19590
		private AIEntity entity;

		// Token: 0x04004C87 RID: 19591
		private NavMeshAgent agent;

		// Token: 0x04004C89 RID: 19593
		public bool chaseOver;
	}
}
