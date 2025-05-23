using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200083D RID: 2109
internal abstract class WarningsServer : MonoBehaviour
{
	// Token: 0x06003373 RID: 13171
	public abstract Task<PlayerAgeGateWarningStatus?> FetchPlayerData(CancellationToken token);

	// Token: 0x06003374 RID: 13172
	public abstract Task<PlayerAgeGateWarningStatus?> GetOptInFollowUpMessage(CancellationToken token);

	// Token: 0x04003A63 RID: 14947
	public static volatile WarningsServer Instance;
}
