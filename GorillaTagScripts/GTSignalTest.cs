using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000B19 RID: 2841
	public class GTSignalTest : GTSignalListener
	{
		// Token: 0x04004870 RID: 18544
		public MeshRenderer[] targets = new MeshRenderer[0];

		// Token: 0x04004871 RID: 18545
		[Space]
		public MeshRenderer target;

		// Token: 0x04004872 RID: 18546
		public List<GTSignalListener> listeners = new List<GTSignalListener>(12);
	}
}
