using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTagScripts.Builder;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000AF7 RID: 2807
	public class BuilderTableNetworking : MonoBehaviourPunCallbacks
	{
		// Token: 0x06004484 RID: 17540 RVA: 0x00143708 File Offset: 0x00141908
		private void Awake()
		{
			this.masterClientTableInit = new List<BuilderTableNetworking.PlayerTableInitState>(10);
			this.masterClientTableValidators = new List<BuilderTableNetworking.PlayerTableInitState>(10);
			this.localClientTableInit = new BuilderTableNetworking.PlayerTableInitState();
			this.localValidationTable = new BuilderTableNetworking.PlayerTableInitState();
			this.callLimiters = new CallLimiter[26];
			this.callLimiters[0] = new CallLimiter(20, 30f, 0.5f);
			this.callLimiters[1] = new CallLimiter(200, 1f, 0.5f);
			this.callLimiters[2] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[3] = new CallLimiter(2, 1f, 0.5f);
			this.callLimiters[4] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[5] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[6] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[7] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[8] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[9] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[10] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[11] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[12] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[13] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[14] = new CallLimiter(100, 1f, 0.5f);
			this.callLimiters[15] = new CallLimiter(100, 1f, 0.5f);
			this.callLimiters[16] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[17] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[18] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[19] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[20] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[21] = new CallLimiter(50, 1f, 0.5f);
			this.callLimiters[22] = new CallLimiter(20, 1f, 0.5f);
			this.callLimiters[23] = new CallLimiter(20, 1f, 0.5f);
			this.callLimiters[24] = new CallLimiter(3, 30f, 0.5f);
			this.callLimiters[25] = new CallLimiter(10, 1f, 0.5f);
			this.armShelfRequests = new List<Player>(10);
		}

		// Token: 0x06004485 RID: 17541 RVA: 0x001439FB File Offset: 0x00141BFB
		public void SetTable(BuilderTable table)
		{
			this.currTable = table;
		}

		// Token: 0x06004486 RID: 17542 RVA: 0x00143A04 File Offset: 0x00141C04
		private BuilderTable GetTable()
		{
			return this.currTable;
		}

		// Token: 0x06004487 RID: 17543 RVA: 0x00143A0C File Offset: 0x00141C0C
		private int CreateLocalCommandId()
		{
			int num = this.nextLocalCommandId;
			this.nextLocalCommandId++;
			return num;
		}

		// Token: 0x06004488 RID: 17544 RVA: 0x00143A22 File Offset: 0x00141C22
		public BuilderTableNetworking.PlayerTableInitState GetLocalTableInit()
		{
			return this.localClientTableInit;
		}

		// Token: 0x06004489 RID: 17545 RVA: 0x00143A2C File Offset: 0x00141C2C
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			if (!newMasterClient.IsLocal)
			{
				this.localClientTableInit.Reset();
				BuilderTable table = this.GetTable();
				if (table.GetTableState() != BuilderTable.TableState.WaitingForZoneAndRoom)
				{
					if (table.GetTableState() == BuilderTable.TableState.Ready)
					{
						table.SetTableState(BuilderTable.TableState.WaitForMasterResync);
					}
					else if (table.GetTableState() == BuilderTable.TableState.WaitForMasterResync || table.GetTableState() == BuilderTable.TableState.ReceivingMasterResync)
					{
						table.SetTableState(BuilderTable.TableState.WaitForMasterResync);
					}
					else
					{
						table.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
					}
					this.PlayerEnterBuilder();
				}
				return;
			}
			this.masterClientTableInit.Clear();
			this.localClientTableInit.Reset();
			BuilderTable table2 = this.GetTable();
			BuilderTable.TableState tableState = table2.GetTableState();
			bool flag = (tableState != BuilderTable.TableState.Ready && tableState != BuilderTable.TableState.WaitingForZoneAndRoom && tableState != BuilderTable.TableState.WaitForMasterResync && tableState != BuilderTable.TableState.ReceivingMasterResync) || table2.pieces.Count <= 0;
			if (!flag)
			{
				flag |= table2.pieces.Count <= 0;
			}
			if (flag)
			{
				table2.ClearTable();
				table2.ClearQueuedCommands();
				table2.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
				return;
			}
			for (int i = 0; i < table2.pieces.Count; i++)
			{
				BuilderPiece builderPiece = table2.pieces[i];
				Player player = PhotonNetwork.CurrentRoom.GetPlayer(builderPiece.heldByPlayerActorNumber, false);
				if (table2.pieces[i].state == BuilderPiece.State.Grabbed && player == null)
				{
					Vector3 position = builderPiece.transform.position;
					Quaternion rotation = builderPiece.transform.rotation;
					Debug.LogErrorFormat("We have a piece {0} {1} held by an invalid player {2} dropping", new object[] { builderPiece.name, builderPiece.pieceId, builderPiece.heldByPlayerActorNumber });
					this.CreateLocalCommandId();
					builderPiece.ClearParentHeld();
					builderPiece.ClearParentPiece(false);
					builderPiece.transform.localScale = Vector3.one;
					builderPiece.SetState(BuilderPiece.State.Dropped, false);
					builderPiece.transform.SetLocalPositionAndRotation(position, rotation);
					if (builderPiece.rigidBody != null)
					{
						builderPiece.rigidBody.position = position;
						builderPiece.rigidBody.rotation = rotation;
						builderPiece.rigidBody.velocity = Vector3.zero;
						builderPiece.rigidBody.angularVelocity = Vector3.zero;
					}
				}
			}
			table2.ClearQueuedCommands();
			table2.SetTableState(BuilderTable.TableState.Ready);
		}

		// Token: 0x0600448A RID: 17546 RVA: 0x00143C68 File Offset: 0x00141E68
		public override void OnPlayerLeftRoom(Player player)
		{
			Debug.LogFormat("Player {0} left room", new object[] { player.ActorNumber });
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				if (table.isTableMutable)
				{
					if (!PhotonNetwork.IsMasterClient)
					{
						table.DropAllPiecesForPlayerLeaving(player.ActorNumber);
					}
					else
					{
						table.RecycleAllPiecesForPlayerLeaving(player.ActorNumber);
					}
				}
				table.PlayerLeftRoom(player.ActorNumber);
			}
			if (!table.isTableMutable && table.linkedTerminal != null && table.linkedTerminal.IsPlayerDriver(player))
			{
				table.linkedTerminal.ResetTerminalControl();
				if (NetworkSystem.Instance.IsMasterClient)
				{
					base.photonView.RPC("SetBlocksTerminalDriverRPC", RpcTarget.All, new object[] { -2 });
				}
			}
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			table.RemoveArmShelfForPlayer(player);
			table.VerifySetSelections();
			if (player != PhotonNetwork.LocalPlayer)
			{
				this.DestroyPlayerTableInit(player);
			}
		}

		// Token: 0x0600448B RID: 17547 RVA: 0x00143D57 File Offset: 0x00141F57
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			BuilderTable table = this.GetTable();
			table.SetPendingMap(null);
			table.SetInRoom(true);
		}

		// Token: 0x0600448C RID: 17548 RVA: 0x00143D72 File Offset: 0x00141F72
		public override void OnLeftRoom()
		{
			this.PlayerExitBuilder();
			BuilderTable table = this.GetTable();
			table.SetPendingMap(null);
			table.SetInRoom(false);
			this.armShelfRequests.Clear();
		}

		// Token: 0x0600448D RID: 17549 RVA: 0x00143D98 File Offset: 0x00141F98
		private void Update()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				this.UpdateNewPlayerInit();
			}
		}

		// Token: 0x0600448E RID: 17550 RVA: 0x00143DA8 File Offset: 0x00141FA8
		public void PlayerEnterBuilder()
		{
			this.tablePhotonView.RPC("PlayerEnterBuilderRPC", PhotonNetwork.MasterClient, new object[]
			{
				PhotonNetwork.LocalPlayer,
				true
			});
			GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
			if (gorillaGuardianManager != null && gorillaGuardianManager.isPlaying && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				gorillaGuardianManager.RequestEjectGuardian(NetworkSystem.Instance.LocalPlayer);
			}
		}

		// Token: 0x0600448F RID: 17551 RVA: 0x00143E1C File Offset: 0x0014201C
		[PunRPC]
		public void PlayerEnterBuilderRPC(Player player, bool entered, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerEnterBuilderRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlayerEnterMaster, info))
			{
				return;
			}
			if (player == null || !player.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (entered)
			{
				BuilderTable.TableState tableState = table.GetTableState();
				if (tableState == BuilderTable.TableState.WaitingForInitalBuild || (this.IsPrivateMasterClient() && tableState == BuilderTable.TableState.WaitingForZoneAndRoom))
				{
					table.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
				}
				if (player != PhotonNetwork.LocalPlayer)
				{
					this.CreateSerializedTableForNewPlayerInit(player);
				}
				if (table.isTableMutable)
				{
					this.RequestCreateArmShelfForPlayer(player);
					return;
				}
				if (table.linkedTerminal != null)
				{
					base.photonView.RPC("SetBlocksTerminalDriverRPC", player, new object[] { table.linkedTerminal.GetDriverID });
					return;
				}
			}
			else
			{
				if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.DestroyPlayerTableInit(player);
				}
				if (table.isTableMutable)
				{
					table.RemoveArmShelfForPlayer(player);
				}
			}
		}

		// Token: 0x06004490 RID: 17552 RVA: 0x00143F00 File Offset: 0x00142100
		public void PlayerExitBuilder()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				this.tablePhotonView.RPC("PlayerEnterBuilderRPC", PhotonNetwork.MasterClient, new object[]
				{
					PhotonNetwork.LocalPlayer,
					false
				});
			}
			BuilderTable table = this.GetTable();
			table.ClearTable();
			table.ClearQueuedCommands();
			this.localClientTableInit.Reset();
			this.armShelfRequests.Clear();
			this.masterClientTableInit.Clear();
		}

		// Token: 0x06004491 RID: 17553 RVA: 0x00143F77 File Offset: 0x00142177
		public bool IsPrivateMasterClient()
		{
			return PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient && NetworkSystem.Instance.SessionIsPrivate;
		}

		// Token: 0x06004492 RID: 17554 RVA: 0x00143F94 File Offset: 0x00142194
		private void UpdateNewPlayerInit()
		{
			if (this.GetTable().GetTableState() == BuilderTable.TableState.Ready)
			{
				for (int i = 0; i < this.masterClientTableInit.Count; i++)
				{
					if (this.masterClientTableInit[i].waitForInitTimeRemaining >= 0f)
					{
						this.masterClientTableInit[i].waitForInitTimeRemaining -= Time.deltaTime;
						if (this.masterClientTableInit[i].waitForInitTimeRemaining <= 0f)
						{
							this.StartCreatingSerializedTable(this.masterClientTableInit[i].player);
							this.masterClientTableInit[i].waitForInitTimeRemaining = -1f;
							this.masterClientTableInit[i].sendNextChunkTimeRemaining = 0f;
						}
					}
					else if (this.masterClientTableInit[i].sendNextChunkTimeRemaining >= 0f)
					{
						this.masterClientTableInit[i].sendNextChunkTimeRemaining -= Time.deltaTime;
						if (this.masterClientTableInit[i].sendNextChunkTimeRemaining <= 0f)
						{
							this.SendNextTableData(this.masterClientTableInit[i].player);
							if (this.masterClientTableInit[i].numSerializedBytes < this.masterClientTableInit[i].totalSerializedBytes)
							{
								this.masterClientTableInit[i].sendNextChunkTimeRemaining = 0f;
							}
							else
							{
								this.masterClientTableInit[i].sendNextChunkTimeRemaining = -1f;
							}
						}
					}
				}
			}
		}

		// Token: 0x06004493 RID: 17555 RVA: 0x00144124 File Offset: 0x00142324
		private void StartCreatingSerializedTable(Player newPlayer)
		{
			BuilderTable table = this.GetTable();
			BuilderTableNetworking.PlayerTableInitState playerTableInit = this.GetPlayerTableInit(newPlayer);
			playerTableInit.totalSerializedBytes = table.SerializeTableState(playerTableInit.serializedTableState, 1048576);
			byte[] array = GZipStream.CompressBuffer(playerTableInit.serializedTableState);
			playerTableInit.totalSerializedBytes = array.Length;
			Array.Copy(array, 0, playerTableInit.serializedTableState, 0, playerTableInit.totalSerializedBytes);
			playerTableInit.numSerializedBytes = 0;
			this.tablePhotonView.RPC("StartBuildTableRPC", newPlayer, new object[] { playerTableInit.totalSerializedBytes });
		}

		// Token: 0x06004494 RID: 17556 RVA: 0x001441AC File Offset: 0x001423AC
		[PunRPC]
		public void StartBuildTableRPC(int totalBytes, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "StartBuildTableRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.TableDataStart, info))
			{
				return;
			}
			if (totalBytes <= 0 || totalBytes > 1048576)
			{
				Debug.LogError("Builder Table Bytes is too large: " + totalBytes.ToString());
				return;
			}
			BuilderTable table = this.GetTable();
			GTDev.Log<string>("StartBuildTableRPC with current state " + table.GetTableState().ToString(), null);
			if (table.GetTableState() != BuilderTable.TableState.WaitForMasterResync && table.GetTableState() != BuilderTable.TableState.WaitingForInitalBuild)
			{
				return;
			}
			if (table.GetTableState() == BuilderTable.TableState.WaitForMasterResync)
			{
				table.SetTableState(BuilderTable.TableState.ReceivingMasterResync);
			}
			else
			{
				table.SetTableState(BuilderTable.TableState.ReceivingInitialBuild);
			}
			this.localClientTableInit.Reset();
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = this.localClientTableInit;
			playerTableInitState.player = PhotonNetwork.LocalPlayer;
			playerTableInitState.totalSerializedBytes = totalBytes;
			table.ClearQueuedCommands();
		}

		// Token: 0x06004495 RID: 17557 RVA: 0x00144288 File Offset: 0x00142488
		private void SendNextTableData(Player requestingPlayer)
		{
			BuilderTableNetworking.PlayerTableInitState playerTableInit = this.GetPlayerTableInit(requestingPlayer);
			if (playerTableInit == null)
			{
				Debug.LogErrorFormat("No Table init found for player {0}", new object[] { requestingPlayer.ActorNumber });
				return;
			}
			int num = Mathf.Min(1000, playerTableInit.totalSerializedBytes - playerTableInit.numSerializedBytes);
			if (num <= 0)
			{
				return;
			}
			Array.Copy(playerTableInit.serializedTableState, playerTableInit.numSerializedBytes, playerTableInit.chunk, 0, num);
			playerTableInit.numSerializedBytes += num;
			this.tablePhotonView.RPC("SendTableDataRPC", requestingPlayer, new object[] { num, playerTableInit.chunk });
		}

		// Token: 0x06004496 RID: 17558 RVA: 0x0014432C File Offset: 0x0014252C
		[PunRPC]
		public void SendTableDataRPC(int numBytes, byte[] bytes, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SendTableDataRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (numBytes <= 0 || numBytes > 1000)
			{
				Debug.LogErrorFormat("Builder Table Send Data numBytes is too large {0}", new object[] { numBytes });
				return;
			}
			if (bytes.Length > 1000)
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.TableData, info))
			{
				return;
			}
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = this.localClientTableInit;
			if (playerTableInitState.numSerializedBytes + bytes.Length > 1048576)
			{
				Debug.LogErrorFormat("Builder Table serialized bytes is larger than buffer {0}", new object[] { playerTableInitState.numSerializedBytes + bytes.Length });
				return;
			}
			Array.Copy(bytes, 0, playerTableInitState.serializedTableState, playerTableInitState.numSerializedBytes, bytes.Length);
			playerTableInitState.numSerializedBytes += bytes.Length;
			if (playerTableInitState.numSerializedBytes >= playerTableInitState.totalSerializedBytes)
			{
				this.GetTable().SetTableState(BuilderTable.TableState.InitialBuild);
			}
		}

		// Token: 0x06004497 RID: 17559 RVA: 0x00144414 File Offset: 0x00142614
		private bool DoesTableInitExist(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004498 RID: 17560 RVA: 0x00144458 File Offset: 0x00142658
		private BuilderTableNetworking.PlayerTableInitState CreatePlayerTableInit(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					this.masterClientTableInit[i].Reset();
					return this.masterClientTableInit[i];
				}
			}
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = new BuilderTableNetworking.PlayerTableInitState();
			playerTableInitState.player = player;
			this.masterClientTableInit.Add(playerTableInitState);
			return playerTableInitState;
		}

		// Token: 0x06004499 RID: 17561 RVA: 0x001444D4 File Offset: 0x001426D4
		public void ResetSerializedTableForAllPlayers()
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				this.masterClientTableInit[i].waitForInitTimeRemaining = 1f;
				this.masterClientTableInit[i].sendNextChunkTimeRemaining = -1f;
				this.masterClientTableInit[i].numSerializedBytes = 0;
				this.masterClientTableInit[i].totalSerializedBytes = 0;
			}
		}

		// Token: 0x0600449A RID: 17562 RVA: 0x00144547 File Offset: 0x00142747
		private void CreateSerializedTableForNewPlayerInit(Player newPlayer)
		{
			if (this.DoesTableInitExist(newPlayer))
			{
				return;
			}
			BuilderTableNetworking.PlayerTableInitState playerTableInitState = this.CreatePlayerTableInit(newPlayer);
			playerTableInitState.waitForInitTimeRemaining = 1f;
			playerTableInitState.sendNextChunkTimeRemaining = -1f;
		}

		// Token: 0x0600449B RID: 17563 RVA: 0x00144570 File Offset: 0x00142770
		private void DestroyPlayerTableInit(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					this.masterClientTableInit.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x0600449C RID: 17564 RVA: 0x001445C4 File Offset: 0x001427C4
		private BuilderTableNetworking.PlayerTableInitState GetPlayerTableInit(Player player)
		{
			for (int i = 0; i < this.masterClientTableInit.Count; i++)
			{
				if (this.masterClientTableInit[i].player.ActorNumber == player.ActorNumber)
				{
					return this.masterClientTableInit[i];
				}
			}
			return null;
		}

		// Token: 0x0600449D RID: 17565 RVA: 0x00144614 File Offset: 0x00142814
		private bool ValidateMasterClientIsReady(Player player)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return false;
			}
			if (player != null && !player.IsMasterClient)
			{
				BuilderTableNetworking.PlayerTableInitState playerTableInit = this.GetPlayerTableInit(player);
				if (playerTableInit != null && playerTableInit.numSerializedBytes < playerTableInit.totalSerializedBytes)
				{
					return false;
				}
			}
			return this.GetTable().GetTableState() == BuilderTable.TableState.Ready;
		}

		// Token: 0x0600449E RID: 17566 RVA: 0x00144664 File Offset: 0x00142864
		private bool ValidateCallLimits(BuilderTableNetworking.RPC rpcCall, PhotonMessageInfo info)
		{
			return rpcCall >= BuilderTableNetworking.RPC.PlayerEnterMaster && rpcCall < BuilderTableNetworking.RPC.Count && this.callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		}

		// Token: 0x0600449F RID: 17567 RVA: 0x00144692 File Offset: 0x00142892
		[PunRPC]
		public void RequestFailedRPC(int localCommandId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestFailedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.RequestFailed, info))
			{
				return;
			}
			this.GetTable().RollbackFailedCommand(localCommandId);
		}

		// Token: 0x060044A0 RID: 17568 RVA: 0x000023F4 File Offset: 0x000005F4
		public void RequestCreatePiece(int newPieceType, Vector3 position, Quaternion rotation, int materialType)
		{
		}

		// Token: 0x060044A1 RID: 17569 RVA: 0x000023F4 File Offset: 0x000005F4
		public void RequestCreatePieceRPC(int newPieceType, long packedPosition, int packedRotation, int materialType, PhotonMessageInfo info)
		{
		}

		// Token: 0x060044A2 RID: 17570 RVA: 0x000023F4 File Offset: 0x000005F4
		public void PieceCreatedRPC(int pieceType, int pieceId, long packedPosition, int packedRotation, int materialType, Player creatingPlayer, PhotonMessageInfo info)
		{
		}

		// Token: 0x060044A3 RID: 17571 RVA: 0x001446C8 File Offset: 0x001428C8
		public void CreateShelfPiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, BuilderPiece.State state, int shelfID)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			BuilderPiece piecePrefab = table.GetPiecePrefab(pieceType);
			if (!table.HasEnoughResources(piecePrefab))
			{
				Debug.Log("Not Enough Resources");
				return;
			}
			if (state != BuilderPiece.State.OnShelf)
			{
				if (state != BuilderPiece.State.OnConveyor)
				{
					return;
				}
				if (shelfID < 0 || shelfID >= table.conveyors.Count)
				{
					return;
				}
			}
			else if (shelfID < 0 || shelfID >= table.dispenserShelves.Count)
			{
				return;
			}
			int num = table.CreatePieceId();
			long num2 = BitPackUtils.PackWorldPosForNetwork(position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
			base.photonView.RPC("PieceCreatedByShelfRPC", RpcTarget.All, new object[]
			{
				pieceType,
				num,
				num2,
				num3,
				materialType,
				(byte)state,
				shelfID,
				PhotonNetwork.LocalPlayer
			});
		}

		// Token: 0x060044A4 RID: 17572 RVA: 0x001447C4 File Offset: 0x001429C4
		[PunRPC]
		public void PieceCreatedByShelfRPC(int pieceType, int pieceId, long packedPosition, int packedRotation, int materialType, byte state, int shelfID, Player creatingPlayer, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.CreateShelfPieceMaster, info))
			{
				return;
			}
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPosition);
			Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(packedRotation);
			if (!table.ValidatePieceWorldTransform(vector, quaternion))
			{
				return;
			}
			if (state == 4)
			{
				table.CreateDispenserShelfPiece(pieceType, pieceId, vector, quaternion, materialType, shelfID);
				return;
			}
			if (state != 7)
			{
				return;
			}
			table.CreateConveyorPiece(pieceType, pieceId, vector, quaternion, materialType, shelfID, info.SentServerTimestamp);
		}

		// Token: 0x060044A5 RID: 17573 RVA: 0x00144848 File Offset: 0x00142A48
		public void RequestRecyclePiece(int pieceId, Vector3 position, Quaternion rotation, bool playFX, int recyclerID)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			if (!table.isTableMutable)
			{
				return;
			}
			float num = 10000f;
			if (!(in position).IsValid(in num) || !(in rotation).IsValid())
			{
				return;
			}
			if (recyclerID > 32767 || recyclerID < -1)
			{
				return;
			}
			long num2 = BitPackUtils.PackWorldPosForNetwork(position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
			base.photonView.RPC("PieceDestroyedRPC", RpcTarget.All, new object[]
			{
				pieceId,
				num2,
				num3,
				playFX,
				(short)recyclerID
			});
		}

		// Token: 0x060044A6 RID: 17574 RVA: 0x001448F8 File Offset: 0x00142AF8
		[PunRPC]
		public void PieceDestroyedRPC(int pieceId, long packedPosition, int packedRotation, bool playFX, short recyclerID, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceDestroyedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.RecyclePieceMaster, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPosition);
			Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(packedRotation);
			float num = 10000f;
			if (!(in vector).IsValid(in num) || !(in quaternion).IsValid())
			{
				return;
			}
			table.RecyclePiece(pieceId, vector, quaternion, playFX, (int)recyclerID, info.Sender);
		}

		// Token: 0x060044A7 RID: 17575 RVA: 0x0014497C File Offset: 0x00142B7C
		public void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
		{
			if (piece == null)
			{
				return;
			}
			int pieceId = piece.pieceId;
			int num = ((parentPiece != null) ? parentPiece.pieceId : (-1));
			int num2 = ((attachPiece != null) ? attachPiece.pieceId : (-1));
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidatePlacePieceParams(pieceId, num2, bumpOffsetX, bumpOffsetZ, twist, num, attachIndex, parentAttachIndex, NetPlayer.Get(PhotonNetwork.LocalPlayer)))
			{
				return;
			}
			int num3 = this.CreateLocalCommandId();
			attachPiece.requestedParentPiece = parentPiece;
			table.UpdatePieceData(attachPiece);
			table.PlacePiece(num3, pieceId, num2, bumpOffsetX, bumpOffsetZ, twist, num, attachIndex, parentAttachIndex, NetPlayer.Get(PhotonNetwork.LocalPlayer), PhotonNetwork.ServerTimestamp, true);
			int num4 = BuilderTable.PackPiecePlacement(twist, bumpOffsetX, bumpOffsetZ);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestPlacePieceRPC", RpcTarget.MasterClient, new object[]
				{
					num3,
					pieceId,
					num2,
					num4,
					num,
					attachIndex,
					parentAttachIndex,
					PhotonNetwork.LocalPlayer
				});
			}
		}

		// Token: 0x060044A8 RID: 17576 RVA: 0x00144AA4 File Offset: 0x00142CA4
		[PunRPC]
		public void RequestPlacePieceRPC(int localCommandId, int pieceId, int attachPieceId, int placement, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestPlacePieceRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlacePieceMaster, info) || placedByPlayer == null || !placedByPlayer.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			bool isMasterClient = info.Sender.IsMasterClient;
			byte b;
			sbyte b2;
			sbyte b3;
			BuilderTable.UnpackPiecePlacement(placement, out b, out b2, out b3);
			bool flag = isMasterClient || table.ValidatePlacePieceParams(pieceId, attachPieceId, b2, b3, b, parentPieceId, attachIndex, parentAttachIndex, NetPlayer.Get(placedByPlayer));
			if (flag)
			{
				flag &= isMasterClient || table.ValidatePlacePieceState(pieceId, attachPieceId, b2, b3, b, parentPieceId, attachIndex, parentAttachIndex, placedByPlayer);
			}
			if (flag)
			{
				BuilderPiece piece = table.GetPiece(parentPieceId);
				BuilderPiecePrivatePlot builderPiecePrivatePlot;
				if (piece != null && piece.TryGetPlotComponent(out builderPiecePrivatePlot) && !builderPiecePrivatePlot.IsPlotClaimed())
				{
					base.photonView.RPC("PlotClaimedRPC", RpcTarget.All, new object[] { parentPieceId, placedByPlayer, true });
				}
				base.photonView.RPC("PiecePlacedRPC", RpcTarget.All, new object[] { localCommandId, pieceId, attachPieceId, placement, parentPieceId, attachIndex, parentAttachIndex, placedByPlayer, info.SentServerTimestamp });
				return;
			}
			base.photonView.RPC("RequestFailedRPC", info.Sender, new object[] { localCommandId });
		}

		// Token: 0x060044A9 RID: 17577 RVA: 0x00144C58 File Offset: 0x00142E58
		[PunRPC]
		public void PiecePlacedRPC(int localCommandId, int pieceId, int attachPieceId, int placement, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer, int timeStamp, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PiecePlacedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlacePiece, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (placedByPlayer == null)
			{
				return;
			}
			if ((ulong)(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout) || (ulong)(info.SentServerTimestamp - timeStamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout))
			{
				timeStamp = PhotonNetwork.ServerTimestamp;
			}
			byte b;
			sbyte b2;
			sbyte b3;
			BuilderTable.UnpackPiecePlacement(placement, out b, out b2, out b3);
			table.PlacePiece(localCommandId, pieceId, attachPieceId, b2, b3, b, parentPieceId, attachIndex, parentAttachIndex, NetPlayer.Get(placedByPlayer), timeStamp, false);
		}

		// Token: 0x060044AA RID: 17578 RVA: 0x00144D10 File Offset: 0x00142F10
		public void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
		{
			if (piece == null)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidateGrabPieceParams(piece.pieceId, isLefHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer)))
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				this.CheckForFreedPlot(piece.pieceId, PhotonNetwork.LocalPlayer);
			}
			int num = this.CreateLocalCommandId();
			table.GrabPiece(num, piece.pieceId, isLefHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer), true);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				long num2 = BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation);
				base.photonView.RPC("RequestGrabPieceRPC", RpcTarget.MasterClient, new object[]
				{
					num,
					piece.pieceId,
					isLefHand,
					num2,
					PhotonNetwork.LocalPlayer
				});
			}
		}

		// Token: 0x060044AB RID: 17579 RVA: 0x00144DEC File Offset: 0x00142FEC
		[PunRPC]
		public void RequestGrabPieceRPC(int localCommandId, int pieceId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestGrabPieceRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.GrabPieceMaster, info) || !grabbedByPlayer.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 vector;
			Quaternion quaternion;
			BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				bool isMasterClient = info.Sender.IsMasterClient;
				bool flag = isMasterClient || table.ValidateGrabPieceParams(pieceId, isLeftHand, vector, quaternion, NetPlayer.Get(grabbedByPlayer));
				if (flag)
				{
					flag &= isMasterClient || table.ValidateGrabPieceState(pieceId, isLeftHand, vector, quaternion, grabbedByPlayer);
				}
				if (flag)
				{
					if (!info.Sender.IsMasterClient)
					{
						this.CheckForFreedPlot(pieceId, grabbedByPlayer);
					}
					base.photonView.RPC("PieceGrabbedRPC", RpcTarget.All, new object[] { localCommandId, pieceId, isLeftHand, packedPosRot, grabbedByPlayer });
					return;
				}
				base.photonView.RPC("RequestFailedRPC", info.Sender, new object[] { localCommandId });
			}
		}

		// Token: 0x060044AC RID: 17580 RVA: 0x00144F24 File Offset: 0x00143124
		private void CheckForFreedPlot(int pieceId, Player grabbedByPlayer)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderPiece piece = this.GetTable().GetPiece(pieceId);
			if (piece != null && piece.parentPiece != null && piece.parentPiece.IsPrivatePlot() && piece.parentPiece.firstChildPiece.Equals(piece) && piece.nextSiblingPiece == null)
			{
				base.photonView.RPC("PlotClaimedRPC", RpcTarget.All, new object[]
				{
					piece.parentPiece.pieceId,
					grabbedByPlayer,
					false
				});
			}
		}

		// Token: 0x060044AD RID: 17581 RVA: 0x00144FC4 File Offset: 0x001431C4
		[PunRPC]
		public void PieceGrabbedRPC(int localCommandId, int pieceId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceGrabbedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.GrabPiece, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			Vector3 vector;
			Quaternion quaternion;
			BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
			table.GrabPiece(localCommandId, pieceId, isLeftHand, vector, quaternion, NetPlayer.Get(grabbedByPlayer), false);
		}

		// Token: 0x060044AE RID: 17582 RVA: 0x00145028 File Offset: 0x00143228
		public void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
		{
			if (piece == null)
			{
				return;
			}
			int pieceId = piece.pieceId;
			float num = 10000f;
			if ((in velocity).IsValid(in num) && velocity.sqrMagnitude > BuilderTable.MAX_DROP_VELOCITY * BuilderTable.MAX_DROP_VELOCITY)
			{
				velocity = velocity.normalized * BuilderTable.MAX_DROP_VELOCITY;
			}
			num = 10000f;
			if ((in angVelocity).IsValid(in num) && angVelocity.sqrMagnitude > BuilderTable.MAX_DROP_ANG_VELOCITY * BuilderTable.MAX_DROP_ANG_VELOCITY)
			{
				angVelocity = angVelocity.normalized * BuilderTable.MAX_DROP_ANG_VELOCITY;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidateDropPieceParams(pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer)))
			{
				return;
			}
			int num2 = this.CreateLocalCommandId();
			table.DropPiece(num2, pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer), true);
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestDropPieceRPC", RpcTarget.MasterClient, new object[]
				{
					num2,
					pieceId,
					position,
					rotation,
					velocity,
					angVelocity,
					PhotonNetwork.LocalPlayer
				});
			}
		}

		// Token: 0x060044AF RID: 17583 RVA: 0x00145160 File Offset: 0x00143360
		[PunRPC]
		public void RequestDropPieceRPC(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestDropPieceRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.DropPieceMaster, info) || !droppedByPlayer.Equals(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			bool isMasterClient = info.Sender.IsMasterClient;
			bool flag = isMasterClient || table.ValidateDropPieceParams(pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(droppedByPlayer));
			if (flag)
			{
				flag &= isMasterClient || table.ValidateDropPieceState(pieceId, position, rotation, velocity, angVelocity, droppedByPlayer);
			}
			if (flag)
			{
				base.photonView.RPC("PieceDroppedRPC", RpcTarget.All, new object[] { localCommandId, pieceId, position, rotation, velocity, angVelocity, droppedByPlayer });
				return;
			}
			base.photonView.RPC("RequestFailedRPC", info.Sender, new object[] { localCommandId });
		}

		// Token: 0x060044B0 RID: 17584 RVA: 0x0014528C File Offset: 0x0014348C
		[PunRPC]
		public void PieceDroppedRPC(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceDroppedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.DropPiece, info))
			{
				return;
			}
			float num = 10000f;
			if ((in position).IsValid(in num) && (in rotation).IsValid())
			{
				float num2 = 10000f;
				if ((in velocity).IsValid(in num2))
				{
					float num3 = 10000f;
					if ((in angVelocity).IsValid(in num3))
					{
						BuilderTable table = this.GetTable();
						if (!table.isTableMutable)
						{
							return;
						}
						table.DropPiece(localCommandId, pieceId, position, rotation, velocity, angVelocity, NetPlayer.Get(droppedByPlayer), false);
						return;
					}
				}
			}
		}

		// Token: 0x060044B1 RID: 17585 RVA: 0x00145328 File Offset: 0x00143528
		public void PieceEnteredDropZone(BuilderPiece piece, BuilderDropZone.DropType dropType, int dropZoneId)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			BuilderPiece rootPiece = piece.GetRootPiece();
			if (!table.ValidateRepelPiece(rootPiece))
			{
				return;
			}
			long num = BitPackUtils.PackWorldPosForNetwork(rootPiece.transform.position);
			int num2 = BitPackUtils.PackQuaternionForNetwork(rootPiece.transform.rotation);
			base.photonView.RPC("PieceEnteredDropZoneRPC", RpcTarget.All, new object[] { rootPiece.pieceId, num, num2, dropZoneId });
		}

		// Token: 0x060044B2 RID: 17586 RVA: 0x001453C0 File Offset: 0x001435C0
		[PunRPC]
		public void PieceEnteredDropZoneRPC(int pieceId, long position, int rotation, int dropZoneId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PieceEnteredDropZoneRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PieceDropZone, info))
			{
				return;
			}
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(position);
			float num = 10000f;
			if (!(in vector).IsValid(in num))
			{
				return;
			}
			Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(rotation);
			if (!(in quaternion).IsValid())
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			table.PieceEnteredDropZone(pieceId, vector, quaternion, dropZoneId);
		}

		// Token: 0x060044B3 RID: 17587 RVA: 0x0014543C File Offset: 0x0014363C
		[PunRPC]
		public void PlotClaimedRPC(int pieceId, Player claimingPlayer, bool claimed, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlotClaimedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.PlotClaimedMaster, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (claimed)
			{
				table.PlotClaimed(pieceId, claimingPlayer);
				return;
			}
			table.PlotFreed(pieceId, claimingPlayer);
		}

		// Token: 0x060044B4 RID: 17588 RVA: 0x00145498 File Offset: 0x00143698
		public void RequestCreateArmShelfForPlayer(Player player)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				if (!this.armShelfRequests.Contains(player))
				{
					this.armShelfRequests.Add(player);
				}
				return;
			}
			if (table.playerToArmShelfLeft.ContainsKey(player.ActorNumber))
			{
				return;
			}
			int num = table.CreatePieceId();
			int num2 = table.CreatePieceId();
			int staticHash = table.armShelfPieceType.name.GetStaticHash();
			base.photonView.RPC("ArmShelfCreatedRPC", RpcTarget.All, new object[] { num, num2, staticHash, player });
		}

		// Token: 0x060044B5 RID: 17589 RVA: 0x0014554C File Offset: 0x0014374C
		[PunRPC]
		public void ArmShelfCreatedRPC(int pieceIdLeft, int pieceIdRight, int pieceType, Player owningPlayer, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ArmShelfCreatedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.ArmShelfCreated, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (pieceType != table.armShelfPieceType.name.GetStaticHash())
			{
				return;
			}
			table.CreateArmShelf(pieceIdLeft, pieceIdRight, pieceType, owningPlayer);
		}

		// Token: 0x060044B6 RID: 17590 RVA: 0x001455B0 File Offset: 0x001437B0
		public void RequestShelfSelection(int shelfID, int setId, bool isConveyor)
		{
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (isConveyor)
			{
				if (shelfID < 0 || shelfID >= table.conveyors.Count)
				{
					return;
				}
			}
			else if (shelfID < 0 || shelfID >= table.dispenserShelves.Count)
			{
				return;
			}
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestShelfSelectionRPC", RpcTarget.MasterClient, new object[] { shelfID, setId, isConveyor });
			}
		}

		// Token: 0x060044B7 RID: 17591 RVA: 0x00145634 File Offset: 0x00143834
		[PunRPC]
		public void RequestShelfSelectionRPC(int shelfId, int setId, bool isConveyor, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestShelfSelectionRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.ShelfSelection, info))
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (!table.ValidateShelfSelectionParams(shelfId, setId, isConveyor, info.Sender))
			{
				return;
			}
			base.photonView.RPC("ShelfSelectionChangedRPC", RpcTarget.All, new object[] { shelfId, setId, isConveyor, info.Sender });
		}

		// Token: 0x060044B8 RID: 17592 RVA: 0x001456D4 File Offset: 0x001438D4
		[PunRPC]
		public void ShelfSelectionChangedRPC(int shelfId, int setId, bool isConveyor, Player caller, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ShelfSelectionChangedRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.ShelfSelectionMaster, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (!table.isTableMutable)
			{
				return;
			}
			if (shelfId < 0 || ((!isConveyor || shelfId >= table.conveyors.Count) && (isConveyor || shelfId >= table.dispenserShelves.Count)))
			{
				return;
			}
			table.ChangeSetSelection(shelfId, setId, isConveyor);
		}

		// Token: 0x060044B9 RID: 17593 RVA: 0x00145754 File Offset: 0x00143954
		public void RequestFunctionalPieceStateChange(int pieceID, byte state)
		{
			BuilderTable table = this.GetTable();
			if (!table.ValidateFunctionalPieceState(pieceID, state, NetworkSystem.Instance.LocalPlayer))
			{
				return;
			}
			if (table.GetTableState() == BuilderTable.TableState.Ready)
			{
				base.photonView.RPC("RequestFunctionalPieceStateChangeRPC", RpcTarget.MasterClient, new object[] { pieceID, state });
			}
		}

		// Token: 0x060044BA RID: 17594 RVA: 0x001457B0 File Offset: 0x001439B0
		[PunRPC]
		public void RequestFunctionalPieceStateChangeRPC(int pieceID, byte state, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestFunctionalPieceStateChangeRPC");
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateMasterClientIsReady(info.Sender))
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SetFunctionalState, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.Ready)
			{
				return;
			}
			if (table.ValidateFunctionalPieceState(pieceID, state, NetPlayer.Get(info.Sender)))
			{
				table.OnFunctionalStateRequest(pieceID, state, NetPlayer.Get(info.Sender), info.SentServerTimestamp);
			}
		}

		// Token: 0x060044BB RID: 17595 RVA: 0x0014582C File Offset: 0x00143A2C
		public void FunctionalPieceStateChangeMaster(int pieceID, byte state, Player instigator, int timeStamp)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.ValidateFunctionalPieceState(pieceID, state, NetPlayer.Get(instigator)) && state != table.GetPiece(pieceID).functionalPieceState)
			{
				base.photonView.RPC("FunctionalPieceStateChangeRPC", RpcTarget.All, new object[] { pieceID, state, instigator, timeStamp });
			}
		}

		// Token: 0x060044BC RID: 17596 RVA: 0x001458A0 File Offset: 0x00143AA0
		[PunRPC]
		public void FunctionalPieceStateChangeRPC(int pieceID, byte state, Player caller, int timeStamp, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "FunctionalPieceStateChangeRPC");
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SetFunctionalStateMaster, info))
			{
				return;
			}
			if (caller == null)
			{
				return;
			}
			if ((ulong)(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout) || (ulong)(info.SentServerTimestamp - timeStamp) > (ulong)((long)PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout))
			{
				timeStamp = PhotonNetwork.ServerTimestamp;
			}
			BuilderTable table = this.GetTable();
			if (table.ValidateFunctionalPieceState(pieceID, state, NetPlayer.Get(info.Sender)))
			{
				table.SetFunctionalPieceState(pieceID, state, NetPlayer.Get(caller), timeStamp);
			}
		}

		// Token: 0x060044BD RID: 17597 RVA: 0x0014594C File Offset: 0x00143B4C
		public void RequestBlocksTerminalControl(bool locked)
		{
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			if (table.linkedTerminal.IsTerminalLocked == locked)
			{
				return;
			}
			base.photonView.RPC("RequestBlocksTerminalControlRPC", RpcTarget.MasterClient, new object[] { locked });
		}

		// Token: 0x060044BE RID: 17598 RVA: 0x001459A8 File Offset: 0x00143BA8
		[PunRPC]
		private void RequestBlocksTerminalControlRPC(bool lockedStatus, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RequestBlocksTerminalControlRPC");
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.RequestTerminalControl, info))
			{
				return;
			}
			if (info.Sender == null)
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			RigContainer rigContainer;
			if (!(VRRigCache.Instance != null) || !VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				return;
			}
			if ((table.linkedTerminal.transform.position - rigContainer.Rig.bodyTransform.position).sqrMagnitude > 9f)
			{
				return;
			}
			if (table.linkedTerminal.ValidateTerminalControlRequest(lockedStatus, info.Sender.ActorNumber))
			{
				int num = (lockedStatus ? info.Sender.ActorNumber : (-2));
				base.photonView.RPC("SetBlocksTerminalDriverRPC", RpcTarget.All, new object[] { num });
			}
		}

		// Token: 0x060044BF RID: 17599 RVA: 0x00145AA4 File Offset: 0x00143CA4
		[PunRPC]
		private void SetBlocksTerminalDriverRPC(int driver, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SetBlocksTerminalDriverRPC");
			if (info.Sender == null || !info.Sender.IsMasterClient)
			{
				return;
			}
			if (driver != -2 && NetworkSystem.Instance.GetPlayer(driver) == null)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SetTerminalDriver, info))
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			table.linkedTerminal.SetTerminalDriver(driver);
		}

		// Token: 0x060044C0 RID: 17600 RVA: 0x00145B1B File Offset: 0x00143D1B
		public void RequestLoadSharedBlocksMap(string mapID)
		{
			base.photonView.RPC("LoadSharedBlocksMapRPC", RpcTarget.MasterClient, new object[] { mapID });
		}

		// Token: 0x060044C1 RID: 17601 RVA: 0x00145B38 File Offset: 0x00143D38
		[PunRPC]
		private void LoadSharedBlocksMapRPC(string mapID, PhotonMessageInfo info)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "LoadSharedBlocksMapRPC");
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.LoadSharedBlocksMap, info))
			{
				return;
			}
			if (info.Sender == null || mapID.IsNullOrEmpty())
			{
				return;
			}
			BuilderTable table = this.GetTable();
			if (table.isTableMutable || table.linkedTerminal == null)
			{
				return;
			}
			if (!table.linkedTerminal.ValidateLoadMapRequest(mapID, info.Sender.ActorNumber))
			{
				GTDev.LogWarning<string>("SharedBlocks ValidateLoadMapRequest fail", null);
				return;
			}
			BuilderTable.TableState tableState = table.GetTableState();
			if (tableState == BuilderTable.TableState.Ready || tableState == BuilderTable.TableState.BadData)
			{
				table.SetPendingMap(mapID);
				base.photonView.RPC("SharedTableEventRPC", RpcTarget.Others, new object[] { 0, mapID });
				this.localClientTableInit.Reset();
				UnityEvent onMapCleared = table.OnMapCleared;
				if (onMapCleared != null)
				{
					onMapCleared.Invoke();
				}
				table.SetTableState(BuilderTable.TableState.WaitingForSharedMapLoad);
				table.FindAndLoadSharedBlocksMap(mapID);
				return;
			}
			GTDev.LogWarning<string>("SharedBlocks Invalid state " + tableState.ToString(), null);
			this.LoadSharedBlocksFailedMaster(mapID);
		}

		// Token: 0x060044C2 RID: 17602 RVA: 0x00145C4D File Offset: 0x00143E4D
		public void LoadSharedBlocksFailedMaster(string mapID)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (mapID.Length > 8)
			{
				return;
			}
			base.photonView.RPC("SharedTableEventRPC", RpcTarget.All, new object[] { 1, mapID });
		}

		// Token: 0x060044C3 RID: 17603 RVA: 0x00145C8A File Offset: 0x00143E8A
		public void SharedBlocksOutOfBoundsMaster(string mapID)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (mapID.Length > 8)
			{
				return;
			}
			base.photonView.RPC("SharedTableEventRPC", RpcTarget.All, new object[] { 2, mapID });
		}

		// Token: 0x060044C4 RID: 17604 RVA: 0x00145CC8 File Offset: 0x00143EC8
		[PunRPC]
		private void SharedTableEventRPC(byte eventType, string mapID, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SharedTableEventRPC");
			if (eventType >= 3)
			{
				return;
			}
			if (!SharedBlocksManager.IsMapIDValid(mapID) && eventType != 1)
			{
				GTDev.LogWarning<string>("BuilderTableNetworking SharedTableEventRPC Invalid Map ID", null);
				return;
			}
			if (info.Sender == null || !info.Sender.IsMasterClient)
			{
				return;
			}
			if (!this.ValidateCallLimits(BuilderTableNetworking.RPC.SharedTableEvent, info))
			{
				GTDev.LogError<string>("SharedTableEventRPC Failed call limits", null);
				return;
			}
			if (this.GetTable().isTableMutable)
			{
				return;
			}
			switch (eventType)
			{
			case 0:
				this.OnSharedBlocksLoadStarted(mapID);
				return;
			case 1:
				this.OnLoadSharedBlocksFailed(mapID);
				return;
			case 2:
				this.OnSharedBlocksOutOfBounds(mapID);
				return;
			default:
				return;
			}
		}

		// Token: 0x060044C5 RID: 17605 RVA: 0x00145D64 File Offset: 0x00143F64
		private void OnSharedBlocksLoadStarted(string mapID)
		{
			this.localClientTableInit.Reset();
			BuilderTable table = this.GetTable();
			if (table.GetTableState() != BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				table.ClearTable();
				table.ClearQueuedCommands();
				table.SetPendingMap(mapID);
				table.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.PlayerEnterBuilder();
			}
		}

		// Token: 0x060044C6 RID: 17606 RVA: 0x00145DAC File Offset: 0x00143FAC
		private void OnLoadSharedBlocksFailed(string mapID)
		{
			BuilderTable table = this.GetTable();
			string pendingMap = table.GetPendingMap();
			if (!pendingMap.IsNullOrEmpty() && !pendingMap.Equals(mapID))
			{
				GTDev.LogWarning<string>("BuilderTableNetworking OnLoadSharedBlocksFailed Unexpected map ID " + mapID, null);
			}
			BuilderTable.TableState tableState = table.GetTableState();
			if (!NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitForMasterResync && tableState != BuilderTable.TableState.WaitingForInitalBuild && tableState != BuilderTable.TableState.Ready && tableState != BuilderTable.TableState.BadData)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnLoadSharedBlocksFailed Unexpected table state {0}", tableState), null);
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitingForSharedMapLoad && tableState != BuilderTable.TableState.WaitForInitialBuildMaster && tableState != BuilderTable.TableState.Ready && tableState != BuilderTable.TableState.BadData)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnLoadSharedBlocksFailed Unexpected table state {0}", tableState), null);
				return;
			}
			table.SetPendingMap(null);
			if (table != null && !table.isTableMutable && table.linkedTerminal != null)
			{
				if (!SharedBlocksManager.IsMapIDValid(mapID))
				{
					UnityEvent<string> onMapLoadFailed = table.OnMapLoadFailed;
					if (onMapLoadFailed == null)
					{
						return;
					}
					onMapLoadFailed.Invoke("BAD MAP ID");
					return;
				}
				else
				{
					UnityEvent<string> onMapLoadFailed2 = table.OnMapLoadFailed;
					if (onMapLoadFailed2 == null)
					{
						return;
					}
					onMapLoadFailed2.Invoke("LOAD FAILED");
				}
			}
		}

		// Token: 0x060044C7 RID: 17607 RVA: 0x00145EB4 File Offset: 0x001440B4
		private void OnSharedBlocksOutOfBounds(string mapID)
		{
			BuilderTable table = this.GetTable();
			string pendingMap = table.GetPendingMap();
			if (!pendingMap.IsNullOrEmpty() && !pendingMap.Equals(mapID))
			{
				GTDev.LogWarning<string>("BuilderTableNetworking OnSharedBlocksOutOfBounds Unexpected map ID " + mapID, null);
			}
			BuilderTable.TableState tableState = table.GetTableState();
			if (!NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitForMasterResync && tableState != BuilderTable.TableState.WaitingForInitalBuild)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnSharedBlocksOutOfBounds Unexpected table state {0}", tableState), null);
				return;
			}
			if (NetworkSystem.Instance.IsMasterClient && tableState != BuilderTable.TableState.WaitForInitialBuildMaster && tableState != BuilderTable.TableState.BadData)
			{
				GTDev.LogWarning<string>(string.Format("BuilderTableNetworking OnSharedBlocksOutOfBounds Unexpected table state {0}", tableState), null);
				return;
			}
			table.SetPendingMap(null);
			if (table != null && !table.isTableMutable && table.linkedTerminal != null)
			{
				UnityEvent<string> onMapLoadFailed = table.OnMapLoadFailed;
				if (onMapLoadFailed == null)
				{
					return;
				}
				onMapLoadFailed.Invoke("BLOCKS ARE OUT OF BOUNDS FOR SHARED BLOCKS ROOM");
			}
		}

		// Token: 0x060044C8 RID: 17608 RVA: 0x000023F4 File Offset: 0x000005F4
		public void RequestPaintPiece(int pieceID, int materialType)
		{
		}

		// Token: 0x04004749 RID: 18249
		public PhotonView tablePhotonView;

		// Token: 0x0400474A RID: 18250
		private const int MAX_TABLE_BYTES = 1048576;

		// Token: 0x0400474B RID: 18251
		private const int MAX_TABLE_CHUNK_BYTES = 1000;

		// Token: 0x0400474C RID: 18252
		private const float DELAY_CLIENT_TABLE_CREATION_TIME = 1f;

		// Token: 0x0400474D RID: 18253
		private const float SEND_INIT_DATA_COOLDOWN = 0f;

		// Token: 0x0400474E RID: 18254
		private const int PIECE_SYNC_BYTES = 128;

		// Token: 0x0400474F RID: 18255
		private BuilderTable currTable;

		// Token: 0x04004750 RID: 18256
		private int nextLocalCommandId;

		// Token: 0x04004751 RID: 18257
		private List<BuilderTableNetworking.PlayerTableInitState> masterClientTableInit;

		// Token: 0x04004752 RID: 18258
		private List<BuilderTableNetworking.PlayerTableInitState> masterClientTableValidators;

		// Token: 0x04004753 RID: 18259
		private BuilderTableNetworking.PlayerTableInitState localClientTableInit;

		// Token: 0x04004754 RID: 18260
		private BuilderTableNetworking.PlayerTableInitState localValidationTable;

		// Token: 0x04004755 RID: 18261
		[HideInInspector]
		public List<Player> armShelfRequests;

		// Token: 0x04004756 RID: 18262
		private CallLimiter[] callLimiters;

		// Token: 0x02000AF8 RID: 2808
		public class PlayerTableInitState
		{
			// Token: 0x060044CA RID: 17610 RVA: 0x00145F8A File Offset: 0x0014418A
			public PlayerTableInitState()
			{
				this.serializedTableState = new byte[1048576];
				this.chunk = new byte[1000];
				this.Reset();
			}

			// Token: 0x060044CB RID: 17611 RVA: 0x00145FB8 File Offset: 0x001441B8
			public void Reset()
			{
				this.player = null;
				this.numSerializedBytes = 0;
				this.totalSerializedBytes = 0;
			}

			// Token: 0x04004757 RID: 18263
			public Player player;

			// Token: 0x04004758 RID: 18264
			public int numSerializedBytes;

			// Token: 0x04004759 RID: 18265
			public int totalSerializedBytes;

			// Token: 0x0400475A RID: 18266
			public byte[] serializedTableState;

			// Token: 0x0400475B RID: 18267
			public byte[] chunk;

			// Token: 0x0400475C RID: 18268
			public float waitForInitTimeRemaining;

			// Token: 0x0400475D RID: 18269
			public float sendNextChunkTimeRemaining;
		}

		// Token: 0x02000AF9 RID: 2809
		private enum RPC
		{
			// Token: 0x0400475F RID: 18271
			PlayerEnterMaster,
			// Token: 0x04004760 RID: 18272
			TableDataMaster,
			// Token: 0x04004761 RID: 18273
			TableData,
			// Token: 0x04004762 RID: 18274
			TableDataStart,
			// Token: 0x04004763 RID: 18275
			PlacePieceMaster,
			// Token: 0x04004764 RID: 18276
			PlacePiece,
			// Token: 0x04004765 RID: 18277
			GrabPieceMaster,
			// Token: 0x04004766 RID: 18278
			GrabPiece,
			// Token: 0x04004767 RID: 18279
			DropPieceMaster,
			// Token: 0x04004768 RID: 18280
			DropPiece,
			// Token: 0x04004769 RID: 18281
			RequestFailed,
			// Token: 0x0400476A RID: 18282
			PieceDropZone,
			// Token: 0x0400476B RID: 18283
			CreatePiece,
			// Token: 0x0400476C RID: 18284
			CreatePieceMaster,
			// Token: 0x0400476D RID: 18285
			CreateShelfPieceMaster,
			// Token: 0x0400476E RID: 18286
			RecyclePieceMaster,
			// Token: 0x0400476F RID: 18287
			PlotClaimedMaster,
			// Token: 0x04004770 RID: 18288
			ArmShelfCreated,
			// Token: 0x04004771 RID: 18289
			ShelfSelection,
			// Token: 0x04004772 RID: 18290
			ShelfSelectionMaster,
			// Token: 0x04004773 RID: 18291
			SetFunctionalState,
			// Token: 0x04004774 RID: 18292
			SetFunctionalStateMaster,
			// Token: 0x04004775 RID: 18293
			RequestTerminalControl,
			// Token: 0x04004776 RID: 18294
			SetTerminalDriver,
			// Token: 0x04004777 RID: 18295
			LoadSharedBlocksMap,
			// Token: 0x04004778 RID: 18296
			SharedTableEvent,
			// Token: 0x04004779 RID: 18297
			Count
		}

		// Token: 0x02000AFA RID: 2810
		private enum SharedTableEventTypes
		{
			// Token: 0x0400477B RID: 18299
			LOAD_STARTED,
			// Token: 0x0400477C RID: 18300
			LOAD_FAILED,
			// Token: 0x0400477D RID: 18301
			OUT_OF_BOUNDS,
			// Token: 0x0400477E RID: 18302
			COUNT
		}
	}
}
