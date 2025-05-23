using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200043A RID: 1082
public class PurchaseItemButton : GorillaPressableButton
{
	// Token: 0x06001AB3 RID: 6835 RVA: 0x00082D0B File Offset: 0x00080F0B
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressPurchaseItemButton(this, isLeftHand);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x00082D2E File Offset: 0x00080F2E
	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x04001DC6 RID: 7622
	public string buttonSide;
}
