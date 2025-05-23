using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000811 RID: 2065
public class KIDUI_AgeAppealEmailConfirmation : MonoBehaviour
{
	// Token: 0x060032AE RID: 12974 RVA: 0x000F9C65 File Offset: 0x000F7E65
	private void OnEnable()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Combine(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x060032AF RID: 12975 RVA: 0x000F9C87 File Offset: 0x000F7E87
	private void OnDisable()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x060032B0 RID: 12976 RVA: 0x000F9CAC File Offset: 0x000F7EAC
	public void ShowAgeAppealConfirmationScreen(bool hasChallenge, int newAge, string emailToConfirm)
	{
		this.hasChallenge = hasChallenge;
		this.newAgeToAppeal = newAge;
		this._confirmText.text = (this.hasChallenge ? this.CONFIRM_PARENT_EMAIL : this.CONFIRM_YOUR_EMAIL);
		this._emailText.text = emailToConfirm;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060032B1 RID: 12977 RVA: 0x000F9D00 File Offset: 0x000F7F00
	public void OnConfirmPressed()
	{
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_age_appeal_confirm_email",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"email_type",
					this.hasChallenge ? "under_dac" : "over_dac"
				},
				{ "button_pressed", "confirm" }
			}
		});
		if (this.hasChallenge)
		{
			this.StartAgeAppealChallengeEmail();
			return;
		}
		this.StartAgeAppealEmail();
	}

	// Token: 0x060032B2 RID: 12978 RVA: 0x000F9DA0 File Offset: 0x000F7FA0
	public void OnBackPressed()
	{
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_age_appeal_confirm_email",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"email_type",
					this.hasChallenge ? "under_dac" : "over_dac"
				},
				{ "button_pressed", "go_back" }
			}
		});
		base.gameObject.SetActive(false);
		this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(this.hasChallenge, this.newAgeToAppeal);
	}

	// Token: 0x060032B3 RID: 12979 RVA: 0x000F9E4C File Offset: 0x000F804C
	private void StartAgeAppealChallengeEmail()
	{
		KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealChallengeEmail>d__16 <StartAgeAppealChallengeEmail>d__;
		<StartAgeAppealChallengeEmail>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartAgeAppealChallengeEmail>d__.<>4__this = this;
		<StartAgeAppealChallengeEmail>d__.<>1__state = -1;
		<StartAgeAppealChallengeEmail>d__.<>t__builder.Start<KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealChallengeEmail>d__16>(ref <StartAgeAppealChallengeEmail>d__);
	}

	// Token: 0x060032B4 RID: 12980 RVA: 0x000F9E84 File Offset: 0x000F8084
	private async Task StartAgeAppealEmail()
	{
		TaskAwaiter<bool> taskAwaiter = KIDManager.TryAppealAge(this._emailText.text, this.newAgeToAppeal).GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<bool> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		if (!taskAwaiter.GetResult())
		{
			base.gameObject.SetActive(false);
			this._errorScreen.ShowAgeAppealEmailErrorScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
		}
		else
		{
			Debug.Log("[KID::UI::APPEAL_AGE_EMAIL] Age appeal succesful for [" + this._emailText.text + "]. Proceeding tu Success screen");
			base.gameObject.SetActive(false);
			this._successScreen.ShowSuccessScreenAppeal(this._emailText.text);
		}
	}

	// Token: 0x060032B5 RID: 12981 RVA: 0x000F9EC8 File Offset: 0x000F80C8
	private void NotifyOfEmailResult(bool success)
	{
		if (this._successScreen == null)
		{
			Debug.LogError("[KID::AGE_APPEAL_EMAIL] _successScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		this._hasCompletedSendEmailRequest = true;
		if (success)
		{
			base.gameObject.SetActive(false);
			this._successScreen.ShowSuccessScreenAppeal(this._emailText.text);
			return;
		}
	}

	// Token: 0x060032B6 RID: 12982 RVA: 0x000F9F1B File Offset: 0x000F811B
	private void ShowErrorScreen()
	{
		Debug.LogErrorFormat("[KID::UI::Setup] K-ID Confirmation Failed - Failed to send email", Array.Empty<object>());
		base.gameObject.SetActive(false);
		this._errorScreen.ShowAgeAppealEmailErrorScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
	}

	// Token: 0x04003961 RID: 14689
	[SerializeField]
	private TMP_Text _confirmText;

	// Token: 0x04003962 RID: 14690
	[SerializeField]
	private TMP_Text _emailText;

	// Token: 0x04003963 RID: 14691
	private string CONFIRM_PARENT_EMAIL = "Please confirm your parent or guardian's email address.";

	// Token: 0x04003964 RID: 14692
	private string CONFIRM_YOUR_EMAIL = "Please confirm your email address.";

	// Token: 0x04003965 RID: 14693
	private bool hasChallenge = true;

	// Token: 0x04003966 RID: 14694
	private int newAgeToAppeal;

	// Token: 0x04003967 RID: 14695
	private bool _hasCompletedSendEmailRequest;

	// Token: 0x04003968 RID: 14696
	[SerializeField]
	private KIDUI_EmailSuccess _successScreen;

	// Token: 0x04003969 RID: 14697
	[SerializeField]
	private KIDUI_AgeAppealEmailError _errorScreen;

	// Token: 0x0400396A RID: 14698
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x0400396B RID: 14699
	[SerializeField]
	private int _minimumDelay = 1000;
}
