using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B4B RID: 2891
	public class TappableBell : Tappable
	{
		// Token: 0x1400007D RID: 125
		// (add) Token: 0x06004753 RID: 18259 RVA: 0x00153188 File Offset: 0x00151388
		// (remove) Token: 0x06004754 RID: 18260 RVA: 0x001531C0 File Offset: 0x001513C0
		public event TappableBell.ObstacleCourseTriggerEvent OnTapped;

		// Token: 0x06004755 RID: 18261 RVA: 0x001531F8 File Offset: 0x001513F8
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

		// Token: 0x040049A9 RID: 18857
		private VRRig winnerRig;

		// Token: 0x040049AB RID: 18859
		public CallLimiter rpcCooldown;

		// Token: 0x02000B4C RID: 2892
		// (Invoke) Token: 0x06004758 RID: 18264
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
