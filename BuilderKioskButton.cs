using System;
using UnityEngine.UI;

// Token: 0x020004EB RID: 1259
public class BuilderKioskButton : GorillaPressableButton
{
	// Token: 0x06001E87 RID: 7815 RVA: 0x00095495 File Offset: 0x00093695
	public override void Start()
	{
		this.currentPieceSet = BuilderKiosk.nullItem;
	}

	// Token: 0x06001E88 RID: 7816 RVA: 0x000954A2 File Offset: 0x000936A2
	public override void UpdateColor()
	{
		if (this.currentPieceSet.isNullItem)
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			this.myText.text = "";
			return;
		}
		base.UpdateColor();
	}

	// Token: 0x06001E89 RID: 7817 RVA: 0x000954D9 File Offset: 0x000936D9
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
	}

	// Token: 0x040021E1 RID: 8673
	public BuilderSetManager.BuilderSetStoreItem currentPieceSet;

	// Token: 0x040021E2 RID: 8674
	public BuilderKiosk kiosk;

	// Token: 0x040021E3 RID: 8675
	public Text setNameText;
}
