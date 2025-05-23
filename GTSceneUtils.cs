using System;
using System.Diagnostics;
using UnityEngine.SceneManagement;

// Token: 0x020009E9 RID: 2537
public static class GTSceneUtils
{
	// Token: 0x06003CAC RID: 15532 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("UNITY_EDITOR")]
	public static void AddToBuild(GTScene scene)
	{
	}

	// Token: 0x06003CAD RID: 15533 RVA: 0x001213B5 File Offset: 0x0011F5B5
	public static bool Equals(GTScene x, Scene y)
	{
		return !(x == null) && y.IsValid() && x.Equals(y);
	}

	// Token: 0x06003CAE RID: 15534 RVA: 0x001213D9 File Offset: 0x0011F5D9
	public static GTScene[] ScenesInBuild()
	{
		return Array.Empty<GTScene>();
	}

	// Token: 0x06003CAF RID: 15535 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("UNITY_EDITOR")]
	public static void SyncBuildScenes()
	{
	}
}
