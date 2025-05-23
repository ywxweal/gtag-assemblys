using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020007C1 RID: 1985
public class KIDManager : MonoBehaviour
{
	// Token: 0x170004F7 RID: 1271
	// (get) Token: 0x06003101 RID: 12545 RVA: 0x000F0E4C File Offset: 0x000EF04C
	public static KIDManager Instance
	{
		get
		{
			return KIDManager._instance;
		}
	}

	// Token: 0x170004F8 RID: 1272
	// (get) Token: 0x06003102 RID: 12546 RVA: 0x000F0E53 File Offset: 0x000EF053
	// (set) Token: 0x06003103 RID: 12547 RVA: 0x000F0E5A File Offset: 0x000EF05A
	public static bool InitialisationComplete { get; private set; } = false;

	// Token: 0x170004F9 RID: 1273
	// (get) Token: 0x06003104 RID: 12548 RVA: 0x000F0E62 File Offset: 0x000EF062
	// (set) Token: 0x06003105 RID: 12549 RVA: 0x000F0E69 File Offset: 0x000EF069
	public static bool InitialisationSuccessful { get; private set; } = false;

	// Token: 0x170004FA RID: 1274
	// (get) Token: 0x06003106 RID: 12550 RVA: 0x000F0E71 File Offset: 0x000EF071
	// (set) Token: 0x06003107 RID: 12551 RVA: 0x000F0E78 File Offset: 0x000EF078
	public static TMPSession CurrentSession { get; private set; }

	// Token: 0x170004FB RID: 1275
	// (get) Token: 0x06003108 RID: 12552 RVA: 0x000F0E80 File Offset: 0x000EF080
	// (set) Token: 0x06003109 RID: 12553 RVA: 0x000F0E87 File Offset: 0x000EF087
	public static SessionStatus PreviousStatus { get; private set; }

	// Token: 0x170004FC RID: 1276
	// (get) Token: 0x0600310A RID: 12554 RVA: 0x000F0E8F File Offset: 0x000EF08F
	// (set) Token: 0x0600310B RID: 12555 RVA: 0x000F0E96 File Offset: 0x000EF096
	public static GetRequirementsData _ageGateRequirements { get; private set; }

	// Token: 0x170004FD RID: 1277
	// (get) Token: 0x0600310C RID: 12556 RVA: 0x000F0E9E File Offset: 0x000EF09E
	public static bool KidTitleDataReady
	{
		get
		{
			return KIDManager._titleDataReady;
		}
	}

	// Token: 0x170004FE RID: 1278
	// (get) Token: 0x0600310D RID: 12557 RVA: 0x000F0EA5 File Offset: 0x000EF0A5
	public static bool KidEnabled
	{
		get
		{
			return KIDManager.KidTitleDataReady && KIDManager._useKid;
		}
	}

	// Token: 0x170004FF RID: 1279
	// (get) Token: 0x0600310E RID: 12558 RVA: 0x000F0EB5 File Offset: 0x000EF0B5
	public static bool KidEnabledAndReady
	{
		get
		{
			return KIDManager.KidEnabled && KIDManager.InitialisationSuccessful;
		}
	}

	// Token: 0x17000500 RID: 1280
	// (get) Token: 0x0600310F RID: 12559 RVA: 0x000F0EC5 File Offset: 0x000EF0C5
	public static bool HasSession
	{
		get
		{
			return KIDManager.CurrentSession != null && KIDManager.CurrentSession.SessionId != Guid.Empty;
		}
	}

	// Token: 0x17000501 RID: 1281
	// (get) Token: 0x06003110 RID: 12560 RVA: 0x000F0EE4 File Offset: 0x000EF0E4
	public static string PreviousStatusPlayerPrefRef
	{
		get
		{
			return "previous-status-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x17000502 RID: 1282
	// (get) Token: 0x06003111 RID: 12561 RVA: 0x000F0EFC File Offset: 0x000EF0FC
	// (set) Token: 0x06003112 RID: 12562 RVA: 0x000F0F03 File Offset: 0x000EF103
	public static bool HasOptedInToKID { get; private set; }

	// Token: 0x17000503 RID: 1283
	// (get) Token: 0x06003113 RID: 12563 RVA: 0x000F0F0B File Offset: 0x000EF10B
	private static string KIDSetupPlayerPref
	{
		get
		{
			return "KID-Setup-";
		}
	}

	// Token: 0x17000504 RID: 1284
	// (get) Token: 0x06003114 RID: 12564 RVA: 0x000F0F12 File Offset: 0x000EF112
	// (set) Token: 0x06003115 RID: 12565 RVA: 0x000F0F19 File Offset: 0x000EF119
	public static string DbgLocale { get; set; }

	// Token: 0x17000505 RID: 1285
	// (get) Token: 0x06003116 RID: 12566 RVA: 0x000F0F21 File Offset: 0x000EF121
	public static string DebugKIDLocalePlayerPrefRef
	{
		get
		{
			return KIDManager._debugKIDLocalePlayerPrefRef;
		}
	}

	// Token: 0x17000506 RID: 1286
	// (get) Token: 0x06003117 RID: 12567 RVA: 0x000F0F28 File Offset: 0x000EF128
	public static string GetEmailForUserPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDManager.parentEmailForUserPlayerPrefRef))
			{
				KIDManager.parentEmailForUserPlayerPrefRef = "k-id_EmailAddress" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDManager.parentEmailForUserPlayerPrefRef;
		}
	}

	// Token: 0x17000507 RID: 1287
	// (get) Token: 0x06003118 RID: 12568 RVA: 0x000F0F56 File Offset: 0x000EF156
	public static string GetChallengedBeforePlayerPrefRef
	{
		get
		{
			return "k-id_ChallengedBefore" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06003119 RID: 12569 RVA: 0x000F0F70 File Offset: 0x000EF170
	private void Awake()
	{
		if (KIDManager._instance != null)
		{
			Debug.LogError("Trying to create new instance of [KIDManager], but one already exists. Destroying object [" + base.gameObject.name + "].");
			Object.Destroy(base.gameObject);
			return;
		}
		Debug.Log("[KID] INIT");
		KIDManager._instance = this;
		KIDManager.DbgLocale = PlayerPrefs.GetString(KIDManager._debugKIDLocalePlayerPrefRef, "");
	}

	// Token: 0x0600311A RID: 12570 RVA: 0x000F0FDC File Offset: 0x000EF1DC
	private async void Start()
	{
		TaskAwaiter<bool> taskAwaiter = KIDManager.UseKID().GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<bool> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<bool>);
		}
		KIDManager._useKid = taskAwaiter.GetResult();
		TaskAwaiter<int> taskAwaiter3 = KIDManager.CheckKIDPhase().GetAwaiter();
		if (!taskAwaiter3.IsCompleted)
		{
			await taskAwaiter3;
			TaskAwaiter<int> taskAwaiter4;
			taskAwaiter3 = taskAwaiter4;
			taskAwaiter4 = default(TaskAwaiter<int>);
		}
		KIDManager._kIDPhase = taskAwaiter3.GetResult();
		TaskAwaiter<DateTime?> taskAwaiter5 = KIDManager.CheckKIDNewPlayerDateTime().GetAwaiter();
		if (!taskAwaiter5.IsCompleted)
		{
			await taskAwaiter5;
			TaskAwaiter<DateTime?> taskAwaiter6;
			taskAwaiter5 = taskAwaiter6;
			taskAwaiter6 = default(TaskAwaiter<DateTime?>);
		}
		KIDManager._kIDNewPlayerDateTime = taskAwaiter5.GetResult();
		KIDManager._titleDataReady = true;
	}

	// Token: 0x0600311B RID: 12571 RVA: 0x000F100B File Offset: 0x000EF20B
	private void OnDestroy()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	// Token: 0x0600311C RID: 12572 RVA: 0x000F1017 File Offset: 0x000EF217
	public static AgeStatusType GetActiveAccountStatus()
	{
		if (KIDManager.CurrentSession != null)
		{
			return KIDManager.CurrentSession.AgeStatus;
		}
		if (!PlayFabAuthenticator.instance.GetSafety())
		{
			return AgeStatusType.LEGALADULT;
		}
		return AgeStatusType.DIGITALMINOR;
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x000F103C File Offset: 0x000EF23C
	public static List<Permission> GetAllPermissionsData()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::MANAGER] There is no current session. Unless the age-gate has not yet finished there should always be a session even if it is the default session");
			return new List<Permission>();
		}
		return KIDManager.CurrentSession.GetAllPermissions();
	}

	// Token: 0x0600311E RID: 12574 RVA: 0x000F1060 File Offset: 0x000EF260
	public static bool TryGetAgeStatusTypeFromAge(int age, out AgeStatusType ageType)
	{
		if (KIDManager._ageGateRequirements == null)
		{
			Debug.LogError("[KID::MANAGER] [_ageGateRequirements] is not set - need to Get AgeGate Requirements first");
			ageType = AgeStatusType.DIGITALMINOR;
			return false;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.DigitalConsentAge)
		{
			ageType = AgeStatusType.DIGITALMINOR;
			return true;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.CivilAge)
		{
			ageType = AgeStatusType.DIGITALYOUTH;
			return true;
		}
		ageType = AgeStatusType.LEGALADULT;
		return true;
	}

	// Token: 0x0600311F RID: 12575 RVA: 0x000F10B8 File Offset: 0x000EF2B8
	[return: TupleElementNames(new string[] { "requiresOptIn", "hasOptedInPreviously" })]
	public static ValueTuple<bool, bool> CheckFeatureOptIn(EKIDFeatures feature, Permission permissionData = null)
	{
		if (permissionData == null)
		{
			permissionData = KIDManager.GetPermissionDataByFeature(feature);
			if (permissionData == null)
			{
				Debug.LogError("[KID::MANAGER] Unable to retrieve permission data for feature [" + feature.ToStandardisedString() + "]");
				return new ValueTuple<bool, bool>(false, false);
			}
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		int @int = PlayerPrefs.GetInt(KIDManager.GetOptInKey(feature), 0);
		if (permissionData.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
		{
			return new ValueTuple<bool, bool>(false, @int == 1);
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.PLAYER && permissionData.Enabled)
		{
			return new ValueTuple<bool, bool>(false, true);
		}
		return new ValueTuple<bool, bool>(true, @int == 1);
	}

	// Token: 0x06003120 RID: 12576 RVA: 0x000F114C File Offset: 0x000EF34C
	public static void SetFeatureOptIn(EKIDFeatures feature, bool optedIn)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID] Trying to set Feature Opt in for feature [" + feature.ToStandardisedString() + "] but permission data could not be found. Assumed is opt-in", Array.Empty<object>());
			return;
		}
		string optInKey = KIDManager.GetOptInKey(feature);
		switch (permissionDataByFeature.ManagedBy)
		{
		case Permission.ManagedByEnum.PLAYER:
			PlayerPrefs.SetInt(optInKey, optedIn ? 1 : 0);
			break;
		case Permission.ManagedByEnum.GUARDIAN:
			PlayerPrefs.SetInt(optInKey, permissionDataByFeature.Enabled ? 1 : 0);
			break;
		case Permission.ManagedByEnum.PROHIBITED:
			PlayerPrefs.SetInt(optInKey, 0);
			break;
		}
		PlayerPrefs.Save();
	}

	// Token: 0x06003121 RID: 12577 RVA: 0x000F11D8 File Offset: 0x000EF3D8
	public static bool CheckFeatureSettingEnabled(EKIDFeatures feature)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogError("[KID::MANAGER] Unable to permissions for feature [" + feature.ToStandardisedString() + "]");
			return false;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return false;
		}
		bool item = KIDManager.CheckFeatureOptIn(feature, null).Item2;
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
		case EKIDFeatures.Mods:
			return item;
		case EKIDFeatures.Custom_Nametags:
			return item && GorillaComputer.instance.NametagsEnabled;
		case EKIDFeatures.Voice_Chat:
			return item && GorillaComputer.instance.CheckVoiceChatEnabled();
		case EKIDFeatures.Groups:
			return permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.GUARDIAN || permissionDataByFeature.Enabled;
		default:
			Debug.LogError("[KID::MANAGER] Tried finding feature setting for [" + feature.ToStandardisedString() + "] but failed.");
			return false;
		}
	}

	// Token: 0x06003122 RID: 12578 RVA: 0x000F1294 File Offset: 0x000EF494
	private static async Task<GetPlayerData_Data> TryGetPlayerData(bool forceRefresh)
	{
		return await KIDManager.Server_GetPlayerData(forceRefresh, null);
	}

	// Token: 0x06003123 RID: 12579 RVA: 0x000F12D8 File Offset: 0x000EF4D8
	private static async Task<GetRequirementsData> TryGetRequirements()
	{
		return await KIDManager.Server_GetRequirements();
	}

	// Token: 0x06003124 RID: 12580 RVA: 0x000F1314 File Offset: 0x000EF514
	private static async Task<VerifyAgeData> TryVerifyAgeResponse()
	{
		PlayerPlatform playerPlatform = PlayerPlatform.Steam;
		VerifyAgeRequest verifyAgeRequest = new VerifyAgeRequest();
		verifyAgeRequest.Age = new int?(KIDAgeGate.UserAge);
		verifyAgeRequest.Platform = new PlayerPlatform?(playerPlatform);
		Debug.Log(string.Format("[KID::MANAGER] Sending verify age request for age: [{0}]", KIDAgeGate.UserAge));
		return await KIDManager.Server_VerifyAge(verifyAgeRequest, null);
	}

	// Token: 0x06003125 RID: 12581 RVA: 0x000F1350 File Offset: 0x000EF550
	[return: TupleElementNames(new string[] { "success", "exception" })]
	private static async Task<ValueTuple<bool, string>> TrySendChallengeEmailRequest(Action onFailure)
	{
		do
		{
			await Task.Yield();
		}
		while (string.IsNullOrEmpty(KIDManager._emailAddress));
		ValueTuple<bool, string> valueTuple;
		if (!KIDManager.CanSendEmail())
		{
			string text = "[KID::MANAGER] Unable to send challenge email";
			Debug.LogError(text);
			valueTuple = new ValueTuple<bool, string>(false, text);
		}
		else
		{
			SendChallengeEmailRequest sendChallengeEmailRequest = new SendChallengeEmailRequest();
			sendChallengeEmailRequest.Email = KIDManager._emailAddress;
			sendChallengeEmailRequest.Locale = (string.IsNullOrEmpty(KIDManager.DbgLocale) ? CultureInfo.CurrentCulture.Name : KIDManager.DbgLocale);
			string message = "";
			bool flag = await KIDManager.Server_SendChallengeEmail(sendChallengeEmailRequest, onFailure);
			if (flag)
			{
				KIDManager.OnEmailResultReceived onEmailResultReceived = KIDManager.onEmailResultReceived;
				if (onEmailResultReceived != null)
				{
					onEmailResultReceived(true);
				}
			}
			else
			{
				message = "Failed to send challenge email";
				KIDManager.OnEmailResultReceived onEmailResultReceived2 = KIDManager.onEmailResultReceived;
				if (onEmailResultReceived2 != null)
				{
					onEmailResultReceived2(false);
				}
			}
			valueTuple = new ValueTuple<bool, string>(flag, message);
		}
		return valueTuple;
	}

	// Token: 0x06003126 RID: 12582 RVA: 0x000F1394 File Offset: 0x000EF594
	public static async Task<bool> TrySendUpgradeSessionChallengeEmail()
	{
		return await KIDManager.Server_SendChallengeEmail(new SendChallengeEmailRequest(), null);
	}

	// Token: 0x06003127 RID: 12583 RVA: 0x000F13D0 File Offset: 0x000EF5D0
	public static async Task<UpgradeSessionData> TryUpgradeSession(List<string> requestedPermissions)
	{
		global::UpgradeSessionRequest upgradeSessionRequest = new global::UpgradeSessionRequest();
		upgradeSessionRequest.Permissions = requestedPermissions.Select((string name) => new RequestedPermission(name)).ToList<RequestedPermission>();
		UpgradeSessionData upgradeSessionData = await KIDManager.Server_UpgradeSession(upgradeSessionRequest);
		KIDManager.UpdatePermissions(upgradeSessionData.session);
		return upgradeSessionData;
	}

	// Token: 0x06003128 RID: 12584 RVA: 0x000F1414 File Offset: 0x000EF614
	public static async Task<AttemptAgeUpdateData> TryAttemptAgeUpdate(int age)
	{
		PlayerPlatform playerPlatform = PlayerPlatform.Steam;
		AttemptAgeUpdateRequest attemptAgeUpdateRequest = new AttemptAgeUpdateRequest();
		attemptAgeUpdateRequest.Age = age;
		attemptAgeUpdateRequest.Platform = playerPlatform;
		Debug.Log(string.Format("[KID::MANAGER] Sending age update request for age: [{0}]", age));
		return await KIDManager.Server_AttemptAgeUpdate(attemptAgeUpdateRequest, null);
	}

	// Token: 0x06003129 RID: 12585 RVA: 0x000F1458 File Offset: 0x000EF658
	public static async Task<bool> TryAppealAge(string email, int newAge)
	{
		string text = (string.IsNullOrEmpty(KIDManager.DbgLocale) ? CultureInfo.CurrentCulture.Name : KIDManager.DbgLocale);
		AppealAgeRequest appealAgeRequest = new AppealAgeRequest();
		appealAgeRequest.Age = newAge;
		appealAgeRequest.Email = email;
		appealAgeRequest.Locale = text;
		Debug.Log(string.Format("[KID::MANAGER] Sending age appeal request for age: [{0}] at email [{1}]", newAge, email));
		return await KIDManager.Server_AppealAge(appealAgeRequest, null);
	}

	// Token: 0x0600312A RID: 12586 RVA: 0x000F14A4 File Offset: 0x000EF6A4
	public static async Task UpdateSession()
	{
		GetPlayerData_Data getPlayerData_Data = await KIDManager.TryGetPlayerData(true);
		if (getPlayerData_Data == null)
		{
			Debug.LogError("[KID::MANAGER] Failed to retrieve session");
		}
		else if (getPlayerData_Data.responseType == GetSessionResponseType.ERROR)
		{
			Debug.LogError("[KID::MANAGER] Failed to get session. Resulted in error. Cannot update session");
		}
		else
		{
			KIDManager.UpdatePermissions(getPlayerData_Data.session);
		}
	}

	// Token: 0x0600312B RID: 12587 RVA: 0x000F14E0 File Offset: 0x000EF6E0
	private static async Task<bool> CheckWarningScreensOptedIn()
	{
		bool flag;
		if (GorillaServer.Instance.CheckOptedInKID())
		{
			Debug.Log("[KID::MANAGER] PHASE ONE (A) -- IN PROGRESS - User has already opted in to k-ID, skipping warning screens");
			flag = true;
		}
		else
		{
			Debug.Log("[KID::MANAGER] CHECK WARNING SCREENS - Force Starting Overlay");
			PrivateUIRoom.ForceStartOverlay();
			WarningButtonResult warningButtonResult = await WarningScreens.StartWarningScreen(KIDManager._requestCancellationSource.Token);
			if (warningButtonResult == WarningButtonResult.None)
			{
				if (KIDManager._requestCancellationSource.IsCancellationRequested)
				{
					flag = false;
				}
				else
				{
					Debug.Log("[KID::MANAGER] PHASE ONE (A) -- IN PROGRESS - User not shown any warning screen and has not opted in yet. Is Eligible: [" + (GorillaServer.Instance.CheckIsInKIDOptInCohort() | GorillaServer.Instance.CheckIsInKIDRequiredCohort()).ToString() + "].");
					flag = false;
				}
			}
			else if (warningButtonResult == WarningButtonResult.CloseWarning)
			{
				if (KIDManager._requestCancellationSource.IsCancellationRequested)
				{
					flag = false;
				}
				else
				{
					Debug.Log("[KID::MANAGER] PHASE ONE (A) -- IN PROGRESS - User cancelled the warning screen. Skipping k-ID Opt-in.");
					flag = false;
				}
			}
			else
			{
				if (warningButtonResult == WarningButtonResult.OptIn)
				{
					Debug.Log("[KID::MANAGER] PHASE ONE (A) -- IN PROGRESS - User has newly opted in to k-ID");
					TaskAwaiter<bool> taskAwaiter = KIDManager.Server_OptIn().GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<bool> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<bool>);
					}
					if (!taskAwaiter.GetResult())
					{
						Debug.LogError("[KID::MANAGER] PHASE ONE (A) -- FAILURE - Opting in to k-ID failed!");
						return false;
					}
					if (CosmeticsController.instance != null)
					{
						CosmeticsController.instance.GetCurrencyBalance();
					}
					await WarningScreens.StartOptInFollowUpScreen(KIDManager._requestCancellationSource.Token);
				}
				flag = true;
			}
		}
		return flag;
	}

	// Token: 0x0600312C RID: 12588 RVA: 0x000F151B File Offset: 0x000EF71B
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void InitialiseBootFlow()
	{
		Debug.Log("[KID::MANAGER] PHASE ZERO -- START -- Checking K-ID Flag");
		PlayerPrefs.GetInt(KIDManager.KIDSetupPlayerPref, 0);
	}

	// Token: 0x0600312D RID: 12589 RVA: 0x000F1534 File Offset: 0x000EF734
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static async void InitialiseKID()
	{
		bool snapTurnDisabled = false;
		float? cachedTapHapticsStrength = null;
		object obj = null;
		int num = 0;
		try
		{
			Debug.Log("[KID::MANAGER] PHASE ZERO -- START - Initialising k-ID System");
			TaskAwaiter<bool> taskAwaiter = KIDManager.WaitForAuthentication().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			bool flag = !taskAwaiter.GetResult();
			UGCPermissionManager.UsePlayFabSafety();
			if (flag)
			{
				Debug.Log("[KID::MANAGER] Wait for auth failed. Skipping age gate.");
			}
			else if (!KIDManager._useKid)
			{
				Debug.Log("[KID::MANAGER] Kid disabled. Skipping age gate.");
			}
			else
			{
				Debug.Log("[KID::MANAGER] PlayFab has logged in, starting k-ID initialisation flow");
				Debug.Log("[KID::MANAGER] PHASE ZERO -- COMPLETE");
				GorillaSnapTurn.DisableSnapTurn();
				snapTurnDisabled = true;
				if (GorillaTagger.Instance != null)
				{
					cachedTapHapticsStrength = new float?(GorillaTagger.Instance.tapHapticStrength);
					GorillaTagger.Instance.tapHapticStrength = 0f;
				}
				Debug.Log("[KID::MANAGER] PHASE ONE -- START - Initialising k-ID System");
				GetPlayerData_Data newSessionData = await KIDManager.TryGetPlayerData(true);
				if (!KIDManager._requestCancellationSource.IsCancellationRequested)
				{
					if (newSessionData == null)
					{
						Debug.LogError("[KID::MANAGER] [newSessionData] returned NULL. Something went wrong, we should always get a [GetPlayerData_Data]. Disabling k-ID");
					}
					else if (newSessionData.responseType == GetSessionResponseType.ERROR)
					{
						Debug.LogError("[KID::MANAGER] Failed to retrieve Player Data, response type: [" + newSessionData.responseType.ToString() + "]. Unable to proceed. Will default to Using Safeties");
						Debug.Log(string.Format("[KID::MANAGER] Safeties is: [{0}", PlayFabAuthenticator.instance.GetSafety()));
					}
					else
					{
						KIDManager.HasOptedInToKID = newSessionData.responseType != GetSessionResponseType.NOT_FOUND;
						bool flag2 = await KIDManager.CheckWarningScreensOptedIn();
						if (!KIDManager._requestCancellationSource.IsCancellationRequested)
						{
							if (!flag2)
							{
								Debug.Log("[KID] Kid Not opted into. Aborting k-ID setup.");
							}
							else
							{
								Debug.Log("[KID::MANAGER] PHASE ONE -- COMPLETE");
								Debug.Log("[KID::MANAGER] PHASE TWO -- START - Get Player Data from Server");
								KIDManager.PreviousStatus = (SessionStatus)PlayerPrefs.GetInt(KIDManager.PreviousStatusPlayerPrefRef, 0);
								TMPSession newSession = newSessionData.session;
								TMPSession session = newSessionData.session;
								AgeStatusType ageStatus = ((session != null) ? session.AgeStatus : AgeStatusType.DIGITALMINOR);
								Debug.Log("[KID::MANAGER] PHASE TWO -- IN PROGRESS - Getting Age-Gate Configuration Data");
								KIDManager._ageGateRequirements = await KIDManager.TryGetRequirements();
								KIDAgeGate.SetAgeGateConfig(KIDManager._ageGateRequirements);
								string text = ((KIDManager._ageGateRequirements == null) ? "UNSUCCESSFULLY [_ageGateRequirements] is null" : ((KIDManager._ageGateRequirements.AgeGateRequirements == null) ? "UNSUCCESSFULLY [AgeGateRequirements] is NULL" : "SUCCESSFULLY"));
								Debug.Log("[KID::MANAGER] PHASE TWO -- IN PROGRESS - Age-Gate configuration Completed: " + text);
								SessionStatus? sessionStatus = newSessionData.status;
								SessionStatus sessionStatus2 = SessionStatus.PROHIBITED;
								if (!((sessionStatus.GetValueOrDefault() == sessionStatus2) & (sessionStatus != null)))
								{
									sessionStatus = newSessionData.status;
									sessionStatus2 = SessionStatus.PENDING_AGE_APPEAL;
									if (!((sessionStatus.GetValueOrDefault() == sessionStatus2) & (sessionStatus != null)))
									{
										Debug.Log("[KID::MANAGER] PHASE TWO -- COMPLETE");
										Debug.Log("[KID::MANAGER] PHASE THREE -- START - Check for Age-Gate");
										TMPSession session2 = newSessionData.session;
										bool flag3;
										if (session2 == null)
										{
											flag3 = true;
										}
										else
										{
											AgeStatusType ageStatus2 = session2.AgeStatus;
											flag3 = false;
										}
										if (flag3)
										{
											PrivateUIRoom.ForceStartOverlay();
											Debug.Log("[KID::MANAGER] PHASE THREE -- IN PROGRESS - Age-gate required");
											ValueTuple<AgeStatusType, TMPSession> valueTuple = await KIDManager.AgeGateFlow(newSessionData);
											AgeStatusType item = valueTuple.Item1;
											TMPSession item2 = valueTuple.Item2;
											if (KIDManager._requestCancellationSource.IsCancellationRequested)
											{
												goto IL_0897;
											}
											ageStatus = item;
											newSession = item2;
										}
										Debug.Log("[KID::MANAGER] PHASE THREE -- COMPLETE");
										Debug.Log("[KID::MANAGER] PHASE FOUR -- START - Legal Agreements Processes");
										if (LegalAgreements.instance != null)
										{
											Debug.Log("[KID::MANAGER] Start legal agreements");
											await LegalAgreements.instance.StartLegalAgreements();
											if (KIDManager._requestCancellationSource.IsCancellationRequested)
											{
												goto IL_0897;
											}
										}
										Debug.Log("[KID::MANAGER] PHASE FOUR -- COMPLETE");
										Debug.Log("[KID::MANAGER] PHASE FIVE -- START - Update Permissions");
										if (!KIDManager.UpdatePermissions(newSession))
										{
											string text2 = "[KID::MANAGER] PHASE FIVE -- FAILURE - Failed to update permissions but will continue.\nSession was:\n";
											TMPSession tmpsession = newSession;
											Debug.LogError(text2 + ((tmpsession != null) ? tmpsession.ToString() : null));
											Debug.Log(string.Format("[KID::MANAGER] Safeties is: [{0}", PlayFabAuthenticator.instance.GetSafety()));
											goto IL_0897;
										}
										if (KIDManager.CurrentSession == null)
										{
											Debug.LogError("[KID::MANAGER] PHASE FIVE -- FAILURE -- CurrentSession is NULL, should at least have a default session!");
											Debug.Log(string.Format("[KID::MANAGER] Safeties is: [{0}", PlayFabAuthenticator.instance.GetSafety()));
											goto IL_0897;
										}
										if (KIDManager.CurrentSession.IsDefault)
										{
											KIDManager.WaitForAndUpdateNewSession(true);
										}
										if (KIDManager._requestCancellationSource.IsCancellationRequested)
										{
											goto IL_0897;
										}
										UGCPermissionManager.UseKID();
										Debug.Log("[KID::MANAGER] PHASE FIVE -- COMPLETE");
										Debug.Log("[KID::MANAGER] PHASE SIX -- START - Check for K-ID Screens");
										if (ageStatus != AgeStatusType.LEGALADULT)
										{
											PrivateUIRoom.ForceStartOverlay();
											Debug.Log("[KID::MANAGER] PHASE FIVE -- IN PROGRESS - Not an Adult, K-ID Screen checks required");
											await KIDUI_Controller.Instance.StartKIDScreens(KIDManager._requestCancellationSource.Token);
											while (!KIDManager._requestCancellationSource.IsCancellationRequested)
											{
												await Task.Yield();
												if (!KIDUI_Controller.IsKIDUIActive)
												{
													goto IL_07AD;
												}
											}
											goto IL_0897;
										}
										IL_07AD:
										Debug.Log("[KID::MANAGER] PHASE SIX --  COMPLETE");
										if (KIDManager._requestCancellationSource.IsCancellationRequested)
										{
											goto IL_0897;
										}
										Debug.Log("[KID::MANAGER] PHASE SEVEN -- START - Finalise setup");
										if (KIDManager.CurrentSession == null)
										{
											Debug.LogError("[KID::MANAGER] PHASE SEVEN -- FAILURE -- CurrentSession is NULL, should at least have a default session!");
											Debug.Log(string.Format("[KID::MANAGER] Safeties is: [{0}", PlayFabAuthenticator.instance.GetSafety()));
											goto IL_0897;
										}
										await KIDMessagingController.StartKIDConfirmationScreen(KIDManager._requestCancellationSource.Token);
										PlayerPrefs.SetInt(KIDManager.PreviousStatusPlayerPrefRef, (int)KIDManager.PreviousStatus);
										PlayerPrefs.Save();
										KIDManager.InitialisationSuccessful = true;
										newSessionData = null;
										newSession = null;
										goto IL_08AC;
									}
								}
								PrivateUIRoom.ForceStartOverlay();
								Debug.Log("[KID::MANAGER] User is [" + newSessionData.status.ToString() + "] from playing Gorilla Tag. Skipping to Age-Appeal flow");
								KIDUI_AgeAppealController.Instance.StartAgeAppealScreens(newSessionData.status);
							}
						}
					}
				}
			}
			IL_0897:
			num = 1;
		}
		catch (object obj)
		{
		}
		IL_08AC:
		KIDManager.InitialisationComplete = true;
		if (!KIDManager.InitialisationSuccessful)
		{
			Debug.Log("[KID::MANAGER] k-ID Initialisation has FAILED.");
			if (cachedTapHapticsStrength != null)
			{
				Debug.Log("[KID::MANAGER] Enable back haptics when we're done with the k-ID setup");
				GorillaTagger.Instance.tapHapticStrength = cachedTapHapticsStrength.Value;
			}
			if (snapTurnDisabled)
			{
				Debug.Log("[KID::MANAGER] Reverting Snap Turning to PlayerPref settings");
				GorillaSnapTurn.LoadSettingsFromCache();
			}
			if (LegalAgreements.instance != null)
			{
				Debug.Log("[KID::MANAGER] Start legal agreements");
				await LegalAgreements.instance.StartLegalAgreements();
			}
			Debug.Log("[KID::MANAGER] Stop forced overlay");
			PrivateUIRoom.StopForcedOverlay();
		}
		object obj2 = obj;
		if (obj2 != null)
		{
			Exception ex = obj2 as Exception;
			if (ex == null)
			{
				throw obj2;
			}
			ExceptionDispatchInfo.Capture(ex).Throw();
		}
		if (num != 1)
		{
			obj = null;
			UGCPermissionManager.UseKID();
			bool flag4 = KIDManager.CurrentSession == null && PlayFabAuthenticator.instance.GetSafety();
			Debug.Log(string.Format("[KID::MANAGER] Safeties enabled status: [{0}", flag4));
			if (cachedTapHapticsStrength != null)
			{
				Debug.Log("[KID::MANAGER] Enable back haptics when we're done with the k-ID setup");
				GorillaTagger.Instance.tapHapticStrength = cachedTapHapticsStrength.Value;
			}
			if (snapTurnDisabled)
			{
				Debug.Log("[KID::MANAGER] Reverting Snap Turning to PlayerPref settings");
				GorillaSnapTurn.LoadSettingsFromCache();
			}
			Debug.Log("[KID::MANAGER] Stop forced overlay");
			PrivateUIRoom.StopForcedOverlay();
			Debug.Log("[KID::MANAGER] PHASE SEVEN -- COMPLETE");
			Debug.Log("[KID::MANAGER] K-ID Has been Initialised and is ready!");
		}
	}

	// Token: 0x0600312E RID: 12590 RVA: 0x000F1564 File Offset: 0x000EF764
	private static bool UpdatePermissions(TMPSession newSession)
	{
		Debug.Log("[KID::MANAGER] Updating Permissions to reflect session.");
		if (newSession == null || !newSession.IsValidSession)
		{
			Debug.LogError("[KID::MANAGER] A NULL or Invalid Session was received!");
			return false;
		}
		KIDManager.SaveStoredPermissions();
		KIDManager.CurrentSession = newSession;
		if (KIDUI_Controller.IsKIDUIActive)
		{
			KIDManager.PreviousStatus = KIDManager.CurrentSession.SessionStatus;
			PlayerPrefs.SetInt(KIDManager.PreviousStatusPlayerPrefRef, (int)KIDManager.PreviousStatus);
			PlayerPrefs.Save();
		}
		if (!KIDManager.CurrentSession.IsDefault)
		{
			PlayerPrefs.SetInt(KIDManager.KIDSetupPlayerPref, 1);
			PlayerPrefs.Save();
		}
		if (KIDUI_Controller.Instance)
		{
			KIDUI_Controller.Instance.UpdateScreenStatus();
		}
		KIDManager.OnSessionUpdated();
		return true;
	}

	// Token: 0x0600312F RID: 12591 RVA: 0x000F15FF File Offset: 0x000EF7FF
	private static void ClearSession()
	{
		KIDManager.CurrentSession = null;
		KIDManager.DeleteStoredPermissions();
	}

	// Token: 0x06003130 RID: 12592 RVA: 0x000023F4 File Offset: 0x000005F4
	private static void SaveStoredPermissions()
	{
	}

	// Token: 0x06003131 RID: 12593 RVA: 0x000023F4 File Offset: 0x000005F4
	private static void DeleteStoredPermissions()
	{
	}

	// Token: 0x06003132 RID: 12594 RVA: 0x000F160C File Offset: 0x000EF80C
	public static CancellationTokenSource ResetCancellationToken()
	{
		KIDManager._requestCancellationSource.Dispose();
		KIDManager._requestCancellationSource = new CancellationTokenSource();
		return KIDManager._requestCancellationSource;
	}

	// Token: 0x06003133 RID: 12595 RVA: 0x000F1628 File Offset: 0x000EF828
	public static Permission GetPermissionDataByFeature(EKIDFeatures feature)
	{
		if (KIDManager.CurrentSession == null)
		{
			if (!PlayFabAuthenticator.instance.GetSafety())
			{
				return new Permission(feature.ToStandardisedString(), true, Permission.ManagedByEnum.PLAYER);
			}
			return new Permission(feature.ToStandardisedString(), false, Permission.ManagedByEnum.GUARDIAN);
		}
		else
		{
			Permission permission;
			if (!KIDManager.CurrentSession.TryGetPermission(feature, out permission))
			{
				Debug.LogError("[KID::MANAGER] Failed to retreive permission from session for [" + feature.ToStandardisedString() + "]. Assuming disabled permission");
				return new Permission(feature.ToStandardisedString(), false, Permission.ManagedByEnum.GUARDIAN);
			}
			return permission;
		}
	}

	// Token: 0x06003134 RID: 12596 RVA: 0x000F100B File Offset: 0x000EF20B
	public static void CancelToken()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	// Token: 0x06003135 RID: 12597 RVA: 0x000F16A4 File Offset: 0x000EF8A4
	public static async Task<bool> UseKID()
	{
		Debug.Log("[KID::MANAGER] K-ID CURRENTLY DISABLED");
		return false;
		IL_0021:
		await Task.Yield();
		int state;
		if (state == 0)
		{
			goto IL_0021;
		}
		bool isEnabled;
		if (!(PlayFabAuthenticator.instance.postAuthSetSafety & isEnabled))
		{
			goto IL_00A8;
		}
		PlayFabAuthenticator.instance.DefaultSafetiesByAgeCategory();
		IL_00A8:
		return isEnabled;
	}

	// Token: 0x06003136 RID: 12598 RVA: 0x000F16E0 File Offset: 0x000EF8E0
	public static async Task<int> CheckKIDPhase()
	{
		int num;
		if (KIDManager._titleDataReady)
		{
			num = KIDManager._kIDPhase;
		}
		else
		{
			int state = 0;
			int phase = 0;
			PlayFabTitleDataCache.Instance.GetTitleData("KIDData", delegate(string res)
			{
				state = 1;
				phase = KIDManager.GetPhase(res);
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
			num = phase;
		}
		return num;
	}

	// Token: 0x06003137 RID: 12599 RVA: 0x000F171C File Offset: 0x000EF91C
	public static async Task<DateTime?> CheckKIDNewPlayerDateTime()
	{
		DateTime? dateTime;
		if (KIDManager._titleDataReady)
		{
			dateTime = KIDManager._kIDNewPlayerDateTime;
		}
		else
		{
			int state = 0;
			DateTime? newPlayerDateTime = null;
			PlayFabTitleDataCache.Instance.GetTitleData("KIDData", delegate(string res)
			{
				state = 1;
				newPlayerDateTime = KIDManager.GetNewPlayerDateTime(res);
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
			dateTime = newPlayerDateTime;
		}
		return dateTime;
	}

	// Token: 0x06003138 RID: 12600 RVA: 0x000F1758 File Offset: 0x000EF958
	private static bool GetIsEnabled(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return false;
		}
		bool flag;
		if (!bool.TryParse(kidtitleData.KIDEnabled, out flag))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDEnabled': [KIDEnabled] to bool.");
			return false;
		}
		return flag;
	}

	// Token: 0x06003139 RID: 12601 RVA: 0x000F17A0 File Offset: 0x000EF9A0
	private static int GetPhase(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return 0;
		}
		return kidtitleData.KIDPhase;
	}

	// Token: 0x0600313A RID: 12602 RVA: 0x000F17D0 File Offset: 0x000EF9D0
	private static DateTime? GetNewPlayerDateTime(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		DateTime dateTime;
		if (!DateTime.TryParse(kidtitleData.KIDNewPlayerIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDNewPlayerIsoTimestamp': [KIDNewPlayerIsoTimestamp] to DateTime.");
			return null;
		}
		return new DateTime?(dateTime);
	}

	// Token: 0x0600313B RID: 12603 RVA: 0x000F1834 File Offset: 0x000EFA34
	public static bool IsAdult()
	{
		return KIDManager.CurrentSession.IsValidSession && KIDManager.CurrentSession.AgeStatus == AgeStatusType.LEGALADULT;
	}

	// Token: 0x0600313C RID: 12604 RVA: 0x000F1854 File Offset: 0x000EFA54
	public static bool HasAllPermissions()
	{
		List<Permission> allPermissions = KIDManager.CurrentSession.GetAllPermissions();
		for (int i = 0; i < allPermissions.Count; i++)
		{
			if (allPermissions[i].ManagedBy == Permission.ManagedByEnum.GUARDIAN || !allPermissions[i].Enabled)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600313D RID: 12605 RVA: 0x000F18A0 File Offset: 0x000EFAA0
	public static async Task<bool> SetKIDOptIn()
	{
		return await KIDManager.Server_OptIn();
	}

	// Token: 0x0600313E RID: 12606 RVA: 0x000F18DC File Offset: 0x000EFADC
	[return: TupleElementNames(new string[] { "success", "message" })]
	public static async Task<ValueTuple<bool, string>> SetAndSendEmail(string email, Action onFailure)
	{
		KIDManager._emailAddress = email;
		return await KIDManager.TrySendChallengeEmailRequest(onFailure);
	}

	// Token: 0x0600313F RID: 12607 RVA: 0x000F1928 File Offset: 0x000EFB28
	public static bool HasPermissionToUseFeature(EKIDFeatures feature)
	{
		if (!KIDManager.KidEnabledAndReady)
		{
			return !PlayFabAuthenticator.instance.GetSafety();
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		return (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
	}

	// Token: 0x06003140 RID: 12608 RVA: 0x000F1974 File Offset: 0x000EFB74
	private static async Task<bool> WaitForAuthentication()
	{
		Debug.Log("[KID] Starting Age-Gate process.");
		while (!PlayFabClientAPI.IsClientLoggedIn())
		{
			bool flag;
			if (KIDManager._requestCancellationSource.IsCancellationRequested)
			{
				flag = false;
			}
			else
			{
				if (!PlayFabAuthenticator.instance || !PlayFabAuthenticator.instance.loginFailed)
				{
					await Task.Yield();
					continue;
				}
				flag = false;
			}
			return flag;
		}
		Debug.Log("[KID] Initialisation - PlayFab has signed in. Continuing.");
		while (!GorillaServer.Instance.FeatureFlagsReady)
		{
			if (KIDManager._requestCancellationSource.IsCancellationRequested)
			{
				return false;
			}
			await Task.Yield();
		}
		Debug.Log("[KID] Initialisation - Feature Flags ready. Continuing.");
		while (!KIDManager._titleDataReady)
		{
			if (KIDManager._requestCancellationSource.IsCancellationRequested)
			{
				return false;
			}
			await Task.Yield();
		}
		Debug.Log("[KID] Initialisation - K-ID Title Data loaded in. Continuing.");
		return true;
	}

	// Token: 0x06003141 RID: 12609 RVA: 0x000F19B0 File Offset: 0x000EFBB0
	[return: TupleElementNames(new string[] { "ageStatus", "resp" })]
	private static async Task<ValueTuple<AgeStatusType, TMPSession>> AgeGateFlow(GetPlayerData_Data newPlayerData)
	{
		TMPSession tmpsession = newPlayerData.session;
		TMPSession session = newPlayerData.session;
		AgeStatusType? ageStatusType = ((session != null) ? new AgeStatusType?(session.AgeStatus) : null);
		if (newPlayerData.AgeStatus == null)
		{
			Debug.Log("[KID::MANAGER] PHASE THREE (A) -- IN PROGRESS - Age not set, must process age gate");
			VerifyAgeData verifyAgeData = await KIDManager.ProcessAgeGate();
			Debug.Log("[KID::MANAGER] PHASE THREE (A) -- IN PROGRESS - Age Gate Completed");
			if (verifyAgeData == null)
			{
				Debug.Log("[KID::MANAGER] Verify Response returned NULL, this could happen if a Prohibited response was received and the age-gate exited, or the game shut down");
				return new ValueTuple<AgeStatusType, TMPSession>(AgeStatusType.DIGITALMINOR, null);
			}
			tmpsession = verifyAgeData.Session;
			ageStatusType = new AgeStatusType?(tmpsession.AgeStatus);
			if (tmpsession.IsDefault)
			{
				Debug.Log("[KID::MANAGER] PHASE THREE (A) -- IN PROGRESS - Age Gate completed - Default session received");
			}
		}
		if (ageStatusType == null)
		{
			Debug.LogError("[KID::MANAGER] PHASE THREE (A) -- FAILURE - Age Gate completed, but age status is null. Defaulting to MINOR");
			ageStatusType = new AgeStatusType?(AgeStatusType.DIGITALMINOR);
		}
		return new ValueTuple<AgeStatusType, TMPSession>(ageStatusType.Value, tmpsession);
	}

	// Token: 0x06003142 RID: 12610 RVA: 0x000F19F4 File Offset: 0x000EFBF4
	private static async Task<VerifyAgeData> ProcessAgeGate()
	{
		Debug.Log("[KID::MANAGER] PHASE THREE (B) -- IN PROGRESS - Beginning Age-Gate");
		await KIDAgeGate.BeginAgeGate();
		VerifyAgeData verifyAgeData;
		if (KIDManager._requestCancellationSource.IsCancellationRequested)
		{
			verifyAgeData = null;
		}
		else
		{
			Debug.Log("[KID::MANAGER] PHASE THREE (B) -- IN PROGRESS - Age-Gate completed");
			Debug.Log("[KID::MANAGER] PHASE THREE (C) -- IN PROGRESS - Trying to verify Age Response");
			VerifyAgeData verifyResponse = await KIDManager.TryVerifyAgeResponse();
			if (KIDManager._requestCancellationSource.IsCancellationRequested)
			{
				verifyAgeData = null;
			}
			else
			{
				Debug.Log("[KID::MANAGER] PHASE THREE (C) -- IN PROGRESS - Verify Age Response completed");
				if (verifyResponse.Status == SessionStatus.PROHIBITED || verifyResponse.Status == SessionStatus.PENDING_AGE_APPEAL)
				{
					KIDUI_AgeAppealController.Instance.StartAgeAppealScreens(new SessionStatus?(verifyResponse.Status));
					GetPlayerData_Data getPlayerData_Data = await KIDManager.TryGetPlayerData(true);
					for (;;)
					{
						SessionStatus? sessionStatus = getPlayerData_Data.status;
						SessionStatus sessionStatus2 = SessionStatus.PROHIBITED;
						if (!((sessionStatus.GetValueOrDefault() == sessionStatus2) & (sessionStatus != null)))
						{
							sessionStatus = getPlayerData_Data.status;
							sessionStatus2 = SessionStatus.PENDING_AGE_APPEAL;
							if (!((sessionStatus.GetValueOrDefault() == sessionStatus2) & (sessionStatus != null)))
							{
								break;
							}
						}
						await Task.Delay(30000);
						getPlayerData_Data = await KIDManager.TryGetPlayerData(true);
					}
					verifyAgeData = verifyResponse;
				}
				else
				{
					verifyAgeData = verifyResponse;
				}
			}
		}
		return verifyAgeData;
	}

	// Token: 0x06003143 RID: 12611 RVA: 0x000F1A2F File Offset: 0x000EFC2F
	public static string GetOptInKey(EKIDFeatures feature)
	{
		return feature.ToStandardisedString() + "-opt-in-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
	}

	// Token: 0x06003144 RID: 12612 RVA: 0x000F1A4D File Offset: 0x000EFC4D
	private static bool CanSendEmail()
	{
		Debug.LogError("[KID::MANAGER] TEMP: For now will do cooldown checks on the client. But eventually should move to server");
		return true;
	}

	// Token: 0x06003145 RID: 12613 RVA: 0x000F1A5C File Offset: 0x000EFC5C
	private static async Task<GetPlayerData_Data> Server_GetPlayerData(bool forceRefresh, Action failureCallback)
	{
		string text = string.Format("sessionRefresh={0}", forceRefresh ? "true" : "false");
		ValueTuple<long, GetPlayerDataResponse> valueTuple = await KIDManager.KIDServerWebRequest<GetPlayerDataResponse, KIDRequestData>("GetPlayerData", "GET", null, text, null);
		long item = valueTuple.Item1;
		GetPlayerDataResponse item2 = valueTuple.Item2;
		GetSessionResponseType getSessionResponseType = GetSessionResponseType.ERROR;
		if (item != 200L)
		{
			if (item != 204L)
			{
				if (item == 404L)
				{
					getSessionResponseType = GetSessionResponseType.LOST;
				}
			}
			else
			{
				getSessionResponseType = GetSessionResponseType.NOT_FOUND;
			}
		}
		else
		{
			getSessionResponseType = GetSessionResponseType.OK;
		}
		GetPlayerData_Data getPlayerData_Data = new GetPlayerData_Data(getSessionResponseType, item2);
		if (item < 200L || item >= 300L)
		{
			if (failureCallback != null)
			{
				failureCallback();
			}
		}
		return getPlayerData_Data;
	}

	// Token: 0x06003146 RID: 12614 RVA: 0x000F1AA8 File Offset: 0x000EFCA8
	private static async Task<UpgradeSessionData> Server_UpgradeSession(global::UpgradeSessionRequest request)
	{
		ValueTuple<long, global::UpgradeSessionResponse> valueTuple = await KIDManager.KIDServerWebRequest<global::UpgradeSessionResponse, global::UpgradeSessionRequest>("UpgradeSession", "POST", request, null, null);
		long item = valueTuple.Item1;
		global::UpgradeSessionResponse item2 = valueTuple.Item2;
		if (item != 200L)
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] Upgrade session request failed. Code: {0}", item));
		}
		return new UpgradeSessionData(item2);
	}

	// Token: 0x06003147 RID: 12615 RVA: 0x000F1AEC File Offset: 0x000EFCEC
	private static async Task<VerifyAgeData> Server_VerifyAge(VerifyAgeRequest request, Action failureCallback)
	{
		ValueTuple<long, VerifyAgeResponse> valueTuple = await KIDManager.KIDServerWebRequest<VerifyAgeResponse, VerifyAgeRequest>("VerifyAge", "POST", request, null, null);
		long item = valueTuple.Item1;
		VerifyAgeData verifyAgeData = new VerifyAgeData(valueTuple.Item2);
		if (item < 200L || item >= 300L)
		{
			if (failureCallback != null)
			{
				failureCallback();
			}
		}
		return verifyAgeData;
	}

	// Token: 0x06003148 RID: 12616 RVA: 0x000F1B38 File Offset: 0x000EFD38
	private static async Task<AttemptAgeUpdateData> Server_AttemptAgeUpdate(AttemptAgeUpdateRequest request, Action failureCallback)
	{
		ValueTuple<long, AttemptAgeUpdateResponse> valueTuple = await KIDManager.KIDServerWebRequest<AttemptAgeUpdateResponse, AttemptAgeUpdateRequest>("AttemptAgeUpdate", "POST", request, null, null);
		long item = valueTuple.Item1;
		AttemptAgeUpdateResponse item2 = valueTuple.Item2;
		if (item != 200L)
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] Attempt age update request failed. Code: {0}", item));
		}
		return new AttemptAgeUpdateData(item2.Status);
	}

	// Token: 0x06003149 RID: 12617 RVA: 0x000F1B7C File Offset: 0x000EFD7C
	private static async Task<bool> Server_AppealAge(AppealAgeRequest request, Action failureCallback)
	{
		bool success = false;
		long num = await KIDManager.KIDServerWebRequestNoResponse<AppealAgeRequest>("AppealAge", "POST", request, null);
		if (num == 200L)
		{
			success = true;
		}
		else
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] Appeal age request failed. Code: {0}", num));
		}
		return success;
	}

	// Token: 0x0600314A RID: 12618 RVA: 0x000F1BC0 File Offset: 0x000EFDC0
	private static async Task<bool> Server_SendChallengeEmail(SendChallengeEmailRequest request, Action failureCallback)
	{
		bool success = false;
		long num = await KIDManager.KIDServerWebRequestNoResponse<SendChallengeEmailRequest>("SendChallengeEmail", "POST", request, delegate(long responseCode)
		{
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_0021;
				}
			}
			else if (responseCode != 408L)
			{
				goto IL_0021;
			}
			return true;
			IL_0021:
			return false;
		});
		if (num >= 200L && num < 300L)
		{
			success = true;
		}
		else if (failureCallback != null)
		{
			failureCallback();
		}
		return success;
	}

	// Token: 0x0600314B RID: 12619 RVA: 0x000F1C0C File Offset: 0x000EFE0C
	private static async Task<bool> Server_OptIn()
	{
		long num = await KIDManager.KIDServerWebRequestNoResponse<KIDRequestData>("OptIn", "POST", null, null);
		bool flag;
		if (num == 200L)
		{
			flag = true;
		}
		else
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] Opt in request failed. Code: {0}", num));
			flag = false;
		}
		return flag;
	}

	// Token: 0x0600314C RID: 12620 RVA: 0x000F1C48 File Offset: 0x000EFE48
	private static async Task<GetRequirementsData> Server_GetRequirements()
	{
		ValueTuple<long, GetAgeGateRequirementsResponse> valueTuple = await KIDManager.KIDServerWebRequest<GetAgeGateRequirementsResponse, KIDRequestData>("GetRequirements", "GET", null, null, null);
		long item = valueTuple.Item1;
		GetAgeGateRequirementsResponse item2 = valueTuple.Item2;
		GetRequirementsData getRequirementsData = new GetRequirementsData
		{
			AgeGateRequirements = item2
		};
		GetRequirementsData getRequirementsData2;
		if (item == 200L)
		{
			getRequirementsData2 = getRequirementsData;
		}
		else
		{
			Debug.LogError(string.Format("[KID::SERVER_ROUTER] Get Age-gate Requirements FAILED. Code: {0}", item));
			getRequirementsData2 = getRequirementsData;
		}
		return getRequirementsData2;
	}

	// Token: 0x0600314D RID: 12621 RVA: 0x000F1C84 File Offset: 0x000EFE84
	[return: TupleElementNames(new string[] { "code", "responseModel" })]
	private static async Task<ValueTuple<long, T>> KIDServerWebRequest<T, Q>(string endpoint, string operationType, Q requestData, string queryParams = null, Func<long, bool> responseCodeIsRetryable = null) where T : class where Q : KIDRequestData
	{
		int retryCount = 0;
		string URL = "/api/" + endpoint;
		if (!string.IsNullOrEmpty(queryParams))
		{
			URL = URL + "?" + queryParams;
		}
		Debug.Log("[KID::MANAGER::SERVER_ROUTER] URL: " + URL);
		ValueTuple<long, T> valueTuple;
		for (;;)
		{
			using (UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.KidApiBaseUrl + URL, operationType))
			{
				byte[] array = Array.Empty<byte>();
				string json = "";
				if (requestData != null)
				{
					json = JsonConvert.SerializeObject(requestData);
					array = Encoding.UTF8.GetBytes(json);
				}
				request.uploadHandler = new UploadHandlerRaw(array);
				request.downloadHandler = new DownloadHandlerBuffer();
				request.SetRequestHeader("Content-Type", "application/json");
				request.SetRequestHeader("X-Authorization", PlayFabSettings.staticPlayer.ClientSessionTicket);
				request.SetRequestHeader("X-PlayerId", PlayFabSettings.staticPlayer.PlayFabId);
				request.SetRequestHeader("X-Mothership-Token", MothershipClientContext.Token);
				request.SetRequestHeader("X-Mothership-Player-Id", MothershipClientContext.MothershipId);
				request.SetRequestHeader("X-Mothership-Env-Id", MothershipClientApiUnity.EnvironmentId);
				UnityWebRequest unityWebRequest = await request.SendWebRequest();
				if (unityWebRequest.result == UnityWebRequest.Result.Success)
				{
					if (typeof(T) == typeof(object))
					{
						valueTuple = new ValueTuple<long, T>(unityWebRequest.responseCode, default(T));
						break;
					}
					try
					{
						T t = JsonConvert.DeserializeObject<T>(unityWebRequest.downloadHandler.text);
						valueTuple = new ValueTuple<long, T>(unityWebRequest.responseCode, t);
						break;
					}
					catch (Exception)
					{
						Debug.LogError("[KID::SERVER_ROUTER] Failed to convert to class type [T] via JSON:\n[" + unityWebRequest.downloadHandler.text + "]");
						valueTuple = new ValueTuple<long, T>(unityWebRequest.responseCode, default(T));
						break;
					}
				}
				bool flag = request.result != UnityWebRequest.Result.ProtocolError;
				if (!flag)
				{
					bool flag2;
					if (responseCodeIsRetryable != null)
					{
						flag2 = responseCodeIsRetryable(request.responseCode);
					}
					else
					{
						long responseCode = request.responseCode;
						if (responseCode >= 500L)
						{
							if (responseCode >= 600L)
							{
								goto IL_02FF;
							}
						}
						else if (responseCode != 408L && responseCode != 429L)
						{
							goto IL_02FF;
						}
						bool flag3 = true;
						goto IL_0302;
						IL_02FF:
						flag3 = false;
						IL_0302:
						flag2 = flag3;
					}
					flag = flag2;
				}
				if (flag)
				{
					if (retryCount < 3)
					{
						float num = Random.Range(0f, Mathf.Pow(2f, (float)(++retryCount)));
						Debug.LogWarning(string.Concat(new string[] { "[KID::SERVER_ROUTER] Tried sending request [", operationType, " - ", endpoint, "] but it failed:\n", unityWebRequest.error, "\n\nRequest:\n", json }));
						Debug.LogWarning(string.Format("[KID::SERVER_ROUTER] Retrying {0}... Retry attempt #{1}, waiting for {2} seconds", endpoint, retryCount, num));
						await Task.Delay(TimeSpan.FromSeconds((double)num));
						continue;
					}
					Debug.LogError(string.Concat(new string[] { "[KID::SERVER_ROUTER] Tried sending request [", operationType, " - ", endpoint, "] but it failed:\n", unityWebRequest.error, "\n\nRequest:\n", json }));
					Debug.LogError("[KID::SERVER_ROUTER] Maximum retries attempted. Please check your network connection.");
				}
				if (request.result == UnityWebRequest.Result.ProtocolError)
				{
					Debug.LogError(string.Format("[KID::SERVER_ROUTER] HTTP {0} ERROR: {1}\nMessage: {2}", request.responseCode, request.error, request.downloadHandler.text));
				}
				else if (request.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.LogError("[KID::SERVER_ROUTER] NETWORK ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
				}
				else
				{
					Debug.LogError("[KID::SERVER_ROUTER] ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
				}
				valueTuple = new ValueTuple<long, T>(unityWebRequest.responseCode, default(T));
			}
			break;
		}
		return valueTuple;
	}

	// Token: 0x0600314E RID: 12622 RVA: 0x000F1CE8 File Offset: 0x000EFEE8
	private static async Task<long> KIDServerWebRequestNoResponse<Q>(string endpoint, string operationType, Q requestData, Func<long, bool> responseCodeIsRetryable = null) where Q : KIDRequestData
	{
		TaskAwaiter<ValueTuple<long, object>> taskAwaiter = KIDManager.KIDServerWebRequest<object, Q>(endpoint, operationType, requestData, null, responseCodeIsRetryable).GetAwaiter();
		if (!taskAwaiter.IsCompleted)
		{
			await taskAwaiter;
			TaskAwaiter<ValueTuple<long, object>> taskAwaiter2;
			taskAwaiter = taskAwaiter2;
			taskAwaiter2 = default(TaskAwaiter<ValueTuple<long, object>>);
		}
		return taskAwaiter.GetResult().Item1;
	}

	// Token: 0x0600314F RID: 12623 RVA: 0x000F1D43 File Offset: 0x000EFF43
	public static void RegisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors any permission change");
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Combine(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	// Token: 0x06003150 RID: 12624 RVA: 0x000F1D64 File Offset: 0x000EFF64
	public static void UnregisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		Debug.Log("[KID] Successfully unregistered a new callback to SessionUpdate which monitors any permission change");
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Remove(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	// Token: 0x06003151 RID: 12625 RVA: 0x000F1D85 File Offset: 0x000EFF85
	public static void RegisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Voice Chat permission");
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	// Token: 0x06003152 RID: 12626 RVA: 0x000F1DA6 File Offset: 0x000EFFA6
	public static void UnregisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Voice Chat permission");
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	// Token: 0x06003153 RID: 12627 RVA: 0x000F1DC7 File Offset: 0x000EFFC7
	public static void RegisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Custom Usernames permission");
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	// Token: 0x06003154 RID: 12628 RVA: 0x000F1DE8 File Offset: 0x000EFFE8
	public static void UnregisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Custom Usernames permission");
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	// Token: 0x06003155 RID: 12629 RVA: 0x000F1E09 File Offset: 0x000F0009
	public static void RegisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Private Rooms permission");
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	// Token: 0x06003156 RID: 12630 RVA: 0x000F1E2A File Offset: 0x000F002A
	public static void UnregisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Private Rooms permission");
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	// Token: 0x06003157 RID: 12631 RVA: 0x000F1E4B File Offset: 0x000F004B
	public static void RegisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Multiplayer permission");
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	// Token: 0x06003158 RID: 12632 RVA: 0x000F1E6C File Offset: 0x000F006C
	public static void UnregisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Multiplayer permission");
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	// Token: 0x06003159 RID: 12633 RVA: 0x000F1E8D File Offset: 0x000F008D
	public static void RegisterSessionUpdatedCallback_UGC(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the UGC permission");
		KIDManager._onSessionUpdated_UGC = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_UGC, callback);
	}

	// Token: 0x0600315A RID: 12634 RVA: 0x000F1EB0 File Offset: 0x000F00B0
	private static async Task<bool> WaitForAndUpdateNewSession(bool forceRefresh)
	{
		bool flag;
		if (KIDManager._isUpdatingNewSession)
		{
			Debug.LogError("[KID::MANAGER] Trying to UpdateNewSession, but is already running, or state was not reset. Will not start again");
			flag = false;
		}
		else
		{
			KIDManager._isUpdatingNewSession = true;
			Debug.Log(string.Format("[KID::MANAGER] UpdateNewSession -- START - Starting Update New Session async Loop. Max duration: [{0:#} minutes", 10f));
			float updateTimeout = Time.time + 600f;
			GetPlayerData_Data getPlayerData_Data = await KIDManager.TryGetPlayerData(forceRefresh);
			TMPSession tmpsession = ((getPlayerData_Data != null) ? getPlayerData_Data.session : null);
			bool flag2 = KIDManager.HasSessionChanged(tmpsession);
			while (Time.time < updateTimeout && (tmpsession == null || tmpsession.Age == 0 || !flag2))
			{
				await Task.Delay(30000);
				if (KIDManager._requestCancellationSource.IsCancellationRequested)
				{
					Debug.Log("[KID::MANAGER] UpdateNewSession -- CANCELLED - CancellationTokenSource was cancelled, aborting session Update");
					KIDManager._isUpdatingNewSession = false;
					return false;
				}
				Debug.Log("[KID::MANAGER] UpdateNewSession -- LOOP - Trying to get Player Data");
				getPlayerData_Data = await KIDManager.TryGetPlayerData(forceRefresh);
				tmpsession = ((getPlayerData_Data != null) ? getPlayerData_Data.session : null);
				flag2 = KIDManager.HasSessionChanged(tmpsession);
				if (flag2)
				{
					Debug.Log("[KID::MANAGER] UpdateNewSession -- SUCCESS - Valid, updated session has been found");
					break;
				}
				if (getPlayerData_Data == null)
				{
					Debug.LogError("[KID::MANAGER] UpdateNewSession -- LOOP - Tried getting Player Data but returned NULL");
				}
				else if (getPlayerData_Data.responseType == GetSessionResponseType.ERROR)
				{
					Debug.LogError("[KID::MANAGER] UpdateNewSession -- LOOP - Tried getting a new Session but playerData returned with ERROR");
				}
				else if (tmpsession == null)
				{
					Debug.LogError("[KID::MANAGER] UpdateNewSession -- LOOP - Found Player Data, but SESSION was NULL");
				}
			}
			KIDManager._isUpdatingNewSession = false;
			if (getPlayerData_Data == null || getPlayerData_Data.responseType != GetSessionResponseType.OK || tmpsession == null)
			{
				Debug.Log("[KID::MANAGER] UpdateNewSession -- FAILED - Was unable to get new session in time");
				flag = false;
			}
			else
			{
				flag = KIDManager.UpdatePermissions(tmpsession);
			}
		}
		return flag;
	}

	// Token: 0x0600315B RID: 12635 RVA: 0x000F1EF4 File Offset: 0x000F00F4
	private static bool HasSessionChanged(TMPSession newSession)
	{
		if (newSession == null)
		{
			return false;
		}
		if (KIDManager.CurrentSession == null)
		{
			return true;
		}
		if (!newSession.IsValidSession)
		{
			return false;
		}
		if (newSession.IsDefault)
		{
			Debug.LogError(string.Format("[KID::MANAGER] DEBUG - New Session Is Default! Age: [{0}]", newSession.Age));
			return false;
		}
		return KIDManager.CurrentSession.IsDefault || !newSession.Etag.Equals(KIDManager.CurrentSession.Etag);
	}

	// Token: 0x0600315C RID: 12636 RVA: 0x000F1F68 File Offset: 0x000F0168
	private static void OnSessionUpdated()
	{
		Action onSessionUpdated_AnyPermission = KIDManager._onSessionUpdated_AnyPermission;
		if (onSessionUpdated_AnyPermission != null)
		{
			onSessionUpdated_AnyPermission();
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		int count = allPermissionsData.Count;
		for (int i = 0; i < count; i++)
		{
			Permission permission = allPermissionsData[i];
			string name = permission.Name;
			if (!(name == "voice-chat"))
			{
				if (!(name == "custom-username"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "multiplayer"))
						{
							if (!(name == "mods"))
							{
								Debug.Log("[KID] Tried updating permission with name [" + permission.Name + "] but did not match any of the set cases. Unable to process");
							}
							else if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_UGC = KIDManager._onSessionUpdated_UGC;
								if (onSessionUpdated_UGC != null)
								{
									onSessionUpdated_UGC(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
						}
						else
						{
							if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_Multiplayer = KIDManager._onSessionUpdated_Multiplayer;
								if (onSessionUpdated_Multiplayer != null)
								{
									onSessionUpdated_Multiplayer(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
							bool enabled = permission.Enabled;
						}
					}
					else
					{
						if (KIDManager.HasPermissionChanged(permission))
						{
							Action<bool, Permission.ManagedByEnum> onSessionUpdated_PrivateRooms = KIDManager._onSessionUpdated_PrivateRooms;
							if (onSessionUpdated_PrivateRooms != null)
							{
								onSessionUpdated_PrivateRooms(permission.Enabled, permission.ManagedBy);
							}
							KIDManager._previousPermissionSettings[permission.Name] = permission;
						}
						flag2 = permission.Enabled;
					}
				}
				else
				{
					if (KIDManager.HasPermissionChanged(permission))
					{
						Action<bool, Permission.ManagedByEnum> onSessionUpdated_CustomUsernames = KIDManager._onSessionUpdated_CustomUsernames;
						if (onSessionUpdated_CustomUsernames != null)
						{
							onSessionUpdated_CustomUsernames(permission.Enabled, permission.ManagedBy);
						}
						KIDManager._previousPermissionSettings[permission.Name] = permission;
					}
					flag3 = permission.Enabled;
				}
			}
			else
			{
				if (KIDManager.HasPermissionChanged(permission))
				{
					Action<bool, Permission.ManagedByEnum> onSessionUpdated_VoiceChat = KIDManager._onSessionUpdated_VoiceChat;
					if (onSessionUpdated_VoiceChat != null)
					{
						onSessionUpdated_VoiceChat(permission.Enabled, permission.ManagedBy);
					}
					KIDManager._previousPermissionSettings[permission.Name] = permission;
				}
				flag = permission.Enabled;
			}
		}
		GorillaTelemetry.PostKidEvent(flag2, flag, flag3, KIDManager.CurrentSession.AgeStatus, GTKidEventType.permission_update);
	}

	// Token: 0x0600315D RID: 12637 RVA: 0x000F219C File Offset: 0x000F039C
	private static bool HasPermissionChanged(Permission newValue)
	{
		Permission permission;
		if (KIDManager._previousPermissionSettings.TryGetValue(newValue.Name, out permission))
		{
			return permission.Enabled != newValue.Enabled || permission.ManagedBy != newValue.ManagedBy;
		}
		KIDManager._previousPermissionSettings.Add(newValue.Name, newValue);
		return true;
	}

	// Token: 0x0400376F RID: 14191
	public const string MULTIPLAYER_PERMISSION_NAME = "multiplayer";

	// Token: 0x04003770 RID: 14192
	public const string UGC_PERMISSION_NAME = "mods";

	// Token: 0x04003771 RID: 14193
	public const string PRIVATE_ROOM_PERMISSION_NAME = "join-groups";

	// Token: 0x04003772 RID: 14194
	public const string VOICE_CHAT_PERMISSION_NAME = "voice-chat";

	// Token: 0x04003773 RID: 14195
	public const string CUSTOM_USERNAME_PERMISSION_NAME = "custom-username";

	// Token: 0x04003774 RID: 14196
	public const string PREVIOUS_STATUS_PREF_KEY_PREFIX = "previous-status-";

	// Token: 0x04003775 RID: 14197
	public const string KID_DATA_KEY = "KIDData";

	// Token: 0x04003776 RID: 14198
	private const string KID_EMAIL_KEY = "k-id_EmailAddress";

	// Token: 0x04003777 RID: 14199
	private const int SECONDS_BETWEEN_UPDATE_ATTEMPTS = 30;

	// Token: 0x04003778 RID: 14200
	private const string KID_SETUP_FLAG = "KID-Setup-";

	// Token: 0x04003779 RID: 14201
	[OnEnterPlay_SetNull]
	private static KIDManager _instance;

	// Token: 0x0400377E RID: 14206
	private static string _emailAddress;

	// Token: 0x0400377F RID: 14207
	private static CancellationTokenSource _requestCancellationSource = new CancellationTokenSource();

	// Token: 0x04003780 RID: 14208
	private static bool _titleDataReady = false;

	// Token: 0x04003781 RID: 14209
	private static bool _useKid = false;

	// Token: 0x04003782 RID: 14210
	private static int _kIDPhase = 0;

	// Token: 0x04003783 RID: 14211
	private static DateTime? _kIDNewPlayerDateTime = null;

	// Token: 0x04003787 RID: 14215
	private static string _debugKIDLocalePlayerPrefRef = "KID_SPOOF_LOCALE";

	// Token: 0x04003788 RID: 14216
	private static string parentEmailForUserPlayerPrefRef;

	// Token: 0x04003789 RID: 14217
	[OnEnterPlay_SetNull]
	private static Action _sessionUpdatedCallback = null;

	// Token: 0x0400378A RID: 14218
	[OnEnterPlay_SetNull]
	private static Action _onKIDInitialisationComplete = null;

	// Token: 0x0400378B RID: 14219
	public static KIDManager.OnEmailResultReceived onEmailResultReceived;

	// Token: 0x0400378C RID: 14220
	private const string KID_GET_SESSION = "GetPlayerData";

	// Token: 0x0400378D RID: 14221
	private const string KID_VERIFY_AGE = "VerifyAge";

	// Token: 0x0400378E RID: 14222
	private const string KID_UPGRADE_SESSION = "UpgradeSession";

	// Token: 0x0400378F RID: 14223
	private const string KID_SEND_CHALLENGE_EMAIL = "SendChallengeEmail";

	// Token: 0x04003790 RID: 14224
	private const string KID_ATTEMPT_AGE_UPDATE = "AttemptAgeUpdate";

	// Token: 0x04003791 RID: 14225
	private const string KID_APPEAL_AGE = "AppealAge";

	// Token: 0x04003792 RID: 14226
	private const string KID_OPT_IN = "OptIn";

	// Token: 0x04003793 RID: 14227
	private const string KID_GET_REQUIREMENTS = "GetRequirements";

	// Token: 0x04003794 RID: 14228
	private const string KID_FORCE_REFRESH = "sessionRefresh";

	// Token: 0x04003795 RID: 14229
	private const int KID_SERVER_RETRIES = 3;

	// Token: 0x04003796 RID: 14230
	public const string KID_PERMISSION__VOICE_CHAT = "voice-chat";

	// Token: 0x04003797 RID: 14231
	public const string KID_PERMISSION__CUSTOM_NAMES = "custom-username";

	// Token: 0x04003798 RID: 14232
	public const string KID_PERMISSION__PRIVATE_ROOMS = "join-groups";

	// Token: 0x04003799 RID: 14233
	public const string KID_PERMISSION__MULTIPLAYER = "multiplayer";

	// Token: 0x0400379A RID: 14234
	public const string KID_PERMISSION__UGC = "mods";

	// Token: 0x0400379B RID: 14235
	private const float MAX_SESSION_UPDATE_TIME = 600f;

	// Token: 0x0400379C RID: 14236
	private const int TIME_BETWEEN_SESSION_UPDATE_ATTEMPTS = 30;

	// Token: 0x0400379D RID: 14237
	[OnEnterPlay_SetNull]
	private static Action _onSessionUpdated_AnyPermission;

	// Token: 0x0400379E RID: 14238
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_VoiceChat;

	// Token: 0x0400379F RID: 14239
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_CustomUsernames;

	// Token: 0x040037A0 RID: 14240
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_PrivateRooms;

	// Token: 0x040037A1 RID: 14241
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_Multiplayer;

	// Token: 0x040037A2 RID: 14242
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_UGC;

	// Token: 0x040037A3 RID: 14243
	private static bool _isUpdatingNewSession = false;

	// Token: 0x040037A4 RID: 14244
	[OnEnterPlay_SetNull]
	private static Dictionary<string, Permission> _previousPermissionSettings = new Dictionary<string, Permission>();

	// Token: 0x020007C2 RID: 1986
	// (Invoke) Token: 0x06003161 RID: 12641
	public delegate void OnEmailResultReceived(bool result);
}
