using System;

namespace GorillaNetworking
{
	// Token: 0x02000C3E RID: 3134
	public class CustomMapNetworkJoinTrigger : GorillaNetworkJoinTrigger
	{
		// Token: 0x06004DFF RID: 19967 RVA: 0x001741E0 File Offset: 0x001723E0
		public override string GetFullDesiredGameModeString()
		{
			return this.networkZone + GorillaComputer.instance.currentQueue + CustomMapLoader.LoadedMapModId.ToString() + base.GetDesiredGameType();
		}
	}
}
