using System;
using System.Collections.Generic;

namespace Viveport
{
	// Token: 0x02000A4B RID: 2635
	public class SubscriptionStatus
	{
		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06003EA1 RID: 16033 RVA: 0x001284B6 File Offset: 0x001266B6
		// (set) Token: 0x06003EA2 RID: 16034 RVA: 0x001284BE File Offset: 0x001266BE
		public List<SubscriptionStatus.Platform> Platforms { get; set; }

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06003EA3 RID: 16035 RVA: 0x001284C7 File Offset: 0x001266C7
		// (set) Token: 0x06003EA4 RID: 16036 RVA: 0x001284CF File Offset: 0x001266CF
		public SubscriptionStatus.TransactionType Type { get; set; }

		// Token: 0x06003EA5 RID: 16037 RVA: 0x001284D8 File Offset: 0x001266D8
		public SubscriptionStatus()
		{
			this.Platforms = new List<SubscriptionStatus.Platform>();
			this.Type = SubscriptionStatus.TransactionType.Unknown;
		}

		// Token: 0x02000A4C RID: 2636
		public enum Platform
		{
			// Token: 0x04004314 RID: 17172
			Windows,
			// Token: 0x04004315 RID: 17173
			Android
		}

		// Token: 0x02000A4D RID: 2637
		public enum TransactionType
		{
			// Token: 0x04004317 RID: 17175
			Unknown,
			// Token: 0x04004318 RID: 17176
			Paid,
			// Token: 0x04004319 RID: 17177
			Redeem,
			// Token: 0x0400431A RID: 17178
			FreeTrial
		}
	}
}
