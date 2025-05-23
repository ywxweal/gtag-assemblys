using System;
using UnityEngine;

// Token: 0x020006FC RID: 1788
public class CustomMapsTerminalControlButton : GorillaPressableButton
{
	// Token: 0x1700045A RID: 1114
	// (get) Token: 0x06002C7B RID: 11387 RVA: 0x000D73F9 File Offset: 0x000D55F9
	// (set) Token: 0x06002C7C RID: 11388 RVA: 0x000DB60F File Offset: 0x000D980F
	public bool IsLocked
	{
		get
		{
			return this.isOn;
		}
		set
		{
			this.isOn = value;
		}
	}

	// Token: 0x06002C7D RID: 11389 RVA: 0x000DB618 File Offset: 0x000D9818
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.mapsTerminal == null)
		{
			return;
		}
		this.mapsTerminal.HandleTerminalControlButtonPressed();
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x000DB63C File Offset: 0x000D983C
	public void LockTerminalControl()
	{
		if (this.IsLocked)
		{
			return;
		}
		this.IsLocked = true;
		this.UpdateColor();
		if (this.myText != null)
		{
			this.myText.color = this.lockedTextColor;
			return;
		}
		if (this.myTmpText != null)
		{
			this.myTmpText.color = this.lockedTextColor;
		}
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x000DB6A0 File Offset: 0x000D98A0
	public void UnlockTerminalControl()
	{
		if (!this.IsLocked)
		{
			return;
		}
		this.IsLocked = false;
		this.UpdateColor();
		if (this.myText != null)
		{
			this.myText.color = this.unlockedTextColor;
			return;
		}
		if (this.myTmpText != null)
		{
			this.myTmpText.color = this.unlockedTextColor;
		}
	}

	// Token: 0x040032C7 RID: 12999
	[SerializeField]
	private Color unlockedTextColor = Color.black;

	// Token: 0x040032C8 RID: 13000
	[SerializeField]
	private Color lockedTextColor = Color.white;

	// Token: 0x040032C9 RID: 13001
	[SerializeField]
	private CustomMapsTerminal mapsTerminal;
}
