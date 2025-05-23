using System;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class NativeSizeChangerButton : GorillaPressableButton
{
	// Token: 0x06000E81 RID: 3713 RVA: 0x00049188 File Offset: 0x00047388
	public override void ButtonActivation()
	{
		this.nativeSizeChanger.Activate(this.settings);
	}

	// Token: 0x040011B8 RID: 4536
	[SerializeField]
	private NativeSizeChanger nativeSizeChanger;

	// Token: 0x040011B9 RID: 4537
	[SerializeField]
	private NativeSizeChangerSettings settings;
}
