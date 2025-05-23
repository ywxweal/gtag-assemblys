using System;

namespace GorillaNetworking
{
	// Token: 0x02000C2D RID: 3117
	public class GorillaATMKeyButton : GorillaKeyButton<GorillaATMKeyBindings>
	{
		// Token: 0x06004D29 RID: 19753 RVA: 0x0016F4A5 File Offset: 0x0016D6A5
		public override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaATMKeyButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
