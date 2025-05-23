using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C8C RID: 3212
	public class StandTypeData
	{
		// Token: 0x06004FA9 RID: 20393 RVA: 0x0017BD70 File Offset: 0x00179F70
		public StandTypeData(string[] spawnData)
		{
			this.departmentID = spawnData[0];
			this.displayID = spawnData[1];
			this.standID = spawnData[2];
			this.bustType = spawnData[3];
			if (spawnData.Length == 5)
			{
				this.playFabID = spawnData[4];
			}
			Debug.Log(string.Concat(new string[] { "StoreStuff: StandTypeData: ", this.departmentID, "\n", this.displayID, "\n", this.standID, "\n", this.bustType, "\n", this.playFabID }));
		}

		// Token: 0x040052B5 RID: 21173
		public string departmentID = "";

		// Token: 0x040052B6 RID: 21174
		public string displayID = "";

		// Token: 0x040052B7 RID: 21175
		public string standID = "";

		// Token: 0x040052B8 RID: 21176
		public string bustType = "";

		// Token: 0x040052B9 RID: 21177
		public string playFabID = "";

		// Token: 0x02000C8D RID: 3213
		public enum EStandDataID
		{
			// Token: 0x040052BB RID: 21179
			departmentID,
			// Token: 0x040052BC RID: 21180
			displayID,
			// Token: 0x040052BD RID: 21181
			standID,
			// Token: 0x040052BE RID: 21182
			bustType,
			// Token: 0x040052BF RID: 21183
			playFabID,
			// Token: 0x040052C0 RID: 21184
			Count
		}
	}
}
