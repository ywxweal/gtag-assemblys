using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003EE RID: 1006
public class FittingRoomButton : GorillaPressableButton
{
	// Token: 0x06001827 RID: 6183 RVA: 0x00075747 File Offset: 0x00073947
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x0007575C File Offset: 0x0007395C
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

	// Token: 0x06001829 RID: 6185 RVA: 0x000757F0 File Offset: 0x000739F0
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressFittingRoomButton(this, isLeftHand);
	}

	// Token: 0x04001B0A RID: 6922
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x04001B0B RID: 6923
	public Image currentImage;

	// Token: 0x04001B0C RID: 6924
	public MeshRenderer button;

	// Token: 0x04001B0D RID: 6925
	public Material blank;

	// Token: 0x04001B0E RID: 6926
	public string noCosmeticText;

	// Token: 0x04001B0F RID: 6927
	public Text buttonText;
}
