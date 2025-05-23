using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000A76 RID: 2678
	internal struct LeaderboardEntry_t
	{
		// Token: 0x040043B9 RID: 17337
		internal int m_nGlobalRank;

		// Token: 0x040043BA RID: 17338
		internal int m_nScore;

		// Token: 0x040043BB RID: 17339
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		internal string m_pUserName;
	}
}
