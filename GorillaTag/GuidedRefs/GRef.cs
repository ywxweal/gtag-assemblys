using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D5A RID: 3418
	public static class GRef
	{
		// Token: 0x06005576 RID: 21878 RVA: 0x001A03AC File Offset: 0x0019E5AC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ShouldResolveNow(GRef.EResolveModes mode)
		{
			return Application.isPlaying && (mode & GRef.EResolveModes.Runtime) == GRef.EResolveModes.Runtime;
		}

		// Token: 0x06005577 RID: 21879 RVA: 0x001A03BD File Offset: 0x0019E5BD
		public static bool IsAnyResolveModeOn(GRef.EResolveModes mode)
		{
			return mode > GRef.EResolveModes.None;
		}

		// Token: 0x02000D5B RID: 3419
		[Flags]
		public enum EResolveModes
		{
			// Token: 0x040058DE RID: 22750
			None = 0,
			// Token: 0x040058DF RID: 22751
			Runtime = 1,
			// Token: 0x040058E0 RID: 22752
			SceneProcessing = 2
		}
	}
}
