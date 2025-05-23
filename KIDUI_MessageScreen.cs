using System;
using UnityEngine;

// Token: 0x0200082D RID: 2093
public class KIDUI_MessageScreen : MonoBehaviour
{
	// Token: 0x06003345 RID: 13125 RVA: 0x000F7B26 File Offset: 0x000F5D26
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003346 RID: 13126 RVA: 0x000FCFA2 File Offset: 0x000FB1A2
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x04003A1B RID: 14875
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
