using System;
using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using KID.Model;
using UnityEngine;

// Token: 0x0200082A RID: 2090
public class KIDUI_MainScreen : MonoBehaviour
{
	// Token: 0x0600331E RID: 13086 RVA: 0x000FBF3E File Offset: 0x000FA13E
	private void Awake()
	{
		KIDUI_MainScreen._featuresList.Clear();
		if (this._setupKidScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Setup K-ID Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._initialised)
		{
			return;
		}
		this.InitialiseMainScreen();
	}

	// Token: 0x0600331F RID: 13087 RVA: 0x000FBF77 File Offset: 0x000FA177
	private void OnEnable()
	{
		KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.UpdatePermissionsAndFeaturesScreen));
		this.UpdatePermissionsAndFeaturesScreen();
	}

	// Token: 0x06003320 RID: 13088 RVA: 0x000FBF90 File Offset: 0x000FA190
	private void OnDisable()
	{
		KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.UpdatePermissionsAndFeaturesScreen));
	}

	// Token: 0x06003321 RID: 13089 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnDestroy()
	{
	}

	// Token: 0x06003322 RID: 13090 RVA: 0x000FBFA4 File Offset: 0x000FA1A4
	private void ConstructFeatureSettings()
	{
		for (int i = 0; i < this._displayOrder.Length; i++)
		{
			for (int j = 0; j < this._featureSetups.Count; j++)
			{
				if (this._featureSetups[j].linkedFeature == this._displayOrder[i])
				{
					this.CreateNewFeatureDisplay(this._featureSetups[j]);
					break;
				}
			}
		}
		this.UpdatePermissionsAndFeaturesScreen();
	}

	// Token: 0x06003323 RID: 13091 RVA: 0x000FC010 File Offset: 0x000FA210
	private void CreateNewFeatureDisplay(KIDUI_MainScreen.FeatureToggleSetup setup)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(setup.linkedFeature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID::UI::MAIN] Failed to retrieve permission data for feature; [" + setup.linkedFeature.ToString() + "]", Array.Empty<object>());
			return;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER)
		{
			if (permissionDataByFeature.Enabled)
			{
				return;
			}
			if (KIDManager.CheckFeatureOptIn(setup.linkedFeature, null).Item2)
			{
				return;
			}
		}
		if (setup.alwaysCheckFeatureSetting && KIDManager.CheckFeatureSettingEnabled(setup.linkedFeature))
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this._featurePrefab, this._featureRootTransform);
		KIDUIFeatureSetting component = gameObject.GetComponent<KIDUIFeatureSetting>();
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
		{
			Debug.LogFormat(string.Format("[KID::UI::MAIN_SCREEN] Adding new Locked Feature:  {0} Is enabled: {1}", setup.linkedFeature.ToString(), permissionDataByFeature.Enabled), Array.Empty<object>());
			component.CreateNewFeatureSettingGuardianManaged(setup, permissionDataByFeature.Enabled);
			if (!KIDUI_MainScreen._featuresList.ContainsKey(setup.linkedFeature))
			{
				KIDUI_MainScreen._featuresList.Add(setup.linkedFeature, new List<KIDUIFeatureSetting>());
			}
			KIDUI_MainScreen._featuresList[setup.linkedFeature].Add(component);
			return;
		}
		if (setup.requiresToggle)
		{
			component.CreateNewFeatureSettingWithToggle(setup, false, setup.alwaysCheckFeatureSetting);
		}
		else
		{
			component.CreateNewFeatureSettingWithoutToggle(setup, setup.alwaysCheckFeatureSetting);
		}
		if (!KIDUI_MainScreen._featuresList.ContainsKey(setup.linkedFeature))
		{
			KIDUI_MainScreen._featuresList.Add(setup.linkedFeature, new List<KIDUIFeatureSetting>());
		}
		KIDUI_MainScreen._featuresList[setup.linkedFeature].Add(component);
		this.ConstructAdditionalSetup(setup.linkedFeature, gameObject);
	}

	// Token: 0x06003324 RID: 13092 RVA: 0x000FC1AC File Offset: 0x000FA3AC
	private void ConstructAdditionalSetup(EKIDFeatures feature, GameObject featureObject)
	{
	}

	// Token: 0x06003325 RID: 13093 RVA: 0x000FC1B4 File Offset: 0x000FA3B4
	private void UpdatePermissionsAndFeaturesScreen()
	{
		int num = 0;
		Debug.LogFormat(string.Format("[KID::UI::MAIN] Updated Feature listings. To Update: [{0}]", KIDUI_MainScreen._featuresList.Count), Array.Empty<object>());
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair in KIDUI_MainScreen._featuresList)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(keyValuePair.Key);
				if (permissionDataByFeature == null)
				{
					Debug.LogErrorFormat("[KID::UI::MAIN] Failed to find permission data for feature: [" + keyValuePair.Key.ToString() + "]", Array.Empty<object>());
				}
				else if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
				{
					keyValuePair.Value[i].SetGuardianManagedState(permissionDataByFeature.Enabled);
				}
				else
				{
					bool flag = KIDManager.CheckFeatureOptIn(keyValuePair.Key, permissionDataByFeature).Item2;
					if (keyValuePair.Value[i].AlwaysCheckFeatureSetting)
					{
						flag = KIDManager.CheckFeatureSettingEnabled(keyValuePair.Key);
					}
					keyValuePair.Value[i].SetPlayerManagedState(permissionDataByFeature.Enabled, flag);
				}
			}
		}
		int num2 = 0;
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair2 in KIDUI_MainScreen._featuresList)
		{
			for (int j = 0; j < keyValuePair2.Value.Count; j++)
			{
				num2++;
				Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(keyValuePair2.Key);
				if (keyValuePair2.Value[j].GetFeatureToggleState() || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PLAYER)
				{
					num++;
				}
			}
		}
		if (num >= num2)
		{
			this._hasAllPermissions = true;
			this._getPermissionsButton.gameObject.SetActive(false);
			this._gettingPermissionsButton.gameObject.SetActive(false);
			this._requestPermissionsButton.gameObject.SetActive(false);
			this._permissionsTip.SetActive(false);
			this.SetButtonContainersVisibility(EGetPermissionsStatus.RequestedPermission);
		}
	}

	// Token: 0x06003326 RID: 13094 RVA: 0x000FC3EC File Offset: 0x000FA5EC
	private bool IsFeatureToggledOn(EKIDFeatures permissionFeature)
	{
		List<KIDUIFeatureSetting> list;
		if (!KIDUI_MainScreen._featuresList.TryGetValue(permissionFeature, out list))
		{
			return true;
		}
		KIDUIFeatureSetting kiduifeatureSetting = list.FirstOrDefault<KIDUIFeatureSetting>();
		if (kiduifeatureSetting == null)
		{
			Debug.LogErrorFormat(string.Format("[KID::UI::MAIN] Empty list for permission Name [{0}]", permissionFeature), Array.Empty<object>());
			return false;
		}
		return kiduifeatureSetting.GetFeatureToggleState();
	}

	// Token: 0x06003327 RID: 13095 RVA: 0x000FC43C File Offset: 0x000FA63C
	public void InitialiseMainScreen()
	{
		if (this._initialised)
		{
			Debug.Log("[KID::MAIN_SCREEN] Already Initialised");
			return;
		}
		this.ConstructFeatureSettings();
		this._declinedStatus.SetActive(false);
		this._timeoutStatus.SetActive(false);
		this._pendingStatus.SetActive(false);
		this._updatedStatus.SetActive(false);
		this._setupRequiredStatus.SetActive(false);
		this._initialised = true;
	}

	// Token: 0x06003328 RID: 13096 RVA: 0x000FC4A8 File Offset: 0x000FA6A8
	public void ShowMainScreen(EMainScreenStatus showStatus, KIDUI_Controller.Metrics_ShowReason reason)
	{
		this.ShowMainScreen(showStatus);
		this._mainScreenOpenedReason = reason;
		string text = reason.ToString().Replace("_", "-").ToLower();
		KIDTelemetryData kidtelemetryData = new KIDTelemetryData
		{
			EventName = "kid_game_settings",
			CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment,
				KIDTelemetry.Open_MetricActionCustomTag
			},
			BodyData = new Dictionary<string, string> { { "screen_shown_reason", text } }
		};
		foreach (Permission permission in KIDManager.GetAllPermissionsData())
		{
			kidtelemetryData.BodyData.Add(KIDTelemetry.GetPermissionManagedByBodyData(permission.Name), permission.ManagedBy.ToString().ToLower());
			kidtelemetryData.BodyData.Add(KIDTelemetry.GetPermissionEnabledBodyData(permission.Name), permission.Enabled.ToString().ToLower());
		}
		GorillaTelemetry.SendMothershipAnalytics(kidtelemetryData);
	}

	// Token: 0x06003329 RID: 13097 RVA: 0x000FC5E4 File Offset: 0x000FA7E4
	public void ShowMainScreen(EMainScreenStatus showStatus)
	{
		KIDUI_MainScreen.ShownSettingsScreen = true;
		base.gameObject.SetActive(true);
		this.ConfigurePermissionsButtons();
		this.UpdateScreenStatus(showStatus, false);
	}

	// Token: 0x0600332A RID: 13098 RVA: 0x000FC608 File Offset: 0x000FA808
	public void UpdateScreenStatus(EMainScreenStatus showStatus, bool sendMetrics = false)
	{
		if (sendMetrics && showStatus == EMainScreenStatus.Updated)
		{
			string text = this._mainScreenOpenedReason.ToString().Replace("_", "-").ToLower();
			KIDTelemetryData kidtelemetryData = new KIDTelemetryData
			{
				EventName = "kid_game_settings",
				CustomTags = new string[]
				{
					"kid_setup",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment,
					KIDTelemetry.Updated_MetricActionCustomTag
				},
				BodyData = new Dictionary<string, string> { { "screen_shown_reason", text } }
			};
			foreach (Permission permission in KIDManager.GetAllPermissionsData())
			{
				kidtelemetryData.BodyData.Add(KIDTelemetry.GetPermissionManagedByBodyData(permission.Name), permission.ManagedBy.ToString().ToLower());
				kidtelemetryData.BodyData.Add(KIDTelemetry.GetPermissionEnabledBodyData(permission.Name), permission.Enabled.ToString().ToLower());
			}
			GorillaTelemetry.SendMothershipAnalytics(kidtelemetryData);
		}
		GameObject activeStatusObject = this.GetActiveStatusObject();
		this._declinedStatus.SetActive(false);
		this._timeoutStatus.SetActive(false);
		this._pendingStatus.SetActive(false);
		this._updatedStatus.SetActive(false);
		this._setupRequiredStatus.SetActive(false);
		switch (showStatus)
		{
		default:
			this._updatedStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Declined:
			this._declinedStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Pending:
			this._pendingStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Timedout:
			this._timeoutStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Setup:
			this._setupRequiredStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Previous:
			if (activeStatusObject != null)
			{
				activeStatusObject.SetActive(true);
			}
			else
			{
				this._updatedStatus.SetActive(true);
			}
			break;
		}
		this.SetButtonContainersVisibility(KIDUI_MainScreen.GetPermissionState());
	}

	// Token: 0x0600332B RID: 13099 RVA: 0x0001F6FF File Offset: 0x0001D8FF
	public void HideMainScreen()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600332C RID: 13100 RVA: 0x000FC840 File Offset: 0x000FAA40
	public void OnAskForPermission()
	{
		base.gameObject.SetActive(false);
		if (KIDManager.CurrentSession.IsDefault)
		{
			this._setupKidScreen.OnStartSetup();
			return;
		}
		List<string> list = new List<string>(this.CollectPermissionsToUpgrade());
		this._sendUpgradeEmailScreen.SendUpgradeEmail(list);
	}

	// Token: 0x0600332D RID: 13101 RVA: 0x000FC88C File Offset: 0x000FAA8C
	public void OnSaveAndExit()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::KID_UI_MAINSCREEN] There is no session as such cannot opt into anything");
			KIDUI_Controller.Instance.CloseKIDScreens();
			return;
		}
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		for (int i = 0; i < allPermissionsData.Count; i++)
		{
			string name = allPermissionsData[i].Name;
			if (!(name == "multiplayer"))
			{
				if (!(name == "mods"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "voice-chat"))
						{
							if (!(name == "custom-username"))
							{
								Debug.LogError("[KID::UI::MainScreen] Unhandled permission when saving and exiting: [" + allPermissionsData[i].Name + "]");
							}
							else
							{
								this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Custom_Nametags, delegate(bool b, Permission p, bool hasOptedInPreviously)
								{
									GorillaComputer.instance.SetNametagSetting(b, p.ManagedBy, hasOptedInPreviously);
								});
							}
						}
						else
						{
							this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Voice_Chat, delegate(bool b, Permission p, bool hasOptedInPreviously)
							{
								GorillaComputer.instance.KID_SetVoiceChatSettingOnStart(b, p.ManagedBy, hasOptedInPreviously);
							});
						}
					}
				}
				else
				{
					this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Mods, null);
				}
			}
			else
			{
				this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Multiplayer, null);
			}
		}
		if (this._screenStatus != EMainScreenStatus.None)
		{
			string text = this._mainScreenOpenedReason.ToString().Replace("_", "-").ToLower();
			GorillaTelemetry.SendMothershipAnalytics(new KIDTelemetryData
			{
				EventName = "kid_game_settings",
				CustomTags = new string[]
				{
					"kid_setup",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string>
				{
					{ "screen_shown_reason", text },
					{
						"kid_status",
						this._screenStatus.ToString().ToLower()
					},
					{ "button_pressed", "save_and_continue" }
				}
			});
		}
		else
		{
			Debug.LogError("[KID::UI::MAIN_SCREEN] Trying to close k-ID Main Screen, but screen status is set to [None] - Invalid status, will not submit analytics");
		}
		KIDUI_Controller.Instance.CloseKIDScreens();
	}

	// Token: 0x0600332E RID: 13102 RVA: 0x000FCA9C File Offset: 0x000FAC9C
	public int GetFeatureListingCount()
	{
		int num = 0;
		foreach (List<KIDUIFeatureSetting> list in KIDUI_MainScreen._featuresList.Values)
		{
			num += list.Count;
		}
		return num;
	}

	// Token: 0x0600332F RID: 13103 RVA: 0x000FCAF8 File Offset: 0x000FACF8
	private void UpdateOptInSetting(Permission permissionData, EKIDFeatures feature, Action<bool, Permission, bool> onOptedIn)
	{
		bool item = KIDManager.CheckFeatureOptIn(feature, permissionData).Item2;
		bool flag = this.IsFeatureToggledOn(feature);
		Debug.Log(string.Format("[KID::UI::MainScreen] Update opt in for {0}. Has opted in: {1}. Toggled on: {2}", feature.ToString(), item, flag));
		KIDManager.SetFeatureOptIn(feature, flag);
		if (onOptedIn != null)
		{
			onOptedIn(flag, permissionData, item);
		}
	}

	// Token: 0x06003330 RID: 13104 RVA: 0x000FCB55 File Offset: 0x000FAD55
	public void OnConfirmedEmailAddress(string emailAddress)
	{
		this._emailAddress = emailAddress;
		Debug.LogFormat("[KID::UI::Main] Email has been confirmed: " + this._emailAddress, Array.Empty<object>());
	}

	// Token: 0x06003331 RID: 13105 RVA: 0x000FCB78 File Offset: 0x000FAD78
	private IEnumerable<string> CollectPermissionsToUpgrade()
	{
		return from permission in KIDManager.GetAllPermissionsData()
			where permission.ManagedBy == Permission.ManagedByEnum.GUARDIAN && !permission.Enabled
			select permission.Name;
	}

	// Token: 0x06003332 RID: 13106 RVA: 0x000FCBD4 File Offset: 0x000FADD4
	private void ConfigurePermissionsButtons()
	{
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS");
		if (!this._getPermissionsButton.gameObject.activeSelf && !this._gettingPermissionsButton.gameObject.activeSelf)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - GET PERMISSIONS IS DISABLED");
			return;
		}
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - CHECK SESSION STATUS: Is Default: [" + KIDManager.CurrentSession.IsDefault.ToString() + "]");
		this.SetButtonContainersVisibility(KIDUI_MainScreen.GetPermissionState());
	}

	// Token: 0x06003333 RID: 13107 RVA: 0x000FCC48 File Offset: 0x000FAE48
	private void SetButtonContainersVisibility(EGetPermissionsStatus permissionStatus)
	{
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - PERMISSION STATE: [" + permissionStatus.ToString() + "]");
		this._defaultButtonsContainer.SetActive(permissionStatus == EGetPermissionsStatus.GetPermission);
		this._permissionsRequestingButtonContainer.SetActive(permissionStatus == EGetPermissionsStatus.RequestingPermission);
		this._permissionsRequestedButtonContainer.SetActive(permissionStatus == EGetPermissionsStatus.RequestedPermission);
	}

	// Token: 0x06003334 RID: 13108 RVA: 0x000FCCA4 File Offset: 0x000FAEA4
	private GameObject GetActiveStatusObject()
	{
		foreach (GameObject gameObject in new List<GameObject> { this._declinedStatus, this._timeoutStatus, this._pendingStatus, this._updatedStatus, this._setupRequiredStatus })
		{
			if (gameObject.activeInHierarchy)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x06003335 RID: 13109 RVA: 0x000FCD3C File Offset: 0x000FAF3C
	private static EGetPermissionsStatus GetPermissionState()
	{
		if (!KIDManager.CurrentSession.IsDefault)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW REQUESTED");
			return EGetPermissionsStatus.RequestedPermission;
		}
		if (PlayerPrefs.GetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 0) == 0)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW DEFAULT");
			return EGetPermissionsStatus.GetPermission;
		}
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW SWAPPED DEFAULT");
		return EGetPermissionsStatus.RequestingPermission;
	}

	// Token: 0x06003336 RID: 13110 RVA: 0x000FCD7C File Offset: 0x000FAF7C
	private void OnFeatureToggleChanged(EKIDFeatures feature)
	{
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
			this.OnMultiplayerToggled();
			return;
		case EKIDFeatures.Custom_Nametags:
			this.OnCustomNametagsToggled();
			return;
		case EKIDFeatures.Voice_Chat:
			this.OnVoiceChatToggled();
			return;
		case EKIDFeatures.Mods:
			this.OnModToggleChanged();
			return;
		case EKIDFeatures.Groups:
			this.OnGroupToggleChanged();
			return;
		default:
			Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] Toggle NOT YET IMPLEMENTED for Feature: " + feature.ToString() + ".", Array.Empty<object>());
			return;
		}
	}

	// Token: 0x06003337 RID: 13111 RVA: 0x000FCDEE File Offset: 0x000FAFEE
	private void OnMultiplayerToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] MULTIPLAYER Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06003338 RID: 13112 RVA: 0x000FCDFF File Offset: 0x000FAFFF
	private void OnVoiceChatToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] VOICE CHAT Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06003339 RID: 13113 RVA: 0x000FCE10 File Offset: 0x000FB010
	private void OnGroupToggleChanged()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] GROUPS Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x0600333A RID: 13114 RVA: 0x000FCE21 File Offset: 0x000FB021
	private void OnModToggleChanged()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] MODS Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x0600333B RID: 13115 RVA: 0x000FCE32 File Offset: 0x000FB032
	private void OnCustomNametagsToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] CUSTOM USERNAMES Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x040039EE RID: 14830
	public const string OPT_IN_SUFFIX = "-opt-in";

	// Token: 0x040039EF RID: 14831
	public static bool ShownSettingsScreen = false;

	// Token: 0x040039F0 RID: 14832
	[SerializeField]
	private GameObject _kidScreensGroup;

	// Token: 0x040039F1 RID: 14833
	[SerializeField]
	private KIDUI_SetupScreen _setupKidScreen;

	// Token: 0x040039F2 RID: 14834
	[SerializeField]
	private KIDUI_SendUpgradeEmailScreen _sendUpgradeEmailScreen;

	// Token: 0x040039F3 RID: 14835
	[Header("Permission Request Buttons")]
	[SerializeField]
	private KIDUIButton _getPermissionsButton;

	// Token: 0x040039F4 RID: 14836
	[SerializeField]
	private KIDUIButton _gettingPermissionsButton;

	// Token: 0x040039F5 RID: 14837
	[SerializeField]
	private KIDUIButton _requestPermissionsButton;

	// Token: 0x040039F6 RID: 14838
	[SerializeField]
	private GameObject _defaultButtonsContainer;

	// Token: 0x040039F7 RID: 14839
	[SerializeField]
	private GameObject _permissionsRequestingButtonContainer;

	// Token: 0x040039F8 RID: 14840
	[SerializeField]
	private GameObject _permissionsRequestedButtonContainer;

	// Token: 0x040039F9 RID: 14841
	private bool _hasAllPermissions;

	// Token: 0x040039FA RID: 14842
	[Header("Dynamic Feature Settings Setup")]
	[SerializeField]
	private GameObject _featurePrefab;

	// Token: 0x040039FB RID: 14843
	[SerializeField]
	private Transform _featureRootTransform;

	// Token: 0x040039FC RID: 14844
	[SerializeField]
	private EKIDFeatures[] _displayOrder = new EKIDFeatures[4];

	// Token: 0x040039FD RID: 14845
	[SerializeField]
	private List<KIDUI_MainScreen.FeatureToggleSetup> _featureSetups = new List<KIDUI_MainScreen.FeatureToggleSetup>();

	// Token: 0x040039FE RID: 14846
	[Header("Additional Feature-Specific Setup")]
	[SerializeField]
	private GameObject _voiceChatLabel;

	// Token: 0x040039FF RID: 14847
	[Header("Hide Permissions Tip")]
	[SerializeField]
	private GameObject _permissionsTip;

	// Token: 0x04003A00 RID: 14848
	[Header("Game Status Setup")]
	[SerializeField]
	private GameObject _updatedStatus;

	// Token: 0x04003A01 RID: 14849
	[SerializeField]
	private GameObject _declinedStatus;

	// Token: 0x04003A02 RID: 14850
	[SerializeField]
	private GameObject _pendingStatus;

	// Token: 0x04003A03 RID: 14851
	[SerializeField]
	private GameObject _timeoutStatus;

	// Token: 0x04003A04 RID: 14852
	[SerializeField]
	private GameObject _setupRequiredStatus;

	// Token: 0x04003A05 RID: 14853
	private string _emailAddress;

	// Token: 0x04003A06 RID: 14854
	private bool _multiplayerEnabled;

	// Token: 0x04003A07 RID: 14855
	private bool _customNameEnabled;

	// Token: 0x04003A08 RID: 14856
	private bool _voiceChatEnabled;

	// Token: 0x04003A09 RID: 14857
	private bool _initialised;

	// Token: 0x04003A0A RID: 14858
	private KIDUI_Controller.Metrics_ShowReason _mainScreenOpenedReason;

	// Token: 0x04003A0B RID: 14859
	private EMainScreenStatus _screenStatus;

	// Token: 0x04003A0C RID: 14860
	private GameObject _eventSystemObj;

	// Token: 0x04003A0D RID: 14861
	private static Dictionary<EKIDFeatures, List<KIDUIFeatureSetting>> _featuresList = new Dictionary<EKIDFeatures, List<KIDUIFeatureSetting>>();

	// Token: 0x0200082B RID: 2091
	[Serializable]
	public struct FeatureToggleSetup
	{
		// Token: 0x04003A0E RID: 14862
		public EKIDFeatures linkedFeature;

		// Token: 0x04003A0F RID: 14863
		public string permissionName;

		// Token: 0x04003A10 RID: 14864
		public string featureName;

		// Token: 0x04003A11 RID: 14865
		public bool requiresToggle;

		// Token: 0x04003A12 RID: 14866
		public bool alwaysCheckFeatureSetting;

		// Token: 0x04003A13 RID: 14867
		public string enabledText;

		// Token: 0x04003A14 RID: 14868
		public string disabledText;
	}
}
