using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000B23 RID: 2851
	public class PlayerTimerManager : MonoBehaviourPunCallbacks
	{
		// Token: 0x06004637 RID: 17975 RVA: 0x0014D880 File Offset: 0x0014BA80
		private void Awake()
		{
			if (PlayerTimerManager.instance == null)
			{
				PlayerTimerManager.instance = this;
			}
			else if (PlayerTimerManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.callLimiters = new CallLimiter[2];
			this.callLimiters[0] = new CallLimiter(10, 1f, 0.5f);
			this.callLimiters[1] = new CallLimiter(30, 1f, 0.5f);
			this.playerTimerData = new Dictionary<int, PlayerTimerManager.PlayerTimerData>(10);
			this.timerToggleLimiters = new Dictionary<int, CallLimiter>(10);
			this.limiterPool = new List<CallLimiter>(10);
			this.serializedTimerData = new byte[256];
		}

		// Token: 0x06004638 RID: 17976 RVA: 0x0014D930 File Offset: 0x0014BB30
		private CallLimiter CreateLimiterFromPool()
		{
			if (this.limiterPool.Count > 0)
			{
				CallLimiter callLimiter = this.limiterPool[this.limiterPool.Count - 1];
				this.limiterPool.RemoveAt(this.limiterPool.Count - 1);
				return callLimiter;
			}
			return new CallLimiter(5, 1f, 0.5f);
		}

		// Token: 0x06004639 RID: 17977 RVA: 0x0014D98C File Offset: 0x0014BB8C
		private void ReturnCallLimiterToPool(CallLimiter limiter)
		{
			if (limiter == null)
			{
				return;
			}
			limiter.Reset();
			this.limiterPool.Add(limiter);
		}

		// Token: 0x0600463A RID: 17978 RVA: 0x0014D9A4 File Offset: 0x0014BBA4
		public void RegisterTimerBoard(PlayerTimerBoard board)
		{
			if (!PlayerTimerManager.timerBoards.Contains(board))
			{
				PlayerTimerManager.timerBoards.Add(board);
				this.UpdateTimerBoard(board);
			}
		}

		// Token: 0x0600463B RID: 17979 RVA: 0x0014D9C5 File Offset: 0x0014BBC5
		public void UnregisterTimerBoard(PlayerTimerBoard board)
		{
			PlayerTimerManager.timerBoards.Remove(board);
		}

		// Token: 0x0600463C RID: 17980 RVA: 0x0014D9D4 File Offset: 0x0014BBD4
		public bool IsLocalTimerStarted()
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			return this.playerTimerData.TryGetValue(NetworkSystem.Instance.LocalPlayer.ActorNumber, out playerTimerData) && playerTimerData.isStarted;
		}

		// Token: 0x0600463D RID: 17981 RVA: 0x0014DA08 File Offset: 0x0014BC08
		public float GetTimeForPlayer(int actorNumber)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (!this.playerTimerData.TryGetValue(actorNumber, out playerTimerData))
			{
				return 0f;
			}
			if (playerTimerData.isStarted)
			{
				return Mathf.Clamp((PhotonNetwork.ServerTimestamp - playerTimerData.startTimeStamp) / 1000f, 0f, 3599.99f);
			}
			return Mathf.Clamp(playerTimerData.lastTimerDuration / 1000f, 0f, 3599.99f);
		}

		// Token: 0x0600463E RID: 17982 RVA: 0x0014DA74 File Offset: 0x0014BC74
		public float GetLastDurationForPlayer(int actorNumber)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (this.playerTimerData.TryGetValue(actorNumber, out playerTimerData))
			{
				return Mathf.Clamp(playerTimerData.lastTimerDuration / 1000f, 0f, 3599.99f);
			}
			return -1f;
		}

		// Token: 0x0600463F RID: 17983 RVA: 0x0014DAB4 File Offset: 0x0014BCB4
		[PunRPC]
		private void InitTimersMasterRPC(int numBytes, byte[] bytes, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "InitTimersMasterRPC");
			if (!this.ValidateCallLimits(PlayerTimerManager.RPC.InitTimersMaster, info))
			{
				return;
			}
			if (this.areTimersInitialized)
			{
				return;
			}
			this.DeserializeTimerState(bytes.Length, bytes);
			this.areTimersInitialized = true;
			this.UpdateAllTimerBoards();
		}

		// Token: 0x06004640 RID: 17984 RVA: 0x0014DB08 File Offset: 0x0014BD08
		private int SerializeTimerState()
		{
			Array.Clear(this.serializedTimerData, 0, this.serializedTimerData.Length);
			MemoryStream memoryStream = new MemoryStream(this.serializedTimerData);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			if (this.playerTimerData.Count > 10)
			{
				this.ClearOldPlayerData();
			}
			binaryWriter.Write(this.playerTimerData.Count);
			foreach (KeyValuePair<int, PlayerTimerManager.PlayerTimerData> keyValuePair in this.playerTimerData)
			{
				binaryWriter.Write(keyValuePair.Key);
				binaryWriter.Write(keyValuePair.Value.startTimeStamp);
				binaryWriter.Write(keyValuePair.Value.endTimeStamp);
				binaryWriter.Write(keyValuePair.Value.isStarted ? 1 : 0);
				binaryWriter.Write(keyValuePair.Value.lastTimerDuration);
			}
			return (int)memoryStream.Position;
		}

		// Token: 0x06004641 RID: 17985 RVA: 0x0014DC04 File Offset: 0x0014BE04
		private void DeserializeTimerState(int numBytes, byte[] bytes)
		{
			if (numBytes <= 0 || numBytes > 256)
			{
				return;
			}
			if (bytes == null || bytes.Length < numBytes)
			{
				return;
			}
			MemoryStream memoryStream = new MemoryStream(bytes);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			this.playerTimerData.Clear();
			try
			{
				List<Player> list = PhotonNetwork.PlayerList.ToList<Player>();
				if (bytes.Length < 4)
				{
					this.playerTimerData.Clear();
					return;
				}
				int num = binaryReader.ReadInt32();
				if (num < 0 || num > 10)
				{
					this.playerTimerData.Clear();
					return;
				}
				int num2 = 17;
				if (memoryStream.Position + (long)(num2 * num) > (long)bytes.Length)
				{
					this.playerTimerData.Clear();
					return;
				}
				for (int i = 0; i < num; i++)
				{
					int actorNum = binaryReader.ReadInt32();
					int num3 = binaryReader.ReadInt32();
					int num4 = binaryReader.ReadInt32();
					bool flag = binaryReader.ReadByte() > 0;
					uint num5 = binaryReader.ReadUInt32();
					if (list.FindIndex((Player x) => x.ActorNumber == actorNum) >= 0)
					{
						PlayerTimerManager.PlayerTimerData playerTimerData = new PlayerTimerManager.PlayerTimerData
						{
							startTimeStamp = num3,
							endTimeStamp = num4,
							isStarted = flag,
							lastTimerDuration = num5
						};
						this.playerTimerData.TryAdd(actorNum, playerTimerData);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				this.playerTimerData.Clear();
			}
			if (Time.time - this.requestSendTime < 5f && this.IsLocalTimerStarted() != this.localPlayerRequestedStart)
			{
				this.timerPV.RPC("RequestTimerToggleRPC", RpcTarget.MasterClient, new object[] { this.localPlayerRequestedStart });
			}
		}

		// Token: 0x06004642 RID: 17986 RVA: 0x0014DDC8 File Offset: 0x0014BFC8
		private void ClearOldPlayerData()
		{
			List<int> list = new List<int>(this.playerTimerData.Count);
			List<Player> list2 = PhotonNetwork.PlayerList.ToList<Player>();
			using (Dictionary<int, PlayerTimerManager.PlayerTimerData>.KeyCollection.Enumerator enumerator = this.playerTimerData.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int actorNum = enumerator.Current;
					if (list2.FindIndex((Player x) => x.ActorNumber == actorNum) < 0)
					{
						list.Add(actorNum);
					}
				}
			}
			foreach (int num in list)
			{
				this.playerTimerData.Remove(num);
			}
		}

		// Token: 0x06004643 RID: 17987 RVA: 0x0014DEA8 File Offset: 0x0014C0A8
		public void RequestTimerToggle(bool startTimer)
		{
			this.requestSendTime = Time.time;
			this.localPlayerRequestedStart = startTimer;
			this.timerPV.RPC("RequestTimerToggleRPC", RpcTarget.MasterClient, new object[] { startTimer });
		}

		// Token: 0x06004644 RID: 17988 RVA: 0x0014DEDC File Offset: 0x0014C0DC
		[PunRPC]
		private void RequestTimerToggleRPC(bool startTimer, PhotonMessageInfo info)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "RequestTimerToggleRPC");
			CallLimiter callLimiter;
			if (this.timerToggleLimiters.TryGetValue(info.Sender.ActorNumber, out callLimiter))
			{
				if (!callLimiter.CheckCallTime(Time.time))
				{
				}
			}
			else
			{
				CallLimiter callLimiter2 = this.CreateLimiterFromPool();
				this.timerToggleLimiters.Add(info.Sender.ActorNumber, callLimiter2);
				callLimiter2.CheckCallTime(Time.time);
			}
			if (info.Sender == null)
			{
				return;
			}
			PlayerTimerManager.PlayerTimerData playerTimerData;
			bool flag = this.playerTimerData.TryGetValue(info.Sender.ActorNumber, out playerTimerData);
			if (!startTimer && !flag)
			{
				return;
			}
			if (flag && !startTimer && !playerTimerData.isStarted)
			{
				return;
			}
			int num = info.SentServerTimestamp;
			if (PhotonNetwork.ServerTimestamp - num > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num = PhotonNetwork.ServerTimestamp - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			this.timerPV.RPC("TimerToggledMasterRPC", RpcTarget.All, new object[] { startTimer, num, info.Sender });
		}

		// Token: 0x06004645 RID: 17989 RVA: 0x0014DFF4 File Offset: 0x0014C1F4
		[PunRPC]
		private void TimerToggledMasterRPC(bool startTimer, int toggleTimeStamp, Player player, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "TimerToggledMasterRPC");
			if (!this.ValidateCallLimits(PlayerTimerManager.RPC.ToggleTimerMaster, info))
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			if (!this.areTimersInitialized)
			{
				return;
			}
			int num = toggleTimeStamp;
			int num2 = info.SentServerTimestamp;
			if (PhotonNetwork.ServerTimestamp - num2 > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num2 = PhotonNetwork.ServerTimestamp - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			if (num2 - num > PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout)
			{
				num = num2 - PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout;
			}
			this.OnToggleTimerForPlayer(startTimer, player, num);
		}

		// Token: 0x06004646 RID: 17990 RVA: 0x0014E09C File Offset: 0x0014C29C
		private void OnToggleTimerForPlayer(bool startTimer, Player player, int toggleTime)
		{
			PlayerTimerManager.PlayerTimerData playerTimerData;
			if (this.playerTimerData.TryGetValue(player.ActorNumber, out playerTimerData))
			{
				if (startTimer && !playerTimerData.isStarted)
				{
					playerTimerData.startTimeStamp = toggleTime;
					playerTimerData.isStarted = true;
					UnityEvent<int> onTimerStartedForPlayer = this.OnTimerStartedForPlayer;
					if (onTimerStartedForPlayer != null)
					{
						onTimerStartedForPlayer.Invoke(player.ActorNumber);
					}
					if (player.IsLocal)
					{
						UnityEvent onLocalTimerStarted = this.OnLocalTimerStarted;
						if (onLocalTimerStarted != null)
						{
							onLocalTimerStarted.Invoke();
						}
					}
				}
				else if (!startTimer && playerTimerData.isStarted)
				{
					playerTimerData.endTimeStamp = toggleTime;
					playerTimerData.isStarted = false;
					playerTimerData.lastTimerDuration = (uint)(playerTimerData.endTimeStamp - playerTimerData.startTimeStamp);
					UnityEvent<int, int> onTimerStopped = this.OnTimerStopped;
					if (onTimerStopped != null)
					{
						onTimerStopped.Invoke(player.ActorNumber, playerTimerData.endTimeStamp - playerTimerData.startTimeStamp);
					}
				}
				this.playerTimerData[player.ActorNumber] = playerTimerData;
			}
			else
			{
				PlayerTimerManager.PlayerTimerData playerTimerData2 = new PlayerTimerManager.PlayerTimerData
				{
					startTimeStamp = (startTimer ? toggleTime : 0),
					endTimeStamp = (startTimer ? 0 : toggleTime),
					isStarted = startTimer,
					lastTimerDuration = 0U
				};
				this.playerTimerData.TryAdd(player.ActorNumber, playerTimerData2);
				UnityEvent<int> onTimerStartedForPlayer2 = this.OnTimerStartedForPlayer;
				if (onTimerStartedForPlayer2 != null)
				{
					onTimerStartedForPlayer2.Invoke(player.ActorNumber);
				}
				if (player.IsLocal)
				{
					UnityEvent onLocalTimerStarted2 = this.OnLocalTimerStarted;
					if (onLocalTimerStarted2 != null)
					{
						onLocalTimerStarted2.Invoke();
					}
				}
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x06004647 RID: 17991 RVA: 0x0014E1F4 File Offset: 0x0014C3F4
		private bool ValidateCallLimits(PlayerTimerManager.RPC rpcCall, PhotonMessageInfo info)
		{
			return rpcCall >= PlayerTimerManager.RPC.InitTimersMaster && rpcCall < PlayerTimerManager.RPC.Count && this.callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		}

		// Token: 0x06004648 RID: 17992 RVA: 0x0014E224 File Offset: 0x0014C424
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			base.OnMasterClientSwitched(newMasterClient);
			if (newMasterClient.IsLocal)
			{
				int num = this.SerializeTimerState();
				this.timerPV.RPC("InitTimersMasterRPC", RpcTarget.Others, new object[] { num, this.serializedTimerData });
				return;
			}
			this.playerTimerData.Clear();
			this.areTimersInitialized = false;
		}

		// Token: 0x06004649 RID: 17993 RVA: 0x0014E284 File Offset: 0x0014C484
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			base.OnPlayerEnteredRoom(newPlayer);
			if (PhotonNetwork.IsMasterClient && !newPlayer.IsLocal)
			{
				int num = this.SerializeTimerState();
				this.timerPV.RPC("InitTimersMasterRPC", newPlayer, new object[] { num, this.serializedTimerData });
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x0600464A RID: 17994 RVA: 0x0014E2E0 File Offset: 0x0014C4E0
		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			this.playerTimerData.Remove(otherPlayer.ActorNumber);
			CallLimiter callLimiter;
			if (this.timerToggleLimiters.TryGetValue(otherPlayer.ActorNumber, out callLimiter))
			{
				this.ReturnCallLimiterToPool(callLimiter);
				this.timerToggleLimiters.Remove(otherPlayer.ActorNumber);
			}
			this.UpdateAllTimerBoards();
		}

		// Token: 0x0600464B RID: 17995 RVA: 0x0014E33C File Offset: 0x0014C53C
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			this.joinedRoom = true;
			if (PhotonNetwork.IsMasterClient)
			{
				this.playerTimerData.Clear();
				foreach (CallLimiter callLimiter in this.timerToggleLimiters.Values)
				{
					this.ReturnCallLimiterToPool(callLimiter);
				}
				this.timerToggleLimiters.Clear();
				this.areTimersInitialized = true;
				this.UpdateAllTimerBoards();
				return;
			}
			this.requestSendTime = 0f;
			this.areTimersInitialized = false;
		}

		// Token: 0x0600464C RID: 17996 RVA: 0x0014E3E0 File Offset: 0x0014C5E0
		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
			this.joinedRoom = false;
			this.playerTimerData.Clear();
			foreach (CallLimiter callLimiter in this.timerToggleLimiters.Values)
			{
				this.ReturnCallLimiterToPool(callLimiter);
			}
			this.timerToggleLimiters.Clear();
			this.areTimersInitialized = false;
			this.requestSendTime = 0f;
			this.localPlayerRequestedStart = false;
			this.UpdateAllTimerBoards();
		}

		// Token: 0x0600464D RID: 17997 RVA: 0x0014E47C File Offset: 0x0014C67C
		private void UpdateAllTimerBoards()
		{
			foreach (PlayerTimerBoard playerTimerBoard in PlayerTimerManager.timerBoards)
			{
				this.UpdateTimerBoard(playerTimerBoard);
			}
		}

		// Token: 0x0600464E RID: 17998 RVA: 0x0014E4D0 File Offset: 0x0014C6D0
		private void UpdateTimerBoard(PlayerTimerBoard board)
		{
			board.SetSleepState(this.joinedRoom);
			if (GorillaComputer.instance == null)
			{
				return;
			}
			if (!this.joinedRoom)
			{
				if (board.notInRoomText != null)
				{
					board.notInRoomText.gameObject.SetActive(true);
					board.notInRoomText.text = GorillaComputer.instance.offlineTextInitialString;
				}
				for (int i = 0; i < board.lines.Count; i++)
				{
					board.lines[i].ResetData();
				}
				return;
			}
			if (board.notInRoomText != null)
			{
				board.notInRoomText.gameObject.SetActive(false);
			}
			for (int j = 0; j < board.lines.Count; j++)
			{
				PlayerTimerBoardLine playerTimerBoardLine = board.lines[j];
				if (j < PhotonNetwork.PlayerList.Length)
				{
					playerTimerBoardLine.gameObject.SetActive(true);
					playerTimerBoardLine.SetLineData(NetworkSystem.Instance.GetPlayer(PhotonNetwork.PlayerList[j]));
					playerTimerBoardLine.UpdateLine();
				}
				else
				{
					playerTimerBoardLine.ResetData();
					playerTimerBoardLine.gameObject.SetActive(false);
				}
			}
			board.RedrawPlayerLines();
		}

		// Token: 0x040048D4 RID: 18644
		public static PlayerTimerManager instance;

		// Token: 0x040048D5 RID: 18645
		public PhotonView timerPV;

		// Token: 0x040048D6 RID: 18646
		public UnityEvent OnLocalTimerStarted;

		// Token: 0x040048D7 RID: 18647
		public UnityEvent<int> OnTimerStartedForPlayer;

		// Token: 0x040048D8 RID: 18648
		public UnityEvent<int, int> OnTimerStopped;

		// Token: 0x040048D9 RID: 18649
		public const float MAX_DURATION_SECONDS = 3599.99f;

		// Token: 0x040048DA RID: 18650
		private float requestSendTime;

		// Token: 0x040048DB RID: 18651
		private bool localPlayerRequestedStart;

		// Token: 0x040048DC RID: 18652
		private CallLimiter[] callLimiters;

		// Token: 0x040048DD RID: 18653
		private Dictionary<int, CallLimiter> timerToggleLimiters;

		// Token: 0x040048DE RID: 18654
		private List<CallLimiter> limiterPool;

		// Token: 0x040048DF RID: 18655
		private bool areTimersInitialized;

		// Token: 0x040048E0 RID: 18656
		private Dictionary<int, PlayerTimerManager.PlayerTimerData> playerTimerData;

		// Token: 0x040048E1 RID: 18657
		private const int MAX_TIMER_INIT_BYTES = 256;

		// Token: 0x040048E2 RID: 18658
		private byte[] serializedTimerData;

		// Token: 0x040048E3 RID: 18659
		private static List<PlayerTimerBoard> timerBoards = new List<PlayerTimerBoard>(10);

		// Token: 0x040048E4 RID: 18660
		private bool joinedRoom;

		// Token: 0x02000B24 RID: 2852
		private enum RPC
		{
			// Token: 0x040048E6 RID: 18662
			InitTimersMaster,
			// Token: 0x040048E7 RID: 18663
			ToggleTimerMaster,
			// Token: 0x040048E8 RID: 18664
			Count
		}

		// Token: 0x02000B25 RID: 2853
		public struct PlayerTimerData
		{
			// Token: 0x040048E9 RID: 18665
			public int startTimeStamp;

			// Token: 0x040048EA RID: 18666
			public int endTimeStamp;

			// Token: 0x040048EB RID: 18667
			public bool isStarted;

			// Token: 0x040048EC RID: 18668
			public uint lastTimerDuration;
		}
	}
}
