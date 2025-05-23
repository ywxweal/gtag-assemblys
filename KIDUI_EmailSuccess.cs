using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000827 RID: 2087
public class KIDUI_EmailSuccess : MonoBehaviour
{
	// Token: 0x06003309 RID: 13065 RVA: 0x000FBAA0 File Offset: 0x000F9CA0
	public void ShowSuccessScreen(string email)
	{
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "email_sent" } }
		});
	}

	// Token: 0x0600330A RID: 13066 RVA: 0x000FBB20 File Offset: 0x000F9D20
	public void ShowSuccessScreenAppeal(string email)
	{
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "age_appeal_email_sent" } }
		});
	}

	// Token: 0x0600330B RID: 13067 RVA: 0x000FBBA0 File Offset: 0x000F9DA0
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x0600330C RID: 13068 RVA: 0x0008B8E9 File Offset: 0x00089AE9
	public void OnCloseGame()
	{
		Application.Quit();
	}

	// Token: 0x040039E1 RID: 14817
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x040039E2 RID: 14818
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
