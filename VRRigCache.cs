using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020005EB RID: 1515
internal class VRRigCache : MonoBehaviour
{
	// Token: 0x1700038C RID: 908
	// (get) Token: 0x06002529 RID: 9513 RVA: 0x000B986A File Offset: 0x000B7A6A
	// (set) Token: 0x0600252A RID: 9514 RVA: 0x000B9871 File Offset: 0x000B7A71
	public static VRRigCache Instance { get; private set; }

	// Token: 0x1700038D RID: 909
	// (get) Token: 0x0600252B RID: 9515 RVA: 0x000B9879 File Offset: 0x000B7A79
	public Transform NetworkParent
	{
		get
		{
			return this.networkParent;
		}
	}

	// Token: 0x1700038E RID: 910
	// (get) Token: 0x0600252C RID: 9516 RVA: 0x000B9881 File Offset: 0x000B7A81
	// (set) Token: 0x0600252D RID: 9517 RVA: 0x000B9888 File Offset: 0x000B7A88
	public static bool isInitialized { get; private set; }

	// Token: 0x14000052 RID: 82
	// (add) Token: 0x0600252E RID: 9518 RVA: 0x000B9890 File Offset: 0x000B7A90
	// (remove) Token: 0x0600252F RID: 9519 RVA: 0x000B98C4 File Offset: 0x000B7AC4
	public static event Action OnPostInitialize;

	// Token: 0x14000053 RID: 83
	// (add) Token: 0x06002530 RID: 9520 RVA: 0x000B98F8 File Offset: 0x000B7AF8
	// (remove) Token: 0x06002531 RID: 9521 RVA: 0x000B992C File Offset: 0x000B7B2C
	public static event Action OnPostSpawnRig;

	// Token: 0x14000054 RID: 84
	// (add) Token: 0x06002532 RID: 9522 RVA: 0x000B9960 File Offset: 0x000B7B60
	// (remove) Token: 0x06002533 RID: 9523 RVA: 0x000B9994 File Offset: 0x000B7B94
	public static event Action<RigContainer> OnRigActivated;

	// Token: 0x14000055 RID: 85
	// (add) Token: 0x06002534 RID: 9524 RVA: 0x000B99C8 File Offset: 0x000B7BC8
	// (remove) Token: 0x06002535 RID: 9525 RVA: 0x000B99FC File Offset: 0x000B7BFC
	public static event Action<RigContainer> OnRigDeactivated;

	// Token: 0x14000056 RID: 86
	// (add) Token: 0x06002536 RID: 9526 RVA: 0x000B9A30 File Offset: 0x000B7C30
	// (remove) Token: 0x06002537 RID: 9527 RVA: 0x000B9A64 File Offset: 0x000B7C64
	public static event Action<RigContainer> OnRigNameChanged;

	// Token: 0x06002538 RID: 9528 RVA: 0x000B9A98 File Offset: 0x000B7C98
	private void Awake()
	{
		this.InitializeVRRigCache();
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			if (this.localRig.Rig.bodyRenderer != null)
			{
				this.localRig.Rig.bodyRenderer.SetupAsLocalPlayerBody();
			}
		}
	}

	// Token: 0x06002539 RID: 9529 RVA: 0x000B9B20 File Offset: 0x000B7D20
	private void OnDestroy()
	{
		if (VRRigCache.Instance == this)
		{
			VRRigCache.Instance = null;
		}
		VRRigCache.isInitialized = false;
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		}
	}

	// Token: 0x0600253A RID: 9530 RVA: 0x000B9B8C File Offset: 0x000B7D8C
	public void InitializeVRRigCache()
	{
		if (VRRigCache.isInitialized || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (VRRigCache.Instance != null && VRRigCache.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		VRRigCache.Instance = this;
		if (this.rigParent == null)
		{
			this.rigParent = base.transform;
		}
		if (this.networkParent == null)
		{
			this.networkParent = base.transform;
		}
		for (int i = 0; i < this.rigAmount; i++)
		{
			RigContainer rigContainer = this.SpawnRig();
			VRRigCache.freeRigs.Enqueue(rigContainer);
			rigContainer.Rig.BuildInitialize();
			rigContainer.Rig.transform.parent = null;
		}
		VRRigCache.isInitialized = true;
		Action onPostInitialize = VRRigCache.OnPostInitialize;
		if (onPostInitialize == null)
		{
			return;
		}
		onPostInitialize();
	}

	// Token: 0x0600253B RID: 9531 RVA: 0x000B9C58 File Offset: 0x000B7E58
	private RigContainer SpawnRig()
	{
		if (this.rigTemplate.activeSelf)
		{
			this.rigTemplate.SetActive(false);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.rigTemplate, this.rigParent, false);
		Action onPostSpawnRig = VRRigCache.OnPostSpawnRig;
		if (onPostSpawnRig != null)
		{
			onPostSpawnRig();
		}
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<RigContainer>();
	}

	// Token: 0x0600253C RID: 9532 RVA: 0x000B9CAB File Offset: 0x000B7EAB
	internal bool TryGetVrrig(Player targetPlayer, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayer.ActorNumber), out playerRig);
	}

	// Token: 0x0600253D RID: 9533 RVA: 0x000B9CC4 File Offset: 0x000B7EC4
	internal bool TryGetVrrig(NetPlayer targetPlayer, out RigContainer playerRig)
	{
		playerRig = null;
		if (ApplicationQuittingState.IsQuitting)
		{
			return false;
		}
		if (targetPlayer == null || targetPlayer.IsNull)
		{
			this.LogError("VrRigCache - target player is null");
			return false;
		}
		if (targetPlayer.IsLocal)
		{
			playerRig = this.localRig;
			return true;
		}
		if (!targetPlayer.InRoom)
		{
			this.LogWarning("player is not in room?? " + targetPlayer.UserId);
			return false;
		}
		if (VRRigCache.rigsInUse.ContainsKey(targetPlayer))
		{
			playerRig = VRRigCache.rigsInUse[targetPlayer];
		}
		else
		{
			if (VRRigCache.freeRigs.Count <= 0)
			{
				this.LogWarning("all rigs are in use");
				return false;
			}
			playerRig = VRRigCache.freeRigs.Dequeue();
			playerRig.Creator = targetPlayer;
			VRRigCache.rigsInUse.Add(targetPlayer, playerRig);
			VRRig rig = playerRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			VRRig rig2 = playerRig.Rig;
			rig2.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
			playerRig.gameObject.SetActive(true);
			Action<RigContainer> onRigActivated = VRRigCache.OnRigActivated;
			if (onRigActivated != null)
			{
				onRigActivated(playerRig);
			}
		}
		return true;
	}

	// Token: 0x0600253E RID: 9534 RVA: 0x000B9DE8 File Offset: 0x000B7FE8
	private void AddRigToGorillaParent(NetPlayer player, VRRig vrrig)
	{
		GorillaParent instance = GorillaParent.instance;
		if (instance == null)
		{
			return;
		}
		if (!instance.vrrigs.Contains(vrrig))
		{
			instance.vrrigs.Add(vrrig);
		}
		if (!instance.vrrigDict.ContainsKey(player))
		{
			instance.vrrigDict.Add(player, vrrig);
			return;
		}
		instance.vrrigDict[player] = vrrig;
	}

	// Token: 0x0600253F RID: 9535 RVA: 0x000B9E4C File Offset: 0x000B804C
	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (newPlayer.ActorNumber == -1)
		{
			Debug.LogError("LocalPlayer returned, vrrig no correctly initialised");
		}
		RigContainer rigContainer;
		if (this.TryGetVrrig(newPlayer, out rigContainer))
		{
			this.AddRigToGorillaParent(newPlayer, rigContainer.Rig);
		}
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x000B9E84 File Offset: 0x000B8084
	public void OnJoinedRoom()
	{
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			RigContainer rigContainer;
			if (this.TryGetVrrig(netPlayer, out rigContainer))
			{
				this.AddRigToGorillaParent(netPlayer, rigContainer.Rig);
			}
		}
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x000B9EC8 File Offset: 0x000B80C8
	private void RemoveRigFromGorillaParent(NetPlayer player, VRRig vrrig)
	{
		GorillaParent instance = GorillaParent.instance;
		if (instance == null)
		{
			return;
		}
		if (instance.vrrigs.Contains(vrrig))
		{
			instance.vrrigs.Remove(vrrig);
		}
		if (instance.vrrigDict.ContainsKey(player))
		{
			instance.vrrigDict.Remove(player);
		}
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x000B9F20 File Offset: 0x000B8120
	public void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (leavingPlayer.IsNull)
		{
			Debug.LogError("Leaving players NetPlayer is Null");
			this.CheckForMissingPlayer();
		}
		RigContainer rigContainer;
		if (!VRRigCache.rigsInUse.TryGetValue(leavingPlayer, out rigContainer))
		{
			this.LogError("failed to find player's vrrig who left " + leavingPlayer.UserId);
			return;
		}
		rigContainer.gameObject.Disable();
		VRRig rig = rigContainer.Rig;
		rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		VRRigCache.freeRigs.Enqueue(rigContainer);
		VRRigCache.rigsInUse.Remove(leavingPlayer);
		this.RemoveRigFromGorillaParent(leavingPlayer, rigContainer.Rig);
		Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
		if (onRigDeactivated == null)
		{
			return;
		}
		onRigDeactivated(rigContainer);
	}

	// Token: 0x06002543 RID: 9539 RVA: 0x000B9FCC File Offset: 0x000B81CC
	private void CheckForMissingPlayer()
	{
		foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in VRRigCache.rigsInUse)
		{
			if (keyValuePair.Key == null || keyValuePair.Value == null)
			{
				Debug.LogError("Somehow null reference in rigsInUse");
			}
			else if (!keyValuePair.Key.InRoom)
			{
				keyValuePair.Value.gameObject.Disable();
				VRRig rig = keyValuePair.Value.Rig;
				rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
				VRRigCache.freeRigs.Enqueue(keyValuePair.Value);
				VRRigCache.rigsInUse.Remove(keyValuePair.Key);
				this.RemoveRigFromGorillaParent(keyValuePair.Key, keyValuePair.Value.Rig);
				Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
				if (onRigDeactivated != null)
				{
					onRigDeactivated(keyValuePair.Value);
				}
			}
		}
	}

	// Token: 0x06002544 RID: 9540 RVA: 0x000BA0E0 File Offset: 0x000B82E0
	public void OnLeftRoom()
	{
		foreach (NetPlayer netPlayer in VRRigCache.rigsInUse.Keys.ToArray<NetPlayer>())
		{
			RigContainer rigContainer = VRRigCache.rigsInUse[netPlayer];
			if (!(rigContainer == null))
			{
				VRRig rig = VRRigCache.rigsInUse[netPlayer].Rig;
				VRRig rig2 = rigContainer.Rig;
				rig2.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
				rigContainer.gameObject.Disable();
				VRRigCache.rigsInUse.Remove(netPlayer);
				this.RemoveRigFromGorillaParent(netPlayer, rig);
				VRRigCache.freeRigs.Enqueue(rigContainer);
				Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
				if (onRigDeactivated != null)
				{
					onRigDeactivated(rigContainer);
				}
			}
		}
	}

	// Token: 0x06002545 RID: 9541 RVA: 0x000BA198 File Offset: 0x000B8398
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal VRRig[] GetAllRigs()
	{
		VRRig[] array = new VRRig[VRRigCache.rigsInUse.Count + VRRigCache.freeRigs.Count];
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			array[num] = rigContainer.Rig;
			num++;
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			array[num] = rigContainer2.Rig;
			num++;
		}
		return array;
	}

	// Token: 0x06002546 RID: 9542 RVA: 0x000BA260 File Offset: 0x000B8460
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void GetAllUsedRigs(List<VRRig> rigs)
	{
		if (rigs == null)
		{
			return;
		}
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigs.Add(rigContainer.Rig);
			num++;
		}
	}

	// Token: 0x06002547 RID: 9543 RVA: 0x000BA2C8 File Offset: 0x000B84C8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal int GetAllRigsHash()
	{
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			num += rigContainer.GetInstanceID();
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			num += rigContainer2.GetInstanceID();
		}
		return num;
	}

	// Token: 0x06002548 RID: 9544 RVA: 0x000BA36C File Offset: 0x000B856C
	internal void InstantiateNetworkObject()
	{
		PrefabType prefabType;
		if (!VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("Player Network Controller", out prefabType) || prefabType.prefab == null)
		{
			Debug.LogError("OnJoinedRoom: Unable to find player prefab to spawn");
			return;
		}
		GameObject gameObject = GTPlayer.Instance.gameObject;
		Color playerColor = this.localRig.Rig.playerColor;
		VRRigCache.rigRGBData[0] = playerColor.r;
		VRRigCache.rigRGBData[1] = playerColor.g;
		VRRigCache.rigRGBData[2] = playerColor.b;
		NetworkSystem.Instance.NetInstantiate(prefabType.prefab, gameObject.transform.position, gameObject.transform.rotation, false, 0, VRRigCache.rigRGBData, null);
	}

	// Token: 0x06002549 RID: 9545 RVA: 0x000023F4 File Offset: 0x000005F4
	private void LogInfo(string log)
	{
	}

	// Token: 0x0600254A RID: 9546 RVA: 0x000023F4 File Offset: 0x000005F4
	private void LogWarning(string log)
	{
	}

	// Token: 0x0600254B RID: 9547 RVA: 0x000023F4 File Offset: 0x000005F4
	private void LogError(string log)
	{
	}

	// Token: 0x040029F2 RID: 10738
	public RigContainer localRig;

	// Token: 0x040029F3 RID: 10739
	[SerializeField]
	private Transform rigParent;

	// Token: 0x040029F4 RID: 10740
	[SerializeField]
	private Transform networkParent;

	// Token: 0x040029F5 RID: 10741
	[SerializeField]
	private GameObject rigTemplate;

	// Token: 0x040029F6 RID: 10742
	[SerializeField]
	private int rigAmount = 9;

	// Token: 0x040029F7 RID: 10743
	[OnEnterPlay_Clear]
	private static Queue<RigContainer> freeRigs = new Queue<RigContainer>(10);

	// Token: 0x040029F8 RID: 10744
	[OnEnterPlay_Clear]
	private static Dictionary<NetPlayer, RigContainer> rigsInUse = new Dictionary<NetPlayer, RigContainer>(10);

	// Token: 0x040029FF RID: 10751
	private static object[] rigRGBData = new object[] { 0f, 0f, 0f };
}
