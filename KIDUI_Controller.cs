using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using UnityEngine;

// Token: 0x02000822 RID: 2082
public class KIDUI_Controller : MonoBehaviour
{
	// Token: 0x17000524 RID: 1316
	// (get) Token: 0x060032F0 RID: 13040 RVA: 0x000FB38A File Offset: 0x000F958A
	public static KIDUI_Controller Instance
	{
		get
		{
			return KIDUI_Controller._instance;
		}
	}

	// Token: 0x17000525 RID: 1317
	// (get) Token: 0x060032F1 RID: 13041 RVA: 0x000FB391 File Offset: 0x000F9591
	public static bool IsKIDUIActive
	{
		get
		{
			return !(KIDUI_Controller.Instance == null) && KIDUI_Controller.Instance._isKidUIActive;
		}
	}

	// Token: 0x17000526 RID: 1318
	// (get) Token: 0x060032F2 RID: 13042 RVA: 0x000FB3AC File Offset: 0x000F95AC
	private static string EtagOnCloseBlackScreenPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr))
			{
				KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr = "closeBlackScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr;
		}
	}

	// Token: 0x060032F3 RID: 13043 RVA: 0x000FB3DA File Offset: 0x000F95DA
	private void Awake()
	{
		KIDUI_Controller._instance = this;
		Debug.LogFormat("[KID::UI::CONTROLLER] Controller Initialised", Array.Empty<object>());
	}

	// Token: 0x060032F4 RID: 13044 RVA: 0x000FB3F1 File Offset: 0x000F95F1
	private void OnDestroy()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x060032F5 RID: 13045 RVA: 0x000FB414 File Offset: 0x000F9614
	public async Task StartKIDScreens(CancellationToken cancellationToken)
	{
		Debug.LogFormat("[KID::UI::CONTROLLER] Starting k-ID Screens", Array.Empty<object>());
		bool flag = await this.ShouldShowKIDScreen(cancellationToken);
		if (!cancellationToken.IsCancellationRequested)
		{
			if (!flag)
			{
				Debug.LogFormat("[KID::UI::CONTROLLER] Should NOT Show k-ID Screens", Array.Empty<object>());
			}
			else
			{
				Debug.LogFormat("[KID::UI::CONTROLLER] Showing k-ID Screens", Array.Empty<object>());
				HandRayController.Instance.EnableHandRays();
				PrivateUIRoom.AddUI(base.transform);
				EMainScreenStatus screenStatusFromSession = this.GetScreenStatusFromSession();
				this._mainKIDScreen.ShowMainScreen(screenStatusFromSession, this._showReason);
				this._isKidUIActive = true;
				KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Combine(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
			}
		}
	}

	// Token: 0x060032F6 RID: 13046 RVA: 0x000FB460 File Offset: 0x000F9660
	public void CloseKIDScreens()
	{
		this.SaveEtagOnCloseScreen();
		this._isKidUIActive = false;
		this._mainKIDScreen.HideMainScreen();
		PrivateUIRoom.RemoveUI(base.transform);
		HandRayController.Instance.DisableHandRays();
		Object.DestroyImmediate(base.gameObject);
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x060032F7 RID: 13047 RVA: 0x000FB4C8 File Offset: 0x000F96C8
	public void UpdateScreenStatus()
	{
		EMainScreenStatus screenStatusFromSession = this.GetScreenStatusFromSession();
		KIDUI_MainScreen mainKIDScreen = this._mainKIDScreen;
		if (mainKIDScreen == null)
		{
			return;
		}
		mainKIDScreen.UpdateScreenStatus(screenStatusFromSession, true);
	}

	// Token: 0x060032F8 RID: 13048 RVA: 0x000FB4F0 File Offset: 0x000F96F0
	public void NotifyOfEmailResult(bool success)
	{
		if (this._confirmScreen == null)
		{
			Debug.LogError("[KID::UI_CONTROLLER] _confirmScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		if (success)
		{
			PlayerPrefs.SetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 1);
			PlayerPrefs.Save();
		}
		Debug.Log("[KID::UI_CONTROLLER] Notifying user about email result. Showing confirm screen.");
		this._confirmScreen.NotifyOfResult(success);
	}

	// Token: 0x060032F9 RID: 13049 RVA: 0x000FB540 File Offset: 0x000F9740
	private EMainScreenStatus GetScreenStatusFromSession()
	{
		EMainScreenStatus emainScreenStatus;
		switch (KIDManager.CurrentSession.SessionStatus)
		{
		case SessionStatus.PASS:
			if (this.ShouldShowScreenOnPermissionChange())
			{
				emainScreenStatus = EMainScreenStatus.Updated;
			}
			else if (KIDManager.PreviousStatus == SessionStatus.CHALLENGE_SESSION_UPGRADE)
			{
				emainScreenStatus = EMainScreenStatus.Declined;
			}
			else
			{
				emainScreenStatus = EMainScreenStatus.Missing;
			}
			break;
		case SessionStatus.PROHIBITED:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Status is PROHIBITED but is trying to show k-ID screens");
			emainScreenStatus = EMainScreenStatus.Declined;
			break;
		case SessionStatus.CHALLENGE:
		case SessionStatus.CHALLENGE_SESSION_UPGRADE:
		case SessionStatus.PENDING_AGE_APPEAL:
			if (string.IsNullOrEmpty(PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "")))
			{
				emainScreenStatus = EMainScreenStatus.Setup;
			}
			else
			{
				emainScreenStatus = EMainScreenStatus.Pending;
			}
			break;
		default:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Unknown status");
			emainScreenStatus = EMainScreenStatus.None;
			break;
		}
		return emainScreenStatus;
	}

	// Token: 0x060032FA RID: 13050 RVA: 0x000FB5CC File Offset: 0x000F97CC
	private async Task<bool> ShouldShowKIDScreen(CancellationToken cancellationToken)
	{
		bool flag;
		if (KIDManager.CurrentSession == null)
		{
			this._showReason = KIDUI_Controller.Metrics_ShowReason.No_Session;
			flag = true;
		}
		else
		{
			if (!KIDManager.CurrentSession.IsValidSession)
			{
				while (!KIDManager.CurrentSession.IsValidSession)
				{
					Debug.Log("[KID::UI::CONTROLLER] K-ID Session not found yet");
					await Task.Delay(100, cancellationToken);
				}
			}
			Debug.Log("[KID::UI::CONTROLLER] K-ID Session has been found and is proceeding ");
			if (KIDManager.HasAllPermissions())
			{
				flag = false;
			}
			else
			{
				for (int i = 0; i < this._inaccessibleSettings.Count; i++)
				{
					Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(this._inaccessibleSettings[i]);
					if (permissionDataByFeature == null)
					{
						Debug.LogErrorFormat(string.Format("[KID::UI::CONTROLLER] Failed to get Permission with name [{0}]", this._inaccessibleSettings[i]), Array.Empty<object>());
						return true;
					}
					if (permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED && !KIDManager.CheckFeatureSettingEnabled(this._inaccessibleSettings[i]))
					{
						this._showReason = KIDUI_Controller.Metrics_ShowReason.Inaccessible;
						if (KIDManager.CurrentSession.IsDefault)
						{
							this._showReason = KIDUI_Controller.Metrics_ShowReason.Default_Session;
						}
						return true;
					}
				}
				List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
				for (int j = 0; j < allPermissionsData.Count; j++)
				{
					if (allPermissionsData[j].ManagedBy == Permission.ManagedByEnum.GUARDIAN && !allPermissionsData[j].Enabled)
					{
						this._showReason = KIDUI_Controller.Metrics_ShowReason.Guardian_Disabled;
						if (KIDManager.CurrentSession.IsDefault)
						{
							this._showReason = KIDUI_Controller.Metrics_ShowReason.Default_Session;
						}
						return true;
					}
				}
				this._mainKIDScreen.InitialiseMainScreen();
				if (this._mainKIDScreen.GetFeatureListingCount() == 0)
				{
					Debug.Log("[KID::CONTROLLER] Nothing to show on k-ID UI. Skipping");
					flag = false;
				}
				else if (this.ShouldShowScreenOnPermissionChange())
				{
					this._showReason = KIDUI_Controller.Metrics_ShowReason.Permissions_Changed;
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
		}
		return flag;
	}

	// Token: 0x060032FB RID: 13051 RVA: 0x000FB617 File Offset: 0x000F9817
	private bool ShouldShowScreenOnPermissionChange()
	{
		this._lastEtagOnClose = this.GetLastBlackScreenEtag();
		string lastEtagOnClose = this._lastEtagOnClose;
		TMPSession currentSession = KIDManager.CurrentSession;
		return lastEtagOnClose != (((currentSession != null) ? currentSession.Etag : null) ?? string.Empty);
	}

	// Token: 0x060032FC RID: 13052 RVA: 0x000FB64A File Offset: 0x000F984A
	private string GetLastBlackScreenEtag()
	{
		return PlayerPrefs.GetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, "");
	}

	// Token: 0x060032FD RID: 13053 RVA: 0x000FB65B File Offset: 0x000F985B
	private void SaveEtagOnCloseScreen()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.Log("[KID::MANAGER] Trying to save Pre-Game Screen ETAG, but [CurrentSession] is null");
			return;
		}
		PlayerPrefs.SetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, KIDManager.CurrentSession.Etag);
		PlayerPrefs.Save();
	}

	// Token: 0x040039C5 RID: 14789
	private const string CLOSE_BLACK_SCREEN_ETAG_PLAYER_PREF_PREFIX = "closeBlackScreen-";

	// Token: 0x040039C6 RID: 14790
	private const string FIRST_TIME_POST_CHANGE_PLAYER_PREF = "hasShownFirstTimePostChange-";

	// Token: 0x040039C7 RID: 14791
	private static KIDUI_Controller _instance;

	// Token: 0x040039C8 RID: 14792
	[SerializeField]
	private KIDUI_MainScreen _mainKIDScreen;

	// Token: 0x040039C9 RID: 14793
	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	// Token: 0x040039CA RID: 14794
	[SerializeField]
	private List<string> _PermissionsWithToggles = new List<string>();

	// Token: 0x040039CB RID: 14795
	[SerializeField]
	private List<EKIDFeatures> _inaccessibleSettings = new List<EKIDFeatures>
	{
		EKIDFeatures.Multiplayer,
		EKIDFeatures.Mods
	};

	// Token: 0x040039CC RID: 14796
	private KIDUI_Controller.Metrics_ShowReason _showReason;

	// Token: 0x040039CD RID: 14797
	private bool _isKidUIActive;

	// Token: 0x040039CE RID: 14798
	private static string etagOnCloseBlackScreenPlayerPrefStr;

	// Token: 0x040039CF RID: 14799
	private string _lastEtagOnClose;

	// Token: 0x02000823 RID: 2083
	public enum Metrics_ShowReason
	{
		// Token: 0x040039D1 RID: 14801
		None,
		// Token: 0x040039D2 RID: 14802
		Inaccessible,
		// Token: 0x040039D3 RID: 14803
		Guardian_Disabled,
		// Token: 0x040039D4 RID: 14804
		Permissions_Changed,
		// Token: 0x040039D5 RID: 14805
		Default_Session,
		// Token: 0x040039D6 RID: 14806
		No_Session
	}
}
