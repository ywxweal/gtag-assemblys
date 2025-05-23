using System;

namespace GorillaNetworking
{
	// Token: 0x02000C2D RID: 3117
	public class GorillaATMKeyButton : GorillaKeyButton<GorillaATMKeyBindings>
	{
		// Token: 0x06004D2A RID: 19754 RVA: 0x0016F57D File Offset: 0x0016D77D
		public override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaATMKeyButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
