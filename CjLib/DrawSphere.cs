using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E36 RID: 3638
	[ExecuteInEditMode]
	public class DrawSphere : DrawBase
	{
		// Token: 0x06005AE1 RID: 23265 RVA: 0x001BA2DB File Offset: 0x001B84DB
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06005AE2 RID: 23266 RVA: 0x001BA308 File Offset: 0x001B8508
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion quaternion = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * quaternion * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04005EE9 RID: 24297
		public float Radius = 1f;

		// Token: 0x04005EEA RID: 24298
		public int NumSegments = 64;

		// Token: 0x04005EEB RID: 24299
		public float StartAngle;

		// Token: 0x04005EEC RID: 24300
		public float ArcAngle = 60f;
	}
}
