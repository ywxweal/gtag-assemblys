using System;
using UnityEngine;

// Token: 0x020009E7 RID: 2535
public class AssetContentAPI : ScriptableObject
{
	// Token: 0x04004080 RID: 16512
	public string bundleName;

	// Token: 0x04004081 RID: 16513
	public LazyLoadReference<TextAsset> bundleFile;

	// Token: 0x04004082 RID: 16514
	public Object[] assets = new Object[0];
}
