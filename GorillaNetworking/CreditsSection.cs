using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02000C29 RID: 3113
	[Serializable]
	internal class CreditsSection
	{
		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x06004D17 RID: 19735 RVA: 0x0016F12B File Offset: 0x0016D32B
		// (set) Token: 0x06004D18 RID: 19736 RVA: 0x0016F133 File Offset: 0x0016D333
		public string Title { get; set; }

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x06004D19 RID: 19737 RVA: 0x0016F13C File Offset: 0x0016D33C
		// (set) Token: 0x06004D1A RID: 19738 RVA: 0x0016F144 File Offset: 0x0016D344
		public List<string> Entries { get; set; }
	}
}
