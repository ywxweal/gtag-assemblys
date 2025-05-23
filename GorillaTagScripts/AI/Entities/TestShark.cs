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
		// Token: 0x060049B9 RID: 18873 RVA: 0x00160178 File Offset: 0x0015E378
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

		// Token: 0x060049BA RID: 18874 RVA: 0x00160210 File Offset: 0x0015E410
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

		// Token: 0x060049BC RID: 18876 RVA: 0x00160299 File Offset: 0x0015E499
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldChase|7_0()
		{
			return () => this.shouldChase && PhotonNetwork.InRoom;
		}

		// Token: 0x060049BE RID: 18878 RVA: 0x001602B8 File Offset: 0x0015E4B8
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldPatrol|7_1()
		{
			return () => this.chase.chaseOver;
		}

		// Token: 0x04004C8D RID: 19597
		public float nextTimeToChasePlayer = 30f;

		// Token: 0x04004C8E RID: 19598
		private float chasingTimer;

		// Token: 0x04004C8F RID: 19599
		private bool shouldChase;

		// Token: 0x04004C90 RID: 19600
		private StateMachine _stateMachine;

		// Token: 0x04004C91 RID: 19601
		private CircularPatrol_State circularPatrol;

		// Token: 0x04004C92 RID: 19602
		private Patrol_State patrol;

		// Token: 0x04004C93 RID: 19603
		private Chase_State chase;
	}
}
