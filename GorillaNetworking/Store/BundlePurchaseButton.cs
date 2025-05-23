using System;
using System.Collections;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000C80 RID: 3200
	public class BundlePurchaseButton : GorillaPressableButton, IGorillaSliceableSimple
	{
		// Token: 0x06004F4D RID: 20301 RVA: 0x00017251 File Offset: 0x00015451
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06004F4E RID: 20302 RVA: 0x0001725A File Offset: 0x0001545A
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06004F4F RID: 20303 RVA: 0x00179D9C File Offset: 0x00177F9C
		public void SliceUpdate()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion && !this.bError)
			{
				base.enabled = false;
				base.GetComponent<BoxCollider>().enabled = false;
				this.buttonRenderer.material = this.pressedMaterial;
				this.myText.text = this.UnavailableText;
			}
		}

		// Token: 0x06004F50 RID: 20304 RVA: 0x00179DFF File Offset: 0x00177FFF
		public override void ButtonActivation()
		{
			if (this.bError)
			{
				return;
			}
			base.ButtonActivation();
			BundleManager.instance.BundlePurchaseButtonPressed(this.playfabID);
			base.StartCoroutine(this.ButtonColorUpdate());
		}

		// Token: 0x06004F51 RID: 20305 RVA: 0x00179E30 File Offset: 0x00178030
		public void AlreadyOwn()
		{
			if (this.bError)
			{
				return;
			}
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.onText = this.AlreadyOwnText;
			this.myText.text = this.AlreadyOwnText;
			this.isOn = true;
		}

		// Token: 0x06004F52 RID: 20306 RVA: 0x00179E90 File Offset: 0x00178090
		public void ResetButton()
		{
			if (this.bError)
			{
				return;
			}
			base.enabled = true;
			base.GetComponent<BoxCollider>().enabled = true;
			this.buttonRenderer.material = this.unpressedMaterial;
			this.myText.text = this.offText;
			this.isOn = false;
		}

		// Token: 0x06004F53 RID: 20307 RVA: 0x00179EE2 File Offset: 0x001780E2
		private IEnumerator ButtonColorUpdate()
		{
			this.buttonRenderer.material = this.pressedMaterial;
			yield return new WaitForSeconds(this.debounceTime);
			this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
			yield break;
		}

		// Token: 0x06004F54 RID: 20308 RVA: 0x00179EF4 File Offset: 0x001780F4
		public void ErrorHappened()
		{
			this.bError = true;
			this.myText.text = this.ErrorText;
			this.buttonRenderer.material = this.unpressedMaterial;
			base.enabled = false;
			this.offText = this.ErrorText;
			this.onText = this.ErrorText;
			this.isOn = false;
		}

		// Token: 0x06004F55 RID: 20309 RVA: 0x00179F50 File Offset: 0x00178150
		public void InitializeData()
		{
			if (this.bError)
			{
				return;
			}
			this.myText.text = this.offText;
			this.buttonRenderer.material = this.unpressedMaterial;
			base.enabled = true;
			this.isOn = false;
		}

		// Token: 0x06004F56 RID: 20310 RVA: 0x00179F8B File Offset: 0x0017818B
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (!this.bError)
			{
				this.offText = purchaseText;
				this.UpdateColor();
			}
		}

		// Token: 0x06004F58 RID: 20312 RVA: 0x00011040 File Offset: 0x0000F240
		bool IGorillaSliceableSimple.get_isActiveAndEnabled()
		{
			return base.isActiveAndEnabled;
		}

		// Token: 0x0400526D RID: 21101
		public bool bError;

		// Token: 0x0400526E RID: 21102
		public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";

		// Token: 0x0400526F RID: 21103
		public string AlreadyOwnText = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";

		// Token: 0x04005270 RID: 21104
		public string UnavailableText = "UNAVAILABLE";

		// Token: 0x04005271 RID: 21105
		public string playfabID = "";
	}
}
