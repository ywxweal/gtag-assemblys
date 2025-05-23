using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000B2C RID: 2860
	public class VirtualStumpReturnWatchTrigger : MonoBehaviour
	{
		// Token: 0x0600466D RID: 18029 RVA: 0x0014EC37 File Offset: 0x0014CE37
		public void OnTriggerEnter(Collider other)
		{
			if (other == GTPlayer.Instance.headCollider)
			{
				VRRig.LocalRig.EnableVStumpReturnWatch(false);
			}
		}

		// Token: 0x0600466E RID: 18030 RVA: 0x0014EC56 File Offset: 0x0014CE56
		public void OnTriggerExit(Collider other)
		{
			if (other == GTPlayer.Instance.headCollider && GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				VRRig.LocalRig.EnableVStumpReturnWatch(true);
			}
		}
	}
}
