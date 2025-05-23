using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B4B RID: 2891
	public class TappableBell : Tappable
	{
		// Token: 0x1400007D RID: 125
		// (add) Token: 0x06004754 RID: 18260 RVA: 0x00153260 File Offset: 0x00151460
		// (remove) Token: 0x06004755 RID: 18261 RVA: 0x00153298 File Offset: 0x00151498
		public event TappableBell.ObstacleCourseTriggerEvent OnTapped;

		// Token: 0x06004756 RID: 18262 RVA: 0x001532D0 File Offset: 0x001514D0
		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
		{
			if (!PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				return;
			}
			if (!this.rpcCooldown.CheckCallTime(Time.time))
			{
				return;
			}
			this.winnerRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (this.winnerRig != null)
			{
				TappableBell.ObstacleCourseTriggerEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(this.winnerRig);
			}
		}

		// Token: 0x040049AA RID: 18858
		private VRRig winnerRig;

		// Token: 0x040049AC RID: 18860
		public CallLimiter rpcCooldown;

		// Token: 0x02000B4C RID: 2892
		// (Invoke) Token: 0x06004759 RID: 18265
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
