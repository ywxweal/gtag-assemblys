using System;
using TMPro;
using UnityEngine;

// Token: 0x0200072B RID: 1835
public class CustomMapsAccessScreen : CustomMapsTerminalScreen
{
	// Token: 0x06002DA5 RID: 11685 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void Initialize()
	{
	}

	// Token: 0x06002DA6 RID: 11686 RVA: 0x000E254C File Offset: 0x000E074C
	public override void Show()
	{
		base.Show();
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(true);
		for (int i = 0; i < this.buttonsToHide.Length; i++)
		{
			this.buttonsToHide[i].SetActive(false);
		}
	}

	// Token: 0x06002DA7 RID: 11687 RVA: 0x000E25B3 File Offset: 0x000E07B3
	public override void Hide()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(true);
		base.Hide();
	}

	// Token: 0x06002DA8 RID: 11688 RVA: 0x000E25EE File Offset: 0x000E07EE
	public void Reset()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(true);
	}

	// Token: 0x06002DA9 RID: 11689 RVA: 0x000E2624 File Offset: 0x000E0824
	public void DisplayError(string errorMessage)
	{
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(false);
		this.errorText.text = errorMessage;
		this.errorText.gameObject.SetActive(true);
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x000E2670 File Offset: 0x000E0870
	public void ShowTerminalControlPrompt()
	{
		this.errorText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(true);
	}

	// Token: 0x040033EF RID: 13295
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x040033F0 RID: 13296
	[SerializeField]
	private TMP_Text loginRequiredText;

	// Token: 0x040033F1 RID: 13297
	[SerializeField]
	private TMP_Text terminalControlPromptText;

	// Token: 0x040033F2 RID: 13298
	[SerializeField]
	private GameObject[] buttonsToHide;
}
