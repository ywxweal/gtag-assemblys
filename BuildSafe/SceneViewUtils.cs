using System;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02000BB9 RID: 3001
	public static class SceneViewUtils
	{
		// Token: 0x06004A47 RID: 19015 RVA: 0x00161D69 File Offset: 0x0015FF69
		private static bool RaycastWorldSafe(Vector2 screenPos, out RaycastHit hit)
		{
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x04004D15 RID: 19733
		public static readonly SceneViewUtils.FuncRaycastWorld RaycastWorld = new SceneViewUtils.FuncRaycastWorld(SceneViewUtils.RaycastWorldSafe);

		// Token: 0x02000BBA RID: 3002
		// (Invoke) Token: 0x06004A4A RID: 19018
		public delegate bool FuncRaycastWorld(Vector2 screenPos, out RaycastHit hit);

		// Token: 0x02000BBB RID: 3003
		// (Invoke) Token: 0x06004A4E RID: 19022
		public delegate GameObject FuncPickClosestGameObject(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);
	}
}
