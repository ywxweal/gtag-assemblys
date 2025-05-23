using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// Token: 0x02000810 RID: 2064
public class KIDUI_AgeAppealEmailScreen : MonoBehaviour
{
	// Token: 0x060032AA RID: 12970 RVA: 0x000F9AD8 File Offset: 0x000F7CD8
	public void ShowAgeAppealEmailScreen(bool receivedChallenge, int newAge)
	{
		this.newAgeToAppeal = newAge;
		base.gameObject.SetActive(true);
		this.hasChallenge = receivedChallenge;
		this._enterEmailText.text = (this.hasChallenge ? this.PARENT_EMAIL_DESCRIPTION : this.VERIFY_AGE_EMAIL_DESCRIPTION);
		if (this._parentPermissionNotice)
		{
			this._parentPermissionNotice.SetActive(this.hasChallenge);
		}
		this._confirmButton.interactable = true;
		GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
		{
			EventName = "kid_age_appeal_enter_email",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string> { 
			{
				"email_type",
				this.hasChallenge ? "under_dac" : "over_dac"
			} }
		});
	}

	// Token: 0x060032AB RID: 12971 RVA: 0x000F9BB4 File Offset: 0x000F7DB4
	public void OnInputChanged(string newVal)
	{
		bool flag = !string.IsNullOrEmpty(newVal);
		if (flag)
		{
			flag = Regex.IsMatch(newVal, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
		}
		this._confirmButton.interactable = flag;
	}

	// Token: 0x060032AC RID: 12972 RVA: 0x000F9BE8 File Offset: 0x000F7DE8
	public void OnConfirmPressed()
	{
		if (string.IsNullOrEmpty(this._emailText.text))
		{
			Debug.LogError("[KID::UI::APPEAL_AGE_EMAIL] Age Appeal Email Text is empty");
			return;
		}
		this._confirmationScreen.ShowAgeAppealConfirmationScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
		base.gameObject.SetActive(false);
	}

	// Token: 0x04003958 RID: 14680
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04003959 RID: 14681
	[SerializeField]
	private KIDUI_AgeAppealEmailConfirmation _confirmationScreen;

	// Token: 0x0400395A RID: 14682
	[SerializeField]
	private TMP_Text _enterEmailText;

	// Token: 0x0400395B RID: 14683
	[SerializeField]
	private TMP_InputField _emailText;

	// Token: 0x0400395C RID: 14684
	[SerializeField]
	private GameObject _parentPermissionNotice;

	// Token: 0x0400395D RID: 14685
	private string PARENT_EMAIL_DESCRIPTION = "Enter your parent or guardian's email address below.";

	// Token: 0x0400395E RID: 14686
	private string VERIFY_AGE_EMAIL_DESCRIPTION = "Enter your email address below";

	// Token: 0x0400395F RID: 14687
	private bool hasChallenge = true;

	// Token: 0x04003960 RID: 14688
	private int newAgeToAppeal;
}
