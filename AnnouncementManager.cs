using System;
using GorillaNetworking;
using LitJson;
using PlayFab;
using UnityEngine;

// Token: 0x0200078C RID: 1932
public class AnnouncementManager : MonoBehaviour
{
	// Token: 0x06003066 RID: 12390 RVA: 0x000EF1CC File Offset: 0x000ED3CC
	public bool ShowAnnouncement()
	{
		return this._showAnnouncement;
	}

	// Token: 0x170004D5 RID: 1237
	// (get) Token: 0x06003067 RID: 12391 RVA: 0x000EF1D4 File Offset: 0x000ED3D4
	// (set) Token: 0x06003068 RID: 12392 RVA: 0x000EF1DC File Offset: 0x000ED3DC
	public bool _completedSetup { get; private set; }

	// Token: 0x170004D6 RID: 1238
	// (get) Token: 0x06003069 RID: 12393 RVA: 0x000EF1E5 File Offset: 0x000ED3E5
	// (set) Token: 0x0600306A RID: 12394 RVA: 0x000EF1ED File Offset: 0x000ED3ED
	public bool _announcementActive { get; private set; }

	// Token: 0x170004D7 RID: 1239
	// (get) Token: 0x0600306B RID: 12395 RVA: 0x000EF1F6 File Offset: 0x000ED3F6
	public static AnnouncementManager Instance
	{
		get
		{
			if (AnnouncementManager._instance == null)
			{
				Debug.LogError("[KID::ANNOUNCEMENT] [_instance] is NULL, does it exist in the scene?");
			}
			return AnnouncementManager._instance;
		}
	}

	// Token: 0x170004D8 RID: 1240
	// (get) Token: 0x0600306C RID: 12396 RVA: 0x000EF214 File Offset: 0x000ED414
	private static string AnnouncementDPlayerPref
	{
		get
		{
			if (string.IsNullOrEmpty(AnnouncementManager._announcementIDPref))
			{
				AnnouncementManager._announcementIDPref = "announcement-id-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return AnnouncementManager._announcementIDPref;
		}
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x000EF242 File Offset: 0x000ED442
	private void Awake()
	{
		if (AnnouncementManager._instance != null)
		{
			Debug.LogError("[KID::ANNOUNCEMENT] [AnnouncementManager] has already been setup, does another already exist in the scene?");
			return;
		}
		AnnouncementManager._instance = this;
		if (this._announcementMessageBox == null)
		{
			Debug.LogError("[ANNOUNCEMENT] Announcement Message Box has not been set. Announcement system will not work without it");
		}
	}

	// Token: 0x0600306E RID: 12398 RVA: 0x000EF27C File Offset: 0x000ED47C
	private void Start()
	{
		if (this._announcementMessageBox == null)
		{
			return;
		}
		this._announcementMessageBox.RightButton = "";
		this._announcementMessageBox.LeftButton = "Continue";
		PlayFabTitleDataCache.Instance.GetTitleData("AnnouncementData", new Action<string>(this.ConfigureAnnouncement), new Action<PlayFabError>(this.OnError));
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x000EF2E0 File Offset: 0x000ED4E0
	public void OnContinuePressed()
	{
		HandRayController.Instance.DisableHandRays();
		if (this._announcementMessageBox == null)
		{
			Debug.LogError("[ANNOUNCEMENT] Message Box is null, Continue Button cannot work");
			return;
		}
		PrivateUIRoom.RemoveUI(this._announcementMessageBox.transform);
		this._announcementActive = false;
		PlayerPrefs.SetString(AnnouncementManager.AnnouncementDPlayerPref, this._announcementData.AnnouncementID);
		PlayerPrefs.Save();
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x000EF341 File Offset: 0x000ED541
	private void OnError(PlayFabError error)
	{
		Debug.LogError("[ANNOUNCEMENT] Failed to Get Title Data for key [AnnouncementData]. Error:\n[" + error.ErrorMessage);
		this._completedSetup = true;
	}

	// Token: 0x06003071 RID: 12401 RVA: 0x000EF360 File Offset: 0x000ED560
	private void ConfigureAnnouncement(string data)
	{
		this._announcementString = data;
		this._announcementData = JsonMapper.ToObject<SAnnouncementData>(this._announcementString);
		if (!bool.TryParse(this._announcementData.ShowAnnouncement, out this._showAnnouncement))
		{
			this._completedSetup = true;
			Debug.LogError("[ANNOUNCEMENT] Failed to parse [ShowAnnouncement] with value [" + this._announcementData.ShowAnnouncement + "] to a bool, assuming false");
			return;
		}
		if (!this.ShowAnnouncement())
		{
			this._completedSetup = true;
			return;
		}
		if (string.IsNullOrEmpty(this._announcementData.AnnouncementID))
		{
			this._completedSetup = true;
			Debug.LogError("[ANNOUNCEMENT] Announcement Version is empty or null. Will not show announcement");
			return;
		}
		string @string = PlayerPrefs.GetString(AnnouncementManager.AnnouncementDPlayerPref, "");
		if (this._announcementData.AnnouncementID == @string)
		{
			this._completedSetup = true;
			return;
		}
		PrivateUIRoom.ForceStartOverlay();
		HandRayController.Instance.EnableHandRays();
		this._announcementMessageBox.Header = this._announcementData.AnnouncementTitle;
		this._announcementMessageBox.Body = this._announcementData.Message;
		this._announcementActive = true;
		PrivateUIRoom.AddUI(this._announcementMessageBox.transform);
		this._completedSetup = true;
	}

	// Token: 0x04003699 RID: 13977
	private const string ANNOUNCEMENT_ID_PLAYERPREF_PREFIX = "announcement-id-";

	// Token: 0x0400369A RID: 13978
	private const string ANNOUNCEMENT_TITLE_DATA_KEY = "AnnouncementData";

	// Token: 0x0400369B RID: 13979
	private const string ANNOUNCEMENT_HEADING = "Announcement!";

	// Token: 0x0400369C RID: 13980
	private const string ANNOUNCEMENT_BUTTON_TEXT = "Continue";

	// Token: 0x0400369D RID: 13981
	[SerializeField]
	private MessageBox _announcementMessageBox;

	// Token: 0x0400369E RID: 13982
	private string _announcementString = string.Empty;

	// Token: 0x0400369F RID: 13983
	private SAnnouncementData _announcementData;

	// Token: 0x040036A0 RID: 13984
	private bool _showAnnouncement;

	// Token: 0x040036A3 RID: 13987
	private static AnnouncementManager _instance;

	// Token: 0x040036A4 RID: 13988
	private static string _announcementIDPref = "";
}
