using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000827 RID: 2087
public class KIDUI_EmailSuccess : MonoBehaviour
{
	// Token: 0x0600330A RID: 13066 RVA: 0x000FBB78 File Offset: 0x000F9D78
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

	// Token: 0x0600330B RID: 13067 RVA: 0x000FBBF8 File Offset: 0x000F9DF8
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

	// Token: 0x0600330C RID: 13068 RVA: 0x000FBC78 File Offset: 0x000F9E78
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x0600330D RID: 13069 RVA: 0x0008B909 File Offset: 0x00089B09
	public void OnCloseGame()
	{
		Application.Quit();
	}

	// Token: 0x040039E2 RID: 14818
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x040039E3 RID: 14819
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
