using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000AB4 RID: 2740
	public class NetTimeAverages : DoubleAverages
	{
		// Token: 0x06004220 RID: 16928 RVA: 0x00131956 File Offset: 0x0012FB56
		public NetTimeAverages(int sampleCount)
			: base(sampleCount)
		{
		}

		// Token: 0x06004221 RID: 16929 RVA: 0x0013195F File Offset: 0x0012FB5F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double DefaultTypeValue()
		{
			return 1.0;
		}
	}
}
