using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

// Token: 0x0200081E RID: 2078
public class KIDUI_ConfirmScreen : MonoBehaviour
{
	// Token: 0x060032E2 RID: 13026 RVA: 0x000FAC88 File Offset: 0x000F8E88
	private void Awake()
	{
		if (this._emailToConfirmTxt == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Email To Confirm Field is NULL", Array.Empty<object>());
			return;
		}
		if (this._setupScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Setup K-ID Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._mainScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Main Screen is NULL", Array.Empty<object>());
			return;
		}
		this._cancellationTokenSource = new CancellationTokenSource();
	}

	// Token: 0x060032E3 RID: 13027 RVA: 0x000FACFA File Offset: 0x000F8EFA
	private void OnEnable()
	{
		this._confirmButton.interactable = true;
		this._backButton.interactable = true;
	}

	// Token: 0x060032E4 RID: 13028 RVA: 0x000FAD14 File Offset: 0x000F8F14
	public void OnEmailSubmitted(string emailAddress)
	{
		this._submittedEmailAddress = emailAddress;
		this._emailToConfirmTxt.text = this._submittedEmailAddress;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060032E5 RID: 13029 RVA: 0x000FAD3C File Offset: 0x000F8F3C
	public void OnConfirmPressed()
	{
		KIDUI_ConfirmScreen.<OnConfirmPressed>d__16 <OnConfirmPressed>d__;
		<OnConfirmPressed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnConfirmPressed>d__.<>4__this = this;
		<OnConfirmPressed>d__.<>1__state = -1;
		<OnConfirmPressed>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<OnConfirmPressed>d__16>(ref <OnConfirmPressed>d__);
	}

	// Token: 0x060032E6 RID: 13030 RVA: 0x000FAD74 File Offset: 0x000F8F74
	public async void OnBackPressed()
	{
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_email_confirm",
			CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { { "button_pressed", "go_back" } }
		});
		this._cancellationTokenSource.Cancel();
		await this._animatedEllipsis.StopAnimation();
		base.gameObject.SetActive(false);
		this._setupScreen.OnStartSetup();
	}

	// Token: 0x060032E7 RID: 13031 RVA: 0x000FADAB File Offset: 0x000F8FAB
	public void NotifyOfResult(bool success)
	{
		this._hasCompletedSendEmailRequest = true;
		this._emailRequestResult = success;
	}

	// Token: 0x060032E8 RID: 13032 RVA: 0x000FADBC File Offset: 0x000F8FBC
	private async void ShowErrorScreen()
	{
		new StringBuilder();
		Debug.LogErrorFormat("[KID::UI::Setup] K-ID Confirmation Failed - Failed to send email", Array.Empty<object>());
		this._cancellationTokenSource.Cancel();
		await this._animatedEllipsis.StopAnimation();
		base.gameObject.SetActive(false);
		this._errorScreen.ShowErrorScreen("Confirmation Error", this._submittedEmailAddress);
	}

	// Token: 0x040039A9 RID: 14761
	[SerializeField]
	private TMP_Text _emailToConfirmTxt;

	// Token: 0x040039AA RID: 14762
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x040039AB RID: 14763
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;

	// Token: 0x040039AC RID: 14764
	[SerializeField]
	private KIDUI_ErrorScreen _errorScreen;

	// Token: 0x040039AD RID: 14765
	[SerializeField]
	private KIDUI_EmailSuccess _successScreen;

	// Token: 0x040039AE RID: 14766
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x040039AF RID: 14767
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x040039B0 RID: 14768
	[SerializeField]
	private KIDUIButton _backButton;

	// Token: 0x040039B1 RID: 14769
	[SerializeField]
	private int _minimumDelay = 1000;

	// Token: 0x040039B2 RID: 14770
	private string _submittedEmailAddress;

	// Token: 0x040039B3 RID: 14771
	private CancellationTokenSource _cancellationTokenSource;

	// Token: 0x040039B4 RID: 14772
	private bool _hasCompletedSendEmailRequest;

	// Token: 0x040039B5 RID: 14773
	private bool _emailRequestResult;
}
