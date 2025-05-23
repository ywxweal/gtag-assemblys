using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000B9E RID: 2974
	public class Patrol_State : IState
	{
		// Token: 0x060049B5 RID: 18869 RVA: 0x00160083 File Offset: 0x0015E283
		public Patrol_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x060049B6 RID: 18870 RVA: 0x001600A4 File Offset: 0x0015E2A4
		public void Tick()
		{
			if (this.agent.remainingDistance <= this.agent.stoppingDistance)
			{
				Vector3 position = this.entity.waypoints[Random.Range(0, this.entity.waypoints.Count - 1)].transform.position;
				this.agent.SetDestination(position);
			}
		}

		// Token: 0x060049B7 RID: 18871 RVA: 0x0016010C File Offset: 0x0015E30C
		public void OnEnter()
		{
			string text = "Current State: ";
			Type typeFromHandle = typeof(Patrol_State);
			Debug.Log(text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
			if (this.entity.waypoints.Count > 0)
			{
				this.agent.SetDestination(this.entity.waypoints[0].transform.position);
			}
		}

		// Token: 0x060049B8 RID: 18872 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnExit()
		{
		}

		// Token: 0x04004C8B RID: 19595
		private AIEntity entity;

		// Token: 0x04004C8C RID: 19596
		private NavMeshAgent agent;
	}
}
