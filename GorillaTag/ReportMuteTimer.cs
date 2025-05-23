using System;
using Photon.Realtime;

namespace GorillaTag
{
	// Token: 0x02000D3A RID: 3386
	internal class ReportMuteTimer : TickSystemTimerAbstract, ObjectPoolEvents
	{
		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x060054D6 RID: 21718 RVA: 0x0019DC1D File Offset: 0x0019BE1D
		// (set) Token: 0x060054D7 RID: 21719 RVA: 0x0019DC25 File Offset: 0x0019BE25
		public int Muted { get; set; }

		// Token: 0x060054D8 RID: 21720 RVA: 0x0019DC30 File Offset: 0x0019BE30
		public override void OnTimedEvent()
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				this.Stop();
				return;
			}
			ReportMuteTimer.content[0] = this.m_playerID;
			ReportMuteTimer.content[1] = this.Muted;
			ReportMuteTimer.content[2] = ((this.m_nickName.Length > 12) ? this.m_nickName.Remove(12) : this.m_nickName);
			ReportMuteTimer.content[3] = NetworkSystem.Instance.LocalPlayer.NickName;
			ReportMuteTimer.content[4] = !NetworkSystem.Instance.SessionIsPrivate;
			ReportMuteTimer.content[5] = NetworkSystem.Instance.RoomStringStripped();
			NetworkSystemRaiseEvent.RaiseEvent(51, ReportMuteTimer.content, ReportMuteTimer.netEventOptions, true);
			this.Stop();
		}

		// Token: 0x060054D9 RID: 21721 RVA: 0x0019DCF2 File Offset: 0x0019BEF2
		public void SetReportData(string id, string name, int muted)
		{
			this.Muted = muted;
			this.m_playerID = id;
			this.m_nickName = name;
		}

		// Token: 0x060054DA RID: 21722 RVA: 0x000023F4 File Offset: 0x000005F4
		void ObjectPoolEvents.OnTaken()
		{
		}

		// Token: 0x060054DB RID: 21723 RVA: 0x0019DD09 File Offset: 0x0019BF09
		void ObjectPoolEvents.OnReturned()
		{
			if (base.Running)
			{
				this.OnTimedEvent();
			}
			this.m_playerID = string.Empty;
			this.m_nickName = string.Empty;
			this.Muted = 0;
		}

		// Token: 0x0400583E RID: 22590
		private static readonly NetEventOptions netEventOptions = new NetEventOptions
		{
			Flags = new WebFlags(1),
			TargetActors = new int[] { -1 }
		};

		// Token: 0x0400583F RID: 22591
		private static readonly object[] content = new object[6];

		// Token: 0x04005840 RID: 22592
		private const byte evCode = 51;

		// Token: 0x04005842 RID: 22594
		private string m_playerID;

		// Token: 0x04005843 RID: 22595
		private string m_nickName;
	}
}
