using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02000D3F RID: 3391
	internal class TickSystemTimer : TickSystemTimerAbstract
	{
		// Token: 0x060054F7 RID: 21751 RVA: 0x0019DD36 File Offset: 0x0019BF36
		public TickSystemTimer()
		{
		}

		// Token: 0x060054F8 RID: 21752 RVA: 0x0019DEF5 File Offset: 0x0019C0F5
		public TickSystemTimer(float cd)
			: base(cd)
		{
		}

		// Token: 0x060054F9 RID: 21753 RVA: 0x0019DEFE File Offset: 0x0019C0FE
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

		// Token: 0x0400584A RID: 22602
		public Action callback;
	}
}
