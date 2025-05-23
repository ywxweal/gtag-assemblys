using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000600 RID: 1536
public abstract class GorillaGameManager : MonoBehaviourPunCallbacks, ITickSystemTick, IWrappedSerializable, INetworkStruct
{
	// Token: 0x14000057 RID: 87
	// (add) Token: 0x060025D6 RID: 9686 RVA: 0x000BC28C File Offset: 0x000BA48C
	// (remove) Token: 0x060025D7 RID: 9687 RVA: 0x000BC2C0 File Offset: 0x000BA4C0
	public static event GorillaGameManager.OnTouchDelegate OnTouch;

	// Token: 0x1700039D RID: 925
	// (get) Token: 0x060025D8 RID: 9688 RVA: 0x000BC2F3 File Offset: 0x000BA4F3
	public static GorillaGameManager instance
	{
		get
		{
			return global::GorillaGameModes.GameMode.ActiveGameMode;
		}
	}

	// Token: 0x1700039E RID: 926
	// (get) Token: 0x060025D9 RID: 9689 RVA: 0x000BC2FA File Offset: 0x000BA4FA
	// (set) Token: 0x060025DA RID: 9690 RVA: 0x000BC302 File Offset: 0x000BA502
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x060025DB RID: 9691 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void Awake()
	{
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x000023F4 File Offset: 0x000005F4
	private new void OnEnable()
	{
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x000023F4 File Offset: 0x000005F4
	private new void OnDisable()
	{
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x000BC30C File Offset: 0x000BA50C
	public virtual void Tick()
	{
		if (this.lastCheck + this.checkCooldown < Time.time)
		{
			this.lastCheck = Time.time;
			if (NetworkSystem.Instance.IsMasterClient && !this.ValidGameMode())
			{
				global::GorillaGameModes.GameMode.ChangeGameFromProperty();
				return;
			}
			this.InfrequentUpdate();
		}
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x000BC359 File Offset: 0x000BA559
	public virtual void InfrequentUpdate()
	{
		global::GorillaGameModes.GameMode.RefreshPlayers();
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x060025E0 RID: 9696 RVA: 0x000BC370 File Offset: 0x000BA570
	public virtual string GameModeName()
	{
		return "NONE";
	}

	// Token: 0x060025E1 RID: 9697 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
	}

	// Token: 0x060025E2 RID: 9698 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
	}

	// Token: 0x060025E3 RID: 9699 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void HitPlayer(NetPlayer player)
	{
	}

	// Token: 0x060025E4 RID: 9700 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return false;
	}

	// Token: 0x060025E5 RID: 9701 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void HandleHandTap(NetPlayer tappingPlayer, Tappable hitTappable, bool leftHand, Vector3 handVelocity, Vector3 tapSurfaceNormal)
	{
	}

	// Token: 0x060025E6 RID: 9702 RVA: 0x00047642 File Offset: 0x00045842
	public virtual bool CanJoinFrienship(NetPlayer player)
	{
		return true;
	}

	// Token: 0x060025E7 RID: 9703 RVA: 0x000BC377 File Offset: 0x000BA577
	public virtual void HandleRoundComplete()
	{
		PlayerGameEvents.GameModeCompleteRound();
	}

	// Token: 0x060025E8 RID: 9704 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
	}

	// Token: 0x060025E9 RID: 9705 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return false;
	}

	// Token: 0x060025EA RID: 9706 RVA: 0x000BC37E File Offset: 0x000BA57E
	public virtual VRRig FindPlayerVRRig(NetPlayer player)
	{
		this.returnRig = null;
		this.outContainer = null;
		if (player == null)
		{
			return null;
		}
		if (VRRigCache.Instance.TryGetVrrig(player, out this.outContainer))
		{
			this.returnRig = this.outContainer.Rig;
		}
		return this.returnRig;
	}

	// Token: 0x060025EB RID: 9707 RVA: 0x000BC3C0 File Offset: 0x000BA5C0
	public static VRRig StaticFindRigForPlayer(NetPlayer player)
	{
		VRRig vrrig = null;
		RigContainer rigContainer;
		if (GorillaGameManager.instance != null)
		{
			vrrig = GorillaGameManager.instance.FindPlayerVRRig(player);
		}
		else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			vrrig = rigContainer.Rig;
		}
		return vrrig;
	}

	// Token: 0x060025EC RID: 9708 RVA: 0x000BC401 File Offset: 0x000BA601
	public virtual float[] LocalPlayerSpeed()
	{
		this.playerSpeed[0] = this.slowJumpLimit;
		this.playerSpeed[1] = this.slowJumpMultiplier;
		return this.playerSpeed;
	}

	// Token: 0x060025ED RID: 9709 RVA: 0x000BC428 File Offset: 0x000BA628
	public virtual void UpdatePlayerAppearance(VRRig rig)
	{
		ScienceExperimentManager instance = ScienceExperimentManager.instance;
		int num;
		if (instance != null && instance.GetMaterialIfPlayerInGame(rig.creator.ActorNumber, out num))
		{
			rig.ChangeMaterialLocal(num);
			return;
		}
		int num2 = this.MyMatIndex(rig.creator);
		rig.ChangeMaterialLocal(num2);
	}

	// Token: 0x060025EE RID: 9710 RVA: 0x00002076 File Offset: 0x00000276
	public virtual int MyMatIndex(NetPlayer forPlayer)
	{
		return 0;
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x000BC477 File Offset: 0x000BA677
	public virtual int SpecialHandFX(NetPlayer player, RigContainer rigContainer)
	{
		return -1;
	}

	// Token: 0x060025F0 RID: 9712 RVA: 0x000BC47A File Offset: 0x000BA67A
	public virtual bool ValidGameMode()
	{
		return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains(this.GameTypeName());
	}

	// Token: 0x060025F1 RID: 9713 RVA: 0x000BC4A2 File Offset: 0x000BA6A2
	public static void OnInstanceReady(Action action)
	{
		GorillaParent.OnReplicatedClientReady(delegate
		{
			if (GorillaGameManager.instance)
			{
				action();
				return;
			}
			GorillaGameManager.onInstanceReady = (Action)Delegate.Combine(GorillaGameManager.onInstanceReady, action);
		});
	}

	// Token: 0x060025F2 RID: 9714 RVA: 0x000BC4C0 File Offset: 0x000BA6C0
	public static void ReplicatedClientReady()
	{
		GorillaGameManager.replicatedClientReady = true;
	}

	// Token: 0x060025F3 RID: 9715 RVA: 0x000BC4C8 File Offset: 0x000BA6C8
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaGameManager.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaGameManager.onReplicatedClientReady = (Action)Delegate.Combine(GorillaGameManager.onReplicatedClientReady, action);
	}

	// Token: 0x1700039F RID: 927
	// (get) Token: 0x060025F4 RID: 9716 RVA: 0x000BC4ED File Offset: 0x000BA6ED
	internal GameModeSerializer Serializer
	{
		get
		{
			return this.serializer;
		}
	}

	// Token: 0x060025F5 RID: 9717 RVA: 0x000BC4F5 File Offset: 0x000BA6F5
	internal virtual void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		this.serializer = netSerializer;
	}

	// Token: 0x060025F6 RID: 9718 RVA: 0x000BC4FE File Offset: 0x000BA6FE
	internal virtual void NetworkLinkDestroyed(GameModeSerializer netSerializer)
	{
		if (this.serializer == netSerializer)
		{
			this.serializer = null;
		}
	}

	// Token: 0x060025F7 RID: 9719
	public abstract GameModeType GameType();

	// Token: 0x060025F8 RID: 9720 RVA: 0x000BC518 File Offset: 0x000BA718
	public string GameTypeName()
	{
		return this.GameType().ToString();
	}

	// Token: 0x060025F9 RID: 9721
	public abstract void AddFusionDataBehaviour(NetworkObject behaviour);

	// Token: 0x060025FA RID: 9722
	public abstract void OnSerializeRead(object newData);

	// Token: 0x060025FB RID: 9723
	public abstract object OnSerializeWrite();

	// Token: 0x060025FC RID: 9724
	public abstract void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x060025FD RID: 9725
	public abstract void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x060025FE RID: 9726 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void Reset()
	{
	}

	// Token: 0x060025FF RID: 9727 RVA: 0x000BC53C File Offset: 0x000BA73C
	public virtual void StartPlaying()
	{
		TickSystem<object>.AddTickCallback(this);
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.OnMasterClientSwitched;
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x000BC5A4 File Offset: 0x000BA7A4
	public virtual void StopPlaying()
	{
		TickSystem<object>.RemoveTickCallback(this);
		NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerEnteredRoom;
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.OnMasterClientSwitched;
		this.lastCheck = 0f;
	}

	// Token: 0x06002601 RID: 9729 RVA: 0x000023F4 File Offset: 0x000005F4
	public new virtual void OnMasterClientSwitched(Player newMaster)
	{
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x000023F4 File Offset: 0x000005F4
	public new virtual void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x000023F4 File Offset: 0x000005F4
	public new virtual void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x000BC607 File Offset: 0x000BA807
	public virtual void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
		if (this.lastTaggedActorNr.ContainsKey(otherPlayer.ActorNumber))
		{
			this.lastTaggedActorNr.Remove(otherPlayer.ActorNumber);
		}
	}

	// Token: 0x06002605 RID: 9733 RVA: 0x000BC63E File Offset: 0x000BA83E
	public virtual void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void OnMasterClientSwitched(NetPlayer newMaster)
	{
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x000BC650 File Offset: 0x000BA850
	internal static void ForceStopGame_DisconnectAndDestroy()
	{
		Application.Quit();
		NetworkSystem instance = NetworkSystem.Instance;
		if (instance != null)
		{
			instance.ReturnToSinglePlayer();
		}
		Object.DestroyImmediate(PhotonNetworkController.Instance);
		Object.DestroyImmediate(GTPlayer.Instance);
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
	}

	// Token: 0x06002608 RID: 9736 RVA: 0x000BC6A8 File Offset: 0x000BA8A8
	public void AddLastTagged(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (this.lastTaggedActorNr.ContainsKey(taggedPlayer.ActorNumber))
		{
			this.lastTaggedActorNr[taggedPlayer.ActorNumber] = taggingPlayer.ActorNumber;
			return;
		}
		this.lastTaggedActorNr.Add(taggedPlayer.ActorNumber, taggingPlayer.ActorNumber);
	}

	// Token: 0x06002609 RID: 9737 RVA: 0x000BC6F8 File Offset: 0x000BA8F8
	public void WriteLastTagged(PhotonStream stream)
	{
		stream.SendNext(this.lastTaggedActorNr.Count);
		foreach (KeyValuePair<int, int> keyValuePair in this.lastTaggedActorNr)
		{
			stream.SendNext(keyValuePair.Key);
			stream.SendNext(keyValuePair.Value);
		}
	}

	// Token: 0x0600260A RID: 9738 RVA: 0x000BC780 File Offset: 0x000BA980
	public void ReadLastTagged(PhotonStream stream)
	{
		this.lastTaggedActorNr.Clear();
		int num = Mathf.Min((int)stream.ReceiveNext(), 10);
		for (int i = 0; i < num; i++)
		{
			this.lastTaggedActorNr.Add((int)stream.ReceiveNext(), (int)stream.ReceiveNext());
		}
	}

	// Token: 0x04002A7C RID: 10876
	public object obj;

	// Token: 0x04002A7D RID: 10877
	public float fastJumpLimit;

	// Token: 0x04002A7E RID: 10878
	public float fastJumpMultiplier;

	// Token: 0x04002A7F RID: 10879
	public float slowJumpLimit;

	// Token: 0x04002A80 RID: 10880
	public float slowJumpMultiplier;

	// Token: 0x04002A81 RID: 10881
	public float lastCheck;

	// Token: 0x04002A82 RID: 10882
	public float checkCooldown = 3f;

	// Token: 0x04002A83 RID: 10883
	public float startingToLookForFriend;

	// Token: 0x04002A84 RID: 10884
	public float timeToSpendLookingForFriend = 10f;

	// Token: 0x04002A85 RID: 10885
	public bool successfullyFoundFriend;

	// Token: 0x04002A86 RID: 10886
	public float tagDistanceThreshold = 4f;

	// Token: 0x04002A87 RID: 10887
	public bool testAssault;

	// Token: 0x04002A88 RID: 10888
	public bool endGameManually;

	// Token: 0x04002A89 RID: 10889
	public NetPlayer currentMasterClient;

	// Token: 0x04002A8A RID: 10890
	public VRRig returnRig;

	// Token: 0x04002A8B RID: 10891
	private NetPlayer outPlayer;

	// Token: 0x04002A8C RID: 10892
	private int outInt;

	// Token: 0x04002A8D RID: 10893
	private VRRig tempRig;

	// Token: 0x04002A8E RID: 10894
	public NetPlayer[] currentNetPlayerArray;

	// Token: 0x04002A8F RID: 10895
	public float[] playerSpeed = new float[2];

	// Token: 0x04002A90 RID: 10896
	private RigContainer outContainer;

	// Token: 0x04002A91 RID: 10897
	public Dictionary<int, int> lastTaggedActorNr = new Dictionary<int, int>();

	// Token: 0x04002A93 RID: 10899
	private static Action onInstanceReady;

	// Token: 0x04002A94 RID: 10900
	private static bool replicatedClientReady;

	// Token: 0x04002A95 RID: 10901
	private static Action onReplicatedClientReady;

	// Token: 0x04002A96 RID: 10902
	private GameModeSerializer serializer;

	// Token: 0x02000601 RID: 1537
	// (Invoke) Token: 0x0600260D RID: 9741
	public delegate void OnTouchDelegate(NetPlayer taggedPlayer, NetPlayer taggingPlayer);
}
