using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AEF RID: 2799
	public struct BuilderGridPlaneData
	{
		// Token: 0x06004470 RID: 17520 RVA: 0x001428CC File Offset: 0x00140ACC
		public BuilderGridPlaneData(BuilderAttachGridPlane gridPlane, int pieceIndex)
		{
			gridPlane.center.transform.GetPositionAndRotation(out this.position, out this.rotation);
			this.localPosition = gridPlane.pieceToGridPosition;
			this.localRotation = gridPlane.pieceToGridRotation;
			this.width = gridPlane.width;
			this.length = gridPlane.length;
			this.male = gridPlane.male;
			this.pieceId = gridPlane.piece.pieceId;
			this.pieceIndex = pieceIndex;
			this.boundingRadius = gridPlane.boundingRadius;
			this.attachIndex = gridPlane.attachIndex;
		}

		// Token: 0x0400470A RID: 18186
		public int width;

		// Token: 0x0400470B RID: 18187
		public int length;

		// Token: 0x0400470C RID: 18188
		public bool male;

		// Token: 0x0400470D RID: 18189
		public int pieceId;

		// Token: 0x0400470E RID: 18190
		public int pieceIndex;

		// Token: 0x0400470F RID: 18191
		public float boundingRadius;

		// Token: 0x04004710 RID: 18192
		public int attachIndex;

		// Token: 0x04004711 RID: 18193
		public Vector3 position;

		// Token: 0x04004712 RID: 18194
		public Quaternion rotation;

		// Token: 0x04004713 RID: 18195
		public Vector3 localPosition;

		// Token: 0x04004714 RID: 18196
		public Quaternion localRotation;
	}
}
