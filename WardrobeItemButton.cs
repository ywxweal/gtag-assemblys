using System;
using GorillaNetworking;

// Token: 0x02000457 RID: 1111
public class WardrobeItemButton : GorillaPressableButton
{
	// Token: 0x06001B5D RID: 7005 RVA: 0x00086B0D File Offset: 0x00084D0D
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressWardrobeItemButton(this.currentCosmeticItem, isLeftHand);
	}

	// Token: 0x04001E54 RID: 7764
	public HeadModel controlledModel;

	// Token: 0x04001E55 RID: 7765
	public CosmeticsController.CosmeticItem currentCosmeticItem;
}
