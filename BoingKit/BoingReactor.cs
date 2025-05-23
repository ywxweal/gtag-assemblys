using System;

namespace BoingKit
{
	// Token: 0x02000E6C RID: 3692
	public class BoingReactor : BoingBehavior
	{
		// Token: 0x06005C5F RID: 23647 RVA: 0x001C5259 File Offset: 0x001C3459
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06005C60 RID: 23648 RVA: 0x001C5261 File Offset: 0x001C3461
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06005C61 RID: 23649 RVA: 0x001C5269 File Offset: 0x001C3469
		public override void PrepareExecute()
		{
			base.PrepareExecute(true);
		}
	}
}
