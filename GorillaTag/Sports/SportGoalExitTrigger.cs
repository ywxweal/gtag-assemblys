using System;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000D44 RID: 3396
	public class SportGoalExitTrigger : MonoBehaviour
	{
		// Token: 0x06005507 RID: 21767 RVA: 0x0019E0F8 File Offset: 0x0019C2F8
		private void OnTriggerExit(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.goalTrigger != null)
			{
				this.goalTrigger.BallExitedGoalTrigger(componentInParent);
			}
		}

		// Token: 0x0400584F RID: 22607
		[SerializeField]
		private SportGoalTrigger goalTrigger;
	}
}
