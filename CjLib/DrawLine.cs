using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E35 RID: 3637
	[ExecuteInEditMode]
	public class DrawLine : DrawBase
	{
		// Token: 0x06005ADE RID: 23262 RVA: 0x001BA283 File Offset: 0x001B8483
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
		}

		// Token: 0x06005ADF RID: 23263 RVA: 0x001BA293 File Offset: 0x001B8493
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawLine(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), color, depthTest);
		}

		// Token: 0x04005EE8 RID: 24296
		public Vector3 LocalEndVector = Vector3.right;
	}
}
