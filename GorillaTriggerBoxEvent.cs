using System;
using UnityEngine.Events;

// Token: 0x0200049E RID: 1182
public class GorillaTriggerBoxEvent : GorillaTriggerBox
{
	// Token: 0x06001CB0 RID: 7344 RVA: 0x0008B96B File Offset: 0x00089B6B
	public override void OnBoxTriggered()
	{
		if (this.onBoxTriggered != null)
		{
			this.onBoxTriggered.Invoke();
		}
	}

	// Token: 0x04001FEE RID: 8174
	public UnityEvent onBoxTriggered;
}
