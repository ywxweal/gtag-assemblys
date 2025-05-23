using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003E3 RID: 995
public class CheckoutCartButton : GorillaPressableButton
{
	// Token: 0x06001803 RID: 6147 RVA: 0x00074FC7 File Offset: 0x000731C7
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x00074FDC File Offset: 0x000731DC
	public override void UpdateColor()
	{
		if (this.currentCosmeticItem.itemName == "null")
		{
			this.button.material = this.unpressedMaterial;
			this.buttonText.text = this.noCosmeticText;
			return;
		}
		if (this.isOn)
		{
			this.button.material = this.pressedMaterial;
			this.buttonText.text = this.onText;
			return;
		}
		this.button.material = this.unpressedMaterial;
		this.buttonText.text = this.offText;
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x00075070 File Offset: 0x00073270
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCheckoutCartButton(this, isLeftHand);
	}

	// Token: 0x04001AE1 RID: 6881
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x04001AE2 RID: 6882
	public Image currentImage;

	// Token: 0x04001AE3 RID: 6883
	public MeshRenderer button;

	// Token: 0x04001AE4 RID: 6884
	public Material blank;

	// Token: 0x04001AE5 RID: 6885
	public string noCosmeticText;

	// Token: 0x04001AE6 RID: 6886
	public Text buttonText;
}
