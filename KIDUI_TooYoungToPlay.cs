using System;
using UnityEngine;

// Token: 0x02000832 RID: 2098
public class KIDUI_TooYoungToPlay : MonoBehaviour
{
	// Token: 0x0600335A RID: 13146 RVA: 0x000F7A4E File Offset: 0x000F5C4E
	public void ShowTooYoungToPlayScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600335B RID: 13147 RVA: 0x0008B8E9 File Offset: 0x00089AE9
	public void OnQuitPressed()
	{
		Application.Quit();
	}
}
