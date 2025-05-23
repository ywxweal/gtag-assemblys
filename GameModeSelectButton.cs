using System;
using UnityEngine;

// Token: 0x0200048F RID: 1167
public class GameModeSelectButton : GorillaPressableButton
{
	// Token: 0x06001C89 RID: 7305 RVA: 0x0008B4C3 File Offset: 0x000896C3
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.SelectEntryOnPage(this.buttonIndex);
	}

	// Token: 0x04001F95 RID: 8085
	[SerializeField]
	internal GameModePages selector;

	// Token: 0x04001F96 RID: 8086
	[SerializeField]
	internal int buttonIndex;
}
