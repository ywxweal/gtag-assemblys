using System;

namespace BuildSafe
{
	// Token: 0x02000BBC RID: 3004
	public class SessionState
	{
		// Token: 0x1700072C RID: 1836
		public string this[string key]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x04004D16 RID: 19734
		public static readonly SessionState Shared = new SessionState();
	}
}
