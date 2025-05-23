using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AE3 RID: 2787
	public struct BuilderPotentialPlacement
	{
		// Token: 0x06004383 RID: 17283 RVA: 0x001389D8 File Offset: 0x00136BD8
		public void Reset()
		{
			this.attachPiece = null;
			this.parentPiece = null;
			this.attachIndex = -1;
			this.parentAttachIndex = -1;
			this.localPosition = Vector3.zero;
			this.localRotation = Quaternion.identity;
			this.attachDistance = float.MaxValue;
			this.attachPlaneNormal = Vector3.zero;
			this.score = float.MinValue;
			this.twist = 0;
			this.bumpOffsetX = 0;
			this.bumpOffsetZ = 0;
		}

		// Token: 0x04004610 RID: 17936
		public BuilderPiece attachPiece;

		// Token: 0x04004611 RID: 17937
		public BuilderPiece parentPiece;

		// Token: 0x04004612 RID: 17938
		public int attachIndex;

		// Token: 0x04004613 RID: 17939
		public int parentAttachIndex;

		// Token: 0x04004614 RID: 17940
		public Vector3 localPosition;

		// Token: 0x04004615 RID: 17941
		public Quaternion localRotation;

		// Token: 0x04004616 RID: 17942
		public Vector3 attachPlaneNormal;

		// Token: 0x04004617 RID: 17943
		public float attachDistance;

		// Token: 0x04004618 RID: 17944
		public float score;

		// Token: 0x04004619 RID: 17945
		public SnapBounds attachBounds;

		// Token: 0x0400461A RID: 17946
		public SnapBounds parentAttachBounds;

		// Token: 0x0400461B RID: 17947
		public byte twist;

		// Token: 0x0400461C RID: 17948
		public sbyte bumpOffsetX;

		// Token: 0x0400461D RID: 17949
		public sbyte bumpOffsetZ;
	}
}
