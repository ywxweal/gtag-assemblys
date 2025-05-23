using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02000D3E RID: 3390
	internal abstract class TickSystemTimerAbstract : CoolDownHelper, ITickSystemPre
	{
		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x060054EC RID: 21740 RVA: 0x0019DDCE File Offset: 0x0019BFCE
		// (set) Token: 0x060054ED RID: 21741 RVA: 0x0019DDD6 File Offset: 0x0019BFD6
		bool ITickSystemPre.PreTickRunning
		{
			get
			{
				return this.registered;
			}
			set
			{
				this.registered = value;
			}
		}

		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x060054EE RID: 21742 RVA: 0x0019DDCE File Offset: 0x0019BFCE
		public bool Running
		{
			get
			{
				return this.registered;
			}
		}

		// Token: 0x060054EF RID: 21743 RVA: 0x0019DDDF File Offset: 0x0019BFDF
		protected TickSystemTimerAbstract()
		{
		}

		// Token: 0x060054F0 RID: 21744 RVA: 0x0019DDE7 File Offset: 0x0019BFE7
		protected TickSystemTimerAbstract(float cd)
			: base(cd)
		{
		}

		// Token: 0x060054F1 RID: 21745 RVA: 0x0019DDF0 File Offset: 0x0019BFF0
		public override void Start()
		{
			base.Start();
			TickSystem<object>.AddPreTickCallback(this);
		}

		// Token: 0x060054F2 RID: 21746 RVA: 0x0019DDFE File Offset: 0x0019BFFE
		public override void Stop()
		{
			base.Stop();
			TickSystem<object>.RemovePreTickCallback(this);
		}

		// Token: 0x060054F3 RID: 21747 RVA: 0x0019DE0C File Offset: 0x0019C00C
		public override void OnCheckPass()
		{
			this.OnTimedEvent();
		}

		// Token: 0x060054F4 RID: 21748
		public abstract void OnTimedEvent();

		// Token: 0x060054F5 RID: 21749 RVA: 0x0019DE14 File Offset: 0x0019C014
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ITickSystemPre.PreTick()
		{
			base.CheckCooldown();
		}

		// Token: 0x04005848 RID: 22600
		[NonSerialized]
		internal bool registered;
	}
}
