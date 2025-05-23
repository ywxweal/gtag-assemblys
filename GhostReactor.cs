using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200057B RID: 1403
public class GhostReactor : MonoBehaviour
{
	// Token: 0x06002248 RID: 8776 RVA: 0x000AB7C4 File Offset: 0x000A99C4
	private void Awake()
	{
		GhostReactor.instance = this;
		this.reviveStations = new List<GRReviveStation>();
		base.GetComponentsInChildren<GRReviveStation>(this.reviveStations);
		for (int i = 0; i < this.reviveStations.Count; i++)
		{
			this.reviveStations[i].Index = i;
		}
		this.vrRigs = new List<VRRig>();
		for (int j = 0; j < this.itemPurchaseStands.Count; j++)
		{
			if (this.itemPurchaseStands[j] == null)
			{
				Debug.LogErrorFormat("Null Item Purchase Stand {0}", new object[] { j });
			}
			else
			{
				this.itemPurchaseStands[j].Setup(j);
			}
		}
		for (int k = 0; k < this.toolPurchasingStations.Count; k++)
		{
			if (this.toolPurchasingStations[k] == null)
			{
				Debug.LogErrorFormat("Null Tool Purchasing Station {0}", new object[] { k });
			}
			else
			{
				this.toolPurchasingStations[k].PurchaseStationId = k;
			}
		}
		this.randomGenerator = new SRand(Random.Range(0, int.MaxValue));
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000AB8E8 File Offset: 0x000A9AE8
	private void OnEnable()
	{
		GhostReactorManager.instance.reactor = this;
		GameEntityManager.instance.zoneLimit = this.zoneLimit;
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
		VRRigCache.OnRigActivated += this.OnVRRigsChanged;
		VRRigCache.OnRigDeactivated += this.OnVRRigsChanged;
		VRRigCache.OnRigNameChanged += this.OnVRRigsChanged;
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnLocalPlayerConnectedToRoom;
		}
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x000AB978 File Offset: 0x000A9B78
	private void OnDisable()
	{
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(false);
		VRRigCache.OnRigActivated -= this.OnVRRigsChanged;
		VRRigCache.OnRigDeactivated -= this.OnVRRigsChanged;
		VRRigCache.OnRigNameChanged -= this.OnVRRigsChanged;
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnLocalPlayerConnectedToRoom;
		}
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x000AB9E8 File Offset: 0x000A9BE8
	public GRPatrolPath GetPatrolPath(int patrolPathId)
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		return this.levelGenerator.GetPatrolPath(patrolPathId);
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x000ABA08 File Offset: 0x000A9C08
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsAuthority() && Time.timeAsDouble - this.lastCollectibleDispenserUpdateTime > (double)this.collectibleDeispenserUpdateFrequency)
		{
			this.lastCollectibleDispenserUpdateTime = Time.timeAsDouble;
			for (int i = 0; i < this.collectibleDispensers.Count; i++)
			{
				if (this.collectibleDispensers[i] != null && this.collectibleDispensers[i].ReadyToDispenseNewCollectible)
				{
					this.collectibleDispensers[i].RequestDispenseCollectible();
				}
			}
		}
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x000ABA91 File Offset: 0x000A9C91
	private bool IsAuthority()
	{
		return GameEntityManager.instance.IsAuthority();
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x000ABA9F File Offset: 0x000A9C9F
	private bool IsAuthorityPlayer(NetPlayer player)
	{
		return GameEntityManager.instance.IsAuthorityPlayer(player);
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x000ABAAE File Offset: 0x000A9CAE
	private bool IsAuthorityPlayer(Player player)
	{
		return GameEntityManager.instance.IsAuthorityPlayer(player);
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x000ABABD File Offset: 0x000A9CBD
	private Player GetAuthorityPlayer()
	{
		return GameEntityManager.instance.GetAuthorityPlayer();
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x000ABACC File Offset: 0x000A9CCC
	private void OnLocalPlayerConnectedToRoom()
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null)
		{
			grplayer.currency = 0;
		}
		this.shiftManager.EnemyDeaths = 0;
		this.shiftManager.CoresCollected = 0;
		this.shiftManager.PlayerDeaths = 0;
		this.shiftManager.RefreshShiftStatsDisplay();
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x000ABB23 File Offset: 0x000A9D23
	private void OnVRRigsChanged(RigContainer container)
	{
		this.VRRigRefresh();
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x000ABB2C File Offset: 0x000A9D2C
	public void VRRigRefresh()
	{
		this.vrRigs.Clear();
		this.vrRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.vrRigs.Sort(delegate(VRRig a, VRRig b)
		{
			if (a == null || a.OwningNetPlayer == null)
			{
				return 1;
			}
			if (b == null || b.OwningNetPlayer == null)
			{
				return -1;
			}
			return a.OwningNetPlayer.ActorNumber.CompareTo(b.OwningNetPlayer.ActorNumber);
		});
		this.RefreshScoreboards();
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x000ABB94 File Offset: 0x000A9D94
	public void RefreshScoreboards()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].Refresh(this.vrRigs);
		}
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x000ABBD0 File Offset: 0x000A9DD0
	public int GetItemCost(int entityTypeId)
	{
		int num;
		if (!GameEntityManager.PriceLookup(entityTypeId, out num))
		{
			return 100;
		}
		return num;
	}

	// Token: 0x04002649 RID: 9801
	public const int GHOSTREACTOR_ZONE_ID = 5;

	// Token: 0x0400264A RID: 9802
	public const GTZone GT_ZONE_GHOSTREACTOR = GTZone.ghostReactor;

	// Token: 0x0400264B RID: 9803
	public static GhostReactor instance;

	// Token: 0x0400264C RID: 9804
	public Transform restartMarker;

	// Token: 0x0400264D RID: 9805
	public PhotonView photonView;

	// Token: 0x0400264E RID: 9806
	public AudioSource entryRoomAudio;

	// Token: 0x0400264F RID: 9807
	public AudioClip entryRoomDeathSound;

	// Token: 0x04002650 RID: 9808
	public BoxCollider zoneLimit;

	// Token: 0x04002651 RID: 9809
	public BoxCollider safeZoneLimit;

	// Token: 0x04002652 RID: 9810
	public List<GhostReactor.TempEnemySpawnInfo> tempSpawnEnemies;

	// Token: 0x04002653 RID: 9811
	public GameEntity overrideEnemySpawn;

	// Token: 0x04002654 RID: 9812
	public List<GameEntity> tempSpawnItems;

	// Token: 0x04002655 RID: 9813
	public Transform tempSpawnItemsMarker;

	// Token: 0x04002656 RID: 9814
	public List<GRUIBuyItem> itemPurchaseStands;

	// Token: 0x04002657 RID: 9815
	public List<GRToolPurchaseStation> toolPurchasingStations;

	// Token: 0x04002658 RID: 9816
	public List<GRUIScoreboard> scoreboards;

	// Token: 0x04002659 RID: 9817
	public List<GRCollectibleDispenser> collectibleDispensers = new List<GRCollectibleDispenser>();

	// Token: 0x0400265A RID: 9818
	public GRUIStationEmployeeBadges employeeBadges;

	// Token: 0x0400265B RID: 9819
	public GRUIEmployeeTerminal employeeTerminal;

	// Token: 0x0400265C RID: 9820
	public GhostReactorShiftManager shiftManager;

	// Token: 0x0400265D RID: 9821
	public GhostReactorLevelGenerator levelGenerator;

	// Token: 0x0400265E RID: 9822
	public GRCurrencyDepositor currencyDepositor;

	// Token: 0x0400265F RID: 9823
	public LayerMask envLayerMask;

	// Token: 0x04002660 RID: 9824
	[ReadOnly]
	public List<GRReviveStation> reviveStations;

	// Token: 0x04002661 RID: 9825
	private List<VRRig> vrRigs;

	// Token: 0x04002662 RID: 9826
	private float collectibleDeispenserUpdateFrequency = 3f;

	// Token: 0x04002663 RID: 9827
	private double lastCollectibleDispenserUpdateTime = -10.0;

	// Token: 0x04002664 RID: 9828
	private SRand randomGenerator;

	// Token: 0x04002665 RID: 9829
	public GRDropZone dropZone;

	// Token: 0x04002666 RID: 9830
	public static float DROP_ZONE_REPEL = 2.25f;

	// Token: 0x0200057C RID: 1404
	[Serializable]
	public class TempEnemySpawnInfo
	{
		// Token: 0x04002667 RID: 9831
		public GameEntity prefab;

		// Token: 0x04002668 RID: 9832
		public Transform spawnMarker;

		// Token: 0x04002669 RID: 9833
		public int patrolPath;
	}

	// Token: 0x0200057D RID: 1405
	public enum EntityGroupTypes
	{
		// Token: 0x0400266B RID: 9835
		EnemyChaser,
		// Token: 0x0400266C RID: 9836
		EnemyChaserArmored,
		// Token: 0x0400266D RID: 9837
		EnemyRanged,
		// Token: 0x0400266E RID: 9838
		EnemyRangedArmored,
		// Token: 0x0400266F RID: 9839
		CollectibleFlower,
		// Token: 0x04002670 RID: 9840
		BarrierEnergyCostGate,
		// Token: 0x04002671 RID: 9841
		BarrierSpectralWall,
		// Token: 0x04002672 RID: 9842
		HazardSpectralLiquid
	}

	// Token: 0x0200057E RID: 1406
	public enum EnemyType
	{
		// Token: 0x04002674 RID: 9844
		Chaser,
		// Token: 0x04002675 RID: 9845
		Ranged,
		// Token: 0x04002676 RID: 9846
		Environment
	}
}
