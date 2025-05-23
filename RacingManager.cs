using System;
using System.Collections.Generic;
using System.Text;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class RacingManager : NetworkSceneObject, ITickSystemTick
{
	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000BCB RID: 3019 RVA: 0x0003E759 File Offset: 0x0003C959
	// (set) Token: 0x06000BCC RID: 3020 RVA: 0x0003E760 File Offset: 0x0003C960
	public static RacingManager instance { get; private set; }

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000BCD RID: 3021 RVA: 0x0003E768 File Offset: 0x0003C968
	// (set) Token: 0x06000BCE RID: 3022 RVA: 0x0003E770 File Offset: 0x0003C970
	public bool TickRunning { get; set; }

	// Token: 0x06000BCF RID: 3023 RVA: 0x0003E77C File Offset: 0x0003C97C
	private void Awake()
	{
		RacingManager.instance = this;
		HashSet<int> hashSet = new HashSet<int>();
		this.races = new RacingManager.Race[this.raceSetups.Length];
		for (int i = 0; i < this.raceSetups.Length; i++)
		{
			this.races[i] = new RacingManager.Race(i, this.raceSetups[i], hashSet, this.photonView);
		}
		RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.OnRoomJoin));
		RoomSystem.PlayerJoinedEvent = (Action<NetPlayer>)Delegate.Combine(RoomSystem.PlayerJoinedEvent, new Action<NetPlayer>(this.OnPlayerJoined));
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x0003E81C File Offset: 0x0003CA1C
	protected override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		TickSystem<object>.AddTickCallback(this);
		base.OnEnable();
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x0003E830 File Offset: 0x0003CA30
	protected override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		TickSystem<object>.RemoveTickCallback(this);
		base.OnDisable();
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x0003E844 File Offset: 0x0003CA44
	private void OnRoomJoin()
	{
		for (int i = 0; i < this.races.Length; i++)
		{
			this.races[i].Clear();
		}
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x0003E874 File Offset: 0x0003CA74
	private void OnPlayerJoined(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		for (int i = 0; i < this.races.Length; i++)
		{
			this.races[i].SendStateToNewPlayer(player);
		}
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x0003E8B0 File Offset: 0x0003CAB0
	public void RegisterVisual(RaceVisual visual)
	{
		int raceId = visual.raceId;
		if (raceId >= 0 && raceId < this.races.Length)
		{
			this.races[raceId].RegisterVisual(visual);
		}
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x0003E8E1 File Offset: 0x0003CAE1
	public void Button_StartRace(int raceId, int laps)
	{
		if (raceId >= 0 && raceId < this.races.Length)
		{
			this.races[raceId].Button_StartRace(laps);
		}
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x0003E900 File Offset: 0x0003CB00
	[PunRPC]
	private void RequestRaceStart_RPC(int raceId, int laps, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestRaceStart_RPC");
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (laps != 1 && laps != 3 && laps != 5)
		{
			return;
		}
		if (raceId >= 0 && raceId < this.races.Length)
		{
			this.races[raceId].Host_RequestRaceStart(laps, info.Sender.ActorNumber);
		}
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x0003E958 File Offset: 0x0003CB58
	[PunRPC]
	private void RaceBeginCountdown_RPC(byte raceId, byte laps, double startTime, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RaceBeginCountdown_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (laps != 1 && laps != 3 && laps != 5)
		{
			return;
		}
		if (!double.IsFinite(startTime))
		{
			return;
		}
		if (startTime < PhotonNetwork.Time || startTime > PhotonNetwork.Time + 4.0)
		{
			return;
		}
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].BeginCountdown(startTime, (int)laps);
		}
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x0003E9D0 File Offset: 0x0003CBD0
	[PunRPC]
	private void RaceLockInParticipants_RPC(byte raceId, int[] participantActorNumbers, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RaceLockInParticipants_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (participantActorNumbers.Length > 10)
		{
			return;
		}
		for (int i = 1; i < participantActorNumbers.Length; i++)
		{
			if (participantActorNumbers[i] <= participantActorNumbers[i - 1])
			{
				return;
			}
		}
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].LockInParticipants(participantActorNumbers, false);
		}
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x0003EA35 File Offset: 0x0003CC35
	public void OnCheckpointPassed(int raceId, int checkpointIndex)
	{
		this.photonView.RPC("PassCheckpoint_RPC", RpcTarget.All, new object[]
		{
			(byte)raceId,
			(byte)checkpointIndex
		});
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x0003EA62 File Offset: 0x0003CC62
	[PunRPC]
	private void PassCheckpoint_RPC(byte raceId, byte checkpointIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "PassCheckpoint_RPC");
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].PassCheckpoint(info.Sender, (int)checkpointIndex, info.SentServerTime);
		}
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x0003EA99 File Offset: 0x0003CC99
	[PunRPC]
	private void RaceEnded_RPC(byte raceId, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RaceEnded_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (raceId >= 0 && (int)raceId < this.races.Length)
		{
			this.races[(int)raceId].RaceEnded();
		}
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x0003EAD0 File Offset: 0x0003CCD0
	void ITickSystemTick.Tick()
	{
		for (int i = 0; i < this.races.Length; i++)
		{
			this.races[i].Tick();
		}
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x0003EB00 File Offset: 0x0003CD00
	public bool IsActorLockedIntoAnyRace(int actorNumber)
	{
		for (int i = 0; i < this.races.Length; i++)
		{
			if (this.races[i].IsActorLockedIntoRace(actorNumber))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000E59 RID: 3673
	[SerializeField]
	private RacingManager.RaceSetup[] raceSetups;

	// Token: 0x04000E5A RID: 3674
	private const int MinPlayersInRace = 1;

	// Token: 0x04000E5B RID: 3675
	private const float ResultsDuration = 10f;

	// Token: 0x04000E5C RID: 3676
	private RacingManager.Race[] races;

	// Token: 0x020001FD RID: 509
	[Serializable]
	private struct RaceSetup
	{
		// Token: 0x04000E5D RID: 3677
		public BoxCollider startVolume;

		// Token: 0x04000E5E RID: 3678
		public int numCheckpoints;

		// Token: 0x04000E5F RID: 3679
		public float dqBaseDuration;

		// Token: 0x04000E60 RID: 3680
		public float dqInterval;
	}

	// Token: 0x020001FE RID: 510
	private struct RacerData
	{
		// Token: 0x04000E61 RID: 3681
		public int actorNumber;

		// Token: 0x04000E62 RID: 3682
		public string playerName;

		// Token: 0x04000E63 RID: 3683
		public int numCheckpointsPassed;

		// Token: 0x04000E64 RID: 3684
		public double latestCheckpointTime;

		// Token: 0x04000E65 RID: 3685
		public bool isDisqualified;
	}

	// Token: 0x020001FF RID: 511
	private class RacerComparer : IComparer<RacingManager.RacerData>
	{
		// Token: 0x06000BDF RID: 3039 RVA: 0x0003EB3C File Offset: 0x0003CD3C
		public int Compare(RacingManager.RacerData a, RacingManager.RacerData b)
		{
			int num = a.isDisqualified.CompareTo(b.isDisqualified);
			if (num != 0)
			{
				return num;
			}
			int num2 = a.numCheckpointsPassed.CompareTo(b.numCheckpointsPassed);
			if (num2 != 0)
			{
				return -num2;
			}
			if (a.numCheckpointsPassed > 0)
			{
				return a.latestCheckpointTime.CompareTo(b.latestCheckpointTime);
			}
			return a.actorNumber.CompareTo(b.actorNumber);
		}

		// Token: 0x04000E66 RID: 3686
		public static RacingManager.RacerComparer instance = new RacingManager.RacerComparer();
	}

	// Token: 0x02000200 RID: 512
	public enum RacingState
	{
		// Token: 0x04000E68 RID: 3688
		Inactive,
		// Token: 0x04000E69 RID: 3689
		Countdown,
		// Token: 0x04000E6A RID: 3690
		InProgress,
		// Token: 0x04000E6B RID: 3691
		Results
	}

	// Token: 0x02000201 RID: 513
	private class Race
	{
		// Token: 0x06000BE2 RID: 3042 RVA: 0x0003EBB4 File Offset: 0x0003CDB4
		public Race(int raceIndex, RacingManager.RaceSetup setup, HashSet<int> actorsInAnyRace, PhotonView photonView)
		{
			this.raceIndex = raceIndex;
			this.numCheckpoints = setup.numCheckpoints;
			this.raceStartZone = setup.startVolume;
			this.dqBaseDuration = setup.dqBaseDuration;
			this.dqInterval = setup.dqInterval;
			this.photonView = photonView;
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000BE3 RID: 3043 RVA: 0x0003EC46 File Offset: 0x0003CE46
		// (set) Token: 0x06000BE4 RID: 3044 RVA: 0x0003EC4E File Offset: 0x0003CE4E
		public RacingManager.RacingState racingState { get; private set; }

		// Token: 0x06000BE5 RID: 3045 RVA: 0x0003EC57 File Offset: 0x0003CE57
		public void RegisterVisual(RaceVisual visual)
		{
			this.raceVisual = visual;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x0003EC60 File Offset: 0x0003CE60
		public void Clear()
		{
			this.hasLockedInParticipants = false;
			this.racers.Clear();
			this.playerLookup.Clear();
			this.racingState = RacingManager.RacingState.Inactive;
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x0003EC88 File Offset: 0x0003CE88
		public bool IsActorLockedIntoRace(int actorNumber)
		{
			if (this.racingState != RacingManager.RacingState.InProgress || !this.hasLockedInParticipants)
			{
				return false;
			}
			for (int i = 0; i < this.racers.Count; i++)
			{
				if (this.racers[i].actorNumber == actorNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x0003ECD8 File Offset: 0x0003CED8
		public void SendStateToNewPlayer(NetPlayer newPlayer)
		{
			switch (this.racingState)
			{
			case RacingManager.RacingState.Inactive:
			case RacingManager.RacingState.Results:
				return;
			case RacingManager.RacingState.Countdown:
				this.photonView.RPC("RaceBeginCountdown_RPC", RpcTarget.All, new object[]
				{
					(byte)this.raceIndex,
					(byte)this.numLapsSelected,
					this.raceStartTime
				});
				return;
			case RacingManager.RacingState.InProgress:
				return;
			default:
				return;
			}
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x0003ED48 File Offset: 0x0003CF48
		public void Tick()
		{
			if (Time.time >= this.nextTickTimestamp)
			{
				this.nextTickTimestamp = Time.time + this.TickWithNextDelay();
			}
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x0003ED6C File Offset: 0x0003CF6C
		public float TickWithNextDelay()
		{
			bool flag = this.raceVisual != null;
			if (flag)
			{
				this.raceVisual.ActivateStartingWall(this.racingState == RacingManager.RacingState.Countdown);
			}
			switch (this.racingState)
			{
			case RacingManager.RacingState.Inactive:
				if (flag)
				{
					this.RefreshStartingPlayerList();
				}
				return 1f;
			case RacingManager.RacingState.Countdown:
				if (this.raceStartTime > PhotonNetwork.Time)
				{
					if (flag)
					{
						this.RefreshStartingPlayerList();
						this.raceVisual.UpdateCountdown(Mathf.CeilToInt((float)(this.raceStartTime - PhotonNetwork.Time)));
					}
				}
				else
				{
					this.RaceCountdownEnds();
				}
				return 0.1f;
			case RacingManager.RacingState.InProgress:
				if (PhotonNetwork.IsMasterClient)
				{
					if (PhotonNetwork.Time > this.abortRaceAtTimestamp)
					{
						this.photonView.RPC("RaceEnded_RPC", RpcTarget.All, new object[] { (byte)this.raceIndex });
					}
					else
					{
						int num = 0;
						for (int i = 0; i < this.racers.Count; i++)
						{
							if (this.racers[i].numCheckpointsPassed < this.numCheckpointsToWin)
							{
								num++;
							}
						}
						if (num == 0)
						{
							this.photonView.RPC("RaceEnded_RPC", RpcTarget.All, new object[] { (byte)this.raceIndex });
						}
					}
				}
				return 1f;
			case RacingManager.RacingState.Results:
				if (Time.time >= this.resultsEndTimestamp)
				{
					if (flag)
					{
						this.raceVisual.OnRaceReset();
					}
					this.racingState = RacingManager.RacingState.Inactive;
				}
				return 1f;
			default:
				return 1f;
			}
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x0003EEE0 File Offset: 0x0003D0E0
		public void RaceEnded()
		{
			if (this.racingState != RacingManager.RacingState.InProgress)
			{
				return;
			}
			this.racingState = RacingManager.RacingState.Results;
			this.resultsEndTimestamp = Time.time + 10f;
			if (this.raceVisual != null)
			{
				this.raceVisual.OnRaceEnded();
			}
			for (int i = 0; i < this.racers.Count; i++)
			{
				RacingManager.RacerData racerData = this.racers[i];
				if (racerData.numCheckpointsPassed < this.numCheckpointsToWin)
				{
					racerData.isDisqualified = true;
					this.racers[i] = racerData;
				}
			}
			this.racers.Sort(RacingManager.RacerComparer.instance);
			this.OnRacerOrderChanged();
			for (int j = 0; j < this.racers.Count; j++)
			{
				if (this.racers[j].actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					VRRig.LocalRig.hoverboardVisual.SetRaceDisplay("");
					VRRig.LocalRig.hoverboardVisual.SetRaceLapsDisplay("");
					return;
				}
			}
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x0003EFE0 File Offset: 0x0003D1E0
		private void RefreshStartingPlayerList()
		{
			if (this.raceVisual != null && this.UpdateActorsInStartZone())
			{
				RacingManager.Race.stringBuilder.Clear();
				RacingManager.Race.stringBuilder.AppendLine("NEXT RACE LINEUP");
				for (int i = 0; i < this.actorsInStartZone.Count; i++)
				{
					RacingManager.Race.stringBuilder.Append("    ");
					RacingManager.Race.stringBuilder.AppendLine(this.playerNamesInStartZone[this.actorsInStartZone[i]]);
				}
				this.raceVisual.SetRaceStartScoreboardText(RacingManager.Race.stringBuilder.ToString(), "");
			}
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x0003F083 File Offset: 0x0003D283
		public void Button_StartRace(int laps)
		{
			if (this.racingState != RacingManager.RacingState.Inactive)
			{
				return;
			}
			this.photonView.RPC("RequestRaceStart_RPC", RpcTarget.MasterClient, new object[] { this.raceIndex, laps });
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x0003F0BC File Offset: 0x0003D2BC
		public void Host_RequestRaceStart(int laps, int requestedByActorNumber)
		{
			if (this.racingState != RacingManager.RacingState.Inactive)
			{
				return;
			}
			this.UpdateActorsInStartZone();
			if (this.actorsInStartZone.Contains(requestedByActorNumber))
			{
				this.photonView.RPC("RaceBeginCountdown_RPC", RpcTarget.All, new object[]
				{
					(byte)this.raceIndex,
					(byte)laps,
					PhotonNetwork.Time + 4.0
				});
			}
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x0003F130 File Offset: 0x0003D330
		public void BeginCountdown(double startTime, int laps)
		{
			if (this.racingState != RacingManager.RacingState.Inactive)
			{
				return;
			}
			this.racingState = RacingManager.RacingState.Countdown;
			this.raceStartTime = startTime;
			this.abortRaceAtTimestamp = startTime + (double)this.dqBaseDuration;
			this.numLapsSelected = laps;
			this.numCheckpointsToWin = this.numCheckpoints * laps + 1;
			this.hasLockedInParticipants = false;
			if (this.raceVisual != null)
			{
				this.raceVisual.OnCountdownStart(laps, (float)(startTime - PhotonNetwork.Time));
			}
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x0003F1A4 File Offset: 0x0003D3A4
		public void RaceCountdownEnds()
		{
			if (this.racingState != RacingManager.RacingState.Countdown)
			{
				return;
			}
			this.racingState = RacingManager.RacingState.InProgress;
			if (this.raceVisual != null)
			{
				this.raceVisual.OnRaceStart();
			}
			this.UpdateActorsInStartZone();
			if (PhotonNetwork.IsMasterClient)
			{
				this.photonView.RPC("RaceLockInParticipants_RPC", RpcTarget.All, new object[]
				{
					(byte)this.raceIndex,
					this.actorsInStartZone.ToArray()
				});
				return;
			}
			if (this.actorsInStartZone.Count >= 1)
			{
				this.LockInParticipants(this.actorsInStartZone.ToArray(), true);
			}
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x0003F240 File Offset: 0x0003D440
		public void LockInParticipants(int[] participantActorNumbers, bool isProvisional = false)
		{
			if (this.hasLockedInParticipants)
			{
				return;
			}
			if (!isProvisional && participantActorNumbers.Length < 1)
			{
				this.racingState = RacingManager.RacingState.Inactive;
				return;
			}
			this.racers.Clear();
			if (participantActorNumbers.Length != 0)
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					int actorNumber = vrrig.OwningNetPlayer.ActorNumber;
					if (participantActorNumbers.BinarySearch(actorNumber) >= 0 && !RacingManager.instance.IsActorLockedIntoAnyRace(actorNumber))
					{
						this.racers.Add(new RacingManager.RacerData
						{
							actorNumber = actorNumber,
							playerName = vrrig.OwningNetPlayer.SanitizedNickName,
							latestCheckpointTime = this.raceStartTime
						});
					}
				}
			}
			if (!isProvisional)
			{
				if (this.racers.Count < 1)
				{
					this.racingState = RacingManager.RacingState.Inactive;
					return;
				}
				this.hasLockedInParticipants = true;
			}
			this.racers.Sort(RacingManager.RacerComparer.instance);
			this.OnRacerOrderChanged();
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x0003F354 File Offset: 0x0003D554
		public void PassCheckpoint(Player player, int checkpointIndex, double time)
		{
			if (this.racingState == RacingManager.RacingState.Inactive)
			{
				return;
			}
			if (time < this.raceStartTime || time < PhotonNetwork.Time - 5.0 || time > PhotonNetwork.Time + 0.10000000149011612)
			{
				return;
			}
			if (this.abortRaceAtTimestamp < time + (double)this.dqInterval)
			{
				this.abortRaceAtTimestamp = time + (double)this.dqInterval;
			}
			RacingManager.RacerData racerData = default(RacingManager.RacerData);
			int i = 0;
			while (i < this.racers.Count)
			{
				racerData = this.racers[i];
				if (racerData.actorNumber == player.ActorNumber)
				{
					if (racerData.numCheckpointsPassed >= this.numCheckpointsToWin || racerData.isDisqualified)
					{
						return;
					}
					if (checkpointIndex != racerData.numCheckpointsPassed % this.numCheckpoints)
					{
						return;
					}
					RigContainer rigContainer;
					if (this.raceVisual != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer) && !this.raceVisual.IsPlayerNearCheckpoint(rigContainer.Rig, checkpointIndex))
					{
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			if (racerData.actorNumber != player.ActorNumber)
			{
				return;
			}
			racerData.numCheckpointsPassed++;
			racerData.latestCheckpointTime = time;
			this.racers[i] = racerData;
			if (racerData.numCheckpointsPassed >= this.numCheckpointsToWin || (i > 0 && RacingManager.RacerComparer.instance.Compare(this.racers[i - 1], racerData) > 0))
			{
				this.racers.Sort(RacingManager.RacerComparer.instance);
				this.OnRacerOrderChanged();
			}
			if (player.IsLocal)
			{
				if (checkpointIndex == this.numCheckpoints - 1)
				{
					int num = racerData.numCheckpointsPassed / this.numCheckpoints + 1;
					if (num > this.numLapsSelected)
					{
						this.raceVisual.ShowFinishLineText("FINISH");
						this.raceVisual.EnableRaceEndSound();
						return;
					}
					if (num == this.numLapsSelected)
					{
						this.raceVisual.ShowFinishLineText("FINAL LAP");
						return;
					}
					this.raceVisual.ShowFinishLineText("NEXT LAP");
					return;
				}
				else if (checkpointIndex == 0)
				{
					int num2 = racerData.numCheckpointsPassed / this.numCheckpoints + 1;
					if (num2 > this.numLapsSelected)
					{
						VRRig.LocalRig.hoverboardVisual.SetRaceLapsDisplay("");
						return;
					}
					VRRig.LocalRig.hoverboardVisual.SetRaceLapsDisplay(string.Format("LAP {0}/{1}", num2, this.numLapsSelected));
				}
			}
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x0003F598 File Offset: 0x0003D798
		private void OnRacerOrderChanged()
		{
			if (this.raceVisual != null)
			{
				RacingManager.Race.stringBuilder.Clear();
				RacingManager.Race.timesStringBuilder.Clear();
				RacingManager.Race.timesStringBuilder.AppendLine("");
				bool flag = false;
				switch (this.racingState)
				{
				case RacingManager.RacingState.Inactive:
					return;
				case RacingManager.RacingState.Countdown:
					RacingManager.Race.stringBuilder.AppendLine("STARTING LINEUP");
					flag = true;
					break;
				case RacingManager.RacingState.InProgress:
					RacingManager.Race.stringBuilder.AppendLine("RACE LEADERBOARD");
					break;
				case RacingManager.RacingState.Results:
					RacingManager.Race.stringBuilder.AppendLine("RACE RESULTS");
					break;
				}
				for (int i = 0; i < this.racers.Count; i++)
				{
					RacingManager.RacerData racerData = this.racers[i];
					if (racerData.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
					{
						VRRig.LocalRig.hoverboardVisual.SetRaceDisplay(racerData.isDisqualified ? "DQ" : (i + 1).ToString());
					}
					string text = (racerData.isDisqualified ? "DQ. " : (flag ? "    " : ((i + 1).ToString() + ". ")));
					RacingManager.Race.stringBuilder.Append(text);
					if (text.Length <= 3)
					{
						RacingManager.Race.stringBuilder.Append(" ");
					}
					RacingManager.Race.stringBuilder.AppendLine(racerData.playerName);
					if (racerData.isDisqualified)
					{
						RacingManager.Race.timesStringBuilder.AppendLine("--.--");
					}
					else if (racerData.numCheckpointsPassed < this.numCheckpointsToWin)
					{
						RacingManager.Race.timesStringBuilder.AppendLine("");
					}
					else
					{
						RacingManager.Race.timesStringBuilder.AppendLine(string.Format("{0:0.00}", racerData.latestCheckpointTime - this.raceStartTime));
					}
				}
				string text2 = RacingManager.Race.stringBuilder.ToString();
				string text3 = RacingManager.Race.timesStringBuilder.ToString();
				this.raceVisual.SetScoreboardText(text2, text3);
				this.raceVisual.SetRaceStartScoreboardText(text2, text3);
			}
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x0003F7A4 File Offset: 0x0003D9A4
		private bool UpdateActorsInStartZone()
		{
			if (Time.time < this.nextStartZoneUpdateTimestamp)
			{
				return false;
			}
			this.nextStartZoneUpdateTimestamp = Time.time + 0.1f;
			List<int> list = this.actorsInStartZone2;
			List<int> list2 = this.actorsInStartZone;
			this.actorsInStartZone = list;
			this.actorsInStartZone2 = list2;
			this.actorsInStartZone.Clear();
			this.playerNamesInStartZone.Clear();
			int num = Physics.OverlapBoxNonAlloc(this.raceStartZone.transform.position, this.raceStartZone.size / 2f, RacingManager.Race.overlapColliders, this.raceStartZone.transform.rotation, RacingManager.Race.playerLayerMask);
			num = Mathf.Min(num, RacingManager.Race.overlapColliders.Length);
			for (int i = 0; i < num; i++)
			{
				Collider collider = RacingManager.Race.overlapColliders[i];
				if (!(collider == null))
				{
					VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
					int count = this.actorsInStartZone.Count;
					if (!(component == null))
					{
						if (component.isLocal)
						{
							if (NetworkSystem.Instance.LocalPlayer == null)
							{
								RacingManager.Race.overlapColliders[i] = null;
								goto IL_01E2;
							}
							if (RacingManager.instance.IsActorLockedIntoAnyRace(NetworkSystem.Instance.LocalPlayer.ActorNumber))
							{
								goto IL_01E2;
							}
							this.actorsInStartZone.AddSortedUnique(NetworkSystem.Instance.LocalPlayer.ActorNumber);
							if (this.actorsInStartZone.Count > count)
							{
								this.playerNamesInStartZone.Add(NetworkSystem.Instance.LocalPlayer.ActorNumber, component.playerNameVisible);
							}
						}
						else
						{
							if (RacingManager.instance.IsActorLockedIntoAnyRace(component.OwningNetPlayer.ActorNumber))
							{
								goto IL_01E2;
							}
							this.actorsInStartZone.AddSortedUnique(component.OwningNetPlayer.ActorNumber);
							if (this.actorsInStartZone.Count > count)
							{
								this.playerNamesInStartZone.Add(component.OwningNetPlayer.ActorNumber, component.playerNameVisible);
							}
						}
						RacingManager.Race.overlapColliders[i] = null;
					}
				}
				IL_01E2:;
			}
			if (this.actorsInStartZone2.Count != this.actorsInStartZone.Count)
			{
				return true;
			}
			for (int j = 0; j < this.actorsInStartZone.Count; j++)
			{
				if (this.actorsInStartZone[j] != this.actorsInStartZone2[j])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000E6C RID: 3692
		private int raceIndex;

		// Token: 0x04000E6D RID: 3693
		private int numCheckpoints;

		// Token: 0x04000E6E RID: 3694
		private float dqBaseDuration;

		// Token: 0x04000E6F RID: 3695
		private float dqInterval;

		// Token: 0x04000E70 RID: 3696
		private BoxCollider raceStartZone;

		// Token: 0x04000E71 RID: 3697
		private PhotonView photonView;

		// Token: 0x04000E72 RID: 3698
		private List<RacingManager.RacerData> racers = new List<RacingManager.RacerData>(10);

		// Token: 0x04000E73 RID: 3699
		private Dictionary<NetPlayer, int> playerLookup = new Dictionary<NetPlayer, int>();

		// Token: 0x04000E74 RID: 3700
		private List<int> actorsInStartZone = new List<int>();

		// Token: 0x04000E75 RID: 3701
		private List<int> actorsInStartZone2 = new List<int>();

		// Token: 0x04000E76 RID: 3702
		private Dictionary<int, string> playerNamesInStartZone = new Dictionary<int, string>();

		// Token: 0x04000E77 RID: 3703
		private int numLapsSelected = 1;

		// Token: 0x04000E79 RID: 3705
		private double raceStartTime;

		// Token: 0x04000E7A RID: 3706
		private double abortRaceAtTimestamp;

		// Token: 0x04000E7B RID: 3707
		private float resultsEndTimestamp;

		// Token: 0x04000E7C RID: 3708
		private bool isInstanceLoaded;

		// Token: 0x04000E7D RID: 3709
		private int numCheckpointsToWin;

		// Token: 0x04000E7E RID: 3710
		private RaceVisual raceVisual;

		// Token: 0x04000E7F RID: 3711
		private bool hasLockedInParticipants;

		// Token: 0x04000E80 RID: 3712
		private float nextTickTimestamp;

		// Token: 0x04000E81 RID: 3713
		private static StringBuilder stringBuilder = new StringBuilder();

		// Token: 0x04000E82 RID: 3714
		private static StringBuilder timesStringBuilder = new StringBuilder();

		// Token: 0x04000E83 RID: 3715
		private static Collider[] overlapColliders = new Collider[20];

		// Token: 0x04000E84 RID: 3716
		private static int playerLayerMask = UnityLayer.GorillaBodyCollider.ToLayerMask() | UnityLayer.GorillaTagCollider.ToLayerMask();

		// Token: 0x04000E85 RID: 3717
		private float nextStartZoneUpdateTimestamp;
	}
}
