using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x02000BA9 RID: 2985
	public static class Callbacks
	{
		// Token: 0x02000BAA RID: 2986
		[Conditional("UNITY_EDITOR")]
		public class DidReloadScripts : Attribute
		{
			// Token: 0x06004A12 RID: 18962 RVA: 0x00161AE9 File Offset: 0x0015FCE9
			public DidReloadScripts(bool activeOnly = false)
			{
				this.activeOnly = activeOnly;
			}

			// Token: 0x04004CFE RID: 19710
			public bool activeOnly;
		}
	}
}
