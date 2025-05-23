using System;

// Token: 0x020008DC RID: 2268
public class GorillaNetworkLeaveTutorialTrigger : GorillaTriggerBox
{
	// Token: 0x0600372C RID: 14124 RVA: 0x0010AF8A File Offset: 0x0010918A
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		NetworkSystem.Instance.SetMyTutorialComplete();
	}
}
