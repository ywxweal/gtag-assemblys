using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E30 RID: 3632
	[ExecuteInEditMode]
	public class DrawArc : DrawBase
	{
		// Token: 0x06005AD0 RID: 23248 RVA: 0x001B9FBF File Offset: 0x001B81BF
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06005AD1 RID: 23249 RVA: 0x001B9FF8 File Offset: 0x001B81F8
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion quaternion = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * quaternion * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04005ED5 RID: 24277
		public float Radius = 1f;

		// Token: 0x04005ED6 RID: 24278
		public int NumSegments = 64;

		// Token: 0x04005ED7 RID: 24279
		public float StartAngle;

		// Token: 0x04005ED8 RID: 24280
		public float ArcAngle = 60f;
	}
}
