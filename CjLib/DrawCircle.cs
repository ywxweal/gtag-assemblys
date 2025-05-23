using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E34 RID: 3636
	[ExecuteInEditMode]
	public class DrawCircle : DrawBase
	{
		// Token: 0x06005ADB RID: 23259 RVA: 0x001BA208 File Offset: 0x001B8408
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06005ADC RID: 23260 RVA: 0x001BA232 File Offset: 0x001B8432
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawCircle(base.transform.position, base.transform.rotation * Vector3.back, this.Radius, this.NumSegments, color, depthTest, style);
		}

		// Token: 0x04005EE6 RID: 24294
		public float Radius = 1f;

		// Token: 0x04005EE7 RID: 24295
		public int NumSegments = 64;
	}
}
