using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02000C54 RID: 3156
	[Serializable]
	internal class FeatureFlagData
	{
		// Token: 0x04005151 RID: 20817
		public string name;

		// Token: 0x04005152 RID: 20818
		public int value;

		// Token: 0x04005153 RID: 20819
		public string valueType;

		// Token: 0x04005154 RID: 20820
		public List<string> alwaysOnForUsers;
	}
}
