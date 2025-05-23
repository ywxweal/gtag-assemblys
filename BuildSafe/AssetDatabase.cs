using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02000BA8 RID: 2984
	public static class AssetDatabase
	{
		// Token: 0x06004A0C RID: 18956 RVA: 0x001619E4 File Offset: 0x0015FBE4
		public static T LoadAssetAtPath<T>(string assetPath) where T : Object
		{
			return default(T);
		}

		// Token: 0x06004A0D RID: 18957 RVA: 0x001619FA File Offset: 0x0015FBFA
		public static T[] LoadAssetsOfType<T>() where T : Object
		{
			return Array.Empty<T>();
		}

		// Token: 0x06004A0E RID: 18958 RVA: 0x00161A01 File Offset: 0x0015FC01
		public static string[] FindAssetsOfType<T>() where T : Object
		{
			return Array.Empty<string>();
		}

		// Token: 0x06004A0F RID: 18959 RVA: 0x00161A08 File Offset: 0x0015FC08
		[Conditional("UNITY_EDITOR")]
		public static void SaveToDisk(params Object[] assetsToSave)
		{
			AssetDatabase.SaveAssetsToDisk(assetsToSave, true);
		}

		// Token: 0x06004A10 RID: 18960 RVA: 0x000023F4 File Offset: 0x000005F4
		public static void SaveAssetsToDisk(Object[] assetsToSave, bool saveProject = true)
		{
		}
	}
}
