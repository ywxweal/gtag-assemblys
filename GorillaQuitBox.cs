using System;
using UnityEngine;

// Token: 0x02000499 RID: 1177
public class GorillaQuitBox : GorillaTriggerBox
{
	// Token: 0x06001CA6 RID: 7334 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Start()
	{
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x0008B909 File Offset: 0x00089B09
	public override void OnBoxTriggered()
	{
		Application.Quit();
	}
}
