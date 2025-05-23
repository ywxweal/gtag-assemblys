using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E33 RID: 3635
	[ExecuteInEditMode]
	public class DrawBox : DrawBase
	{
		// Token: 0x06005AD8 RID: 23256 RVA: 0x001BA138 File Offset: 0x001B8338
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06005AD9 RID: 23257 RVA: 0x001BA164 File Offset: 0x001B8364
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion quaternion = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * quaternion * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04005EE2 RID: 24290
		public float Radius = 1f;

		// Token: 0x04005EE3 RID: 24291
		public int NumSegments = 64;

		// Token: 0x04005EE4 RID: 24292
		public float StartAngle;

		// Token: 0x04005EE5 RID: 24293
		public float ArcAngle = 60f;
	}
}
