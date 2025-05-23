using System;
using System.Collections.Generic;

namespace Viveport
{
	// Token: 0x02000A4B RID: 2635
	public class SubscriptionStatus
	{
		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06003EA0 RID: 16032 RVA: 0x001283DE File Offset: 0x001265DE
		// (set) Token: 0x06003EA1 RID: 16033 RVA: 0x001283E6 File Offset: 0x001265E6
		public List<SubscriptionStatus.Platform> Platforms { get; set; }

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06003EA2 RID: 16034 RVA: 0x001283EF File Offset: 0x001265EF
		// (set) Token: 0x06003EA3 RID: 16035 RVA: 0x001283F7 File Offset: 0x001265F7
		public SubscriptionStatus.TransactionType Type { get; set; }

		// Token: 0x06003EA4 RID: 16036 RVA: 0x00128400 File Offset: 0x00126600
		public SubscriptionStatus()
		{
			this.Platforms = new List<SubscriptionStatus.Platform>();
			this.Type = SubscriptionStatus.TransactionType.Unknown;
		}

		// Token: 0x02000A4C RID: 2636
		public enum Platform
		{
			// Token: 0x04004313 RID: 17171
			Windows,
			// Token: 0x04004314 RID: 17172
			Android
		}

		// Token: 0x02000A4D RID: 2637
		public enum TransactionType
		{
			// Token: 0x04004316 RID: 17174
			Unknown,
			// Token: 0x04004317 RID: 17175
			Paid,
			// Token: 0x04004318 RID: 17176
			Redeem,
			// Token: 0x04004319 RID: 17177
			FreeTrial
		}
	}
}
