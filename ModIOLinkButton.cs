using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000712 RID: 1810
public class ModIOLinkButton : GorillaPressableButton
{
	// Token: 0x06002D2B RID: 11563 RVA: 0x000DF7F9 File Offset: 0x000DD9F9
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		base.StartCoroutine(this.ButtonPressed_Local());
		if (this.AccountLinkingTerminal != null)
		{
			this.AccountLinkingTerminal.LinkButtonPressed();
		}
	}

	// Token: 0x06002D2C RID: 11564 RVA: 0x000DF827 File Offset: 0x000DDA27
	private IEnumerator ButtonPressed_Local()
	{
		this.isOn = true;
		this.UpdateColor();
		yield return new WaitForSeconds(this.pressedTime);
		this.isOn = false;
		this.UpdateColor();
		yield break;
	}

	// Token: 0x0400336F RID: 13167
	[SerializeField]
	private float pressedTime = 0.2f;

	// Token: 0x04003370 RID: 13168
	[SerializeField]
	private ModIOAccountLinkingTerminal AccountLinkingTerminal;
}
