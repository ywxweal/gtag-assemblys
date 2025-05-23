using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02000D3E RID: 3390
	internal abstract class TickSystemTimerAbstract : CoolDownHelper, ITickSystemPre
	{
		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x060054ED RID: 21741 RVA: 0x0019DEA6 File Offset: 0x0019C0A6
		// (set) Token: 0x060054EE RID: 21742 RVA: 0x0019DEAE File Offset: 0x0019C0AE
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
		// (get) Token: 0x060054EF RID: 21743 RVA: 0x0019DEA6 File Offset: 0x0019C0A6
		public bool Running
		{
			get
			{
				return this.registered;
			}
		}

		// Token: 0x060054F0 RID: 21744 RVA: 0x0019DEB7 File Offset: 0x0019C0B7
		protected TickSystemTimerAbstract()
		{
		}

		// Token: 0x060054F1 RID: 21745 RVA: 0x0019DEBF File Offset: 0x0019C0BF
		protected TickSystemTimerAbstract(float cd)
			: base(cd)
		{
		}

		// Token: 0x060054F2 RID: 21746 RVA: 0x0019DEC8 File Offset: 0x0019C0C8
		public override void Start()
		{
			base.Start();
			TickSystem<object>.AddPreTickCallback(this);
		}

		// Token: 0x060054F3 RID: 21747 RVA: 0x0019DED6 File Offset: 0x0019C0D6
		public override void Stop()
		{
			base.Stop();
			TickSystem<object>.RemovePreTickCallback(this);
		}

		// Token: 0x060054F4 RID: 21748 RVA: 0x0019DEE4 File Offset: 0x0019C0E4
		public override void OnCheckPass()
		{
			this.OnTimedEvent();
		}

		// Token: 0x060054F5 RID: 21749
		public abstract void OnTimedEvent();

		// Token: 0x060054F6 RID: 21750 RVA: 0x0019DEEC File Offset: 0x0019C0EC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ITickSystemPre.PreTick()
		{
			base.CheckCooldown();
		}

		// Token: 0x04005849 RID: 22601
		[NonSerialized]
		internal bool registered;
	}
}
