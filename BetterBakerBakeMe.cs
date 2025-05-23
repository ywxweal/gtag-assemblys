using System;
using System.Collections.Generic;
using GorillaTag.Rendering.Shaders;
using UnityEngine;

// Token: 0x02000975 RID: 2421
public class BetterBakerBakeMe : FlagForBaking
{
	// Token: 0x04003F4B RID: 16203
	public GameObject[] stuffIncludingParentsToBake;

	// Token: 0x04003F4C RID: 16204
	public GameObject getMatStuffFromHere;

	// Token: 0x04003F4D RID: 16205
	public List<ShaderConfigData.ShaderConfig> allConfigs = new List<ShaderConfigData.ShaderConfig>();
}
