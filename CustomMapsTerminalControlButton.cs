using System;
using UnityEngine;

// Token: 0x020006FC RID: 1788
public class CustomMapsTerminalControlButton : GorillaPressableButton
{
	// Token: 0x1700045A RID: 1114
	// (get) Token: 0x06002C7C RID: 11388 RVA: 0x000D749D File Offset: 0x000D569D
	// (set) Token: 0x06002C7D RID: 11389 RVA: 0x000DB6B3 File Offset: 0x000D98B3
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

	// Token: 0x06002C7E RID: 11390 RVA: 0x000DB6BC File Offset: 0x000D98BC
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.mapsTerminal == null)
		{
			return;
		}
		this.mapsTerminal.HandleTerminalControlButtonPressed();
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x000DB6E0 File Offset: 0x000D98E0
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

	// Token: 0x06002C80 RID: 11392 RVA: 0x000DB744 File Offset: 0x000D9944
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

	// Token: 0x040032C9 RID: 13001
	[SerializeField]
	private Color unlockedTextColor = Color.black;

	// Token: 0x040032CA RID: 13002
	[SerializeField]
	private Color lockedTextColor = Color.white;

	// Token: 0x040032CB RID: 13003
	[SerializeField]
	private CustomMapsTerminal mapsTerminal;
}
