using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003EB RID: 1003
public class CosmeticStand : GorillaPressableButton
{
	// Token: 0x06001817 RID: 6167 RVA: 0x00075520 File Offset: 0x00073720
	public void InitializeCosmetic()
	{
		this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((CosmeticsController.CosmeticItem x) => this.thisCosmeticName == x.displayName || this.thisCosmeticName == x.overrideDisplayName || this.thisCosmeticName == x.itemName);
		if (this.slotPriceText != null)
		{
			this.slotPriceText.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
		}
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x0007559E File Offset: 0x0007379E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCosmeticStandButton(this);
	}

	// Token: 0x04001B01 RID: 6913
	public CosmeticsController.CosmeticItem thisCosmeticItem;

	// Token: 0x04001B02 RID: 6914
	public string thisCosmeticName;

	// Token: 0x04001B03 RID: 6915
	public HeadModel thisHeadModel;

	// Token: 0x04001B04 RID: 6916
	public Text slotPriceText;

	// Token: 0x04001B05 RID: 6917
	public Text addToCartText;

	// Token: 0x04001B06 RID: 6918
	[Tooltip("If this is true then this cosmetic stand should have already been updated when the 'Update Cosmetic Stands' button was pressed in the CosmeticsController inspector.")]
	public bool skipMe;
}
