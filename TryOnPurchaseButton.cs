using System;
using System.Collections;
using GorillaNetworking.Store;
using UnityEngine;

// Token: 0x02000449 RID: 1097
public class TryOnPurchaseButton : GorillaPressableButton
{
	// Token: 0x06001B17 RID: 6935 RVA: 0x00085140 File Offset: 0x00083340
	public void Update()
	{
		if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion && !this.bError)
		{
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.myText.text = "UNAVAILABLE";
		}
	}

	// Token: 0x06001B18 RID: 6936 RVA: 0x000851A2 File Offset: 0x000833A2
	public override void ButtonActivation()
	{
		if (this.bError)
		{
			return;
		}
		base.ButtonActivation();
		BundleManager.instance.PressPurchaseTryOnBundleButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001B19 RID: 6937 RVA: 0x000851CC File Offset: 0x000833CC
	public void AlreadyOwn()
	{
		if (this.bError)
		{
			return;
		}
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = this.AlreadyOwnText;
	}

	// Token: 0x06001B1A RID: 6938 RVA: 0x0008520C File Offset: 0x0008340C
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
	}

	// Token: 0x06001B1B RID: 6939 RVA: 0x0008524C File Offset: 0x0008344C
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x06001B1C RID: 6940 RVA: 0x0008525B File Offset: 0x0008345B
	public void ErrorHappened()
	{
		this.bError = true;
		this.myText.text = this.ErrorText;
		this.buttonRenderer.material = this.unpressedMaterial;
		base.enabled = false;
		this.isOn = false;
	}

	// Token: 0x04001E11 RID: 7697
	public bool bError;

	// Token: 0x04001E12 RID: 7698
	public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";

	// Token: 0x04001E13 RID: 7699
	public string AlreadyOwnText;
}
