using System;

namespace GorillaTagScripts
{
	// Token: 0x02000AE2 RID: 2786
	[Serializable]
	public class BuilderTableConfiguration
	{
		// Token: 0x06004381 RID: 17281 RVA: 0x001388CC File Offset: 0x00136ACC
		public BuilderTableConfiguration()
		{
			this.version = 0;
			this.TableResourceLimits = new int[3];
			this.PlotResourceLimits = new int[3];
			this.updateCountdownDate = string.Empty;
		}

		// Token: 0x04004609 RID: 17929
		public const int CONFIGURATION_VERSION = 0;

		// Token: 0x0400460A RID: 17930
		public int version;

		// Token: 0x0400460B RID: 17931
		public int[] TableResourceLimits;

		// Token: 0x0400460C RID: 17932
		public int[] PlotResourceLimits;

		// Token: 0x0400460D RID: 17933
		public int DroppedPieceLimit;

		// Token: 0x0400460E RID: 17934
		public string updateCountdownDate;
	}
}
