using System;
using System.Collections.Generic;
using GorillaTagScripts.Builder;
using UnityEngine;

// Token: 0x020004D4 RID: 1236
public class BuilderPieceScaleHandler : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x06001DF1 RID: 7665 RVA: 0x00091A30 File Offset: 0x0008FC30
	public void OnPieceCreate(int pieceType, int pieceId)
	{
		foreach (BuilderScaleAudioRadius builderScaleAudioRadius in this.audioScalers)
		{
			builderScaleAudioRadius.SetScale(this.myPiece.GetScale());
		}
		foreach (BuilderScaleParticles builderScaleParticles in this.particleScalers)
		{
			builderScaleParticles.SetScale(this.myPiece.GetScale());
		}
	}

	// Token: 0x06001DF2 RID: 7666 RVA: 0x00091AD8 File Offset: 0x0008FCD8
	public void OnPieceDestroy()
	{
		foreach (BuilderScaleAudioRadius builderScaleAudioRadius in this.audioScalers)
		{
			builderScaleAudioRadius.RevertScale();
		}
		foreach (BuilderScaleParticles builderScaleParticles in this.particleScalers)
		{
			builderScaleParticles.RevertScale();
		}
	}

	// Token: 0x06001DF3 RID: 7667 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x06001DF4 RID: 7668 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPieceActivate()
	{
	}

	// Token: 0x06001DF5 RID: 7669 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnPieceDeactivate()
	{
	}

	// Token: 0x04002123 RID: 8483
	[SerializeField]
	private BuilderPiece myPiece;

	// Token: 0x04002124 RID: 8484
	[SerializeField]
	private List<BuilderScaleAudioRadius> audioScalers = new List<BuilderScaleAudioRadius>();

	// Token: 0x04002125 RID: 8485
	[SerializeField]
	private List<BuilderScaleParticles> particleScalers = new List<BuilderScaleParticles>();
}
