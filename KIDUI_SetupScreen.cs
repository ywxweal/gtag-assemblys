using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// Token: 0x02000831 RID: 2097
public class KIDUI_SetupScreen : MonoBehaviour
{
	// Token: 0x06003352 RID: 13138 RVA: 0x000FD2D0 File Offset: 0x000FB4D0
	private void Awake()
	{
		if (this._emailInputField == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Email Input Field is NULL", Array.Empty<object>());
			return;
		}
		if (this._confirmScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Confirm Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._mainScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Main Screen is NULL", Array.Empty<object>());
			return;
		}
	}

	// Token: 0x06003353 RID: 13139 RVA: 0x000FD338 File Offset: 0x000FB538
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "");
		this._emailInputField.text = @string;
		this._confirmButton.interactable = !string.IsNullOrEmpty(@string);
	}

	// Token: 0x06003354 RID: 13140 RVA: 0x000FD375 File Offset: 0x000FB575
	private void OnDisable()
	{
		if (this._keyboard == null)
		{
			return;
		}
		this._keyboard.active = false;
	}

	// Token: 0x06003355 RID: 13141 RVA: 0x000FD38C File Offset: 0x000FB58C
	public void OnStartSetup()
	{
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
			BodyData = new Dictionary<string, string> { { "screen", "enter_email" } }
		});
	}

	// Token: 0x06003356 RID: 13142 RVA: 0x000FD400 File Offset: 0x000FB600
	public void OnInputSelected()
	{
		Debug.LogFormat("[KID::UI::SETUP] Email Input Selected!", Array.Empty<object>());
	}

	// Token: 0x06003357 RID: 13143 RVA: 0x000FD414 File Offset: 0x000FB614
	public void OnInputChanged(string newVal)
	{
		bool flag = !string.IsNullOrEmpty(newVal);
		if (flag)
		{
			flag = Regex.IsMatch(newVal, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
		}
		this._confirmButton.interactable = flag;
	}

	// Token: 0x06003358 RID: 13144 RVA: 0x000FD446 File Offset: 0x000FB646
	public void OnSubmitEmailPressed()
	{
		PlayerPrefs.SetString(KIDManager.GetEmailForUserPlayerPrefRef, this._emailInputField.text);
		PlayerPrefs.Save();
		base.gameObject.SetActive(false);
		this._confirmScreen.OnEmailSubmitted(this._emailInputField.text);
	}

	// Token: 0x06003359 RID: 13145 RVA: 0x000FD484 File Offset: 0x000FB684
	public void OnBackPressed()
	{
		PlayerPrefs.SetString(KIDManager.GetEmailForUserPlayerPrefRef, this._emailInputField.text);
		PlayerPrefs.Save();
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Previous);
	}

	// Token: 0x04003A29 RID: 14889
	[SerializeField]
	private TMP_InputField _emailInputField;

	// Token: 0x04003A2A RID: 14890
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04003A2B RID: 14891
	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	// Token: 0x04003A2C RID: 14892
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04003A2D RID: 14893
	private string _emailStr = string.Empty;

	// Token: 0x04003A2E RID: 14894
	private TouchScreenKeyboard _keyboard;
}
