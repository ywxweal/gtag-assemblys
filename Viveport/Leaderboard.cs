using System;

namespace Viveport
{
	// Token: 0x02000A4A RID: 2634
	public class Leaderboard
	{
		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06003E9A RID: 16026 RVA: 0x00128483 File Offset: 0x00126683
		// (set) Token: 0x06003E9B RID: 16027 RVA: 0x0012848B File Offset: 0x0012668B
		public int Rank { get; set; }

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06003E9C RID: 16028 RVA: 0x00128494 File Offset: 0x00126694
		// (set) Token: 0x06003E9D RID: 16029 RVA: 0x0012849C File Offset: 0x0012669C
		public int Score { get; set; }

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06003E9E RID: 16030 RVA: 0x001284A5 File Offset: 0x001266A5
		// (set) Token: 0x06003E9F RID: 16031 RVA: 0x001284AD File Offset: 0x001266AD
		public string UserName { get; set; }
	}
}
