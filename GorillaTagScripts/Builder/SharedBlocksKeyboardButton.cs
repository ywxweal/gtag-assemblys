using System;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B72 RID: 2930
	public class SharedBlocksKeyboardButton : GorillaKeyButton<SharedBlocksKeyboardBindings>
	{
		// Token: 0x0600488D RID: 18573 RVA: 0x0015AC85 File Offset: 0x00158E85
		public override void OnButtonPressedEvent()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
