using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001D1 RID: 465
public class GorillaActionButton : GorillaPressableButton
{
	// Token: 0x06000ADD RID: 2781 RVA: 0x0003AA13 File Offset: 0x00038C13
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.onPress.Invoke();
	}

	// Token: 0x04000D68 RID: 3432
	[SerializeField]
	public UnityEvent onPress;
}
