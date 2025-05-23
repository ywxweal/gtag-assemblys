using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000AB4 RID: 2740
	public class NetTimeAverages : DoubleAverages
	{
		// Token: 0x0600421F RID: 16927 RVA: 0x0013187E File Offset: 0x0012FA7E
		public NetTimeAverages(int sampleCount)
			: base(sampleCount)
		{
		}

		// Token: 0x06004220 RID: 16928 RVA: 0x00131887 File Offset: 0x0012FA87
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double DefaultTypeValue()
		{
			return 1.0;
		}
	}
}
