using System;
using UnityEngine;

// Token: 0x020004DD RID: 1245
public struct BuilderAction
{
	// Token: 0x0400214F RID: 8527
	public BuilderActionType type;

	// Token: 0x04002150 RID: 8528
	public int pieceId;

	// Token: 0x04002151 RID: 8529
	public int parentPieceId;

	// Token: 0x04002152 RID: 8530
	public Vector3 localPosition;

	// Token: 0x04002153 RID: 8531
	public Quaternion localRotation;

	// Token: 0x04002154 RID: 8532
	public byte twist;

	// Token: 0x04002155 RID: 8533
	public sbyte bumpOffsetx;

	// Token: 0x04002156 RID: 8534
	public sbyte bumpOffsetz;

	// Token: 0x04002157 RID: 8535
	public bool isLeftHand;

	// Token: 0x04002158 RID: 8536
	public int playerActorNumber;

	// Token: 0x04002159 RID: 8537
	public int parentAttachIndex;

	// Token: 0x0400215A RID: 8538
	public int attachIndex;

	// Token: 0x0400215B RID: 8539
	public SnapBounds attachBounds;

	// Token: 0x0400215C RID: 8540
	public SnapBounds parentAttachBounds;

	// Token: 0x0400215D RID: 8541
	public Vector3 velocity;

	// Token: 0x0400215E RID: 8542
	public Vector3 angVelocity;

	// Token: 0x0400215F RID: 8543
	public int localCommandId;

	// Token: 0x04002160 RID: 8544
	public int timeStamp;
}
