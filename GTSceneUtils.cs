using System;
using System.Diagnostics;
using UnityEngine.SceneManagement;

// Token: 0x020009E9 RID: 2537
public static class GTSceneUtils
{
	// Token: 0x06003CAB RID: 15531 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("UNITY_EDITOR")]
	public static void AddToBuild(GTScene scene)
	{
	}

	// Token: 0x06003CAC RID: 15532 RVA: 0x001212DD File Offset: 0x0011F4DD
	public static bool Equals(GTScene x, Scene y)
	{
		return !(x == null) && y.IsValid() && x.Equals(y);
	}

	// Token: 0x06003CAD RID: 15533 RVA: 0x00121301 File Offset: 0x0011F501
	public static GTScene[] ScenesInBuild()
	{
		return Array.Empty<GTScene>();
	}

	// Token: 0x06003CAE RID: 15534 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("UNITY_EDITOR")]
	public static void SyncBuildScenes()
	{
	}
}
