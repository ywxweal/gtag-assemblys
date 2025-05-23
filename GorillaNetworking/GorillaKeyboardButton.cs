using System;

namespace GorillaNetworking
{
	// Token: 0x02000C3C RID: 3132
	public class GorillaKeyboardButton : GorillaKeyButton<GorillaKeyboardBindings>
	{
		// Token: 0x06004DF6 RID: 19958 RVA: 0x00173FA6 File Offset: 0x001721A6
		public override void OnButtonPressedEvent()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
