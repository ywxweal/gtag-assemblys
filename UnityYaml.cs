using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x020009EF RID: 2543
public static class UnityYaml
{
	// Token: 0x04004092 RID: 16530
	private static readonly Assembly EngineAssembly = Assembly.GetAssembly(typeof(MonoBehaviour));

	// Token: 0x04004093 RID: 16531
	private static readonly Assembly TerrainAssembly = Assembly.GetAssembly(typeof(Tree));

	// Token: 0x04004094 RID: 16532
	public static Dictionary<int, Type> ClassIDToType = new Dictionary<int, Type>();
}
