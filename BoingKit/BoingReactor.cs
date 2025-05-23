using System;

namespace BoingKit
{
	// Token: 0x02000E6C RID: 3692
	public class BoingReactor : BoingBehavior
	{
		// Token: 0x06005C5E RID: 23646 RVA: 0x001C5181 File Offset: 0x001C3381
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06005C5F RID: 23647 RVA: 0x001C5189 File Offset: 0x001C3389
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06005C60 RID: 23648 RVA: 0x001C5191 File Offset: 0x001C3391
		public override void PrepareExecute()
		{
			base.PrepareExecute(true);
		}
	}
}
