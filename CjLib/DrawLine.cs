using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E35 RID: 3637
	[ExecuteInEditMode]
	public class DrawLine : DrawBase
	{
		// Token: 0x06005ADF RID: 23263 RVA: 0x001BA35B File Offset: 0x001B855B
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
		}

		// Token: 0x06005AE0 RID: 23264 RVA: 0x001BA36B File Offset: 0x001B856B
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawLine(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), color, depthTest);
		}

		// Token: 0x04005EE9 RID: 24297
		public Vector3 LocalEndVector = Vector3.right;
	}
}
