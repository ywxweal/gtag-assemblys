using System;
using System.Runtime.CompilerServices;
using GorillaTagScripts.AI.States;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.AI.Entities
{
	// Token: 0x02000B9F RID: 2975
	public class TestShark : AIEntity
	{
		// Token: 0x060049BA RID: 18874 RVA: 0x00160250 File Offset: 0x0015E450
		private new void Awake()
		{
			base.Awake();
			this.chasingTimer = 0f;
			this._stateMachine = new StateMachine();
			this.circularPatrol = new CircularPatrol_State(this);
			this.patrol = new Patrol_State(this);
			this.chase = new Chase_State(this);
			this._stateMachine.AddTransition(this.patrol, this.chase, this.<Awake>g__ShouldChase|7_0());
			this._stateMachine.AddTransition(this.chase, this.patrol, this.<Awake>g__ShouldPatrol|7_1());
			this._stateMachine.SetState(this.patrol);
		}

		// Token: 0x060049BB RID: 18875 RVA: 0x001602E8 File Offset: 0x0015E4E8
		private void Update()
		{
			this._stateMachine.Tick();
			this.shouldChase = false;
			this.chasingTimer += Time.deltaTime;
			if (this.chasingTimer >= this.nextTimeToChasePlayer)
			{
				base.ChooseClosestTarget();
				if (this.followTarget != null)
				{
					this.chase.FollowTarget = this.followTarget;
					this.shouldChase = true;
				}
				this.chasingTimer = 0f;
			}
		}

		// Token: 0x060049BD RID: 18877 RVA: 0x00160371 File Offset: 0x0015E571
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldChase|7_0()
		{
			return () => this.shouldChase && PhotonNetwork.InRoom;
		}

		// Token: 0x060049BF RID: 18879 RVA: 0x00160390 File Offset: 0x0015E590
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldPatrol|7_1()
		{
			return () => this.chase.chaseOver;
		}

		// Token: 0x04004C8E RID: 19598
		public float nextTimeToChasePlayer = 30f;

		// Token: 0x04004C8F RID: 19599
		private float chasingTimer;

		// Token: 0x04004C90 RID: 19600
		private bool shouldChase;

		// Token: 0x04004C91 RID: 19601
		private StateMachine _stateMachine;

		// Token: 0x04004C92 RID: 19602
		private CircularPatrol_State circularPatrol;

		// Token: 0x04004C93 RID: 19603
		private Patrol_State patrol;

		// Token: 0x04004C94 RID: 19604
		private Chase_State chase;
	}
}
