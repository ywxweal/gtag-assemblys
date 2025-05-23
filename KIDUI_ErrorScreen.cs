using System;
using TMPro;
using UnityEngine;

// Token: 0x02000828 RID: 2088
public class KIDUI_ErrorScreen : MonoBehaviour
{
	// Token: 0x0600330F RID: 13071 RVA: 0x000FBC92 File Offset: 0x000F9E92
	public void ShowErrorScreen(string title, string email)
	{
		this._titleTxt.text = title;
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003310 RID: 13072 RVA: 0x000FBCB8 File Offset: 0x000F9EB8
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x06003311 RID: 13073 RVA: 0x0008B909 File Offset: 0x00089B09
	public void OnQuitGame()
	{
		Application.Quit();
	}

	// Token: 0x06003312 RID: 13074 RVA: 0x000FBCD2 File Offset: 0x000F9ED2
	public void OnBack()
	{
		base.gameObject.SetActive(false);
		this._setupScreen.OnStartSetup();
	}

	// Token: 0x040039E4 RID: 14820
	[SerializeField]
	private TMP_Text _titleTxt;

	// Token: 0x040039E5 RID: 14821
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x040039E6 RID: 14822
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x040039E7 RID: 14823
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;
}
