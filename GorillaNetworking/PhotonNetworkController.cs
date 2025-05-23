using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000C58 RID: 3160
	public class PhotonNetworkController : MonoBehaviour
	{
		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x06004E5A RID: 20058 RVA: 0x00175949 File Offset: 0x00173B49
		// (set) Token: 0x06004E5B RID: 20059 RVA: 0x00175951 File Offset: 0x00173B51
		public string StartLevel
		{
			get
			{
				return this.startLevel;
			}
			set
			{
				this.startLevel = value;
			}
		}

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x06004E5C RID: 20060 RVA: 0x0017595A File Offset: 0x00173B5A
		// (set) Token: 0x06004E5D RID: 20061 RVA: 0x00175962 File Offset: 0x00173B62
		public GTZone StartZone
		{
			get
			{
				return this.startZone;
			}
			set
			{
				this.startZone = value;
			}
		}

		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x06004E5E RID: 20062 RVA: 0x0017596B File Offset: 0x00173B6B
		public GTZone CurrentRoomZone
		{
			get
			{
				if (!(this.currentJoinTrigger != null))
				{
					return GTZone.none;
				}
				return this.currentJoinTrigger.zone;
			}
		}

		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x06004E5F RID: 20063 RVA: 0x00175989 File Offset: 0x00173B89
		// (set) Token: 0x06004E60 RID: 20064 RVA: 0x00175991 File Offset: 0x00173B91
		public GorillaGeoHideShowTrigger StartGeoTrigger
		{
			get
			{
				return this.startGeoTrigger;
			}
			set
			{
				this.startGeoTrigger = value;
			}
		}

		// Token: 0x06004E61 RID: 20065 RVA: 0x0017599C File Offset: 0x00173B9C
		public void Awake()
		{
			if (PhotonNetworkController.Instance == null)
			{
				PhotonNetworkController.Instance = this;
			}
			else if (PhotonNetworkController.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this.updatedName = false;
			this.playersInRegion = new int[this.serverRegions.Length];
			this.pingInRegion = new int[this.serverRegions.Length];
		}

		// Token: 0x06004E62 RID: 20066 RVA: 0x00175A0C File Offset: 0x00173C0C
		public void Start()
		{
			base.StartCoroutine(this.DisableOnStart());
			NetworkSystem.Instance.OnJoinedRoomEvent += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
		}

		// Token: 0x06004E63 RID: 20067 RVA: 0x00175A62 File Offset: 0x00173C62
		private IEnumerator DisableOnStart()
		{
			ZoneManagement.SetActiveZone(this.StartZone);
			yield break;
		}

		// Token: 0x06004E64 RID: 20068 RVA: 0x00175A74 File Offset: 0x00173C74
		public void FixedUpdate()
		{
			this.headRightHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.rightControllerTransform.position).magnitude;
			this.headLeftHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.leftControllerTransform.position).magnitude;
			this.headQuat = GTPlayer.Instance.headCollider.transform.rotation;
			if (!this.disableAFKKick && Quaternion.Angle(this.headQuat, this.lastHeadQuat) <= 0.01f && Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) < 0.001f && Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) < 0.001f && this.pauseTime + this.disconnectTime < Time.realtimeSinceStartup)
			{
				this.pauseTime = Time.realtimeSinceStartup;
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			else if (Quaternion.Angle(this.headQuat, this.lastHeadQuat) > 0.01f || Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) >= 0.001f || Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) >= 0.001f)
			{
				this.pauseTime = Time.realtimeSinceStartup;
			}
			this.lastHeadRightHandDistance = this.headRightHandDistance;
			this.lastHeadLeftHandDistance = this.headLeftHandDistance;
			this.lastHeadQuat = this.headQuat;
			if (this.deferredJoin && Time.time >= this.partyJoinDeferredUntilTimestamp)
			{
				if ((this.partyJoinDeferredUntilTimestamp != 0f || NetworkSystem.Instance.netState == NetSystemState.Idle) && this.currentJoinTrigger != null)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
					if (this.currentJoinTrigger == this.privateTrigger)
					{
						this.AttemptToJoinSpecificRoom(this.customRoomID, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
						return;
					}
					this.AttemptToJoinPublicRoom(this.currentJoinTrigger, this.currentJoinType);
					return;
				}
				else if (NetworkSystem.Instance.netState != NetSystemState.PingRecon && NetworkSystem.Instance.netState != NetSystemState.Initialization)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
				}
			}
		}

		// Token: 0x06004E65 RID: 20069 RVA: 0x00175CC1 File Offset: 0x00173EC1
		public void DeferJoining(float duration)
		{
			this.partyJoinDeferredUntilTimestamp = Mathf.Max(this.partyJoinDeferredUntilTimestamp, Time.time + duration);
		}

		// Token: 0x06004E66 RID: 20070 RVA: 0x00175CDB File Offset: 0x00173EDB
		public void ClearDeferredJoin()
		{
			this.partyJoinDeferredUntilTimestamp = 0f;
			this.deferredJoin = false;
		}

		// Token: 0x06004E67 RID: 20071 RVA: 0x00175CEF File Offset: 0x00173EEF
		public void AttemptToJoinPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo)
		{
			this.AttemptToJoinPublicRoomAsync(triggeredTrigger, roomJoinType);
		}

		// Token: 0x06004E68 RID: 20072 RVA: 0x00175CFC File Offset: 0x00173EFC
		private async void AttemptToJoinPublicRoomAsync(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType)
		{
			if (KIDManager.CheckFeatureOptIn(EKIDFeatures.Multiplayer, null).Item2 && base.enabled)
			{
				if (NetworkSystem.Instance.netState != NetSystemState.Connecting && NetworkSystem.Instance.netState != NetSystemState.Disconnecting)
				{
					if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon || Time.time < this.partyJoinDeferredUntilTimestamp)
					{
						this.currentJoinTrigger = triggeredTrigger;
						this.currentJoinType = roomJoinType;
						this.deferredJoin = true;
					}
					else
					{
						this.deferredJoin = false;
						string desiredGameMode = triggeredTrigger.GetFullDesiredGameModeString();
						if (NetworkSystem.Instance.InRoom)
						{
							if (NetworkSystem.Instance.SessionIsPrivate)
							{
								if (roomJoinType != JoinType.JoinWithNearby && roomJoinType != JoinType.ForceJoinWithParty)
								{
									return;
								}
							}
							else if (NetworkSystem.Instance.GameModeString.StartsWith(desiredGameMode))
							{
								return;
							}
						}
						if (roomJoinType == JoinType.JoinWithParty || roomJoinType == JoinType.ForceJoinWithParty)
						{
							Debug.Log("Sending Party follow command" + Time.time.ToString());
							await this.SendPartyFollowCommands();
							Debug.Log("After party command" + Time.time.ToString());
						}
						this.currentJoinTrigger = triggeredTrigger;
						this.currentJoinType = roomJoinType;
						if (PlayFabClientAPI.IsClientLoggedIn())
						{
							this.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
						}
						RoomConfig roomConfig = RoomConfig.AnyPublicConfig();
						if (this.currentJoinType == JoinType.JoinWithNearby)
						{
							roomConfig.SetFriendIDs(this.friendIDList);
						}
						else if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.ForceJoinWithParty)
						{
							roomConfig.SetFriendIDs(FriendshipGroupDetection.Instance.PartyMemberIDs.ToList<string>());
						}
						Hashtable hashtable = new Hashtable
						{
							{ "gameMode", desiredGameMode },
							{ "platform", this.platformTag },
							{
								"queueName",
								GorillaComputer.instance.currentQueue
							}
						};
						roomConfig.CustomProps = hashtable;
						roomConfig.MaxPlayers = this.GetRoomSize(this.currentJoinTrigger.networkZone);
						await NetworkSystem.Instance.ConnectToRoom(null, roomConfig, -1);
					}
				}
			}
		}

		// Token: 0x06004E69 RID: 20073 RVA: 0x00175D44 File Offset: 0x00173F44
		private async Task SendPartyFollowCommands()
		{
			PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			RoomSystem.SendPartyFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
			PhotonNetwork.SendAllOutgoingCommands();
			await Task.Delay(200);
		}

		// Token: 0x06004E6A RID: 20074 RVA: 0x00175D7F File Offset: 0x00173F7F
		public void AttemptToJoinSpecificRoom(string roomID, JoinType roomJoinType)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, null);
		}

		// Token: 0x06004E6B RID: 20075 RVA: 0x00175D8B File Offset: 0x00173F8B
		public void AttemptToJoinSpecificRoomWithCallback(string roomID, JoinType roomJoinType, Action<NetJoinResult> callback)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, callback);
		}

		// Token: 0x06004E6C RID: 20076 RVA: 0x00175D98 File Offset: 0x00173F98
		public async Task AttemptToJoinSpecificRoomAsync(string roomID, JoinType roomJoinType, Action<NetJoinResult> callback)
		{
			TaskAwaiter<bool> taskAwaiter = KIDManager.UseKID().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			if (!taskAwaiter.GetResult() || KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer))
			{
				if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
				{
					this.deferredJoin = true;
					this.customRoomID = roomID;
					this.currentJoinType = roomJoinType;
					this.currentJoinTrigger = this.privateTrigger;
				}
				else if (NetworkSystem.Instance.netState == NetSystemState.Idle || NetworkSystem.Instance.netState == NetSystemState.InGame)
				{
					this.customRoomID = roomID;
					this.currentJoinType = roomJoinType;
					this.currentJoinTrigger = this.privateTrigger;
					this.deferredJoin = false;
					if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.ForceJoinWithParty)
					{
						await this.SendPartyFollowCommands();
					}
					string fullDesiredGameModeString = this.currentJoinTrigger.GetFullDesiredGameModeString();
					Hashtable hashtable = new Hashtable
					{
						{ "gameMode", fullDesiredGameModeString },
						{ "platform", this.platformTag },
						{
							"queueName",
							GorillaComputer.instance.currentQueue
						}
					};
					RoomConfig roomConfig = new RoomConfig();
					roomConfig.createIfMissing = true;
					roomConfig.isJoinable = true;
					roomConfig.isPublic = false;
					roomConfig.MaxPlayers = this.GetRoomSize(this.currentJoinTrigger.networkZone);
					roomConfig.CustomProps = hashtable;
					if (roomJoinType == JoinType.FriendStationPublic)
					{
						roomConfig.isPublic = true;
					}
					if (PlayFabClientAPI.IsClientLoggedIn())
					{
						this.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
					}
					Task<NetJoinResult> connectToRoomTask = NetworkSystem.Instance.ConnectToRoom(roomID, roomConfig, -1);
					if (callback != null)
					{
						await connectToRoomTask;
						Debug.Log("AttemptToJoinSpecificRoomAsync ConnectToRoom Result: " + connectToRoomTask.Result.ToString());
						callback(connectToRoomTask.Result);
					}
				}
			}
		}

		// Token: 0x06004E6D RID: 20077 RVA: 0x00175DF4 File Offset: 0x00173FF4
		private void DisconnectCleanup()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (GorillaParent.instance != null)
			{
				GorillaScoreboardSpawner[] componentsInChildren = GorillaParent.instance.GetComponentsInChildren<GorillaScoreboardSpawner>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnLeftRoom();
				}
			}
			this.attemptingToConnect = true;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.offlineVRRig)
			{
				if (skinnedMeshRenderer != null)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
			if (GorillaComputer.instance != null && !ApplicationQuittingState.IsQuitting)
			{
				this.UpdateTriggerScreens();
			}
			GTPlayer.Instance.maxJumpSpeed = 6.5f;
			GTPlayer.Instance.jumpMultiplier = 1.1f;
			GorillaNot.instance.currentMasterClient = null;
			GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
			this.initialGameMode = "";
		}

		// Token: 0x06004E6E RID: 20078 RVA: 0x00175ED4 File Offset: 0x001740D4
		public void OnJoinedRoom()
		{
			if (NetworkSystem.Instance.GameModeString.IsNullOrEmpty())
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			this.initialGameMode = NetworkSystem.Instance.GameModeString;
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.currentJoinTrigger = this.privateTrigger;
				PhotonNetworkController.Instance.UpdateTriggerScreens();
			}
			else if (this.currentJoinType != JoinType.FollowingParty)
			{
				bool flag = false;
				for (int i = 0; i < GorillaComputer.instance.allowedMapsToJoin.Length; i++)
				{
					if (NetworkSystem.Instance.GameModeString.StartsWith(GorillaComputer.instance.allowedMapsToJoin[i]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					GorillaComputer.instance.roomNotAllowed = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
					return;
				}
			}
			NetworkSystem.Instance.SetMyTutorialComplete();
			VRRigCache.Instance.InstantiateNetworkObject();
			if (NetworkSystem.Instance.IsMasterClient)
			{
				global::GorillaGameModes.GameMode.LoadGameModeFromProperty(this.initialGameMode);
			}
			GorillaComputer.instance.roomFull = false;
			GorillaComputer.instance.roomNotAllowed = false;
			if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.JoinWithNearby || this.currentJoinType == JoinType.ForceJoinWithParty)
			{
				this.keyToFollow = NetworkSystem.Instance.LocalPlayer.UserId + this.keyStr;
				NetworkSystem.Instance.BroadcastMyRoom(true, this.keyToFollow, this.shuffler);
			}
			GorillaNot.instance.currentMasterClient = null;
			this.UpdateCurrentJoinTrigger();
			this.UpdateTriggerScreens();
			NetworkSystem.Instance.MultiplayerStarted();
		}

		// Token: 0x06004E6F RID: 20079 RVA: 0x00176051 File Offset: 0x00174251
		public void RegisterJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.allJoinTriggers.Add(trigger);
		}

		// Token: 0x06004E70 RID: 20080 RVA: 0x00176060 File Offset: 0x00174260
		private void UpdateCurrentJoinTrigger()
		{
			GorillaNetworkJoinTrigger joinTriggerFromFullGameModeString = GorillaComputer.instance.GetJoinTriggerFromFullGameModeString(NetworkSystem.Instance.GameModeString);
			if (joinTriggerFromFullGameModeString != null)
			{
				this.currentJoinTrigger = joinTriggerFromFullGameModeString;
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				if (this.currentJoinTrigger != this.privateTrigger)
				{
					Debug.LogError("IN a private game but private trigger isnt current");
					return;
				}
			}
			else
			{
				Debug.LogError("Not in private room and unabel tp update jointrigger.");
			}
		}

		// Token: 0x06004E71 RID: 20081 RVA: 0x001760CC File Offset: 0x001742CC
		public void UpdateTriggerScreens()
		{
			foreach (GorillaNetworkJoinTrigger gorillaNetworkJoinTrigger in this.allJoinTriggers)
			{
				gorillaNetworkJoinTrigger.UpdateUI();
			}
		}

		// Token: 0x06004E72 RID: 20082 RVA: 0x0017611C File Offset: 0x0017431C
		public void AttemptToFollowIntoPub(string userIDToFollow, int actorNumberToFollow, string newKeyStr, string shufflerStr, JoinType joinType)
		{
			this.friendToFollow = userIDToFollow;
			this.keyToFollow = userIDToFollow + newKeyStr;
			this.shuffler = shufflerStr;
			this.currentJoinType = joinType;
			this.ClearDeferredJoin();
			if (NetworkSystem.Instance.InRoom)
			{
				NetworkSystem.Instance.JoinFriendsRoom(this.friendToFollow, actorNumberToFollow, this.keyToFollow, this.shuffler);
			}
		}

		// Token: 0x06004E73 RID: 20083 RVA: 0x0017617D File Offset: 0x0017437D
		public void OnDisconnected()
		{
			this.DisconnectCleanup();
		}

		// Token: 0x06004E74 RID: 20084 RVA: 0x00176185 File Offset: 0x00174385
		public void OnApplicationQuit()
		{
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion != "dev";
			}
		}

		// Token: 0x06004E75 RID: 20085 RVA: 0x001761A8 File Offset: 0x001743A8
		private string ReturnRoomName()
		{
			if (this.isPrivate)
			{
				return this.customRoomID;
			}
			return this.RandomRoomName();
		}

		// Token: 0x06004E76 RID: 20086 RVA: 0x001761C0 File Offset: 0x001743C0
		private string RandomRoomName()
		{
			string text = "";
			for (int i = 0; i < 4; i++)
			{
				text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
			}
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				return text;
			}
			return this.RandomRoomName();
		}

		// Token: 0x06004E77 RID: 20087 RVA: 0x00176218 File Offset: 0x00174418
		public byte GetRoomSize(string gameModeName)
		{
			return 10;
		}

		// Token: 0x06004E78 RID: 20088 RVA: 0x0017621C File Offset: 0x0017441C
		private string GetRegionWithLowestPing()
		{
			int num = 10000;
			int num2 = 0;
			for (int i = 0; i < this.serverRegions.Length; i++)
			{
				Debug.Log("ping in region " + this.serverRegions[i] + " is " + this.pingInRegion[i].ToString());
				if (this.pingInRegion[i] < num && this.pingInRegion[i] > 0)
				{
					num = this.pingInRegion[i];
					num2 = i;
				}
			}
			return this.serverRegions[num2];
		}

		// Token: 0x06004E79 RID: 20089 RVA: 0x0017629C File Offset: 0x0017449C
		public int TotalUsers()
		{
			int num = 0;
			foreach (int num2 in this.playersInRegion)
			{
				num += num2;
			}
			return num;
		}

		// Token: 0x06004E7A RID: 20090 RVA: 0x001762CC File Offset: 0x001744CC
		public string CurrentState()
		{
			if (NetworkSystem.Instance == null)
			{
				Debug.Log("Null netsys!!!");
			}
			return NetworkSystem.Instance.netState.ToString();
		}

		// Token: 0x06004E7B RID: 20091 RVA: 0x00176308 File Offset: 0x00174508
		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				this.timeWhenApplicationPaused = new DateTime?(DateTime.Now);
				return;
			}
			if ((DateTime.Now - (this.timeWhenApplicationPaused ?? DateTime.Now)).TotalSeconds > (double)this.disconnectTime)
			{
				this.timeWhenApplicationPaused = null;
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance != null)
				{
					instance.ReturnToSinglePlayer();
				}
			}
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance2 = NetworkSystem.Instance;
				if (instance2 == null)
				{
					return;
				}
				instance2.ReturnToSinglePlayer();
			}
		}

		// Token: 0x06004E7C RID: 20092 RVA: 0x001763B5 File Offset: 0x001745B5
		private void OnApplicationFocus(bool focus)
		{
			if (!focus && NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance == null)
				{
					return;
				}
				instance.ReturnToSinglePlayer();
			}
		}

		// Token: 0x04005163 RID: 20835
		public static volatile PhotonNetworkController Instance;

		// Token: 0x04005164 RID: 20836
		public int incrementCounter;

		// Token: 0x04005165 RID: 20837
		public PlayFabAuthenticator playFabAuthenticator;

		// Token: 0x04005166 RID: 20838
		public string[] serverRegions;

		// Token: 0x04005167 RID: 20839
		public bool isPrivate;

		// Token: 0x04005168 RID: 20840
		public string customRoomID;

		// Token: 0x04005169 RID: 20841
		public GameObject playerOffset;

		// Token: 0x0400516A RID: 20842
		public SkinnedMeshRenderer[] offlineVRRig;

		// Token: 0x0400516B RID: 20843
		public bool attemptingToConnect;

		// Token: 0x0400516C RID: 20844
		private int currentRegionIndex;

		// Token: 0x0400516D RID: 20845
		public string currentGameType;

		// Token: 0x0400516E RID: 20846
		public bool roomCosmeticsInitialized;

		// Token: 0x0400516F RID: 20847
		public GameObject photonVoiceObjectPrefab;

		// Token: 0x04005170 RID: 20848
		public Dictionary<string, bool> playerCosmeticsLookup = new Dictionary<string, bool>();

		// Token: 0x04005171 RID: 20849
		private float lastHeadRightHandDistance;

		// Token: 0x04005172 RID: 20850
		private float lastHeadLeftHandDistance;

		// Token: 0x04005173 RID: 20851
		private float pauseTime;

		// Token: 0x04005174 RID: 20852
		private float disconnectTime = 120f;

		// Token: 0x04005175 RID: 20853
		public bool disableAFKKick;

		// Token: 0x04005176 RID: 20854
		private float headRightHandDistance;

		// Token: 0x04005177 RID: 20855
		private float headLeftHandDistance;

		// Token: 0x04005178 RID: 20856
		private Quaternion headQuat;

		// Token: 0x04005179 RID: 20857
		private Quaternion lastHeadQuat;

		// Token: 0x0400517A RID: 20858
		public GameObject[] disableOnStartup;

		// Token: 0x0400517B RID: 20859
		public GameObject[] enableOnStartup;

		// Token: 0x0400517C RID: 20860
		public bool updatedName;

		// Token: 0x0400517D RID: 20861
		private int[] playersInRegion;

		// Token: 0x0400517E RID: 20862
		private int[] pingInRegion;

		// Token: 0x0400517F RID: 20863
		public List<string> friendIDList = new List<string>();

		// Token: 0x04005180 RID: 20864
		private JoinType currentJoinType;

		// Token: 0x04005181 RID: 20865
		private string friendToFollow;

		// Token: 0x04005182 RID: 20866
		private string keyToFollow;

		// Token: 0x04005183 RID: 20867
		public string shuffler;

		// Token: 0x04005184 RID: 20868
		public string keyStr;

		// Token: 0x04005185 RID: 20869
		private string platformTag = "OTHER";

		// Token: 0x04005186 RID: 20870
		private string startLevel;

		// Token: 0x04005187 RID: 20871
		[SerializeField]
		private GTZone startZone;

		// Token: 0x04005188 RID: 20872
		private GorillaGeoHideShowTrigger startGeoTrigger;

		// Token: 0x04005189 RID: 20873
		public GorillaNetworkJoinTrigger privateTrigger;

		// Token: 0x0400518A RID: 20874
		internal string initialGameMode = "";

		// Token: 0x0400518B RID: 20875
		public GorillaNetworkJoinTrigger currentJoinTrigger;

		// Token: 0x0400518C RID: 20876
		public string autoJoinRoom;

		// Token: 0x0400518D RID: 20877
		private bool deferredJoin;

		// Token: 0x0400518E RID: 20878
		private float partyJoinDeferredUntilTimestamp;

		// Token: 0x0400518F RID: 20879
		private DateTime? timeWhenApplicationPaused;

		// Token: 0x04005190 RID: 20880
		[NetworkPrefab]
		[SerializeField]
		private NetworkObject testPlayerPrefab;

		// Token: 0x04005191 RID: 20881
		private List<GorillaNetworkJoinTrigger> allJoinTriggers = new List<GorillaNetworkJoinTrigger>();
	}
}
