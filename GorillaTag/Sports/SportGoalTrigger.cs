using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000D45 RID: 3397
	public class SportGoalTrigger : MonoBehaviour
	{
		// Token: 0x06005508 RID: 21768 RVA: 0x0019E057 File Offset: 0x0019C257
		public void BallExitedGoalTrigger(SportBall ball)
		{
			if (this.ballsPendingTriggerExit.Contains(ball))
			{
				this.ballsPendingTriggerExit.Remove(ball);
			}
		}

		// Token: 0x06005509 RID: 21769 RVA: 0x0019E074 File Offset: 0x0019C274
		private void PruneBallsPendingTriggerExitByDistance()
		{
			foreach (SportBall sportBall in this.ballsPendingTriggerExit)
			{
				if ((sportBall.transform.position - base.transform.position).sqrMagnitude > this.ballTriggerExitDistanceFallback * this.ballTriggerExitDistanceFallback)
				{
					this.ballsPendingTriggerExit.Remove(sportBall);
				}
			}
		}

		// Token: 0x0600550A RID: 21770 RVA: 0x0019E100 File Offset: 0x0019C300
		private void OnTriggerEnter(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.scoreboard != null)
			{
				this.PruneBallsPendingTriggerExitByDistance();
				if (!this.ballsPendingTriggerExit.Contains(componentInParent))
				{
					this.scoreboard.TeamScored(this.teamScoringOnThisGoal);
					this.ballsPendingTriggerExit.Add(componentInParent);
				}
			}
		}

		// Token: 0x0400584F RID: 22607
		[SerializeField]
		private SportScoreboard scoreboard;

		// Token: 0x04005850 RID: 22608
		[SerializeField]
		private int teamScoringOnThisGoal = 1;

		// Token: 0x04005851 RID: 22609
		[SerializeField]
		private float ballTriggerExitDistanceFallback = 3f;

		// Token: 0x04005852 RID: 22610
		private HashSet<SportBall> ballsPendingTriggerExit = new HashSet<SportBall>();
	}
}
