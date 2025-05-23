using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTagScripts.ModIO;
using ModIO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200072D RID: 1837
public class CustomMapsDetailsScreen : CustomMapsTerminalScreen
{
	// Token: 0x1700047F RID: 1151
	// (get) Token: 0x06002DAE RID: 11694 RVA: 0x000E26BE File Offset: 0x000E08BE
	// (set) Token: 0x06002DAD RID: 11693 RVA: 0x000E26B5 File Offset: 0x000E08B5
	public ModProfile currentModProfile { get; private set; }

	// Token: 0x06002DAF RID: 11695 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void Initialize()
	{
	}

	// Token: 0x06002DB0 RID: 11696 RVA: 0x000E26C8 File Offset: 0x000E08C8
	public override void Show()
	{
		base.Show();
		GameEvents.ModIOModManagementEvent.RemoveListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloaded));
		GameEvents.ModIOModManagementEvent.AddListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.AddListener(new UnityAction(this.OnMapUnloaded));
		for (int i = 0; i < this.buttonsToHide.Length; i++)
		{
			this.buttonsToHide[i].SetActive(false);
		}
		for (int j = 0; j < this.buttonsToShow.Length; j++)
		{
			this.buttonsToShow[j].SetActive(true);
		}
		this.ResetToDefaultView();
	}

	// Token: 0x06002DB1 RID: 11697 RVA: 0x000E2800 File Offset: 0x000E0A00
	public override void Hide()
	{
		base.Hide();
		GameEvents.ModIOModManagementEvent.RemoveListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloaded));
		this.lastCompletedOperation = ModManagementOperationType.None_ErrorOcurred;
	}

	// Token: 0x06002DB2 RID: 11698 RVA: 0x000E2888 File Offset: 0x000E0A88
	private void HandleModManagementEvent(ModManagementEventType eventType, ModId modId, Result result)
	{
		if (base.isActiveAndEnabled && this.hasModProfile && this.currentModProfile.id.id == modId.id)
		{
			this.UpdateSubscriptionStatus(eventType == ModManagementEventType.InstallFailed || eventType == ModManagementEventType.UninstallFailed || eventType == ModManagementEventType.DownloadFailed || eventType == ModManagementEventType.UpdateFailed);
			if (result.errorCode == 20460U)
			{
				this.modDescriptionText.gameObject.SetActive(false);
				this.loadingMapLabelText.text = this.mapLoadingErrorString;
				this.loadingMapLabelText.gameObject.SetActive(true);
				this.loadingMapMessageText.text = this.mapLoadingErrorInvalidModFile;
				this.loadingMapMessageText.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06002DB3 RID: 11699 RVA: 0x000E2940 File Offset: 0x000E0B40
	private void Update()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.currentProgressHandle != null && this.currentProgressHandle.modId != this.currentModProfile.id)
		{
			this.currentProgressHandle = null;
		}
		if (this.currentProgressHandle == null)
		{
			if (!ModIOUnity.IsModManagementBusy())
			{
				return;
			}
			ProgressHandle currentModManagementOperation = ModIOUnity.GetCurrentModManagementOperation();
			if (currentModManagementOperation == null || currentModManagementOperation.Completed || !(currentModManagementOperation.modId == this.currentModProfile.id))
			{
				return;
			}
			this.currentProgressHandle = currentModManagementOperation;
		}
		string text;
		if (this.modOperationTypeStrings.TryGetValue(this.currentProgressHandle.OperationType, out text))
		{
			float num = this.currentProgressHandle.Progress * 100f;
			this.modStatusText.text = text + string.Format(" {0}%", Mathf.RoundToInt(num));
		}
		if (this.currentProgressHandle.Completed)
		{
			this.lastCompletedOperation = this.currentProgressHandle.OperationType;
			this.currentProgressHandle = null;
			this.UpdateSubscriptionStatus(false);
		}
	}

	// Token: 0x06002DB4 RID: 11700 RVA: 0x000E2A44 File Offset: 0x000E0C44
	public void RetrieveProfileFromModIO(long id, Action<ModIORequestResultAnd<ModProfile>> callback = null)
	{
		if (this.hasModProfile && this.currentModProfile.id == id)
		{
			this.UpdateModDetails(true);
			return;
		}
		this.pendingModId = id;
		ModIOManager.GetModProfile(new ModId(id), (callback != null) ? callback : new Action<ModIORequestResultAnd<ModProfile>>(this.OnProfileReceived));
	}

	// Token: 0x06002DB5 RID: 11701 RVA: 0x000E2A98 File Offset: 0x000E0C98
	public void SetModProfile(ModProfile modProfile)
	{
		if (modProfile.id != ModId.Null)
		{
			this.pendingModId = 0L;
			this.currentModProfile = modProfile;
			this.hasModProfile = true;
			this.UpdateModDetails(true);
		}
	}

	// Token: 0x06002DB6 RID: 11702 RVA: 0x000E2ACC File Offset: 0x000E0CCC
	protected override void PressButton(CustomMapsTerminalButton.ModIOKeyboardBindings buttonPressed)
	{
		if (!base.isActiveAndEnabled || !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapsTerminalButton.ModIOKeyboardBindings.goback)
		{
			if (CustomMapManager.IsLoading(0L))
			{
				return;
			}
			if (CustomMapManager.IsUnloading())
			{
				return;
			}
			if (this.mapLoadError)
			{
				this.mapLoadError = false;
				CustomMapManager.ClearRoomMap();
				this.ResetToDefaultView();
				return;
			}
			if (!CustomMapLoader.IsModLoaded(0L) && !(CustomMapManager.GetRoomMapId() != ModId.Null))
			{
				CustomMapsTerminal.ReturnFromDetailsScreen();
				this.hasModProfile = false;
				this.currentModProfile = default(ModProfile);
				return;
			}
			string text;
			if (!this.CanChangeMapState(false, out text))
			{
				this.modDescriptionText.gameObject.SetActive(false);
				this.errorText.text = text;
				this.errorText.gameObject.SetActive(true);
				return;
			}
			this.UnloadMod();
			return;
		}
		else
		{
			if (!this.hasModProfile)
			{
				return;
			}
			if (buttonPressed != CustomMapsTerminalButton.ModIOKeyboardBindings.option3)
			{
				if (buttonPressed == CustomMapsTerminalButton.ModIOKeyboardBindings.map)
				{
					if (CustomMapLoader.IsModLoaded(0L) || CustomMapManager.IsLoading(0L) || CustomMapManager.IsUnloading())
					{
						return;
					}
					this.errorText.gameObject.SetActive(false);
					this.errorText.text = "";
					this.loadingMapLabelText.gameObject.SetActive(false);
					this.loadingMapMessageText.gameObject.SetActive(false);
					this.modDescriptionText.gameObject.SetActive(true);
					ModIOManager.Refresh(delegate(bool result)
					{
						SubscribedModStatus subscribedModStatus2;
						if (ModIOManager.GetSubscribedModStatus(this.currentModProfile.id, out subscribedModStatus2))
						{
							ModIOManager.UnsubscribeFromMod(this.currentModProfile.id, delegate(Result result)
							{
								if (result.Succeeded())
								{
									this.UpdateModDetails(false);
								}
							});
							return;
						}
						ModIOManager.SubscribeToMod(this.currentModProfile.id, delegate(Result result)
						{
							if (result.Succeeded())
							{
								this.UpdateModDetails(false);
								ModIOManager.DownloadMod(this.currentModProfile.id, delegate(ModIORequestResult result)
								{
									GorillaTelemetry.PostCustomMapDownloadEvent(this.currentModProfile.name, this.currentModProfile.id, this.currentModProfile.creator.username);
									this.UpdateModDetails(false);
								});
							}
						});
					}, false);
				}
				SubscribedModStatus subscribedModStatus;
				if (buttonPressed == CustomMapsTerminalButton.ModIOKeyboardBindings.enter && !CustomMapManager.IsLoading(0L) && !CustomMapManager.IsUnloading() && !CustomMapLoader.IsModLoaded(0L) && this.lastCompletedOperation != ModManagementOperationType.Update && ModIOManager.GetSubscribedModStatus(this.currentModProfile.id, out subscribedModStatus))
				{
					if (subscribedModStatus == SubscribedModStatus.Installed)
					{
						string text2;
						if (!this.CanChangeMapState(true, out text2))
						{
							this.modDescriptionText.gameObject.SetActive(false);
							this.errorText.text = text2;
							this.errorText.gameObject.SetActive(true);
							return;
						}
						this.LoadMap();
						return;
					}
					else if (subscribedModStatus == SubscribedModStatus.WaitingToDownload || (subscribedModStatus == SubscribedModStatus.WaitingToUpdate && this.lastCompletedOperation != ModManagementOperationType.Update))
					{
						ModIOManager.DownloadMod(this.currentModProfile.id, delegate(ModIORequestResult result)
						{
							this.UpdateSubscriptionStatus(!result.success);
							GorillaTelemetry.PostCustomMapDownloadEvent(this.currentModProfile.name, this.currentModProfile.id, this.currentModProfile.creator.username);
						});
					}
				}
				return;
			}
			if (CustomMapLoader.IsModLoaded(0L) || CustomMapManager.IsLoading(0L) || CustomMapManager.IsUnloading() || CustomMapManager.GetRoomMapId() != ModId.Null)
			{
				return;
			}
			if (this.hasModProfile)
			{
				long currentModId = this.currentModProfile.id.id;
				this.hasModProfile = false;
				this.currentModProfile = default(ModProfile);
				ModIOManager.Refresh(delegate(bool result)
				{
					this.RetrieveProfileFromModIO(currentModId, null);
				}, false);
			}
			return;
		}
	}

	// Token: 0x06002DB7 RID: 11703 RVA: 0x000E2D64 File Offset: 0x000E0F64
	private void OnProfileReceived(ModIORequestResultAnd<ModProfile> profile)
	{
		if (profile.result.success)
		{
			this.SetModProfile(profile.data);
			return;
		}
		this.modDescriptionText.gameObject.SetActive(false);
		this.errorText.text = "Failed to retrieve mod details";
		this.errorText.gameObject.SetActive(true);
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x000E2DC0 File Offset: 0x000E0FC0
	private void ResetToDefaultView()
	{
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.modNameText.gameObject.SetActive(false);
		this.modCreatorLabelText.gameObject.SetActive(false);
		this.modCreatorText.gameObject.SetActive(false);
		this.modDescriptionText.gameObject.SetActive(false);
		this.modStatusText.gameObject.SetActive(false);
		this.modSubscriptionStatusText.gameObject.SetActive(false);
		this.OptionButtonTooltipText.gameObject.SetActive(false);
		this.mapScreenshotImage.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
		if (CustomMapLoader.IsModLoaded(0L) || CustomMapManager.IsLoading(0L) || CustomMapManager.IsUnloading())
		{
			ModId modId = new ModId(CustomMapLoader.IsModLoaded(0L) ? CustomMapLoader.LoadedMapModId : (CustomMapManager.IsLoading(0L) ? CustomMapManager.LoadingMapId : CustomMapManager.UnloadingMapId));
			if (this.hasModProfile && this.currentModProfile.id == modId)
			{
				this.UpdateModDetails(true);
				return;
			}
			this.RetrieveProfileFromModIO(modId, delegate(ModIORequestResultAnd<ModProfile> result)
			{
				this.OnProfileReceived(result);
			});
			return;
		}
		else
		{
			if (CustomMapManager.GetRoomMapId() != ModId.Null)
			{
				this.OnRoomMapChanged(CustomMapManager.GetRoomMapId());
				return;
			}
			if (this.hasModProfile)
			{
				this.UpdateModDetails(true);
			}
			return;
		}
	}

	// Token: 0x06002DB9 RID: 11705 RVA: 0x000E2F7C File Offset: 0x000E117C
	private void UpdateModDetails(bool refreshScreenState = true)
	{
		if (!this.hasModProfile)
		{
			return;
		}
		this.modNameText.text = this.currentModProfile.name;
		this.modCreatorText.text = this.currentModProfile.creator.username;
		this.modDescriptionText.text = this.currentModProfile.description;
		this.UpdateSubscriptionStatus(false);
		ModIOUnity.DownloadTexture(this.currentModProfile.logoImage320x180, new Action<ResultAnd<Texture2D>>(this.OnTextureDownloaded));
		if (refreshScreenState)
		{
			this.loadingText.gameObject.SetActive(false);
			this.loadingMapLabelText.gameObject.SetActive(false);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadRoomMapPromptText.gameObject.SetActive(false);
			this.mapReadyText.gameObject.SetActive(false);
			this.unloadPromptText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(false);
			this.modNameText.gameObject.SetActive(true);
			this.modCreatorLabelText.gameObject.SetActive(true);
			this.modCreatorText.gameObject.SetActive(true);
			this.modDescriptionText.gameObject.SetActive(true);
			if (CustomMapLoader.IsModLoaded(0L))
			{
				ModId modId = new ModId(CustomMapLoader.LoadedMapModId);
				if (this.currentModProfile.id == modId)
				{
					this.OnMapLoadComplete_UIUpdate();
					return;
				}
				this.RetrieveProfileFromModIO(modId, delegate(ModIORequestResultAnd<ModProfile> result)
				{
					this.OnProfileReceived(result);
				});
				return;
			}
			else
			{
				if (CustomMapManager.IsLoading(0L))
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.text = this.mapLoadingString + " 0%";
					this.loadingMapLabelText.gameObject.SetActive(true);
					return;
				}
				if (CustomMapManager.IsUnloading())
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.text = this.mapUnloadingString;
					this.loadingMapLabelText.gameObject.SetActive(true);
					return;
				}
				if (CustomMapManager.GetRoomMapId() != ModId.Null)
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadRoomMapPromptText.gameObject.SetActive(true);
					return;
				}
				if (this.mapLoadError)
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.gameObject.SetActive(true);
					this.loadingMapMessageText.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x06002DBA RID: 11706 RVA: 0x000E31F8 File Offset: 0x000E13F8
	private void OnTextureDownloaded(ResultAnd<Texture2D> resultAnd)
	{
		if (!resultAnd.result.Succeeded())
		{
			return;
		}
		Texture2D value = resultAnd.value;
		this.mapScreenshotImage.sprite = Sprite.Create(value, new Rect(0f, 0f, (float)value.width, (float)value.height), new Vector2(0.5f, 0.5f));
		this.mapScreenshotImage.gameObject.SetActive(true);
	}

	// Token: 0x06002DBB RID: 11707 RVA: 0x000E3268 File Offset: 0x000E1468
	private void UpdateSubscriptionStatus(bool errorEncountered = false)
	{
		if (base.isActiveAndEnabled)
		{
			bool flag = this.mapLoadError || CustomMapManager.IsUnloading() || CustomMapManager.IsLoading(0L) || CustomMapLoader.IsModLoaded(0L) || CustomMapManager.GetRoomMapId() != ModId.Null;
			this.outdatedText.gameObject.SetActive(false);
			if (!flag)
			{
				SubscribedModStatus subscribedModStatus2;
				bool subscribedModStatus = ModIOManager.GetSubscribedModStatus(this.currentModProfile.id, out subscribedModStatus2);
				if (errorEncountered)
				{
					this.lastCompletedOperation = ModManagementOperationType.None_ErrorOcurred;
					subscribedModStatus2 = SubscribedModStatus.ProblemOccurred;
				}
				this.modSubscriptionStatusText.text = (subscribedModStatus ? this.subscribedStatusString : this.unsubscribedStatusString);
				if (this.subscriptionToggleButton.IsNotNull())
				{
					this.subscriptionToggleButton.SetButtonStatus(subscribedModStatus);
				}
				if (subscribedModStatus)
				{
					switch (subscribedModStatus2)
					{
					case SubscribedModStatus.Installed:
					{
						this.selectButtonTooltipText.text = this.loadMapTooltipString;
						this.selectButtonTooltipText.gameObject.SetActive(true);
						this.lastCompletedOperation = ModManagementOperationType.None_ErrorOcurred;
						int num;
						if (ModIOManager.IsModOutdated(this.currentModProfile.id, out num))
						{
							this.outdatedText.gameObject.SetActive(true);
							goto IL_0186;
						}
						this.outdatedText.gameObject.SetActive(false);
						goto IL_0186;
					}
					case SubscribedModStatus.WaitingToDownload:
						this.selectButtonTooltipText.text = this.downloadMapTooltipString;
						this.selectButtonTooltipText.gameObject.SetActive(true);
						goto IL_0186;
					case SubscribedModStatus.WaitingToUpdate:
						this.selectButtonTooltipText.text = this.updateMapTooltipString;
						this.selectButtonTooltipText.gameObject.SetActive(true);
						goto IL_0186;
					}
					this.selectButtonTooltipText.gameObject.SetActive(false);
					IL_0186:
					string text;
					if (subscribedModStatus2 == SubscribedModStatus.WaitingToUpdate && this.lastCompletedOperation == ModManagementOperationType.Update)
					{
						this.modStatusText.text = "INSTALLING";
					}
					else if (this.modStatusStrings.TryGetValue(subscribedModStatus2, out text))
					{
						this.modStatusText.text = text;
					}
					else
					{
						this.modStatusText.text = "STATUS STRING MISSING!";
					}
					if (ModIOManager.IsModInDownloadQueue(this.currentModProfile.id))
					{
						this.selectButtonTooltipText.gameObject.SetActive(false);
						this.modStatusText.text = this.mapDownloadQueuedString;
					}
					this.OptionButtonTooltipText.text = this.unsubscribeTooltipString;
				}
				else
				{
					this.selectButtonTooltipText.gameObject.SetActive(false);
					this.modStatusText.text = this.modAvailableString;
					this.OptionButtonTooltipText.text = this.subscribeTooltipString;
				}
				this.modStatusText.gameObject.SetActive(true);
				this.OptionButtonTooltipText.gameObject.SetActive(true);
				this.modSubscriptionStatusText.gameObject.SetActive(true);
				return;
			}
			this.lastCompletedOperation = ModManagementOperationType.None_ErrorOcurred;
			this.selectButtonTooltipText.gameObject.SetActive(false);
			this.modStatusText.gameObject.SetActive(false);
			this.OptionButtonTooltipText.gameObject.SetActive(false);
			this.modSubscriptionStatusText.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002DBC RID: 11708 RVA: 0x000E3544 File Offset: 0x000E1744
	private bool CanChangeMapState(bool load, out string disallowedReason)
	{
		disallowedReason = "";
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			if (!CustomMapManager.AreAllPlayersInVirtualStump())
			{
				disallowedReason = "ALL PLAYERS IN THE ROOM MUST BE INSIDE THE VIRTUAL STUMP BEFORE " + (load ? "" : "UN") + "LOADING A MAP.";
				return false;
			}
			return true;
		}
		else
		{
			if (!CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				disallowedReason = "YOU MUST BE INSIDE THE VIRTUAL STUMP TO " + (load ? "" : "UN") + "LOAD A MAP.";
				return false;
			}
			return true;
		}
	}

	// Token: 0x06002DBD RID: 11709 RVA: 0x000E35C8 File Offset: 0x000E17C8
	private void LoadMap()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.OptionButtonTooltipText.gameObject.SetActive(false);
		this.selectButtonTooltipText.gameObject.SetActive(false);
		this.modStatusText.gameObject.SetActive(false);
		this.modSubscriptionStatusText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
		{
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
		this.networkObject.LoadModSynced(this.currentModProfile.id);
	}

	// Token: 0x06002DBE RID: 11710 RVA: 0x000E368A File Offset: 0x000E188A
	private void UnloadMod()
	{
		this.networkObject.UnloadModSynced();
	}

	// Token: 0x06002DBF RID: 11711 RVA: 0x000E3697 File Offset: 0x000E1897
	public void OnMapLoadComplete(bool success)
	{
		if (success)
		{
			this.OnMapLoadComplete_UIUpdate();
			return;
		}
		this.mapLoadError = true;
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x000E36AC File Offset: 0x000E18AC
	private void OnMapLoadComplete_UIUpdate()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(true);
		this.unloadPromptText.gameObject.SetActive(true);
	}

	// Token: 0x06002DC1 RID: 11713 RVA: 0x000E3730 File Offset: 0x000E1930
	private void OnMapUnloaded()
	{
		this.mapLoadError = false;
		this.UpdateModDetails(true);
	}

	// Token: 0x06002DC2 RID: 11714 RVA: 0x000E3740 File Offset: 0x000E1940
	private void OnRoomMapChanged(ModId roomMapID)
	{
		if (roomMapID == ModId.Null)
		{
			this.UpdateModDetails(true);
			return;
		}
		if (this.currentModProfile.id != roomMapID)
		{
			this.RetrieveProfileFromModIO(roomMapID.id, new Action<ModIORequestResultAnd<ModProfile>>(this.OnRoomMapProfileReceived));
			return;
		}
		this.ShowLoadRoomMapPrompt();
	}

	// Token: 0x06002DC3 RID: 11715 RVA: 0x000E3794 File Offset: 0x000E1994
	private void OnRoomMapProfileReceived(ModIORequestResultAnd<ModProfile> result)
	{
		this.OnProfileReceived(result);
		if (result.result.success)
		{
			this.ShowLoadRoomMapPrompt();
		}
	}

	// Token: 0x06002DC4 RID: 11716 RVA: 0x000E37B0 File Offset: 0x000E19B0
	private void ShowLoadRoomMapPrompt()
	{
		if (CustomMapManager.IsUnloading() || CustomMapManager.IsLoading(0L) || CustomMapLoader.IsModLoaded(this.currentModProfile.id))
		{
			return;
		}
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(false);
		this.unloadPromptText.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(true);
	}

	// Token: 0x06002DC5 RID: 11717 RVA: 0x000E384C File Offset: 0x000E1A4C
	public void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
	{
		if (loadStatus != MapLoadStatus.None)
		{
			this.mapLoadError = false;
			this.loadRoomMapPromptText.gameObject.SetActive(false);
			this.modDescriptionText.gameObject.SetActive(false);
		}
		switch (loadStatus)
		{
		case MapLoadStatus.Downloading:
			this.loadingMapLabelText.text = this.mapAutoDownloadingString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		case MapLoadStatus.Loading:
			this.loadingMapLabelText.text = this.mapLoadingString + " " + progress.ToString() + "%";
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.text = message;
			this.loadingMapMessageText.gameObject.SetActive(true);
			return;
		case MapLoadStatus.Unloading:
			this.mapReadyText.gameObject.SetActive(false);
			this.unloadPromptText.gameObject.SetActive(false);
			this.loadingMapLabelText.text = this.mapUnloadingString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		case MapLoadStatus.Error:
			this.loadingMapLabelText.text = this.mapLoadingErrorString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			if (CustomMapsTerminal.IsDriver)
			{
				this.loadingMapMessageText.text = message + "\n" + this.mapLoadingErrorDriverString;
			}
			else
			{
				this.loadingMapMessageText.text = message + "\n" + this.mapLoadingErrorNonDriverString;
			}
			this.loadingMapMessageText.gameObject.SetActive(true);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002DC6 RID: 11718 RVA: 0x000E3A12 File Offset: 0x000E1C12
	public long GetModId()
	{
		return this.currentModProfile.id.id;
	}

	// Token: 0x040033F3 RID: 13299
	[SerializeField]
	private SpriteRenderer mapScreenshotImage;

	// Token: 0x040033F4 RID: 13300
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x040033F5 RID: 13301
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x040033F6 RID: 13302
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x040033F7 RID: 13303
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x040033F8 RID: 13304
	[SerializeField]
	private TMP_Text modDescriptionText;

	// Token: 0x040033F9 RID: 13305
	[SerializeField]
	private TMP_Text selectButtonTooltipText;

	// Token: 0x040033FA RID: 13306
	[SerializeField]
	private TMP_Text OptionButtonTooltipText;

	// Token: 0x040033FB RID: 13307
	[SerializeField]
	private TMP_Text modStatusText;

	// Token: 0x040033FC RID: 13308
	[SerializeField]
	private TMP_Text modSubscriptionStatusText;

	// Token: 0x040033FD RID: 13309
	[SerializeField]
	private TMP_Text loadingMapLabelText;

	// Token: 0x040033FE RID: 13310
	[SerializeField]
	private TMP_Text loadingMapMessageText;

	// Token: 0x040033FF RID: 13311
	[SerializeField]
	private TMP_Text loadRoomMapPromptText;

	// Token: 0x04003400 RID: 13312
	[SerializeField]
	private TMP_Text mapReadyText;

	// Token: 0x04003401 RID: 13313
	[SerializeField]
	private TMP_Text unloadPromptText;

	// Token: 0x04003402 RID: 13314
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x04003403 RID: 13315
	[SerializeField]
	private TMP_Text outdatedText;

	// Token: 0x04003404 RID: 13316
	[SerializeField]
	private GameObject[] buttonsToHide;

	// Token: 0x04003405 RID: 13317
	[SerializeField]
	private GameObject[] buttonsToShow;

	// Token: 0x04003406 RID: 13318
	[SerializeField]
	private CustomMapsTerminalToggleButton subscriptionToggleButton;

	// Token: 0x04003407 RID: 13319
	[SerializeField]
	private string modAvailableString = "AVAILABLE";

	// Token: 0x04003408 RID: 13320
	[SerializeField]
	private string mapAutoDownloadingString = "DOWNLOADING...";

	// Token: 0x04003409 RID: 13321
	[SerializeField]
	private string mapDownloadQueuedString = "DOWNLOAD QUEUED";

	// Token: 0x0400340A RID: 13322
	[SerializeField]
	private string mapLoadingString = "LOADING:";

	// Token: 0x0400340B RID: 13323
	[SerializeField]
	private string mapUnloadingString = "UNLOADING...";

	// Token: 0x0400340C RID: 13324
	[SerializeField]
	private string mapLoadingErrorString = "ERROR:";

	// Token: 0x0400340D RID: 13325
	[SerializeField]
	private string mapLoadingErrorDriverString = "PRESS THE 'BACK' BUTTON TO TRY AGAIN";

	// Token: 0x0400340E RID: 13326
	[SerializeField]
	private string mapLoadingErrorNonDriverString = "LEAVE AND REJOIN THE VIRTUAL STUMP TO TRY AGAIN";

	// Token: 0x0400340F RID: 13327
	[SerializeField]
	private string mapLoadingErrorInvalidModFile = "INSTALL FAILED DUE TO INVALID MAP FILE";

	// Token: 0x04003410 RID: 13328
	[SerializeField]
	private VirtualStumpSerializer networkObject;

	// Token: 0x04003411 RID: 13329
	private Dictionary<SubscribedModStatus, string> modStatusStrings = new Dictionary<SubscribedModStatus, string>
	{
		{
			SubscribedModStatus.Installed,
			"READY"
		},
		{
			SubscribedModStatus.WaitingToDownload,
			"NOT DOWNLOADED"
		},
		{
			SubscribedModStatus.WaitingToInstall,
			"INSTALL QUEUED"
		},
		{
			SubscribedModStatus.WaitingToUpdate,
			"NEEDS UPDATE"
		},
		{
			SubscribedModStatus.WaitingToUninstall,
			"UNINSTALL QUEUED"
		},
		{
			SubscribedModStatus.Downloading,
			"DOWNLOADING"
		},
		{
			SubscribedModStatus.Installing,
			"INSTALLING"
		},
		{
			SubscribedModStatus.Uninstalling,
			"UNINSTALLING"
		},
		{
			SubscribedModStatus.Updating,
			"UPDATING"
		},
		{
			SubscribedModStatus.ProblemOccurred,
			"ERROR"
		},
		{
			SubscribedModStatus.None,
			""
		}
	};

	// Token: 0x04003412 RID: 13330
	private Dictionary<ModManagementOperationType, string> modOperationTypeStrings = new Dictionary<ModManagementOperationType, string>
	{
		{
			ModManagementOperationType.Install,
			"INSTALLING"
		},
		{
			ModManagementOperationType.Download,
			"DOWNLOADING"
		},
		{
			ModManagementOperationType.Update,
			"UPDATING"
		},
		{
			ModManagementOperationType.Uninstall,
			"UNINSTALLING"
		},
		{
			ModManagementOperationType.None_ErrorOcurred,
			"ERROR"
		},
		{
			ModManagementOperationType.None_AlreadyInstalled,
			"READY"
		}
	};

	// Token: 0x04003413 RID: 13331
	[FormerlySerializedAs("unsubscribedTooltipString")]
	[SerializeField]
	private string subscribeTooltipString = "MAP: SUBSCRIBE TO MAP";

	// Token: 0x04003414 RID: 13332
	[FormerlySerializedAs("subscribedTooltipString")]
	[SerializeField]
	private string unsubscribeTooltipString = "MAP: UNSUBSCRIBE FROM MAP";

	// Token: 0x04003415 RID: 13333
	[SerializeField]
	private string subscribedStatusString = "SUBSCRIBED";

	// Token: 0x04003416 RID: 13334
	[SerializeField]
	private string unsubscribedStatusString = "NOT SUBSCRIBED";

	// Token: 0x04003417 RID: 13335
	[SerializeField]
	private string loadMapTooltipString = "SELECT: LOAD MAP";

	// Token: 0x04003418 RID: 13336
	[SerializeField]
	private string downloadMapTooltipString = "SELECT: DOWNLOAD MAP";

	// Token: 0x04003419 RID: 13337
	[SerializeField]
	private string updateMapTooltipString = "SELECT: UPDATE MAP";

	// Token: 0x0400341A RID: 13338
	public long pendingModId;

	// Token: 0x0400341C RID: 13340
	private bool hasModProfile;

	// Token: 0x0400341D RID: 13341
	private bool mapLoadError;

	// Token: 0x0400341E RID: 13342
	private const uint IO_ModFileInvalid_ErrorCode = 20460U;

	// Token: 0x0400341F RID: 13343
	private ProgressHandle currentProgressHandle;

	// Token: 0x04003420 RID: 13344
	private ModManagementOperationType lastCompletedOperation = ModManagementOperationType.None_ErrorOcurred;
}
