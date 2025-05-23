using System;
using UnityEngine;

namespace GorillaTagScripts.ModIO
{
	// Token: 0x02000B3B RID: 2875
	[Serializable]
	public class ZoneEjectLocations
	{
		// Token: 0x04004949 RID: 18761
		public GTZone ejectZone = GTZone.none;

		// Token: 0x0400494A RID: 18762
		public GameObject[] ejectLocations;
	}
}
