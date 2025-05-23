using System;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTagScripts.ModIO;
using ModIO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000732 RID: 1842
public class CustomMapsTerminal : MonoBehaviour
{
	// Token: 0x17000484 RID: 1156
	// (get) Token: 0x06002DFD RID: 11773 RVA: 0x000E585A File Offset: 0x000E3A5A
	public static int LocalPlayerID
	{
		get
		{
			return NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x17000485 RID: 1157
	// (get) Token: 0x06002DFE RID: 11774 RVA: 0x000E586B File Offset: 0x000E3A6B
	public static bool IsDriver
	{
		get
		{
			return CustomMapsTerminal.localDriverID == CustomMapsTerminal.LocalPlayerID;
		}
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x000E5879 File Offset: 0x000E3A79
	private void Awake()
	{
		CustomMapsTerminal.instance = this;
		CustomMapsTerminal.hasInstance = true;
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x000E5888 File Offset: 0x000E3A88
	private void Start()
	{
		CustomMapsTerminal.localDriverID = -2;
		CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.Access;
		CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.ScreenType.Access;
		CustomMapsTerminal.localStatus.modsPerPage = CustomMapsTerminal.instance.modListScreen.ModsPerPage;
		CustomMapsTerminal.cachedNetStatus.modsPerPage = CustomMapsTerminal.localStatus.modsPerPage;
		CustomMapsTerminal.HideTerminalControlScreen();
		this.accessScreen.Show();
		this.modListScreen.Hide();
		this.modDetailsScreen.Hide();
		GameEvents.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		GameEvents.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnReturnedToSinglePlayer;
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x000E5960 File Offset: 0x000E3B60
	private void LateUpdate()
	{
		if (CustomMapsTerminal.localDriverID == -2)
		{
			return;
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		if (CustomMapsTerminal.useNametags == GorillaComputer.instance.NametagsEnabled)
		{
			return;
		}
		CustomMapsTerminal.useNametags = GorillaComputer.instance.NametagsEnabled;
		CustomMapsTerminal.RefreshDriverNickName();
	}

	// Token: 0x06002E02 RID: 11778 RVA: 0x000E59B4 File Offset: 0x000E3BB4
	private void OnDestroy()
	{
		GameEvents.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		GameEvents.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnReturnedToSinglePlayer;
	}

	// Token: 0x06002E03 RID: 11779 RVA: 0x000E5A1C File Offset: 0x000E3C1C
	public static CustomMapsTerminal.TerminalStatus UpdateAndRetrieveLocalStatus()
	{
		CustomMapsTerminal.localStatus.sortType = CustomMapsTerminal.instance.modListScreen.SortType;
		if (CustomMapsTerminal.instance.modDetailsScreen.isActiveAndEnabled)
		{
			CustomMapsTerminal.localStatus.modDetailsID = CustomMapsTerminal.instance.modDetailsScreen.GetModId();
		}
		if (CustomMapsTerminal.instance.modListScreen.isActiveAndEnabled)
		{
			CustomMapsTerminal.localStatus.modIndex = CustomMapsTerminal.instance.modListScreen.SelectedModIndex;
			CustomMapsTerminal.localStatus.pageIndex = CustomMapsTerminal.instance.modListScreen.CurrentModPage;
			if (CustomMapsTerminal.instance.modListScreen.currentState == CustomMapsListScreen.ListScreenState.SubscribedMods)
			{
				CustomMapsTerminal.instance.modListScreen.GetModList(out CustomMapsTerminal.localStatus.modList);
				CustomMapsTerminal.localStatus.numModPages = CustomMapsTerminal.instance.modListScreen.GetNumPages();
			}
			else
			{
				CustomMapsTerminal.localStatus.modList = Array.Empty<long>();
				CustomMapsTerminal.localStatus.numModPages = -1;
			}
		}
		return CustomMapsTerminal.localStatus;
	}

	// Token: 0x06002E04 RID: 11780 RVA: 0x000E5B17 File Offset: 0x000E3D17
	public static void UpdateListScreenState(CustomMapsListScreen.ListScreenState screenState)
	{
		CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.localStatus.currentScreen;
		CustomMapsTerminal.localStatus.currentScreen = ((screenState == CustomMapsListScreen.ListScreenState.AvailableMods) ? CustomMapsTerminal.ScreenType.AvailableMods : CustomMapsTerminal.ScreenType.SubscribedMods);
		CustomMapsTerminal.UpdateSubscriptionButtonStatus(screenState);
	}

	// Token: 0x06002E05 RID: 11781 RVA: 0x000E5B44 File Offset: 0x000E3D44
	public static void ShowDetailsScreen(ModProfile profile)
	{
		CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.localStatus.currentScreen;
		CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.ModDetails;
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.accessScreen.Hide();
		CustomMapsTerminal.instance.modDetailsScreen.Show();
		CustomMapsTerminal.instance.modDetailsScreen.SetModProfile(profile);
		CustomMapsTerminal.SendTerminalStatus(false, false);
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x000E5BB4 File Offset: 0x000E3DB4
	public static void ReturnFromDetailsScreen()
	{
		CustomMapsTerminal.ScreenType previousScreen = CustomMapsTerminal.localStatus.previousScreen;
		if (previousScreen == CustomMapsTerminal.ScreenType.ModDetails || previousScreen == CustomMapsTerminal.ScreenType.Invalid || previousScreen == CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			CustomMapsTerminal.localStatus.pageIndex = 0;
			CustomMapsTerminal.localStatus.modIndex = 0;
		}
		else
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.localStatus.previousScreen;
		}
		switch (CustomMapsTerminal.localStatus.currentScreen)
		{
		case CustomMapsTerminal.ScreenType.Access:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.accessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.AvailableMods:
			CustomMapsTerminal.instance.modListScreen.UpdateFromTerminalStatus(CustomMapsTerminal.localStatus);
			CustomMapsTerminal.instance.modListScreen.Show();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.accessScreen.Hide();
			break;
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			CustomMapsTerminal.instance.modListScreen.UpdateFromTerminalStatus(CustomMapsTerminal.localStatus);
			CustomMapsTerminal.instance.modListScreen.Show();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.accessScreen.Hide();
			break;
		}
		CustomMapsTerminal.SendTerminalStatus(CustomMapsTerminal.localStatus.currentScreen == CustomMapsTerminal.ScreenType.SubscribedMods, false);
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x000E5D0C File Offset: 0x000E3F0C
	public static bool SetTagButtonStatus(short tagIndex, out string tagText)
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			tagText = string.Empty;
			return false;
		}
		int num = (int)MathF.Max(0f, (float)(tagIndex - 1));
		short num2 = (short)MathF.Pow(2f, (float)num);
		int num3;
		bool flag;
		if ((CustomMapsTerminal.localStatus.tagFlags & num2) == 0)
		{
			num3 = (int)(CustomMapsTerminal.localStatus.tagFlags | num2);
			flag = true;
		}
		else
		{
			num3 = (int)(CustomMapsTerminal.localStatus.tagFlags & ~(int)num2);
			flag = false;
		}
		CustomMapsTerminal.localStatus.tagFlags = (short)num3;
		CustomMapsTerminal.instance.tagButtons[num].SetButtonStatus(flag);
		tagText = CustomMapsTerminal.instance.tagButtons[num].tagText;
		return flag;
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x000E5DB2 File Offset: 0x000E3FB2
	public static void SendTerminalStatus(bool sendFullModList = false, bool forceSearch = false)
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.instance.mapTerminalNetworkObject.SendTerminalStatus(sendFullModList, forceSearch);
	}

	// Token: 0x06002E09 RID: 11785 RVA: 0x000E5DCD File Offset: 0x000E3FCD
	public static void ResetTerminalControl()
	{
		CustomMapsTerminal.localDriverID = -2;
		CustomMapsTerminal.instance.terminalControlButton.UnlockTerminalControl();
		CustomMapsTerminal.ShowTerminalControlScreen();
	}

	// Token: 0x06002E0A RID: 11786 RVA: 0x000E5DEA File Offset: 0x000E3FEA
	public static void HandleTerminalControlStatusChangeRequest(bool lockedStatus, int playerID)
	{
		if (lockedStatus && playerID == -2)
		{
			return;
		}
		if (CustomMapsTerminal.localDriverID == -2)
		{
			if (!lockedStatus)
			{
				return;
			}
		}
		else if (CustomMapsTerminal.localDriverID != playerID)
		{
			return;
		}
		CustomMapsTerminal.SetTerminalControlStatus(lockedStatus, playerID, true);
	}

	// Token: 0x06002E0B RID: 11787 RVA: 0x000E5E14 File Offset: 0x000E4014
	public static void SetTerminalControlStatus(bool isLocked, int driverID = -2, bool sendRPC = false)
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return;
		}
		if (isLocked)
		{
			CustomMapsTerminal.localDriverID = driverID;
			CustomMapsTerminal.instance.terminalControlButton.LockTerminalControl();
			CustomMapsTerminal.HideTerminalControlScreen();
		}
		else
		{
			CustomMapsTerminal.localDriverID = -2;
			CustomMapsTerminal.instance.terminalControlButton.UnlockTerminalControl();
			CustomMapsTerminal.ShowTerminalControlScreen();
		}
		if (sendRPC && NetworkSystem.Instance.IsMasterClient)
		{
			CustomMapsTerminal.instance.mapTerminalNetworkObject.SetTerminalControlStatus(isLocked, CustomMapsTerminal.localDriverID);
		}
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x000E5E88 File Offset: 0x000E4088
	public static void UpdateStatusFromDriver(long[] data, int driverID, bool forceSearch = false)
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.localDriverID = driverID;
		CustomMapsTerminal.cachedNetStatus.UnpackData(data);
		if (!ModIOManager.IsLoggedIn())
		{
			return;
		}
		CustomMapsTerminal.localStatus.UnpackData(data);
		if (CustomMapsTerminal.localDriverID != -2)
		{
			CustomMapsTerminal.RefreshDriverNickName();
		}
		CustomMapsTerminal.instance.UpdateScreenToMatchStatus(forceSearch);
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x000E5EDA File Offset: 0x000E40DA
	public static void ClearTags()
	{
		CustomMapsTerminal.localTagStatus = 0;
		CustomMapsTerminal.instance.modListScreen.ClearTags(true);
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x000E5EF4 File Offset: 0x000E40F4
	private void UpdateTagsAndSortFromDriver()
	{
		CustomMapsTerminal.instance.modListScreen.SortType = CustomMapsTerminal.localStatus.sortType;
		if (CustomMapsTerminal.localTagStatus == CustomMapsTerminal.localStatus.tagFlags)
		{
			return;
		}
		CustomMapsTerminal.localTagStatus = CustomMapsTerminal.localStatus.tagFlags;
		List<string> list = new List<string>();
		for (int i = 0; i < 8; i++)
		{
			int num = (int)Mathf.Pow(2f, (float)i);
			if (((int)CustomMapsTerminal.localStatus.tagFlags & num) == 0)
			{
				CustomMapsTerminal.instance.tagButtons[i].SetButtonStatus(false);
			}
			else
			{
				CustomMapsTerminal.instance.tagButtons[i].SetButtonStatus(true);
				list.Add(CustomMapsTerminal.instance.tagButtons[i].tagText);
			}
		}
		CustomMapsTerminal.instance.modListScreen.UpdateTagsFromDriver(list);
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x000E5FC4 File Offset: 0x000E41C4
	private void UpdateScreenToMatchStatus(bool forceSearch = false)
	{
		if (CustomMapsTerminal.localDriverID == -2)
		{
			this.terminalControlButton.UnlockTerminalControl();
		}
		else
		{
			this.terminalControlButton.LockTerminalControl();
		}
		this.ValidateLocalStatus();
		this.UpdateTagsAndSortFromDriver();
		CustomMapsTerminal.UpdateSubscriptionButtonStatus(CustomMapsListScreen.ListScreenState.AvailableMods);
		switch (CustomMapsTerminal.localStatus.currentScreen)
		{
		case CustomMapsTerminal.ScreenType.Access:
			this.modListScreen.Hide();
			this.modDetailsScreen.Hide();
			this.accessScreen.Show();
			return;
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			this.modListScreen.Hide();
			this.modDetailsScreen.Hide();
			this.accessScreen.Show();
			this.accessScreen.ShowTerminalControlPrompt();
			return;
		case CustomMapsTerminal.ScreenType.AvailableMods:
			this.accessScreen.Hide();
			this.modDetailsScreen.Hide();
			this.modListScreen.UpdateFromTerminalStatus(CustomMapsTerminal.localStatus);
			this.modListScreen.Show();
			if (forceSearch)
			{
				this.modListScreen.Refresh(null);
			}
			return;
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			this.accessScreen.Hide();
			this.modDetailsScreen.Hide();
			this.modListScreen.UpdateFromTerminalStatus(CustomMapsTerminal.localStatus);
			this.modListScreen.Show();
			CustomMapsTerminal.UpdateSubscriptionButtonStatus(CustomMapsListScreen.ListScreenState.SubscribedMods);
			if (forceSearch)
			{
				this.modListScreen.Refresh(CustomMapsTerminal.localStatus.modList);
				return;
			}
			if (!CustomMapsTerminal.IsDriver && !this.modListScreen.DoesModListMatchDisplay(CustomMapsTerminal.localStatus.modList))
			{
				this.modListScreen.ShowCustomModList(CustomMapsTerminal.localStatus.modList);
			}
			return;
		case CustomMapsTerminal.ScreenType.ModDetails:
			this.accessScreen.Hide();
			this.modListScreen.UpdateFromTerminalStatus(CustomMapsTerminal.localStatus);
			this.modListScreen.Hide();
			this.modDetailsScreen.Show();
			this.modDetailsScreen.RetrieveProfileFromModIO(CustomMapsTerminal.localStatus.modDetailsID, null);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x000E6188 File Offset: 0x000E4388
	private void ValidateLocalStatus()
	{
		if (!ModIOManager.IsLoggedIn() || CustomMapsTerminal.localDriverID == -2)
		{
			return;
		}
		if (CustomMapLoader.IsModLoaded(0L))
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localStatus.modDetailsID = CustomMapLoader.LoadedMapModId;
			CustomMapsTerminal.SendTerminalStatus(false, false);
			return;
		}
		if (CustomMapManager.IsLoading(0L))
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localStatus.modDetailsID = CustomMapManager.LoadingMapId;
			CustomMapsTerminal.SendTerminalStatus(false, false);
			return;
		}
		if (CustomMapManager.GetRoomMapId() != ModId.Null)
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localStatus.modDetailsID = CustomMapManager.GetRoomMapId().id;
			CustomMapsTerminal.SendTerminalStatus(false, false);
		}
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x000E6234 File Offset: 0x000E4434
	private void OnModIOLoggedIn()
	{
		if (CustomMapsTerminal.localStatus.currentScreen == CustomMapsTerminal.ScreenType.Access)
		{
			if (!NetworkSystem.Instance.InRoom || CustomMapsTerminal.cachedNetStatus.currentScreen == CustomMapsTerminal.ScreenType.Invalid)
			{
				CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
				CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
				CustomMapsTerminal.localStatus.modIndex = 0;
			}
			else
			{
				CustomMapsTerminal.localStatus.Copy(CustomMapsTerminal.cachedNetStatus);
				this.UpdateScreenToMatchStatus(false);
			}
			if (CustomMapsTerminal.localDriverID == -2)
			{
				CustomMapsTerminal.ShowTerminalControlScreen();
			}
		}
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x000E62AD File Offset: 0x000E44AD
	private void OnModIOLoggedOut()
	{
		CustomMapsTerminal.ResetTerminalControl();
		CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.Access;
		CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.ScreenType.Access;
		this.accessScreen.Reset();
		this.UpdateScreenToMatchStatus(false);
	}

	// Token: 0x06002E13 RID: 11795 RVA: 0x000E62DC File Offset: 0x000E44DC
	public void HandleTerminalControlButtonPressed()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			this.accessScreen.DisplayError("User is logged out of mod.io, not allowing terminal check-out");
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			CustomMapsTerminal.SetTerminalControlStatus(!this.terminalControlButton.IsLocked, CustomMapsTerminal.LocalPlayerID, false);
			return;
		}
		if (CustomMapsTerminal.localDriverID != -2 && !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (this.mapTerminalNetworkObject.HasAuthority)
		{
			CustomMapsTerminal.HandleTerminalControlStatusChangeRequest(!this.terminalControlButton.IsLocked, CustomMapsTerminal.LocalPlayerID);
			return;
		}
		this.mapTerminalNetworkObject.RequestTerminalControlStatusChange(!this.terminalControlButton.IsLocked);
	}

	// Token: 0x06002E14 RID: 11796 RVA: 0x000E6377 File Offset: 0x000E4577
	private static void UpdateSubscriptionButtonStatus(CustomMapsListScreen.ListScreenState screenState)
	{
		if (CustomMapsTerminal.hasInstance && CustomMapsTerminal.instance.subscribedOnlyButton != null)
		{
			CustomMapsTerminal.instance.subscribedOnlyButton.SetButtonStatus(screenState == CustomMapsListScreen.ListScreenState.SubscribedMods);
		}
	}

	// Token: 0x06002E15 RID: 11797 RVA: 0x000E63A8 File Offset: 0x000E45A8
	private static void ShowTerminalControlScreen()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.instance.terminalControllerLabelText.gameObject.SetActive(false);
		CustomMapsTerminal.instance.terminalControllerText.gameObject.SetActive(false);
		if (!ModIOManager.IsLoggedIn())
		{
			return;
		}
		if (CustomMapsTerminal.localStatus.currentScreen > CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.localStatus.currentScreen;
		}
		else
		{
			CustomMapsTerminal.localStatus.previousScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
		}
		CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
		CustomMapsTerminal.instance.UpdateScreenToMatchStatus(false);
	}

	// Token: 0x06002E16 RID: 11798 RVA: 0x000E6434 File Offset: 0x000E4634
	private static void HideTerminalControlScreen()
	{
		if (!CustomMapsTerminal.hasInstance || !ModIOManager.IsLoggedIn())
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
		if (CustomMapsTerminal.localStatus.currentScreen != CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			return;
		}
		if (CustomMapsTerminal.localStatus.previousScreen > CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.localStatus.previousScreen;
			if (CustomMapsTerminal.localStatus.currentScreen == CustomMapsTerminal.ScreenType.SubscribedMods)
			{
				CustomMapsTerminal.localStatus.modIndex = 0;
			}
		}
		else if (CustomMapLoader.IsModLoaded(0L) || CustomMapManager.IsLoading(0L) || CustomMapManager.GetRoomMapId() != ModId.Null)
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.ModDetails;
		}
		else
		{
			CustomMapsTerminal.localStatus.currentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		CustomMapsTerminal.instance.terminalControllerLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(CustomMapsTerminal.localDriverID);
			CustomMapsTerminal.instance.terminalControllerText.text = ((CustomMapsTerminal.useNametags && flag) ? netPlayerByID.NickName : netPlayerByID.DefaultName);
		}
		else
		{
			CustomMapsTerminal.instance.terminalControllerText.text = ((CustomMapsTerminal.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
		}
		CustomMapsTerminal.instance.terminalControllerText.gameObject.SetActive(true);
		CustomMapsTerminal.instance.UpdateScreenToMatchStatus(false);
	}

	// Token: 0x06002E17 RID: 11799 RVA: 0x000E6591 File Offset: 0x000E4791
	public static void RequestDriverNickNameRefresh()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
		CustomMapsTerminal.instance.mapTerminalNetworkObject.RefreshDriverNickName();
	}

	// Token: 0x06002E18 RID: 11800 RVA: 0x000E65B8 File Offset: 0x000E47B8
	public static void RefreshDriverNickName()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		CustomMapsTerminal.instance.terminalControllerLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(CustomMapsTerminal.localDriverID);
			CustomMapsTerminal.instance.terminalControllerText.text = netPlayerByID.DefaultName;
			if (CustomMapsTerminal.useNametags && flag)
			{
				RigContainer rigContainer;
				if (netPlayerByID.IsLocal)
				{
					CustomMapsTerminal.instance.terminalControllerText.text = netPlayerByID.NickName;
				}
				else if (VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					CustomMapsTerminal.instance.terminalControllerText.text = rigContainer.Rig.playerNameVisible;
				}
			}
		}
		else
		{
			CustomMapsTerminal.instance.terminalControllerText.text = ((CustomMapsTerminal.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
		}
		CustomMapsTerminal.instance.terminalControllerText.gameObject.SetActive(true);
		CustomMapsTerminal.instance.modListScreen.RefreshDriverNickname(CustomMapsTerminal.instance.terminalControllerText.text);
	}

	// Token: 0x06002E19 RID: 11801 RVA: 0x000E66DB File Offset: 0x000E48DB
	private void OnReturnedToSinglePlayer()
	{
		if (CustomMapsTerminal.localDriverID != CustomMapsTerminal.cachedLocalPlayerID)
		{
			CustomMapsTerminal.ResetTerminalControl();
		}
		else
		{
			CustomMapsTerminal.localDriverID = CustomMapsTerminal.LocalPlayerID;
		}
		CustomMapsTerminal.cachedLocalPlayerID = -1;
	}

	// Token: 0x06002E1A RID: 11802 RVA: 0x000E6700 File Offset: 0x000E4900
	private void OnJoinedRoom()
	{
		CustomMapsTerminal.cachedLocalPlayerID = CustomMapsTerminal.LocalPlayerID;
		CustomMapsTerminal.ResetTerminalControl();
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x000E6711 File Offset: 0x000E4911
	public static bool IsLocked()
	{
		return CustomMapsTerminal.localDriverID != -2;
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x000E671F File Offset: 0x000E491F
	public static int GetDriverID()
	{
		return CustomMapsTerminal.localDriverID;
	}

	// Token: 0x06002E1D RID: 11805 RVA: 0x000E6726 File Offset: 0x000E4926
	public static string GetDriverNickname()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return "";
		}
		return CustomMapsTerminal.instance.terminalControllerText.text;
	}

	// Token: 0x0400346B RID: 13419
	[SerializeField]
	private CustomMapsAccessScreen accessScreen;

	// Token: 0x0400346C RID: 13420
	[SerializeField]
	private CustomMapsListScreen modListScreen;

	// Token: 0x0400346D RID: 13421
	[SerializeField]
	private CustomMapsDetailsScreen modDetailsScreen;

	// Token: 0x0400346E RID: 13422
	[SerializeField]
	private VirtualStumpSerializer mapTerminalNetworkObject;

	// Token: 0x0400346F RID: 13423
	[SerializeField]
	private CustomMapsTerminalControlButton terminalControlButton;

	// Token: 0x04003470 RID: 13424
	[SerializeField]
	private TMP_Text terminalControllerLabelText;

	// Token: 0x04003471 RID: 13425
	[SerializeField]
	private TMP_Text terminalControllerText;

	// Token: 0x04003472 RID: 13426
	[SerializeField]
	private List<CustomMapsTerminalTagButton> tagButtons = new List<CustomMapsTerminalTagButton>();

	// Token: 0x04003473 RID: 13427
	[SerializeField]
	private CustomMapsTerminalToggleButton subscribedOnlyButton;

	// Token: 0x04003474 RID: 13428
	public const int NO_DRIVER_ID = -2;

	// Token: 0x04003475 RID: 13429
	private const short NUM_OF_TAGS = 8;

	// Token: 0x04003476 RID: 13430
	private static CustomMapsTerminal instance;

	// Token: 0x04003477 RID: 13431
	private static bool hasInstance;

	// Token: 0x04003478 RID: 13432
	private static CustomMapsTerminal.TerminalStatus localStatus = new CustomMapsTerminal.TerminalStatus();

	// Token: 0x04003479 RID: 13433
	private static CustomMapsTerminal.TerminalStatus cachedNetStatus = new CustomMapsTerminal.TerminalStatus();

	// Token: 0x0400347A RID: 13434
	private static int localDriverID = -1;

	// Token: 0x0400347B RID: 13435
	private static int cachedLocalPlayerID = -1;

	// Token: 0x0400347C RID: 13436
	private static bool useNametags;

	// Token: 0x0400347D RID: 13437
	private static short localTagStatus = 0;

	// Token: 0x02000733 RID: 1843
	public enum ScreenType
	{
		// Token: 0x0400347F RID: 13439
		Invalid = -1,
		// Token: 0x04003480 RID: 13440
		Access,
		// Token: 0x04003481 RID: 13441
		TerminalControlPrompt,
		// Token: 0x04003482 RID: 13442
		AvailableMods,
		// Token: 0x04003483 RID: 13443
		SubscribedMods,
		// Token: 0x04003484 RID: 13444
		ModDetails
	}

	// Token: 0x02000734 RID: 1844
	public class TerminalStatus
	{
		// Token: 0x06002E20 RID: 11808 RVA: 0x000E6780 File Offset: 0x000E4980
		public long[] PackData(bool packModList)
		{
			long[] array;
			if (packModList && !this.modList.IsNullOrEmpty<long>())
			{
				array = new long[3 + this.modList.Length];
				for (int i = 3; i < array.Length; i++)
				{
					array[i] = this.modList[i - 3];
				}
			}
			else
			{
				array = new long[3];
			}
			array[0] = (long)this.currentScreen;
			array[0] += (long)((long)this.previousScreen << 4);
			array[0] += (long)((long)(this.modIndex + 1) << 8);
			array[0] += (long)((long)(this.numModPages + 1) << 16);
			array[0] += (long)(this.pageIndex + 1) << 32;
			array[1] = this.modDetailsID;
			array[2] = (long)this.tagFlags;
			array[2] += (long)this.sortType << 32;
			return array;
		}

		// Token: 0x06002E21 RID: 11809 RVA: 0x000E685C File Offset: 0x000E4A5C
		public void UnpackData(long[] data)
		{
			if (data.Length < 3 || data.Length > 3 + this.modsPerPage)
			{
				return;
			}
			int num = (int)(data[0] & 15L);
			this.currentScreen = (CustomMapsTerminal.ScreenType)((num >= -1 && num <= 4) ? num : (-1));
			num = (int)((data[0] >> 4) & 15L);
			this.previousScreen = (CustomMapsTerminal.ScreenType)((num >= -1 && num <= 4) ? num : (-1));
			this.modIndex = (int)((data[0] >> 8) & 255L);
			this.modIndex = Mathf.Clamp(this.modIndex - 1, -1, this.modsPerPage);
			this.numModPages = (int)((data[0] >> 16) & 65535L);
			this.numModPages = Mathf.Clamp(this.numModPages - 1, -1, 65535);
			this.pageIndex = (int)(data[0] >> 32);
			this.pageIndex = Mathf.Max(this.pageIndex - 1, -1);
			this.modDetailsID = ((data[1] > 0L) ? data[1] : 0L);
			this.tagFlags = (short)Mathf.Clamp((float)(data[2] & 255L), 0f, 255f);
			num = (int)(data[2] >> 32);
			this.sortType = (SortModsBy)((num >= 0 && num <= 6) ? num : 3);
			if (data.Length <= 3)
			{
				return;
			}
			this.modList = new long[data.Length - 3];
			for (int i = 0; i < this.modList.Length; i++)
			{
				this.modList[i] = data[i + 3];
			}
		}

		// Token: 0x06002E22 RID: 11810 RVA: 0x000E69B8 File Offset: 0x000E4BB8
		public void Copy(CustomMapsTerminal.TerminalStatus other)
		{
			this.currentScreen = other.currentScreen;
			this.previousScreen = other.previousScreen;
			this.modIndex = other.modIndex;
			this.numModPages = other.numModPages;
			this.pageIndex = other.pageIndex;
			this.modDetailsID = other.modDetailsID;
			this.modList = other.modList;
			this.tagFlags = other.tagFlags;
			this.sortType = other.sortType;
		}

		// Token: 0x04003485 RID: 13445
		public CustomMapsTerminal.ScreenType currentScreen = CustomMapsTerminal.ScreenType.Invalid;

		// Token: 0x04003486 RID: 13446
		public CustomMapsTerminal.ScreenType previousScreen = CustomMapsTerminal.ScreenType.Invalid;

		// Token: 0x04003487 RID: 13447
		public int modIndex;

		// Token: 0x04003488 RID: 13448
		public int pageIndex;

		// Token: 0x04003489 RID: 13449
		public long modDetailsID;

		// Token: 0x0400348A RID: 13450
		public long[] modList;

		// Token: 0x0400348B RID: 13451
		public int numModPages = -1;

		// Token: 0x0400348C RID: 13452
		public short tagFlags = 128;

		// Token: 0x0400348D RID: 13453
		public SortModsBy sortType = SortModsBy.Popular;

		// Token: 0x0400348E RID: 13454
		private const int MINIMUM_ARRAY_LENGTH = 3;

		// Token: 0x0400348F RID: 13455
		public int modsPerPage;
	}
}
