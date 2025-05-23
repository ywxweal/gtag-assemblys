using System;

namespace GorillaTagScripts
{
	// Token: 0x02000AF2 RID: 2802
	public struct BuilderPrivatePlotData
	{
		// Token: 0x06004471 RID: 17521 RVA: 0x00142937 File Offset: 0x00140B37
		public BuilderPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			this.plotState = plot.plotState;
			this.ownerActorNumber = plot.GetOwnerActorNumber();
			this.isUnderCapacityLeft = false;
			this.isUnderCapacityRight = false;
		}

		// Token: 0x04004720 RID: 18208
		public BuilderPiecePrivatePlot.PlotState plotState;

		// Token: 0x04004721 RID: 18209
		public int ownerActorNumber;

		// Token: 0x04004722 RID: 18210
		public bool isUnderCapacityLeft;

		// Token: 0x04004723 RID: 18211
		public bool isUnderCapacityRight;
	}
}
