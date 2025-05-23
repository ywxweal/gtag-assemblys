using System;
using System.Collections.Generic;

namespace GorillaTagScripts
{
	// Token: 0x02000AFB RID: 2811
	[Serializable]
	public class BuilderTableData
	{
		// Token: 0x060044CB RID: 17611 RVA: 0x00145EF8 File Offset: 0x001440F8
		public BuilderTableData()
		{
			this.version = 4;
			this.numEdits = 0;
			this.numPieces = 0;
			this.pieceType = new List<int>(1024);
			this.pieceId = new List<int>(1024);
			this.parentId = new List<int>(1024);
			this.attachIndex = new List<int>(1024);
			this.parentAttachIndex = new List<int>(1024);
			this.placement = new List<int>(1024);
			this.materialType = new List<int>(1024);
			this.overlapingPieces = new List<int>(1024);
			this.overlappedPieces = new List<int>(1024);
			this.overlapInfo = new List<long>(1024);
			this.timeOffset = new List<int>(1024);
		}

		// Token: 0x060044CC RID: 17612 RVA: 0x00145FD0 File Offset: 0x001441D0
		public void Clear()
		{
			this.numPieces = 0;
			this.pieceType.Clear();
			this.pieceId.Clear();
			this.parentId.Clear();
			this.attachIndex.Clear();
			this.parentAttachIndex.Clear();
			this.placement.Clear();
			this.materialType.Clear();
			this.overlapingPieces.Clear();
			this.overlappedPieces.Clear();
			this.overlapInfo.Clear();
			this.timeOffset.Clear();
		}

		// Token: 0x0400477E RID: 18302
		public const int BUILDER_TABLE_DATA_VERSION = 4;

		// Token: 0x0400477F RID: 18303
		public int version;

		// Token: 0x04004780 RID: 18304
		public int numEdits;

		// Token: 0x04004781 RID: 18305
		public int numPieces;

		// Token: 0x04004782 RID: 18306
		public List<int> pieceType;

		// Token: 0x04004783 RID: 18307
		public List<int> pieceId;

		// Token: 0x04004784 RID: 18308
		public List<int> parentId;

		// Token: 0x04004785 RID: 18309
		public List<int> attachIndex;

		// Token: 0x04004786 RID: 18310
		public List<int> parentAttachIndex;

		// Token: 0x04004787 RID: 18311
		public List<int> placement;

		// Token: 0x04004788 RID: 18312
		public List<int> materialType;

		// Token: 0x04004789 RID: 18313
		public List<int> overlapingPieces;

		// Token: 0x0400478A RID: 18314
		public List<int> overlappedPieces;

		// Token: 0x0400478B RID: 18315
		public List<long> overlapInfo;

		// Token: 0x0400478C RID: 18316
		public List<int> timeOffset;
	}
}
