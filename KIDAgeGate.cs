using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KID.Model;
using TMPro;
using UnityEngine;

// Token: 0x020007B4 RID: 1972
public class KIDAgeGate : MonoBehaviour
{
	// Token: 0x170004F2 RID: 1266
	// (get) Token: 0x060030CC RID: 12492 RVA: 0x000EFFB6 File Offset: 0x000EE1B6
	public static int UserAge
	{
		get
		{
			return KIDAgeGate._ageValue;
		}
	}

	// Token: 0x170004F3 RID: 1267
	// (get) Token: 0x060030CD RID: 12493 RVA: 0x000EFFBD File Offset: 0x000EE1BD
	// (set) Token: 0x060030CE RID: 12494 RVA: 0x000EFFC4 File Offset: 0x000EE1C4
	public static bool DisplayedScreen { get; private set; }

	// Token: 0x060030CF RID: 12495 RVA: 0x000EFFCC File Offset: 0x000EE1CC
	private void Awake()
	{
		if (KIDAgeGate._activeReference != null)
		{
			Debug.LogError("[KID::Age_Gate] Age Gate already exists, this is a duplicate, deleting the new one");
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		KIDAgeGate._activeReference = this;
	}

	// Token: 0x060030D0 RID: 12496 RVA: 0x000EFFF8 File Offset: 0x000EE1F8
	private async void Start()
	{
	}

	// Token: 0x060030D1 RID: 12497 RVA: 0x000F0027 File Offset: 0x000EE227
	private void OnDestroy()
	{
		this.requestCancellationSource.Cancel();
	}

	// Token: 0x060030D2 RID: 12498 RVA: 0x000F0034 File Offset: 0x000EE234
	public static async Task BeginAgeGate()
	{
		if (KIDAgeGate._activeReference == null)
		{
			Debug.LogError("[KID::Age_Gate] Unable to start Age Gate. No active reference assigned. Has it initialised yet?");
			do
			{
				await Task.Yield();
			}
			while (KIDAgeGate._activeReference == null);
		}
		await KIDAgeGate._activeReference.StartAgeGate();
	}

	// Token: 0x060030D3 RID: 12499 RVA: 0x000F0070 File Offset: 0x000EE270
	private async Task StartAgeGate()
	{
		await this.InitialiseAgeGate();
	}

	// Token: 0x060030D4 RID: 12500 RVA: 0x000F00B4 File Offset: 0x000EE2B4
	private async Task InitialiseAgeGate()
	{
		Debug.Log("[KID] Initialising Age-Gate");
		KIDTelemetryData kidtelemetryData = new KIDTelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				KIDTelemetry.Open_MetricActionCustomTag,
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "age_gate" } }
		};
		GorillaTelemetry.SendMothershipAnalytics(kidtelemetryData);
		for (;;)
		{
			KIDAgeGate.DisplayedScreen = true;
			this._ageSlider.ControllerActive = true;
			PrivateUIRoom.AddUI(this._uiParent.transform);
			HandRayController.Instance.EnableHandRays();
			await this.ProcessAgeGate();
			this._ageSlider.ControllerActive = false;
			PrivateUIRoom.RemoveUI(this._uiParent.transform);
			if (this.requestCancellationSource.IsCancellationRequested)
			{
				break;
			}
			AgeStatusType ageStatusType;
			if (KIDManager.TryGetAgeStatusTypeFromAge(KIDAgeGate.UserAge, out ageStatusType))
			{
				kidtelemetryData = new KIDTelemetryData
				{
					EventName = "kid_age_gate",
					CustomTags = new string[]
					{
						KIDTelemetry.Closed_MetricActionCustomTag,
						"kid_age_gate",
						KIDTelemetry.GameVersionCustomTag,
						KIDTelemetry.GameEnvironment
					},
					BodyData = new Dictionary<string, string> { 
					{
						"age_declared",
						ageStatusType.ToString()
					} }
				};
				GorillaTelemetry.SendMothershipAnalytics(kidtelemetryData);
			}
			this._confirmationUIManager.Reset();
			PrivateUIRoom.AddUI(this._confirmationUI.transform);
			bool flag = await this.ProcessAgeGateConfirmation();
			kidtelemetryData = new KIDTelemetryData
			{
				EventName = "kid_age_gate_confirm",
				CustomTags = new string[]
				{
					"kid_age_gate",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string> { 
				{
					"button_pressed",
					flag ? "confirm" : "go_back"
				} }
			};
			GorillaTelemetry.SendMothershipAnalytics(kidtelemetryData);
			PrivateUIRoom.RemoveUI(this._confirmationUI.transform);
			HandRayController.Instance.DisableHandRays();
			if (flag)
			{
				goto Block_4;
			}
		}
		return;
		Block_4:
		await this.OnAgeGateCompleted();
		Debug.Log("[KID] Age Gate Complete");
	}

	// Token: 0x060030D5 RID: 12501 RVA: 0x000F00F8 File Offset: 0x000EE2F8
	private async Task ProcessAgeGate()
	{
		Debug.Log("[KID] Waiting for Age Confirmation");
		await this.WaitForAgeChoice();
	}

	// Token: 0x060030D6 RID: 12502 RVA: 0x000F013C File Offset: 0x000EE33C
	private async Task<bool> ProcessAgeGateConfirmation()
	{
		while (this._confirmationUIManager.Result == KidAgeConfirmationResult.None)
		{
			if (this.requestCancellationSource.IsCancellationRequested)
			{
				return false;
			}
			await Task.Yield();
		}
		return this._confirmationUIManager.Result == KidAgeConfirmationResult.Confirm;
	}

	// Token: 0x060030D7 RID: 12503 RVA: 0x000F0180 File Offset: 0x000EE380
	private async Task WaitForAgeChoice()
	{
		KIDAgeGate._hasChosenAge = false;
		while (!this.requestCancellationSource.IsCancellationRequested)
		{
			await Task.Yield();
			if (KIDAgeGate._hasChosenAge)
			{
				KIDAgeGate._ageValue = this._ageSlider.CurrentAge;
				string ageString = this._ageSlider.GetAgeString();
				this._confirmationAgeText.text = "You entered " + ageString + "\n\nPlease be sure to enter your real age so we can customize your experience!";
				return;
			}
		}
	}

	// Token: 0x060030D8 RID: 12504 RVA: 0x000F01C3 File Offset: 0x000EE3C3
	public static void OnConfirmAgePressed(int currentAge)
	{
		KIDAgeGate._hasChosenAge = true;
	}

	// Token: 0x060030D9 RID: 12505 RVA: 0x000F01CC File Offset: 0x000EE3CC
	private async Task OnAgeGateCompleted()
	{
		this.FinaliseAgeGateAndContinue();
	}

	// Token: 0x060030DA RID: 12506 RVA: 0x000F020F File Offset: 0x000EE40F
	private void FinaliseAgeGateAndContinue()
	{
		if (this.requestCancellationSource.IsCancellationRequested)
		{
			return;
		}
		Debug.Log("[KID::AGE_GATE] Age gate completed");
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060030DB RID: 12507 RVA: 0x000F0234 File Offset: 0x000EE434
	private void QuitGame()
	{
		Debug.Log("[KID] QUIT PRESSED");
		Application.Quit();
	}

	// Token: 0x060030DC RID: 12508 RVA: 0x000F0248 File Offset: 0x000EE448
	private async void AppealAge()
	{
		Debug.Log("[KID] APPEAL PRESSED");
		if (!KIDManager.InitialisationComplete)
		{
			Debug.LogError("[KID] [KIDManager] has not been Initialised yet. Unable to start appeals flow. Will wait until ready");
			do
			{
				await Task.Yield();
			}
			while (!KIDManager.InitialisationComplete);
		}
		if (KIDManager.InitialisationSuccessful)
		{
			string text = "VERIFY AGE";
			string text2 = "GETTING ONE TIME PASSCODE. PLEASE WAIT.\n\nGIVE IT TO A PARENT/GUARDIAN TO ENTER IT AT: k-id.com/code";
			string empty = string.Empty;
			this._pregameMessageReference.ShowMessage(text, text2, empty, new Action(this.RefreshChallengeStatus), 0.25f, 0f);
		}
		Debug.LogError("[KID::AGE_GATE] TODO: Refactor Age-Appeal flow");
	}

	// Token: 0x060030DD RID: 12509 RVA: 0x000F0280 File Offset: 0x000EE480
	private void AppealRejected()
	{
		Debug.Log("[KID] APPEAL REJECTED");
		string text = "UNDER AGE";
		string text2 = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";
		string text3 = "Hold any face button to appeal";
		this._pregameMessageReference.ShowMessage(text, text2, text3, new Action(this.AppealAge), 0.25f, 0f);
	}

	// Token: 0x060030DE RID: 12510 RVA: 0x000023F4 File Offset: 0x000005F4
	private void RefreshChallengeStatus()
	{
	}

	// Token: 0x060030DF RID: 12511 RVA: 0x000F02CD File Offset: 0x000EE4CD
	public static void SetAgeGateConfig(GetRequirementsData response)
	{
		KIDAgeGate._ageGateConfig = response;
	}

	// Token: 0x060030E0 RID: 12512 RVA: 0x000F02D8 File Offset: 0x000EE4D8
	public void OnWhyAgeGateButtonPressed()
	{
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "why_age_gate" } }
		});
		this._uiParent.SetActive(false);
		PrivateUIRoom.AddUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(true);
	}

	// Token: 0x060030E1 RID: 12513 RVA: 0x000F0368 File Offset: 0x000EE568
	public void OnWhyAgeGateButtonBackPressed()
	{
		this._uiParent.SetActive(true);
		PrivateUIRoom.RemoveUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(false);
	}

	// Token: 0x060030E2 RID: 12514 RVA: 0x000F0394 File Offset: 0x000EE594
	public void OnLearnMoreAboutKIDPressed()
	{
		this._metrics_LearnMorePressed = true;
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "screen", "learn_more_url" } }
		});
		Application.OpenURL("https://whyagegate.com/");
	}

	// Token: 0x0400372C RID: 14124
	private const string LEARN_MORE_URL = "https://whyagegate.com/";

	// Token: 0x0400372D RID: 14125
	private const string DEFAULT_AGE_VALUE_STRING = "SET AGE";

	// Token: 0x0400372E RID: 14126
	private const int MINIMUM_PLATFORM_AGE = 13;

	// Token: 0x0400372F RID: 14127
	[Header("Age Gate Settings")]
	[SerializeField]
	private PreGameMessage _pregameMessageReference;

	// Token: 0x04003730 RID: 14128
	[SerializeField]
	private KIDUI_AgeDiscrepancyScreen _ageDiscrepancyScreen;

	// Token: 0x04003731 RID: 14129
	[SerializeField]
	private GameObject _uiParent;

	// Token: 0x04003732 RID: 14130
	[SerializeField]
	private AgeSliderWithProgressBar _ageSlider;

	// Token: 0x04003733 RID: 14131
	[SerializeField]
	private GameObject _confirmationUI;

	// Token: 0x04003734 RID: 14132
	[SerializeField]
	private KIDAgeGateConfirmation _confirmationUIManager;

	// Token: 0x04003735 RID: 14133
	[SerializeField]
	private TMP_Text _confirmationAgeText;

	// Token: 0x04003736 RID: 14134
	[SerializeField]
	private GameObject _whyAgeGateScreen;

	// Token: 0x04003737 RID: 14135
	private const string strBlockAccessTitle = "UNDER AGE";

	// Token: 0x04003738 RID: 14136
	private const string strBlockAccessMessage = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";

	// Token: 0x04003739 RID: 14137
	private const string strBlockAccessConfirm = "Hold any face button to appeal";

	// Token: 0x0400373A RID: 14138
	private const string strVerifyAgeTitle = "VERIFY AGE";

	// Token: 0x0400373B RID: 14139
	private const string strVerifyAgeMessage = "GETTING ONE TIME PASSCODE. PLEASE WAIT.\n\nGIVE IT TO A PARENT/GUARDIAN TO ENTER IT AT: k-id.com/code";

	// Token: 0x0400373C RID: 14140
	private const string strDiscrepancyMessage = "You entered {0} for your age,\nbut your Meta account says you should be {1}. You could be logged into the wrong Meta account on this device.\n\nWe will use the lowest age ({2})\nif you Continue.";

	// Token: 0x0400373D RID: 14141
	private static KIDAgeGate _activeReference;

	// Token: 0x0400373E RID: 14142
	private static GetRequirementsData _ageGateConfig;

	// Token: 0x0400373F RID: 14143
	private static int _ageValue;

	// Token: 0x04003740 RID: 14144
	private bool _hasCompletedMeta;

	// Token: 0x04003741 RID: 14145
	private CancellationTokenSource requestCancellationSource = new CancellationTokenSource();

	// Token: 0x04003742 RID: 14146
	private static bool _hasChosenAge;

	// Token: 0x04003744 RID: 14148
	private bool _metrics_LearnMorePressed;
}
