using System;
using UnityEngine;

// Token: 0x02000A29 RID: 2601
[Serializable]
public struct ShaderGroup
{
	// Token: 0x06003DEB RID: 15851 RVA: 0x00126110 File Offset: 0x00124310
	public ShaderGroup(Material material, Shader original, Shader gameplay, Shader baking)
	{
		this.material = material;
		this.originalShader = original;
		this.gameplayShader = gameplay;
		this.bakingShader = baking;
	}

	// Token: 0x04004257 RID: 16983
	public Material material;

	// Token: 0x04004258 RID: 16984
	public Shader originalShader;

	// Token: 0x04004259 RID: 16985
	public Shader gameplayShader;

	// Token: 0x0400425A RID: 16986
	public Shader bakingShader;
}
