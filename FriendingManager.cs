﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020008CB RID: 2251
public class FriendingManager : MonoBehaviourPun, IPunObservable, IGorillaSliceableSimple
{
	// Token: 0x060036C2 RID: 14018 RVA: 0x00107F49 File Offset: 0x00106149
	private void Awake()
	{
		if (FriendingManager.Instance == null)
		{
			FriendingManager.Instance = this;
			PhotonNetwork.AddCallbackTarget(this);
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060036C3 RID: 14019 RVA: 0x00107F74 File Offset: 0x00106174
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnMultiplayerStarted += this.ValidateState;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.ValidateState;
	}

	// Token: 0x060036C4 RID: 14020 RVA: 0x00107FC4 File Offset: 0x001061C4
	private void OnDestroy()
	{
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
			NetworkSystem.Instance.OnMultiplayerStarted -= this.ValidateState;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.ValidateState;
		}
	}

	// Token: 0x060036C5 RID: 14021 RVA: 0x00017251 File Offset: 0x00015451
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060036C6 RID: 14022 RVA: 0x0001725A File Offset: 0x0001545A
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060036C7 RID: 14023 RVA: 0x00108020 File Offset: 0x00106220
	public void SliceUpdate()
	{
		this.AuthorityUpdate();
	}

	// Token: 0x060036C8 RID: 14024 RVA: 0x00108028 File Offset: 0x00106228
	private void AuthorityUpdate()
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			for (int i = 0; i < this.activeFriendStationData.Count; i++)
			{
				if (this.activeFriendStationData[i].state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.activeFriendStationData[i].state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4)
				{
					FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
					int num = 4;
					float num2 = (Time.time - friendStationData.progressBarStartTime) / this.progressBarDuration;
					if (num2 < 1f)
					{
						int num3 = Mathf.RoundToInt(num2 * (float)num);
						friendStationData.state = num3 + FriendingManager.FriendStationState.ButtonConfirmationTimer0;
					}
					else
					{
						base.photonView.RPC("NotifyClientsFriendRequestReadyRPC", RpcTarget.All, new object[] { friendStationData.zone });
						friendStationData.state = FriendingManager.FriendStationState.WaitingOnRequestBoth;
					}
					this.activeFriendStationData[i] = friendStationData;
				}
			}
		}
	}

	// Token: 0x060036C9 RID: 14025 RVA: 0x00108115 File Offset: 0x00106315
	private void OnPlayerLeftRoom(NetPlayer player)
	{
		this.ValidateState();
	}

	// Token: 0x060036CA RID: 14026 RVA: 0x00108120 File Offset: 0x00106320
	private void ValidateState()
	{
		for (int i = 0; i < this.activeFriendStationData.Count; i++)
		{
			FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
			if (friendStationData.actorNumberA != -1 && NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberA) == null)
			{
				friendStationData.actorNumberA = -1;
			}
			if (friendStationData.actorNumberB != -1 && NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberB) == null)
			{
				friendStationData.actorNumberB = -1;
			}
			if (friendStationData.actorNumberA == -1 || friendStationData.actorNumberB == -1)
			{
				friendStationData.state = FriendingManager.FriendStationState.WaitingForPlayers;
			}
			this.activeFriendStationData[i] = friendStationData;
		}
		this.UpdateFriendingStations();
	}

	// Token: 0x060036CB RID: 14027 RVA: 0x001081C8 File Offset: 0x001063C8
	private void UpdateFriendingStations()
	{
		for (int i = 0; i < this.activeFriendStationData.Count; i++)
		{
			FriendingStation friendingStation;
			if (this.friendingStations.TryGetValue(this.activeFriendStationData[i].zone, out friendingStation))
			{
				friendingStation.UpdateState(this.activeFriendStationData[i]);
			}
		}
	}

	// Token: 0x060036CC RID: 14028 RVA: 0x0010821D File Offset: 0x0010641D
	public void RegisterFriendingStation(FriendingStation friendingStation)
	{
		if (!this.friendingStations.ContainsKey(friendingStation.Zone))
		{
			this.friendingStations.Add(friendingStation.Zone, friendingStation);
		}
	}

	// Token: 0x060036CD RID: 14029 RVA: 0x00108244 File Offset: 0x00106444
	public void UnregisterFriendingStation(FriendingStation friendingStation)
	{
		this.friendingStations.Remove(friendingStation.Zone);
	}

	// Token: 0x060036CE RID: 14030 RVA: 0x00108258 File Offset: 0x00106458
	private void DebugLogFriendingStations()
	{
		string text = string.Format("Friending Stations: Count: {0} ", this.friendingStations.Count);
		foreach (KeyValuePair<GTZone, FriendingStation> keyValuePair in this.friendingStations)
		{
			text += string.Format("Station Zone {0}", keyValuePair.Key);
		}
		Debug.Log(text);
	}

	// Token: 0x060036CF RID: 14031 RVA: 0x001082E4 File Offset: 0x001064E4
	public void PlayerEnteredStation(GTZone zone, NetPlayer netPlayer)
	{
		if (netPlayer != null && netPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.localPlayerZone = zone;
		}
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int num = -1;
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					num = i;
					if (this.activeFriendStationData[i].actorNumberA == -1 && this.activeFriendStationData[i].actorNumberB != netPlayer.ActorNumber)
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.actorNumberA = netPlayer.ActorNumber;
						if (friendStationData.actorNumberA != -1 && friendStationData.actorNumberB != -1)
						{
							friendStationData.state = FriendingManager.FriendStationState.WaitingOnFriendStatusBoth;
						}
						else
						{
							friendStationData.state = FriendingManager.FriendStationState.WaitingForPlayers;
						}
						this.activeFriendStationData[i] = friendStationData;
					}
					else if (this.activeFriendStationData[i].actorNumberA != -1 && this.activeFriendStationData[i].actorNumberA != netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberB == -1)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.actorNumberB = netPlayer.ActorNumber;
						if (friendStationData2.actorNumberA != -1 && friendStationData2.actorNumberB != -1)
						{
							friendStationData2.state = FriendingManager.FriendStationState.WaitingOnFriendStatusBoth;
						}
						else
						{
							friendStationData2.state = FriendingManager.FriendStationState.WaitingForPlayers;
						}
						this.activeFriendStationData[i] = friendStationData2;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusBoth)
					{
						base.photonView.RPC("CheckFriendStatusRequestRPC", RpcTarget.All, new object[]
						{
							this.activeFriendStationData[i].zone,
							this.activeFriendStationData[i].actorNumberA,
							this.activeFriendStationData[i].actorNumberB
						});
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			if (num < 0)
			{
				this.activeFriendStationData.Add(new FriendingManager.FriendStationData
				{
					zone = zone,
					actorNumberA = netPlayer.ActorNumber,
					actorNumberB = -1,
					state = FriendingManager.FriendStationState.WaitingForPlayers
				});
			}
			this.UpdateFriendingStations();
		}
	}

	// Token: 0x060036D0 RID: 14032 RVA: 0x00108528 File Offset: 0x00106728
	public void PlayerExitedStation(GTZone zone, NetPlayer netPlayer)
	{
		if (netPlayer != null && netPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.localPlayerZone = GTZone.none;
		}
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int num = -1;
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if ((this.activeFriendStationData[i].actorNumberA == netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberB == -1) || (this.activeFriendStationData[i].actorNumberA == -1 && this.activeFriendStationData[i].actorNumberB == netPlayer.ActorNumber))
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.actorNumberA = -1;
						friendStationData.actorNumberB = -1;
						friendStationData.state = FriendingManager.FriendStationState.WaitingForPlayers;
						this.activeFriendStationData[i] = friendStationData;
						num = i;
						break;
					}
					if (this.activeFriendStationData[i].actorNumberA != -1 && this.activeFriendStationData[i].actorNumberA != netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberB == netPlayer.ActorNumber)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.actorNumberB = -1;
						friendStationData2.state = FriendingManager.FriendStationState.WaitingForPlayers;
						this.activeFriendStationData[i] = friendStationData2;
						break;
					}
					if (this.activeFriendStationData[i].actorNumberB != -1 && this.activeFriendStationData[i].actorNumberB != netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberA == netPlayer.ActorNumber)
					{
						FriendingManager.FriendStationData friendStationData3 = this.activeFriendStationData[i];
						friendStationData3.actorNumberA = -1;
						friendStationData3.state = FriendingManager.FriendStationState.WaitingForPlayers;
						this.activeFriendStationData[i] = friendStationData3;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			this.UpdateFriendingStations();
			if (num >= 0)
			{
				base.photonView.RPC("StationNoLongerActiveRPC", RpcTarget.Others, new object[] { this.activeFriendStationData[num].zone });
				this.activeFriendStationData.RemoveAt(num);
			}
		}
	}

	// Token: 0x060036D1 RID: 14033 RVA: 0x00108768 File Offset: 0x00106968
	private void PlayerPressedButton(GTZone zone, int playerId)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (this.activeFriendStationData[i].actorNumberA == -1 || this.activeFriendStationData[i].actorNumberB == -1)
					{
						break;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA && this.activeFriendStationData[i].actorNumberA == playerId) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB && this.activeFriendStationData[i].actorNumberB == playerId))
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.state = FriendingManager.FriendStationState.ButtonConfirmationTimer0;
						friendStationData.progressBarStartTime = Time.time;
						this.activeFriendStationData[i] = friendStationData;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonBoth && this.activeFriendStationData[i].actorNumberA == playerId)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerB;
						this.activeFriendStationData[i] = friendStationData2;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonBoth && this.activeFriendStationData[i].actorNumberB == playerId)
					{
						FriendingManager.FriendStationData friendStationData3 = this.activeFriendStationData[i];
						friendStationData3.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerA;
						this.activeFriendStationData[i] = friendStationData3;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060036D2 RID: 14034 RVA: 0x00108908 File Offset: 0x00106B08
	private void PlayerUnpressedButton(GTZone zone, int playerId)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (this.activeFriendStationData[i].actorNumberA == -1 || this.activeFriendStationData[i].actorNumberB == -1)
					{
						break;
					}
					bool flag = this.activeFriendStationData[i].state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.activeFriendStationData[i].state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4;
					if (flag && this.activeFriendStationData[i].actorNumberA == playerId)
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerA;
						this.activeFriendStationData[i] = friendStationData;
						return;
					}
					if (flag && this.activeFriendStationData[i].actorNumberB == playerId)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerB;
						this.activeFriendStationData[i] = friendStationData2;
						return;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA && this.activeFriendStationData[i].actorNumberB == playerId) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB && this.activeFriendStationData[i].actorNumberA == playerId))
					{
						FriendingManager.FriendStationData friendStationData3 = this.activeFriendStationData[i];
						friendStationData3.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						this.activeFriendStationData[i] = friendStationData3;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060036D3 RID: 14035 RVA: 0x00108AA9 File Offset: 0x00106CA9
	private void CheckFriendStatusRequest(GTZone zone, int actorNumberA, int actorNumberB)
	{
		FriendSystem.Instance.OnFriendListRefresh -= this.CheckFriendStatusOnFriendListRefresh;
		FriendSystem.Instance.OnFriendListRefresh += this.CheckFriendStatusOnFriendListRefresh;
		FriendSystem.Instance.RefreshFriendsList();
	}

	// Token: 0x060036D4 RID: 14036 RVA: 0x00108AE8 File Offset: 0x00106CE8
	private void CheckFriendStatusOnFriendListRefresh(List<FriendBackendController.Friend> friendList)
	{
		FriendSystem.Instance.OnFriendListRefresh -= this.CheckFriendStatusOnFriendListRefresh;
		int i = 0;
		while (i < this.activeFriendStationData.Count)
		{
			if (this.activeFriendStationData[i].zone == this.localPlayerZone)
			{
				int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
				int num = -1;
				if (this.activeFriendStationData[i].actorNumberA == actorNumber)
				{
					num = this.activeFriendStationData[i].actorNumberB;
				}
				else if (this.activeFriendStationData[i].actorNumberB == actorNumber)
				{
					num = this.activeFriendStationData[i].actorNumberA;
				}
				if (num != -1 && FriendSystem.Instance.CheckFriendshipWithPlayer(num))
				{
					base.photonView.RPC("CheckFriendStatusResponseRPC", RpcTarget.MasterClient, new object[] { this.localPlayerZone, num, true });
					return;
				}
				base.photonView.RPC("CheckFriendStatusResponseRPC", RpcTarget.MasterClient, new object[] { this.localPlayerZone, num, false });
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x060036D5 RID: 14037 RVA: 0x00108C28 File Offset: 0x00106E28
	private void CheckFriendStatusResponse(GTZone zone, int responderActorNumber, int friendTargetActorNumber, bool friends)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (this.activeFriendStationData[i].actorNumberA == -1 || this.activeFriendStationData[i].actorNumberB == -1)
					{
						break;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerA && this.activeFriendStationData[i].actorNumberA == responderActorNumber) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerB && this.activeFriendStationData[i].actorNumberB == responderActorNumber))
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						if (friends)
						{
							friendStationData.state = FriendingManager.FriendStationState.AlreadyFriends;
						}
						else
						{
							friendStationData.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						}
						this.activeFriendStationData[i] = friendStationData;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusBoth && this.activeFriendStationData[i].actorNumberA == responderActorNumber)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						if (friends)
						{
							friendStationData2.state = FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerB;
						}
						else
						{
							friendStationData2.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						}
						this.activeFriendStationData[i] = friendStationData2;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusBoth && this.activeFriendStationData[i].actorNumberB == responderActorNumber)
					{
						FriendingManager.FriendStationData friendStationData3 = this.activeFriendStationData[i];
						if (friends)
						{
							friendStationData3.state = FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerA;
						}
						else
						{
							friendStationData3.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						}
						this.activeFriendStationData[i] = friendStationData3;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060036D6 RID: 14038 RVA: 0x00108DE4 File Offset: 0x00106FE4
	private void SendFriendRequestIfApplicable(GTZone zone)
	{
		int i = 0;
		while (i < this.activeFriendStationData.Count)
		{
			if (this.activeFriendStationData[i].zone == zone)
			{
				FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				NetPlayer netPlayer = null;
				if (friendStationData.actorNumberA == actorNumber)
				{
					netPlayer = NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberB);
				}
				else if (friendStationData.actorNumberB == actorNumber)
				{
					netPlayer = NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberA);
				}
				if (netPlayer == null)
				{
					return;
				}
				FriendingStation friendingStation;
				if (this.friendingStations.TryGetValue(friendStationData.zone, out friendingStation) && (GTPlayer.Instance.HeadCenterPosition - friendingStation.transform.position).sqrMagnitude < this.requiredProximityToStation * this.requiredProximityToStation)
				{
					FriendSystem.Instance.SendFriendRequest(netPlayer, friendStationData.zone, new FriendSystem.FriendRequestCallback(this.FriendRequestCallback));
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x060036D7 RID: 14039 RVA: 0x00108EE0 File Offset: 0x001070E0
	private void FriendRequestCompletedAuthority(GTZone zone, int playerId, bool succeeded)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (!succeeded)
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.state = FriendingManager.FriendStationState.RequestFailed;
						this.activeFriendStationData[i] = friendStationData;
						return;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestPlayerA && this.activeFriendStationData[i].actorNumberA == playerId) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestPlayerB && this.activeFriendStationData[i].actorNumberB == playerId))
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.state = FriendingManager.FriendStationState.Friends;
						this.activeFriendStationData[i] = friendStationData2;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestBoth && this.activeFriendStationData[i].actorNumberA == playerId)
					{
						FriendingManager.FriendStationData friendStationData3 = this.activeFriendStationData[i];
						friendStationData3.state = FriendingManager.FriendStationState.WaitingOnRequestPlayerB;
						this.activeFriendStationData[i] = friendStationData3;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestBoth && this.activeFriendStationData[i].actorNumberB == playerId)
					{
						FriendingManager.FriendStationData friendStationData4 = this.activeFriendStationData[i];
						friendStationData4.state = FriendingManager.FriendStationState.WaitingOnRequestPlayerA;
						this.activeFriendStationData[i] = friendStationData4;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060036D8 RID: 14040 RVA: 0x00109078 File Offset: 0x00107278
	private void FriendRequestCallback(GTZone zone, int localId, int friendId, bool success)
	{
		if (base.photonView.IsMine)
		{
			this.FriendRequestCompletedAuthority(zone, PhotonNetwork.LocalPlayer.ActorNumber, success);
			return;
		}
		base.photonView.RPC("FriendRequestCompletedRPC", RpcTarget.MasterClient, new object[] { zone, success });
	}

	// Token: 0x060036D9 RID: 14041 RVA: 0x001090D0 File Offset: 0x001072D0
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.activeFriendStationData.Count);
			for (int i = 0; i < this.activeFriendStationData.Count; i++)
			{
				FriendingManager.<OnPhotonSerializeView>g__SendFriendStationData|31_0(stream, this.activeFriendStationData[i]);
			}
		}
		else if (stream.IsReading && info.Sender.IsMasterClient)
		{
			int num = (int)stream.ReceiveNext();
			if (num >= 0 && num <= 10)
			{
				this.activeFriendStationData.Clear();
				for (int j = 0; j < num; j++)
				{
					this.activeFriendStationData.Add(FriendingManager.<OnPhotonSerializeView>g__ReceiveFriendStationData|31_1(stream));
				}
			}
		}
		this.UpdateFriendingStations();
	}

	// Token: 0x060036DA RID: 14042 RVA: 0x00109180 File Offset: 0x00107380
	[PunRPC]
	public void CheckFriendStatusRequestRPC(GTZone zone, int actorNumberA, int actorNumberB, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "CheckFriendStatusRequestRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.CheckFriendStatusRequest(zone, actorNumberA, actorNumberB);
	}

	// Token: 0x060036DB RID: 14043 RVA: 0x001091E8 File Offset: 0x001073E8
	[PunRPC]
	public void CheckFriendStatusResponseRPC(GTZone zone, int friendTargetActorNumber, bool friends, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "CheckFriendStatusRequestRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.CheckFriendStatusResponse(zone, info.Sender.ActorNumber, friendTargetActorNumber, friends);
	}

	// Token: 0x060036DC RID: 14044 RVA: 0x0010925C File Offset: 0x0010745C
	[PunRPC]
	public void FriendButtonPressedRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "FriendButtonPressedRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.PlayerPressedButton(zone, info.Sender.ActorNumber);
	}

	// Token: 0x060036DD RID: 14045 RVA: 0x001092CC File Offset: 0x001074CC
	[PunRPC]
	public void FriendButtonUnpressedRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "FriendButtonUnpressedRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.PlayerUnpressedButton(zone, info.Sender.ActorNumber);
	}

	// Token: 0x060036DE RID: 14046 RVA: 0x0010933C File Offset: 0x0010753C
	[PunRPC]
	public void StationNoLongerActiveRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "StationNoLongerActiveRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		FriendingStation friendingStation;
		if (info.Sender.IsMasterClient && this.friendingStations.TryGetValue(zone, out friendingStation))
		{
			friendingStation.UpdateState(new FriendingManager.FriendStationData
			{
				zone = zone,
				actorNumberA = -1,
				actorNumberB = -1,
				state = FriendingManager.FriendStationState.WaitingForPlayers
			});
		}
	}

	// Token: 0x060036DF RID: 14047 RVA: 0x001093E4 File Offset: 0x001075E4
	[PunRPC]
	public void NotifyClientsFriendRequestReadyRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "NotifyClientsFriendRequestReadyRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.SendFriendRequestIfApplicable(zone);
	}

	// Token: 0x060036E0 RID: 14048 RVA: 0x00109448 File Offset: 0x00107648
	[PunRPC]
	public void FriendRequestCompletedRPC(GTZone zone, bool succeeded, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "FriendRequestCompletedRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.FriendRequestCompletedAuthority(zone, info.Sender.ActorNumber, succeeded);
	}

	// Token: 0x060036E2 RID: 14050 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x060036E3 RID: 14051 RVA: 0x001094F4 File Offset: 0x001076F4
	[CompilerGenerated]
	internal static void <OnPhotonSerializeView>g__SendFriendStationData|31_0(PhotonStream stream, FriendingManager.FriendStationData data)
	{
		stream.SendNext((int)data.zone);
		stream.SendNext(data.actorNumberA);
		stream.SendNext(data.actorNumberB);
		stream.SendNext((int)data.state);
	}

	// Token: 0x060036E4 RID: 14052 RVA: 0x00109548 File Offset: 0x00107748
	[CompilerGenerated]
	internal static FriendingManager.FriendStationData <OnPhotonSerializeView>g__ReceiveFriendStationData|31_1(PhotonStream stream)
	{
		return new FriendingManager.FriendStationData
		{
			zone = (GTZone)((int)stream.ReceiveNext()),
			actorNumberA = (int)stream.ReceiveNext(),
			actorNumberB = (int)stream.ReceiveNext(),
			state = (FriendingManager.FriendStationState)((int)stream.ReceiveNext())
		};
	}

	// Token: 0x04003C59 RID: 15449
	[OnEnterPlay_SetNull]
	public static volatile FriendingManager Instance;

	// Token: 0x04003C5A RID: 15450
	[SerializeField]
	private float progressBarDuration = 3f;

	// Token: 0x04003C5B RID: 15451
	[SerializeField]
	private float requiredProximityToStation = 3f;

	// Token: 0x04003C5C RID: 15452
	private List<FriendingManager.FriendStationData> activeFriendStationData = new List<FriendingManager.FriendStationData>(10);

	// Token: 0x04003C5D RID: 15453
	private Dictionary<GTZone, FriendingStation> friendingStations = new Dictionary<GTZone, FriendingStation>();

	// Token: 0x04003C5E RID: 15454
	private GTZone localPlayerZone = GTZone.none;

	// Token: 0x020008CC RID: 2252
	public enum FriendStationState
	{
		// Token: 0x04003C60 RID: 15456
		NotInRoom,
		// Token: 0x04003C61 RID: 15457
		WaitingForPlayers,
		// Token: 0x04003C62 RID: 15458
		WaitingOnFriendStatusBoth,
		// Token: 0x04003C63 RID: 15459
		WaitingOnFriendStatusPlayerA,
		// Token: 0x04003C64 RID: 15460
		WaitingOnFriendStatusPlayerB,
		// Token: 0x04003C65 RID: 15461
		WaitingOnButtonBoth,
		// Token: 0x04003C66 RID: 15462
		WaitingOnButtonPlayerA,
		// Token: 0x04003C67 RID: 15463
		WaitingOnButtonPlayerB,
		// Token: 0x04003C68 RID: 15464
		ButtonConfirmationTimer0,
		// Token: 0x04003C69 RID: 15465
		ButtonConfirmationTimer1,
		// Token: 0x04003C6A RID: 15466
		ButtonConfirmationTimer2,
		// Token: 0x04003C6B RID: 15467
		ButtonConfirmationTimer3,
		// Token: 0x04003C6C RID: 15468
		ButtonConfirmationTimer4,
		// Token: 0x04003C6D RID: 15469
		WaitingOnRequestBoth,
		// Token: 0x04003C6E RID: 15470
		WaitingOnRequestPlayerA,
		// Token: 0x04003C6F RID: 15471
		WaitingOnRequestPlayerB,
		// Token: 0x04003C70 RID: 15472
		RequestFailed,
		// Token: 0x04003C71 RID: 15473
		Friends,
		// Token: 0x04003C72 RID: 15474
		AlreadyFriends
	}

	// Token: 0x020008CD RID: 2253
	public struct FriendStationData
	{
		// Token: 0x04003C73 RID: 15475
		public GTZone zone;

		// Token: 0x04003C74 RID: 15476
		public int actorNumberA;

		// Token: 0x04003C75 RID: 15477
		public int actorNumberB;

		// Token: 0x04003C76 RID: 15478
		public FriendingManager.FriendStationState state;

		// Token: 0x04003C77 RID: 15479
		public float progressBarStartTime;
	}
}
