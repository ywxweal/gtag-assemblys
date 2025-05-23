using System;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicSceneManagerHelper
{
	// Token: 0x02000BBD RID: 3005
	internal class SceneSnapshot
	{
		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x06004A55 RID: 19029 RVA: 0x00161D92 File Offset: 0x0015FF92
		public Dictionary<OVRAnchor, SceneSnapshot.Data> Anchors { get; } = new Dictionary<OVRAnchor, SceneSnapshot.Data>();

		// Token: 0x06004A56 RID: 19030 RVA: 0x00161D9A File Offset: 0x0015FF9A
		public bool Contains(OVRAnchor anchor)
		{
			return this.Anchors.ContainsKey(anchor);
		}

		// Token: 0x02000BBE RID: 3006
		public class Data
		{
			// Token: 0x04004D18 RID: 19736
			public List<OVRAnchor> Children;

			// Token: 0x04004D19 RID: 19737
			public Rect? Rect;

			// Token: 0x04004D1A RID: 19738
			public Bounds? Bounds;
		}
	}
}
