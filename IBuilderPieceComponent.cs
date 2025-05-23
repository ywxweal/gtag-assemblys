using System;

// Token: 0x020004EF RID: 1263
public interface IBuilderPieceComponent
{
	// Token: 0x06001E93 RID: 7827
	void OnPieceCreate(int pieceType, int pieceId);

	// Token: 0x06001E94 RID: 7828
	void OnPieceDestroy();

	// Token: 0x06001E95 RID: 7829
	void OnPiecePlacementDeserialized();

	// Token: 0x06001E96 RID: 7830
	void OnPieceActivate();

	// Token: 0x06001E97 RID: 7831
	void OnPieceDeactivate();
}
