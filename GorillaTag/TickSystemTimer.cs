using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02000D3F RID: 3391
	internal class TickSystemTimer : TickSystemTimerAbstract
	{
		// Token: 0x060054F6 RID: 21750 RVA: 0x0019DC5E File Offset: 0x0019BE5E
		public TickSystemTimer()
		{
		}

		// Token: 0x060054F7 RID: 21751 RVA: 0x0019DE1D File Offset: 0x0019C01D
		public TickSystemTimer(float cd)
			: base(cd)
		{
		}

		// Token: 0x060054F8 RID: 21752 RVA: 0x0019DE26 File Offset: 0x0019C026
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void OnTimedEvent()
		{
			Action action = this.callback;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x04005849 RID: 22601
		public Action callback;
	}
}
