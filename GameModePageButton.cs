using System;
using UnityEngine;

// Token: 0x0200048D RID: 1165
public class GameModePageButton : GorillaPressableButton
{
	// Token: 0x06001C7A RID: 7290 RVA: 0x0008B1CC File Offset: 0x000893CC
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.ChangePage(this.left);
	}

	// Token: 0x04001F8C RID: 8076
	[SerializeField]
	private GameModePages selector;

	// Token: 0x04001F8D RID: 8077
	[SerializeField]
	private bool left;
}
