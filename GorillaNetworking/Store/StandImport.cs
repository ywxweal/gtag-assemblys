using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C8B RID: 3211
	public class StandImport
	{
		// Token: 0x06004FA8 RID: 20392 RVA: 0x0017BDB4 File Offset: 0x00179FB4
		public void DecomposeStandData(string dataString)
		{
			string[] array = dataString.Split('\t', StringSplitOptions.None);
			if (array.Length == 5)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			if (array.Length == 4)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			string text = "";
			foreach (string text2 in array)
			{
				text = text + text2 + "|";
			}
			Debug.LogError("Store Importer Data String is not valid : " + text);
		}

		// Token: 0x040052B5 RID: 21173
		public List<StandTypeData> standData = new List<StandTypeData>();
	}
}
