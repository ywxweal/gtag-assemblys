using System;

// Token: 0x020008DC RID: 2268
public class GorillaNetworkLeaveTutorialTrigger : GorillaTriggerBox
{
	// Token: 0x0600372D RID: 14125 RVA: 0x0010B062 File Offset: 0x00109262
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		NetworkSystem.Instance.SetMyTutorialComplete();
	}
}
