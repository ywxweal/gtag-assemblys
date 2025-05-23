using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003E0 RID: 992
public class BetaButton : GorillaPressableButton
{
	// Token: 0x060017F7 RID: 6135 RVA: 0x00074E00 File Offset: 0x00073000
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.count++;
		base.StartCoroutine(this.ButtonColorUpdate());
		if (this.count >= 10)
		{
			this.betaParent.SetActive(false);
			PlayerPrefs.SetString("CheckedBox2", "true");
			PlayerPrefs.Save();
		}
	}

	// Token: 0x060017F8 RID: 6136 RVA: 0x00074E58 File Offset: 0x00073058
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04001AD8 RID: 6872
	public GameObject betaParent;

	// Token: 0x04001AD9 RID: 6873
	public int count;

	// Token: 0x04001ADA RID: 6874
	public float buttonFadeTime = 0.25f;

	// Token: 0x04001ADB RID: 6875
	public Text messageText;
}
