using System;

namespace GorillaTagScripts
{
	// Token: 0x02000AF2 RID: 2802
	public struct BuilderPrivatePlotData
	{
		// Token: 0x06004472 RID: 17522 RVA: 0x00142A0F File Offset: 0x00140C0F
		public BuilderPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			this.plotState = plot.plotState;
			this.ownerActorNumber = plot.GetOwnerActorNumber();
			this.isUnderCapacityLeft = false;
			this.isUnderCapacityRight = false;
		}

		// Token: 0x04004721 RID: 18209
		public BuilderPiecePrivatePlot.PlotState plotState;

		// Token: 0x04004722 RID: 18210
		public int ownerActorNumber;

		// Token: 0x04004723 RID: 18211
		public bool isUnderCapacityLeft;

		// Token: 0x04004724 RID: 18212
		public bool isUnderCapacityRight;
	}
}
