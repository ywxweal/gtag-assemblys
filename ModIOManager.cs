using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using GorillaTagScripts.ModIO;
using GT_CustomMapSupportRuntime;
using ModIO;
using Steamworks;
using UnityEngine;

// Token: 0x02000716 RID: 1814
public class ModIOManager : MonoBehaviour
{
	// Token: 0x06002D38 RID: 11576 RVA: 0x000DF95C File Offset: 0x000DDB5C
	private void Awake()
	{
		if (ModIOManager.instance == null)
		{
			ModIOManager.instance = this;
			ModIOManager.hasInstance = true;
			UGCPermissionManager.SubscribeToUGCEnabled(new Action(ModIOManager.OnUGCEnabled));
			UGCPermissionManager.SubscribeToUGCDisabled(new Action(ModIOManager.OnUGCDisabled));
			return;
		}
		if (ModIOManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x000DF9C3 File Offset: 0x000DDBC3
	private void Start()
	{
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x000DF9DC File Offset: 0x000DDBDC
	private void OnDestroy()
	{
		if (ModIOManager.instance == this)
		{
			ModIOManager.instance = null;
			ModIOManager.hasInstance = false;
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(ModIOManager.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(ModIOManager.OnUGCDisabled));
		}
		NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x000DFA40 File Offset: 0x000DDC40
	private void Update()
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (ModIOManager.modDownloadQueue.IsNullOrEmpty<ModId>())
		{
			return;
		}
		if (ModIOUnity.IsModManagementBusy())
		{
			return;
		}
		ModId modId = ModIOManager.modDownloadQueue[0];
		ModIOManager.DownloadMod(modId, null);
		ModIOManager.modDownloadQueue.Remove(modId);
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x000023F4 File Offset: 0x000005F4
	private static void OnUGCEnabled()
	{
	}

	// Token: 0x06002D3D RID: 11581 RVA: 0x000023F4 File Offset: 0x000005F4
	private static void OnUGCDisabled()
	{
	}

	// Token: 0x06002D3E RID: 11582 RVA: 0x000DFA89 File Offset: 0x000DDC89
	public static void Initialize(Action<ModIORequestResult> callback)
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			if (callback != null)
			{
				callback(ModIORequestResult.CreateFailureResult("MOD.IO FUNCTIONALITY IS CURRENTLY DISABLED."));
			}
			return;
		}
		if (ModIOManager.initialized && callback != null)
		{
			callback(ModIORequestResult.CreateSuccessResult());
		}
		ModIOManager.InitInternal(callback);
	}

	// Token: 0x06002D3F RID: 11583 RVA: 0x000DFAC4 File Offset: 0x000DDCC4
	private static void InitInternal(Action<ModIORequestResult> callback)
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		string text = "default";
		if (SteamManager.Initialized && SteamUser.BLoggedOn())
		{
			text = SteamUser.GetSteamID().ToString();
		}
		Result result = ModIOUnity.InitializeForUser(text);
		if (result.Succeeded())
		{
			ModIOManager.initialized = true;
			if (callback != null)
			{
				callback(ModIORequestResult.CreateSuccessResult());
				return;
			}
		}
		else if (callback != null)
		{
			callback(ModIORequestResult.CreateFailureResult(result.message));
		}
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x000DFB3C File Offset: 0x000DDD3C
	private void HasAcceptedLatestTerms(Action<bool> callback)
	{
		if (ModIOManager.initialized)
		{
			ModIOUnity.GetTermsOfUse(delegate(ResultAnd<TermsOfUse> result)
			{
				if (result.result.Succeeded())
				{
					this.OnReceivedTermsOfUse(result.value, callback);
					return;
				}
				Action<bool> callback3 = callback;
				if (callback3 == null)
				{
					return;
				}
				callback3(false);
			});
			return;
		}
		Action<bool> callback2 = callback;
		if (callback2 == null)
		{
			return;
		}
		callback2(false);
	}

	// Token: 0x06002D41 RID: 11585 RVA: 0x000DFB88 File Offset: 0x000DDD88
	private void OnReceivedTermsOfUse(TermsOfUse terms, Action<bool> callback)
	{
		ModIOManager.cachedModIOTerms = terms;
		ref TermsHash hash = ModIOManager.cachedModIOTerms.hash;
		string @string = PlayerPrefs.GetString("modIOAcceptedTermsHash");
		bool flag = hash.md5hash.Equals(@string);
		if (callback != null)
		{
			callback(flag);
		}
	}

	// Token: 0x06002D42 RID: 11586 RVA: 0x000DFBC8 File Offset: 0x000DDDC8
	private void ShowModIOTermsOfUse(Action<bool> callback)
	{
		if (!ModIOManager.initialized)
		{
			if (callback != null)
			{
				callback(false);
			}
			return;
		}
		if (this.modIOTermsOfUsePrefab != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.modIOTermsOfUsePrefab, base.transform);
			if (gameObject != null)
			{
				ModIOTermsOfUse component = gameObject.GetComponent<ModIOTermsOfUse>();
				if (component != null)
				{
					CustomMapManager.DisableTeleportHUD();
					ModIOManager.modIOTermsAcknowledgedCallback = callback;
					gameObject.SetActive(true);
					component.Initialize(ModIOManager.cachedModIOTerms, new Action<bool>(this.OnModIOTermsOfUseAcknowledged));
					return;
				}
				if (this.linkingTerminal != null)
				{
					this.linkingTerminal.DisplayLoginError("FAILED TO DISPLAY MOD.IO TERMS OF USE: \n'ModIOTermsOfUse' PREFAB DOES NOT CONTAIN A 'ModIOTermsOfUse' SCRIPT COMPONENT.");
				}
				if (callback != null)
				{
					callback(false);
					return;
				}
			}
			else
			{
				if (this.linkingTerminal != null)
				{
					this.linkingTerminal.DisplayLoginError("FAILED TO DISPLAY MOD.IO TERMS OF USE: \nFAILED TO INSTANTIATE TERMS OF USE GAME OBJECT FROM 'ModIOTermsOfUse' PREFAB");
				}
				if (callback != null)
				{
					callback(false);
					return;
				}
			}
		}
		else if (this.modIOTermsOfUsePrefab == null)
		{
			if (this.linkingTerminal != null)
			{
				this.linkingTerminal.DisplayLoginError("FAILED TO DISPLAY MOD.IO TERMS OF USE: \n`ModIOTermsOfUse` PREFAB IS SET TO NULL.");
			}
			if (callback != null)
			{
				callback(false);
			}
		}
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x000DFCD8 File Offset: 0x000DDED8
	private void OnModIOTermsOfUseAcknowledged(bool accepted)
	{
		if (!accepted && this.linkingTerminal != null)
		{
			this.linkingTerminal.DisplayLoginError("MOD.IO TERMS OF USE HAVE NOT BEEN ACCEPTED. YOU MUST ACCEPT THE MOD.IO TERMS OF USE TO LOGIN WITH YOUR PLATFORM CREDENTIALS OR YOU CAN LOGIN WITH AN EXISTING MOD.IO ACCOUNT BY PRESSING THE 'LINK MOD.IO ACCOUNT' BUTTON AND FOLLOWING THE INSTRUCTIONS.");
		}
		if (accepted)
		{
			PlayerPrefs.SetString("modIOAcceptedTermsHash", ModIOManager.cachedModIOTerms.hash.md5hash);
		}
		CustomMapManager.RequestEnableTeleportHUD(true);
		Action<bool> action = ModIOManager.modIOTermsAcknowledgedCallback;
		if (action != null)
		{
			action(accepted);
		}
		ModIOManager.modIOTermsAcknowledgedCallback = null;
	}

	// Token: 0x06002D44 RID: 11588 RVA: 0x000DFD3F File Offset: 0x000DDF3F
	private void EnableModManagement()
	{
		if (!ModIOManager.modManagementEnabled)
		{
			ModIOManager.Refresh(delegate(bool result)
			{
				if (ModIOUnity.EnableModManagement(new ModManagementEventDelegate(this.HandleModManagementEvent), ModIOManager.allowedAutomaticModOperations).Succeeded())
				{
					ModIOManager.modManagementEnabled = true;
				}
			}, false);
		}
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x000DFD5C File Offset: 0x000DDF5C
	private void HandleModManagementEvent(ModManagementEventType eventType, ModId modId, Result result)
	{
		switch (eventType)
		{
		case ModManagementEventType.InstallStarted:
		case ModManagementEventType.InstallFailed:
		case ModManagementEventType.DownloadStarted:
		case ModManagementEventType.DownloadFailed:
		case ModManagementEventType.UninstallStarted:
		case ModManagementEventType.UninstallFailed:
		case ModManagementEventType.UpdateStarted:
		case ModManagementEventType.UpdateFailed:
			break;
		case ModManagementEventType.Installed:
		{
			ModIOManager.outdatedModCMSVersions.Remove(modId.id);
			int num;
			ModIOManager.IsModOutdated(modId, out num);
			break;
		}
		case ModManagementEventType.Downloaded:
		case ModManagementEventType.Uninstalled:
		case ModManagementEventType.Updated:
			ModIOManager.outdatedModCMSVersions.Remove(modId.id);
			break;
		default:
			return;
		}
		GameEvents.ModIOModManagementEvent.Invoke(eventType, modId, result);
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x000DFDDC File Offset: 0x000DDFDC
	public static SubscribedModStatus ConvertModManagementEventToSubscribedModStatus(ModManagementEventType eventType)
	{
		switch (eventType)
		{
		case ModManagementEventType.InstallStarted:
			return SubscribedModStatus.Installing;
		case ModManagementEventType.Installed:
			return SubscribedModStatus.Installed;
		case ModManagementEventType.InstallFailed:
			return SubscribedModStatus.ProblemOccurred;
		case ModManagementEventType.DownloadStarted:
			return SubscribedModStatus.Downloading;
		case ModManagementEventType.Downloaded:
			return SubscribedModStatus.Downloading;
		case ModManagementEventType.DownloadFailed:
			return SubscribedModStatus.ProblemOccurred;
		case ModManagementEventType.UninstallStarted:
			return SubscribedModStatus.Uninstalling;
		case ModManagementEventType.Uninstalled:
			return SubscribedModStatus.None;
		case ModManagementEventType.UninstallFailed:
			return SubscribedModStatus.ProblemOccurred;
		case ModManagementEventType.UpdateStarted:
			return SubscribedModStatus.Updating;
		case ModManagementEventType.Updated:
			return SubscribedModStatus.Installed;
		case ModManagementEventType.UpdateFailed:
			return SubscribedModStatus.ProblemOccurred;
		default:
			return SubscribedModStatus.None;
		}
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x000DFE40 File Offset: 0x000DE040
	public static bool IsModOutdated(ModId modId, out int exportedVersion)
	{
		exportedVersion = -1;
		SubscribedMod subscribedMod;
		return ModIOManager.hasInstance && (ModIOManager.outdatedModCMSVersions.TryGetValue(modId.id, out exportedVersion) || (ModIOManager.GetSubscribedModProfile(modId, out subscribedMod) && ModIOManager.IsInstalledModOutdated(subscribedMod)));
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x000DFE80 File Offset: 0x000DE080
	private static bool IsInstalledModOutdated(SubscribedMod subscribedMod)
	{
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		if (subscribedMod.status != SubscribedModStatus.Installed)
		{
			return false;
		}
		FileInfo[] files = new DirectoryInfo(subscribedMod.directory).GetFiles("package.json");
		try
		{
			MapPackageInfo packageInfo = CustomMapLoader.GetPackageInfo(files[0].FullName);
			if (packageInfo.customMapSupportVersion != global::GT_CustomMapSupportRuntime.Constants.customMapSupportVersion)
			{
				ModIOManager.outdatedModCMSVersions.Add(subscribedMod.modProfile.id.id, packageInfo.customMapSupportVersion);
				return true;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x000DFF10 File Offset: 0x000DE110
	public static void Refresh(Action<bool> callback = null, bool force = false)
	{
		if (!ModIOManager.hasInstance)
		{
			if (callback != null)
			{
				callback(false);
			}
			return;
		}
		if (ModIOManager.refreshing)
		{
			ModIOManager.currentRefreshCallbacks.Add(callback);
			return;
		}
		if (force || Mathf.Approximately(0f, ModIOManager.lastRefreshTime) || Time.realtimeSinceStartup - ModIOManager.lastRefreshTime >= 5f)
		{
			ModIOManager.currentRefreshCallbacks.Add(callback);
			ModIOManager.lastRefreshTime = Time.realtimeSinceStartup;
			ModIOManager.refreshing = true;
			ModIOUnity.FetchUpdates(delegate(Result result)
			{
				ModIOManager.refreshing = false;
				foreach (Action<bool> action in ModIOManager.currentRefreshCallbacks)
				{
					if (action != null)
					{
						action(true);
					}
				}
				ModIOManager.currentRefreshCallbacks.Clear();
			});
			return;
		}
		if (callback != null)
		{
			callback(false);
		}
	}

	// Token: 0x06002D4A RID: 11594 RVA: 0x000DFFB4 File Offset: 0x000DE1B4
	public static void GetModProfile(ModId modId, Action<ModIORequestResultAnd<ModProfile>> callback)
	{
		if (ModIOManager.hasInstance)
		{
			ModIOUnity.GetMod(modId, delegate(ResultAnd<ModProfile> result)
			{
				if (!result.result.Succeeded())
				{
					Action<ModIORequestResultAnd<ModProfile>> callback3 = callback;
					if (callback3 == null)
					{
						return;
					}
					callback3(ModIORequestResultAnd<ModProfile>.CreateFailureResult(result.result.message));
					return;
				}
				else
				{
					Action<ModIORequestResultAnd<ModProfile>> callback4 = callback;
					if (callback4 == null)
					{
						return;
					}
					callback4(ModIORequestResultAnd<ModProfile>.CreateSuccessResult(result.value));
					return;
				}
			});
			return;
		}
		Action<ModIORequestResultAnd<ModProfile>> callback2 = callback;
		if (callback2 == null)
		{
			return;
		}
		callback2(ModIORequestResultAnd<ModProfile>.CreateFailureResult("MODIODATASTORE INSTANCE DOES NOT EXIST!"));
	}

	// Token: 0x06002D4B RID: 11595 RVA: 0x000E0002 File Offset: 0x000DE202
	public static bool IsLoggedIn()
	{
		return ModIOManager.hasInstance && ModIOManager.loggedIn;
	}

	// Token: 0x06002D4C RID: 11596 RVA: 0x000E0014 File Offset: 0x000DE214
	public static void IsAuthenticated(Action<Result> callback)
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		ModIOUnity.IsAuthenticated(delegate(Result result)
		{
			if (result.Succeeded())
			{
				ModIOManager.loggedIn = true;
				ModIOManager.instance.EnableModManagement();
				GameEvents.OnModIOLoggedIn.Invoke();
			}
			else
			{
				ModIOManager.loggedIn = false;
				ModIOManager.modManagementEnabled = false;
			}
			Action<Result> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(result);
		});
	}

	// Token: 0x06002D4D RID: 11597 RVA: 0x000E0048 File Offset: 0x000DE248
	public static void LogoutFromModIO()
	{
		if (!ModIOManager.hasInstance || ModIOManager.loggingIn)
		{
			return;
		}
		ModIOManager.CancelExternalAuthentication();
		ModIOManager.loggingIn = false;
		ModIOManager.loggedIn = false;
		ModIOUnity.LogOutCurrentUser();
		ModIOUnity.DisableModManagement();
		ModIOManager.modManagementEnabled = false;
		GameEvents.OnModIOLoggedOut.Invoke();
		PlayerPrefs.SetInt("modIOLassSuccessfulAuthMethod", ModIOManager.ModIOAuthMethod.Invalid.GetIndex<ModIOManager.ModIOAuthMethod>());
	}

	// Token: 0x06002D4E RID: 11598 RVA: 0x000E00A4 File Offset: 0x000DE2A4
	public static void RequestAccountLinkCode(Action<ModIORequestResult, string, string> onGetCodeCallback, Action<ModIORequestResult> onAuthCompleteCallback)
	{
		if (!ModIOManager.hasInstance)
		{
			if (onGetCodeCallback != null)
			{
				onGetCodeCallback(ModIORequestResult.CreateFailureResult("MODIODATASTORE INSTANCE DOES NOT EXIST!"), null, null);
			}
			if (onAuthCompleteCallback != null)
			{
				onAuthCompleteCallback(ModIORequestResult.CreateFailureResult("MODIODATASTORE INSTANCE DOES NOT EXIST!"));
			}
			return;
		}
		if (ModIOManager.loggingIn || ModIOManager.loggedIn)
		{
			if (onGetCodeCallback != null)
			{
				onGetCodeCallback(ModIORequestResult.CreateFailureResult("YOU MUST BE LOGGED OUT OF MOD.IO TO LINK AN EXISTING ACCOUNT."), null, null);
			}
			if (onAuthCompleteCallback != null)
			{
				onAuthCompleteCallback(ModIORequestResult.CreateFailureResult("YOU MUST BE LOGGED OUT OF MOD.IO TO LINK AN EXISTING ACCOUNT."));
			}
			return;
		}
		ModIOManager.loggingIn = true;
		ModIOManager.externalAuthGetCodeCallback = onGetCodeCallback;
		ModIOManager.externalAuthCallback = onAuthCompleteCallback;
		ModIOUnity.RequestExternalAuthentication(new Action<ResultAnd<ExternalAuthenticationToken>>(ModIOManager.instance.OnRequestExternalAuthentication));
	}

	// Token: 0x06002D4F RID: 11599 RVA: 0x000E0144 File Offset: 0x000DE344
	private async void OnRequestExternalAuthentication(ResultAnd<ExternalAuthenticationToken> resultAndToken)
	{
		if (!resultAndToken.result.Succeeded())
		{
			Action<ModIORequestResult, string, string> action = ModIOManager.externalAuthGetCodeCallback;
			if (action != null)
			{
				action(ModIORequestResult.CreateFailureResult("PLATFORM AUTHENTICATION FAILED: " + resultAndToken.result.message), null, null);
			}
			Action<ModIORequestResult> action2 = ModIOManager.externalAuthCallback;
			if (action2 != null)
			{
				action2(ModIORequestResult.CreateFailureResult("PLATFORM AUTHENTICATION FAILED: " + resultAndToken.result.message));
			}
			ModIOManager.loggingIn = false;
		}
		else
		{
			ModIOManager.externalAuthenticationToken = resultAndToken.value;
			Action<ModIORequestResult, string, string> action3 = ModIOManager.externalAuthGetCodeCallback;
			if (action3 != null)
			{
				action3(ModIORequestResult.CreateSuccessResult(), ModIOManager.externalAuthenticationToken.url, ModIOManager.externalAuthenticationToken.code);
			}
			TaskAwaiter<Result> taskAwaiter = ModIOManager.externalAuthenticationToken.task.GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<Result> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<Result>);
			}
			if (!taskAwaiter.GetResult().Succeeded())
			{
				Action<ModIORequestResult> action4 = ModIOManager.externalAuthCallback;
				if (action4 != null)
				{
					action4(ModIORequestResult.CreateFailureResult("AUTHENTICATION FAILED (POSSIBLY TIMED OUT)"));
				}
				ModIOManager.loggingIn = false;
			}
			else
			{
				PlayerPrefs.SetInt("modIOLassSuccessfulAuthMethod", ModIOManager.ModIOAuthMethod.LinkedAccount.GetIndex<ModIOManager.ModIOAuthMethod>());
				ModIOManager.externalAuthenticationToken = default(ExternalAuthenticationToken);
				this.OnExternalAuthenticationComplete(ModIORequestResult.CreateSuccessResult());
			}
		}
	}

	// Token: 0x06002D50 RID: 11600 RVA: 0x000E0184 File Offset: 0x000DE384
	public static void CancelExternalAuthentication()
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (ModIOManager.externalAuthenticationToken.task != null)
		{
			ModIOManager.externalAuthenticationToken.Cancel();
			ModIOManager.externalAuthenticationToken = default(ExternalAuthenticationToken);
			Action<ModIORequestResult> action = ModIOManager.externalAuthCallback;
			if (action != null)
			{
				action(ModIORequestResult.CreateFailureResult("AUTHENTICATION CANCELED"));
			}
			ModIOManager.externalAuthCallback = null;
			ModIOManager.loggedIn = false;
			ModIOManager.loggingIn = false;
		}
	}

	// Token: 0x06002D51 RID: 11601 RVA: 0x000E01E8 File Offset: 0x000DE3E8
	public static void RequestPlatformLogin(Action<ModIORequestResult> callback)
	{
		if (!ModIOManager.hasInstance)
		{
			Action<ModIORequestResult> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(ModIORequestResult.CreateFailureResult("MODIODATASTORE INSTANCE DOES NOT EXIST!"));
			return;
		}
		else
		{
			if (!ModIOManager.loggingIn)
			{
				ModIOManager.loggingIn = true;
				ModIOManager.IsAuthenticated(delegate(Result result)
				{
					if (!result.Succeeded())
					{
						ModIOManager.instance.InitiatePlatformLogin(callback);
						return;
					}
					ModIOManager.loggingIn = false;
					Action<ModIORequestResult> callback4 = callback;
					if (callback4 == null)
					{
						return;
					}
					callback4(ModIORequestResult.CreateSuccessResult());
				});
				return;
			}
			Action<ModIORequestResult> callback3 = callback;
			if (callback3 == null)
			{
				return;
			}
			callback3(ModIORequestResult.CreateFailureResult("LOGIN ALREADY IN PROGRESS"));
			return;
		}
	}

	// Token: 0x06002D52 RID: 11602 RVA: 0x000E0260 File Offset: 0x000DE460
	private void InitiatePlatformLogin(Action<ModIORequestResult> callback)
	{
		if (this.linkingTerminal != null)
		{
			this.linkingTerminal.NotifyLoggingIn();
		}
		Action<bool> <>9__1;
		this.HasAcceptedLatestTerms(delegate(bool termsAlreadyAccepted)
		{
			if (!termsAlreadyAccepted)
			{
				ModIOManager <>4__this = this;
				Action<bool> action;
				if ((action = <>9__1) == null)
				{
					action = (<>9__1 = delegate(bool termsAccepted)
					{
						if (!termsAccepted)
						{
							ModIORequestResult modIORequestResult = ModIORequestResult.CreateFailureResult("MOD.IO TERMS OF USE HAVE NOT BEEN ACCEPTED, CANNOT PERFORM PLATFORM LOGIN.");
							Action<ModIORequestResult> callback2 = callback;
							if (callback2 != null)
							{
								callback2(modIORequestResult);
							}
							this.OnExternalAuthenticationComplete(ModIORequestResult.CreateFailureResult("MOD.IO TERMS OF USE HAVE NOT BEEN ACCEPTED, CANNOT PERFORM PLATFORM LOGIN."));
							return;
						}
						this.ContinuePlatformLogin(callback);
					});
				}
				<>4__this.ShowModIOTermsOfUse(action);
				return;
			}
			this.ContinuePlatformLogin(callback);
		});
	}

	// Token: 0x06002D53 RID: 11603 RVA: 0x000E02AC File Offset: 0x000DE4AC
	private void ContinuePlatformLogin(Action<ModIORequestResult> callback)
	{
		if (SteamManager.Initialized)
		{
			if (ModIOManager.RequestEncryptedAppTicketResponse == null)
			{
				ModIOManager.RequestEncryptedAppTicketResponse = CallResult<EncryptedAppTicketResponse_t>.Create(new CallResult<EncryptedAppTicketResponse_t>.APIDispatchDelegate(this.OnRequestEncryptedAppTicketFinished));
			}
			SteamAPICall_t steamAPICall_t = SteamUser.RequestEncryptedAppTicket(null, 0);
			ModIOManager.RequestEncryptedAppTicketResponse.Set(steamAPICall_t, null);
			ModIOManager.externalAuthCallback = callback;
			return;
		}
		if (this.linkingTerminal != null)
		{
			this.linkingTerminal.DisplayLoginError("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nSTEAM IS ENABLED BUT NOT INITIALIZED.");
		}
		if (callback != null)
		{
			callback(ModIORequestResult.CreateFailureResult("STEAM IS ENABLED BUT NOT INITIALIZED"));
		}
	}

	// Token: 0x06002D54 RID: 11604 RVA: 0x000E0329 File Offset: 0x000DE529
	private string GetSteamEncryptedAppTicket()
	{
		Array.Resize<byte>(ref ModIOManager.ticketBlob, (int)ModIOManager.ticketSize);
		return Convert.ToBase64String(ModIOManager.ticketBlob);
	}

	// Token: 0x06002D55 RID: 11605 RVA: 0x000E0344 File Offset: 0x000DE544
	private void OnRequestEncryptedAppTicketFinished(EncryptedAppTicketResponse_t response, bool bIOFailure)
	{
		if (bIOFailure)
		{
			this.OnExternalAuthenticationComplete(ModIORequestResult.CreateFailureResult("FAILED TO RETRIEVE 'EncryptedAppTicket' DUE TO A STEAM API IO FAILURE."));
			if (this.linkingTerminal != null)
			{
				this.linkingTerminal.DisplayLoginError("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nFAILED TO RETRIEVE 'EncryptedAppTicket' DUE TO A STEAM API IO FAILURE.");
			}
			return;
		}
		EResult eResult = response.m_eResult;
		if (eResult <= EResult.k_EResultNoConnection)
		{
			if (eResult != EResult.k_EResultOK)
			{
				if (eResult == EResult.k_EResultNoConnection)
				{
					this.OnExternalAuthenticationComplete(ModIORequestResult.CreateFailureResult("NOT CONNECTED TO STEAM."));
					if (this.linkingTerminal != null)
					{
						this.linkingTerminal.DisplayLoginError("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nNOT CONNECTED TO STEAM.");
						return;
					}
					return;
				}
			}
			else
			{
				if (!SteamUser.GetEncryptedAppTicket(ModIOManager.ticketBlob, ModIOManager.ticketBlob.Length, out ModIOManager.ticketSize))
				{
					this.OnExternalAuthenticationComplete(ModIORequestResult.CreateFailureResult("FAILED TO RETRIEVE 'EncryptedAppTicket'."));
					if (this.linkingTerminal != null)
					{
						this.linkingTerminal.DisplayLoginError("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nFAILED TO RETRIEVE 'EncryptedAppTicket'.");
					}
					return;
				}
				ModIOUnity.AuthenticateUserViaSteam(this.GetSteamEncryptedAppTicket(), null, new TermsHash?(ModIOManager.cachedModIOTerms.hash), new Action<Result>(this.OnAuthWithSteam));
				return;
			}
		}
		else if (eResult != EResult.k_EResultLimitExceeded)
		{
			if (eResult == EResult.k_EResultDuplicateRequest)
			{
				this.OnExternalAuthenticationComplete(ModIORequestResult.CreateFailureResult("THERE IS ALREADY AN 'EncryptedAppTicket' REQUEST IN PROGRESS."));
				if (this.linkingTerminal != null)
				{
					this.linkingTerminal.DisplayLoginError("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nTHERE IS ALREADY AN 'EncryptedAppTicket' REQUEST IN PROGRESS.");
					return;
				}
				return;
			}
		}
		else
		{
			this.OnExternalAuthenticationComplete(ModIORequestResult.CreateFailureResult("RATE LIMIT EXCEEDED, CAN ONLY REQUEST ONE 'EncryptedAppTicket' PER MINUTE."));
			if (this.linkingTerminal != null)
			{
				this.linkingTerminal.DisplayLoginError("FAILED TO LOGIN TO MOD.IO VIA STEAM:\nRATE LIMIT EXCEEDED, CAN ONLY REQUEST ONE 'EncryptedAppTicket' PER MINUTE.");
				return;
			}
			return;
		}
		this.OnExternalAuthenticationComplete(ModIORequestResult.CreateFailureResult(string.Format("ERROR: {0}", response.m_eResult)));
		if (this.linkingTerminal != null)
		{
			this.linkingTerminal.DisplayLoginError(string.Format("FAILED TO LOGIN TO MOD.IO VIA STEAM:\n{0}", response.m_eResult));
		}
	}

	// Token: 0x06002D56 RID: 11606 RVA: 0x000E0504 File Offset: 0x000DE704
	private void OnAuthWithSteam(Result result)
	{
		if (result.Succeeded())
		{
			PlayerPrefs.SetInt("modIOLassSuccessfulAuthMethod", ModIOManager.ModIOAuthMethod.Steam.GetIndex<ModIOManager.ModIOAuthMethod>());
			this.OnExternalAuthenticationComplete(ModIORequestResult.CreateSuccessResult());
			return;
		}
		this.OnExternalAuthenticationComplete(ModIORequestResult.CreateFailureResult(result.message));
		if (this.linkingTerminal != null)
		{
			this.linkingTerminal.DisplayLoginError("FAILED TO LOGIN TO MOD.IO VIA STEAM:\n" + result.message);
		}
	}

	// Token: 0x06002D57 RID: 11607 RVA: 0x000E0574 File Offset: 0x000DE774
	private void OnExternalAuthenticationComplete(ModIORequestResult result)
	{
		if (result.success)
		{
			ModIOManager.loggedIn = true;
			ModIOManager.Refresh(null, true);
			this.EnableModManagement();
			GameEvents.OnModIOLoggedIn.Invoke();
		}
		else
		{
			ModIOManager.loggedIn = false;
		}
		ModIOManager.loggingIn = false;
		Action<ModIORequestResult> action = ModIOManager.externalAuthCallback;
		if (action != null)
		{
			action(result);
		}
		ModIOManager.externalAuthCallback = null;
	}

	// Token: 0x06002D58 RID: 11608 RVA: 0x000E05CC File Offset: 0x000DE7CC
	public static ModIOManager.ModIOAuthMethod GetLastAuthMethod()
	{
		int @int = PlayerPrefs.GetInt("modIOLassSuccessfulAuthMethod", -1);
		if (@int == -1)
		{
			return ModIOManager.ModIOAuthMethod.Invalid;
		}
		return (ModIOManager.ModIOAuthMethod)@int;
	}

	// Token: 0x06002D59 RID: 11609 RVA: 0x000E05EC File Offset: 0x000DE7EC
	public static SubscribedMod[] GetSubscribedMods()
	{
		Result result;
		SubscribedMod[] subscribedMods = ModIOUnity.GetSubscribedMods(out result);
		if (result.Succeeded())
		{
			return subscribedMods;
		}
		return null;
	}

	// Token: 0x06002D5A RID: 11610 RVA: 0x000E0610 File Offset: 0x000DE810
	public static void SubscribeToMod(ModId modId, Action<Result> callback)
	{
		ModIOUnity.SubscribeToMod(modId, delegate(Result result)
		{
			Action<Result> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(result);
		});
	}

	// Token: 0x06002D5B RID: 11611 RVA: 0x000E063C File Offset: 0x000DE83C
	public static void UnsubscribeFromMod(ModId modId, Action<Result> callback)
	{
		ModIOUnity.UnsubscribeFromMod(modId, delegate(Result result)
		{
			Action<Result> callback2 = callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(result);
		});
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x000E0668 File Offset: 0x000DE868
	public static bool GetSubscribedModStatus(ModId modId, out SubscribedModStatus modStatus)
	{
		modStatus = SubscribedModStatus.None;
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		Result result;
		SubscribedMod[] subscribedMods = ModIOUnity.GetSubscribedMods(out result);
		if (!result.Succeeded())
		{
			return false;
		}
		foreach (SubscribedMod subscribedMod in subscribedMods)
		{
			if (subscribedMod.modProfile.id.Equals(modId))
			{
				modStatus = subscribedMod.status;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x000E06D0 File Offset: 0x000DE8D0
	public static bool GetSubscribedModProfile(ModId modId, out SubscribedMod subscribedMod)
	{
		subscribedMod = default(SubscribedMod);
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		Result result;
		SubscribedMod[] subscribedMods = ModIOUnity.GetSubscribedMods(out result);
		if (!result.Succeeded())
		{
			return false;
		}
		foreach (SubscribedMod subscribedMod2 in subscribedMods)
		{
			if (subscribedMod2.modProfile.id.Equals(modId))
			{
				subscribedMod = subscribedMod2;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002D5E RID: 11614 RVA: 0x000E0738 File Offset: 0x000DE938
	public static void DownloadMod(ModId modId, Action<ModIORequestResult> callback)
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (!ModIOUnity.IsModManagementBusy())
		{
			ModIOUnity.DownloadNow(modId, delegate(Result result)
			{
				if (result.Succeeded())
				{
					Action<ModIORequestResult> callback3 = callback;
					if (callback3 == null)
					{
						return;
					}
					callback3(ModIORequestResult.CreateSuccessResult());
					return;
				}
				else
				{
					Action<ModIORequestResult> callback4 = callback;
					if (callback4 == null)
					{
						return;
					}
					callback4(ModIORequestResult.CreateFailureResult(result.message));
					return;
				}
			});
			return;
		}
		if (!ModIOManager.modDownloadQueue.Contains(modId))
		{
			ModIOManager.modDownloadQueue.Add(modId);
		}
		Action<ModIORequestResult> callback2 = callback;
		if (callback2 == null)
		{
			return;
		}
		callback2(ModIORequestResult.CreateSuccessResult());
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x000E07A1 File Offset: 0x000DE9A1
	public static bool IsModInDownloadQueue(ModId modId)
	{
		return ModIOManager.hasInstance && ModIOManager.modDownloadQueue.Contains(modId);
	}

	// Token: 0x06002D60 RID: 11616 RVA: 0x000E07B7 File Offset: 0x000DE9B7
	public static void AbortModDownload(ModId modId)
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (ModIOManager.modDownloadQueue.Contains(modId))
		{
			ModIOManager.modDownloadQueue.Remove(modId);
			return;
		}
		if (ModIOUnity.IsModManagementBusy())
		{
			ModIOUnity.AbortDownload(modId);
		}
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x000E07E8 File Offset: 0x000DE9E8
	private void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.RoomName.Contains(GorillaComputer.instance.VStumpRoomPrepend) && !GorillaComputer.instance.IsPlayerInVirtualStump() && !CustomMapManager.IsLocalPlayerInVirtualStump())
		{
			Debug.LogError("[ModIOManager::OnJoinedRoom] Player joined @ room while not in the VStump! Leaving the room...");
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
	}

	// Token: 0x06002D62 RID: 11618 RVA: 0x000E083C File Offset: 0x000DEA3C
	public static ModId GetNewMapsModId()
	{
		if (!ModIOManager.hasInstance)
		{
			return ModId.Null;
		}
		return new ModId(ModIOManager.instance.newMapsModId);
	}

	// Token: 0x04003378 RID: 13176
	private const string MODIO_ACCEPTED_TERMS_KEY = "modIOAcceptedTermsHash";

	// Token: 0x04003379 RID: 13177
	private const string MODIO_LAST_AUTH_METHOD_KEY = "modIOLassSuccessfulAuthMethod";

	// Token: 0x0400337A RID: 13178
	private const float REFRESH_RATE_LIMIT = 5f;

	// Token: 0x0400337B RID: 13179
	[OnEnterPlay_SetNull]
	private static volatile ModIOManager instance;

	// Token: 0x0400337C RID: 13180
	[OnEnterPlay_Set(false)]
	private static bool hasInstance;

	// Token: 0x0400337D RID: 13181
	private static readonly List<ModManagementOperationType> allowedAutomaticModOperations = new List<ModManagementOperationType>
	{
		ModManagementOperationType.Install,
		ModManagementOperationType.Uninstall
	};

	// Token: 0x0400337E RID: 13182
	private static bool initialized;

	// Token: 0x0400337F RID: 13183
	private static bool refreshing;

	// Token: 0x04003380 RID: 13184
	private static bool modManagementEnabled;

	// Token: 0x04003381 RID: 13185
	private static bool loggingIn;

	// Token: 0x04003382 RID: 13186
	private static bool loggedIn;

	// Token: 0x04003383 RID: 13187
	private static Coroutine preInitCoroutine;

	// Token: 0x04003384 RID: 13188
	private static Coroutine refreshDisabledCoroutine;

	// Token: 0x04003385 RID: 13189
	private static float lastRefreshTime;

	// Token: 0x04003386 RID: 13190
	private static List<Action<bool>> currentRefreshCallbacks = new List<Action<bool>>();

	// Token: 0x04003387 RID: 13191
	private static TermsOfUse cachedModIOTerms;

	// Token: 0x04003388 RID: 13192
	private static Action<bool> modIOTermsAcknowledgedCallback;

	// Token: 0x04003389 RID: 13193
	private static List<ModId> modDownloadQueue = new List<ModId>();

	// Token: 0x0400338A RID: 13194
	private static Dictionary<long, int> outdatedModCMSVersions = new Dictionary<long, int>();

	// Token: 0x0400338B RID: 13195
	private static Action<ModIORequestResult> externalAuthCallback;

	// Token: 0x0400338C RID: 13196
	private static ExternalAuthenticationToken externalAuthenticationToken;

	// Token: 0x0400338D RID: 13197
	private static Action<ModIORequestResult, string, string> externalAuthGetCodeCallback;

	// Token: 0x0400338E RID: 13198
	private static byte[] ticketBlob = new byte[1024];

	// Token: 0x0400338F RID: 13199
	private static uint ticketSize;

	// Token: 0x04003390 RID: 13200
	protected static CallResult<EncryptedAppTicketResponse_t> RequestEncryptedAppTicketResponse = null;

	// Token: 0x04003391 RID: 13201
	[SerializeField]
	private GameObject modIOTermsOfUsePrefab;

	// Token: 0x04003392 RID: 13202
	[SerializeField]
	private ModIOAccountLinkingTerminal linkingTerminal;

	// Token: 0x04003393 RID: 13203
	[SerializeField]
	private long newMapsModId;

	// Token: 0x02000717 RID: 1815
	public enum ModIOAuthMethod
	{
		// Token: 0x04003395 RID: 13205
		Invalid,
		// Token: 0x04003396 RID: 13206
		LinkedAccount,
		// Token: 0x04003397 RID: 13207
		Steam,
		// Token: 0x04003398 RID: 13208
		Oculus
	}
}
