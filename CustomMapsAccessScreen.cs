using System;
using TMPro;
using UnityEngine;

// Token: 0x0200072B RID: 1835
public class CustomMapsAccessScreen : CustomMapsTerminalScreen
{
	// Token: 0x06002DA6 RID: 11686 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void Initialize()
	{
	}

	// Token: 0x06002DA7 RID: 11687 RVA: 0x000E25F0 File Offset: 0x000E07F0
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

	// Token: 0x06002DA8 RID: 11688 RVA: 0x000E2657 File Offset: 0x000E0857
	public override void Hide()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(true);
		base.Hide();
	}

	// Token: 0x06002DA9 RID: 11689 RVA: 0x000E2692 File Offset: 0x000E0892
	public void Reset()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(true);
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x000E26C8 File Offset: 0x000E08C8
	public void DisplayError(string errorMessage)
	{
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(false);
		this.errorText.text = errorMessage;
		this.errorText.gameObject.SetActive(true);
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x000E2714 File Offset: 0x000E0914
	public void ShowTerminalControlPrompt()
	{
		this.errorText.gameObject.SetActive(false);
		this.loginRequiredText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(true);
	}

	// Token: 0x040033F1 RID: 13297
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x040033F2 RID: 13298
	[SerializeField]
	private TMP_Text loginRequiredText;

	// Token: 0x040033F3 RID: 13299
	[SerializeField]
	private TMP_Text terminalControlPromptText;

	// Token: 0x040033F4 RID: 13300
	[SerializeField]
	private GameObject[] buttonsToHide;
}
