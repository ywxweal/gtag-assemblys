using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200096C RID: 2412
public static class AssetUtils
{
	// Token: 0x06003A2C RID: 14892 RVA: 0x000023F4 File Offset: 0x000005F4
	[Conditional("UNITY_EDITOR")]
	public static void ExecAndUnloadUnused(Action action)
	{
	}

	// Token: 0x06003A2D RID: 14893 RVA: 0x00116ED2 File Offset: 0x001150D2
	[Conditional("UNITY_EDITOR")]
	public static void LoadAssetOfType<T>(ref T result, ref string resultPath) where T : Object
	{
		result = default(T);
		resultPath = null;
	}

	// Token: 0x06003A2E RID: 14894 RVA: 0x00116EDE File Offset: 0x001150DE
	[Conditional("UNITY_EDITOR")]
	public static void FindAllAssetsOfType<T>(ref T[] results, ref string[] assetPaths) where T : Object
	{
		results = Array.Empty<T>();
	}

	// Token: 0x06003A2F RID: 14895 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void ForceSave<T>(this IList<T> assets, Action<T> onPreSave = null, bool unloadUnusedAfter = false) where T : Object
	{
	}

	// Token: 0x06003A30 RID: 14896 RVA: 0x000023F4 File Offset: 0x000005F4
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void ForceSave(this Object asset)
	{
	}

	// Token: 0x06003A31 RID: 14897 RVA: 0x00116EE7 File Offset: 0x001150E7
	public static long ComputeAssetId(this Object asset, bool unsigned = false)
	{
		return 0L;
	}
}
