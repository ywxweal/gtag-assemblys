using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

// Token: 0x020007FD RID: 2045
public class KIDMessagingController : MonoBehaviour
{
	// Token: 0x17000516 RID: 1302
	// (get) Token: 0x06003224 RID: 12836 RVA: 0x000F7A6F File Offset: 0x000F5C6F
	private static string HasShownConfirmationScreenPlayerPref
	{
		get
		{
			return "hasShownKIDConfirmationScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06003225 RID: 12837 RVA: 0x000F7A87 File Offset: 0x000F5C87
	public void OnConfirmPressed()
	{
		this._closeMessageBox = true;
	}

	// Token: 0x06003226 RID: 12838 RVA: 0x000F7A90 File Offset: 0x000F5C90
	private void Awake()
	{
		if (KIDMessagingController.instance != null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] Trying to start a new [KIDMessagingController] but one already exists");
			Object.Destroy(this);
			return;
		}
		KIDMessagingController.instance = this;
	}

	// Token: 0x06003227 RID: 12839 RVA: 0x000F7AB6 File Offset: 0x000F5CB6
	private bool ShouldShowConfirmationScreen()
	{
		return !KIDManager.CurrentSession.IsDefault && PlayerPrefs.GetInt(KIDMessagingController.HasShownConfirmationScreenPlayerPref, 0) != 1;
	}

	// Token: 0x06003228 RID: 12840 RVA: 0x000F7AD8 File Offset: 0x000F5CD8
	private async Task StartKIDConfirmationScreenInternal(CancellationToken token)
	{
		if (this._confirmationMessageBox == null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] Trying to show confirmation screen but [_confirmationMessageBox] is null");
		}
		else
		{
			string text = await KIDMessagingController.GetSetupConfirmationMessage();
			if (string.IsNullOrEmpty(text))
			{
				text = "k-ID setup is now complete. Thanks and have fun in Gorilla World!";
			}
			this._confirmationMessageBox.Header = "Thank you";
			this._confirmationMessageBox.Body = text;
			this._confirmationMessageBox.LeftButton = string.Empty;
			this._confirmationMessageBox.RightButton = "Continue";
			this._confirmationMessageBox.gameObject.SetActive(true);
			HandRayController.Instance.EnableHandRays();
			PrivateUIRoom.AddUI(base.transform);
			while (!token.IsCancellationRequested)
			{
				await Task.Yield();
				if (this._closeMessageBox)
				{
					PrivateUIRoom.RemoveUI(base.transform);
					HandRayController.Instance.DisableHandRays();
					this._confirmationMessageBox.gameObject.SetActive(false);
					PlayerPrefs.SetInt(KIDMessagingController.HasShownConfirmationScreenPlayerPref, 1);
					PlayerPrefs.Save();
					break;
				}
			}
		}
	}

	// Token: 0x06003229 RID: 12841 RVA: 0x000F7B24 File Offset: 0x000F5D24
	public static async Task StartKIDConfirmationScreen(CancellationToken token)
	{
		KIDMessagingController kidmessagingController = KIDMessagingController.instance;
		if (kidmessagingController == null || kidmessagingController.ShouldShowConfirmationScreen())
		{
			await KIDMessagingController.instance.StartKIDConfirmationScreenInternal(token);
			GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
			{
				EventName = "kid_screen_shown",
				CustomTags = new string[]
				{
					"kid_setup",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string>
				{
					{ "screen", "setup_complete" },
					{
						"saw_game_settings",
						KIDUI_MainScreen.ShownSettingsScreen.ToString().ToLower() ?? ""
					}
				}
			});
		}
	}

	// Token: 0x0600322A RID: 12842 RVA: 0x000F7B68 File Offset: 0x000F5D68
	private static async Task<string> GetSetupConfirmationMessage()
	{
		int state = 0;
		string bodyText = string.Empty;
		PlayFabTitleDataCache.Instance.GetTitleData("KIDData", delegate(string res)
		{
			state = 1;
			bodyText = KIDMessagingController.GetConfirmMessageFromTitleDataJson(res);
		}, delegate(PlayFabError err)
		{
			state = -1;
			Debug.LogError("[KID_MANAGER] Something went wrong trying to get title data for key: [KIDData]. Error:\n" + err.ErrorMessage);
		});
		do
		{
			await Task.Yield();
		}
		while (state == 0);
		return bodyText;
	}

	// Token: 0x0600322B RID: 12843 RVA: 0x000F7BA4 File Offset: 0x000F5DA4
	private static string GetConfirmMessageFromTitleDataJson(string jsonTxt)
	{
		if (string.IsNullOrEmpty(jsonTxt))
		{
			Debug.LogError("[KID_MANAGER] Cannot get Confirmation Message. JSON is null or empty!");
			return null;
		}
		KIDMessagingTitleData kidmessagingTitleData = JsonConvert.DeserializeObject<KIDMessagingTitleData>(jsonTxt);
		if (kidmessagingTitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		if (string.IsNullOrEmpty(kidmessagingTitleData.KIDSetupConfirmation))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData] - [KIDSetupConfirmation] is null or empty. Json: \n" + jsonTxt);
			return null;
		}
		return kidmessagingTitleData.KIDSetupConfirmation;
	}

	// Token: 0x040038D8 RID: 14552
	private const string SHOWN_CONFIRMATION_SCREEN_PREFIX = "hasShownKIDConfirmationScreen-";

	// Token: 0x040038D9 RID: 14553
	private const string CONFIRMATION_HEADER = "Thank you";

	// Token: 0x040038DA RID: 14554
	private const string CONFIRMATION_BODY = "k-ID setup is now complete. Thanks and have fun in Gorilla World!";

	// Token: 0x040038DB RID: 14555
	private const string CONFIRMATION_BUTTON = "Continue";

	// Token: 0x040038DC RID: 14556
	private static KIDMessagingController instance;

	// Token: 0x040038DD RID: 14557
	[SerializeField]
	private MessageBox _confirmationMessageBox;

	// Token: 0x040038DE RID: 14558
	private bool _closeMessageBox;
}
