using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000710 RID: 1808
public class ForceDisableHoverboardTrigger : MonoBehaviour
{
	// Token: 0x06002D19 RID: 11545 RVA: 0x000DEDBD File Offset: 0x000DCFBD
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.wasEnabled = GTPlayer.Instance.isHoverAllowed;
			GTPlayer.Instance.SetHoverAllowed(false, true);
		}
	}

	// Token: 0x06002D1A RID: 11546 RVA: 0x000DEDF0 File Offset: 0x000DCFF0
	public void OnTriggerExit(Collider other)
	{
		if (!this.reEnableOnExit || !this.wasEnabled)
		{
			return;
		}
		if (this.reEnableOnlyInVStump && !GorillaComputer.instance.IsPlayerInVirtualStump())
		{
			return;
		}
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(true, false);
		}
	}

	// Token: 0x0400335E RID: 13150
	[Tooltip("If TRUE and the Hoverboard was enabled when the player entered this trigger, it will be re-enabled when they exit.")]
	public bool reEnableOnExit = true;

	// Token: 0x0400335F RID: 13151
	public bool reEnableOnlyInVStump = true;

	// Token: 0x04003360 RID: 13152
	private bool wasEnabled;
}
