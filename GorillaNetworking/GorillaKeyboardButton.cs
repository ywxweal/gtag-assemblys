using System;

namespace GorillaNetworking
{
	// Token: 0x02000C3C RID: 3132
	public class GorillaKeyboardButton : GorillaKeyButton<GorillaKeyboardBindings>
	{
		// Token: 0x06004DF7 RID: 19959 RVA: 0x0017407E File Offset: 0x0017227E
		public override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
