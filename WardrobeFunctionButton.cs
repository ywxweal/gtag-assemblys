using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000454 RID: 1108
public class WardrobeFunctionButton : GorillaPressableButton
{
	// Token: 0x06001B50 RID: 6992 RVA: 0x00086A1E File Offset: 0x00084C1E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressWardrobeFunctionButton(this.function);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001B51 RID: 6993 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void UpdateColor()
	{
	}

	// Token: 0x06001B52 RID: 6994 RVA: 0x00086A45 File Offset: 0x00084C45
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04001E4D RID: 7757
	public string function;

	// Token: 0x04001E4E RID: 7758
	public float buttonFadeTime = 0.25f;
}
