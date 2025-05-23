using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000533 RID: 1331
public class CosmeticCritterManager : NetworkSceneObject
{
	// Token: 0x17000344 RID: 836
	// (get) Token: 0x0600204D RID: 8269 RVA: 0x000A264F File Offset: 0x000A084F
	// (set) Token: 0x0600204E RID: 8270 RVA: 0x000A2656 File Offset: 0x000A0856
	public static CosmeticCritterManager Instance { get; private set; }

	// Token: 0x0600204F RID: 8271 RVA: 0x000A265E File Offset: 0x000A085E
	public void RegisterLocalHoldable(CosmeticCritterHoldable holdable)
	{
		this.localHoldables.Add(holdable);
	}

	// Token: 0x06002050 RID: 8272 RVA: 0x000A266C File Offset: 0x000A086C
	public void RegisterIndependentSpawner(CosmeticCritterSpawnerIndependent spawner)
	{
		if (spawner.IsLocal)
		{
			this.localCritterSpawners.AddIfNew(spawner);
			return;
		}
		this.remoteCritterSpawners.AddIfNew(spawner);
	}

	// Token: 0x06002051 RID: 8273 RVA: 0x000A268F File Offset: 0x000A088F
	public void UnregisterIndependentSpawner(CosmeticCritterSpawnerIndependent spawner)
	{
		if (spawner.IsLocal)
		{
			this.localCritterSpawners.Remove(spawner);
			return;
		}
		this.remoteCritterSpawners.Remove(spawner);
	}

	// Token: 0x06002052 RID: 8274 RVA: 0x000A26B4 File Offset: 0x000A08B4
	public void RegisterCatcher(CosmeticCritterCatcher catcher)
	{
		if (catcher.IsLocal)
		{
			this.localCritterCatchers.AddIfNew(catcher);
			return;
		}
		this.remoteCritterCatchers.AddIfNew(catcher);
	}

	// Token: 0x06002053 RID: 8275 RVA: 0x000A26D7 File Offset: 0x000A08D7
	public void UnregisterCatcher(CosmeticCritterCatcher catcher)
	{
		if (catcher.IsLocal)
		{
			this.localCritterCatchers.Remove(catcher);
			return;
		}
		this.remoteCritterCatchers.Remove(catcher);
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x000A26FC File Offset: 0x000A08FC
	public void RegisterTickForEachCritter(Type type, ICosmeticCritterTickForEach target)
	{
		List<ICosmeticCritterTickForEach> list;
		if (!this.tickForEachCritterOfType.TryGetValue(type, out list) || list == null)
		{
			list = new List<ICosmeticCritterTickForEach>();
			this.tickForEachCritterOfType.Add(type, list);
		}
		list.AddIfNew(target);
	}

	// Token: 0x06002055 RID: 8277 RVA: 0x000A2738 File Offset: 0x000A0938
	public void UnregisterTickForEachCritter(Type type, ICosmeticCritterTickForEach target)
	{
		List<ICosmeticCritterTickForEach> list;
		if (this.tickForEachCritterOfType.TryGetValue(type, out list) && list != null)
		{
			list.Remove(target);
		}
	}

	// Token: 0x06002056 RID: 8278 RVA: 0x000A2760 File Offset: 0x000A0960
	private void ResetLocalCallLimiters()
	{
		int i = 0;
		while (i < this.localHoldables.Count)
		{
			if (this.localHoldables[i] == null)
			{
				this.localHoldables.RemoveAt(i);
			}
			else
			{
				this.localHoldables[i].ResetCallLimiter();
				i++;
			}
		}
	}

	// Token: 0x06002057 RID: 8279 RVA: 0x000A27B8 File Offset: 0x000A09B8
	private void ResetCosmeticCritters(NetPlayer player)
	{
		if (NetworkSystem.Instance.LocalPlayer != player)
		{
			return;
		}
		this.ResetLocalCallLimiters();
		for (int i = 0; i < this.activeCritters.Count; i++)
		{
			this.FreeCritter(this.activeCritters[i]);
		}
	}

	// Token: 0x06002058 RID: 8280 RVA: 0x000A2804 File Offset: 0x000A0A04
	private void Awake()
	{
		if (CosmeticCritterManager.Instance != null && CosmeticCritterManager.Instance != this)
		{
			global::UnityEngine.Object.Destroy(this);
			return;
		}
		CosmeticCritterManager.Instance = this;
		this.localHoldables = new List<CosmeticCritterHoldable>();
		this.localCritterSpawners = new List<CosmeticCritterSpawnerIndependent>();
		this.remoteCritterSpawners = new List<CosmeticCritterSpawnerIndependent>();
		this.localCritterCatchers = new List<CosmeticCritterCatcher>();
		this.remoteCritterCatchers = new List<CosmeticCritterCatcher>();
		this.activeCritters = new List<CosmeticCritter>();
		this.activeCrittersPerType = new Dictionary<Type, int>();
		this.activeCrittersBySeed = new Dictionary<int, CosmeticCritter>();
		this.inactiveCrittersByType = new Dictionary<Type, Stack<CosmeticCritter>>();
		this.tickForEachCritterOfType = new Dictionary<Type, List<ICosmeticCritterTickForEach>>();
		NetworkSystem.Instance.OnPlayerJoined += this.ResetCosmeticCritters;
		NetworkSystem.Instance.OnPlayerLeft += this.ResetCosmeticCritters;
	}

	// Token: 0x06002059 RID: 8281 RVA: 0x000A28D4 File Offset: 0x000A0AD4
	private void ReuseOrSpawnNewCritter(CosmeticCritterSpawner spawner, int seed, double time)
	{
		Type critterType = spawner.GetCritterType();
		Stack<CosmeticCritter> stack;
		CosmeticCritter cosmeticCritter;
		if (!this.inactiveCrittersByType.TryGetValue(critterType, out stack))
		{
			stack = new Stack<CosmeticCritter>();
			this.inactiveCrittersByType.Add(critterType, stack);
			cosmeticCritter = global::UnityEngine.Object.Instantiate<GameObject>(spawner.GetCritterPrefab(), base.transform).GetComponent<CosmeticCritter>();
		}
		else if (stack.TryPop(out cosmeticCritter))
		{
			cosmeticCritter.gameObject.SetActive(true);
		}
		else
		{
			cosmeticCritter = global::UnityEngine.Object.Instantiate<GameObject>(spawner.GetCritterPrefab(), base.transform).GetComponent<CosmeticCritter>();
		}
		cosmeticCritter.SetSeedSpawnerTypeAndTime(seed, spawner, critterType, time);
		this.activeCritters.Add(cosmeticCritter);
		if (!this.activeCrittersPerType.ContainsKey(critterType))
		{
			this.activeCrittersPerType.Add(critterType, 1);
		}
		else
		{
			Dictionary<Type, int> dictionary = this.activeCrittersPerType;
			Type type = critterType;
			dictionary[type]++;
		}
		this.activeCrittersBySeed.Add(seed, cosmeticCritter);
		Random.State state = Random.state;
		Random.InitState(seed);
		spawner.SetRandomVariables(cosmeticCritter);
		cosmeticCritter.SetRandomVariables();
		Random.state = state;
		spawner.OnSpawn(cosmeticCritter);
		cosmeticCritter.OnSpawn();
	}

	// Token: 0x0600205A RID: 8282 RVA: 0x000A29DC File Offset: 0x000A0BDC
	private void FreeCritter(CosmeticCritter critter)
	{
		critter.OnDespawn();
		if (critter.Spawner != null)
		{
			critter.Spawner.OnDespawn(critter);
		}
		critter.gameObject.SetActive(false);
		Type cachedType = critter.CachedType;
		Stack<CosmeticCritter> stack;
		if (!this.inactiveCrittersByType.TryGetValue(cachedType, out stack))
		{
			stack = new Stack<CosmeticCritter>();
			this.inactiveCrittersByType.Add(cachedType, stack);
		}
		stack.Push(critter);
		this.activeCritters.Remove(critter);
		int num;
		if (this.activeCrittersPerType.TryGetValue(cachedType, out num))
		{
			this.activeCrittersPerType[cachedType] = Math.Max(num - 1, 0);
		}
		this.activeCrittersBySeed.Remove(critter.Seed);
	}

	// Token: 0x0600205B RID: 8283 RVA: 0x000A2A8C File Offset: 0x000A0C8C
	private void Update()
	{
		for (int i = 0; i < this.activeCritters.Count; i++)
		{
			CosmeticCritter cosmeticCritter = this.activeCritters[i];
			if (cosmeticCritter.Expired())
			{
				this.FreeCritter(cosmeticCritter);
			}
			else
			{
				cosmeticCritter.Tick();
				List<ICosmeticCritterTickForEach> list;
				if (this.tickForEachCritterOfType.TryGetValue(cosmeticCritter.CachedType, out list))
				{
					for (int j = 0; j < list.Count; j++)
					{
						list[j].TickForEachCritter(cosmeticCritter);
					}
				}
				int k = 0;
				while (k < this.localCritterCatchers.Count)
				{
					CosmeticCritterCatcher cosmeticCritterCatcher = this.localCritterCatchers[k];
					CosmeticCritterAction localCatchAction = cosmeticCritterCatcher.GetLocalCatchAction(cosmeticCritter);
					if (localCatchAction != CosmeticCritterAction.None)
					{
						double num = (PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.timeAsDouble);
						cosmeticCritterCatcher.OnCatch(cosmeticCritter, localCatchAction, num);
						if ((localCatchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
						{
							this.FreeCritter(cosmeticCritter);
							i--;
						}
						if ((localCatchAction & CosmeticCritterAction.SpawnLinked) != CosmeticCritterAction.None && cosmeticCritterCatcher.GetLinkedSpawner() != null)
						{
							this.ReuseOrSpawnNewCritter(cosmeticCritterCatcher.GetLinkedSpawner(), cosmeticCritter.Seed + 1, num);
						}
						if (PhotonNetwork.InRoom && (localCatchAction & CosmeticCritterAction.RPC) != CosmeticCritterAction.None)
						{
							this.photonView.RPC("CosmeticCritterRPC", RpcTarget.Others, new object[] { localCatchAction, cosmeticCritterCatcher.OwnerID, cosmeticCritter.Seed });
							break;
						}
						break;
					}
					else
					{
						k++;
					}
				}
			}
		}
		for (int l = 0; l < this.localCritterSpawners.Count; l++)
		{
			CosmeticCritterSpawnerIndependent cosmeticCritterSpawnerIndependent = this.localCritterSpawners[l];
			int num2;
			if ((!this.activeCrittersPerType.TryGetValue(cosmeticCritterSpawnerIndependent.GetCritterType(), out num2) || num2 < cosmeticCritterSpawnerIndependent.GetCritter().GetGlobalMaxCritters()) && cosmeticCritterSpawnerIndependent.CanSpawnLocal())
			{
				int num3 = Random.Range(0, int.MaxValue);
				if (!this.activeCrittersBySeed.ContainsKey(num3))
				{
					this.ReuseOrSpawnNewCritter(cosmeticCritterSpawnerIndependent, num3, PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.timeAsDouble);
					if (PhotonNetwork.InRoom)
					{
						this.photonView.RPC("CosmeticCritterRPC", RpcTarget.Others, new object[]
						{
							CosmeticCritterAction.RPC | CosmeticCritterAction.Spawn,
							cosmeticCritterSpawnerIndependent.OwnerID,
							num3
						});
					}
				}
			}
		}
	}

	// Token: 0x0600205C RID: 8284 RVA: 0x000A2CD4 File Offset: 0x000A0ED4
	[PunRPC]
	private void CosmeticCritterRPC(CosmeticCritterAction action, int holdableID, int seed, PhotonMessageInfo info)
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		GorillaNot.IncrementRPCCall(photonMessageInfoWrapped, "CosmeticCritterRPC");
		if ((action & CosmeticCritterAction.RPC) == CosmeticCritterAction.None)
		{
			return;
		}
		if (action == (CosmeticCritterAction.RPC | CosmeticCritterAction.Spawn))
		{
			this.SpawnCosmeticCritterRPC(holdableID, seed, photonMessageInfoWrapped);
			return;
		}
		this.CatchCosmeticCritterRPC(action, holdableID, seed, photonMessageInfoWrapped);
	}

	// Token: 0x0600205D RID: 8285 RVA: 0x000A2D14 File Offset: 0x000A0F14
	private void CatchCosmeticCritterRPC(CosmeticCritterAction catchAction, int catcherID, int seed, PhotonMessageInfoWrapped info)
	{
		CosmeticCritter cosmeticCritter;
		if (!this.activeCrittersBySeed.TryGetValue(seed, out cosmeticCritter))
		{
			return;
		}
		int i = 0;
		while (i < this.remoteCritterCatchers.Count)
		{
			CosmeticCritterCatcher cosmeticCritterCatcher = this.remoteCritterCatchers[i];
			if (cosmeticCritterCatcher.OwnerID == catcherID)
			{
				if (!cosmeticCritterCatcher.OwningPlayerMatches(info))
				{
					return;
				}
				if (cosmeticCritterCatcher.ValidateRemoteCatchAction(cosmeticCritter, catchAction, info.SentServerTime))
				{
					cosmeticCritterCatcher.OnCatch(cosmeticCritter, catchAction, info.SentServerTime);
					if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
					{
						this.FreeCritter(cosmeticCritter);
					}
					int num;
					if ((catchAction & CosmeticCritterAction.SpawnLinked) != CosmeticCritterAction.None && cosmeticCritterCatcher.GetLinkedSpawner() != null && (!this.activeCrittersPerType.TryGetValue(cosmeticCritterCatcher.GetLinkedSpawner().GetCritterType(), out num) || num < cosmeticCritterCatcher.GetLinkedSpawner().GetCritter().GetGlobalMaxCritters() + 1))
					{
						this.ReuseOrSpawnNewCritter(cosmeticCritterCatcher.GetLinkedSpawner(), seed + 1, info.SentServerTime);
					}
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x0600205E RID: 8286 RVA: 0x000A2DF8 File Offset: 0x000A0FF8
	private void SpawnCosmeticCritterRPC(int spawnerID, int seed, PhotonMessageInfoWrapped info)
	{
		if (this.activeCrittersBySeed.ContainsKey(seed))
		{
			return;
		}
		int i = 0;
		while (i < this.remoteCritterSpawners.Count)
		{
			CosmeticCritterSpawnerIndependent cosmeticCritterSpawnerIndependent = this.remoteCritterSpawners[i];
			if (cosmeticCritterSpawnerIndependent.OwnerID == spawnerID)
			{
				if (!cosmeticCritterSpawnerIndependent.OwningPlayerMatches(info))
				{
					return;
				}
				int num;
				if ((!this.activeCrittersPerType.TryGetValue(cosmeticCritterSpawnerIndependent.GetCritterType(), out num) || num < cosmeticCritterSpawnerIndependent.GetCritter().GetGlobalMaxCritters()) && cosmeticCritterSpawnerIndependent.CanSpawnRemote(info.SentServerTime))
				{
					this.ReuseOrSpawnNewCritter(cosmeticCritterSpawnerIndependent, seed, info.SentServerTime);
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x04002454 RID: 9300
	private List<CosmeticCritterHoldable> localHoldables;

	// Token: 0x04002455 RID: 9301
	private List<CosmeticCritterSpawnerIndependent> localCritterSpawners;

	// Token: 0x04002456 RID: 9302
	private List<CosmeticCritterSpawnerIndependent> remoteCritterSpawners;

	// Token: 0x04002457 RID: 9303
	private List<CosmeticCritterCatcher> localCritterCatchers;

	// Token: 0x04002458 RID: 9304
	private List<CosmeticCritterCatcher> remoteCritterCatchers;

	// Token: 0x04002459 RID: 9305
	private List<CosmeticCritter> activeCritters;

	// Token: 0x0400245A RID: 9306
	private Dictionary<Type, int> activeCrittersPerType;

	// Token: 0x0400245B RID: 9307
	private Dictionary<int, CosmeticCritter> activeCrittersBySeed;

	// Token: 0x0400245C RID: 9308
	private Dictionary<Type, Stack<CosmeticCritter>> inactiveCrittersByType;

	// Token: 0x0400245D RID: 9309
	private Dictionary<Type, List<ICosmeticCritterTickForEach>> tickForEachCritterOfType;
}
