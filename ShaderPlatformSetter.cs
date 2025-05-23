using System;
using UnityEngine;

// Token: 0x0200020C RID: 524
public static class ShaderPlatformSetter
{
	// Token: 0x06000C1E RID: 3102 RVA: 0x00040231 File Offset: 0x0003E431
	[RuntimeInitializeOnLoadMethod]
	public static void HandleRuntimeInitializeOnLoad()
	{
		Shader.DisableKeyword("PLATFORM_IS_ANDROID");
		Shader.DisableKeyword("QATESTING");
	}
}
