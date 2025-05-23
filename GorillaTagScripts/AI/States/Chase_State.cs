using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000B9C RID: 2972
	public class Chase_State : IState
	{
		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x060049AB RID: 18859 RVA: 0x0015FF3C File Offset: 0x0015E13C
		// (set) Token: 0x060049AC RID: 18860 RVA: 0x0015FF44 File Offset: 0x0015E144
		public Transform FollowTarget { get; set; }

		// Token: 0x060049AD RID: 18861 RVA: 0x0015FF4D File Offset: 0x0015E14D
		public Chase_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x060049AE RID: 18862 RVA: 0x0015FF6D File Offset: 0x0015E16D
		public void Tick()
		{
			this.agent.SetDestination(this.FollowTarget.position);
			if (this.agent.remainingDistance < this.entity.attackDistance)
			{
				this.chaseOver = true;
			}
		}

		// Token: 0x060049AF RID: 18863 RVA: 0x0015FFA5 File Offset: 0x0015E1A5
		public void OnEnter()
		{
			this.chaseOver = false;
			string text = "Current State: ";
			Type typeFromHandle = typeof(Chase_State);
			Debug.Log(text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
		}

		// Token: 0x060049B0 RID: 18864 RVA: 0x0015FFD3 File Offset: 0x0015E1D3
		public void OnExit()
		{
			this.chaseOver = true;
		}

		// Token: 0x04004C85 RID: 19589
		private AIEntity entity;

		// Token: 0x04004C86 RID: 19590
		private NavMeshAgent agent;

		// Token: 0x04004C88 RID: 19592
		public bool chaseOver;
	}
}
