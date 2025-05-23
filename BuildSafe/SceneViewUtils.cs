using System;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02000BB9 RID: 3001
	public static class SceneViewUtils
	{
		// Token: 0x06004A48 RID: 19016 RVA: 0x00161E41 File Offset: 0x00160041
		private static bool RaycastWorldSafe(Vector2 screenPos, out RaycastHit hit)
		{
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x04004D16 RID: 19734
		public static readonly SceneViewUtils.FuncRaycastWorld RaycastWorld = new SceneViewUtils.FuncRaycastWorld(SceneViewUtils.RaycastWorldSafe);

		// Token: 0x02000BBA RID: 3002
		// (Invoke) Token: 0x06004A4B RID: 19019
		public delegate bool FuncRaycastWorld(Vector2 screenPos, out RaycastHit hit);

		// Token: 0x02000BBB RID: 3003
		// (Invoke) Token: 0x06004A4F RID: 19023
		public delegate GameObject FuncPickClosestGameObject(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, out int materialIndex);
	}
}
