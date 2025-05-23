using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000438 RID: 1080
public class PurchaseCurrencyButton : GorillaPressableButton
{
	// Token: 0x06001AAA RID: 6826 RVA: 0x00082C1C File Offset: 0x00080E1C
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		ATM_Manager.instance.PressCurrencyPurchaseButton(this.purchaseCurrencySize);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x00082C43 File Offset: 0x00080E43
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04001DC1 RID: 7617
	public string purchaseCurrencySize;

	// Token: 0x04001DC2 RID: 7618
	public float buttonFadeTime = 0.25f;
}
