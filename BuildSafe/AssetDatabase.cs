using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02000BA8 RID: 2984
	public static class AssetDatabase
	{
		// Token: 0x06004A0D RID: 18957 RVA: 0x00161ABC File Offset: 0x0015FCBC
		public static T LoadAssetAtPath<T>(string assetPath) where T : Object
		{
			return default(T);
		}

		// Token: 0x06004A0E RID: 18958 RVA: 0x00161AD2 File Offset: 0x0015FCD2
		public static T[] LoadAssetsOfType<T>() where T : Object
		{
			return Array.Empty<T>();
		}

		// Token: 0x06004A0F RID: 18959 RVA: 0x00161AD9 File Offset: 0x0015FCD9
		public static string[] FindAssetsOfType<T>() where T : Object
		{
			return Array.Empty<string>();
		}

		// Token: 0x06004A10 RID: 18960 RVA: 0x00161AE0 File Offset: 0x0015FCE0
		[Conditional("UNITY_EDITOR")]
		public static void SaveToDisk(params Object[] assetsToSave)
		{
			AssetDatabase.SaveAssetsToDisk(assetsToSave, true);
		}

		// Token: 0x06004A11 RID: 18961 RVA: 0x000023F4 File Offset: 0x000005F4
		public static void SaveAssetsToDisk(Object[] assetsToSave, bool saveProject = true)
		{
		}
	}
}
