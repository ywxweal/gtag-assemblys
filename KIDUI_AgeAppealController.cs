using System;
using System.Collections.Generic;
using KID.Model;
using UnityEngine;

// Token: 0x0200080F RID: 2063
public class KIDUI_AgeAppealController : MonoBehaviour
{
	// Token: 0x1700051F RID: 1311
	// (get) Token: 0x060032A3 RID: 12963 RVA: 0x000F993C File Offset: 0x000F7B3C
	public static KIDUI_AgeAppealController Instance
	{
		get
		{
			return KIDUI_AgeAppealController._instance;
		}
	}

	// Token: 0x060032A4 RID: 12964 RVA: 0x000F9943 File Offset: 0x000F7B43
	private void Awake()
	{
		KIDUI_AgeAppealController._instance = this;
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Controller Initialised", Array.Empty<object>());
	}

	// Token: 0x060032A5 RID: 12965 RVA: 0x000F995C File Offset: 0x000F7B5C
	public void StartAgeAppealScreens(SessionStatus? sessionStatus)
	{
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Showing k-ID Age Appeal Screens", Array.Empty<object>());
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(base.transform);
		this._firstAgeAppealScreen.ShowRestrictedAccessScreen(sessionStatus);
		AgeStatusType ageStatusType;
		if (KIDManager.TryGetAgeStatusTypeFromAge(KIDAgeGate.UserAge, out ageStatusType))
		{
			GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
			{
				EventName = "kid_age_appeal",
				CustomTags = new string[]
				{
					"kid_age_appeal",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string> { 
				{
					"submitted_age",
					ageStatusType.ToString()
				} }
			});
		}
	}

	// Token: 0x060032A6 RID: 12966 RVA: 0x000F9A0A File Offset: 0x000F7C0A
	public void CloseKIDScreens()
	{
		PrivateUIRoom.RemoveUI(base.transform);
		HandRayController.Instance.DisableHandRays();
		this._firstAgeAppealScreen.gameObject.SetActive(false);
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x060032A7 RID: 12967 RVA: 0x000F9A40 File Offset: 0x000F7C40
	public void StartTooYoungToPlayScreen()
	{
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Showing k-ID Too Young to Play Screen", Array.Empty<object>());
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(base.transform);
		this._tooYoungToPlayScreen.ShowTooYoungToPlayScreen();
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "blocked" } }
		});
	}

	// Token: 0x060032A8 RID: 12968 RVA: 0x0008B8E9 File Offset: 0x00089AE9
	public void OnQuitGamePressed()
	{
		Application.Quit();
	}

	// Token: 0x04003955 RID: 14677
	private static KIDUI_AgeAppealController _instance;

	// Token: 0x04003956 RID: 14678
	[SerializeField]
	private KIDUI_RestrictedAccessScreen _firstAgeAppealScreen;

	// Token: 0x04003957 RID: 14679
	[SerializeField]
	private KIDUI_TooYoungToPlay _tooYoungToPlayScreen;
}
