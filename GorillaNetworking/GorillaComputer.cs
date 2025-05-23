using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaGameModes;
using GorillaTagScripts;
using GorillaTagScripts.ModIO;
using KID.Model;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaNetworking
{
	// Token: 0x02000C2E RID: 3118
	public class GorillaComputer : MonoBehaviour, IMatchmakingCallbacks, IGorillaSliceableSimple
	{
		// Token: 0x06004D2B RID: 19755 RVA: 0x0016F4BF File Offset: 0x0016D6BF
		public DateTime GetServerTime()
		{
			return this.startupTime + TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
		}

		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x06004D2C RID: 19756 RVA: 0x0016F4D7 File Offset: 0x0016D6D7
		public string VStumpRoomPrepend
		{
			get
			{
				return this.virtualStumpRoomPrepend;
			}
		}

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x06004D2D RID: 19757 RVA: 0x0016F4E0 File Offset: 0x0016D6E0
		public GorillaComputer.ComputerState currentState
		{
			get
			{
				GorillaComputer.ComputerState computerState;
				this.stateStack.TryPeek(out computerState);
				return computerState;
			}
		}

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x06004D2E RID: 19758 RVA: 0x0016F4FC File Offset: 0x0016D6FC
		public string NameTagPlayerPref
		{
			get
			{
				if (PlayFabAuthenticator.instance == null)
				{
					Debug.LogError("Trying to access PlayFab Authenticator Instance, but it is null. Will use a shared key for the nametag instead");
					return "nameTagsOn";
				}
				return "nameTagsOn-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
		}

		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x06004D2F RID: 19759 RVA: 0x0016F533 File Offset: 0x0016D733
		// (set) Token: 0x06004D30 RID: 19760 RVA: 0x0016F53B File Offset: 0x0016D73B
		public bool NametagsEnabled { get; private set; }

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x06004D31 RID: 19761 RVA: 0x0016F544 File Offset: 0x0016D744
		// (set) Token: 0x06004D32 RID: 19762 RVA: 0x0016F54C File Offset: 0x0016D74C
		public GorillaComputer.RedemptionResult RedemptionStatus
		{
			get
			{
				return this.redemptionResult;
			}
			set
			{
				this.redemptionResult = value;
				this.UpdateScreen();
			}
		}

		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x06004D33 RID: 19763 RVA: 0x0016F55B File Offset: 0x0016D75B
		// (set) Token: 0x06004D34 RID: 19764 RVA: 0x0016F563 File Offset: 0x0016D763
		public string RedemptionCode
		{
			get
			{
				return this.redemptionCode;
			}
			set
			{
				this.redemptionCode = value;
			}
		}

		// Token: 0x06004D35 RID: 19765 RVA: 0x0016F56C File Offset: 0x0016D76C
		private void Awake()
		{
			if (GorillaComputer.instance == null)
			{
				GorillaComputer.instance = this;
				GorillaComputer.hasInstance = true;
			}
			else if (GorillaComputer.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this._activeOrderList = this.OrderList;
		}

		// Token: 0x06004D36 RID: 19766 RVA: 0x0016F5BE File Offset: 0x0016D7BE
		private void Start()
		{
			Debug.Log("Computer Init");
			this.Initialise();
		}

		// Token: 0x06004D37 RID: 19767 RVA: 0x0016F5D0 File Offset: 0x0016D7D0
		public void OnEnable()
		{
			KIDManager.RegisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.RegisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06004D38 RID: 19768 RVA: 0x0016F5FB File Offset: 0x0016D7FB
		public void OnDisable()
		{
			KIDManager.UnregisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.UnregisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06004D39 RID: 19769 RVA: 0x0016F626 File Offset: 0x0016D826
		protected void OnDestroy()
		{
			if (GorillaComputer.instance == this)
			{
				GorillaComputer.hasInstance = false;
				GorillaComputer.instance = null;
			}
			KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		// Token: 0x06004D3A RID: 19770 RVA: 0x0016F658 File Offset: 0x0016D858
		public void SliceUpdate()
		{
			if ((this.internetFailure && Time.time < this.lastCheckedWifi + this.checkIfConnectedSeconds) || (!this.internetFailure && Time.time < this.lastCheckedWifi + this.checkIfDisconnectedSeconds))
			{
				if (!this.internetFailure && this.isConnectedToMaster && Time.time > this.lastUpdateTime + this.updateCooldown)
				{
					this.lastUpdateTime = Time.time;
					this.UpdateScreen();
				}
				return;
			}
			this.lastCheckedWifi = Time.time;
			this.stateUpdated = false;
			if (!this.CheckInternetConnection())
			{
				this.UpdateFailureText("NO WIFI OR LAN CONNECTION DETECTED.");
				this.internetFailure = true;
				return;
			}
			if (this.internetFailure)
			{
				if (this.CheckInternetConnection())
				{
					this.internetFailure = false;
				}
				this.RestoreFromFailureState();
				this.UpdateScreen();
				return;
			}
			if (this.isConnectedToMaster && Time.time > this.lastUpdateTime + this.updateCooldown)
			{
				this.lastUpdateTime = Time.time;
				this.UpdateScreen();
			}
		}

		// Token: 0x06004D3B RID: 19771 RVA: 0x0016F754 File Offset: 0x0016D954
		private void Initialise()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.AddListener(new UnityAction<GorillaKeyboardBindings>(this.PressButton));
			RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.UpdateScreen));
			RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(this.UpdateScreen));
			RoomSystem.PlayerJoinedEvent = (Action<NetPlayer>)Delegate.Combine(RoomSystem.PlayerJoinedEvent, new Action<NetPlayer>(this.PlayerCountChangedCallback));
			RoomSystem.PlayerLeftEvent = (Action<NetPlayer>)Delegate.Combine(RoomSystem.PlayerLeftEvent, new Action<NetPlayer>(this.PlayerCountChangedCallback));
			this.InitialiseRoomScreens();
			this.InitialiseStrings();
			this.InitialiseAllRoomStates();
			this.UpdateScreen();
			byte[] array = new byte[] { Convert.ToByte(64) };
			this.virtualStumpRoomPrepend = Encoding.ASCII.GetString(array);
			this.initialized = true;
		}

		// Token: 0x06004D3C RID: 19772 RVA: 0x0016F838 File Offset: 0x0016DA38
		private void InitialiseRoomScreens()
		{
			this.screenText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.ScreenTextChangedEvent, GameEvents.ScreenTextMaterialsEvent);
			this.functionSelectText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.FunctionSelectTextChangedEvent, null);
		}

		// Token: 0x06004D3D RID: 19773 RVA: 0x0016F890 File Offset: 0x0016DA90
		private void InitialiseStrings()
		{
			this.roomToJoin = "";
			this.redText = "";
			this.blueText = "";
			this.greenText = "";
			this.currentName = "";
			this.savedName = "";
		}

		// Token: 0x06004D3E RID: 19774 RVA: 0x0016F8E0 File Offset: 0x0016DAE0
		private void InitialiseAllRoomStates()
		{
			this.SwitchState(GorillaComputer.ComputerState.Startup, true);
			this.InitializeColorState();
			this.InitializeNameState();
			this.InitializeRoomState();
			this.InitializeTurnState();
			this.InitializeStartupState();
			this.InitializeQueueState();
			this.InitializeMicState();
			this.InitializeGroupState();
			this.InitializeVoiceState();
			this.InitializeAutoMuteState();
			this.InitializeGameMode();
			this.InitializeVisualsState();
			this.InitializeCreditsState();
			this.InitializeTimeState();
			this.InitializeSupportState();
			this.InitializeTroopState();
			this.InitializeKIdState();
			this.InitializeRedeemState();
		}

		// Token: 0x06004D3F RID: 19775 RVA: 0x000023F4 File Offset: 0x000005F4
		private void InitializeStartupState()
		{
		}

		// Token: 0x06004D40 RID: 19776 RVA: 0x000023F4 File Offset: 0x000005F4
		private void InitializeRoomState()
		{
		}

		// Token: 0x06004D41 RID: 19777 RVA: 0x0016F964 File Offset: 0x0016DB64
		private void InitializeColorState()
		{
			this.redValue = PlayerPrefs.GetFloat("redValue", 0f);
			this.greenValue = PlayerPrefs.GetFloat("greenValue", 0f);
			this.blueValue = PlayerPrefs.GetFloat("blueValue", 0f);
			this.blueText = Mathf.Floor(this.blueValue * 9f).ToString();
			this.redText = Mathf.Floor(this.redValue * 9f).ToString();
			this.greenText = Mathf.Floor(this.greenValue * 9f).ToString();
			this.colorCursorLine = 0;
			GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
		}

		// Token: 0x06004D42 RID: 19778 RVA: 0x0016FA30 File Offset: 0x0016DC30
		private void InitializeNameState()
		{
			int @int = PlayerPrefs.GetInt("nameTagsOn", -1);
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			switch (permissionDataByFeature.ManagedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (@int == -1)
				{
					this.NametagsEnabled = permissionDataByFeature.Enabled;
				}
				else
				{
					this.NametagsEnabled = @int > 0;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				this.NametagsEnabled = permissionDataByFeature.Enabled && @int > 0;
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.NametagsEnabled = false;
				break;
			}
			this.savedName = PlayerPrefs.GetString("playerName", "gorilla");
			NetworkSystem.Instance.SetMyNickName(this.savedName);
			this.currentName = this.savedName;
			VRRigCache.Instance.localRig.Rig.UpdateName();
			this.exactOneWeek = this.exactOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereOneWeek = this.anywhereOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereTwoWeek = this.anywhereTwoWeekFile.text.Split('\n', StringSplitOptions.None);
			for (int i = 0; i < this.exactOneWeek.Length; i++)
			{
				this.exactOneWeek[i] = this.exactOneWeek[i].ToLower().TrimEnd(new char[] { '\r', '\n' });
			}
			for (int j = 0; j < this.anywhereOneWeek.Length; j++)
			{
				this.anywhereOneWeek[j] = this.anywhereOneWeek[j].ToLower().TrimEnd(new char[] { '\r', '\n' });
			}
			for (int k = 0; k < this.anywhereTwoWeek.Length; k++)
			{
				this.anywhereTwoWeek[k] = this.anywhereTwoWeek[k].ToLower().TrimEnd(new char[] { '\r', '\n' });
			}
		}

		// Token: 0x06004D43 RID: 19779 RVA: 0x0016FBFC File Offset: 0x0016DDFC
		private void InitializeTurnState()
		{
			GorillaSnapTurn.LoadSettingsFromPlayerPrefs();
		}

		// Token: 0x06004D44 RID: 19780 RVA: 0x0016FC03 File Offset: 0x0016DE03
		private void InitializeMicState()
		{
			this.pttType = PlayerPrefs.GetString("pttType", "ALL CHAT");
		}

		// Token: 0x06004D45 RID: 19781 RVA: 0x0016FC1C File Offset: 0x0016DE1C
		private void InitializeAutoMuteState()
		{
			int @int = PlayerPrefs.GetInt("autoMute", 1);
			if (@int == 0)
			{
				this.autoMuteType = "OFF";
				return;
			}
			if (@int == 1)
			{
				this.autoMuteType = "MODERATE";
				return;
			}
			if (@int == 2)
			{
				this.autoMuteType = "AGGRESSIVE";
			}
		}

		// Token: 0x06004D46 RID: 19782 RVA: 0x0016FC64 File Offset: 0x0016DE64
		private void InitializeQueueState()
		{
			this.currentQueue = PlayerPrefs.GetString("currentQueue", "DEFAULT");
			this.allowedInCompetitive = PlayerPrefs.GetInt("allowedInCompetitive", 0) == 1;
			if (!this.allowedInCompetitive && this.currentQueue == "COMPETITIVE")
			{
				PlayerPrefs.SetString("currentQueue", "DEFAULT");
				PlayerPrefs.Save();
				this.currentQueue = "DEFAULT";
			}
		}

		// Token: 0x06004D47 RID: 19783 RVA: 0x0016FCD3 File Offset: 0x0016DED3
		private void InitializeGroupState()
		{
			this.groupMapJoin = PlayerPrefs.GetString("groupMapJoin", "FOREST");
			this.groupMapJoinIndex = PlayerPrefs.GetInt("groupMapJoinIndex", 0);
			this.allowedMapsToJoin = this.friendJoinCollider.myAllowedMapsToJoin;
		}

		// Token: 0x06004D48 RID: 19784 RVA: 0x0016FD0C File Offset: 0x0016DF0C
		private void InitializeTroopState()
		{
			bool flag = false;
			this.troopToJoin = (this.troopName = PlayerPrefs.GetString("troopName", string.Empty));
			this.troopQueueActive = PlayerPrefs.GetInt("troopQueueActive", 0) == 1;
			if (this.troopQueueActive && !this.IsValidTroopName(this.troopName))
			{
				this.troopQueueActive = false;
				PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
				this.currentQueue = "DEFAULT";
				PlayerPrefs.SetString("currentQueue", this.currentQueue);
				flag = true;
			}
			if (this.troopQueueActive)
			{
				base.StartCoroutine(this.HandleInitialTroopQueueState());
			}
			if (flag)
			{
				PlayerPrefs.Save();
			}
		}

		// Token: 0x06004D49 RID: 19785 RVA: 0x0016FDBA File Offset: 0x0016DFBA
		private IEnumerator HandleInitialTroopQueueState()
		{
			Debug.Log("HandleInitialTroopQueueState()");
			while (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				yield return null;
			}
			this.RequestTroopPopulation(false);
			while (this.currentTroopPopulation < 0)
			{
				yield return null;
			}
			if (this.currentTroopPopulation < 2)
			{
				Debug.Log("Low population - starting in DEFAULT queue");
				this.JoinDefaultQueue();
			}
			yield break;
		}

		// Token: 0x06004D4A RID: 19786 RVA: 0x0016FDCC File Offset: 0x0016DFCC
		private void InitializeVoiceState()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			string text = PlayerPrefs.GetString("voiceChatOn", "");
			string text2 = "FALSE";
			switch (permissionDataByFeature.ManagedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (string.IsNullOrEmpty(text))
				{
					text2 = (permissionDataByFeature.Enabled ? "TRUE" : "FALSE");
				}
				else
				{
					text2 = text;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				if (permissionDataByFeature.Enabled)
				{
					text = (string.IsNullOrEmpty(text) ? "FALSE" : text);
					text2 = text;
				}
				else
				{
					text2 = "FALSE";
				}
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				text2 = "FALSE";
				break;
			}
			this.voiceChatOn = PlayerPrefs.GetString("voiceChatOn", text2);
		}

		// Token: 0x06004D4B RID: 19787 RVA: 0x0016FE74 File Offset: 0x0016E074
		private void InitializeGameMode()
		{
			string text = PlayerPrefs.GetString("currentGameMode", GameModeType.Infection.ToString());
			GameModeType gameModeType;
			try
			{
				gameModeType = Enum.Parse<GameModeType>(text, true);
			}
			catch
			{
				gameModeType = GameModeType.Infection;
				text = GameModeType.Infection.ToString();
			}
			if (gameModeType != GameModeType.Casual && gameModeType != GameModeType.Infection && gameModeType != GameModeType.Hunt && gameModeType != GameModeType.Paintbrawl && gameModeType != GameModeType.Ambush)
			{
				PlayerPrefs.SetString("currentGameMode", GameModeType.Infection.ToString());
				PlayerPrefs.Save();
				text = GameModeType.Infection.ToString();
			}
			this.leftHanded = PlayerPrefs.GetInt("leftHanded", 0) == 1;
			this.OnModeSelectButtonPress(text, this.leftHanded);
			GameModePages.SetSelectedGameModeShared(text);
		}

		// Token: 0x06004D4C RID: 19788 RVA: 0x000023F4 File Offset: 0x000005F4
		private void InitializeCreditsState()
		{
		}

		// Token: 0x06004D4D RID: 19789 RVA: 0x0016FF34 File Offset: 0x0016E134
		private void InitializeTimeState()
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}

		// Token: 0x06004D4E RID: 19790 RVA: 0x0016FF43 File Offset: 0x0016E143
		private void InitializeSupportState()
		{
			this.displaySupport = false;
		}

		// Token: 0x06004D4F RID: 19791 RVA: 0x0016FF4C File Offset: 0x0016E14C
		private void InitializeVisualsState()
		{
			this.disableParticles = PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE";
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
			this.instrumentVolume = PlayerPrefs.GetFloat("instrumentVolume", 0.1f);
		}

		// Token: 0x06004D50 RID: 19792 RVA: 0x0016FFA0 File Offset: 0x0016E1A0
		private void InitializeRedeemState()
		{
			this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
		}

		// Token: 0x06004D51 RID: 19793 RVA: 0x0016FFA9 File Offset: 0x0016E1A9
		private bool CheckInternetConnection()
		{
			return Application.internetReachability > NetworkReachability.NotReachable;
		}

		// Token: 0x06004D52 RID: 19794 RVA: 0x0016FFB4 File Offset: 0x0016E1B4
		public void OnConnectedToMasterStuff()
		{
			if (!this.isConnectedToMaster)
			{
				this.isConnectedToMaster = true;
				GorillaServer.Instance.ReturnCurrentVersion(new ReturnCurrentVersionRequest
				{
					CurrentVersion = NetworkSystemConfig.AppVersionStripped,
					UpdatedSynchTest = new int?(this.includeUpdatedServerSynchTest)
				}, new Action<ExecuteFunctionResult>(this.OnReturnCurrentVersion), new Action<PlayFabError>(GorillaComputer.OnErrorShared));
				if (this.startupMillis == 0L && !this.tryGetTimeAgain)
				{
					this.GetCurrentTime();
				}
				RuntimePlatform platform = Application.platform;
				this.SaveModAccountData();
				bool safety = PlayFabAuthenticator.instance.GetSafety();
				if (!KIDManager.KidEnabledAndReady && !KIDManager.HasSession)
				{
					this.SetComputerSettingsBySafety(safety, new GorillaComputer.ComputerState[]
					{
						GorillaComputer.ComputerState.Voice,
						GorillaComputer.ComputerState.AutoMute,
						GorillaComputer.ComputerState.Name,
						GorillaComputer.ComputerState.Group
					}, false);
				}
			}
		}

		// Token: 0x06004D53 RID: 19795 RVA: 0x00170074 File Offset: 0x0016E274
		private void OnReturnCurrentVersion(ExecuteFunctionResult result)
		{
			JsonObject jsonObject = (JsonObject)result.FunctionResult;
			if (jsonObject == null)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			object obj;
			if (jsonObject.TryGetValue("SynchTime", out obj))
			{
				Debug.Log("message value is: " + (string)obj);
			}
			if (jsonObject.TryGetValue("Fail", out obj) && (bool)obj)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("ResultCode", out obj) && (ulong)obj != 0UL)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("QueueStats", out obj) && ((JsonObject)obj).TryGetValue("TopTroops", out obj))
			{
				this.topTroops.Clear();
				foreach (object obj2 in ((JsonArray)obj))
				{
					this.topTroops.Add(obj2.ToString());
				}
			}
			if (jsonObject.TryGetValue("BannedUsers", out obj))
			{
				this.usersBanned = int.Parse((string)obj);
			}
			this.UpdateScreen();
		}

		// Token: 0x06004D54 RID: 19796 RVA: 0x001701B0 File Offset: 0x0016E3B0
		public void SaveModAccountData()
		{
			string path = Application.persistentDataPath + "/DoNotShareWithAnyoneEVERNoMatterWhatTheySay.txt";
			if (File.Exists(path))
			{
				return;
			}
			GorillaServer.Instance.ReturnMyOculusHash(delegate(ExecuteFunctionResult result)
			{
				object obj;
				if (((JsonObject)result.FunctionResult).TryGetValue("oculusHash", out obj))
				{
					StreamWriter streamWriter = new StreamWriter(path);
					streamWriter.Write(PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "." + (string)obj);
					streamWriter.Close();
				}
			}, delegate(PlayFabError error)
			{
				if (error.Error == PlayFabErrorCode.NotAuthenticated)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == PlayFabErrorCode.AccountBanned)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			});
		}

		// Token: 0x06004D55 RID: 19797 RVA: 0x00170220 File Offset: 0x0016E420
		public void PressButton(GorillaKeyboardBindings buttonPressed)
		{
			if (this.currentState == GorillaComputer.ComputerState.Startup)
			{
				this.ProcessStartupState(buttonPressed);
				this.UpdateScreen();
				return;
			}
			this.RequestTroopPopulation(false);
			bool flag = true;
			if (buttonPressed == GorillaKeyboardBindings.up)
			{
				flag = false;
				this.DecreaseState();
			}
			else if (buttonPressed == GorillaKeyboardBindings.down)
			{
				flag = false;
				this.IncreaseState();
			}
			if (flag)
			{
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Color:
					this.ProcessColorState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Name:
					this.ProcessNameState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Turn:
					this.ProcessTurnState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Mic:
					this.ProcessMicState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Room:
					this.ProcessRoomState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Queue:
					this.ProcessQueueState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Group:
					this.ProcessGroupState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Voice:
					this.ProcessVoiceState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.ProcessAutoMuteState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Credits:
					this.ProcessCreditsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.ProcessVisualsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.ProcessNameWarningState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Support:
					this.ProcessSupportState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Troop:
					this.ProcessTroopState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.KID:
					this.ProcessKIdState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.ProcessRedemptionState(buttonPressed);
					break;
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x06004D56 RID: 19798 RVA: 0x00170364 File Offset: 0x0016E564
		public void OnModeSelectButtonPress(string gameMode, bool leftHand)
		{
			this.lastPressedGameMode = gameMode;
			PlayerPrefs.SetString("currentGameMode", gameMode);
			if (leftHand != this.leftHanded)
			{
				PlayerPrefs.SetInt("leftHanded", leftHand ? 1 : 0);
				this.leftHanded = leftHand;
			}
			PlayerPrefs.Save();
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				FriendshipGroupDetection.Instance.SendRequestPartyGameMode(gameMode);
				return;
			}
			this.SetGameModeWithoutButton(gameMode);
		}

		// Token: 0x06004D57 RID: 19799 RVA: 0x001703C8 File Offset: 0x0016E5C8
		public void SetGameModeWithoutButton(string gameMode)
		{
			this.currentGameMode.Value = gameMode;
			this.UpdateGameModeText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
		}

		// Token: 0x06004D58 RID: 19800 RVA: 0x001703E8 File Offset: 0x0016E5E8
		public void RegisterPrimaryJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.primaryTriggersByZone[trigger.networkZone] = trigger;
		}

		// Token: 0x06004D59 RID: 19801 RVA: 0x001703FC File Offset: 0x0016E5FC
		private GorillaNetworkJoinTrigger GetSelectedMapJoinTrigger()
		{
			GorillaNetworkJoinTrigger gorillaNetworkJoinTrigger;
			this.primaryTriggersByZone.TryGetValue(this.allowedMapsToJoin[Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex)], out gorillaNetworkJoinTrigger);
			return gorillaNetworkJoinTrigger;
		}

		// Token: 0x06004D5A RID: 19802 RVA: 0x00170434 File Offset: 0x0016E634
		public GorillaNetworkJoinTrigger GetJoinTriggerForZone(string zone)
		{
			GorillaNetworkJoinTrigger gorillaNetworkJoinTrigger;
			this.primaryTriggersByZone.TryGetValue(zone, out gorillaNetworkJoinTrigger);
			return gorillaNetworkJoinTrigger;
		}

		// Token: 0x06004D5B RID: 19803 RVA: 0x00170454 File Offset: 0x0016E654
		public GorillaNetworkJoinTrigger GetJoinTriggerFromFullGameModeString(string gameModeString)
		{
			foreach (KeyValuePair<string, GorillaNetworkJoinTrigger> keyValuePair in this.primaryTriggersByZone)
			{
				if (gameModeString.StartsWith(keyValuePair.Key))
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		// Token: 0x06004D5C RID: 19804 RVA: 0x001704BC File Offset: 0x0016E6BC
		public void OnGroupJoinButtonPress(int mapJoinIndex, GorillaFriendCollider chosenFriendJoinCollider)
		{
			Debug.Log("On Group button press. Map:" + mapJoinIndex.ToString() + " - collider: " + chosenFriendJoinCollider.name);
			if (mapJoinIndex >= this.allowedMapsToJoin.Length)
			{
				this.roomNotAllowed = true;
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
			if (!FriendshipGroupDetection.Instance.IsInParty)
			{
				if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
				{
					PhotonNetworkController.Instance.friendIDList = new List<string>(chosenFriendJoinCollider.playerIDsCurrentlyTouching);
					foreach (string text in this.networkController.friendIDList)
					{
						Debug.Log("Friend ID:" + text);
					}
					PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					RoomSystem.SendNearbyFollowCommand(chosenFriendJoinCollider, PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
					PhotonNetwork.SendAllOutgoingCommands();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.JoinWithNearby);
					this.currentStateIndex = 0;
					this.SwitchState(this.GetState(this.currentStateIndex), true);
				}
				return;
			}
			if (selectedMapJoinTrigger != null && selectedMapJoinTrigger.CanPartyJoin())
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.ForceJoinWithParty);
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			this.UpdateScreen();
		}

		// Token: 0x06004D5D RID: 19805 RVA: 0x001706AC File Offset: 0x0016E8AC
		public void CompQueueUnlockButtonPress()
		{
			this.allowedInCompetitive = true;
			PlayerPrefs.SetInt("allowedInCompetitive", 1);
			PlayerPrefs.Save();
		}

		// Token: 0x06004D5E RID: 19806 RVA: 0x001706C8 File Offset: 0x0016E8C8
		private void SwitchState(GorillaComputer.ComputerState newState, bool clearStack = true)
		{
			if (this.previousComputerState != this.currentComputerState)
			{
				this.previousComputerState = this.currentComputerState;
			}
			this.currentComputerState = newState;
			if (this.LoadingRoutine != null)
			{
				base.StopCoroutine(this.LoadingRoutine);
			}
			if (clearStack)
			{
				this.stateStack.Clear();
			}
			this.stateStack.Push(newState);
		}

		// Token: 0x06004D5F RID: 19807 RVA: 0x00170724 File Offset: 0x0016E924
		private void PopState()
		{
			this.currentComputerState = this.previousComputerState;
			if (this.stateStack.Count <= 1)
			{
				Debug.LogError("Can't pop into an empty stack");
				return;
			}
			this.stateStack.Pop();
			this.UpdateScreen();
		}

		// Token: 0x06004D60 RID: 19808 RVA: 0x0017075D File Offset: 0x0016E95D
		private void SwitchToWarningState()
		{
			this.warningConfirmationInputString = string.Empty;
			this.SwitchState(GorillaComputer.ComputerState.NameWarning, false);
		}

		// Token: 0x06004D61 RID: 19809 RVA: 0x00170773 File Offset: 0x0016E973
		private void SwitchToLoadingState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Loading, false);
		}

		// Token: 0x06004D62 RID: 19810 RVA: 0x0017077E File Offset: 0x0016E97E
		private void ProcessStartupState(GorillaKeyboardBindings buttonPressed)
		{
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06004D63 RID: 19811 RVA: 0x00170794 File Offset: 0x0016E994
		private void ProcessColorState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.enter:
				return;
			case GorillaKeyboardBindings.option1:
				this.colorCursorLine = 0;
				return;
			case GorillaKeyboardBindings.option2:
				this.colorCursorLine = 1;
				return;
			case GorillaKeyboardBindings.option3:
				this.colorCursorLine = 2;
				return;
			default:
			{
				int num = (int)buttonPressed;
				if (num < 10)
				{
					switch (this.colorCursorLine)
					{
					case 0:
						this.redText = num.ToString();
						this.redValue = (float)num / 9f;
						PlayerPrefs.SetFloat("redValue", this.redValue);
						break;
					case 1:
						this.greenText = num.ToString();
						this.greenValue = (float)num / 9f;
						PlayerPrefs.SetFloat("greenValue", this.greenValue);
						break;
					case 2:
						this.blueText = num.ToString();
						this.blueValue = (float)num / 9f;
						PlayerPrefs.SetFloat("blueValue", this.blueValue);
						break;
					}
					GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
					PlayerPrefs.Save();
					if (NetworkSystem.Instance.InRoom)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { this.redValue, this.greenValue, this.blueValue });
					}
				}
				return;
			}
			}
		}

		// Token: 0x06004D64 RID: 19812 RVA: 0x001708F4 File Offset: 0x0016EAF4
		public void ProcessNameState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (this.currentName.Length > 0 && this.NametagsEnabled)
					{
						this.currentName = this.currentName.Substring(0, this.currentName.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (this.currentName != this.savedName && this.currentName != "" && this.NametagsEnabled)
					{
						this.CheckAutoBanListForPlayerName(this.currentName);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.UpdateNametagSetting(!this.NametagsEnabled, true);
					return;
				default:
					if (this.NametagsEnabled && this.currentName.Length < 12 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
					{
						string text = this.currentName;
						string text2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							text2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							text2 = num.ToString();
						}
						this.currentName = text + text2;
						return;
					}
					break;
				}
			}
			else if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				if (buttonPressed != GorillaKeyboardBindings.option3)
				{
					return;
				}
				if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
				{
					return;
				}
				this.ProcessScreen_SetupKID();
			}
			else
			{
				if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
				{
					this.ProcessScreen_SetupKID();
					return;
				}
				this.RequestUpdatedPermissions();
				return;
			}
		}

		// Token: 0x06004D65 RID: 19813 RVA: 0x00170A38 File Offset: 0x0016EC38
		private void ProcessRoomState(GorillaKeyboardBindings buttonPressed)
		{
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.delete:
				if (flag && ((this.playerInVirtualStump && this.roomToJoin.Length > 1) || (!this.playerInVirtualStump && this.roomToJoin.Length > 0)))
				{
					this.roomToJoin = this.roomToJoin.Substring(0, this.roomToJoin.Length - 1);
					return;
				}
				break;
			case GorillaKeyboardBindings.enter:
				if (flag && ((!this.playerInVirtualStump && this.roomToJoin != "") || (this.playerInVirtualStump && this.roomToJoin.Length > 1)))
				{
					this.CheckAutoBanListForRoomName(this.roomToJoin);
					return;
				}
				break;
			case GorillaKeyboardBindings.option1:
				if (FriendshipGroupDetection.Instance.IsInParty)
				{
					FriendshipGroupDetection.Instance.LeaveParty();
					this.DisconnectAfterDelay(1f);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
				return;
			case GorillaKeyboardBindings.option2:
				if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
				{
					this.ProcessScreen_SetupKID();
					return;
				}
				this.RequestUpdatedPermissions();
				return;
			case GorillaKeyboardBindings.option3:
				if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
				{
					return;
				}
				this.ProcessScreen_SetupKID();
				return;
			default:
				if (flag && this.roomToJoin.Length < 10)
				{
					string text = this.roomToJoin;
					string text2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						text2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						text2 = num.ToString();
					}
					this.roomToJoin = text + text2;
				}
				break;
			}
		}

		// Token: 0x06004D66 RID: 19814 RVA: 0x00170BA0 File Offset: 0x0016EDA0
		private async void DisconnectAfterDelay(float seconds)
		{
			await Task.Delay((int)(1000f * seconds));
			await NetworkSystem.Instance.ReturnToSinglePlayer();
		}

		// Token: 0x06004D67 RID: 19815 RVA: 0x00170BD8 File Offset: 0x0016EDD8
		private void ProcessTurnState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				GorillaSnapTurn.UpdateAndSaveTurnFactor((int)buttonPressed);
				return;
			}
			string text = string.Empty;
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				text = "SNAP";
				break;
			case GorillaKeyboardBindings.option2:
				text = "SMOOTH";
				break;
			case GorillaKeyboardBindings.option3:
				text = "NONE";
				break;
			}
			if (text.Length > 0)
			{
				GorillaSnapTurn.UpdateAndSaveTurnType(text);
			}
		}

		// Token: 0x06004D68 RID: 19816 RVA: 0x00170C38 File Offset: 0x0016EE38
		private void ProcessMicState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.pttType = "ALL CHAT";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option2:
				this.pttType = "PUSH TO TALK";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option3:
				this.pttType = "PUSH TO MUTE";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			default:
				return;
			}
		}

		// Token: 0x06004D69 RID: 19817 RVA: 0x00170CC0 File Offset: 0x0016EEC0
		private void ProcessQueueState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.JoinQueue("DEFAULT", false);
				return;
			case GorillaKeyboardBindings.option2:
				this.JoinQueue("MINIGAMES", false);
				return;
			case GorillaKeyboardBindings.option3:
				if (this.allowedInCompetitive)
				{
					this.JoinQueue("COMPETITIVE", false);
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x06004D6A RID: 19818 RVA: 0x00170D14 File Offset: 0x0016EF14
		public void JoinTroop(string newTroopName)
		{
			if (this.IsValidTroopName(newTroopName))
			{
				this.currentTroopPopulation = -1;
				this.troopName = newTroopName;
				PlayerPrefs.SetString("troopName", this.troopName);
				if (this.troopQueueActive)
				{
					this.currentQueue = this.GetQueueNameForTroop(this.troopName);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
				}
				PlayerPrefs.Save();
				this.JoinTroopQueue();
			}
		}

		// Token: 0x06004D6B RID: 19819 RVA: 0x00170D7D File Offset: 0x0016EF7D
		public void JoinTroopQueue()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.currentTroopPopulation = -1;
				this.JoinQueue(this.GetQueueNameForTroop(this.troopName), true);
				this.RequestTroopPopulation(true);
			}
		}

		// Token: 0x06004D6C RID: 19820 RVA: 0x00170DB0 File Offset: 0x0016EFB0
		private void RequestTroopPopulation(bool forceUpdate = false)
		{
			if (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				return;
			}
			if (!this.hasRequestedInitialTroopPopulation || forceUpdate)
			{
				if (this.nextPopulationCheckTime > Time.time)
				{
					return;
				}
				this.nextPopulationCheckTime = Time.time + this.troopPopulationCheckCooldown;
				this.hasRequestedInitialTroopPopulation = true;
				GorillaServer.Instance.ReturnQueueStats(new ReturnQueueStatsRequest
				{
					queueName = this.troopName
				}, delegate(ExecuteFunctionResult result)
				{
					Debug.Log("Troop pop received");
					object obj;
					if (((JsonObject)result.FunctionResult).TryGetValue("PlayerCount", out obj))
					{
						this.currentTroopPopulation = int.Parse(obj.ToString());
						if (this.currentComputerState == GorillaComputer.ComputerState.Queue)
						{
							this.UpdateScreen();
							return;
						}
					}
					else
					{
						this.currentTroopPopulation = 0;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogError(string.Format("Error requesting troop population: {0}", error));
					this.currentTroopPopulation = -1;
				});
			}
		}

		// Token: 0x06004D6D RID: 19821 RVA: 0x00170E2E File Offset: 0x0016F02E
		public void JoinDefaultQueue()
		{
			this.JoinQueue("DEFAULT", false);
		}

		// Token: 0x06004D6E RID: 19822 RVA: 0x00170E3C File Offset: 0x0016F03C
		public void LeaveTroop()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.troopToJoin = this.troopName;
			}
			this.currentTroopPopulation = -1;
			this.troopName = string.Empty;
			PlayerPrefs.SetString("troopName", this.troopName);
			if (this.troopQueueActive)
			{
				this.JoinDefaultQueue();
			}
			PlayerPrefs.Save();
		}

		// Token: 0x06004D6F RID: 19823 RVA: 0x00170E98 File Offset: 0x0016F098
		public string GetCurrentTroop()
		{
			if (this.troopQueueActive)
			{
				return this.troopName;
			}
			return this.currentQueue;
		}

		// Token: 0x06004D70 RID: 19824 RVA: 0x00170EAF File Offset: 0x0016F0AF
		public int GetCurrentTroopPopulation()
		{
			if (this.troopQueueActive)
			{
				return this.currentTroopPopulation;
			}
			return -1;
		}

		// Token: 0x06004D71 RID: 19825 RVA: 0x00170EC4 File Offset: 0x0016F0C4
		private void JoinQueue(string queueName, bool isTroopQueue = false)
		{
			this.currentQueue = queueName;
			this.troopQueueActive = isTroopQueue;
			this.currentTroopPopulation = -1;
			PlayerPrefs.SetString("currentQueue", this.currentQueue);
			PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
			PlayerPrefs.Save();
		}

		// Token: 0x06004D72 RID: 19826 RVA: 0x00170F14 File Offset: 0x0016F114
		private void ProcessGroupState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.one:
				this.groupMapJoin = "FOREST";
				this.groupMapJoinIndex = 0;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.two:
				this.groupMapJoin = "CAVE";
				this.groupMapJoinIndex = 1;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.three:
				this.groupMapJoin = "CANYON";
				this.groupMapJoinIndex = 2;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.four:
				this.groupMapJoin = "CITY";
				this.groupMapJoinIndex = 3;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.five:
				this.groupMapJoin = "CLOUDS";
				this.groupMapJoinIndex = 4;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			default:
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					this.OnGroupJoinButtonPress(Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex), this.friendJoinCollider);
				}
				break;
			}
			this.roomFull = false;
		}

		// Token: 0x06004D73 RID: 19827 RVA: 0x00171098 File Offset: 0x0016F298
		private void ProcessTroopState(GorillaKeyboardBindings buttonPressed)
		{
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
			bool flag2 = this.IsValidTroopName(this.troopName);
			if (flag)
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (!flag2 && this.troopToJoin.Length > 0)
					{
						this.troopToJoin = this.troopToJoin.Substring(0, this.troopToJoin.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (!flag2)
					{
						this.CheckAutoBanListForTroopName(this.troopToJoin);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.JoinTroopQueue();
					return;
				case GorillaKeyboardBindings.option2:
					this.JoinDefaultQueue();
					return;
				case GorillaKeyboardBindings.option3:
					this.LeaveTroop();
					return;
				default:
					if (!flag2 && this.troopToJoin.Length < 12)
					{
						string text = this.troopToJoin;
						string text2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							text2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							text2 = num.ToString();
						}
						this.troopToJoin = text + text2;
						return;
					}
					break;
				}
			}
			else
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.option1:
					break;
				case GorillaKeyboardBindings.option2:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
					{
						this.ProcessScreen_SetupKID();
						return;
					}
					this.RequestUpdatedPermissions();
					return;
				case GorillaKeyboardBindings.option3:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06004D74 RID: 19828 RVA: 0x001711B8 File Offset: 0x0016F3B8
		private bool IsValidTroopName(string troop)
		{
			return !string.IsNullOrEmpty(troop) && troop.Length <= 12 && (this.allowedInCompetitive || troop != "COMPETITIVE");
		}

		// Token: 0x06004D75 RID: 19829 RVA: 0x0006D17B File Offset: 0x0006B37B
		private string GetQueueNameForTroop(string troop)
		{
			return troop;
		}

		// Token: 0x06004D76 RID: 19830 RVA: 0x001711E4 File Offset: 0x0016F3E4
		private void ProcessVoiceState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				if (buttonPressed != GorillaKeyboardBindings.option1)
				{
					if (buttonPressed == GorillaKeyboardBindings.option2)
					{
						this.SetVoice(false, true);
					}
				}
				else
				{
					this.SetVoice(true, true);
				}
			}
			else if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				if (buttonPressed == GorillaKeyboardBindings.option3)
				{
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
				}
			}
			else if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
			{
				this.ProcessScreen_SetupKID();
			}
			else
			{
				this.RequestUpdatedPermissions();
			}
			RigContainer.RefreshAllRigVoices();
		}

		// Token: 0x06004D77 RID: 19831 RVA: 0x00171254 File Offset: 0x0016F454
		private void ProcessAutoMuteState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.autoMuteType = "AGGRESSIVE";
				PlayerPrefs.SetInt("autoMute", 2);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option2:
				this.autoMuteType = "MODERATE";
				PlayerPrefs.SetInt("autoMute", 1);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option3:
				this.autoMuteType = "OFF";
				PlayerPrefs.SetInt("autoMute", 0);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			}
			this.UpdateScreen();
		}

		// Token: 0x06004D78 RID: 19832 RVA: 0x001712E4 File Offset: 0x0016F4E4
		private void ProcessVisualsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				this.instrumentVolume = (float)buttonPressed / 50f;
				PlayerPrefs.SetFloat("instrumentVolume", this.instrumentVolume);
				PlayerPrefs.Save();
				return;
			}
			if (buttonPressed == GorillaKeyboardBindings.option1)
			{
				this.disableParticles = false;
				PlayerPrefs.SetString("disableParticles", "FALSE");
				PlayerPrefs.Save();
				GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
				return;
			}
			if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				return;
			}
			this.disableParticles = true;
			PlayerPrefs.SetString("disableParticles", "TRUE");
			PlayerPrefs.Save();
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
		}

		// Token: 0x06004D79 RID: 19833 RVA: 0x00171384 File Offset: 0x0016F584
		private void ProcessCreditsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.creditsView.ProcessButtonPress(buttonPressed);
			}
		}

		// Token: 0x06004D7A RID: 19834 RVA: 0x00171397 File Offset: 0x0016F597
		private void ProcessSupportState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.displaySupport = true;
			}
		}

		// Token: 0x06004D7B RID: 19835 RVA: 0x001713A8 File Offset: 0x0016F5A8
		private void ProcessRedemptionState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.RedemptionStatus == GorillaComputer.RedemptionResult.Checking)
			{
				return;
			}
			if (buttonPressed != GorillaKeyboardBindings.delete)
			{
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					if (this.redemptionCode != "")
					{
						if (this.redemptionCode.Length < 8)
						{
							this.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
							return;
						}
						CodeRedemption.Instance.HandleCodeRedemption(this.redemptionCode);
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Checking;
						return;
					}
					else if (this.RedemptionStatus != GorillaComputer.RedemptionResult.Success)
					{
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
						return;
					}
				}
				else if (this.redemptionCode.Length < 8 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
				{
					string text = this.redemptionCode;
					string text2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						text2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						text2 = num.ToString();
					}
					this.redemptionCode = text + text2;
				}
			}
			else if (this.redemptionCode.Length > 0)
			{
				this.redemptionCode = this.redemptionCode.Substring(0, this.redemptionCode.Length - 1);
				return;
			}
		}

		// Token: 0x06004D7C RID: 19836 RVA: 0x00171494 File Offset: 0x0016F694
		private void ProcessNameWarningState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				this.PopState();
				return;
			}
			if (buttonPressed == GorillaKeyboardBindings.delete)
			{
				if (this.warningConfirmationInputString.Length > 0)
				{
					this.warningConfirmationInputString = this.warningConfirmationInputString.Substring(0, this.warningConfirmationInputString.Length - 1);
					return;
				}
			}
			else if (this.warningConfirmationInputString.Length < 3)
			{
				this.warningConfirmationInputString += buttonPressed.ToString();
			}
		}

		// Token: 0x06004D7D RID: 19837 RVA: 0x00171520 File Offset: 0x0016F720
		public void UpdateScreen()
		{
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.WrongVersion)
			{
				this.UpdateFunctionScreen();
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Startup:
					this.StartupScreen();
					break;
				case GorillaComputer.ComputerState.Color:
					this.ColourScreen();
					break;
				case GorillaComputer.ComputerState.Name:
					this.NameScreen();
					break;
				case GorillaComputer.ComputerState.Turn:
					this.TurnScreen();
					break;
				case GorillaComputer.ComputerState.Mic:
					this.MicScreen();
					break;
				case GorillaComputer.ComputerState.Room:
					this.RoomScreen();
					break;
				case GorillaComputer.ComputerState.Queue:
					this.QueueScreen();
					break;
				case GorillaComputer.ComputerState.Group:
					this.GroupScreen();
					break;
				case GorillaComputer.ComputerState.Voice:
					this.VoiceScreen();
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.AutomuteScreen();
					break;
				case GorillaComputer.ComputerState.Credits:
					this.CreditsScreen();
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.VisualsScreen();
					break;
				case GorillaComputer.ComputerState.Time:
					this.TimeScreen();
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.NameWarningScreen();
					break;
				case GorillaComputer.ComputerState.Loading:
					this.LoadingScreen();
					break;
				case GorillaComputer.ComputerState.Support:
					this.SupportScreen();
					break;
				case GorillaComputer.ComputerState.Troop:
					this.TroopScreen();
					break;
				case GorillaComputer.ComputerState.KID:
					this.KIdScreen();
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.RedemptionScreen();
					break;
				}
			}
			this.UpdateGameModeText();
		}

		// Token: 0x06004D7E RID: 19838 RVA: 0x00171652 File Offset: 0x0016F852
		private void LoadingScreen()
		{
			this.screenText.Text = "LOADING";
			this.LoadingRoutine = base.StartCoroutine(this.<LoadingScreen>g__LoadingScreenLocal|193_0());
		}

		// Token: 0x06004D7F RID: 19839 RVA: 0x00171678 File Offset: 0x0016F878
		private void NameWarningScreen()
		{
			this.screenText.Text = "<color=red>WARNING: PLEASE CHOOSE A BETTER NAME\n\nENTERING ANOTHER BAD NAME WILL RESULT IN A BAN</color>";
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += "\n\nPRESS ANY KEY TO CONTINUE";
				return;
			}
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text = gorillaText2.Text + "\n\nTYPE 'YES' TO CONFIRM: " + this.warningConfirmationInputString;
		}

		// Token: 0x06004D80 RID: 19840 RVA: 0x001716EC File Offset: 0x0016F8EC
		private void SupportScreen()
		{
			if (this.displaySupport)
			{
				string text = PlayFabAuthenticator.instance.platform.ToString().ToUpper();
				string text2;
				if (text == "PC")
				{
					text2 = "OCULUS PC";
				}
				else
				{
					text2 = text;
				}
				text = text2;
				this.screenText.Text = string.Concat(new string[]
				{
					"SUPPORT\n\nPLAYERID   ",
					PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
					"\nVERSION    ",
					this.version.ToUpper(),
					"\nPLATFORM   ",
					text,
					"\nBUILD DATE ",
					this.buildDate,
					"\n"
				});
				return;
			}
			this.screenText.Text = "SUPPORT\n\n";
			GorillaText gorillaText = this.screenText;
			gorillaText.Text += "PRESS ENTER TO DISPLAY SUPPORT AND ACCOUNT INFORMATION\n\n\n\n";
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += "<color=red>DO NOT SHARE ACCOUNT INFORMATION WITH ANYONE OTHER ";
			GorillaText gorillaText3 = this.screenText;
			gorillaText3.Text += "THAN ANOTHER AXIOM SUPPORT</color>";
		}

		// Token: 0x06004D81 RID: 19841 RVA: 0x00171800 File Offset: 0x0016FA00
		private void TimeScreen()
		{
			this.screenText.Text = string.Concat(new string[]
			{
				"UPDATE TIME SETTINGS. (LOCALLY ONLY). \nPRESS OPTION 1 FOR NORMAL MODE. \nPRESS OPTION 2 FOR STATIC MODE. \nPRESS 1-10 TO CHANGE TIME OF DAY. \nCURRENT MODE: ",
				BetterDayNightManager.instance.currentSetting.ToString().ToUpper(),
				". \nTIME OF DAY: ",
				BetterDayNightManager.instance.currentTimeOfDay.ToUpper(),
				". \n"
			});
		}

		// Token: 0x06004D82 RID: 19842 RVA: 0x0017186E File Offset: 0x0016FA6E
		private void CreditsScreen()
		{
			this.screenText.Text = this.creditsView.GetScreenText();
		}

		// Token: 0x06004D83 RID: 19843 RVA: 0x00171888 File Offset: 0x0016FA88
		private void VisualsScreen()
		{
			this.screenText.Text = "UPDATE ITEMS SETTINGS. PRESS OPTION 1 TO ENABLE ITEM PARTICLES. PRESS OPTION 2 TO DISABLE ITEM PARTICLES. PRESS 1-10 TO CHANGE INSTRUMENT VOLUME FOR OTHER PLAYERS.\n\nITEM PARTICLES ON: " + (this.disableParticles ? "FALSE" : "TRUE") + "\nINSTRUMENT VOLUME: " + Mathf.CeilToInt(this.instrumentVolume * 50f).ToString();
		}

		// Token: 0x06004D84 RID: 19844 RVA: 0x001718DC File Offset: 0x0016FADC
		private void VoiceScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				this.screenText.Text = "CHOOSE WHICH TYPE OF VOICE YOU WANT TO HEAR AND SPEAK. \nPRESS OPTION 1 = HUMAN VOICES. \nPRESS OPTION 2 = MONKE VOICES. \n\nVOICE TYPE: " + ((this.voiceChatOn == "TRUE") ? "HUMAN" : ((this.voiceChatOn == "FALSE") ? "MONKE" : "OFF"));
				return;
			}
			if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.VoiceScreen_KIdProhibited();
				return;
			}
			this.VoiceScreen_Permission();
		}

		// Token: 0x06004D85 RID: 19845 RVA: 0x0017195B File Offset: 0x0016FB5B
		private void AutomuteScreen()
		{
			this.screenText.Text = "AUTOMOD AUTOMATICALLY MUTES PLAYERS WHEN THEY JOIN YOUR ROOM IF A LOT OF OTHER PLAYERS HAVE MUTED THEM\nPRESS OPTION 1 FOR AGGRESSIVE MUTING\nPRESS OPTION 2 FOR MODERATE MUTING\nPRESS OPTION 3 TO TURN AUTOMOD OFF\n\nCURRENT AUTOMOD LEVEL: " + this.autoMuteType;
		}

		// Token: 0x06004D86 RID: 19846 RVA: 0x00171978 File Offset: 0x0016FB78
		private void GroupScreen()
		{
			string text = ((this.allowedMapsToJoin.Length > 1) ? this.groupMapJoin : this.allowedMapsToJoin[0].ToUpper());
			string text2 = ((this.allowedMapsToJoin.Length > 1) ? "\n\nUSE NUMBER KEYS TO SELECT DESTINATION\n1: FOREST, 2: CAVE, 3: CANYON, 4: CITY, 5: CLOUDS." : "");
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				string text3 = (this.GetSelectedMapJoinTrigger().CanPartyJoin() ? "" : "\n\n<color=red>CANNOT JOIN BECAUSE YOUR GROUP IS NOT HERE</color>");
				this.screenText.Text = "PRESS ENTER TO JOIN A PUBLIC GAME WITH YOUR FRIENDSHIP GROUP.\n\nACTIVE ZONE WILL BE: " + text + text2 + text3;
				return;
			}
			this.screenText.Text = "PRESS ENTER TO JOIN A PUBLIC GAME AND BRING EVERYONE IN THIS ROOM WITH YOU.\n\nACTIVE ZONE WILL BE: " + text + text2;
		}

		// Token: 0x06004D87 RID: 19847 RVA: 0x00171A14 File Offset: 0x0016FC14
		private void MicScreen()
		{
			if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.MicScreen_KIdProhibited();
				return;
			}
			this.screenText.Text = "PRESS OPTION 1 = ALL CHAT.\nPRESS OPTION 2 = PUSH TO TALK.\nPRESS OPTION 3 = PUSH TO MUTE.\n\nCURRENT MIC SETTING: " + this.pttType + "\n\nPUSH TO TALK AND PUSH TO MUTE WORK WITH ANY FACE BUTTON";
		}

		// Token: 0x06004D88 RID: 19848 RVA: 0x00171A4C File Offset: 0x0016FC4C
		private void QueueScreen()
		{
			if (this.allowedInCompetitive)
			{
				this.screenText.Text = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES. COMPETITIVE IS FOR PLAYERS WHO WANT TO PLAY THE GAME AND TRY AS HARD AS THEY CAN. PRESS OPTION 1 FOR DEFAULT, OPTION 2 FOR MINIGAMES, OR OPTION 3 FOR COMPETITIVE.";
			}
			else
			{
				this.screenText.Text = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES. BEAT THE OBSTACLE COURSE IN CITY TO ALLOW COMPETITIVE PLAY. PRESS OPTION 1 FOR DEFAULT, OR OPTION 2 FOR MINIGAMES.";
			}
			GorillaText gorillaText = this.screenText;
			gorillaText.Text = gorillaText.Text + "\n\nCURRENT QUEUE: " + this.currentQueue;
		}

		// Token: 0x06004D89 RID: 19849 RVA: 0x00171AA4 File Offset: 0x0016FCA4
		private void TroopScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			bool flag2 = this.IsValidTroopName(this.troopName);
			this.screenText.Text = string.Empty;
			if (flag)
			{
				this.screenText.Text = "PLAY WITH A PERSISTENT GROUP ACROSS MULTIPLE ROOMS.";
				if (!flag2)
				{
					GorillaText gorillaText = this.screenText;
					gorillaText.Text += " PRESS ENTER TO JOIN OR CREATE A TROOP.";
				}
			}
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += "\n\nCURRENT TROOP: ";
			if (flag2)
			{
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text = gorillaText3.Text + this.troopName + "\n";
				if (flag)
				{
					bool flag3 = this.currentTroopPopulation > -1;
					if (this.troopQueueActive)
					{
						GorillaText gorillaText4 = this.screenText;
						gorillaText4.Text += "  -IN TROOP QUEUE-\n";
						if (flag3)
						{
							GorillaText gorillaText5 = this.screenText;
							gorillaText5.Text += string.Format("\nPLAYERS IN TROOP: {0}\n", Mathf.Max(1, this.currentTroopPopulation));
						}
						GorillaText gorillaText6 = this.screenText;
						gorillaText6.Text += "\nPRESS OPTION 2 FOR DEFAULT QUEUE.\n";
					}
					else
					{
						GorillaText gorillaText7 = this.screenText;
						gorillaText7.Text = gorillaText7.Text + "  -IN " + this.currentQueue + " QUEUE-\n";
						if (flag3)
						{
							GorillaText gorillaText8 = this.screenText;
							gorillaText8.Text += string.Format("\nPLAYERS IN TROOP: {0}\n", Mathf.Max(1, this.currentTroopPopulation));
						}
						GorillaText gorillaText9 = this.screenText;
						gorillaText9.Text += "\nPRESS OPTION 1 FOR TROOP QUEUE.\n";
					}
					GorillaText gorillaText10 = this.screenText;
					gorillaText10.Text += "PRESS OPTION 3 TO LEAVE YOUR TROOP.";
				}
			}
			else
			{
				GorillaText gorillaText11 = this.screenText;
				gorillaText11.Text += "-NOT IN TROOP-";
			}
			if (flag)
			{
				if (!flag2)
				{
					GorillaText gorillaText12 = this.screenText;
					gorillaText12.Text = gorillaText12.Text + "\n\nTROOP TO JOIN: " + this.troopToJoin;
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
				{
					this.TroopScreen_KIdProhibited();
					return;
				}
				this.TroopScreen_Permission();
			}
		}

		// Token: 0x06004D8A RID: 19850 RVA: 0x00171CDC File Offset: 0x0016FEDC
		private void TurnScreen()
		{
			this.screenText.Text = "PRESS OPTION 1 TO USE SNAP TURN. PRESS OPTION 2 TO USE SMOOTH TURN. PRESS OPTION 3 TO USE NO ARTIFICIAL TURNING. PRESS THE NUMBER KEYS TO CHOOSE A TURNING SPEED.\n CURRENT TURN TYPE: " + GorillaSnapTurn.CachedSnapTurnRef.turnType + "\nCURRENT TURN SPEED: " + GorillaSnapTurn.CachedSnapTurnRef.turnFactor.ToString();
		}

		// Token: 0x06004D8B RID: 19851 RVA: 0x00171D20 File Offset: 0x0016FF20
		private void NameScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				this.screenText.Text = "PRESS ENTER TO CHANGE YOUR NAME TO THE ENTERED NEW NAME.\n\nCURRENT NAME: " + this.savedName;
				if (this.NametagsEnabled)
				{
					GorillaText gorillaText = this.screenText;
					gorillaText.Text = gorillaText.Text + "\n\n    NEW NAME: " + this.currentName;
				}
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text = gorillaText2.Text + "\n\nPRESS OPTION 1 TO TOGGLE NAMETAGS.\nCURRENTLY NAMETAGS ARE: " + (this.NametagsEnabled ? "ON" : "OFF");
				return;
			}
			if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.NameScreen_KIdProhibited();
				return;
			}
			this.NameScreen_Permission();
		}

		// Token: 0x06004D8C RID: 19852 RVA: 0x00171DC8 File Offset: 0x0016FFC8
		private void StartupScreen()
		{
			string text = string.Empty;
			if (KIDManager.GetActiveAccountStatus() == AgeStatusType.DIGITALMINOR)
			{
				text = "YOU ARE PLAYING ON A MANAGED ACCOUNT. SOME SETTINGS MAY BE DISABLED WITHOUT PARENT OR GUARDIAN APPROVAL\n\n";
			}
			string empty = string.Empty;
			this.screenText.Text = string.Concat(new string[]
			{
				"GORILLA OS\n\n",
				text,
				NetworkSystem.Instance.GlobalPlayerCount().ToString(),
				" PLAYERS ONLINE\n\n",
				this.usersBanned.ToString(),
				" USERS BANNED YESTERDAY\n\n",
				empty,
				"PRESS ANY KEY TO BEGIN"
			});
		}

		// Token: 0x06004D8D RID: 19853 RVA: 0x00171E50 File Offset: 0x00170050
		private void ColourScreen()
		{
			this.screenText.Text = "USE THE OPTIONS BUTTONS TO SELECT THE COLOR TO UPDATE, THEN PRESS 0-9 TO SET A NEW VALUE.";
			GorillaText gorillaText = this.screenText;
			gorillaText.Text = gorillaText.Text + "\n\n  RED: " + Mathf.FloorToInt(this.redValue * 9f).ToString() + ((this.colorCursorLine == 0) ? "<--" : "");
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text = gorillaText2.Text + "\n\nGREEN: " + Mathf.FloorToInt(this.greenValue * 9f).ToString() + ((this.colorCursorLine == 1) ? "<--" : "");
			GorillaText gorillaText3 = this.screenText;
			gorillaText3.Text = gorillaText3.Text + "\n\n BLUE: " + Mathf.FloorToInt(this.blueValue * 9f).ToString() + ((this.colorCursorLine == 2) ? "<--" : "");
		}

		// Token: 0x06004D8E RID: 19854 RVA: 0x00171F48 File Offset: 0x00170148
		private void RoomScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			object obj = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			this.screenText.Text = "";
			object obj2 = obj;
			if (obj2 != null)
			{
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += "PRESS ENTER TO JOIN OR CREATE A CUSTOM ROOM WITH THE ENTERED CODE. ";
			}
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += "PRESS OPTION 1 TO DISCONNECT FROM THE CURRENT ROOM. ";
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
				{
					GorillaText gorillaText3 = this.screenText;
					gorillaText3.Text += "YOUR GROUP WILL TRAVEL WITH YOU. ";
				}
				else
				{
					GorillaText gorillaText4 = this.screenText;
					gorillaText4.Text += "<color=red>YOU WILL LEAVE YOUR PARTY UNLESS YOU GATHER THEM HERE FIRST!</color> ";
				}
			}
			GorillaText gorillaText5 = this.screenText;
			gorillaText5.Text += "\n\nCURRENT ROOM: ";
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaText gorillaText6 = this.screenText;
				gorillaText6.Text += NetworkSystem.Instance.RoomName;
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
					string text = ((activeGameMode != null) ? activeGameMode.GameModeName() : null);
					if (text != null && text != this.currentGameMode.Value)
					{
						GorillaText gorillaText7 = this.screenText;
						gorillaText7.Text = gorillaText7.Text + " (" + text + " GAME)";
					}
				}
				GorillaText gorillaText8 = this.screenText;
				gorillaText8.Text = gorillaText8.Text + "\n\nPLAYERS IN ROOM: " + NetworkSystem.Instance.RoomPlayerCount.ToString();
			}
			else
			{
				GorillaText gorillaText9 = this.screenText;
				gorillaText9.Text += "-NOT IN ROOM-";
				GorillaText gorillaText10 = this.screenText;
				gorillaText10.Text = gorillaText10.Text + "\n\nPLAYERS ONLINE: " + NetworkSystem.Instance.GlobalPlayerCount().ToString();
			}
			if (obj2 != null)
			{
				GorillaText gorillaText11 = this.screenText;
				gorillaText11.Text = gorillaText11.Text + "\n\nROOM TO JOIN: " + this.roomToJoin;
				if (this.roomFull)
				{
					GorillaText gorillaText12 = this.screenText;
					gorillaText12.Text += "\n\nROOM FULL. JOIN ROOM FAILED.";
					return;
				}
				if (this.roomNotAllowed)
				{
					GorillaText gorillaText13 = this.screenText;
					gorillaText13.Text += "\n\nCANNOT JOIN ROOM TYPE FROM HERE.";
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
				{
					this.RoomScreen_KIdProhibited();
					return;
				}
				this.RoomScreen_Permission();
			}
		}

		// Token: 0x06004D8F RID: 19855 RVA: 0x001721B4 File Offset: 0x001703B4
		private void RedemptionScreen()
		{
			this.screenText.Text = "TYPE REDEMPTION CODE AND PRESS ENTER";
			GorillaText gorillaText = this.screenText;
			gorillaText.Text = gorillaText.Text + "\n\nCODE: " + this.redemptionCode;
			switch (this.RedemptionStatus)
			{
			case GorillaComputer.RedemptionResult.Empty:
				break;
			case GorillaComputer.RedemptionResult.Invalid:
			{
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text += "\n\nINVALID CODE";
				return;
			}
			case GorillaComputer.RedemptionResult.Checking:
			{
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += "\n\nVALIDATING...";
				return;
			}
			case GorillaComputer.RedemptionResult.AlreadyUsed:
			{
				GorillaText gorillaText4 = this.screenText;
				gorillaText4.Text += "\n\nCODE ALREADY CLAIMED";
				return;
			}
			case GorillaComputer.RedemptionResult.Success:
			{
				GorillaText gorillaText5 = this.screenText;
				gorillaText5.Text += "\n\nSUCCESSFULLY CLAIMED!";
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06004D90 RID: 19856 RVA: 0x00172284 File Offset: 0x00170484
		private void UpdateGameModeText()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (GorillaGameManager.instance != null)
				{
					this.currentGameModeText.Value = "CURRENT MODE\n" + GorillaGameManager.instance.GameModeName();
					return;
				}
				this.currentGameModeText.Value = "CURRENT MODE\n-NOT IN ROOM-";
			}
		}

		// Token: 0x06004D91 RID: 19857 RVA: 0x001722DA File Offset: 0x001704DA
		private void UpdateFunctionScreen()
		{
			this.functionSelectText.Text = this.GetOrderListForScreen(this.currentState);
		}

		// Token: 0x06004D92 RID: 19858 RVA: 0x001722F3 File Offset: 0x001704F3
		private void CheckAutoBanListForRoomName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.AutoBanPlayfabFunction(nameToCheck, true, new Action<ExecuteFunctionResult>(this.OnRoomNameChecked));
		}

		// Token: 0x06004D93 RID: 19859 RVA: 0x0017230F File Offset: 0x0017050F
		private void CheckAutoBanListForPlayerName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.AutoBanPlayfabFunction(nameToCheck, false, new Action<ExecuteFunctionResult>(this.OnPlayerNameChecked));
		}

		// Token: 0x06004D94 RID: 19860 RVA: 0x0017232B File Offset: 0x0017052B
		private void CheckAutoBanListForTroopName(string nameToCheck)
		{
			if (this.IsValidTroopName(this.troopToJoin))
			{
				this.SwitchToLoadingState();
				this.AutoBanPlayfabFunction(nameToCheck, false, new Action<ExecuteFunctionResult>(this.OnTroopNameChecked));
			}
		}

		// Token: 0x06004D95 RID: 19861 RVA: 0x00172355 File Offset: 0x00170555
		private void AutoBanPlayfabFunction(string nameToCheck, bool forRoom, Action<ExecuteFunctionResult> resultCallback)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = forRoom
			}, resultCallback, new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x06004D96 RID: 19862 RVA: 0x00172384 File Offset: 0x00170584
		private void OnRoomNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					if (FriendshipGroupDetection.Instance.IsInParty && !FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
					{
						FriendshipGroupDetection.Instance.LeaveParty();
					}
					if (this.playerInVirtualStump)
					{
						CustomMapManager.UnloadMod(false);
					}
					this.networkController.AttemptToJoinSpecificRoom(this.roomToJoin, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
					break;
				case 1:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "");
					this.SwitchToWarningState();
					break;
				case 2:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "");
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x06004D97 RID: 19863 RVA: 0x001724AC File Offset: 0x001706AC
		private void OnPlayerNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					NetworkSystem.Instance.SetMyNickName(this.currentName);
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					break;
				case 1:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					this.SwitchToWarningState();
					break;
				case 2:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			this.SetLocalNameTagText(this.currentName);
			this.savedName = this.currentName;
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { this.redValue, this.greenValue, this.blueValue });
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x06004D98 RID: 19864 RVA: 0x001725E8 File Offset: 0x001707E8
		private void OnTroopNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					this.JoinTroop(this.troopToJoin);
					break;
				case 1:
					this.troopToJoin = string.Empty;
					this.SwitchToWarningState();
					break;
				case 2:
					this.troopToJoin = string.Empty;
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x06004D99 RID: 19865 RVA: 0x0017266F File Offset: 0x0017086F
		private void OnErrorNameCheck(PlayFabError error)
		{
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
			GorillaComputer.OnErrorShared(error);
		}

		// Token: 0x06004D9A RID: 19866 RVA: 0x00172688 File Offset: 0x00170888
		public bool CheckAutoBanListForName(string nameToCheck)
		{
			nameToCheck = nameToCheck.ToLower();
			nameToCheck = new string(Array.FindAll<char>(nameToCheck.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
			foreach (string text in this.anywhereTwoWeek)
			{
				if (nameToCheck.IndexOf(text) >= 0)
				{
					return false;
				}
			}
			foreach (string text2 in this.anywhereOneWeek)
			{
				if (nameToCheck.IndexOf(text2) >= 0 && !nameToCheck.Contains("fagol"))
				{
					return false;
				}
			}
			string[] array = this.exactOneWeek;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == nameToCheck)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004D9B RID: 19867 RVA: 0x00172748 File Offset: 0x00170948
		public void UpdateColor(float red, float green, float blue)
		{
			this.redValue = Mathf.Clamp(red, 0f, 1f);
			this.greenValue = Mathf.Clamp(green, 0f, 1f);
			this.blueValue = Mathf.Clamp(blue, 0f, 1f);
		}

		// Token: 0x06004D9C RID: 19868 RVA: 0x00172797 File Offset: 0x00170997
		public void UpdateFailureText(string failMessage)
		{
			GorillaScoreboardTotalUpdater.instance.SetOfflineFailureText(failMessage);
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.EnableFailedState(failMessage);
			this.functionSelectText.EnableFailedState(failMessage);
		}

		// Token: 0x06004D9D RID: 19869 RVA: 0x001727C8 File Offset: 0x001709C8
		private void RestoreFromFailureState()
		{
			GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.DisableFailedState();
			this.functionSelectText.DisableFailedState();
		}

		// Token: 0x06004D9E RID: 19870 RVA: 0x001727F6 File Offset: 0x001709F6
		public void GeneralFailureMessage(string failMessage)
		{
			this.isConnectedToMaster = false;
			NetworkSystem.Instance.SetWrongVersion();
			this.UpdateFailureText(failMessage);
			this.UpdateScreen();
		}

		// Token: 0x06004D9F RID: 19871 RVA: 0x00172818 File Offset: 0x00170A18
		private static void OnErrorShared(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			else if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
			if (error.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						GorillaComputer.instance.GeneralFailureMessage(string.Concat(new string[]
						{
							"YOUR ACCOUNT ",
							PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
							" HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: ",
							keyValuePair.Key,
							"\nHOURS LEFT: ",
							((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()
						}));
						return;
					}
					GorillaComputer.instance.GeneralFailureMessage("YOUR ACCOUNT " + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + " HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (error.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value[0] != "Indefinite")
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						}
						else
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
						}
					}
				}
			}
		}

		// Token: 0x06004DA0 RID: 19872 RVA: 0x00172A70 File Offset: 0x00170C70
		private void DecreaseState()
		{
			this.currentStateIndex--;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex--;
			}
			if (this.currentStateIndex < 0)
			{
				this.currentStateIndex = this.FunctionsCount - 1;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06004DA1 RID: 19873 RVA: 0x00172AD4 File Offset: 0x00170CD4
		private void IncreaseState()
		{
			this.currentStateIndex++;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex++;
			}
			if (this.currentStateIndex >= this.FunctionsCount)
			{
				this.currentStateIndex = 0;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06004DA2 RID: 19874 RVA: 0x00172B38 File Offset: 0x00170D38
		public GorillaComputer.ComputerState GetState(int index)
		{
			GorillaComputer.ComputerState computerState;
			try
			{
				computerState = this._activeOrderList[index].State;
			}
			catch
			{
				computerState = this._activeOrderList[0].State;
			}
			return computerState;
		}

		// Token: 0x06004DA3 RID: 19875 RVA: 0x00172B80 File Offset: 0x00170D80
		public int GetStateIndex(GorillaComputer.ComputerState state)
		{
			return this._activeOrderList.FindIndex((GorillaComputer.StateOrderItem s) => s.State == state);
		}

		// Token: 0x06004DA4 RID: 19876 RVA: 0x00172BB4 File Offset: 0x00170DB4
		public string GetOrderListForScreen(GorillaComputer.ComputerState currentState)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int stateIndex = this.GetStateIndex(currentState);
			for (int i = 0; i < this.FunctionsCount; i++)
			{
				stringBuilder.Append(this.FunctionNames[i]);
				if (i == stateIndex)
				{
					stringBuilder.Append(this.Pointer);
				}
				if (i < this.FunctionsCount - 1)
				{
					stringBuilder.Append("\n");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004DA5 RID: 19877 RVA: 0x00172C21 File Offset: 0x00170E21
		private void GetCurrentTime()
		{
			this.tryGetTimeAgain = true;
			PlayFabClientAPI.GetTime(new GetTimeRequest(), new Action<GetTimeResult>(this.OnGetTimeSuccess), new Action<PlayFabError>(this.OnGetTimeFailure), null, null);
		}

		// Token: 0x06004DA6 RID: 19878 RVA: 0x00172C50 File Offset: 0x00170E50
		private void OnGetTimeSuccess(GetTimeResult result)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(result.Time.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = result.Time - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated == null)
			{
				return;
			}
			onServerTimeUpdated();
		}

		// Token: 0x06004DA7 RID: 19879 RVA: 0x00172CB8 File Offset: 0x00170EB8
		private void OnGetTimeFailure(PlayFabError error)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = DateTime.UtcNow - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated != null)
			{
				onServerTimeUpdated();
			}
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
		}

		// Token: 0x06004DA8 RID: 19880 RVA: 0x00172D4B File Offset: 0x00170F4B
		private void PlayerCountChangedCallback(NetPlayer player)
		{
			this.UpdateScreen();
		}

		// Token: 0x06004DA9 RID: 19881 RVA: 0x00172D54 File Offset: 0x00170F54
		public void SetNameBySafety(bool isSafety)
		{
			if (!isSafety)
			{
				return;
			}
			PlayerPrefs.SetString("playerNameBackup", this.currentName);
			this.currentName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			this.savedName = this.currentName;
			NetworkSystem.Instance.SetMyNickName(this.currentName);
			this.SetLocalNameTagText(this.currentName);
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { this.redValue, this.greenValue, this.blueValue });
			}
		}

		// Token: 0x06004DAA RID: 19882 RVA: 0x00172E2E File Offset: 0x0017102E
		public void SetLocalNameTagText(string newName)
		{
			VRRig.LocalRig.SetNameTagText(newName);
		}

		// Token: 0x06004DAB RID: 19883 RVA: 0x00172E3C File Offset: 0x0017103C
		public void SetComputerSettingsBySafety(bool isSafety, GorillaComputer.ComputerState[] toFilterOut, bool shouldHide)
		{
			this._activeOrderList = this.OrderList;
			if (!isSafety)
			{
				this._activeOrderList = this.OrderList;
				if (this._filteredStates.Count > 0 && toFilterOut.Length != 0)
				{
					for (int i = 0; i < toFilterOut.Length; i++)
					{
						if (this._filteredStates.Contains(toFilterOut[i]))
						{
							this._filteredStates.Remove(toFilterOut[i]);
						}
					}
				}
			}
			else if (shouldHide)
			{
				for (int j = 0; j < toFilterOut.Length; j++)
				{
					if (!this._filteredStates.Contains(toFilterOut[j]))
					{
						this._filteredStates.Add(toFilterOut[j]);
					}
				}
			}
			if (this._filteredStates.Count > 0)
			{
				int k = 0;
				int num = this._activeOrderList.Count;
				while (k < num)
				{
					if (this._filteredStates.Contains(this._activeOrderList[k].State))
					{
						this._activeOrderList.RemoveAt(k);
						k--;
						num--;
					}
					k++;
				}
			}
			this.FunctionsCount = this._activeOrderList.Count;
			this.FunctionNames.Clear();
			this._activeOrderList.ForEach(delegate(GorillaComputer.StateOrderItem s)
			{
				string name = s.GetName();
				if (name.Length > this.highestCharacterCount)
				{
					this.highestCharacterCount = name.Length;
				}
				this.FunctionNames.Add(name);
			});
			for (int l = 0; l < this.FunctionsCount; l++)
			{
				int num2 = this.highestCharacterCount - this.FunctionNames[l].Length;
				for (int m = 0; m < num2; m++)
				{
					List<string> functionNames = this.FunctionNames;
					int num3 = l;
					functionNames[num3] += " ";
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x06004DAC RID: 19884 RVA: 0x00172FD0 File Offset: 0x001711D0
		public void KID_SetVoiceChatSettingOnStart(bool voiceChatEnabled, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				return;
			}
			if (managedBy == Permission.ManagedByEnum.GUARDIAN)
			{
				string @string = PlayerPrefs.GetString("voiceChatOn", "");
				voiceChatEnabled = (string.IsNullOrEmpty(@string) ? voiceChatEnabled : (@string == "TRUE"));
				this.SetVoice(voiceChatEnabled, false);
				return;
			}
			voiceChatEnabled = PlayerPrefs.GetString("voiceChatOn", voiceChatEnabled ? "TRUE" : "") == "TRUE";
			this.SetVoice(voiceChatEnabled, !hasOptedInPreviously && voiceChatEnabled);
		}

		// Token: 0x06004DAD RID: 19885 RVA: 0x00173049 File Offset: 0x00171249
		private void SetVoice(bool setting, bool saveSetting = true)
		{
			this.voiceChatOn = (setting ? "TRUE" : "FALSE");
			if (setting)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Voice_Chat, true);
			}
			if (!saveSetting)
			{
				return;
			}
			PlayerPrefs.SetString("voiceChatOn", this.voiceChatOn);
			PlayerPrefs.Save();
		}

		// Token: 0x06004DAE RID: 19886 RVA: 0x00173083 File Offset: 0x00171283
		public bool CheckVoiceChatEnabled()
		{
			return this.voiceChatOn == "TRUE";
		}

		// Token: 0x06004DAF RID: 19887 RVA: 0x00173098 File Offset: 0x00171298
		private void SetVoiceChatBySafety(bool voiceChatEnabled, Permission.ManagedByEnum managedBy)
		{
			bool flag = !voiceChatEnabled;
			this.SetComputerSettingsBySafety(flag, new GorillaComputer.ComputerState[]
			{
				GorillaComputer.ComputerState.Voice,
				GorillaComputer.ComputerState.AutoMute,
				GorillaComputer.ComputerState.Mic
			}, false);
			string text = PlayerPrefs.GetString("voiceChatOn", "");
			if (KIDManager.KidEnabledAndReady)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
				if (permissionDataByFeature != null)
				{
					ValueTuple<bool, bool> valueTuple = KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, permissionDataByFeature);
					if (valueTuple.Item1 && !valueTuple.Item2)
					{
						text = "FALSE";
					}
				}
				else
				{
					Debug.LogErrorFormat("[KID] Could not find permission data for [" + EKIDFeatures.Voice_Chat.ToStandardisedString() + "]", Array.Empty<object>());
				}
			}
			switch (managedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (string.IsNullOrEmpty(text))
				{
					this.voiceChatOn = (voiceChatEnabled ? "TRUE" : "FALSE");
				}
				else
				{
					this.voiceChatOn = text;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).Enabled)
				{
					if (string.IsNullOrEmpty(text))
					{
						this.voiceChatOn = "TRUE";
					}
					else
					{
						this.voiceChatOn = text;
					}
				}
				else
				{
					this.voiceChatOn = "FALSE";
				}
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.voiceChatOn = "FALSE";
				break;
			}
			RigContainer.RefreshAllRigVoices();
			Debug.Log("[KID] On Session Update - Voice Chat Permission changed - Has enabled voiceChat? [" + voiceChatEnabled.ToString() + "]");
		}

		// Token: 0x06004DB0 RID: 19888 RVA: 0x001731C4 File Offset: 0x001713C4
		public void SetNametagSetting(bool setting, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				return;
			}
			if (managedBy == Permission.ManagedByEnum.GUARDIAN)
			{
				int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, 1);
				setting = setting && @int == 1;
				this.UpdateNametagSetting(setting, false);
				return;
			}
			setting = PlayerPrefs.GetInt(this.NameTagPlayerPref, setting ? 1 : 0) == 1;
			this.UpdateNametagSetting(setting, !hasOptedInPreviously && setting);
		}

		// Token: 0x06004DB1 RID: 19889 RVA: 0x00173220 File Offset: 0x00171420
		public static void RegisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Combine(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		// Token: 0x06004DB2 RID: 19890 RVA: 0x00173237 File Offset: 0x00171437
		public static void UnregisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Remove(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		// Token: 0x06004DB3 RID: 19891 RVA: 0x00173250 File Offset: 0x00171450
		private void UpdateNametagSetting(bool newSettingValue, bool saveSetting = true)
		{
			if (newSettingValue)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Custom_Nametags, true);
			}
			this.NametagsEnabled = newSettingValue;
			if (this.NametagsEnabled)
			{
				NetworkSystem.Instance.SetMyNickName(this.savedName);
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action != null)
			{
				action(this.NametagsEnabled);
			}
			if (!saveSetting)
			{
				return;
			}
			int num = (this.NametagsEnabled ? 1 : 0);
			PlayerPrefs.SetInt(this.NameTagPlayerPref, num);
			PlayerPrefs.Save();
		}

		// Token: 0x06004DB4 RID: 19892 RVA: 0x000023F4 File Offset: 0x000005F4
		void IMatchmakingCallbacks.OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
		{
		}

		// Token: 0x06004DB5 RID: 19893 RVA: 0x000023F4 File Offset: 0x000005F4
		void IMatchmakingCallbacks.OnCreatedRoom()
		{
		}

		// Token: 0x06004DB6 RID: 19894 RVA: 0x000023F4 File Offset: 0x000005F4
		void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06004DB7 RID: 19895 RVA: 0x000023F4 File Offset: 0x000005F4
		void IMatchmakingCallbacks.OnJoinedRoom()
		{
		}

		// Token: 0x06004DB8 RID: 19896 RVA: 0x000023F4 File Offset: 0x000005F4
		void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06004DB9 RID: 19897 RVA: 0x000023F4 File Offset: 0x000005F4
		void IMatchmakingCallbacks.OnLeftRoom()
		{
		}

		// Token: 0x06004DBA RID: 19898 RVA: 0x000023F4 File Offset: 0x000005F4
		void IMatchmakingCallbacks.OnPreLeavingRoom()
		{
		}

		// Token: 0x06004DBB RID: 19899 RVA: 0x001732BE File Offset: 0x001714BE
		void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
		{
			if (returnCode == 32765)
			{
				this.roomFull = true;
			}
		}

		// Token: 0x06004DBC RID: 19900 RVA: 0x001732CF File Offset: 0x001714CF
		public void SetInVirtualStump(bool inVirtualStump)
		{
			this.playerInVirtualStump = inVirtualStump;
			this.roomToJoin = (this.playerInVirtualStump ? (this.virtualStumpRoomPrepend + this.roomToJoin) : this.roomToJoin.RemoveAll(this.virtualStumpRoomPrepend, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x06004DBD RID: 19901 RVA: 0x0017330B File Offset: 0x0017150B
		public bool IsPlayerInVirtualStump()
		{
			return this.playerInVirtualStump;
		}

		// Token: 0x06004DBE RID: 19902 RVA: 0x00173313 File Offset: 0x00171513
		private void InitializeKIdState()
		{
			KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		// Token: 0x06004DBF RID: 19903 RVA: 0x00173326 File Offset: 0x00171526
		private void UpdateKidState()
		{
			this._currentScreentState = GorillaComputer.EKidScreenState.Ready;
		}

		// Token: 0x06004DC0 RID: 19904 RVA: 0x0017332F File Offset: 0x0017152F
		private void RequestUpdatedPermissions()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				return;
			}
			if (Time.time < this._nextUpdateAttemptTime)
			{
				return;
			}
			this._waitingForUpdatedSession = true;
			this.UpdateSession();
		}

		// Token: 0x06004DC1 RID: 19905 RVA: 0x00173360 File Offset: 0x00171560
		private async void UpdateSession()
		{
			this._nextUpdateAttemptTime = Time.time + this._updateAttemptCooldown;
			await KIDManager.UpdateSession();
			this._waitingForUpdatedSession = false;
		}

		// Token: 0x06004DC2 RID: 19906 RVA: 0x00173397 File Offset: 0x00171597
		private void OnSessionUpdate_GorillaComputer()
		{
			this.UpdateKidState();
			this.UpdateScreen();
		}

		// Token: 0x06004DC3 RID: 19907 RVA: 0x001733A5 File Offset: 0x001715A5
		private void ProcessScreen_SetupKID()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				Debug.LogError("[KID] Unable to start k-ID Flow. Kid is disabled");
				return;
			}
		}

		// Token: 0x06004DC4 RID: 19908 RVA: 0x001733BC File Offset: 0x001715BC
		private bool GuardianConsentMessage(string setupKIDButtonName, string featureDescription)
		{
			GorillaText gorillaText = this.screenText;
			gorillaText.Text = gorillaText.Text + "PARENT/GUARDIAN PERMISSION REQUIRED TO " + featureDescription.ToUpper() + "!";
			if (this._waitingForUpdatedSession)
			{
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text += "\n\nWAITING FOR PARENT/GUARDIAN CONSENT!";
				return true;
			}
			if (Time.time >= this._nextUpdateAttemptTime)
			{
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += "\n\nPRESS OPTION 2 TO REFRESH PERMISSIONS!";
			}
			else
			{
				GorillaText gorillaText4 = this.screenText;
				gorillaText4.Text = gorillaText4.Text + "\n\nCHECK AGAIN IN " + ((int)(this._nextUpdateAttemptTime - Time.time)).ToString() + " SECONDS!";
			}
			return false;
		}

		// Token: 0x06004DC5 RID: 19909 RVA: 0x00173474 File Offset: 0x00171674
		private void ProhibitedMessage(string verb)
		{
			this.screenText.Text = "";
			GorillaText gorillaText = this.screenText;
			gorillaText.Text = gorillaText.Text + "\n\nYOU ARE NOT ALLOWED TO " + verb + " IN YOUR JURISDICTION.";
		}

		// Token: 0x06004DC6 RID: 19910 RVA: 0x001734A7 File Offset: 0x001716A7
		private void RoomScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				this.screenText.Text = "YOU CANNOT USE THE PRIVATE ROOM FEATURE RIGHT NOW";
				return;
			}
			this.screenText.Text = "";
			this.GuardianConsentMessage("OPTION 3", "JOIN PRIVATE ROOMS");
		}

		// Token: 0x06004DC7 RID: 19911 RVA: 0x001734E2 File Offset: 0x001716E2
		private void RoomScreen_KIdProhibited()
		{
			this.ProhibitedMessage("CREATE OR JOIN PRIVATE ROOMS");
		}

		// Token: 0x06004DC8 RID: 19912 RVA: 0x001734F0 File Offset: 0x001716F0
		private void VoiceScreen_Permission()
		{
			this.screenText.Text = "VOICE TYPE: \"MONKE\"\n\n";
			if (!KIDManager.KidEnabled)
			{
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += "YOU CANNOT USE THE HUMAN VOICE TYPE FEATURE RIGHT NOW";
				return;
			}
			this.GuardianConsentMessage("OPTION 3", "ENABLE HUMAN VOICE CHAT");
		}

		// Token: 0x06004DC9 RID: 19913 RVA: 0x00173541 File Offset: 0x00171741
		private void VoiceScreen_KIdProhibited()
		{
			this.ProhibitedMessage("USE THE VOICE CHAT");
		}

		// Token: 0x06004DCA RID: 19914 RVA: 0x0017354E File Offset: 0x0017174E
		private void MicScreen_Permission()
		{
			this.screenText.Text = "";
			this.GuardianConsentMessage("OPTION 3", "ENABLE HUMAN VOICE CHAT");
		}

		// Token: 0x06004DCB RID: 19915 RVA: 0x00173571 File Offset: 0x00171771
		private void MicScreen_KIdProhibited()
		{
			this.VoiceScreen_KIdProhibited();
		}

		// Token: 0x06004DCC RID: 19916 RVA: 0x00173579 File Offset: 0x00171779
		private void NameScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				this.screenText.Text = "YOU CANNOT USE THE CUSTOM NICKNAME FEATURE RIGHT NOW";
				return;
			}
			this.screenText.Text = "";
			this.GuardianConsentMessage("OPTION 3", "SET CUSTOM NICKNAMES");
		}

		// Token: 0x06004DCD RID: 19917 RVA: 0x001735B4 File Offset: 0x001717B4
		private void NameScreen_KIdProhibited()
		{
			this.ProhibitedMessage("SET CUSTOM NICKNAMES");
		}

		// Token: 0x06004DCE RID: 19918 RVA: 0x001735C4 File Offset: 0x001717C4
		private void OnKIDSessionUpdated_CustomNicknames(bool showCustomNames, Permission.ManagedByEnum managedBy)
		{
			bool flag = (showCustomNames || managedBy == Permission.ManagedByEnum.PLAYER) && managedBy != Permission.ManagedByEnum.PROHIBITED;
			this.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[] { GorillaComputer.ComputerState.Name }, false);
			int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, -1);
			bool flag2 = @int > 0;
			switch (managedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (showCustomNames)
				{
					this.NametagsEnabled = @int == -1 || flag2;
				}
				else
				{
					this.NametagsEnabled = @int != -1 && flag2;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				this.NametagsEnabled = showCustomNames && (flag2 || @int == -1);
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.NametagsEnabled = false;
				break;
			}
			if (this.NametagsEnabled)
			{
				NetworkSystem.Instance.SetMyNickName(this.savedName);
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action == null)
			{
				return;
			}
			action(this.NametagsEnabled);
		}

		// Token: 0x06004DCF RID: 19919 RVA: 0x00173690 File Offset: 0x00171890
		private void TroopScreen_Permission()
		{
			this.screenText.Text = "";
			if (!KIDManager.KidEnabled)
			{
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += "YOU CANNOT USE THE TROOPS FEATURE RIGHT NOW";
				return;
			}
			this.GuardianConsentMessage("OPTION 3", "JOIN TROOPS");
		}

		// Token: 0x06004DD0 RID: 19920 RVA: 0x001736E1 File Offset: 0x001718E1
		private void TroopScreen_KIdProhibited()
		{
			this.ProhibitedMessage("CREATE OR JOIN TROOPS");
		}

		// Token: 0x06004DD1 RID: 19921 RVA: 0x001736EE File Offset: 0x001718EE
		private void ProcessKIdState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.option1 && this._currentScreentState == GorillaComputer.EKidScreenState.Ready)
			{
				this.RequestUpdatedPermissions();
			}
		}

		// Token: 0x06004DD2 RID: 19922 RVA: 0x00173703 File Offset: 0x00171903
		private void KIdScreen()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (!KIDManager.HasSession)
			{
				this.GuardianConsentMessage("OPTION 3", "");
				return;
			}
			this.KIdScreen_DisplayPermissions();
		}

		// Token: 0x06004DD3 RID: 19923 RVA: 0x0017372C File Offset: 0x0017192C
		private void KIdScreen_DisplayPermissions()
		{
			AgeStatusType activeAccountStatus = KIDManager.GetActiveAccountStatus();
			string text = ((!KIDManager.InitialisationSuccessful) ? "NOT READY" : activeAccountStatus.ToString());
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("k-ID Account Status:\t" + text);
			if (activeAccountStatus == (AgeStatusType)0)
			{
				stringBuilder.AppendLine("\nPress 'OPTION 1' to get permissions!");
				this.screenText.Text = stringBuilder.ToString();
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				stringBuilder.AppendLine("\nWAITING FOR PARENT/GUARDIAN CONSENT!");
				this.screenText.Text = stringBuilder.ToString();
				return;
			}
			stringBuilder.AppendLine("\nPermissions:");
			List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
			int count = allPermissionsData.Count;
			int num = 1;
			for (int i = 0; i < count; i++)
			{
				if (this._interestedPermissionNames.Contains(allPermissionsData[i].Name))
				{
					string text2 = (allPermissionsData[i].Enabled ? "<color=#85ffa5>" : "<color=\"RED\">");
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"[",
						num.ToString(),
						"] ",
						text2,
						allPermissionsData[i].Name,
						"</color>"
					}));
					num++;
				}
			}
			stringBuilder.AppendLine("\nTO REFRESH PERMISSIONS PRESS OPTION 1!");
			this.screenText.Text = stringBuilder.ToString();
		}

		// Token: 0x06004DD5 RID: 19925 RVA: 0x00011040 File Offset: 0x0000F240
		bool IGorillaSliceableSimple.get_isActiveAndEnabled()
		{
			return base.isActiveAndEnabled;
		}

		// Token: 0x06004DD8 RID: 19928 RVA: 0x00173ACD File Offset: 0x00171CCD
		[CompilerGenerated]
		private IEnumerator <LoadingScreen>g__LoadingScreenLocal|193_0()
		{
			int dotsCount = 0;
			while (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				int num = dotsCount;
				dotsCount = num + 1;
				if (dotsCount == 3)
				{
					dotsCount = 0;
				}
				this.screenText.Text = "LOADING";
				for (int i = 0; i < dotsCount; i++)
				{
					GorillaText gorillaText = this.screenText;
					gorillaText.Text += ". ";
				}
				yield return this.waitOneSecond;
			}
			yield break;
		}

		// Token: 0x04005030 RID: 20528
		private const bool HIDE_SCREENS = false;

		// Token: 0x04005031 RID: 20529
		public const string NAMETAG_PLAYER_PREF_KEY = "nameTagsOn";

		// Token: 0x04005032 RID: 20530
		[OnEnterPlay_SetNull]
		public static volatile GorillaComputer instance;

		// Token: 0x04005033 RID: 20531
		[OnEnterPlay_Set(false)]
		public static bool hasInstance;

		// Token: 0x04005034 RID: 20532
		[OnEnterPlay_SetNull]
		private static Action<bool> onNametagSettingChangedAction;

		// Token: 0x04005035 RID: 20533
		public bool tryGetTimeAgain;

		// Token: 0x04005036 RID: 20534
		public Material unpressedMaterial;

		// Token: 0x04005037 RID: 20535
		public Material pressedMaterial;

		// Token: 0x04005038 RID: 20536
		public string currentTextField;

		// Token: 0x04005039 RID: 20537
		public float buttonFadeTime;

		// Token: 0x0400503A RID: 20538
		public string offlineTextInitialString;

		// Token: 0x0400503B RID: 20539
		public GorillaText screenText;

		// Token: 0x0400503C RID: 20540
		public GorillaText functionSelectText;

		// Token: 0x0400503D RID: 20541
		public GorillaText wallScreenText;

		// Token: 0x0400503E RID: 20542
		public string versionMismatch = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";

		// Token: 0x0400503F RID: 20543
		public string unableToConnect = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";

		// Token: 0x04005040 RID: 20544
		public Material wrongVersionMaterial;

		// Token: 0x04005041 RID: 20545
		public MeshRenderer wallScreenRenderer;

		// Token: 0x04005042 RID: 20546
		public MeshRenderer computerScreenRenderer;

		// Token: 0x04005043 RID: 20547
		public long startupMillis;

		// Token: 0x04005044 RID: 20548
		public DateTime startupTime;

		// Token: 0x04005045 RID: 20549
		public string lastPressedGameMode;

		// Token: 0x04005046 RID: 20550
		public WatchableStringSO currentGameMode;

		// Token: 0x04005047 RID: 20551
		public WatchableStringSO currentGameModeText;

		// Token: 0x04005048 RID: 20552
		public int includeUpdatedServerSynchTest;

		// Token: 0x04005049 RID: 20553
		public PhotonNetworkController networkController;

		// Token: 0x0400504A RID: 20554
		public float updateCooldown = 1f;

		// Token: 0x0400504B RID: 20555
		public float lastUpdateTime;

		// Token: 0x0400504C RID: 20556
		public bool isConnectedToMaster;

		// Token: 0x0400504D RID: 20557
		public bool internetFailure;

		// Token: 0x0400504E RID: 20558
		public string[] allowedMapsToJoin;

		// Token: 0x0400504F RID: 20559
		[Header("State vars")]
		public bool stateUpdated;

		// Token: 0x04005050 RID: 20560
		public bool screenChanged;

		// Token: 0x04005051 RID: 20561
		public bool initialized;

		// Token: 0x04005052 RID: 20562
		public List<GorillaComputer.StateOrderItem> OrderList = new List<GorillaComputer.StateOrderItem>
		{
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Room),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Name),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Color),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Turn),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Mic),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Queue),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Troop),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Group),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Voice),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.AutoMute, "Automod"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Visuals, "Items"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Credits),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Support)
		};

		// Token: 0x04005053 RID: 20563
		public string Pointer = "<-";

		// Token: 0x04005054 RID: 20564
		public int highestCharacterCount;

		// Token: 0x04005055 RID: 20565
		public List<string> FunctionNames = new List<string>();

		// Token: 0x04005056 RID: 20566
		public int FunctionsCount;

		// Token: 0x04005057 RID: 20567
		[Header("Room vars")]
		public string roomToJoin;

		// Token: 0x04005058 RID: 20568
		public bool roomFull;

		// Token: 0x04005059 RID: 20569
		public bool roomNotAllowed;

		// Token: 0x0400505A RID: 20570
		[Header("Mic vars")]
		public string pttType;

		// Token: 0x0400505B RID: 20571
		[Header("Automute vars")]
		public string autoMuteType;

		// Token: 0x0400505C RID: 20572
		[Header("Queue vars")]
		public string currentQueue;

		// Token: 0x0400505D RID: 20573
		public bool allowedInCompetitive;

		// Token: 0x0400505E RID: 20574
		[Header("Group Vars")]
		public string groupMapJoin;

		// Token: 0x0400505F RID: 20575
		public int groupMapJoinIndex;

		// Token: 0x04005060 RID: 20576
		public GorillaFriendCollider friendJoinCollider;

		// Token: 0x04005061 RID: 20577
		[Header("Troop vars")]
		public string troopName;

		// Token: 0x04005062 RID: 20578
		public bool troopQueueActive;

		// Token: 0x04005063 RID: 20579
		public string troopToJoin;

		// Token: 0x04005064 RID: 20580
		[Header("Join Triggers")]
		public Dictionary<string, GorillaNetworkJoinTrigger> primaryTriggersByZone = new Dictionary<string, GorillaNetworkJoinTrigger>();

		// Token: 0x04005065 RID: 20581
		public string voiceChatOn;

		// Token: 0x04005066 RID: 20582
		[Header("Mode select vars")]
		public ModeSelectButton[] modeSelectButtons;

		// Token: 0x04005067 RID: 20583
		public string version;

		// Token: 0x04005068 RID: 20584
		public string buildDate;

		// Token: 0x04005069 RID: 20585
		public string buildCode;

		// Token: 0x0400506A RID: 20586
		[Header("Cosmetics")]
		public bool disableParticles;

		// Token: 0x0400506B RID: 20587
		public float instrumentVolume;

		// Token: 0x0400506C RID: 20588
		[Header("Credits")]
		public CreditsView creditsView;

		// Token: 0x0400506D RID: 20589
		[Header("Handedness")]
		public bool leftHanded;

		// Token: 0x0400506E RID: 20590
		[Header("Name state vars")]
		public string savedName;

		// Token: 0x0400506F RID: 20591
		public string currentName;

		// Token: 0x04005070 RID: 20592
		public TextAsset exactOneWeekFile;

		// Token: 0x04005071 RID: 20593
		public TextAsset anywhereOneWeekFile;

		// Token: 0x04005072 RID: 20594
		public TextAsset anywhereTwoWeekFile;

		// Token: 0x04005073 RID: 20595
		private List<GorillaComputer.ComputerState> _filteredStates = new List<GorillaComputer.ComputerState>();

		// Token: 0x04005074 RID: 20596
		private List<GorillaComputer.StateOrderItem> _activeOrderList = new List<GorillaComputer.StateOrderItem>();

		// Token: 0x04005075 RID: 20597
		private Stack<GorillaComputer.ComputerState> stateStack = new Stack<GorillaComputer.ComputerState>();

		// Token: 0x04005076 RID: 20598
		private GorillaComputer.ComputerState currentComputerState;

		// Token: 0x04005077 RID: 20599
		private GorillaComputer.ComputerState previousComputerState;

		// Token: 0x04005078 RID: 20600
		private int currentStateIndex;

		// Token: 0x04005079 RID: 20601
		private int usersBanned;

		// Token: 0x0400507A RID: 20602
		private float redValue;

		// Token: 0x0400507B RID: 20603
		private string redText;

		// Token: 0x0400507C RID: 20604
		private float blueValue;

		// Token: 0x0400507D RID: 20605
		private string blueText;

		// Token: 0x0400507E RID: 20606
		private float greenValue;

		// Token: 0x0400507F RID: 20607
		private string greenText;

		// Token: 0x04005080 RID: 20608
		private int colorCursorLine;

		// Token: 0x04005081 RID: 20609
		private string warningConfirmationInputString = string.Empty;

		// Token: 0x04005082 RID: 20610
		private bool displaySupport;

		// Token: 0x04005083 RID: 20611
		private string[] exactOneWeek;

		// Token: 0x04005084 RID: 20612
		private string[] anywhereOneWeek;

		// Token: 0x04005085 RID: 20613
		private string[] anywhereTwoWeek;

		// Token: 0x04005086 RID: 20614
		private GorillaComputer.RedemptionResult redemptionResult;

		// Token: 0x04005087 RID: 20615
		private string redemptionCode = "";

		// Token: 0x04005088 RID: 20616
		private bool playerInVirtualStump;

		// Token: 0x04005089 RID: 20617
		private string virtualStumpRoomPrepend = "";

		// Token: 0x0400508A RID: 20618
		private WaitForSeconds waitOneSecond = new WaitForSeconds(1f);

		// Token: 0x0400508B RID: 20619
		private Coroutine LoadingRoutine;

		// Token: 0x0400508C RID: 20620
		private List<string> topTroops = new List<string>();

		// Token: 0x0400508D RID: 20621
		private bool hasRequestedInitialTroopPopulation;

		// Token: 0x0400508E RID: 20622
		private int currentTroopPopulation = -1;

		// Token: 0x04005090 RID: 20624
		private float lastCheckedWifi;

		// Token: 0x04005091 RID: 20625
		private float checkIfDisconnectedSeconds = 10f;

		// Token: 0x04005092 RID: 20626
		private float checkIfConnectedSeconds = 1f;

		// Token: 0x04005093 RID: 20627
		private float troopPopulationCheckCooldown = 3f;

		// Token: 0x04005094 RID: 20628
		private float nextPopulationCheckTime;

		// Token: 0x04005095 RID: 20629
		public Action OnServerTimeUpdated;

		// Token: 0x04005096 RID: 20630
		private const string ENABLED_COLOUR = "#85ffa5";

		// Token: 0x04005097 RID: 20631
		private const string DISABLED_COLOUR = "\"RED\"";

		// Token: 0x04005098 RID: 20632
		private const string FAMILY_PORTAL_URL = "k-id.com/code";

		// Token: 0x04005099 RID: 20633
		private float _updateAttemptCooldown = 60f;

		// Token: 0x0400509A RID: 20634
		private float _nextUpdateAttemptTime;

		// Token: 0x0400509B RID: 20635
		private bool _waitingForUpdatedSession;

		// Token: 0x0400509C RID: 20636
		private GorillaComputer.EKidScreenState _currentScreentState = GorillaComputer.EKidScreenState.Show_OTP;

		// Token: 0x0400509D RID: 20637
		private string[] _interestedPermissionNames = new string[] { "custom-username", "voice-chat", "join-groups" };

		// Token: 0x02000C2F RID: 3119
		public enum ComputerState
		{
			// Token: 0x0400509F RID: 20639
			Startup,
			// Token: 0x040050A0 RID: 20640
			Color,
			// Token: 0x040050A1 RID: 20641
			Name,
			// Token: 0x040050A2 RID: 20642
			Turn,
			// Token: 0x040050A3 RID: 20643
			Mic,
			// Token: 0x040050A4 RID: 20644
			Room,
			// Token: 0x040050A5 RID: 20645
			Queue,
			// Token: 0x040050A6 RID: 20646
			Group,
			// Token: 0x040050A7 RID: 20647
			Voice,
			// Token: 0x040050A8 RID: 20648
			AutoMute,
			// Token: 0x040050A9 RID: 20649
			Credits,
			// Token: 0x040050AA RID: 20650
			Visuals,
			// Token: 0x040050AB RID: 20651
			Time,
			// Token: 0x040050AC RID: 20652
			NameWarning,
			// Token: 0x040050AD RID: 20653
			Loading,
			// Token: 0x040050AE RID: 20654
			Support,
			// Token: 0x040050AF RID: 20655
			Troop,
			// Token: 0x040050B0 RID: 20656
			KID,
			// Token: 0x040050B1 RID: 20657
			Redemption
		}

		// Token: 0x02000C30 RID: 3120
		private enum NameCheckResult
		{
			// Token: 0x040050B3 RID: 20659
			Success,
			// Token: 0x040050B4 RID: 20660
			Warning,
			// Token: 0x040050B5 RID: 20661
			Ban
		}

		// Token: 0x02000C31 RID: 3121
		public enum RedemptionResult
		{
			// Token: 0x040050B7 RID: 20663
			Empty,
			// Token: 0x040050B8 RID: 20664
			Invalid,
			// Token: 0x040050B9 RID: 20665
			Checking,
			// Token: 0x040050BA RID: 20666
			AlreadyUsed,
			// Token: 0x040050BB RID: 20667
			Success
		}

		// Token: 0x02000C32 RID: 3122
		[Serializable]
		public class StateOrderItem
		{
			// Token: 0x06004DDA RID: 19930 RVA: 0x00173B16 File Offset: 0x00171D16
			public StateOrderItem()
			{
			}

			// Token: 0x06004DDB RID: 19931 RVA: 0x00173B29 File Offset: 0x00171D29
			public StateOrderItem(GorillaComputer.ComputerState state)
			{
				this.State = state;
			}

			// Token: 0x06004DDC RID: 19932 RVA: 0x00173B43 File Offset: 0x00171D43
			public StateOrderItem(GorillaComputer.ComputerState state, string overrideName)
			{
				this.State = state;
				this.OverrideName = overrideName;
			}

			// Token: 0x06004DDD RID: 19933 RVA: 0x00173B64 File Offset: 0x00171D64
			public string GetName()
			{
				if (!string.IsNullOrEmpty(this.OverrideName))
				{
					return this.OverrideName.ToUpper();
				}
				return this.State.ToString().ToUpper();
			}

			// Token: 0x040050BC RID: 20668
			public GorillaComputer.ComputerState State;

			// Token: 0x040050BD RID: 20669
			[Tooltip("Case not important - ToUpper applied at runtime")]
			public string OverrideName = "";
		}

		// Token: 0x02000C33 RID: 3123
		private enum EKidScreenState
		{
			// Token: 0x040050BF RID: 20671
			Ready,
			// Token: 0x040050C0 RID: 20672
			Show_OTP,
			// Token: 0x040050C1 RID: 20673
			Show_Setup_Screen
		}
	}
}
