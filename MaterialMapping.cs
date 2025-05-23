using System;
using UnityEngine;

// Token: 0x02000A28 RID: 2600
public class MaterialMapping : ScriptableObject
{
	// Token: 0x06003DE7 RID: 15847 RVA: 0x000023F4 File Offset: 0x000005F4
	public void CleanUpData()
	{
	}

	// Token: 0x04004250 RID: 16976
	private static string path = "Assets/UberShaderConversion/MaterialMap.asset";

	// Token: 0x04004251 RID: 16977
	public static string materialDirectory = "Assets/UberShaderConversion/Materials/";

	// Token: 0x04004252 RID: 16978
	private static MaterialMapping instance;

	// Token: 0x04004253 RID: 16979
	public ShaderGroup[] map;

	// Token: 0x04004254 RID: 16980
	public Material mirrorMat;

	// Token: 0x04004255 RID: 16981
	public RenderTexture mirrorTexture;
}
