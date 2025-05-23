using System;
using UnityEngine;

// Token: 0x0200082D RID: 2093
public class KIDUI_MessageScreen : MonoBehaviour
{
	// Token: 0x06003344 RID: 13124 RVA: 0x000F7A4E File Offset: 0x000F5C4E
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003345 RID: 13125 RVA: 0x000FCECA File Offset: 0x000FB0CA
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x04003A1A RID: 14874
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
