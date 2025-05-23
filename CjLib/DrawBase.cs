using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000E32 RID: 3634
	public abstract class DrawBase : MonoBehaviour
	{
		// Token: 0x06005AD5 RID: 23253 RVA: 0x001BA0BC File Offset: 0x001B82BC
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

		// Token: 0x06005AD6 RID: 23254
		protected abstract void Draw(Color color, DebugUtil.Style style, bool depthTest);

		// Token: 0x04005EDD RID: 24285
		public Color WireframeColor = Color.white;

		// Token: 0x04005EDE RID: 24286
		public Color ShadededColor = Color.gray;

		// Token: 0x04005EDF RID: 24287
		public bool Wireframe;

		// Token: 0x04005EE0 RID: 24288
		public DebugUtil.Style Style = DebugUtil.Style.FlatShaded;

		// Token: 0x04005EE1 RID: 24289
		public bool DepthTest = true;
	}
}
