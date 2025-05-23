using System;
using UnityEngine;

// Token: 0x02000832 RID: 2098
public class KIDUI_TooYoungToPlay : MonoBehaviour
{
	// Token: 0x0600335B RID: 13147 RVA: 0x000F7B26 File Offset: 0x000F5D26
	public void ShowTooYoungToPlayScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600335C RID: 13148 RVA: 0x0008B909 File Offset: 0x00089B09
	public void OnQuitPressed()
	{
		Application.Quit();
	}
}
