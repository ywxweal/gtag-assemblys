using System;

namespace Viveport
{
	// Token: 0x02000A4A RID: 2634
	public class Leaderboard
	{
		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06003E99 RID: 16025 RVA: 0x001283AB File Offset: 0x001265AB
		// (set) Token: 0x06003E9A RID: 16026 RVA: 0x001283B3 File Offset: 0x001265B3
		public int Rank { get; set; }

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06003E9B RID: 16027 RVA: 0x001283BC File Offset: 0x001265BC
		// (set) Token: 0x06003E9C RID: 16028 RVA: 0x001283C4 File Offset: 0x001265C4
		public int Score { get; set; }

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06003E9D RID: 16029 RVA: 0x001283CD File Offset: 0x001265CD
		// (set) Token: 0x06003E9E RID: 16030 RVA: 0x001283D5 File Offset: 0x001265D5
		public string UserName { get; set; }
	}
}
