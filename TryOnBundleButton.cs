using System;
using GorillaNetworking.Store;

// Token: 0x02000447 RID: 1095
public class TryOnBundleButton : GorillaPressableButton
{
	// Token: 0x06001AFB RID: 6907 RVA: 0x0008475F File Offset: 0x0008295F
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		BundleManager.instance.PressTryOnBundleButton(this, isLeftHand);
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x00084778 File Offset: 0x00082978
	public override void UpdateColor()
	{
		if (this.playfabBundleID == "NULL")
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = "";
			}
			return;
		}
		base.UpdateColor();
	}

	// Token: 0x04001DFE RID: 7678
	public int buttonIndex;

	// Token: 0x04001DFF RID: 7679
	public string playfabBundleID = "NULL";
}
