using System;
using UnityEngine;

// Token: 0x02000973 RID: 2419
public class BetterBaker : MonoBehaviour
{
	// Token: 0x04003F45 RID: 16197
	public string bakeryLightmapDirectory;

	// Token: 0x04003F46 RID: 16198
	public string dayNightLightmapsDirectory;

	// Token: 0x04003F47 RID: 16199
	public GameObject[] allLights;

	// Token: 0x02000974 RID: 2420
	public struct LightMapMap
	{
		// Token: 0x04003F48 RID: 16200
		public string timeOfDayName;

		// Token: 0x04003F49 RID: 16201
		public GameObject lightObject;
	}
}
