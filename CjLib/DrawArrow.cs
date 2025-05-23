using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E31 RID: 3633
	[ExecuteInEditMode]
	public class DrawArrow : DrawBase
	{
		// Token: 0x06005AD2 RID: 23250 RVA: 0x001B9FC4 File Offset: 0x001B81C4
		private void OnValidate()
		{
			this.ConeRadius = Mathf.Max(0f, this.ConeRadius);
			this.ConeHeight = Mathf.Max(0f, this.ConeHeight);
			this.StemThickness = Mathf.Max(0f, this.StemThickness);
			this.NumSegments = Mathf.Max(4, this.NumSegments);
		}

		// Token: 0x06005AD3 RID: 23251 RVA: 0x001BA028 File Offset: 0x001B8228
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawArrow(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), this.ConeRadius, this.ConeHeight, this.NumSegments, this.StemThickness, color, depthTest, style);
		}

		// Token: 0x04005ED8 RID: 24280
		public Vector3 LocalEndVector = Vector3.right;

		// Token: 0x04005ED9 RID: 24281
		public float ConeRadius = 0.05f;

		// Token: 0x04005EDA RID: 24282
		public float ConeHeight = 0.1f;

		// Token: 0x04005EDB RID: 24283
		public float StemThickness = 0.05f;

		// Token: 0x04005EDC RID: 24284
		public int NumSegments = 8;
	}
}
