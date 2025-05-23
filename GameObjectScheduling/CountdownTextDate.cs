using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000E25 RID: 3621
	[CreateAssetMenu(fileName = "New CountdownText Date", menuName = "Game Object Scheduling/CountdownText Date", order = 1)]
	public class CountdownTextDate : ScriptableObject
	{
		// Token: 0x04005EB1 RID: 24241
		public string CountdownTo = "1/1/0001 00:00:00";

		// Token: 0x04005EB2 RID: 24242
		public string FormatString = "{0} {1}";

		// Token: 0x04005EB3 RID: 24243
		public string DefaultString = "";

		// Token: 0x04005EB4 RID: 24244
		public int DaysThreshold = 365;
	}
}
