using System;
using System.Collections.Generic;
using KID.Model;
using TMPro;
using UnityEngine;

// Token: 0x020007B2 RID: 1970
public class KIDAgeAppeal : MonoBehaviour
{
	// Token: 0x060030C6 RID: 12486 RVA: 0x000EFCD0 File Offset: 0x000EDED0
	public void ShowAgeAppealScreen()
	{
		this._ageSlider = base.GetComponentInChildren<AgeSliderWithProgressBar>(true);
		this._ageSlider.ControllerActive = true;
		base.gameObject.SetActive(true);
		this._inputsContainer.SetActive(true);
		this._monkeLoader.SetActive(false);
	}

	// Token: 0x060030C7 RID: 12487 RVA: 0x000EFD10 File Offset: 0x000EDF10
	public async void OnNewAgeConfirmed()
	{
		this._inputsContainer.SetActive(false);
		this._monkeLoader.SetActive(true);
		AgeStatusType ageStatusType;
		if (KIDManager.TryGetAgeStatusTypeFromAge(this._ageSlider.CurrentAge, out ageStatusType))
		{
			GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
			{
				EventName = "kid_age_appeal_age_gate",
				CustomTags = new string[]
				{
					"kid_age_appeal",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string> { 
				{
					"correct_age",
					ageStatusType.ToString()
				} }
			});
		}
		AttemptAgeUpdateData attemptAgeUpdateData = await KIDManager.TryAttemptAgeUpdate(this._ageSlider.CurrentAge);
		if (attemptAgeUpdateData.status == SessionStatus.PROHIBITED)
		{
			Debug.LogError("[KID::AGE-APPEAL] Age Appeal Status: PROHIBITED");
			base.gameObject.SetActive(false);
			KIDUI_AgeAppealController.Instance.StartTooYoungToPlayScreen();
		}
		else
		{
			this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(attemptAgeUpdateData.status == SessionStatus.CHALLENGE, this._ageSlider.CurrentAge);
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04003722 RID: 14114
	[SerializeField]
	private TMP_Text _ageText;

	// Token: 0x04003723 RID: 14115
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x04003724 RID: 14116
	[SerializeField]
	private GameObject _inputsContainer;

	// Token: 0x04003725 RID: 14117
	[SerializeField]
	private GameObject _monkeLoader;

	// Token: 0x04003726 RID: 14118
	private AgeSliderWithProgressBar _ageSlider;
}
