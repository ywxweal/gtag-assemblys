using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020007EA RID: 2026
internal class MockWarningServer : WarningsServer
{
	// Token: 0x17000512 RID: 1298
	// (get) Token: 0x060031C8 RID: 12744 RVA: 0x000F5D9B File Offset: 0x000F3F9B
	public static string ShownScreenPlayerPref
	{
		get
		{
			return "screen-shown-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x060031C9 RID: 12745 RVA: 0x000F5DB3 File Offset: 0x000F3FB3
	private void Awake()
	{
		if (WarningsServer.Instance == null)
		{
			WarningsServer.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060031CA RID: 12746 RVA: 0x000F5DD4 File Offset: 0x000F3FD4
	private PlayerAgeGateWarningStatus CreateWarningStatus(string header, string body, MockWarningServer.ButtonSetup? leftButtonSetup, MockWarningServer.ButtonSetup? rightButtonSetup, bool showImage, Action leftButtonCallback, Action rightButtonCallback)
	{
		PlayerAgeGateWarningStatus playerAgeGateWarningStatus;
		playerAgeGateWarningStatus.header = header;
		playerAgeGateWarningStatus.body = body;
		playerAgeGateWarningStatus.leftButtonText = string.Empty;
		playerAgeGateWarningStatus.rightButtonText = string.Empty;
		playerAgeGateWarningStatus.leftButtonResult = WarningButtonResult.None;
		playerAgeGateWarningStatus.rightButtonResult = WarningButtonResult.None;
		playerAgeGateWarningStatus.showImage = showImage;
		playerAgeGateWarningStatus.onLeftButtonPressedAction = leftButtonCallback;
		playerAgeGateWarningStatus.onRightButtonPressedAction = rightButtonCallback;
		if (leftButtonSetup != null)
		{
			playerAgeGateWarningStatus.leftButtonText = leftButtonSetup.Value.buttonText;
			playerAgeGateWarningStatus.leftButtonResult = leftButtonSetup.Value.buttonResult;
		}
		if (rightButtonSetup != null)
		{
			playerAgeGateWarningStatus.rightButtonText = rightButtonSetup.Value.buttonText;
			playerAgeGateWarningStatus.rightButtonResult = rightButtonSetup.Value.buttonResult;
		}
		return playerAgeGateWarningStatus;
	}

	// Token: 0x060031CB RID: 12747 RVA: 0x000F5E94 File Offset: 0x000F4094
	public override async Task<PlayerAgeGateWarningStatus?> FetchPlayerData(CancellationToken token)
	{
		int num = await KIDManager.CheckKIDPhase();
		PlayerAgeGateWarningStatus? playerAgeGateWarningStatus;
		if (token.IsCancellationRequested)
		{
			playerAgeGateWarningStatus = null;
		}
		else
		{
			bool flag = GorillaServer.Instance.CheckIsInKIDOptInCohort();
			bool flag2 = GorillaServer.Instance.CheckIsInKIDRequiredCohort();
			if (!this.ShouldShowWarningScreen(num, flag))
			{
				playerAgeGateWarningStatus = new PlayerAgeGateWarningStatus?(this.CreateWarningStatus("", "", null, null, false, null, null));
			}
			else
			{
				Debug.Log(string.Format("[KID::WARNING_SERVER] Phase Is: [{0}]", num));
				PlayerAgeGateWarningStatus playerAgeGateWarningStatus2;
				switch (num)
				{
				case 1:
				{
					MockWarningServer.ButtonSetup buttonSetup = new MockWarningServer.ButtonSetup("Continue", WarningButtonResult.CloseWarning);
					playerAgeGateWarningStatus2 = this.CreateWarningStatus("IMPORTANT NEWS", "We're working to make Gorilla Tag a better, more age-appropriate experience in our next update. To learn more, please check out our Discord.", null, new MockWarningServer.ButtonSetup?(buttonSetup), false, null, null);
					break;
				}
				case 2:
					if (flag)
					{
						MockWarningServer.ButtonSetup buttonSetup2 = new MockWarningServer.ButtonSetup("Do This Later", WarningButtonResult.CloseWarning);
						MockWarningServer.ButtonSetup buttonSetup3 = new MockWarningServer.ButtonSetup("Opt-In", WarningButtonResult.OptIn);
						playerAgeGateWarningStatus2 = this.CreateWarningStatus("IMPORTANT NEWS", "We have partnered with k-ID to create a better, more age-appropriate experience. Opt-in early and get 500 Shiny Rocks as our way of saying \"Thanks!\"", new MockWarningServer.ButtonSetup?(buttonSetup2), new MockWarningServer.ButtonSetup?(buttonSetup3), true, delegate
						{
							GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
							{
								EventName = "kid_phase2_incohort",
								CustomTags = new string[]
								{
									"kid_warning_screen",
									"kid_phase_2",
									KIDTelemetry.GameVersionCustomTag,
									KIDTelemetry.GameEnvironment
								},
								BodyData = new Dictionary<string, string> { { "opt_in_choice", "skip" } }
							});
						}, delegate
						{
							GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
							{
								EventName = "kid_phase2_incohort",
								CustomTags = new string[]
								{
									"kid_warning_screen",
									"kid_phase_2",
									KIDTelemetry.GameVersionCustomTag,
									KIDTelemetry.GameEnvironment
								},
								BodyData = new Dictionary<string, string> { { "opt_in_choice", "sign_up" } }
							});
						});
					}
					else
					{
						MockWarningServer.ButtonSetup buttonSetup4 = new MockWarningServer.ButtonSetup("Continue", WarningButtonResult.CloseWarning);
						playerAgeGateWarningStatus2 = this.CreateWarningStatus("IMPORTANT NEWS", "We're working to make Gorilla Tag a better, more age-appropriate experience in the coming days. To learn more, please check out our Discord.", null, new MockWarningServer.ButtonSetup?(buttonSetup4), false, null, null);
						GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
						{
							EventName = "kid_screen_shown",
							CustomTags = new string[]
							{
								"kid_warning_screen",
								"kid_phase_2",
								KIDTelemetry.GameVersionCustomTag,
								KIDTelemetry.GameEnvironment
							},
							BodyData = new Dictionary<string, string> { { "screen", "phase2_nocohort" } }
						});
					}
					break;
				case 3:
					if (flag2)
					{
						MockWarningServer.ButtonSetup buttonSetup5 = new MockWarningServer.ButtonSetup("Continue", WarningButtonResult.OptIn);
						playerAgeGateWarningStatus2 = this.CreateWarningStatus("IMPORTANT NEWS", "We have partnered with k-ID to create a better, more age-appropriate experience. Confirm your age and get 500 Shiny Rocks as our way of saying \"Thanks!\"", new MockWarningServer.ButtonSetup?(buttonSetup5), null, true, delegate
						{
							GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
							{
								EventName = "kid_screen_shown",
								CustomTags = new string[]
								{
									"kid_warning_screen",
									"kid_phase_3",
									KIDTelemetry.GameVersionCustomTag,
									KIDTelemetry.GameEnvironment
								},
								BodyData = new Dictionary<string, string> { { "screen", "phase3_required" } }
							});
						}, null);
					}
					else
					{
						MockWarningServer.ButtonSetup buttonSetup6 = new MockWarningServer.ButtonSetup("Do This Later", WarningButtonResult.CloseWarning);
						MockWarningServer.ButtonSetup buttonSetup7 = new MockWarningServer.ButtonSetup("Opt-In", WarningButtonResult.OptIn);
						playerAgeGateWarningStatus2 = this.CreateWarningStatus("IMPORTANT NEWS", "We have partnered with k-ID to create a better, more age-appropriate experience. Opt-in early and get 500 Shiny Rocks as our way of saying \"Thanks!\"", new MockWarningServer.ButtonSetup?(buttonSetup6), new MockWarningServer.ButtonSetup?(buttonSetup7), true, delegate
						{
							GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
							{
								EventName = "kid_phase3_optional",
								CustomTags = new string[]
								{
									"kid_warning_screen",
									"kid_phase_3",
									KIDTelemetry.GameVersionCustomTag,
									KIDTelemetry.GameEnvironment
								},
								BodyData = new Dictionary<string, string> { { "opt_in_choice", "skip" } }
							});
						}, delegate
						{
							GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
							{
								EventName = "kid_phase3_optional",
								CustomTags = new string[]
								{
									"kid_warning_screen",
									"kid_phase_3",
									KIDTelemetry.GameVersionCustomTag,
									KIDTelemetry.GameEnvironment
								},
								BodyData = new Dictionary<string, string> { { "opt_in_choice", "sign_up" } }
							});
						});
					}
					break;
				case 4:
					if (PlayFabAuthenticator.instance.IsReturningPlayer)
					{
						MockWarningServer.ButtonSetup buttonSetup8 = new MockWarningServer.ButtonSetup("Continue", WarningButtonResult.OptIn);
						playerAgeGateWarningStatus2 = this.CreateWarningStatus("IMPORTANT NEWS", "We have partnered with k-ID to create a better, more age-appropriate experience. Confirm your age and get 100 Shiny Rocks as our way of saying \"Thanks!\"", null, new MockWarningServer.ButtonSetup?(buttonSetup8), true, delegate
						{
							GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
							{
								EventName = "kid_screen_shown",
								CustomTags = new string[]
								{
									"kid_warning_screen",
									"kid_phase_4",
									KIDTelemetry.GameVersionCustomTag,
									KIDTelemetry.GameEnvironment
								},
								BodyData = new Dictionary<string, string> { { "screen", "phase4" } }
							});
						}, null);
					}
					else
					{
						playerAgeGateWarningStatus2 = this.CreateWarningStatus("", "", null, null, false, null, null);
					}
					break;
				default:
					return new PlayerAgeGateWarningStatus?(this.CreateWarningStatus("", "", null, null, false, null, null));
				}
				PlayerPrefs.SetInt(string.Format("phase-{0}-{1}", num, MockWarningServer.ShownScreenPlayerPref), 1);
				PlayerPrefs.Save();
				playerAgeGateWarningStatus = new PlayerAgeGateWarningStatus?(playerAgeGateWarningStatus2);
			}
		}
		return playerAgeGateWarningStatus;
	}

	// Token: 0x060031CC RID: 12748 RVA: 0x000F5EE0 File Offset: 0x000F40E0
	public override async Task<PlayerAgeGateWarningStatus?> GetOptInFollowUpMessage(CancellationToken token)
	{
		int num = await KIDManager.CheckKIDPhase();
		PlayerAgeGateWarningStatus? playerAgeGateWarningStatus;
		if (token.IsCancellationRequested)
		{
			playerAgeGateWarningStatus = null;
		}
		else
		{
			PlayerAgeGateWarningStatus? playerAgeGateWarningStatus2 = null;
			switch (num)
			{
			case 2:
			{
				MockWarningServer.ButtonSetup buttonSetup = new MockWarningServer.ButtonSetup("Yay!", WarningButtonResult.CloseWarning);
				playerAgeGateWarningStatus2 = new PlayerAgeGateWarningStatus?(this.CreateWarningStatus("", "Your shiny rocks have been granted.", null, new MockWarningServer.ButtonSetup?(buttonSetup), true, null, null));
				break;
			}
			case 3:
			{
				MockWarningServer.ButtonSetup buttonSetup2 = new MockWarningServer.ButtonSetup("Yay!", WarningButtonResult.CloseWarning);
				playerAgeGateWarningStatus2 = new PlayerAgeGateWarningStatus?(this.CreateWarningStatus("", "Your shiny rocks have been granted.", null, new MockWarningServer.ButtonSetup?(buttonSetup2), true, null, null));
				break;
			}
			case 4:
				if (PlayFabAuthenticator.instance.IsReturningPlayer)
				{
					MockWarningServer.ButtonSetup buttonSetup3 = new MockWarningServer.ButtonSetup("Yay!", WarningButtonResult.CloseWarning);
					playerAgeGateWarningStatus2 = new PlayerAgeGateWarningStatus?(this.CreateWarningStatus("", "Your shiny rocks have been granted.", null, new MockWarningServer.ButtonSetup?(buttonSetup3), true, null, null));
				}
				break;
			}
			playerAgeGateWarningStatus = playerAgeGateWarningStatus2;
		}
		return playerAgeGateWarningStatus;
	}

	// Token: 0x060031CD RID: 12749 RVA: 0x000F5F2B File Offset: 0x000F412B
	private bool ShouldShowWarningScreen(int phase, bool inOptInCohort)
	{
		if (PlayerPrefs.GetInt(string.Format("phase-{0}-{1}", phase, MockWarningServer.ShownScreenPlayerPref), 0) == 0)
		{
			return true;
		}
		switch (phase)
		{
		default:
			return false;
		case 2:
			return inOptInCohort;
		case 3:
		case 4:
			return true;
		}
	}

	// Token: 0x04003880 RID: 14464
	private const string SHOWN_SCREEN_PREFIX = "screen-shown-";

	// Token: 0x020007EB RID: 2027
	public struct ButtonSetup
	{
		// Token: 0x060031CF RID: 12751 RVA: 0x000F5F71 File Offset: 0x000F4171
		public ButtonSetup(string txt, WarningButtonResult result)
		{
			this.buttonText = txt;
			this.buttonResult = result;
		}

		// Token: 0x04003881 RID: 14465
		public string buttonText;

		// Token: 0x04003882 RID: 14466
		public WarningButtonResult buttonResult;
	}
}
