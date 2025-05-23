using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E32 RID: 3634
	public abstract class DrawBase : MonoBehaviour
	{
		// Token: 0x06005AD6 RID: 23254 RVA: 0x001BA194 File Offset: 0x001B8394
		private void Update()
		{
			if (this.Style != DebugUtil.Style.Wireframe)
			{
				this.Draw(this.ShadededColor, this.Style, this.DepthTest);
			}
			if (this.Style == DebugUtil.Style.Wireframe || this.Wireframe)
			{
				this.Draw(this.WireframeColor, DebugUtil.Style.Wireframe, this.DepthTest);
			}
		}

		// Token: 0x06005AD7 RID: 23255
		protected abstract void Draw(Color color, DebugUtil.Style style, bool depthTest);

		// Token: 0x04005EDE RID: 24286
		public Color WireframeColor = Color.white;

		// Token: 0x04005EDF RID: 24287
		public Color ShadededColor = Color.gray;

		// Token: 0x04005EE0 RID: 24288
		public bool Wireframe;

		// Token: 0x04005EE1 RID: 24289
		public DebugUtil.Style Style = DebugUtil.Style.FlatShaded;

		// Token: 0x04005EE2 RID: 24290
		public bool DepthTest = true;
	}
}
