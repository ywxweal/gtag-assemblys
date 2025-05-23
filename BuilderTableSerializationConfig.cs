using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004DA RID: 1242
[CreateAssetMenu(fileName = "BuilderTableSerializationConfig", menuName = "Gorilla Tag/Builder/Serialization", order = 0)]
public class BuilderTableSerializationConfig : ScriptableObject
{
	// Token: 0x04002138 RID: 8504
	public string tableConfigurationKey;

	// Token: 0x04002139 RID: 8505
	public string titleDataKey;

	// Token: 0x0400213A RID: 8506
	public string startingMapConfigKey;

	// Token: 0x0400213B RID: 8507
	public List<string> scanSlotMothershipKeys;

	// Token: 0x0400213C RID: 8508
	public string publishedScanMothershipKey;

	// Token: 0x0400213D RID: 8509
	public string timeAppend;

	// Token: 0x0400213E RID: 8510
	public string playfabScanKey;

	// Token: 0x0400213F RID: 8511
	public string sharedBlocksApiBaseURL;

	// Token: 0x04002140 RID: 8512
	public string recentVotesPrefsKey;

	// Token: 0x04002141 RID: 8513
	public string localMapsPrefsKey;
}
