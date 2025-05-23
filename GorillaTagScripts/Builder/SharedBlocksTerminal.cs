using System;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B92 RID: 2962
	public class SharedBlocksTerminal : MonoBehaviour
	{
		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x0600495F RID: 18783 RVA: 0x0015E7A9 File Offset: 0x0015C9A9
		public SharedBlocksManager.SharedBlocksMap SelectedMap
		{
			get
			{
				return this.selectedMap;
			}
		}

		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x06004960 RID: 18784 RVA: 0x0015E7B1 File Offset: 0x0015C9B1
		public bool IsTerminalLocked
		{
			get
			{
				return this.isTerminalLocked;
			}
		}

		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x06004961 RID: 18785 RVA: 0x0015E7B9 File Offset: 0x0015C9B9
		private int playersInLobby
		{
			get
			{
				return this.lobbyTrigger.playerIDsCurrentlyTouching.Count;
			}
		}

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x06004962 RID: 18786 RVA: 0x0015E7CB File Offset: 0x0015C9CB
		public bool IsDriver
		{
			get
			{
				return this.localState.driverID == NetworkSystem.Instance.LocalPlayer.ActorNumber;
			}
		}

		// Token: 0x06004963 RID: 18787 RVA: 0x0015E7E9 File Offset: 0x0015C9E9
		public BuilderTable GetTable()
		{
			return this.linkedTable;
		}

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x06004964 RID: 18788 RVA: 0x0015E7F1 File Offset: 0x0015C9F1
		public int GetDriverID
		{
			get
			{
				return this.localState.driverID;
			}
		}

		// Token: 0x06004965 RID: 18789 RVA: 0x0015E800 File Offset: 0x0015CA00
		public static string MapIDToDisplayedString(string mapID)
		{
			if (mapID.IsNullOrEmpty())
			{
				return "____-____";
			}
			int num = 4;
			SharedBlocksTerminal.sb.Clear();
			if (mapID.Length > num)
			{
				SharedBlocksTerminal.sb.Append(mapID.Substring(0, num));
				SharedBlocksTerminal.sb.Append("-");
				SharedBlocksTerminal.sb.Append(mapID.Substring(num));
				int num2 = 9 - SharedBlocksTerminal.sb.Length;
				SharedBlocksTerminal.sb.Append('_', num2);
			}
			else
			{
				SharedBlocksTerminal.sb.Append(mapID.Substring(0));
				int num3 = num - SharedBlocksTerminal.sb.Length;
				SharedBlocksTerminal.sb.Append('_', num3);
				SharedBlocksTerminal.sb.Append("-____");
			}
			return SharedBlocksTerminal.sb.ToString();
		}

		// Token: 0x06004966 RID: 18790 RVA: 0x0015E8CC File Offset: 0x0015CACC
		public void Init(BuilderTable table)
		{
			if (this.hasInitialized)
			{
				return;
			}
			this.localState = new SharedBlocksTerminal.SharedBlocksTerminalState
			{
				state = SharedBlocksTerminal.TerminalState.NoStatus,
				driverID = -2
			};
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.AddListener(new UnityAction<SharedBlocksKeyboardBindings>(this.PressButton));
			this.terminalControlButton.onPressButton.AddListener(new UnityAction(this.OnTerminalControlPressed));
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			this.RefreshActiveScreen();
			this.linkedTable = table;
			table.linkedTerminal = this;
			this.linkedTable.OnMapLoaded.AddListener(new UnityAction<string>(this.OnSharedBlocksMapLoaded));
			this.linkedTable.OnMapLoadFailed.AddListener(new UnityAction<string>(this.OnSharedBlocksMapLoadFailed));
			this.linkedTable.OnMapCleared.AddListener(new UnityAction(this.OnSharedBlocksMapLoadStart));
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnReturnedToSinglePlayer;
			this.hasInitialized = true;
		}

		// Token: 0x06004967 RID: 18791 RVA: 0x0015E9D0 File Offset: 0x0015CBD0
		private void Start()
		{
			BuilderTable builderTable;
			if (!this.hasInitialized && BuilderTable.TryGetBuilderTableForZone(this.tableZone, out builderTable))
			{
				this.Init(builderTable);
				return;
			}
			Debug.LogWarning("Could not find builder table for zone " + this.tableZone.ToString());
		}

		// Token: 0x06004968 RID: 18792 RVA: 0x0015EA1C File Offset: 0x0015CC1C
		private void LateUpdate()
		{
			if (this.localState.driverID == -2)
			{
				return;
			}
			if (GorillaComputer.instance == null)
			{
				return;
			}
			if (this.useNametags == GorillaComputer.instance.NametagsEnabled)
			{
				return;
			}
			this.useNametags = GorillaComputer.instance.NametagsEnabled;
			this.RefreshDriverNickname();
		}

		// Token: 0x06004969 RID: 18793 RVA: 0x0015EA78 File Offset: 0x0015CC78
		private void OnDestroy()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.RemoveListener(new UnityAction<SharedBlocksKeyboardBindings>(this.PressButton));
			if (this.terminalControlButton != null)
			{
				this.terminalControlButton.onPressButton.RemoveListener(new UnityAction(this.OnTerminalControlPressed));
			}
			if (NetworkSystem.Instance != null)
			{
				NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
				NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnReturnedToSinglePlayer;
			}
			if (this.linkedTable != null)
			{
				this.linkedTable.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnSharedBlocksMapLoaded));
				this.linkedTable.OnMapLoadFailed.RemoveListener(new UnityAction<string>(this.OnSharedBlocksMapLoadFailed));
				this.linkedTable.OnMapCleared.RemoveListener(new UnityAction(this.OnSharedBlocksMapLoadStart));
			}
		}

		// Token: 0x0600496A RID: 18794 RVA: 0x0015EB60 File Offset: 0x0015CD60
		private void RefreshActiveScreen()
		{
			if (this.localState.driverID == -2)
			{
				if (this.currentScreen != this.noDriverScreen)
				{
					if (this.currentScreen != null)
					{
						this.currentScreen.Hide();
					}
					this.currentScreen = this.noDriverScreen;
					this.currentScreen.Show();
				}
				this.statusMessageText.gameObject.SetActive(false);
				return;
			}
			if (this.currentScreen != this.searchScreen)
			{
				if (this.currentScreen != null)
				{
					this.currentScreen.Hide();
				}
				this.currentScreen = this.searchScreen;
				this.currentScreen.Show();
			}
		}

		// Token: 0x0600496B RID: 18795 RVA: 0x0015EC14 File Offset: 0x0015CE14
		private void SetTerminalState(SharedBlocksTerminal.TerminalState state)
		{
			this.localState.state = state;
			if (this.localState.driverID == -2)
			{
				this.statusMessageText.gameObject.SetActive(false);
				return;
			}
			switch (state)
			{
			case SharedBlocksTerminal.TerminalState.NoStatus:
				this.statusMessageText.gameObject.SetActive(false);
				return;
			case SharedBlocksTerminal.TerminalState.Searching:
				this.SetStatusText("SEARCHING...");
				return;
			case SharedBlocksTerminal.TerminalState.NotFound:
				this.SetStatusText("MAP NOT FOUND");
				return;
			case SharedBlocksTerminal.TerminalState.Found:
				this.SetStatusText("MAP FOUND. PRESS 'ENTER' TO LOAD");
				return;
			case SharedBlocksTerminal.TerminalState.Loading:
				this.SetStatusText("LOADING...");
				return;
			case SharedBlocksTerminal.TerminalState.LoadSuccess:
				this.SetStatusText("LOAD SUCCESS");
				return;
			case SharedBlocksTerminal.TerminalState.LoadFail:
				this.SetStatusText("LOAD FAILED");
				return;
			default:
				return;
			}
		}

		// Token: 0x0600496C RID: 18796 RVA: 0x0015ECCA File Offset: 0x0015CECA
		public void SelectMapIDAndOpenInfo(string mapID)
		{
			if (this.awaitingWebRequest)
			{
				return;
			}
			this.selectedMap = null;
			this.awaitingWebRequest = true;
			this.requestedMapID = mapID;
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.Searching);
			SharedBlocksManager.instance.RequestMapDataFromID(mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.OnPlayerMapRequestComplete));
		}

		// Token: 0x0600496D RID: 18797 RVA: 0x0015ED08 File Offset: 0x0015CF08
		private void OnPlayerMapRequestComplete(SharedBlocksManager.SharedBlocksMap response)
		{
			if (this.awaitingWebRequest)
			{
				this.awaitingWebRequest = false;
				this.requestedMapID = null;
				if (this.IsDriver)
				{
					if (response == null || response.MapID == null)
					{
						this.SetTerminalState(SharedBlocksTerminal.TerminalState.NotFound);
						return;
					}
					this.selectedMap = response;
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Found);
				}
			}
		}

		// Token: 0x0600496E RID: 18798 RVA: 0x0015ED54 File Offset: 0x0015CF54
		private bool CanChangeMapState(bool load, out string disallowedReason)
		{
			disallowedReason = "";
			if (!NetworkSystem.Instance.InRoom)
			{
				disallowedReason = "MUST BE IN A ROOM BEFORE  " + (load ? "" : "UN") + "LOADING A MAP.";
				return false;
			}
			this.RefreshLobbyCount();
			if (!this.AreAllPlayersInLobby())
			{
				disallowedReason = "ALL PLAYERS IN THE ROOM MUST BE INSIDE THE LOBBY BEFORE " + (load ? "" : "UN") + "LOADING A MAP.";
				return false;
			}
			return true;
		}

		// Token: 0x0600496F RID: 18799 RVA: 0x0015EDC7 File Offset: 0x0015CFC7
		public void SetStatusText(string text)
		{
			this.statusMessageText.text = text;
			this.statusMessageText.gameObject.SetActive(true);
		}

		// Token: 0x06004970 RID: 18800 RVA: 0x0015EDE6 File Offset: 0x0015CFE6
		private bool IsLocalPlayerInLobby()
		{
			return base.isActiveAndEnabled && this.lobbyTrigger.playerIDsCurrentlyTouching.Contains(VRRig.LocalRig.creator.UserId);
		}

		// Token: 0x06004971 RID: 18801 RVA: 0x0015EE16 File Offset: 0x0015D016
		public bool AreAllPlayersInLobby()
		{
			return base.isActiveAndEnabled && this.playersInLobby == this.playersInRoom;
		}

		// Token: 0x06004972 RID: 18802 RVA: 0x0015EE30 File Offset: 0x0015D030
		public string GetLobbyText()
		{
			return string.Format("PLAYERS IN ROOM {0}\nPLAYERS IN LOBBY {1}", this.playersInRoom, this.playersInLobby);
		}

		// Token: 0x06004973 RID: 18803 RVA: 0x0015EE52 File Offset: 0x0015D052
		public void RefreshLobbyCount()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom)
			{
				this.playersInRoom = NetworkSystem.Instance.RoomPlayerCount;
				return;
			}
			this.playersInRoom = 0;
		}

		// Token: 0x06004974 RID: 18804 RVA: 0x0015EE88 File Offset: 0x0015D088
		public void PressButton(SharedBlocksKeyboardBindings buttonPressed)
		{
			if (!this.IsDriver)
			{
				this.SetStatusText("NOT TERMINAL CONTROLLER");
				return;
			}
			if (this.localState.state == SharedBlocksTerminal.TerminalState.Searching || this.localState.state == SharedBlocksTerminal.TerminalState.Loading)
			{
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.up)
			{
				this.OnUpButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.down)
			{
				this.OnDownButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.delete)
			{
				this.OnDeleteButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.enter)
			{
				this.OnSelectButtonPressed();
				return;
			}
			if (buttonPressed >= SharedBlocksKeyboardBindings.zero && buttonPressed <= SharedBlocksKeyboardBindings.nine)
			{
				this.OnNumberPressed((int)buttonPressed);
				return;
			}
			if (buttonPressed >= SharedBlocksKeyboardBindings.A && buttonPressed <= SharedBlocksKeyboardBindings.Z)
			{
				this.OnLetterPressed(buttonPressed.ToString());
			}
		}

		// Token: 0x06004975 RID: 18805 RVA: 0x0015EF24 File Offset: 0x0015D124
		private void OnUpButtonPressed()
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnUpPressed();
			}
		}

		// Token: 0x06004976 RID: 18806 RVA: 0x0015EF3F File Offset: 0x0015D13F
		private void OnDownButtonPressed()
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnDownPressed();
			}
		}

		// Token: 0x06004977 RID: 18807 RVA: 0x0015EF5A File Offset: 0x0015D15A
		private void OnSelectButtonPressed()
		{
			if (this.localState.state == SharedBlocksTerminal.TerminalState.Found)
			{
				this.OnLoadMapPressed();
				return;
			}
			if (this.currentScreen != null)
			{
				this.currentScreen.OnSelectPressed();
			}
		}

		// Token: 0x06004978 RID: 18808 RVA: 0x0015EF8A File Offset: 0x0015D18A
		private void OnDeleteButtonPressed()
		{
			if (this.localState.state != SharedBlocksTerminal.TerminalState.Loading && this.localState.state != SharedBlocksTerminal.TerminalState.Searching)
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			}
			if (this.currentScreen != null)
			{
				this.currentScreen.OnDeletePressed();
			}
		}

		// Token: 0x06004979 RID: 18809 RVA: 0x000023F4 File Offset: 0x000005F4
		private void OnBackButtonPressed()
		{
		}

		// Token: 0x0600497A RID: 18810 RVA: 0x0015EFC8 File Offset: 0x0015D1C8
		private void OnNumberPressed(int number)
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnNumberPressed(number);
			}
		}

		// Token: 0x0600497B RID: 18811 RVA: 0x0015EFE4 File Offset: 0x0015D1E4
		private void OnLetterPressed(string letter)
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnLetterPressed(letter);
			}
		}

		// Token: 0x0600497C RID: 18812 RVA: 0x0015F000 File Offset: 0x0015D200
		private void OnTerminalControlPressed()
		{
			if (this.isTerminalLocked)
			{
				if (this.IsDriver)
				{
					if (NetworkSystem.Instance.InRoom)
					{
						this.linkedTable.builderNetworking.RequestBlocksTerminalControl(false);
						return;
					}
					this.SetTerminalDriver(-2);
					return;
				}
			}
			else
			{
				if (NetworkSystem.Instance.InRoom)
				{
					this.linkedTable.builderNetworking.RequestBlocksTerminalControl(true);
					return;
				}
				this.SetTerminalDriver(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			}
		}

		// Token: 0x0600497D RID: 18813 RVA: 0x0015F078 File Offset: 0x0015D278
		public void OnLoadMapPressed()
		{
			if (!this.IsDriver)
			{
				this.SetStatusText("NOT TERMINAL CONTROLLER");
				return;
			}
			if (this.currentScreen == null || this.selectedMap == null)
			{
				this.SetStatusText("NO MAP SELECTED");
				return;
			}
			if (this.awaitingWebRequest || this.isLoadingMap)
			{
				this.SetStatusText("BLOCKS LOAD ALREADY IN PROGRESS");
				return;
			}
			string text;
			if (!this.CanChangeMapState(true, out text))
			{
				this.SetStatusText(text);
				return;
			}
			if (this.linkedTable != null)
			{
				if (Time.time > this.lastLoadTime + this.loadMapCooldown)
				{
					this.SetStatusText("LOADING BLOCKS ...");
					this.isLoadingMap = true;
					this.lastLoadTime = Time.time;
					this.linkedTable.LoadSharedMap(this.selectedMap);
					return;
				}
				int num = Mathf.RoundToInt(this.lastLoadTime + this.loadMapCooldown - Time.time);
				this.SetStatusText(string.Format("PLEASE WAIT {0} SECONDS BEFORE LOADING ANOTHER MAP", num));
			}
		}

		// Token: 0x0600497E RID: 18814 RVA: 0x0015F16A File Offset: 0x0015D36A
		public bool IsPlayerDriver(Player player)
		{
			return player.ActorNumber == this.localState.driverID;
		}

		// Token: 0x0600497F RID: 18815 RVA: 0x0015F17F File Offset: 0x0015D37F
		public bool ValidateTerminalControlRequest(bool locked, int playerNumber)
		{
			if (locked && playerNumber == -2)
			{
				return false;
			}
			if (this.localState.driverID == -2)
			{
				return locked;
			}
			return this.localState.driverID == playerNumber;
		}

		// Token: 0x06004980 RID: 18816 RVA: 0x0015F1AA File Offset: 0x0015D3AA
		private void OnDriverNameChanged()
		{
			this.RefreshDriverNickname();
		}

		// Token: 0x06004981 RID: 18817 RVA: 0x0015F1B4 File Offset: 0x0015D3B4
		public void SetTerminalDriver(int playerNum)
		{
			if (playerNum != -2)
			{
				if (this.localState.driverID != -2 && this.localState.driverID != playerNum)
				{
					GTDev.LogWarning<string>(string.Format("Shared BlocksTerminal SetTerminalDriver cannot set {0} as driver while {1} is driver", playerNum, this.localState.driverID), null);
					return;
				}
				this.localState.driverID = playerNum;
				NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerNum);
				RigContainer rigContainer;
				if (netPlayerByID != null && VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					this.driverRig = rigContainer.Rig;
					this.driverRig.OnPlayerNameVisibleChanged += this.OnDriverNameChanged;
				}
				this.isTerminalLocked = true;
				this.UpdateTerminalButton();
				this.RefreshActiveScreen();
				this.searchScreen.SetInputTextEnabled(this.IsDriver);
				if (this.IsDriver && this.awaitingWebRequest)
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Searching);
					this.searchScreen.SetMapCode(this.requestedMapID);
				}
				else if (this.isLoadingMap)
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Loading);
					this.searchScreen.SetMapCode(this.linkedTable.GetPendingMap());
				}
				else
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
				}
			}
			else
			{
				if (this.driverRig != null)
				{
					this.driverRig.OnPlayerNameVisibleChanged -= this.OnDriverNameChanged;
					this.driverRig = null;
				}
				this.localState.driverID = -2;
				this.isTerminalLocked = false;
				this.UpdateTerminalButton();
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
				this.RefreshActiveScreen();
			}
			this.RefreshDriverNickname();
		}

		// Token: 0x06004982 RID: 18818 RVA: 0x0015F334 File Offset: 0x0015D534
		private void RefreshDriverNickname()
		{
			if (this.localState.driverID == -2)
			{
				this.currentDriverLabel.gameObject.SetActive(false);
				this.currentDriverText.text = "";
				this.currentDriverText.gameObject.SetActive(false);
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
			if (NetworkSystem.Instance.InRoom)
			{
				NetPlayer player = NetworkSystem.Instance.GetPlayer(this.localState.driverID);
				if (player != null && this.useNametags && flag)
				{
					RigContainer rigContainer;
					if (player.IsLocal)
					{
						this.currentDriverText.text = player.NickName;
					}
					else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
					{
						this.currentDriverText.text = rigContainer.Rig.playerNameVisible;
					}
					else
					{
						this.currentDriverText.text = player.DefaultName;
					}
				}
				else
				{
					this.currentDriverText.text = "";
				}
			}
			else
			{
				this.currentDriverText.text = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
			}
			this.currentDriverLabel.gameObject.SetActive(true);
			this.currentDriverText.gameObject.SetActive(true);
		}

		// Token: 0x06004983 RID: 18819 RVA: 0x0015F480 File Offset: 0x0015D680
		public bool ValidateLoadMapRequest(string mapID, int playerNum)
		{
			return playerNum == this.localState.driverID && SharedBlocksManager.IsMapIDValid(mapID);
		}

		// Token: 0x06004984 RID: 18820 RVA: 0x0015F498 File Offset: 0x0015D698
		private void OnJoinedRoom()
		{
			GTDev.Log<string>("[SharedBlocksTerminal::OnJoinedRoom] Joined a multiplayer room, resetting terminal control", null);
			this.cachedLocalPlayerID = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			this.ResetTerminalControl();
		}

		// Token: 0x06004985 RID: 18821 RVA: 0x0015F4C0 File Offset: 0x0015D6C0
		private void OnReturnedToSinglePlayer()
		{
			if (this.localState.driverID != this.cachedLocalPlayerID)
			{
				this.ResetTerminalControl();
			}
			else
			{
				this.localState.driverID = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			}
			this.cachedLocalPlayerID = -1;
		}

		// Token: 0x06004986 RID: 18822 RVA: 0x0015F4FE File Offset: 0x0015D6FE
		public void ResetTerminalControl()
		{
			this.localState.driverID = -2;
			this.isTerminalLocked = false;
			this.selectedMap = null;
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			this.RefreshActiveScreen();
			this.UpdateTerminalButton();
		}

		// Token: 0x06004987 RID: 18823 RVA: 0x0015F52E File Offset: 0x0015D72E
		private void UpdateTerminalButton()
		{
			this.terminalControlButton.isOn = this.isTerminalLocked;
			this.terminalControlButton.UpdateColor();
		}

		// Token: 0x06004988 RID: 18824 RVA: 0x0015F54C File Offset: 0x0015D74C
		private void OnSharedBlocksMapLoaded(string mapID)
		{
			if (!this.IsDriver)
			{
				this.searchScreen.SetMapCode(mapID);
			}
			if (SharedBlocksManager.IsMapIDValid(mapID))
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadSuccess);
			}
			else if (this.localState.state != SharedBlocksTerminal.TerminalState.LoadFail)
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadFail);
			}
			this.isLoadingMap = false;
		}

		// Token: 0x06004989 RID: 18825 RVA: 0x0015F59A File Offset: 0x0015D79A
		private void OnSharedBlocksMapLoadFailed(string message)
		{
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadFail);
			this.SetStatusText(message);
			this.isLoadingMap = false;
		}

		// Token: 0x0600498A RID: 18826 RVA: 0x0015F5B4 File Offset: 0x0015D7B4
		private void OnSharedBlocksMapLoadStart()
		{
			if (this.linkedTable == null)
			{
				return;
			}
			if (!this.IsDriver)
			{
				this.searchScreen.SetMapCode(this.linkedTable.GetPendingMap());
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.Loading);
				this.isLoadingMap = true;
				this.lastLoadTime = Time.time;
			}
		}

		// Token: 0x04004C30 RID: 19504
		[SerializeField]
		private GTZone tableZone = GTZone.monkeBlocksShared;

		// Token: 0x04004C31 RID: 19505
		[SerializeField]
		private TMP_Text currentMapSelectionText;

		// Token: 0x04004C32 RID: 19506
		[SerializeField]
		private TMP_Text statusMessageText;

		// Token: 0x04004C33 RID: 19507
		[SerializeField]
		private TMP_Text currentDriverText;

		// Token: 0x04004C34 RID: 19508
		[SerializeField]
		private TMP_Text currentDriverLabel;

		// Token: 0x04004C35 RID: 19509
		[SerializeField]
		private SharedBlocksScreen noDriverScreen;

		// Token: 0x04004C36 RID: 19510
		[SerializeField]
		private SharedBlocksScreenSearch searchScreen;

		// Token: 0x04004C37 RID: 19511
		[SerializeField]
		private GorillaPressableButton terminalControlButton;

		// Token: 0x04004C38 RID: 19512
		[SerializeField]
		private float loadMapCooldown = 30f;

		// Token: 0x04004C39 RID: 19513
		[SerializeField]
		private GorillaFriendCollider lobbyTrigger;

		// Token: 0x04004C3A RID: 19514
		private SharedBlocksManager.SharedBlocksMap selectedMap;

		// Token: 0x04004C3B RID: 19515
		private SharedBlocksScreen currentScreen;

		// Token: 0x04004C3C RID: 19516
		private BuilderTable linkedTable;

		// Token: 0x04004C3D RID: 19517
		public const int NO_DRIVER_ID = -2;

		// Token: 0x04004C3E RID: 19518
		private bool awaitingWebRequest;

		// Token: 0x04004C3F RID: 19519
		private string requestedMapID;

		// Token: 0x04004C40 RID: 19520
		public const string POINTER = "> ";

		// Token: 0x04004C41 RID: 19521
		public Action<bool> OnMapLoadComplete;

		// Token: 0x04004C42 RID: 19522
		private bool isTerminalLocked;

		// Token: 0x04004C43 RID: 19523
		private SharedBlocksTerminal.SharedBlocksTerminalState localState;

		// Token: 0x04004C44 RID: 19524
		private int cachedLocalPlayerID = -1;

		// Token: 0x04004C45 RID: 19525
		private bool isLoadingMap;

		// Token: 0x04004C46 RID: 19526
		private float lastLoadTime;

		// Token: 0x04004C47 RID: 19527
		private bool useNametags;

		// Token: 0x04004C48 RID: 19528
		private bool hasInitialized;

		// Token: 0x04004C49 RID: 19529
		private static StringBuilder sb = new StringBuilder();

		// Token: 0x04004C4A RID: 19530
		private VRRig driverRig;

		// Token: 0x04004C4B RID: 19531
		private static List<VRRig> tempRigs = new List<VRRig>(16);

		// Token: 0x04004C4C RID: 19532
		private int playersInRoom;

		// Token: 0x02000B93 RID: 2963
		public enum ScreenType
		{
			// Token: 0x04004C4E RID: 19534
			NO_DRIVER,
			// Token: 0x04004C4F RID: 19535
			SEARCH,
			// Token: 0x04004C50 RID: 19536
			LOADING,
			// Token: 0x04004C51 RID: 19537
			ERROR,
			// Token: 0x04004C52 RID: 19538
			SCAN_INFO,
			// Token: 0x04004C53 RID: 19539
			OTHER_DRIVER
		}

		// Token: 0x02000B94 RID: 2964
		public enum TerminalState
		{
			// Token: 0x04004C55 RID: 19541
			NoStatus,
			// Token: 0x04004C56 RID: 19542
			Searching,
			// Token: 0x04004C57 RID: 19543
			NotFound,
			// Token: 0x04004C58 RID: 19544
			Found,
			// Token: 0x04004C59 RID: 19545
			Loading,
			// Token: 0x04004C5A RID: 19546
			LoadSuccess,
			// Token: 0x04004C5B RID: 19547
			LoadFail
		}

		// Token: 0x02000B95 RID: 2965
		public class SharedBlocksTerminalState
		{
			// Token: 0x04004C5C RID: 19548
			public SharedBlocksTerminal.ScreenType currentScreen;

			// Token: 0x04004C5D RID: 19549
			public SharedBlocksTerminal.TerminalState state;

			// Token: 0x04004C5E RID: 19550
			public int driverID;
		}
	}
}
