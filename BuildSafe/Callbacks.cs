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
			// Token: 0x06004A11 RID: 18961 RVA: 0x00161A11 File Offset: 0x0015FC11
			public DidReloadScripts(bool activeOnly = false)
			{
				this.activeOnly = activeOnly;
			}

			// Token: 0x04004CFD RID: 19709
			public bool activeOnly;
		}
	}
}
