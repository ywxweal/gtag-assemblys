using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000D40 RID: 3392
	[Serializable]
	internal class ExpectedUsersDecayTimer : TickSystemTimerAbstract
	{
		// Token: 0x060054FA RID: 21754 RVA: 0x0019DE58 File Offset: 0x0019C058
		public override void OnTimedEvent()
		{
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient)
			{
				int num = 0;
				if (PhotonNetwork.CurrentRoom.ExpectedUsers != null && PhotonNetwork.CurrentRoom.ExpectedUsers.Length != 0)
				{
					foreach (string text in PhotonNetwork.CurrentRoom.ExpectedUsers)
					{
						float num2;
						if (this.expectedUsers.TryGetValue(text, out num2))
						{
							if (num2 + this.decayTime < Time.time)
							{
								num++;
							}
						}
						else
						{
							this.expectedUsers.Add(text, Time.time);
						}
					}
					if (num >= PhotonNetwork.CurrentRoom.ExpectedUsers.Length && num != 0)
					{
						PhotonNetwork.CurrentRoom.ClearExpectedUsers();
						this.expectedUsers.Clear();
					}
				}
			}
		}

		// Token: 0x060054FB RID: 21755 RVA: 0x0019DF1F File Offset: 0x0019C11F
		public override void Stop()
		{
			base.Stop();
			this.expectedUsers.Clear();
		}

		// Token: 0x0400584A RID: 22602
		public float decayTime = 15f;

		// Token: 0x0400584B RID: 22603
		private Dictionary<string, float> expectedUsers = new Dictionary<string, float>(10);
	}
}
