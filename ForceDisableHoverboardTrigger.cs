using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000710 RID: 1808
public class ForceDisableHoverboardTrigger : MonoBehaviour
{
	// Token: 0x06002D1A RID: 11546 RVA: 0x000DEE61 File Offset: 0x000DD061
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.wasEnabled = GTPlayer.Instance.isHoverAllowed;
			GTPlayer.Instance.SetHoverAllowed(false, true);
		}
	}

	// Token: 0x06002D1B RID: 11547 RVA: 0x000DEE94 File Offset: 0x000DD094
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

	// Token: 0x04003360 RID: 13152
	[Tooltip("If TRUE and the Hoverboard was enabled when the player entered this trigger, it will be re-enabled when they exit.")]
	public bool reEnableOnExit = true;

	// Token: 0x04003361 RID: 13153
	public bool reEnableOnlyInVStump = true;

	// Token: 0x04003362 RID: 13154
	private bool wasEnabled;
}
