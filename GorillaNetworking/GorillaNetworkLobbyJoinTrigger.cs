using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C41 RID: 3137
	public class GorillaNetworkLobbyJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x0400511E RID: 20766
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x0400511F RID: 20767
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04005120 RID: 20768
		public string gameModeName;

		// Token: 0x04005121 RID: 20769
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04005122 RID: 20770
		public string componentTypeToRemove;

		// Token: 0x04005123 RID: 20771
		public GameObject componentRemoveTarget;

		// Token: 0x04005124 RID: 20772
		public string componentTypeToAdd;

		// Token: 0x04005125 RID: 20773
		public GameObject componentAddTarget;

		// Token: 0x04005126 RID: 20774
		public GameObject gorillaParent;

		// Token: 0x04005127 RID: 20775
		public GameObject joinFailedBlock;
	}
}
