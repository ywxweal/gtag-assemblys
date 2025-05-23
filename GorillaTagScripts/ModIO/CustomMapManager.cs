using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using GorillaTag.Rendering;
using GorillaTagScripts.CustomMapSupport;
using GorillaTagScripts.UI.ModIO;
using GT_CustomMapSupportRuntime;
using ModIO;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.ModIO
{
	// Token: 0x02000B3C RID: 2876
	public class CustomMapManager : MonoBehaviour, IBuildValidation
	{
		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x060046C2 RID: 18114 RVA: 0x0015085B File Offset: 0x0014EA5B
		public static bool WaitingForRoomJoin
		{
			get
			{
				return CustomMapManager.waitingForRoomJoin;
			}
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x060046C3 RID: 18115 RVA: 0x00150862 File Offset: 0x0014EA62
		public static bool WaitingForDisconnect
		{
			get
			{
				return CustomMapManager.waitingForDisconnect;
			}
		}

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x060046C4 RID: 18116 RVA: 0x00150869 File Offset: 0x0014EA69
		public static long LoadingMapId
		{
			get
			{
				return CustomMapManager.loadingMapId;
			}
		}

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x060046C5 RID: 18117 RVA: 0x00150875 File Offset: 0x0014EA75
		public static long UnloadingMapId
		{
			get
			{
				return CustomMapManager.unloadingMapId;
			}
		}

		// Token: 0x060046C6 RID: 18118 RVA: 0x00150884 File Offset: 0x0014EA84
		public bool BuildValidationCheck()
		{
			for (int i = 0; i < this.virtualStumpEjectLocations.Length; i++)
			{
				if (this.virtualStumpEjectLocations[i].ejectLocations.IsNullOrEmpty<GameObject>())
				{
					Debug.LogError("List of VirtualStumpEjectLocations is empty for zone " + this.virtualStumpEjectLocations[i].ejectZone.ToString(), base.gameObject);
					return false;
				}
			}
			return true;
		}

		// Token: 0x060046C7 RID: 18119 RVA: 0x001508E8 File Offset: 0x0014EAE8
		private void Awake()
		{
			if (CustomMapManager.instance == null)
			{
				CustomMapManager.instance = this;
				CustomMapManager.hasInstance = true;
				return;
			}
			if (CustomMapManager.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060046C8 RID: 18120 RVA: 0x00150924 File Offset: 0x0014EB24
		public void OnEnable()
		{
			CustomMapManager.gamemodeButtonLayout = Object.FindObjectOfType<GameModeSelectorButtonLayout>();
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			CMSSerializer.OnTriggerHistoryProcessedForScene.AddListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			GameEvents.ModIOModManagementEvent.RemoveListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
			GameEvents.ModIOModManagementEvent.AddListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
			GameEvents.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
			GameEvents.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
			GameEvents.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
			GameEvents.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
			RoomSystem.JoinedRoomEvent = (Action)Delegate.Remove(RoomSystem.JoinedRoomEvent, new Action(this.OnJoinedRoom));
			RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.OnJoinedRoom));
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
		}

		// Token: 0x060046C9 RID: 18121 RVA: 0x00150A9C File Offset: 0x0014EC9C
		public void OnDisable()
		{
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			GameEvents.ModIOModManagementEvent.RemoveListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
			GameEvents.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
			GameEvents.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
		}

		// Token: 0x060046CA RID: 18122 RVA: 0x000023F4 File Offset: 0x000005F4
		private void OnUGCEnabled()
		{
		}

		// Token: 0x060046CB RID: 18123 RVA: 0x000023F4 File Offset: 0x000005F4
		private void OnUGCDisabled()
		{
		}

		// Token: 0x060046CC RID: 18124 RVA: 0x00150B50 File Offset: 0x0014ED50
		private void Start()
		{
			for (int i = this.virtualStumpTeleportLocations.Count - 1; i >= 0; i--)
			{
				if (this.virtualStumpTeleportLocations[i] == null)
				{
					this.virtualStumpTeleportLocations.RemoveAt(i);
				}
			}
			for (int j = 0; j < this.virtualStumpEjectLocations.Length; j++)
			{
				if (this.virtualStumpEjectLocations[j].ejectLocations.IsNullOrEmpty<GameObject>())
				{
					GTDev.LogError<string>("List of VirtualStumpEjectLocations is empty for zone " + this.virtualStumpEjectLocations[j].ejectZone.ToString() + "!", null);
				}
				List<GameObject> list = new List<GameObject>(this.virtualStumpEjectLocations[j].ejectLocations);
				for (int k = list.Count - 1; k >= 0; k--)
				{
					if (list[k].IsNull())
					{
						list.RemoveAt(k);
					}
				}
				this.virtualStumpEjectLocations[j].ejectLocations = list.ToArray();
			}
			this.virtualStumpToggleableRoot.SetActive(false);
			base.gameObject.SetActive(false);
		}

		// Token: 0x060046CD RID: 18125 RVA: 0x00150C58 File Offset: 0x0014EE58
		private void OnDestroy()
		{
			if (CustomMapManager.instance == this)
			{
				CustomMapManager.instance = null;
				CustomMapManager.hasInstance = false;
			}
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			GameEvents.ModIOModManagementEvent.RemoveListener(new UnityAction<ModManagementEventType, ModId, Result>(this.HandleModManagementEvent));
			GameEvents.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
			GameEvents.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
		}

		// Token: 0x060046CE RID: 18126 RVA: 0x00150D28 File Offset: 0x0014EF28
		private void HandleModManagementEvent(ModManagementEventType eventType, ModId modId, Result result)
		{
			if (CustomMapManager.waitingForModInstall && CustomMapManager.waitingForModInstallId == modId)
			{
				if (CustomMapManager.abortModLoadIds.Contains(modId))
				{
					CustomMapManager.abortModLoadIds.Remove(modId);
					if (CustomMapManager.waitingForModInstallId.Equals(modId))
					{
						CustomMapManager.waitingForModInstall = false;
						CustomMapManager.waitingForModDownload = false;
						CustomMapManager.waitingForModInstallId = ModId.Null;
					}
					return;
				}
				switch (eventType)
				{
				case ModManagementEventType.Installed:
				case ModManagementEventType.Updated:
					CustomMapManager.waitingForModDownload = false;
					this.LoadInstalledMod(modId);
					break;
				case ModManagementEventType.InstallFailed:
					CustomMapManager.instance.HandleMapLoadFailed("FAILED TO INSTALL MAP: " + result.message);
					return;
				case ModManagementEventType.DownloadStarted:
				case ModManagementEventType.UpdateStarted:
					CustomMapManager.waitingForModDownload = true;
					return;
				case ModManagementEventType.Downloaded:
					CustomMapManager.waitingForModDownload = false;
					return;
				case ModManagementEventType.DownloadFailed:
					CustomMapManager.waitingForModDownload = false;
					return;
				case ModManagementEventType.UninstallStarted:
				case ModManagementEventType.Uninstalled:
				case ModManagementEventType.UninstallFailed:
					break;
				case ModManagementEventType.UpdateFailed:
					CustomMapManager.instance.HandleMapLoadFailed("FAILED TO DOWNLOAD MAP: " + result.message);
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x060046CF RID: 18127 RVA: 0x00150E20 File Offset: 0x0014F020
		private void OnModIOLoggedOut()
		{
			ModId @null = ModId.Null;
			if (CustomMapLoader.IsModLoaded(0L) && NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
			{
				@null = new ModId(CustomMapLoader.LoadedMapModId);
			}
			CustomMapManager.UnloadMod(true);
			CustomMapManager.mapIdToLoadOnLogin = @null;
		}

		// Token: 0x060046D0 RID: 18128 RVA: 0x00150E6D File Offset: 0x0014F06D
		private void OnModIOLoggedIn()
		{
			if (CustomMapManager.mapIdToLoadOnLogin != ModId.Null && CustomMapManager.mapIdToLoadOnLogin.Equals(CustomMapManager.currentRoomMapModId) && CustomMapManager.currentRoomMapApproved)
			{
				CustomMapManager.LoadMod(CustomMapManager.mapIdToLoadOnLogin);
			}
		}

		// Token: 0x060046D1 RID: 18129 RVA: 0x00150EA2 File Offset: 0x0014F0A2
		internal static IEnumerator TeleportToVirtualStump(short teleporterIdx, Action<bool> callback, GTZone playerEntranceZone, VirtualStumpTeleporterSerializer teleporterSerializer)
		{
			if (UGCPermissionManager.IsUGCDisabled)
			{
				yield break;
			}
			if (!CustomMapManager.hasInstance)
			{
				if (callback != null)
				{
					callback(false);
				}
				yield break;
			}
			CustomMapManager.instance.gameObject.SetActive(true);
			CustomMapManager.entranceZone = playerEntranceZone;
			CustomMapManager.teleporterNetworkObj = teleporterSerializer;
			PrivateUIRoom.ForceStartOverlay();
			GorillaTagger.Instance.overrideNotInFocus = true;
			GreyZoneManager greyZoneManager = GreyZoneManager.Instance;
			if (greyZoneManager != null)
			{
				greyZoneManager.ForceStopGreyZone();
			}
			if (CustomMapManager.instance.virtualStumpTeleportLocations.Count > 0)
			{
				int num = Random.Range(0, CustomMapManager.instance.virtualStumpTeleportLocations.Count);
				Transform randTeleportTarget = CustomMapManager.instance.virtualStumpTeleportLocations[num];
				CustomMapManager.instance.EnableTeleportHUD(true);
				if (CustomMapManager.teleporterNetworkObj != null)
				{
					CustomMapManager.teleporterNetworkObj.NotifyPlayerTeleporting(teleporterIdx, CustomMapManager.instance.localTeleportSFXSource);
				}
				yield return new WaitForSeconds(0.75f);
				CustomMapManager.instance.virtualStumpToggleableRoot.SetActive(true);
				GTPlayer.Instance.TeleportTo(randTeleportTarget, true, false);
				GorillaComputer.instance.SetInVirtualStump(true);
				yield return null;
				if (VRRig.LocalRig.IsNotNull() && VRRig.LocalRig.zoneEntity.IsNotNull())
				{
					VRRig.LocalRig.zoneEntity.DisableZoneChanges();
				}
				ZoneManagement.SetActiveZone(GTZone.customMaps);
				foreach (GameObject gameObject in CustomMapManager.instance.rootObjectsToDeactivateAfterTeleport)
				{
					if (gameObject != null)
					{
						gameObject.gameObject.SetActive(false);
					}
				}
				if (CustomMapManager.hasInstance && CustomMapManager.instance.virtualStumpZoneShaderSettings.IsNotNull())
				{
					CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(false);
				}
				else
				{
					ZoneShaderSettings.ActivateDefaultSettings();
				}
				CustomMapManager.currentTeleportCallback = callback;
				CustomMapManager.pendingNewPrivateRoomName = "";
				CustomMapManager.preTeleportInPrivateRoom = false;
				if (NetworkSystem.Instance.InRoom)
				{
					if (NetworkSystem.Instance.SessionIsPrivate)
					{
						CustomMapManager.preTeleportInPrivateRoom = true;
						CustomMapManager.waitingForRoomJoin = true;
						CustomMapManager.pendingNewPrivateRoomName = GorillaComputer.instance.VStumpRoomPrepend + NetworkSystem.Instance.RoomName;
					}
					CustomMapManager.waitingForLoginDisconnect = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				else
				{
					CustomMapManager.RequestPlatformLogin();
				}
				randTeleportTarget = null;
			}
			yield break;
		}

		// Token: 0x060046D2 RID: 18130 RVA: 0x00150EC8 File Offset: 0x0014F0C8
		private static void OnPlatformLoginComplete(ModIORequestResult result)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.preTeleportInPrivateRoom)
			{
				CustomMapManager.delayedEndTeleportCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedEndTeleport());
				if (NetworkSystem.Instance.netState != NetSystemState.Idle)
				{
					CustomMapManager.delayedJoinCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedJoinVStumpPrivateRoom());
				}
				else
				{
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				}
			}
			if (!CustomMapManager.preTeleportInPrivateRoom && !CustomMapManager.waitingForDisconnect)
			{
				CustomMapManager.EndTeleport(true);
			}
			CustomMapManager.preTeleportInPrivateRoom = false;
		}

		// Token: 0x060046D3 RID: 18131 RVA: 0x00150F57 File Offset: 0x0014F157
		private static IEnumerator DelayedJoinVStumpPrivateRoom()
		{
			while (NetworkSystem.Instance.netState != NetSystemState.Idle)
			{
				yield return null;
			}
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
			yield break;
		}

		// Token: 0x060046D4 RID: 18132 RVA: 0x00150F60 File Offset: 0x0014F160
		public static void ExitVirtualStump(Action<bool> callback)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			for (int i = 0; i < CustomMapManager.instance.virtualStumpEjectLocations.Length; i++)
			{
				if (CustomMapManager.instance.virtualStumpEjectLocations[i].ejectZone == CustomMapManager.entranceZone && CustomMapManager.instance.virtualStumpEjectLocations[i].ejectLocations.IsNullOrEmpty<GameObject>())
				{
					if (callback != null)
					{
						callback(false);
					}
					return;
				}
			}
			CustomMapManager.instance.dayNightManager.RequestRepopulateLightmaps();
			PrivateUIRoom.ForceStartOverlay();
			GorillaTagger.Instance.overrideNotInFocus = true;
			CustomMapManager.instance.EnableTeleportHUD(false);
			CustomMapManager.currentTeleportCallback = callback;
			CustomMapManager.exitVirtualStumpPending = true;
			if (!CustomMapManager.UnloadMod(false))
			{
				CustomMapManager.FinalizeExitVirtualStump();
			}
		}

		// Token: 0x060046D5 RID: 18133 RVA: 0x00151014 File Offset: 0x0014F214
		private static void FinalizeExitVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			GTPlayer.Instance.SetHoverActive(false);
			VRRig.LocalRig.hoverboardVisual.SetNotHeld();
			foreach (GameObject gameObject in CustomMapManager.instance.rootObjectsToDeactivateAfterTeleport)
			{
				if (gameObject != null)
				{
					gameObject.gameObject.SetActive(true);
				}
			}
			GTZone gtzone = CustomMapManager.entranceZone;
			if (gtzone != GTZone.forest)
			{
				if (gtzone == GTZone.arcade)
				{
					ZoneManagement.SetActiveZone(GTZone.arcade);
				}
			}
			else
			{
				ZoneManagement.SetActiveZone(GTZone.forest);
			}
			Transform transform = null;
			for (int j = 0; j < CustomMapManager.instance.virtualStumpEjectLocations.Length; j++)
			{
				if (CustomMapManager.instance.virtualStumpEjectLocations[j].ejectZone == CustomMapManager.entranceZone)
				{
					CustomMapManager.pendingTeleportVFXIdx = (short)Random.Range(0, CustomMapManager.instance.virtualStumpEjectLocations[j].ejectLocations.Length);
					transform = CustomMapManager.instance.virtualStumpEjectLocations[j].ejectLocations[(int)CustomMapManager.pendingTeleportVFXIdx].transform;
					break;
				}
			}
			if (VRRig.LocalRig.IsNotNull() && VRRig.LocalRig.zoneEntity.IsNotNull())
			{
				VRRig.LocalRig.zoneEntity.EnableZoneChanges();
			}
			GorillaComputer.instance.SetInVirtualStump(false);
			GTPlayer.Instance.TeleportTo(transform, true, false);
			CustomMapManager.instance.virtualStumpToggleableRoot.SetActive(false);
			ZoneShaderSettings.ActivateDefaultSettings();
			VRRig.LocalRig.EnableVStumpReturnWatch(false);
			GTPlayer.Instance.SetHoverAllowed(false, true);
			CustomMapManager.exitVirtualStumpPending = false;
			if (CustomMapManager.delayedEndTeleportCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
			}
			CustomMapManager.delayedEndTeleportCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedEndTeleport());
			if (CustomMapManager.preTeleportInPrivateRoom)
			{
				CustomMapManager.waitingForRoomJoin = true;
				CustomMapManager.pendingNewPrivateRoomName = CustomMapManager.pendingNewPrivateRoomName.RemoveAll(GorillaComputer.instance.VStumpRoomPrepend, StringComparison.OrdinalIgnoreCase);
				PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				GorillaComputer.instance.allowedMapsToJoin = CustomMapManager.instance.exitVirtualStumpJoinTrigger.myCollider.myAllowedMapsToJoin;
				CustomMapManager.waitingForRoomJoin = true;
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(CustomMapManager.instance.exitVirtualStumpJoinTrigger, JoinType.Solo);
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				CustomMapManager.waitingForRoomJoin = true;
				CustomMapManager.pendingNewPrivateRoomName = NetworkSystem.Instance.RoomName.RemoveAll(GorillaComputer.instance.VStumpRoomPrepend, StringComparison.OrdinalIgnoreCase);
				PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				return;
			}
			GorillaComputer.instance.allowedMapsToJoin = CustomMapManager.instance.exitVirtualStumpJoinTrigger.myCollider.myAllowedMapsToJoin;
			CustomMapManager.waitingForRoomJoin = true;
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(CustomMapManager.instance.exitVirtualStumpJoinTrigger, JoinType.Solo);
		}

		// Token: 0x060046D6 RID: 18134 RVA: 0x001512E9 File Offset: 0x0014F4E9
		private static void OnJoinSpecificRoomResult(NetJoinResult result)
		{
			switch (result)
			{
			case NetJoinResult.Failed_Full:
				CustomMapManager.instance.OnJoinRoomFailed();
				return;
			case NetJoinResult.AlreadyInRoom:
				CustomMapManager.instance.OnJoinedRoom();
				return;
			case NetJoinResult.Failed_Other:
				CustomMapManager.waitingForDisconnect = true;
				CustomMapManager.shouldRetryJoin = true;
				return;
			default:
				return;
			}
		}

		// Token: 0x060046D7 RID: 18135 RVA: 0x00151326 File Offset: 0x0014F526
		private static void OnJoinSpecificRoomResultFailureAllowed(NetJoinResult result)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			switch (result)
			{
			case NetJoinResult.Success:
			case NetJoinResult.FallbackCreated:
				return;
			case NetJoinResult.Failed_Full:
			case NetJoinResult.Failed_Other:
				CustomMapManager.instance.OnJoinRoomFailed();
				return;
			case NetJoinResult.AlreadyInRoom:
				CustomMapManager.instance.OnJoinedRoom();
				return;
			default:
				return;
			}
		}

		// Token: 0x060046D8 RID: 18136 RVA: 0x00151368 File Offset: 0x0014F568
		public static bool AreAllPlayersInVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return false;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(vrrig.creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060046D9 RID: 18137 RVA: 0x001513F0 File Offset: 0x0014F5F0
		public static bool IsRemotePlayerInVirtualStump(string playerID)
		{
			return CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(playerID);
		}

		// Token: 0x060046DA RID: 18138 RVA: 0x00151409 File Offset: 0x0014F609
		public static bool IsLocalPlayerInVirtualStump()
		{
			return CustomMapManager.hasInstance && CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(VRRig.LocalRig.creator.UserId);
		}

		// Token: 0x060046DB RID: 18139 RVA: 0x00151440 File Offset: 0x0014F640
		private void OnDisconnected()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			CustomMapManager.ClearRoomMap();
			if (CustomMapManager.waitingForLoginDisconnect)
			{
				CustomMapManager.waitingForLoginDisconnect = false;
				CustomMapManager.RequestPlatformLogin();
				return;
			}
			if (CustomMapManager.waitingForDisconnect)
			{
				CustomMapManager.waitingForDisconnect = false;
				if (CustomMapManager.shouldRetryJoin)
				{
					CustomMapManager.shouldRetryJoin = false;
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResultFailureAllowed));
					return;
				}
				CustomMapManager.EndTeleport(true);
			}
		}

		// Token: 0x060046DC RID: 18140 RVA: 0x001514AC File Offset: 0x0014F6AC
		private static void RequestPlatformLogin()
		{
			ModIOManager.Initialize(delegate(ModIORequestResult result)
			{
				if (result.success)
				{
					ModIOManager.RequestPlatformLogin(new Action<ModIORequestResult>(CustomMapManager.OnPlatformLoginComplete));
					return;
				}
				CustomMapManager.OnPlatformLoginComplete(result);
			});
		}

		// Token: 0x060046DD RID: 18141 RVA: 0x001514D2 File Offset: 0x0014F6D2
		private void OnJoinRoomFailed()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.waitingForRoomJoin)
			{
				CustomMapManager.waitingForRoomJoin = false;
				CustomMapManager.EndTeleport(false);
			}
		}

		// Token: 0x060046DE RID: 18142 RVA: 0x001514F0 File Offset: 0x0014F6F0
		private static void EndTeleport(bool teleportSuccessful)
		{
			if (CustomMapManager.hasInstance)
			{
				if (CustomMapManager.delayedEndTeleportCoroutine != null)
				{
					CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
					CustomMapManager.delayedEndTeleportCoroutine = null;
				}
				if (CustomMapManager.delayedJoinCoroutine != null)
				{
					CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedJoinCoroutine);
					CustomMapManager.delayedJoinCoroutine = null;
				}
			}
			CustomMapManager.DisableTeleportHUD();
			GorillaTagger.Instance.overrideNotInFocus = false;
			PrivateUIRoom.StopForcedOverlay();
			Action<bool> action = CustomMapManager.currentTeleportCallback;
			if (action != null)
			{
				action(teleportSuccessful);
			}
			CustomMapManager.currentTeleportCallback = null;
			if (CustomMapManager.hasInstance && !GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				CustomMapManager.instance.gameObject.SetActive(false);
			}
		}

		// Token: 0x060046DF RID: 18143 RVA: 0x00151593 File Offset: 0x0014F793
		private static IEnumerator DelayedEndTeleport()
		{
			yield return new WaitForSecondsRealtime(CustomMapManager.instance.maxPostTeleportRoomProcessingTime);
			CustomMapManager.EndTeleport(false);
			yield break;
		}

		// Token: 0x060046E0 RID: 18144 RVA: 0x0015159C File Offset: 0x0014F79C
		private void OnJoinedRoom()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.waitingForRoomJoin)
			{
				CustomMapManager.waitingForRoomJoin = false;
				CustomMapManager.EndTeleport(true);
				if (CustomMapManager.pendingTeleportVFXIdx > -1 && CustomMapManager.teleporterNetworkObj != null)
				{
					CustomMapManager.teleporterNetworkObj.NotifyPlayerReturning(CustomMapManager.pendingTeleportVFXIdx);
					CustomMapManager.pendingTeleportVFXIdx = -1;
				}
			}
		}

		// Token: 0x060046E1 RID: 18145 RVA: 0x001515F0 File Offset: 0x0014F7F0
		public static bool UnloadMod(bool returnToSinglePlayerIfInPublic = true)
		{
			if (CustomMapManager.unloadInProgress)
			{
				return false;
			}
			if (!CustomMapLoader.IsModLoaded(0L) && !CustomMapLoader.IsLoading)
			{
				if (CustomMapManager.loadInProgress)
				{
					CustomMapManager.abortModLoadIds.AddIfNew(CustomMapManager.loadingMapId);
					if (CustomMapManager.waitingForModDownload)
					{
						ModIOManager.AbortModDownload(CustomMapManager.loadingMapId);
					}
					CustomMapManager.loadInProgress = false;
					CustomMapManager.loadingMapId = ModId.Null;
					CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
					CustomMapManager.waitingForModDownload = false;
					CustomMapManager.waitingForModInstall = false;
					CustomMapManager.waitingForModInstallId = ModId.Null;
					CustomMapManager.ClearRoomMap();
				}
				else
				{
					CustomMapManager.ClearRoomMap();
				}
				return false;
			}
			CustomMapManager.unloadInProgress = true;
			CustomMapManager.unloadingMapId = new ModId(CustomMapLoader.IsModLoaded(0L) ? CustomMapLoader.LoadedMapModId : CustomMapLoader.LoadingMapModId);
			CustomMapManager.OnMapLoadStatusChanged.Invoke(MapLoadStatus.Unloading, 0, "");
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.ClearRoomMap();
			CustomGameMode.LuaScript = "";
			if (CustomGameMode.gameScriptRunner != null)
			{
				CustomGameMode.StopScript();
			}
			CustomMapManager.customMapDefaultZoneShaderSettingsInitialized = false;
			CustomMapManager.customMapDefaultZoneShaderProperties = default(CMSZoneShaderSettings.CMSZoneShaderProperties);
			CustomMapManager.loadedCustomMapDefaultZoneShaderSettings = null;
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.CopySettings(CustomMapManager.instance.virtualStumpZoneShaderSettings, false, false);
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(false);
				CustomMapManager.allCustomMapZoneShaderSettings.Clear();
			}
			CustomMapLoader.CloseDoorAndUnloadMod(new Action(CustomMapManager.OnMapUnloadCompleted));
			if (returnToSinglePlayerIfInPublic && NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			return true;
		}

		// Token: 0x060046E2 RID: 18146 RVA: 0x0015178D File Offset: 0x0014F98D
		private static void OnMapUnloadCompleted()
		{
			CustomMapManager.unloadInProgress = false;
			CustomMapManager.OnMapUnloadComplete.Invoke();
			CustomMapManager.currentRoomMapModId = ModId.Null;
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(ModId.Null);
			if (CustomMapManager.exitVirtualStumpPending)
			{
				CustomMapManager.FinalizeExitVirtualStump();
			}
		}

		// Token: 0x060046E3 RID: 18147 RVA: 0x001517CC File Offset: 0x0014F9CC
		public static void LoadMod(ModId modId)
		{
			if (!CustomMapManager.hasInstance || CustomMapManager.loadInProgress)
			{
				return;
			}
			if (CustomMapManager.abortModLoadIds.Contains(modId))
			{
				CustomMapManager.abortModLoadIds.Remove(modId);
			}
			if (!ModIOManager.IsLoggedIn())
			{
				CustomMapManager.SetMapToLoadOnLogin(modId);
				return;
			}
			if (CustomMapLoader.IsModLoaded(modId))
			{
				return;
			}
			CustomMapManager.loadInProgress = true;
			CustomMapManager.loadingMapId = modId;
			CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			SubscribedMod subscribedMod;
			if (!ModIOManager.GetSubscribedModProfile(modId, out subscribedMod))
			{
				ModIOManager.SubscribeToMod(modId, delegate(Result result)
				{
					if (CustomMapManager.abortModLoadIds.Contains(modId))
					{
						CustomMapManager.abortModLoadIds.Remove(modId);
						return;
					}
					if (!result.Succeeded())
					{
						CustomMapManager.instance.HandleMapLoadFailed("FAILED TO SUBSCRIBE TO MAP: " + result.message);
						return;
					}
					if (ModIOManager.GetSubscribedModProfile(modId, out subscribedMod))
					{
						CustomMapManager.OnMapLoadStatusChanged.Invoke(MapLoadStatus.Downloading, 0, "");
						CustomMapManager.instance.LoadSubscribedMod(subscribedMod);
					}
				});
				return;
			}
			CustomMapManager.instance.LoadSubscribedMod(subscribedMod);
		}

		// Token: 0x060046E4 RID: 18148 RVA: 0x001518AC File Offset: 0x0014FAAC
		private void LoadSubscribedMod(SubscribedMod subscribedMod)
		{
			if (subscribedMod.status != SubscribedModStatus.Installed)
			{
				CustomMapManager.waitingForModInstall = true;
				CustomMapManager.waitingForModInstallId = subscribedMod.modProfile.id;
				if (subscribedMod.status == SubscribedModStatus.WaitingToDownload || subscribedMod.status == SubscribedModStatus.WaitingToUpdate)
				{
					ModIOManager.DownloadMod(subscribedMod.modProfile.id, delegate(ModIORequestResult result)
					{
						if (CustomMapManager.abortModLoadIds.Contains(subscribedMod.modProfile.id))
						{
							CustomMapManager.abortModLoadIds.Remove(subscribedMod.modProfile.id);
							return;
						}
						if (!result.success)
						{
							CustomMapManager.instance.HandleMapLoadFailed("FAILED TO START MAP DOWNLOAD: " + result.message);
						}
						GorillaTelemetry.PostCustomMapDownloadEvent(subscribedMod.modProfile.name, subscribedMod.modProfile.id, subscribedMod.modProfile.creator.username);
					});
				}
				return;
			}
			this.LoadInstalledMod(subscribedMod.modProfile.id);
		}

		// Token: 0x060046E5 RID: 18149 RVA: 0x00151944 File Offset: 0x0014FB44
		private void LoadInstalledMod(ModId installedModId)
		{
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			SubscribedMod subscribedMod;
			ModIOManager.GetSubscribedModProfile(installedModId, out subscribedMod);
			if (subscribedMod.status != SubscribedModStatus.Installed)
			{
				this.HandleMapLoadFailed("MAP IS NOT INSTALLED");
				return;
			}
			FileInfo[] files = new DirectoryInfo(subscribedMod.directory).GetFiles("package.json");
			if (files.Length == 0)
			{
				this.HandleMapLoadFailed("COULD NOT FIND PACKAGE.JSON IN MAP FILES");
				return;
			}
			CustomMapLoader.LoadMap(installedModId.id, files[0].FullName, new Action<bool>(this.OnMapLoadFinished), new Action<MapLoadStatus, int, string>(this.OnMapLoadProgress), new Action<string>(CustomMapManager.OnSceneLoaded));
		}

		// Token: 0x060046E6 RID: 18150 RVA: 0x001519DB File Offset: 0x0014FBDB
		private void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
		{
			CustomMapManager.OnMapLoadStatusChanged.Invoke(loadStatus, progress, message);
		}

		// Token: 0x060046E7 RID: 18151 RVA: 0x001519EC File Offset: 0x0014FBEC
		private void OnMapLoadFinished(bool success)
		{
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			if (success)
			{
				CustomMapLoader.OpenDoorToMap();
				if (CustomMapLoader.LoadedMapDescriptor.CustomGamemode != null)
				{
					CustomGameMode.LuaScript = CustomMapLoader.LoadedMapDescriptor.CustomGamemode.text;
					if (CustomGameMode.LuaScript != "" && CustomGameMode.GameModeInitialized && CustomGameMode.gameScriptRunner == null)
					{
						CustomGameMode.LuaStart();
					}
				}
			}
			CustomMapManager.OnMapLoadComplete.Invoke(success);
		}

		// Token: 0x060046E8 RID: 18152 RVA: 0x00151A80 File Offset: 0x0014FC80
		private void HandleMapLoadFailed(string message = null)
		{
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.OnMapLoadStatusChanged.Invoke(MapLoadStatus.Error, 0, message ?? "UNKNOWN ERROR");
			CustomMapManager.OnMapLoadComplete.Invoke(false);
		}

		// Token: 0x060046E9 RID: 18153 RVA: 0x00151ACE File Offset: 0x0014FCCE
		public static bool IsUnloading()
		{
			return CustomMapManager.unloadInProgress;
		}

		// Token: 0x060046EA RID: 18154 RVA: 0x00151AD5 File Offset: 0x0014FCD5
		public static bool IsLoading(long mapId = 0L)
		{
			if (mapId == 0L)
			{
				return CustomMapManager.loadInProgress || CustomMapLoader.IsLoading;
			}
			return CustomMapManager.loadInProgress && CustomMapManager.loadingMapId.id == mapId;
		}

		// Token: 0x060046EB RID: 18155 RVA: 0x00151AFF File Offset: 0x0014FCFF
		public static void SetMapToLoadOnLogin(ModId modId)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			CustomMapManager.mapIdToLoadOnLogin = modId;
		}

		// Token: 0x060046EC RID: 18156 RVA: 0x00151B0F File Offset: 0x0014FD0F
		public static void ClearMapToLoadOnLogin()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
		}

		// Token: 0x060046ED RID: 18157 RVA: 0x00151B24 File Offset: 0x0014FD24
		public static ModId GetRoomMapId()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (CustomMapManager.currentRoomMapModId == ModId.Null && NetworkSystem.Instance.IsMasterClient && CustomMapLoader.IsModLoaded(0L))
				{
					CustomMapManager.currentRoomMapModId = new ModId(CustomMapLoader.LoadedMapModId);
				}
				return CustomMapManager.currentRoomMapModId;
			}
			if (CustomMapManager.IsLoading(0L))
			{
				return CustomMapManager.loadingMapId;
			}
			if (CustomMapLoader.IsModLoaded(0L))
			{
				return new ModId(CustomMapLoader.LoadedMapModId);
			}
			return ModId.Null;
		}

		// Token: 0x060046EE RID: 18158 RVA: 0x00151BA0 File Offset: 0x0014FDA0
		public static void SetRoomMod(long modId)
		{
			if (!CustomMapManager.hasInstance || modId == CustomMapManager.currentRoomMapModId.id)
			{
				return;
			}
			if (CustomMapManager.mapIdToLoadOnLogin.id != modId)
			{
				CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
			}
			CustomMapManager.currentRoomMapModId = new ModId(modId);
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(CustomMapManager.currentRoomMapModId);
		}

		// Token: 0x060046EF RID: 18159 RVA: 0x00151BFC File Offset: 0x0014FDFC
		public static void ClearRoomMap()
		{
			if (!CustomMapManager.hasInstance || CustomMapManager.currentRoomMapModId.Equals(ModId.Null))
			{
				return;
			}
			CustomMapManager.mapIdToLoadOnLogin = ModId.Null;
			CustomMapManager.currentRoomMapModId = ModId.Null;
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(ModId.Null);
		}

		// Token: 0x060046F0 RID: 18160 RVA: 0x00151C4B File Offset: 0x0014FE4B
		public static bool CanLoadRoomMap()
		{
			return CustomMapManager.currentRoomMapModId != ModId.Null;
		}

		// Token: 0x060046F1 RID: 18161 RVA: 0x00151C61 File Offset: 0x0014FE61
		public static void ApproveAndLoadRoomMap()
		{
			CustomMapManager.currentRoomMapApproved = true;
			CMSSerializer.ResetSyncedMapObjects();
			CustomMapManager.LoadMod(CustomMapManager.currentRoomMapModId);
		}

		// Token: 0x060046F2 RID: 18162 RVA: 0x00151C78 File Offset: 0x0014FE78
		public static void RequestEnableTeleportHUD(bool enteringVirtualStump)
		{
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.EnableTeleportHUD(enteringVirtualStump);
			}
		}

		// Token: 0x060046F3 RID: 18163 RVA: 0x00151C90 File Offset: 0x0014FE90
		private void EnableTeleportHUD(bool enteringVirtualStump)
		{
			if (CustomMapManager.teleportingHUD != null)
			{
				CustomMapManager.teleportingHUD.gameObject.SetActive(true);
				CustomMapManager.teleportingHUD.Initialize(enteringVirtualStump);
				return;
			}
			if (this.teleportingHUDPrefab != null)
			{
				Camera main = Camera.main;
				if (main != null)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.teleportingHUDPrefab, main.transform);
					if (gameObject != null)
					{
						CustomMapManager.teleportingHUD = gameObject.GetComponent<VirtualStumpTeleportingHUD>();
						if (CustomMapManager.teleportingHUD != null)
						{
							CustomMapManager.teleportingHUD.Initialize(enteringVirtualStump);
						}
					}
				}
			}
		}

		// Token: 0x060046F4 RID: 18164 RVA: 0x00151D21 File Offset: 0x0014FF21
		public static void DisableTeleportHUD()
		{
			if (CustomMapManager.teleportingHUD != null)
			{
				CustomMapManager.teleportingHUD.gameObject.SetActive(false);
			}
		}

		// Token: 0x060046F5 RID: 18165 RVA: 0x00151D40 File Offset: 0x0014FF40
		public static void LoadZoneTriggered(int[] scenesToLoad, int[] scenesToUnload)
		{
			CustomMapLoader.LoadZoneTriggered(scenesToLoad, scenesToUnload, new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
		}

		// Token: 0x060046F6 RID: 18166 RVA: 0x00151D61 File Offset: 0x0014FF61
		private static void OnSceneLoaded(string sceneName)
		{
			CMSSerializer.ProcessSceneLoad(sceneName);
			CustomMapManager.ProcessZoneShaderSettings(sceneName);
		}

		// Token: 0x060046F7 RID: 18167 RVA: 0x00151D70 File Offset: 0x0014FF70
		private static void OnSceneUnloaded(string sceneName)
		{
			CMSSerializer.UnregisterTriggers(sceneName);
			for (int i = CustomMapManager.allCustomMapZoneShaderSettings.Count - 1; i >= 0; i--)
			{
				if (CustomMapManager.allCustomMapZoneShaderSettings[i].IsNull())
				{
					CustomMapManager.allCustomMapZoneShaderSettings.RemoveAt(i);
				}
			}
		}

		// Token: 0x060046F8 RID: 18168 RVA: 0x00151DB8 File Offset: 0x0014FFB8
		private static void OnSceneTriggerHistoryProcessed(string sceneName)
		{
			CapsuleCollider bodyCollider = GTPlayer.Instance.bodyCollider;
			SphereCollider headCollider = GTPlayer.Instance.headCollider;
			Vector3 vector = bodyCollider.transform.TransformPoint(bodyCollider.center);
			float num = Mathf.Max(bodyCollider.height, bodyCollider.radius) * GTPlayer.Instance.scale;
			Collider[] array = new Collider[100];
			Physics.OverlapSphereNonAlloc(vector, num, array);
			foreach (Collider collider in array)
			{
				if (collider != null && collider.gameObject.scene.name.Equals(sceneName))
				{
					CMSTrigger[] components = collider.gameObject.GetComponents<CMSTrigger>();
					for (int j = 0; j < components.Length; j++)
					{
						if (components[j] != null)
						{
							components[j].OnTriggerEnter(bodyCollider);
							components[j].OnTriggerEnter(headCollider);
						}
					}
					CMSLoadingZone[] components2 = collider.gameObject.GetComponents<CMSLoadingZone>();
					for (int k = 0; k < components2.Length; k++)
					{
						if (components2[k] != null)
						{
							components2[k].OnTriggerEnter(bodyCollider);
						}
					}
					CMSZoneShaderSettingsTrigger[] components3 = collider.gameObject.GetComponents<CMSZoneShaderSettingsTrigger>();
					for (int l = 0; l < components3.Length; l++)
					{
						if (components3[l] != null)
						{
							components3[l].OnTriggerEnter(bodyCollider);
						}
					}
					HoverboardAreaTrigger[] components4 = collider.gameObject.GetComponents<HoverboardAreaTrigger>();
					for (int m = 0; m < components4.Length; m++)
					{
						if (components4[m] != null)
						{
							components4[m].OnTriggerEnter(headCollider);
						}
					}
					WaterVolume[] components5 = collider.gameObject.GetComponents<WaterVolume>();
					for (int n = 0; n < components5.Length; n++)
					{
						if (components5[n] != null)
						{
							components5[n].OnTriggerEnter(bodyCollider);
							components5[n].OnTriggerEnter(headCollider);
						}
					}
				}
			}
		}

		// Token: 0x060046F9 RID: 18169 RVA: 0x00151F97 File Offset: 0x00150197
		public static void SetDefaultZoneShaderSettings(ZoneShaderSettings defaultCustomMapShaderSettings, CMSZoneShaderSettings.CMSZoneShaderProperties defaultZoneShaderProperties)
		{
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.CopySettings(defaultCustomMapShaderSettings, true, false);
				CustomMapManager.loadedCustomMapDefaultZoneShaderSettings = defaultCustomMapShaderSettings;
				CustomMapManager.customMapDefaultZoneShaderProperties = defaultZoneShaderProperties;
				CustomMapManager.customMapDefaultZoneShaderSettingsInitialized = true;
			}
		}

		// Token: 0x060046FA RID: 18170 RVA: 0x00151FC8 File Offset: 0x001501C8
		private static void ProcessZoneShaderSettings(string loadedSceneName)
		{
			if (CustomMapManager.hasInstance && CustomMapManager.customMapDefaultZoneShaderSettingsInitialized && CustomMapManager.customMapDefaultZoneShaderProperties.isInitialized)
			{
				for (int i = 0; i < CustomMapManager.allCustomMapZoneShaderSettings.Count; i++)
				{
					if (CustomMapManager.allCustomMapZoneShaderSettings[i].IsNotNull() && CustomMapManager.allCustomMapZoneShaderSettings[i] != CustomMapManager.loadedCustomMapDefaultZoneShaderSettings && CustomMapManager.allCustomMapZoneShaderSettings[i].gameObject.scene.name.Equals(loadedSceneName))
					{
						CustomMapManager.allCustomMapZoneShaderSettings[i].ReplaceDefaultValues(CustomMapManager.customMapDefaultZoneShaderProperties, true);
					}
				}
				return;
			}
			if (CustomMapManager.hasInstance && CustomMapManager.instance.virtualStumpZoneShaderSettings.IsNotNull())
			{
				for (int j = 0; j < CustomMapManager.allCustomMapZoneShaderSettings.Count; j++)
				{
					if (CustomMapManager.allCustomMapZoneShaderSettings[j].IsNotNull() && CustomMapManager.allCustomMapZoneShaderSettings[j].gameObject.scene.name.Equals(loadedSceneName))
					{
						CustomMapManager.allCustomMapZoneShaderSettings[j].ReplaceDefaultValues(CustomMapManager.instance.virtualStumpZoneShaderSettings, true);
					}
				}
			}
		}

		// Token: 0x060046FB RID: 18171 RVA: 0x001520F2 File Offset: 0x001502F2
		public static void AddZoneShaderSettings(ZoneShaderSettings zoneShaderSettings)
		{
			CustomMapManager.allCustomMapZoneShaderSettings.AddIfNew(zoneShaderSettings);
		}

		// Token: 0x060046FC RID: 18172 RVA: 0x001520FF File Offset: 0x001502FF
		public static void ActivateDefaultZoneShaderSettings()
		{
			if (CustomMapManager.hasInstance && CustomMapManager.customMapDefaultZoneShaderSettingsInitialized)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.BecomeActiveInstance(true);
				return;
			}
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(true);
			}
		}

		// Token: 0x060046FD RID: 18173 RVA: 0x0015213C File Offset: 0x0015033C
		public static void ReturnToVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (!GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				return;
			}
			if (CustomMapManager.instance.returnToVirtualStumpTeleportLocation.IsNotNull())
			{
				GTPlayer gtplayer = GTPlayer.Instance;
				if (gtplayer != null)
				{
					CustomMapLoader.ResetToInitialZone(new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
					gtplayer.TeleportTo(CustomMapManager.instance.returnToVirtualStumpTeleportLocation, true, false);
				}
			}
		}

		// Token: 0x0400494A RID: 18762
		[OnEnterPlay_SetNull]
		private static volatile CustomMapManager instance;

		// Token: 0x0400494B RID: 18763
		[OnEnterPlay_Set(false)]
		private static bool hasInstance = false;

		// Token: 0x0400494C RID: 18764
		[SerializeField]
		private GameObject virtualStumpToggleableRoot;

		// Token: 0x0400494D RID: 18765
		[SerializeField]
		private GorillaNetworkJoinTrigger exitVirtualStumpJoinTrigger;

		// Token: 0x0400494E RID: 18766
		[SerializeField]
		private Transform returnToVirtualStumpTeleportLocation;

		// Token: 0x0400494F RID: 18767
		[SerializeField]
		private List<Transform> virtualStumpTeleportLocations;

		// Token: 0x04004950 RID: 18768
		[SerializeField]
		private ZoneEjectLocations[] virtualStumpEjectLocations;

		// Token: 0x04004951 RID: 18769
		[SerializeField]
		private GameObject[] rootObjectsToDeactivateAfterTeleport;

		// Token: 0x04004952 RID: 18770
		[SerializeField]
		private GorillaFriendCollider virtualStumpPlayerDetector;

		// Token: 0x04004953 RID: 18771
		[SerializeField]
		private ZoneShaderSettings virtualStumpZoneShaderSettings;

		// Token: 0x04004954 RID: 18772
		[SerializeField]
		private BetterDayNightManager dayNightManager;

		// Token: 0x04004955 RID: 18773
		[SerializeField]
		private ZoneShaderSettings customMapDefaultZoneShaderSettings;

		// Token: 0x04004956 RID: 18774
		[SerializeField]
		private GameObject teleportingHUDPrefab;

		// Token: 0x04004957 RID: 18775
		[SerializeField]
		private AudioSource localTeleportSFXSource;

		// Token: 0x04004958 RID: 18776
		private static GTZone entranceZone;

		// Token: 0x04004959 RID: 18777
		private static VirtualStumpTeleporterSerializer teleporterNetworkObj = null;

		// Token: 0x0400495A RID: 18778
		[SerializeField]
		private float maxPostTeleportRoomProcessingTime = 15f;

		// Token: 0x0400495B RID: 18779
		private static bool customMapDefaultZoneShaderSettingsInitialized;

		// Token: 0x0400495C RID: 18780
		private static ZoneShaderSettings loadedCustomMapDefaultZoneShaderSettings;

		// Token: 0x0400495D RID: 18781
		private static CMSZoneShaderSettings.CMSZoneShaderProperties customMapDefaultZoneShaderProperties;

		// Token: 0x0400495E RID: 18782
		private static readonly List<ZoneShaderSettings> allCustomMapZoneShaderSettings = new List<ZoneShaderSettings>();

		// Token: 0x0400495F RID: 18783
		private static GameModeSelectorButtonLayout gamemodeButtonLayout;

		// Token: 0x04004960 RID: 18784
		private static bool loadInProgress = false;

		// Token: 0x04004961 RID: 18785
		private static ModId loadingMapId = ModId.Null;

		// Token: 0x04004962 RID: 18786
		private static bool unloadInProgress = false;

		// Token: 0x04004963 RID: 18787
		private static ModId unloadingMapId = ModId.Null;

		// Token: 0x04004964 RID: 18788
		private static List<ModId> abortModLoadIds = new List<ModId>();

		// Token: 0x04004965 RID: 18789
		private static bool waitingForModDownload = false;

		// Token: 0x04004966 RID: 18790
		private static bool waitingForModInstall = false;

		// Token: 0x04004967 RID: 18791
		private static ModId waitingForModInstallId = ModId.Null;

		// Token: 0x04004968 RID: 18792
		private static bool preTeleportInPrivateRoom = false;

		// Token: 0x04004969 RID: 18793
		private static string pendingNewPrivateRoomName = "";

		// Token: 0x0400496A RID: 18794
		private static ModId mapIdToLoadOnLogin = ModId.Null;

		// Token: 0x0400496B RID: 18795
		private static Action<bool> currentTeleportCallback;

		// Token: 0x0400496C RID: 18796
		private static bool waitingForLoginDisconnect = false;

		// Token: 0x0400496D RID: 18797
		private static bool waitingForDisconnect = false;

		// Token: 0x0400496E RID: 18798
		private static bool waitingForRoomJoin = false;

		// Token: 0x0400496F RID: 18799
		private static bool shouldRetryJoin = false;

		// Token: 0x04004970 RID: 18800
		private static short pendingTeleportVFXIdx = -1;

		// Token: 0x04004971 RID: 18801
		private static bool exitVirtualStumpPending = false;

		// Token: 0x04004972 RID: 18802
		private static ModId currentRoomMapModId = ModId.Null;

		// Token: 0x04004973 RID: 18803
		private static bool currentRoomMapApproved = false;

		// Token: 0x04004974 RID: 18804
		private static VirtualStumpTeleportingHUD teleportingHUD;

		// Token: 0x04004975 RID: 18805
		private static Coroutine delayedEndTeleportCoroutine;

		// Token: 0x04004976 RID: 18806
		private static Coroutine delayedJoinCoroutine;

		// Token: 0x04004977 RID: 18807
		public static UnityEvent<ModId> OnRoomMapChanged = new UnityEvent<ModId>();

		// Token: 0x04004978 RID: 18808
		public static UnityEvent<MapLoadStatus, int, string> OnMapLoadStatusChanged = new UnityEvent<MapLoadStatus, int, string>();

		// Token: 0x04004979 RID: 18809
		public static UnityEvent<bool> OnMapLoadComplete = new UnityEvent<bool>();

		// Token: 0x0400497A RID: 18810
		public static UnityEvent OnMapUnloadComplete = new UnityEvent();
	}
}
