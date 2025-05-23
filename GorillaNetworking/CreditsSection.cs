using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02000C29 RID: 3113
	[Serializable]
	internal class CreditsSection
	{
		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x06004D16 RID: 19734 RVA: 0x0016F053 File Offset: 0x0016D253
		// (set) Token: 0x06004D17 RID: 19735 RVA: 0x0016F05B File Offset: 0x0016D25B
		public string Title { get; set; }

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x06004D18 RID: 19736 RVA: 0x0016F064 File Offset: 0x0016D264
		// (set) Token: 0x06004D19 RID: 19737 RVA: 0x0016F06C File Offset: 0x0016D26C
		public List<string> Entries { get; set; }
	}
}
