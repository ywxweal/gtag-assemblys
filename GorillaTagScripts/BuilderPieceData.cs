﻿using System;

namespace GorillaTagScripts
{
	// Token: 0x02000AF0 RID: 2800
	public struct BuilderPieceData
	{
		// Token: 0x06004471 RID: 17521 RVA: 0x00142964 File Offset: 0x00140B64
		public BuilderPieceData(BuilderPiece piece)
		{
			this.pieceId = piece.pieceId;
			this.pieceIndex = piece.pieceDataIndex;
			BuilderPiece parentPiece = piece.parentPiece;
			this.parentPieceIndex = ((parentPiece == null) ? (-1) : parentPiece.pieceDataIndex);
			BuilderPiece requestedParentPiece = piece.requestedParentPiece;
			this.requestedParentPieceIndex = ((requestedParentPiece == null) ? (-1) : requestedParentPiece.pieceDataIndex);
			this.preventSnapUntilMoved = piece.preventSnapUntilMoved;
			this.isBuiltIntoTable = piece.isBuiltIntoTable;
			this.state = piece.state;
			this.privatePlotIndex = piece.privatePlotIndex;
			this.isArmPiece = piece.isArmShelf;
			this.heldByActorNumber = piece.heldByPlayerActorNumber;
		}

		// Token: 0x04004715 RID: 18197
		public int pieceId;

		// Token: 0x04004716 RID: 18198
		public int pieceIndex;

		// Token: 0x04004717 RID: 18199
		public int parentPieceIndex;

		// Token: 0x04004718 RID: 18200
		public int requestedParentPieceIndex;

		// Token: 0x04004719 RID: 18201
		public int heldByActorNumber;

		// Token: 0x0400471A RID: 18202
		public int preventSnapUntilMoved;

		// Token: 0x0400471B RID: 18203
		public bool isBuiltIntoTable;

		// Token: 0x0400471C RID: 18204
		public BuilderPiece.State state;

		// Token: 0x0400471D RID: 18205
		public int privatePlotIndex;

		// Token: 0x0400471E RID: 18206
		public bool isArmPiece;
	}
}
