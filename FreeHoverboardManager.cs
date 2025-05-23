using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000667 RID: 1639
public class FreeHoverboardManager : NetworkSceneObject
{
	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x060028F7 RID: 10487 RVA: 0x000CC103 File Offset: 0x000CA303
	// (set) Token: 0x060028F8 RID: 10488 RVA: 0x000CC10A File Offset: 0x000CA30A
	public static FreeHoverboardManager instance { get; private set; }

	// Token: 0x060028F9 RID: 10489 RVA: 0x000CC114 File Offset: 0x000CA314
	private FreeHoverboardManager.DataPerPlayer GetOrCreatePlayerData(int actorNumber)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (!this.perPlayerData.TryGetValue(actorNumber, out dataPerPlayer))
		{
			dataPerPlayer = default(FreeHoverboardManager.DataPerPlayer);
			dataPerPlayer.Init(actorNumber, this.freeBoardPool);
			this.perPlayerData.Add(actorNumber, dataPerPlayer);
		}
		return dataPerPlayer;
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x000CC158 File Offset: 0x000CA358
	private void Awake()
	{
		FreeHoverboardManager.instance = this;
		for (int i = 0; i < 20; i++)
		{
			FreeHoverboardInstance freeHoverboardInstance = global::UnityEngine.Object.Instantiate<FreeHoverboardInstance>(this.freeHoverboardPrefab);
			freeHoverboardInstance.gameObject.SetActive(false);
			this.freeBoardPool.Push(freeHoverboardInstance);
		}
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnLeftRoom;
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x000CC1C8 File Offset: 0x000CA3C8
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (this.perPlayerData.TryGetValue(netPlayer.ActorNumber, out dataPerPlayer))
		{
			dataPerPlayer.ReturnBoards(this.freeBoardPool);
			this.perPlayerData.Remove(netPlayer.ActorNumber);
		}
	}

	// Token: 0x060028FC RID: 10492 RVA: 0x000CC20C File Offset: 0x000CA40C
	private void OnLeftRoom()
	{
		foreach (KeyValuePair<int, FreeHoverboardManager.DataPerPlayer> keyValuePair in this.perPlayerData)
		{
			keyValuePair.Value.ReturnBoards(this.freeBoardPool);
		}
		this.perPlayerData.Clear();
	}

	// Token: 0x060028FD RID: 10493 RVA: 0x000CC278 File Offset: 0x000CA478
	private void SpawnBoard(FreeHoverboardManager.DataPerPlayer playerData, int boardIndex, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
	{
		FreeHoverboardInstance freeHoverboardInstance = ((boardIndex == 0) ? playerData.board0 : playerData.board1);
		freeHoverboardInstance.transform.position = position;
		freeHoverboardInstance.transform.rotation = rotation;
		freeHoverboardInstance.Rigidbody.velocity = velocity;
		freeHoverboardInstance.Rigidbody.angularVelocity = avelocity;
		freeHoverboardInstance.SetColor(boardColor);
		freeHoverboardInstance.gameObject.SetActive(true);
		int ownerActorNumber = freeHoverboardInstance.ownerActorNumber;
		NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
		int? num = ((localPlayer != null) ? new int?(localPlayer.ActorNumber) : null);
		if ((ownerActorNumber == num.GetValueOrDefault()) & (num != null))
		{
			this.localPlayerLastSpawnedBoardIndex = boardIndex;
		}
	}

	// Token: 0x060028FE RID: 10494 RVA: 0x000CC320 File Offset: 0x000CA520
	public void SendDropBoardRPC(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
	{
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		int num = ((!orCreatePlayerData.board0.gameObject.activeSelf) ? 0 : ((!orCreatePlayerData.board1.gameObject.activeSelf) ? 1 : (1 - this.localPlayerLastSpawnedBoardIndex)));
		if (PhotonNetwork.InRoom)
		{
			long num2 = BitPackUtils.PackWorldPosForNetwork(position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
			long num4 = BitPackUtils.PackWorldPosForNetwork(velocity);
			long num5 = BitPackUtils.PackWorldPosForNetwork(avelocity);
			short num6 = BitPackUtils.PackColorForNetwork(boardColor);
			this.photonView.RPC("DropBoard_RPC", RpcTarget.All, new object[]
			{
				num == 1,
				num2,
				num3,
				num4,
				num5,
				num6
			});
			return;
		}
		this.SpawnBoard(orCreatePlayerData, num, position, rotation, velocity, avelocity, boardColor);
	}

	// Token: 0x060028FF RID: 10495 RVA: 0x000CC408 File Offset: 0x000CA608
	[PunRPC]
	public void DropBoard_RPC(bool boardIndex1, long positionPacked, int rotationPacked, long velocityPacked, long avelocityPacked, short colorPacked, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "DropBoard_RPC");
		int num = (boardIndex1 ? 1 : 0);
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(info.Sender.ActorNumber);
		if (info.Sender != PhotonNetwork.LocalPlayer && !orCreatePlayerData.spamCheck.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(positionPacked);
		if (!rigContainer.Rig.IsPositionInRange(vector, 5f))
		{
			return;
		}
		this.SpawnBoard(orCreatePlayerData, num, vector, BitPackUtils.UnpackQuaternionFromNetwork(rotationPacked), BitPackUtils.UnpackWorldPosFromNetwork(velocityPacked), BitPackUtils.UnpackWorldPosFromNetwork(avelocityPacked), BitPackUtils.UnpackColorFromNetwork(colorPacked));
	}

	// Token: 0x06002900 RID: 10496 RVA: 0x000CC4B4 File Offset: 0x000CA6B4
	public void SendGrabBoardRPC(FreeHoverboardInstance board)
	{
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("GrabBoard_RPC", RpcTarget.All, new object[]
			{
				board.ownerActorNumber,
				board.boardIndex == 1
			});
			board.gameObject.SetActive(false);
			return;
		}
		board.gameObject.SetActive(false);
	}

	// Token: 0x06002901 RID: 10497 RVA: 0x000CC518 File Offset: 0x000CA718
	[PunRPC]
	public void GrabBoard_RPC(int ownerActorNumber, bool boardIndex1, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "GrabBoard_RPC");
		int num = (boardIndex1 ? 1 : 0);
		if (NetworkSystem.Instance.GetNetPlayerByID(ownerActorNumber) == null)
		{
			return;
		}
		FreeHoverboardManager.DataPerPlayer orCreatePlayerData = this.GetOrCreatePlayerData(ownerActorNumber);
		if (info.Sender != PhotonNetwork.LocalPlayer && !orCreatePlayerData.spamCheck.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		FreeHoverboardInstance board = orCreatePlayerData.GetBoard(num);
		if (board.IsNull())
		{
			return;
		}
		if (info.Sender.ActorNumber != ownerActorNumber)
		{
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				return;
			}
			if (!rigContainer.Rig.IsPositionInRange(board.transform.position, 5f))
			{
				return;
			}
		}
		board.gameObject.SetActive(false);
	}

	// Token: 0x06002902 RID: 10498 RVA: 0x000CC5D0 File Offset: 0x000CA7D0
	public void PreserveMaxHoverboardsConstraint(int actorNumber)
	{
		FreeHoverboardManager.DataPerPlayer dataPerPlayer;
		if (!this.perPlayerData.TryGetValue(actorNumber, out dataPerPlayer))
		{
			return;
		}
		if (dataPerPlayer.board0.gameObject.activeSelf && dataPerPlayer.board1.gameObject.activeSelf)
		{
			FreeHoverboardInstance board = dataPerPlayer.GetBoard(1 - this.localPlayerLastSpawnedBoardIndex);
			this.SendGrabBoardRPC(board);
		}
	}

	// Token: 0x04002E0C RID: 11788
	[SerializeField]
	private FreeHoverboardInstance freeHoverboardPrefab;

	// Token: 0x04002E0D RID: 11789
	private Stack<FreeHoverboardInstance> freeBoardPool = new Stack<FreeHoverboardInstance>(20);

	// Token: 0x04002E0E RID: 11790
	private const int NumPlayers = 10;

	// Token: 0x04002E0F RID: 11791
	private const int NumFreeBoardsPerPlayer = 2;

	// Token: 0x04002E10 RID: 11792
	private int localPlayerLastSpawnedBoardIndex;

	// Token: 0x04002E11 RID: 11793
	private Dictionary<int, FreeHoverboardManager.DataPerPlayer> perPlayerData = new Dictionary<int, FreeHoverboardManager.DataPerPlayer>();

	// Token: 0x02000668 RID: 1640
	private struct DataPerPlayer
	{
		// Token: 0x06002904 RID: 10500 RVA: 0x000CC64C File Offset: 0x000CA84C
		public void Init(int actorNumber, Stack<FreeHoverboardInstance> freeBoardPool)
		{
			this.board0 = freeBoardPool.Pop();
			this.board0.ownerActorNumber = actorNumber;
			this.board0.boardIndex = 0;
			this.board1 = freeBoardPool.Pop();
			this.board1.ownerActorNumber = actorNumber;
			this.board1.boardIndex = 1;
			this.spamCheck = new CallLimiterWithCooldown(5f, 10, 1f);
		}

		// Token: 0x06002905 RID: 10501 RVA: 0x000CC6B8 File Offset: 0x000CA8B8
		public void ReturnBoards(Stack<FreeHoverboardInstance> freeBoardPool)
		{
			this.board0.gameObject.SetActive(false);
			freeBoardPool.Push(this.board0);
			this.board1.gameObject.SetActive(false);
			freeBoardPool.Push(this.board1);
		}

		// Token: 0x06002906 RID: 10502 RVA: 0x000CC6F4 File Offset: 0x000CA8F4
		public FreeHoverboardInstance GetBoard(int boardIndex)
		{
			if (boardIndex != 1)
			{
				return this.board0;
			}
			return this.board1;
		}

		// Token: 0x04002E12 RID: 11794
		public FreeHoverboardInstance board0;

		// Token: 0x04002E13 RID: 11795
		public FreeHoverboardInstance board1;

		// Token: 0x04002E14 RID: 11796
		public CallLimiterWithCooldown spamCheck;
	}
}
