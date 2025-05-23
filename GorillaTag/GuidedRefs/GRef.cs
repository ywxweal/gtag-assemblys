using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D5A RID: 3418
	public static class GRef
	{
		// Token: 0x06005577 RID: 21879 RVA: 0x001A0484 File Offset: 0x0019E684
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ShouldResolveNow(GRef.EResolveModes mode)
		{
			return Application.isPlaying && (mode & GRef.EResolveModes.Runtime) == GRef.EResolveModes.Runtime;
		}

		// Token: 0x06005578 RID: 21880 RVA: 0x001A0495 File Offset: 0x0019E695
		public static bool IsAnyResolveModeOn(GRef.EResolveModes mode)
		{
			return mode > GRef.EResolveModes.None;
		}

		// Token: 0x02000D5B RID: 3419
		[Flags]
		public enum EResolveModes
		{
			// Token: 0x040058DF RID: 22751
			None = 0,
			// Token: 0x040058E0 RID: 22752
			Runtime = 1,
			// Token: 0x040058E1 RID: 22753
			SceneProcessing = 2
		}
	}
}
