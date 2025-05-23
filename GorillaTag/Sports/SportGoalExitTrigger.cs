using System;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000D44 RID: 3396
	public class SportGoalExitTrigger : MonoBehaviour
	{
		// Token: 0x06005506 RID: 21766 RVA: 0x0019E020 File Offset: 0x0019C220
		private void OnTriggerExit(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.goalTrigger != null)
			{
				this.goalTrigger.BallExitedGoalTrigger(componentInParent);
			}
		}

		// Token: 0x0400584E RID: 22606
		[SerializeField]
		private SportGoalTrigger goalTrigger;
	}
}
