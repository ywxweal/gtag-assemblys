using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C85 RID: 3205
	public class StoreBundleData : ScriptableObject
	{
		// Token: 0x06004F79 RID: 20345 RVA: 0x0017A5DC File Offset: 0x001787DC
		public void OnValidate()
		{
			if (this.playfabBundleID.Contains(' '))
			{
				Debug.LogError("ERROR THERE IS A SPACE IN THE PLAYFAB BUNDLE ID " + base.name);
			}
			if (this.bundleSKU.Contains(' '))
			{
				Debug.LogError("ERROR THERE IS A SPACE IN THE BUNDLE SKU " + base.name);
			}
		}

		// Token: 0x04005285 RID: 21125
		public string playfabBundleID = "NULL";

		// Token: 0x04005286 RID: 21126
		public string bundleSKU = "NULL SKU";

		// Token: 0x04005287 RID: 21127
		public Sprite bundleImage;

		// Token: 0x04005288 RID: 21128
		public string bundleDescriptionText = "THE NULL_BUNDLE PACK WITH 10,000 SHINY ROCKS IN THIS LIMITED TIME DLC!";
	}
}
