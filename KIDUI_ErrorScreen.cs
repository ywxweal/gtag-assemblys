using System;
using TMPro;
using UnityEngine;

// Token: 0x02000828 RID: 2088
public class KIDUI_ErrorScreen : MonoBehaviour
{
	// Token: 0x0600330E RID: 13070 RVA: 0x000FBBBA File Offset: 0x000F9DBA
	public void ShowErrorScreen(string title, string email)
	{
		this._titleTxt.text = title;
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600330F RID: 13071 RVA: 0x000FBBE0 File Offset: 0x000F9DE0
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x06003310 RID: 13072 RVA: 0x0008B8E9 File Offset: 0x00089AE9
	public void OnQuitGame()
	{
		Application.Quit();
	}

	// Token: 0x06003311 RID: 13073 RVA: 0x000FBBFA File Offset: 0x000F9DFA
	public void OnBack()
	{
		base.gameObject.SetActive(false);
		this._setupScreen.OnStartSetup();
	}

	// Token: 0x040039E3 RID: 14819
	[SerializeField]
	private TMP_Text _titleTxt;

	// Token: 0x040039E4 RID: 14820
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x040039E5 RID: 14821
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x040039E6 RID: 14822
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;
}
