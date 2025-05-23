using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AF3 RID: 2803
	public struct BuilderPotentialPlacementData
	{
		// Token: 0x06004472 RID: 17522 RVA: 0x00142960 File Offset: 0x00140B60
		public BuilderPotentialPlacement ToPotentialPlacement(BuilderTable table)
		{
			BuilderPotentialPlacement builderPotentialPlacement = new BuilderPotentialPlacement
			{
				attachPiece = table.GetPiece(this.pieceId),
				parentPiece = table.GetPiece(this.parentPieceId),
				score = this.score,
				localPosition = this.localPosition,
				localRotation = this.localRotation,
				attachIndex = this.attachIndex,
				parentAttachIndex = this.parentAttachIndex,
				attachDistance = this.attachDistance,
				attachPlaneNormal = this.attachPlaneNormal,
				attachBounds = this.attachBounds,
				parentAttachBounds = this.parentAttachBounds,
				twist = this.twist,
				bumpOffsetX = this.bumpOffsetX,
				bumpOffsetZ = this.bumpOffsetZ
			};
			if (builderPotentialPlacement.parentPiece != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
				builderPotentialPlacement.localPosition = builderAttachGridPlane.transform.InverseTransformPoint(builderPotentialPlacement.localPosition);
				builderPotentialPlacement.localRotation = Quaternion.Inverse(builderAttachGridPlane.transform.rotation) * builderPotentialPlacement.localRotation;
			}
			return builderPotentialPlacement;
		}

		// Token: 0x04004724 RID: 18212
		public int pieceId;

		// Token: 0x04004725 RID: 18213
		public int parentPieceId;

		// Token: 0x04004726 RID: 18214
		public float score;

		// Token: 0x04004727 RID: 18215
		public Vector3 localPosition;

		// Token: 0x04004728 RID: 18216
		public Quaternion localRotation;

		// Token: 0x04004729 RID: 18217
		public int attachIndex;

		// Token: 0x0400472A RID: 18218
		public int parentAttachIndex;

		// Token: 0x0400472B RID: 18219
		public float attachDistance;

		// Token: 0x0400472C RID: 18220
		public Vector3 attachPlaneNormal;

		// Token: 0x0400472D RID: 18221
		public SnapBounds attachBounds;

		// Token: 0x0400472E RID: 18222
		public SnapBounds parentAttachBounds;

		// Token: 0x0400472F RID: 18223
		public byte twist;

		// Token: 0x04004730 RID: 18224
		public sbyte bumpOffsetX;

		// Token: 0x04004731 RID: 18225
		public sbyte bumpOffsetZ;
	}
}
