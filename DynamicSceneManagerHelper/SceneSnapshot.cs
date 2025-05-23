using System;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicSceneManagerHelper
{
	// Token: 0x02000BBD RID: 3005
	internal class SceneSnapshot
	{
		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x06004A56 RID: 19030 RVA: 0x00161E6A File Offset: 0x0016006A
		public Dictionary<OVRAnchor, SceneSnapshot.Data> Anchors { get; } = new Dictionary<OVRAnchor, SceneSnapshot.Data>();

		// Token: 0x06004A57 RID: 19031 RVA: 0x00161E72 File Offset: 0x00160072
		public bool Contains(OVRAnchor anchor)
		{
			return this.Anchors.ContainsKey(anchor);
		}

		// Token: 0x02000BBE RID: 3006
		public class Data
		{
			// Token: 0x04004D19 RID: 19737
			public List<OVRAnchor> Children;

			// Token: 0x04004D1A RID: 19738
			public Rect? Rect;

			// Token: 0x04004D1B RID: 19739
			public Bounds? Bounds;
		}
	}
}
