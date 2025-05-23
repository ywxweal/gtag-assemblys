using System;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B72 RID: 2930
	public class SharedBlocksKeyboardButton : GorillaKeyButton<SharedBlocksKeyboardBindings>
	{
		// Token: 0x0600488E RID: 18574 RVA: 0x0015AD5D File Offset: 0x00158F5D
		public override void OnButtonPressedEvent()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
