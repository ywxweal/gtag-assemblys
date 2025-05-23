using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E36 RID: 3638
	[ExecuteInEditMode]
	public class DrawSphere : DrawBase
	{
		// Token: 0x06005AE2 RID: 23266 RVA: 0x001BA3B3 File Offset: 0x001B85B3
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06005AE3 RID: 23267 RVA: 0x001BA3E0 File Offset: 0x001B85E0
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion quaternion = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * quaternion * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04005EEA RID: 24298
		public float Radius = 1f;

		// Token: 0x04005EEB RID: 24299
		public int NumSegments = 64;

		// Token: 0x04005EEC RID: 24300
		public float StartAngle;

		// Token: 0x04005EED RID: 24301
		public float ArcAngle = 60f;
	}
}
