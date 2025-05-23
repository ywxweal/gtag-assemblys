using System;
using UnityEngine;

// Token: 0x020004D2 RID: 1234
public class BuilderPieceInteractorFindNearby : MonoBehaviour
{
	// Token: 0x06001DEB RID: 7659 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Awake()
	{
	}

	// Token: 0x06001DEC RID: 7660 RVA: 0x00091A32 File Offset: 0x0008FC32
	private void LateUpdate()
	{
		if (this.pieceInteractor != null)
		{
			this.pieceInteractor.StartFindNearbyPieces();
		}
	}

	// Token: 0x04002121 RID: 8481
	public BuilderPieceInteractor pieceInteractor;
}
