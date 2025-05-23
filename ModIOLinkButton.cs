using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000712 RID: 1810
public class ModIOLinkButton : GorillaPressableButton
{
	// Token: 0x06002D2C RID: 11564 RVA: 0x000DF89D File Offset: 0x000DDA9D
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		base.StartCoroutine(this.ButtonPressed_Local());
		if (this.AccountLinkingTerminal != null)
		{
			this.AccountLinkingTerminal.LinkButtonPressed();
		}
	}

	// Token: 0x06002D2D RID: 11565 RVA: 0x000DF8CB File Offset: 0x000DDACB
	private IEnumerator ButtonPressed_Local()
	{
		this.isOn = true;
		this.UpdateColor();
		yield return new WaitForSeconds(this.pressedTime);
		this.isOn = false;
		this.UpdateColor();
		yield break;
	}

	// Token: 0x04003371 RID: 13169
	[SerializeField]
	private float pressedTime = 0.2f;

	// Token: 0x04003372 RID: 13170
	[SerializeField]
	private ModIOAccountLinkingTerminal AccountLinkingTerminal;
}
