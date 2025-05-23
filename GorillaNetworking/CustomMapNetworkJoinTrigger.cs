using System;

namespace GorillaNetworking
{
	// Token: 0x02000C3E RID: 3134
	public class CustomMapNetworkJoinTrigger : GorillaNetworkJoinTrigger
	{
		// Token: 0x06004DFE RID: 19966 RVA: 0x00174108 File Offset: 0x00172308
		public override string GetFullDesiredGameModeString()
		{
			return this.networkZone + GorillaComputer.instance.currentQueue + CustomMapLoader.LoadedMapModId.ToString() + base.GetDesiredGameType();
		}
	}
}
