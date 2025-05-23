using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using Fusion;
using GorillaExtensions;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000566 RID: 1382
[NetworkBehaviourWeaved(0)]
public class GameEntityManager : NetworkComponent, IMatchmakingCallbacks, IInRoomCallbacks, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x1400004F RID: 79
	// (add) Token: 0x06002185 RID: 8581 RVA: 0x000A764C File Offset: 0x000A584C
	// (remove) Token: 0x06002186 RID: 8582 RVA: 0x000A7684 File Offset: 0x000A5884
	public event GameEntityManager.ZoneStartEvent onZoneStart;

	// Token: 0x14000050 RID: 80
	// (add) Token: 0x06002187 RID: 8583 RVA: 0x000A76BC File Offset: 0x000A58BC
	// (remove) Token: 0x06002188 RID: 8584 RVA: 0x000A76F4 File Offset: 0x000A58F4
	public event GameEntityManager.ZoneClearEvent onZoneClear;

	// Token: 0x06002189 RID: 8585 RVA: 0x000A772C File Offset: 0x000A592C
	protected override void Awake()
	{
		base.Awake();
		GameEntityManager.instance = this;
		this.entities = new List<GameEntity>(64);
		this.gameEntityData = new List<GameEntityData>(64);
		this.netIdToIndex = new Dictionary<int, int>(16384);
		this.netIds = new NativeArray<int>(16384, Unity.Collections.Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.zoneIds = new NativeArray<int>(16384, Unity.Collections.Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.createdItemTypeCount = new Dictionary<int, int>();
		this.zoneStateData = new GameEntityManager.ZoneStateData
		{
			zone = GTZone.ghostReactor,
			zoneStateRequests = new List<GameEntityManager.ZoneStateRequest>(),
			zonePlayers = new List<Player>(),
			recievedStateBytes = new byte[15360],
			numRecievedStateBytes = 0
		};
		this.BuildFactory();
		this.guard.AddCallbackTarget(GameEntityManager.instance);
		this.netIdsForCreate = new List<int>();
		this.entityTypeIdsForCreate = new List<int>();
		this.zoneIdsForCreate = new List<int>();
		this.packedPositionsForCreate = new List<long>();
		this.packedRotationsForCreate = new List<int>();
		this.createDataForCreate = new List<long>();
		this.netIdsForDelete = new List<int>();
		this.netIdsForState = new List<int>();
		this.statesForState = new List<long>();
	}

	// Token: 0x0600218A RID: 8586 RVA: 0x000A785B File Offset: 0x000A5A5B
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.netIds.Dispose();
		this.zoneIds.Dispose();
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x000A787C File Offset: 0x000A5A7C
	private void Update()
	{
		this.UpdateZoneState();
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.netIdsForCreate.Count > 0 && Time.time > this.lastCreateSent + this.createCooldown)
		{
			this.lastCreateSent = Time.time;
			this.photonView.RPC("CreateItemRPC", RpcTarget.Others, new object[]
			{
				this.netIdsForCreate.ToArray(),
				this.zoneIdsForCreate.ToArray(),
				this.entityTypeIdsForCreate.ToArray(),
				this.packedPositionsForCreate.ToArray(),
				this.packedRotationsForCreate.ToArray(),
				this.createDataForCreate.ToArray()
			});
			this.netIdsForCreate.Clear();
			this.zoneIdsForCreate.Clear();
			this.entityTypeIdsForCreate.Clear();
			this.packedPositionsForCreate.Clear();
			this.packedRotationsForCreate.Clear();
			this.createDataForCreate.Clear();
		}
		if (this.netIdsForDelete.Count > 0 && Time.time > this.lastDestroySent + this.destroyCooldown)
		{
			this.lastDestroySent = Time.time;
			this.photonView.RPC("DestroyItemRPC", RpcTarget.Others, new object[] { this.netIdsForDelete.ToArray() });
			this.netIdsForDelete.Clear();
		}
		if (this.netIdsForState.Count > 0 && Time.time > this.lastStateSent + this.stateCooldown)
		{
			this.lastDestroySent = Time.time;
			this.photonView.RPC("ApplyStateRPC", RpcTarget.All, new object[]
			{
				this.netIdsForState.ToArray(),
				this.statesForState.ToArray()
			});
			this.netIdsForState.Clear();
			this.statesForState.Clear();
		}
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x000A7A4C File Offset: 0x000A5C4C
	public GameEntityId AddGameEntity(int netId, int zoneId, GameEntity gameEntity)
	{
		int num = this.FindNewEntityIndex();
		this.entities[num] = gameEntity;
		GameEntityData gameEntityData = default(GameEntityData);
		this.gameEntityData.Add(gameEntityData);
		gameEntity.id = new GameEntityId
		{
			index = num
		};
		this.netIdToIndex[netId] = num;
		this.netIds[num] = netId;
		this.zoneIds[num] = zoneId;
		return gameEntity.id;
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x000A7AC8 File Offset: 0x000A5CC8
	private int FindNewEntityIndex()
	{
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (this.entities[i] == null)
			{
				return i;
			}
		}
		this.entities.Add(null);
		return this.entities.Count - 1;
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x000A7B1C File Offset: 0x000A5D1C
	public void RemoveGameEntity(GameEntity entity)
	{
		int index = entity.id.index;
		if (index < 0 || index > this.entities.Count)
		{
			return;
		}
		if (this.entities[index] == entity)
		{
			this.entities[index] = null;
		}
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x000A7B69 File Offset: 0x000A5D69
	public List<GameEntity> GetGameEntities()
	{
		return this.entities;
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x000A7B74 File Offset: 0x000A5D74
	public static bool IsValidNetId(int netId)
	{
		int num;
		return GameEntityManager.instance != null && GameEntityManager.instance.netIdToIndex.TryGetValue(netId, out num) && num >= 0 && num < GameEntityManager.instance.entities.Count;
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x000A7BC0 File Offset: 0x000A5DC0
	public int FindOpenIndex()
	{
		for (int i = 0; i < this.netIds.Length; i++)
		{
			if (this.netIds[i] != -1)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002192 RID: 8594 RVA: 0x000A7BF8 File Offset: 0x000A5DF8
	public GameEntityId GetEntityIdFromNetId(int netId)
	{
		int num;
		if (this.netIdToIndex.TryGetValue(netId, out num))
		{
			return new GameEntityId
			{
				index = num
			};
		}
		return GameEntityId.Invalid;
	}

	// Token: 0x06002193 RID: 8595 RVA: 0x000A7C2C File Offset: 0x000A5E2C
	public int GetNetIdFromEntityId(GameEntityId id)
	{
		if (id.index < 0 || id.index >= this.netIds.Length)
		{
			return -1;
		}
		return this.netIds[id.index];
	}

	// Token: 0x06002194 RID: 8596 RVA: 0x000A7C5D File Offset: 0x000A5E5D
	private int GetPlayerZoneId()
	{
		return 5;
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x000A7C60 File Offset: 0x000A5E60
	public bool IsAuthority()
	{
		return !NetworkSystem.Instance.InRoom || this.guard.isTrulyMine;
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x000A7C7B File Offset: 0x000A5E7B
	public bool IsAuthorityPlayer(NetPlayer player)
	{
		return player != null && this.IsAuthorityPlayer(player.GetPlayerRef());
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x000A7C8E File Offset: 0x000A5E8E
	public bool IsAuthorityPlayer(Player player)
	{
		return player != null && this.guard.actualOwner != null && player == this.guard.actualOwner.GetPlayerRef();
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x000A7CB5 File Offset: 0x000A5EB5
	public bool IsZoneAuthority(int zoneId)
	{
		return this.IsAuthority();
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x000A7CBD File Offset: 0x000A5EBD
	public bool HasAuthority()
	{
		return this.GetAuthorityPlayer() != null;
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000A7CC8 File Offset: 0x000A5EC8
	public Player GetAuthorityPlayer()
	{
		if (this.guard.actualOwner != null)
		{
			return this.guard.actualOwner.GetPlayerRef();
		}
		return null;
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000A7CE9 File Offset: 0x000A5EE9
	public bool IsZoneActive()
	{
		return this.zoneStateData.state == GameEntityManager.ZoneState.Active;
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000A7CFC File Offset: 0x000A5EFC
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.zoneLimit == null || this.zoneLimit.bounds.Contains(pos);
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x000A7D2D File Offset: 0x000A5F2D
	public bool IsValidClientRPC(Player sender)
	{
		return this.IsAuthorityPlayer(sender) && this.IsZoneActive();
	}

	// Token: 0x0600219E RID: 8606 RVA: 0x000A7D40 File Offset: 0x000A5F40
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.IsValidClientRPC(sender) && GameEntityManager.IsValidNetId(entityNetId);
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x000A7D53 File Offset: 0x000A5F53
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.IsValidClientRPC(sender, entityNetId) && this.IsPositionInZone(pos);
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x000A7D68 File Offset: 0x000A5F68
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.IsValidClientRPC(sender) && this.IsPositionInZone(pos);
	}

	// Token: 0x060021A1 RID: 8609 RVA: 0x000A7D7C File Offset: 0x000A5F7C
	public bool IsValidAuthorityRPC()
	{
		return this.IsAuthority() && this.IsZoneActive();
	}

	// Token: 0x060021A2 RID: 8610 RVA: 0x000A7D8E File Offset: 0x000A5F8E
	public bool IsValidAuthorityRPC(int entityNetId)
	{
		return this.IsValidAuthorityRPC() && GameEntityManager.IsValidNetId(entityNetId);
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x000A7DA0 File Offset: 0x000A5FA0
	public bool IsValidAuthorityRPC(int entityNetId, Vector3 pos)
	{
		return this.IsValidAuthorityRPC(entityNetId) && this.IsPositionInZone(pos);
	}

	// Token: 0x060021A4 RID: 8612 RVA: 0x000A7DB4 File Offset: 0x000A5FB4
	public bool IsValidAuthorityRPC(Vector3 pos)
	{
		return this.IsValidAuthorityRPC() && this.IsPositionInZone(pos);
	}

	// Token: 0x060021A5 RID: 8613 RVA: 0x000A7DC7 File Offset: 0x000A5FC7
	public bool IsValidEntity(GameEntityId id)
	{
		return this.GetGameEntity(id) != null;
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x000A7DD8 File Offset: 0x000A5FD8
	public GameEntity GetGameEntity(GameEntityId id)
	{
		if (!id.IsValid())
		{
			return null;
		}
		int index = id.index;
		if (index < 0 || index >= this.entities.Count)
		{
			Debug.LogErrorFormat("Cannot Get Game Entity Index {0} {1}", new object[]
			{
				id.index,
				this.entities.Count
			});
			return null;
		}
		return this.entities[index];
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x000A7E48 File Offset: 0x000A6048
	private void BuildFactory()
	{
		this.itemPrefabFactory = new Dictionary<int, GameObject>(1024);
		this.priceLookupByEntityId = new Dictionary<int, int>();
		for (int i = 0; i < this.tempFactoryItems.Count; i++)
		{
			GameObject gameObject = this.tempFactoryItems[i].gameObject;
			int staticHash = gameObject.name.GetStaticHash();
			if (gameObject.GetComponent<GRToolLantern>())
			{
				this.priceLookupByEntityId.Add(staticHash, 50);
			}
			else if (gameObject.GetComponent<GRToolCollector>())
			{
				this.priceLookupByEntityId.Add(staticHash, 50);
			}
			Debug.LogFormat("Entity Factory {0} {1}", new object[] { gameObject.name, staticHash });
			this.itemPrefabFactory.Add(staticHash, gameObject);
		}
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x000A7F11 File Offset: 0x000A6111
	private int CreateNetId()
	{
		int num = this.nextNetId;
		this.nextNetId++;
		return num;
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x000A7F28 File Offset: 0x000A6128
	public void RequestCreateItem(int entityTypeId, Vector3 position, Quaternion rotation, long createData)
	{
		int num = 5;
		if (!this.IsZoneAuthority(num) || !this.IsZoneActive() || !this.IsPositionInZone(position))
		{
			return;
		}
		long num2 = BitPackUtils.PackWorldPosForNetwork(position);
		int num3 = BitPackUtils.PackQuaternionForNetwork(rotation);
		int num4 = this.CreateNetId();
		this.netIdsForCreate.Add(num4);
		this.zoneIdsForCreate.Add(num);
		this.entityTypeIdsForCreate.Add(entityTypeId);
		this.packedPositionsForCreate.Add(num2);
		this.packedRotationsForCreate.Add(num3);
		this.createDataForCreate.Add(createData);
		this.CreateItemLocal(num4, num, entityTypeId, position, rotation, createData);
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x000A7FC0 File Offset: 0x000A61C0
	[PunRPC]
	public void CreateItemRPC(int[] netId, int[] zoneId, int[] entityTypeId, long[] packedPos, int[] packedRot, long[] createData, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.CreateItem))
		{
			return;
		}
		if (netId.Length != zoneId.Length || netId.Length != entityTypeId.Length || netId.Length != packedPos.Length || netId.Length != packedRot.Length || netId.Length != createData.Length)
		{
			return;
		}
		for (int i = 0; i < netId.Length; i++)
		{
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPos[i]);
			Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(packedRot[i]);
			float num = 10000f;
			if (!(in vector).IsValid(in num) || !(in quaternion).IsValid() || !this.FactoryHasEntity(entityTypeId[i]) || !this.IsPositionInZone(vector))
			{
				return;
			}
			this.CreateItemLocal(netId[i], zoneId[i], entityTypeId[i], vector, quaternion, createData[i]);
		}
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x000A8080 File Offset: 0x000A6280
	public void RequestCreateItems(List<GameEntityCreateData> entityData)
	{
		int num = 5;
		if (!this.IsZoneAuthority(num) || !this.IsZoneActive())
		{
			return;
		}
		GameEntityManager.ClearByteBuffer(GameEntityManager.tempSerializeGameState);
		MemoryStream memoryStream = new MemoryStream(GameEntityManager.tempSerializeGameState);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(entityData.Count);
		for (int i = 0; i < entityData.Count; i++)
		{
			GameEntityCreateData gameEntityCreateData = entityData[i];
			int num2 = this.CreateNetId();
			long num3 = BitPackUtils.PackWorldPosForNetwork(gameEntityCreateData.localPosition);
			int num4 = BitPackUtils.PackQuaternionForNetwork(gameEntityCreateData.localRotation);
			binaryWriter.Write(num2);
			binaryWriter.Write(gameEntityCreateData.entityTypeId);
			binaryWriter.Write(num3);
			binaryWriter.Write(num4);
			binaryWriter.Write(gameEntityCreateData.createData);
		}
		long position = memoryStream.Position;
		byte[] array = GZipStream.CompressBuffer(GameEntityManager.tempSerializeGameState);
		this.photonView.RPC("CreateItemsRPC", RpcTarget.All, new object[] { num, array });
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x000A8174 File Offset: 0x000A6374
	[PunRPC]
	private void CreateItemsRPC(int zoneId, byte[] stateData, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || stateData == null || stateData.Length >= 15360 || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.CreateItems))
		{
			return;
		}
		try
		{
			byte[] array = GZipStream.UncompressBuffer(stateData);
			int num = array.Length;
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					int num2 = binaryReader.ReadInt32();
					for (int i = 0; i < num2; i++)
					{
						int num3 = binaryReader.ReadInt32();
						int num4 = binaryReader.ReadInt32();
						long num5 = binaryReader.ReadInt64();
						int num6 = binaryReader.ReadInt32();
						long num7 = binaryReader.ReadInt64();
						Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(num5);
						Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(num6);
						float num8 = 10000f;
						if ((in vector).IsValid(in num8) && (in quaternion).IsValid() && this.FactoryHasEntity(num4) && this.IsPositionInZone(vector))
						{
							this.CreateItemLocal(num3, zoneId, num4, vector, quaternion, num7);
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x000A8294 File Offset: 0x000A6494
	public bool FactoryHasEntity(int entityTypeId)
	{
		GameObject gameObject;
		return this.itemPrefabFactory.TryGetValue(entityTypeId, out gameObject);
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x000A82AF File Offset: 0x000A64AF
	public static bool PriceLookup(int entityTypeId, out int price)
	{
		if (GameEntityManager.instance.priceLookupByEntityId.TryGetValue(entityTypeId, out price))
		{
			return true;
		}
		price = -1;
		return false;
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x000A82CC File Offset: 0x000A64CC
	private void ValidateThatNetIdIsNotAlreadyUsed(int netId, int newTypeId)
	{
		for (int i = 0; i < this.netIds.Length; i++)
		{
			if (i < this.entities.Count && this.netIds[i] == netId)
			{
				if (this.entities[i] == null)
				{
					Debug.LogErrorFormat("Error creating entity type {0} Net Id {1} is being reused by null entity at index {2} next netid is {3}", new object[] { newTypeId, netId, i, this.nextNetId });
				}
				else
				{
					Debug.LogErrorFormat("Error creating entity type {0} Net Id {1} is being reused by {2} entity at index {3} {4}", new object[]
					{
						newTypeId,
						netId,
						this.entities[i].gameObject.name,
						i,
						this.nextNetId
					});
				}
			}
		}
	}

	// Token: 0x060021B0 RID: 8624 RVA: 0x000A83BC File Offset: 0x000A65BC
	public GameEntityId CreateItemLocal(int netId, int zoneId, int entityTypeId, Vector3 position, Quaternion rotation, long createData)
	{
		this.nextNetId = Mathf.Max(netId + 1, this.nextNetId);
		GameObject gameObject;
		if (!this.itemPrefabFactory.TryGetValue(entityTypeId, out gameObject))
		{
			return GameEntityId.Invalid;
		}
		if (!this.createdItemTypeCount.ContainsKey(entityTypeId))
		{
			this.createdItemTypeCount[entityTypeId] = 0;
		}
		if (this.createdItemTypeCount[entityTypeId] > 100)
		{
			return GameEntityId.Invalid;
		}
		Dictionary<int, int> dictionary = this.createdItemTypeCount;
		int num = dictionary[entityTypeId];
		dictionary[entityTypeId] = num + 1;
		GameEntity componentInChildren = global::UnityEngine.Object.Instantiate<GameObject>(gameObject, position, rotation).GetComponentInChildren<GameEntity>();
		GameEntityId gameEntityId = this.AddGameEntity(netId, zoneId, componentInChildren);
		componentInChildren.typeId = entityTypeId;
		componentInChildren.createData = createData;
		GREnemyChaser component = componentInChildren.GetComponent<GREnemyChaser>();
		if (component != null)
		{
			int num2 = (int)createData;
			component.Setup(num2);
		}
		GREnemyRanged component2 = componentInChildren.GetComponent<GREnemyRanged>();
		if (component2 != null)
		{
			int num3 = (int)createData;
			component2.Setup(num3);
		}
		GRBarrierSpectral component3 = componentInChildren.GetComponent<GRBarrierSpectral>();
		if (component3 != null)
		{
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(createData);
			component3.transform.localScale = vector;
		}
		GRBadge component4 = componentInChildren.GetComponent<GRBadge>();
		if (component4 != null)
		{
			GhostReactor.instance.employeeBadges.LinkBadgeToDispenser(component4, (long)((int)createData));
		}
		GRCollectible component5 = componentInChildren.GetComponent<GRCollectible>();
		if (component5 != null)
		{
			GameEntity gameEntity = GameEntityManager.instance.GetGameEntity(GameEntityManager.instance.GetEntityIdFromNetId((int)createData));
			if (gameEntity != null)
			{
				GRCollectibleDispenser component6 = gameEntity.GetComponent<GRCollectibleDispenser>();
				if (component6 != null)
				{
					component6.GetSpawnedCollectible(component5);
				}
			}
		}
		return gameEntityId;
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x000A8549 File Offset: 0x000A6749
	public void RequestDestroyItem(GameEntityId entityId)
	{
		if (!this.IsValidAuthorityRPC())
		{
			return;
		}
		if (!this.netIdsForDelete.Contains(GameEntity.GetNetId(entityId)))
		{
			this.netIdsForDelete.Add(GameEntity.GetNetId(entityId));
		}
		this.DestroyItemLocal(entityId);
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x000A8580 File Offset: 0x000A6780
	public void RequestDestroyItems(List<GameEntityId> entityIds)
	{
		if (!this.IsValidAuthorityRPC())
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < entityIds.Count; i++)
		{
			list.Add(entityIds[i].GetNetId());
		}
		this.photonView.RPC("DestroyItemRPC", RpcTarget.All, new object[] { list.ToArray() });
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000A85E4 File Offset: 0x000A67E4
	[PunRPC]
	public void DestroyItemRPC(int[] entityNetId, PhotonMessageInfo info)
	{
		if (this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.DestroyItem))
		{
			return;
		}
		for (int i = 0; i < entityNetId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, entityNetId[i]))
			{
				return;
			}
			this.DestroyItemLocal(GameEntity.GetIdFromNetId(entityNetId[i]));
		}
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x000A8630 File Offset: 0x000A6830
	public void DestroyItemLocal(GameEntityId entityId)
	{
		GameEntity gameEntity = GameEntityManager.instance.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		if (!this.createdItemTypeCount.ContainsKey(gameEntity.typeId))
		{
			this.createdItemTypeCount[gameEntity.typeId] = 1;
		}
		Dictionary<int, int> dictionary = this.createdItemTypeCount;
		int typeId = gameEntity.typeId;
		int num = dictionary[typeId];
		dictionary[typeId] = num - 1;
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber);
		if (gamePlayer != null)
		{
			gamePlayer.ClearGrabbedIfHeld(gameEntity.id);
		}
		if (gamePlayer != null && GamePlayerLocal.instance.gamePlayer == gamePlayer)
		{
			GamePlayerLocal.instance.ClearGrabbedIfHeld(gameEntity.id);
		}
		this.RemoveGameEntity(gameEntity);
		global::UnityEngine.Object.Destroy(gameEntity.gameObject);
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x000A86FA File Offset: 0x000A68FA
	public void RequestState(GameEntityId entityId, long newState)
	{
		this.photonView.RPC("RequestStateRPC", this.GetAuthorityPlayer(), new object[]
		{
			GameEntity.GetNetId(entityId),
			newState
		});
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x000A8730 File Offset: 0x000A6930
	[PunRPC]
	public void RequestStateRPC(int entityNetId, long newState, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(entityNetId))
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
		if (gamePlayer.IsNull() || !gamePlayer.netStateLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		if (gameEntity == null || gameEntity.IsNull())
		{
			return;
		}
		bool flag = false;
		GRToolClub component = gameEntity.GetComponent<GRToolClub>();
		GRToolCollector component2 = gameEntity.GetComponent<GRToolCollector>();
		GRToolRevive component3 = gameEntity.GetComponent<GRToolRevive>();
		GRToolLantern component4 = gameEntity.GetComponent<GRToolLantern>();
		GRToolFlash component5 = gameEntity.GetComponent<GRToolFlash>();
		if (component == null && component2 == null && component3 == null && component4 == null && component5 == null)
		{
			flag = this.IsAuthorityPlayer(info.Sender);
		}
		if (!flag && (gamePlayer.IsHoldingEntity(entityIdFromNetId, false) || gamePlayer.IsHoldingEntity(entityIdFromNetId, true)))
		{
			if (component4 != null)
			{
				flag = component4.CanChangeState(newState);
			}
			if (component5 != null)
			{
				flag = component5.CanChangeState(newState);
			}
			if (component != null || component2 != null || component3 != null)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.netIdsForState.Contains(entityNetId))
			{
				this.statesForState[this.netIdsForState.IndexOf(entityNetId)] = newState;
				return;
			}
			this.netIdsForState.Add(entityNetId);
			this.statesForState.Add(newState);
		}
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x000A8898 File Offset: 0x000A6A98
	[PunRPC]
	public void ApplyStateRPC(int[] netId, long[] newState, PhotonMessageInfo info)
	{
		if (netId.Length != newState.Length || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ApplyState))
		{
			return;
		}
		for (int i = 0; i < netId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netId[i]))
			{
				return;
			}
			GameEntityId idFromNetId = GameEntity.GetIdFromNetId(netId[i]);
			this.entities[idFromNetId.index].SetState(newState[i]);
		}
	}

	// Token: 0x060021B8 RID: 8632 RVA: 0x000A8900 File Offset: 0x000A6B00
	public void RequestGrabEntity(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation)
	{
		if (!this.IsAuthority())
		{
			this.GrabEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		long num = BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation);
		this.photonView.RPC("RequestGrabEntityRPC", this.GetAuthorityPlayer(), new object[]
		{
			GameEntity.GetNetId(gameEntityId),
			isLeftHand,
			num
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x060021B9 RID: 8633 RVA: 0x000A8974 File Offset: 0x000A6B74
	[PunRPC]
	public void RequestGrabEntityRPC(int entityNetId, bool isLeftHand, long packedPosRot, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(entityNetId))
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
		float num = 10000f;
		if (!(in vector).IsValid(in num) || !(in quaternion).IsValid() || vector.sqrMagnitude > 6400f)
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
		if (gamePlayer == null || !this.IsPlayerHandNearEntity(gamePlayer, entityNetId, isLeftHand, false, 16f) || this.IsValidEntity(gamePlayer.GetGameEntityId(isLeftHand)) || !gamePlayer.netGrabLimiter.CheckCallTime(Time.time) || gamePlayer.IsHoldingEntity(isLeftHand))
		{
			return;
		}
		GameEntity gameEntity = GameEntityManager.instance.GetGameEntity(GameEntity.GetIdFromNetId(entityNetId));
		if (gameEntity == null)
		{
			return;
		}
		bool flag = gameEntity.onlyGrabActorNumber != -1 && gameEntity.onlyGrabActorNumber != info.Sender.ActorNumber;
		bool flag2 = gameEntity.heldByActorNumber != -1 && gameEntity.heldByActorNumber != info.Sender.ActorNumber && GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber) != null;
		if (!gameEntity.pickupable || flag2 || flag)
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("GrabEntityRPC", RpcTarget.All, new object[] { entityNetId, isLeftHand, packedPosRot, info.Sender });
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	// Token: 0x060021BA RID: 8634 RVA: 0x000A8ADC File Offset: 0x000A6CDC
	[PunRPC]
	public void GrabEntityRPC(int entityNetId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.GrabEntity))
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
		float num = 10000f;
		if (!(in vector).IsValid(in num) || !(in quaternion).IsValid() || vector.sqrMagnitude > 6400f)
		{
			return;
		}
		this.GrabEntityLocal(GameEntity.GetIdFromNetId(entityNetId), isLeftHand, vector, quaternion, NetPlayer.Get(grabbedByPlayer));
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x000A8B54 File Offset: 0x000A6D54
	private void GrabEntityLocal(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(grabbedByPlayer.ActorNumber), out rigContainer))
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		if (gameEntity == null)
		{
			return;
		}
		if (grabbedByPlayer == null)
		{
			return;
		}
		int handIndex = GamePlayer.GetHandIndex(isLeftHand);
		if (grabbedByPlayer.IsLocal && gameEntity.heldByActorNumber == grabbedByPlayer.ActorNumber && gameEntity.heldByHandIndex == handIndex)
		{
			return;
		}
		GamePlayer gamePlayer = ((gameEntity.heldByActorNumber < 0) ? null : GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber));
		int num = ((gamePlayer == null) ? (-1) : gamePlayer.FindHandIndex(gameEntityId));
		bool flag = gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		if (gamePlayer != null)
		{
			gamePlayer.ClearGrabbedIfHeld(gameEntityId);
			if (num != -1 && flag)
			{
				GamePlayerLocal.instance.ClearGrabbed(num);
			}
		}
		Transform handTransform = GamePlayer.GetHandTransform(rigContainer.Rig, handIndex);
		gameEntity.transform.SetParent(handTransform);
		gameEntity.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = true;
		}
		GamePlayer gamePlayer2 = GamePlayer.GetGamePlayer(grabbedByPlayer.ActorNumber);
		gameEntity.heldByActorNumber = grabbedByPlayer.ActorNumber;
		gameEntity.heldByHandIndex = handIndex;
		gameEntity.lastHeldByActorNumber = gameEntity.heldByActorNumber;
		gamePlayer2.SetGrabbed(gameEntityId, handIndex);
		if (grabbedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GamePlayerLocal.instance.SetGrabbed(gameEntityId, GamePlayer.GetHandIndex(isLeftHand));
			GamePlayerLocal.instance.PlayCatchFx(isLeftHand);
		}
		gameEntity.PlayCatchFx();
		Action onGrabbed = gameEntity.OnGrabbed;
		if (onGrabbed == null)
		{
			return;
		}
		onGrabbed();
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x000A8D15 File Offset: 0x000A6F15
	public void GrabEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		this.GrabEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, grabbedByPlayer);
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x000A8D24 File Offset: 0x000A6F24
	public GameEntityId TryGrabLocal(Vector3 handPosition)
	{
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		GameEntityId gameEntityId = GameEntityId.Invalid;
		float num = float.MaxValue;
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (!(this.entities[i] == null) && this.entities[i].pickupable && (this.entities[i].onlyGrabActorNumber == -1 || this.entities[i].onlyGrabActorNumber == actorNumber) && (this.entities[i].heldByActorNumber == -1 || this.entities[i].heldByActorNumber == actorNumber || !(GamePlayer.GetGamePlayer(this.entities[i].heldByActorNumber) != null)))
			{
				float sqrMagnitude = this.entities[i].GetVelocity().sqrMagnitude;
				double num2 = 0.0625;
				if (sqrMagnitude > 2f)
				{
					num2 = 0.25;
				}
				float sqrMagnitude2 = (handPosition - this.entities[i].transform.position).sqrMagnitude;
				if ((double)sqrMagnitude2 < num2 && sqrMagnitude2 < num)
				{
					gameEntityId = this.entities[i].id;
					num = sqrMagnitude2;
				}
			}
		}
		return gameEntityId;
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x000A8E88 File Offset: 0x000A7088
	public void RequestThrowEntity(GameEntityId entityId, bool isLeftHand, Vector3 velocity, Vector3 angVelocity)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		Vector3 position = gameEntity.transform.position;
		Quaternion rotation = gameEntity.transform.rotation;
		if (!this.IsAuthority())
		{
			this.ThrowEntityLocal(entityId, isLeftHand, position, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		this.photonView.RPC("RequestThrowEntityRPC", this.GetAuthorityPlayer(), new object[]
		{
			GameEntity.GetNetId(entityId),
			isLeftHand,
			position,
			rotation,
			velocity,
			angVelocity
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x000A8F3C File Offset: 0x000A713C
	[PunRPC]
	public void RequestThrowEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, PhotonMessageInfo info)
	{
		if (this.IsValidAuthorityRPC(entityNetId))
		{
			float num = 10000f;
			if ((in position).IsValid(in num) && (in rotation).IsValid())
			{
				float num2 = 10000f;
				if ((in velocity).IsValid(in num2))
				{
					float num3 = 10000f;
					if ((in angVelocity).IsValid(in num3) && velocity.sqrMagnitude <= 1600f && this.IsPositionInZone(position))
					{
						GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
						if (gamePlayer == null || !GameEntityManager.IsPlayerHandNearPosition(gamePlayer, position, isLeftHand, false, 16f) || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId), isLeftHand) || !gamePlayer.netThrowLimiter.CheckCallTime(Time.time))
						{
							return;
						}
						this.photonView.RPC("ThrowEntityRPC", RpcTarget.All, new object[] { entityNetId, isLeftHand, position, rotation, velocity, angVelocity, info.Sender, info.SentServerTime });
						PhotonNetwork.SendAllOutgoingCommands();
						return;
					}
				}
			}
		}
	}

	// Token: 0x060021C0 RID: 8640 RVA: 0x000A9060 File Offset: 0x000A7260
	[PunRPC]
	public void ThrowEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player thrownByPlayer, double throwTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, position) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if ((in position).IsValid(in num) && (in rotation).IsValid())
			{
				float num2 = 10000f;
				if ((in velocity).IsValid(in num2))
				{
					float num3 = 10000f;
					if ((in angVelocity).IsValid(in num3) && velocity.sqrMagnitude <= 1600f)
					{
						this.ThrowEntityLocal(GameEntity.GetIdFromNetId(entityNetId), isLeftHand, position, rotation, velocity, angVelocity, NetPlayer.Get(thrownByPlayer));
						return;
					}
				}
			}
		}
	}

	// Token: 0x060021C1 RID: 8641 RVA: 0x000A90F4 File Offset: 0x000A72F4
	private void ThrowEntityLocal(GameEntityId gameBallId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer thrownByPlayer)
	{
		if (gameBallId.index < 0 || gameBallId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameBallId.index];
		if (gameEntity == null)
		{
			return;
		}
		if (thrownByPlayer == null)
		{
			return;
		}
		if (thrownByPlayer.IsLocal && gameEntity.heldByActorNumber != thrownByPlayer.ActorNumber && gameEntity.lastHeldByActorNumber == thrownByPlayer.ActorNumber)
		{
			return;
		}
		gameEntity.transform.SetParent(null);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = position;
			component.rotation = rotation;
			component.velocity = velocity;
			component.angularVelocity = angVelocity;
		}
		gameEntity.heldByActorNumber = -1;
		gameEntity.heldByHandIndex = -1;
		bool flag = thrownByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		int handIndex = GamePlayer.GetHandIndex(isLeftHand);
		RigContainer rigContainer;
		if (flag)
		{
			GamePlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
			GamePlayerLocal.instance.ClearGrabbed(handIndex);
			GamePlayerLocal.instance.PlayThrowFx(isLeftHand);
		}
		else if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(thrownByPlayer.ActorNumber), out rigContainer))
		{
			GamePlayer component2 = rigContainer.Rig.GetComponent<GamePlayer>();
			if (component2 != null)
			{
				component2.ClearGrabbedIfHeld(gameBallId);
			}
		}
		gameEntity.PlayThrowFx();
		Action onReleased = gameEntity.OnReleased;
		if (onReleased != null)
		{
			onReleased();
		}
		GRBadge component3 = gameEntity.GetComponent<GRBadge>();
		if (component3 != null)
		{
			GRPlayer grplayer = GRPlayer.Get(thrownByPlayer.ActorNumber);
			if (grplayer != null)
			{
				grplayer.AttachBadge(component3);
			}
		}
	}

	// Token: 0x060021C2 RID: 8642 RVA: 0x000A9298 File Offset: 0x000A7498
	public bool IsPlayerHandNearEntity(GamePlayer player, int entityNetId, bool isLeftHand, bool checkBothHands, float acceptableRadius = 16f)
	{
		GameEntityId idFromNetId = GameEntity.GetIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(idFromNetId);
		return !(gameEntity == null) && GameEntityManager.IsPlayerHandNearPosition(player, gameEntity.transform.position, isLeftHand, checkBothHands, acceptableRadius);
	}

	// Token: 0x060021C3 RID: 8643 RVA: 0x000A92D8 File Offset: 0x000A74D8
	public static bool IsPlayerHandNearPosition(GamePlayer player, Vector3 worldPosition, bool isLeftHand, bool checkBothHands, float acceptableRadius = 16f)
	{
		bool flag = true;
		if (player != null && player.rig != null)
		{
			if (isLeftHand || checkBothHands)
			{
				flag = (worldPosition - player.rig.leftHandTransform.position).sqrMagnitude < acceptableRadius * acceptableRadius;
			}
			if (!isLeftHand || checkBothHands)
			{
				float sqrMagnitude = (worldPosition - player.rig.rightHandTransform.position).sqrMagnitude;
				flag = flag && sqrMagnitude < acceptableRadius * acceptableRadius;
			}
		}
		return flag;
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x000A9364 File Offset: 0x000A7564
	public bool IsEntityNearEntity(int entityNetId, int otherEntityNetId, float acceptableRadius = 16f)
	{
		GameEntityId idFromNetId = GameEntity.GetIdFromNetId(otherEntityNetId);
		GameEntity gameEntity = this.GetGameEntity(idFromNetId);
		return !(gameEntity == null) && this.IsEntityNearPosition(entityNetId, gameEntity.transform.position, acceptableRadius);
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x000A93A0 File Offset: 0x000A75A0
	public bool IsEntityNearPosition(int entityNetId, Vector3 position, float acceptableRadius = 16f)
	{
		GameEntityId idFromNetId = GameEntity.GetIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(idFromNetId);
		return !(gameEntity == null) && Vector3.SqrMagnitude(gameEntity.transform.position - position) < acceptableRadius * acceptableRadius;
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x000A93E4 File Offset: 0x000A75E4
	private void ClearZone(GameEntityManager.ZoneStateData zoneStateData)
	{
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (this.entities[i] != null)
			{
				global::UnityEngine.Object.Destroy(this.entities[i].gameObject);
			}
		}
		this.entities.Clear();
		this.gameEntityData.Clear();
		this.createdItemTypeCount.Clear();
		GamePlayer component = VRRig.LocalRig.GetComponent<GamePlayer>();
		if (component != null)
		{
			component.Clear();
		}
		GameEntityManager.ZoneClearEvent zoneClearEvent = this.onZoneClear;
		if (zoneClearEvent == null)
		{
			return;
		}
		zoneClearEvent(zoneStateData.zone);
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x000A9484 File Offset: 0x000A7684
	public int SerializeGameState(int zoneId, byte[] bytes, int maxBytes)
	{
		MemoryStream memoryStream = new MemoryStream(bytes);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		GhostReactorShiftManager shiftManager = GhostReactorManager.instance.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = GhostReactorManager.instance.reactor.levelGenerator;
		binaryWriter.Write(shiftManager.ShiftActive);
		binaryWriter.Write(shiftManager.ShiftStartNetworkTime);
		binaryWriter.Write(shiftManager.EnemyDeaths);
		binaryWriter.Write(shiftManager.PlayerDeaths);
		binaryWriter.Write(shiftManager.CoresCollected);
		binaryWriter.Write(levelGenerator.levelGeneration.seed);
		binaryWriter.Write(levelGenerator.levelGeneration.sectionCount);
		GameEntityManager.tempEntitiesToSerialize.Clear();
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity = this.entities[i];
			if (!(gameEntity == null))
			{
				GameEntityManager.tempEntitiesToSerialize.Add(gameEntity);
			}
		}
		binaryWriter.Write(GameEntityManager.tempEntitiesToSerialize.Count);
		for (int j = 0; j < GameEntityManager.tempEntitiesToSerialize.Count; j++)
		{
			GameEntity gameEntity2 = GameEntityManager.tempEntitiesToSerialize[j];
			if (!(gameEntity2 == null))
			{
				int netIdFromEntityId = this.GetNetIdFromEntityId(gameEntity2.id);
				binaryWriter.Write(netIdFromEntityId);
				binaryWriter.Write(gameEntity2.typeId);
				binaryWriter.Write(gameEntity2.createData);
				binaryWriter.Write(gameEntity2.GetState());
				long num = BitPackUtils.PackWorldPosForNetwork(gameEntity2.transform.position);
				int num2 = BitPackUtils.PackQuaternionForNetwork(gameEntity2.transform.rotation);
				binaryWriter.Write(num);
				binaryWriter.Write(num2);
				int heldByActorNumber = gameEntity2.heldByActorNumber;
				binaryWriter.Write(heldByActorNumber);
				GameAgent component = gameEntity2.GetComponent<GameAgent>();
				bool flag = component != null;
				binaryWriter.Write(flag);
				if (flag)
				{
					long num3 = BitPackUtils.PackWorldPosForNetwork(component.navAgent.destination);
					binaryWriter.Write(num3);
				}
				GRTool component2 = gameEntity2.GetComponent<GRTool>();
				bool flag2 = component2 != null;
				binaryWriter.Write(flag2);
				if (flag2)
				{
					binaryWriter.Write(component2.energy);
				}
				GREnemyChaser component3 = gameEntity2.GetComponent<GREnemyChaser>();
				bool flag3 = component3 != null;
				binaryWriter.Write(flag3);
				if (flag3)
				{
					byte b = (byte)component3.currBehavior;
					byte b2 = (byte)component3.currBodyState;
					byte b3 = (byte)component3.hp;
					byte b4 = (byte)component3.nextPatrolNode;
					int num4 = ((component3.targetPlayer == null) ? (-1) : component3.targetPlayer.ActorNumber);
					binaryWriter.Write(b);
					binaryWriter.Write(b2);
					binaryWriter.Write(b3);
					binaryWriter.Write(b4);
					binaryWriter.Write(num4);
				}
				GREnemyRanged component4 = gameEntity2.GetComponent<GREnemyRanged>();
				bool flag4 = component4 != null;
				binaryWriter.Write(flag4);
				if (flag4)
				{
					byte b5 = (byte)component4.currBehavior;
					byte b6 = (byte)component4.currBodyState;
					byte b7 = (byte)component4.hp;
					byte b8 = (byte)component4.nextPatrolNode;
					int num5 = ((component4.targetPlayer == null) ? (-1) : component4.targetPlayer.ActorNumber);
					binaryWriter.Write(b5);
					binaryWriter.Write(b6);
					binaryWriter.Write(b7);
					binaryWriter.Write(b8);
					binaryWriter.Write(num5);
				}
			}
		}
		GameEntityManager.tempRigs.Clear();
		GameEntityManager.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GameEntityManager.tempRigs);
		int count = GameEntityManager.tempRigs.Count;
		binaryWriter.Write(count);
		for (int k = 0; k < GameEntityManager.tempRigs.Count; k++)
		{
			VRRig vrrig = GameEntityManager.tempRigs[k];
			NetPlayer owningNetPlayer = vrrig.OwningNetPlayer;
			binaryWriter.Write(owningNetPlayer.ActorNumber);
			GamePlayer component5 = vrrig.GetComponent<GamePlayer>();
			bool flag5 = component5 != null;
			binaryWriter.Write(flag5);
			if (flag5)
			{
				component5.SerializeNetworkState(binaryWriter, owningNetPlayer);
			}
			GRPlayer component6 = vrrig.GetComponent<GRPlayer>();
			bool flag6 = component6 != null;
			binaryWriter.Write(flag6);
			if (flag6)
			{
				component6.SerializeNetworkState(binaryWriter, owningNetPlayer);
			}
		}
		List<GRToolPurchaseStation> toolPurchasingStations = GhostReactorManager.instance.reactor.toolPurchasingStations;
		binaryWriter.Write(toolPurchasingStations.Count);
		for (int l = 0; l < toolPurchasingStations.Count; l++)
		{
			binaryWriter.Write(toolPurchasingStations[l].ActiveEntryIndex);
		}
		return (int)memoryStream.Position;
	}

	// Token: 0x060021C8 RID: 8648 RVA: 0x000A98C4 File Offset: 0x000A7AC4
	public void DeserializeTableState(int zoneId, byte[] bytes, int numBytes)
	{
		if (numBytes <= 0)
		{
			return;
		}
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				GhostReactorShiftManager shiftManager = GhostReactorManager.instance.reactor.shiftManager;
				GhostReactorLevelGenerator levelGenerator = GhostReactorManager.instance.reactor.levelGenerator;
				bool flag = binaryReader.ReadBoolean();
				double num = binaryReader.ReadDouble();
				shiftManager.EnemyDeaths = binaryReader.ReadInt32();
				shiftManager.PlayerDeaths = binaryReader.ReadInt32();
				shiftManager.CoresCollected = binaryReader.ReadInt32();
				shiftManager.RefreshShiftStatsDisplay();
				int num2 = binaryReader.ReadInt32();
				int num3 = binaryReader.ReadInt32();
				if (flag)
				{
					levelGenerator.GenerateRandomLevelConfiguration(num3, num2);
					levelGenerator.SpawnSectionsBasedOnLevelGenerationConfig();
					shiftManager.OnShiftStarted(num);
				}
				int num4 = binaryReader.ReadInt32();
				for (int i = 0; i < num4; i++)
				{
					int num5 = binaryReader.ReadInt32();
					int num6 = binaryReader.ReadInt32();
					long num7 = binaryReader.ReadInt64();
					long num8 = binaryReader.ReadInt64();
					long num9 = binaryReader.ReadInt64();
					int num10 = binaryReader.ReadInt32();
					Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(num9);
					Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(num10);
					GameEntityId gameEntityId = GameEntityManager.instance.CreateItemLocal(num5, zoneId, num6, vector, quaternion, num7);
					this.GetNetIdFromEntityId(gameEntityId);
					GameEntity gameEntity = GameEntityManager.instance.GetGameEntity(gameEntityId);
					if (!(gameEntity == null))
					{
						gameEntity.SetState(num8);
						binaryReader.ReadInt32();
						if (binaryReader.ReadBoolean())
						{
							Vector3 vector2 = BitPackUtils.UnpackWorldPosFromNetwork(binaryReader.ReadInt64());
							GameAgent component = gameEntity.GetComponent<GameAgent>();
							if (component != null && component.IsOnNavMesh())
							{
								component.navAgent.destination = vector2;
							}
						}
						if (binaryReader.ReadBoolean())
						{
							int num11 = binaryReader.ReadInt32();
							GRTool component2 = gameEntity.GetComponent<GRTool>();
							if (component2 != null)
							{
								component2.SetEnergy(num11);
							}
						}
						GRBadge component3 = gameEntity.GetComponent<GRBadge>();
						if (component3 != null)
						{
							GhostReactor.instance.employeeBadges.LinkBadgeToDispenser(component3, (long)((int)num7));
						}
						if (binaryReader.ReadBoolean())
						{
							GREnemyChaser.Behavior behavior = (GREnemyChaser.Behavior)binaryReader.ReadByte();
							GREnemyChaser.BodyState bodyState = (GREnemyChaser.BodyState)binaryReader.ReadByte();
							int num12 = (int)binaryReader.ReadByte();
							byte b = binaryReader.ReadByte();
							int num13 = binaryReader.ReadInt32();
							GREnemyChaser component4 = gameEntity.GetComponent<GREnemyChaser>();
							if (component4 != null)
							{
								component4.SetPatrolPath((int)gameEntity.createData);
								component4.SetNextPatrolNode((int)b);
								component4.SetHP(num12);
								component4.SetBehavior(behavior, true);
								component4.SetBodyState(bodyState, true);
								component4.targetPlayer = NetworkSystem.Instance.GetPlayer(num13);
							}
						}
						bool flag2 = binaryReader.ReadBoolean();
						if (flag2)
						{
							GREnemyRanged.Behavior behavior2 = (GREnemyRanged.Behavior)binaryReader.ReadByte();
							GREnemyRanged.BodyState bodyState2 = (GREnemyRanged.BodyState)binaryReader.ReadByte();
							int num14 = (int)binaryReader.ReadByte();
							byte b2 = binaryReader.ReadByte();
							int num15 = binaryReader.ReadInt32();
							GREnemyRanged component5 = gameEntity.GetComponent<GREnemyRanged>();
							if (flag2)
							{
								component5.SetPatrolPath((int)gameEntity.createData);
								component5.SetNextPatrolNode((int)b2);
								component5.SetHP(num14);
								component5.SetBehavior(behavior2, true);
								component5.SetBodyState(bodyState2, true);
								component5.targetPlayer = NetworkSystem.Instance.GetPlayer(num15);
							}
						}
					}
				}
				int num16 = binaryReader.ReadInt32();
				for (int j = 0; j < num16; j++)
				{
					int num17 = binaryReader.ReadInt32();
					if (binaryReader.ReadBoolean())
					{
						GamePlayer gamePlayer = GamePlayer.GetGamePlayer(num17);
						GamePlayer.DeserializeNetworkState(binaryReader, gamePlayer);
					}
					if (binaryReader.ReadBoolean())
					{
						GRPlayer grplayer = GRPlayer.Get(num17);
						GRPlayer.DeserializeNetworkStateAndBurn(binaryReader, grplayer);
					}
				}
				List<GRToolPurchaseStation> toolPurchasingStations = GhostReactorManager.instance.reactor.toolPurchasingStations;
				int num18 = binaryReader.ReadInt32();
				for (int k = 0; k < num18; k++)
				{
					int num19 = binaryReader.ReadInt32();
					if (k < toolPurchasingStations.Count && toolPurchasingStations[k] != null)
					{
						toolPurchasingStations[k].OnSelectionUpdate(num19);
					}
				}
				GhostReactor.instance.VRRigRefresh();
			}
		}
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x000A9CC8 File Offset: 0x000A7EC8
	private void UpdateZoneState()
	{
		GameEntityManager.tempRigs.Clear();
		GameEntityManager.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GameEntityManager.tempRigs);
		this.UpdateAuthority(this.zoneStateData, GameEntityManager.tempRigs);
		if (this.IsAuthority())
		{
			this.UpdateClientsFromAuthority(this.zoneStateData, GameEntityManager.tempRigs);
			this.UpdateZoneStateAuthority(this.zoneStateData);
		}
		else
		{
			this.UpdateZoneStateClient(this.zoneStateData);
		}
		for (int i = this.zoneStateData.zonePlayers.Count - 1; i >= 0; i--)
		{
			if (this.zoneStateData.zonePlayers[i] == null)
			{
				this.zoneStateData.zonePlayers.RemoveAt(i);
			}
		}
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x000A9D84 File Offset: 0x000A7F84
	private void UpdateAuthority(GameEntityManager.ZoneStateData zoneStateData, List<VRRig> allRigs)
	{
		if (!PhotonNetwork.InRoom && base.IsMine)
		{
			if (!this.IsAuthority())
			{
				this.guard.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
				return;
			}
		}
		else if (this.IsAuthority() && !this.IsInZone(zoneStateData.zone))
		{
			Player player = null;
			for (int i = 0; i < allRigs.Count; i++)
			{
				VRRig vrrig = allRigs[i];
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(vrrig);
				if (!(gamePlayer == null) && !(gamePlayer.rig == null) && gamePlayer.rig.OwningNetPlayer != null && !gamePlayer.rig.isLocal && vrrig.zoneEntity.currentZone == zoneStateData.zone)
				{
					player = gamePlayer.rig.OwningNetPlayer.GetPlayerRef();
				}
			}
			if (player != null && player != null)
			{
				this.guard.TransferOwnership(player, "");
			}
		}
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x000A9E74 File Offset: 0x000A8074
	private void UpdateClientsFromAuthority(GameEntityManager.ZoneStateData zoneStateData, List<VRRig> allRigs)
	{
		if (!this.IsInZone(zoneStateData.zone))
		{
			return;
		}
		for (int i = 0; i < zoneStateData.zoneStateRequests.Count; i++)
		{
			GameEntityManager.ZoneStateRequest zoneStateRequest = zoneStateData.zoneStateRequests[i];
			if (zoneStateRequest.player != null && zoneStateRequest.zone == zoneStateData.zone)
			{
				this.SendZoneStateToPlayerOrTarget(zoneStateRequest.zone, zoneStateRequest.player, RpcTarget.MasterClient);
				zoneStateRequest.completed = true;
				zoneStateData.zoneStateRequests[i] = zoneStateRequest;
				zoneStateData.zoneStateRequests.RemoveAt(i);
				return;
			}
			zoneStateData.zoneStateRequests.RemoveAt(i);
			i--;
		}
	}

	// Token: 0x060021CC RID: 8652 RVA: 0x000A9F10 File Offset: 0x000A8110
	public void TestSerializeTableState()
	{
		GameEntityManager.ClearByteBuffer(GameEntityManager.tempSerializeGameState);
		int num = this.SerializeGameState((int)this.zoneStateData.zone, GameEntityManager.tempSerializeGameState, 15360);
		byte[] array = GZipStream.CompressBuffer(GameEntityManager.tempSerializeGameState);
		Debug.LogFormat("Test Serialize Game State Buffer Size Uncompressed {0}", new object[] { num });
		Debug.LogFormat("Test Serialize Game State Buffer Size Compressed {0}", new object[] { array.Length });
	}

	// Token: 0x060021CD RID: 8653 RVA: 0x000A9F84 File Offset: 0x000A8184
	public static void ClearByteBuffer(byte[] buffer)
	{
		int num = buffer.Length;
		for (int i = 0; i < num; i++)
		{
			buffer[i] = 0;
		}
	}

	// Token: 0x060021CE RID: 8654 RVA: 0x000A9FA8 File Offset: 0x000A81A8
	private void SendZoneStateToPlayerOrTarget(GTZone zone, Player player, RpcTarget target)
	{
		GameEntityManager.ClearByteBuffer(GameEntityManager.tempSerializeGameState);
		this.SerializeGameState((int)zone, GameEntityManager.tempSerializeGameState, 15360);
		byte[] array = GZipStream.CompressBuffer(GameEntityManager.tempSerializeGameState);
		byte[] array2 = new byte[512];
		int i = 0;
		int num = 0;
		int num2 = array.Length;
		while (i < num2)
		{
			int num3 = Mathf.Min(512, num2 - i);
			Array.Copy(array, i, array2, 0, num3);
			if (player != null)
			{
				this.photonView.RPC("SendTableDataRPC", player, new object[] { num, num2, array2 });
			}
			else
			{
				this.photonView.RPC("SendTableDataRPC", target, new object[] { num, num2, array2 });
			}
			i += num3;
			num++;
		}
	}

	// Token: 0x060021CF RID: 8655 RVA: 0x000AA084 File Offset: 0x000A8284
	[PunRPC]
	public void SendTableDataRPC(int packetNum, int totalBytes, byte[] bytes, PhotonMessageInfo info)
	{
		if (!this.IsAuthorityPlayer(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.SendTableData) || bytes == null || bytes.Length >= 15360)
		{
			return;
		}
		if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingForState)
		{
			return;
		}
		if (packetNum == 0)
		{
			this.zoneStateData.numRecievedStateBytes = 0;
			for (int i = 0; i < this.zoneStateData.recievedStateBytes.Length; i++)
			{
				this.zoneStateData.recievedStateBytes[i] = 0;
			}
		}
		Array.Copy(bytes, 0, this.zoneStateData.recievedStateBytes, this.zoneStateData.numRecievedStateBytes, bytes.Length);
		this.zoneStateData.numRecievedStateBytes += bytes.Length;
		if (this.zoneStateData.numRecievedStateBytes >= totalBytes)
		{
			this.ClearZone(this.zoneStateData);
			try
			{
				byte[] array = GZipStream.UncompressBuffer(this.zoneStateData.recievedStateBytes);
				int num = array.Length;
				this.DeserializeTableState((int)this.zoneStateData.zone, array, num);
				this.SetZoneState(this.zoneStateData, GameEntityManager.ZoneState.Active);
				GameEntityManager.ZoneStartEvent zoneStartEvent = this.onZoneStart;
				if (zoneStartEvent != null)
				{
					zoneStartEvent(this.zoneStateData.zone);
				}
			}
			catch (Exception)
			{
			}
		}
	}

	// Token: 0x060021D0 RID: 8656 RVA: 0x000AA1B8 File Offset: 0x000A83B8
	private void UpdateZoneStateAuthority(GameEntityManager.ZoneStateData zoneStateData)
	{
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(VRRig.LocalRig);
		if (gamePlayer == null || gamePlayer.rig == null || gamePlayer.rig.OwningNetPlayer == null)
		{
			return;
		}
		if (!this.IsInZone(zoneStateData.zone) && zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.WaitingToEnterZone);
			return;
		}
		GameEntityManager.ZoneState state = zoneStateData.state;
		if (state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			if (state != GameEntityManager.ZoneState.WaitingToRequestState)
			{
				return;
			}
			if (Time.timeAsDouble - zoneStateData.stateStartTime > 1.0)
			{
				this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.WaitingForState);
				this.photonView.RPC("RequestZoneStateRPC", this.GetAuthorityPlayer(), new object[] { (int)zoneStateData.zone });
			}
		}
		else if (this.IsInZone(zoneStateData.zone))
		{
			GameEntityManager.ZoneStartEvent zoneStartEvent = this.onZoneStart;
			if (zoneStartEvent != null)
			{
				zoneStartEvent(zoneStateData.zone);
			}
			this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.Active);
			return;
		}
	}

	// Token: 0x060021D1 RID: 8657 RVA: 0x000AA29C File Offset: 0x000A849C
	private void UpdateZoneStateClient(GameEntityManager.ZoneStateData zoneStateData)
	{
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(VRRig.LocalRig);
		if (gamePlayer == null || gamePlayer.rig == null || gamePlayer.rig.OwningNetPlayer == null)
		{
			return;
		}
		if (!this.IsInZone(zoneStateData.zone) && zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.WaitingToEnterZone);
			return;
		}
		GameEntityManager.ZoneState state = zoneStateData.state;
		if (state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			if (state != GameEntityManager.ZoneState.WaitingToRequestState)
			{
				return;
			}
			if (Time.timeAsDouble - zoneStateData.stateStartTime > 1.0)
			{
				this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.WaitingForState);
				this.photonView.RPC("RequestZoneStateRPC", this.GetAuthorityPlayer(), new object[] { (int)zoneStateData.zone });
			}
		}
		else if (this.HasAuthority() && this.IsInZone(zoneStateData.zone) && !this.IsAuthority())
		{
			this.SetZoneState(zoneStateData, GameEntityManager.ZoneState.WaitingToRequestState);
			return;
		}
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x000AA377 File Offset: 0x000A8577
	private bool IsInZone(GTZone zone)
	{
		return ZoneManagement.instance.IsZoneActive(zone) && GhostReactor.instance != null;
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x000AA394 File Offset: 0x000A8594
	private void SetZoneState(GameEntityManager.ZoneStateData zoneStateData, GameEntityManager.ZoneState newState)
	{
		if (newState == zoneStateData.state)
		{
			return;
		}
		zoneStateData.state = newState;
		zoneStateData.stateStartTime = Time.timeAsDouble;
		GameEntityManager.ZoneState state = zoneStateData.state;
		if (state == GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			this.ClearZone(zoneStateData);
			return;
		}
		if (state != GameEntityManager.ZoneState.WaitingForState)
		{
			return;
		}
		zoneStateData.numRecievedStateBytes = 0;
		for (int i = 0; i < zoneStateData.recievedStateBytes.Length; i++)
		{
			zoneStateData.recievedStateBytes[i] = 0;
		}
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x000AA3F7 File Offset: 0x000A85F7
	public void DebugSendState()
	{
		this.SetZoneState(this.zoneStateData, GameEntityManager.ZoneState.WaitingToRequestState);
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x000AA408 File Offset: 0x000A8608
	[PunRPC]
	private void RequestZoneStateRPC(int zoneId, PhotonMessageInfo info)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (zoneId != (int)this.zoneStateData.zone || this.zoneStateData.zoneStateRequests == null)
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
		if (gamePlayer == null)
		{
			return;
		}
		if (!gamePlayer.newJoinZoneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		for (int i = 0; i < this.zoneStateData.zoneStateRequests.Count; i++)
		{
			if (this.zoneStateData.zoneStateRequests[i].player == info.Sender)
			{
				return;
			}
		}
		this.zoneStateData.zoneStateRequests.Add(new GameEntityManager.ZoneStateRequest
		{
			player = info.Sender,
			zone = this.zoneStateData.zone,
			completed = false
		});
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x000023F4 File Offset: 0x000005F4
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060021D8 RID: 8664 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x000023F4 File Offset: 0x000005F4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x060021DA RID: 8666 RVA: 0x000AA4DD File Offset: 0x000A86DD
	void IMatchmakingCallbacks.OnJoinedRoom()
	{
		this.SetZoneState(this.zoneStateData, GameEntityManager.ZoneState.WaitingToEnterZone);
	}

	// Token: 0x060021DB RID: 8667 RVA: 0x000AA4DD File Offset: 0x000A86DD
	void IMatchmakingCallbacks.OnLeftRoom()
	{
		this.SetZoneState(this.zoneStateData, GameEntityManager.ZoneState.WaitingToEnterZone);
	}

	// Token: 0x060021DC RID: 8668 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x060021DD RID: 8669 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x060021DE RID: 8670 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	// Token: 0x060021DF RID: 8671 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	// Token: 0x060021E0 RID: 8672 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	// Token: 0x060021E1 RID: 8673 RVA: 0x000023F4 File Offset: 0x000005F4
	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	// Token: 0x060021E2 RID: 8674 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	// Token: 0x060021E3 RID: 8675 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x060021E4 RID: 8676 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerLeftRoom(Player newPlayer)
	{
	}

	// Token: 0x060021E5 RID: 8677 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x000023F4 File Offset: 0x000005F4
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x060021E7 RID: 8679 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
	}

	// Token: 0x060021E8 RID: 8680 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x060021E9 RID: 8681 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x060021EA RID: 8682 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x060021EB RID: 8683 RVA: 0x000023F4 File Offset: 0x000005F4
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x060021EE RID: 8686 RVA: 0x00002637 File Offset: 0x00000837
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060021EF RID: 8687 RVA: 0x00002643 File Offset: 0x00000843
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040025BF RID: 9663
	private const int MAX_STATE_BYTES = 15360;

	// Token: 0x040025C0 RID: 9664
	private const int MAX_CHUNK_BYTES = 512;

	// Token: 0x040025C1 RID: 9665
	public const float MAX_LOCAL_MAGNITUDE_SQ = 6400f;

	// Token: 0x040025C2 RID: 9666
	public const float MAX_DISTANCE_FROM_HAND = 16f;

	// Token: 0x040025C3 RID: 9667
	public const float MAX_ENTITY_DIST = 16f;

	// Token: 0x040025C4 RID: 9668
	public const float MAX_THROW_SPEED_SQ = 1600f;

	// Token: 0x040025C5 RID: 9669
	public const int MAX_ENTITY_COUNT_PER_TYPE = 100;

	// Token: 0x040025C6 RID: 9670
	public const int INVALID_ID = -1;

	// Token: 0x040025C7 RID: 9671
	public const int INVALID_INDEX = -1;

	// Token: 0x040025C8 RID: 9672
	[OnEnterPlay_SetNull]
	public static volatile GameEntityManager instance;

	// Token: 0x040025C9 RID: 9673
	public PhotonView photonView;

	// Token: 0x040025CA RID: 9674
	public RequestableOwnershipGuard guard;

	// Token: 0x040025CB RID: 9675
	public Player prevAuthorityPlayer;

	// Token: 0x040025CC RID: 9676
	public BoxCollider zoneLimit;

	// Token: 0x040025CD RID: 9677
	private List<GameEntity> entities;

	// Token: 0x040025CE RID: 9678
	private List<GameEntityData> gameEntityData;

	// Token: 0x040025CF RID: 9679
	public List<GameEntity> tempFactoryItems;

	// Token: 0x040025D2 RID: 9682
	private Dictionary<int, GameObject> itemPrefabFactory;

	// Token: 0x040025D3 RID: 9683
	private Dictionary<int, int> priceLookupByEntityId;

	// Token: 0x040025D4 RID: 9684
	private List<GameEntity> tempEntities = new List<GameEntity>();

	// Token: 0x040025D5 RID: 9685
	private List<int> netIdsForCreate;

	// Token: 0x040025D6 RID: 9686
	private List<int> zoneIdsForCreate;

	// Token: 0x040025D7 RID: 9687
	private List<int> entityTypeIdsForCreate;

	// Token: 0x040025D8 RID: 9688
	private List<int> packedRotationsForCreate;

	// Token: 0x040025D9 RID: 9689
	private List<long> packedPositionsForCreate;

	// Token: 0x040025DA RID: 9690
	private List<long> createDataForCreate;

	// Token: 0x040025DB RID: 9691
	private float createCooldown = 0.24f;

	// Token: 0x040025DC RID: 9692
	private float lastCreateSent;

	// Token: 0x040025DD RID: 9693
	private List<int> netIdsForDelete;

	// Token: 0x040025DE RID: 9694
	private float destroyCooldown = 0.25f;

	// Token: 0x040025DF RID: 9695
	private float lastDestroySent;

	// Token: 0x040025E0 RID: 9696
	private List<int> netIdsForState;

	// Token: 0x040025E1 RID: 9697
	private List<long> statesForState;

	// Token: 0x040025E2 RID: 9698
	private float lastStateSent;

	// Token: 0x040025E3 RID: 9699
	private float stateCooldown;

	// Token: 0x040025E4 RID: 9700
	private Dictionary<int, int> netIdToIndex;

	// Token: 0x040025E5 RID: 9701
	private NativeArray<int> netIds;

	// Token: 0x040025E6 RID: 9702
	private NativeArray<int> zoneIds;

	// Token: 0x040025E7 RID: 9703
	private Dictionary<int, int> createdItemTypeCount;

	// Token: 0x040025E8 RID: 9704
	private GameEntityManager.ZoneStateData zoneStateData;

	// Token: 0x040025E9 RID: 9705
	private int nextNetId = 1;

	// Token: 0x040025EA RID: 9706
	public CallLimitersList<CallLimiter, GameEntityManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GameEntityManager.RPC>();

	// Token: 0x040025EB RID: 9707
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x040025EC RID: 9708
	private static List<GameEntity> tempEntitiesToSerialize = new List<GameEntity>(512);

	// Token: 0x040025ED RID: 9709
	private static byte[] tempSerializeGameState = new byte[15360];

	// Token: 0x02000567 RID: 1383
	// (Invoke) Token: 0x060021F1 RID: 8689
	public delegate void ZoneStartEvent(GTZone zoneId);

	// Token: 0x02000568 RID: 1384
	// (Invoke) Token: 0x060021F5 RID: 8693
	public delegate void ZoneClearEvent(GTZone zoneId);

	// Token: 0x02000569 RID: 1385
	private enum ZoneState
	{
		// Token: 0x040025EF RID: 9711
		WaitingToEnterZone,
		// Token: 0x040025F0 RID: 9712
		WaitingToRequestState,
		// Token: 0x040025F1 RID: 9713
		WaitingForState,
		// Token: 0x040025F2 RID: 9714
		Active
	}

	// Token: 0x0200056A RID: 1386
	private struct ZoneStateRequest
	{
		// Token: 0x040025F3 RID: 9715
		public Player player;

		// Token: 0x040025F4 RID: 9716
		public GTZone zone;

		// Token: 0x040025F5 RID: 9717
		public bool completed;
	}

	// Token: 0x0200056B RID: 1387
	private class ZoneStateData
	{
		// Token: 0x040025F6 RID: 9718
		public GameEntityManager.ZoneState state;

		// Token: 0x040025F7 RID: 9719
		public double stateStartTime;

		// Token: 0x040025F8 RID: 9720
		public GTZone zone;

		// Token: 0x040025F9 RID: 9721
		public List<GameEntityManager.ZoneStateRequest> zoneStateRequests;

		// Token: 0x040025FA RID: 9722
		public List<Player> zonePlayers;

		// Token: 0x040025FB RID: 9723
		public byte[] recievedStateBytes;

		// Token: 0x040025FC RID: 9724
		public int numRecievedStateBytes;
	}

	// Token: 0x0200056C RID: 1388
	public enum RPC
	{
		// Token: 0x040025FE RID: 9726
		CreateItem,
		// Token: 0x040025FF RID: 9727
		CreateItems,
		// Token: 0x04002600 RID: 9728
		DestroyItem,
		// Token: 0x04002601 RID: 9729
		ApplyState,
		// Token: 0x04002602 RID: 9730
		GrabEntity,
		// Token: 0x04002603 RID: 9731
		ThrowEntity,
		// Token: 0x04002604 RID: 9732
		SendTableData
	}
}
