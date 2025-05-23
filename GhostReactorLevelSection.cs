using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000582 RID: 1410
public class GhostReactorLevelSection : MonoBehaviour
{
	// Token: 0x17000357 RID: 855
	// (get) Token: 0x0600226A RID: 8810 RVA: 0x000AC762 File Offset: 0x000AA962
	public Transform Anchor
	{
		get
		{
			return this.anchorTransform;
		}
	}

	// Token: 0x17000358 RID: 856
	// (get) Token: 0x0600226B RID: 8811 RVA: 0x000AC76A File Offset: 0x000AA96A
	public BoxCollider BoundingCollider
	{
		get
		{
			return this.boundingCollider;
		}
	}

	// Token: 0x0600226C RID: 8812 RVA: 0x000AC774 File Offset: 0x000AA974
	private void Awake()
	{
		this.spawnPointGroupLookup = new GhostReactorLevelSection.SpawnPointGroup[4];
		for (int i = 0; i < this.spawnPointGroups.Count; i++)
		{
			this.spawnPointGroups[i].SpawnPointIndexes = new List<int>();
			int type = (int)this.spawnPointGroups[i].type;
			if (type < this.spawnPointGroupLookup.Length)
			{
				this.spawnPointGroupLookup[type] = this.spawnPointGroups[i];
			}
		}
		for (int j = 0; j < this.patrolPaths.Count; j++)
		{
			this.patrolPaths[j].index = j;
		}
		this.prePlacedGameEntities = new List<GameEntity>(128);
		base.GetComponentsInChildren<GameEntity>(this.prePlacedGameEntities);
		for (int k = 0; k < this.prePlacedGameEntities.Count; k++)
		{
			this.prePlacedGameEntities[k].gameObject.SetActive(false);
		}
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x000AC85B File Offset: 0x000AAA5B
	public void DeInit()
	{
		if (this.sectionConnector != null)
		{
			this.sectionConnector.DeInit();
			Object.Destroy(this.sectionConnector.gameObject);
		}
	}

	// Token: 0x0600226E RID: 8814 RVA: 0x000AC888 File Offset: 0x000AAA88
	public static void RandomizeIndices(List<int> list, int count, ref SRand randomGenerator)
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		randomGenerator.Shuffle<int>(list);
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x000AC8B5 File Offset: 0x000AAAB5
	public void InitLevelSection(int sectionIndex)
	{
		this.index = sectionIndex;
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x000AC8C0 File Offset: 0x000AAAC0
	public void SpawnSectionEntities(ref SRand randomGenerator)
	{
		if (this.spawnConfigs.Count > 0)
		{
			GhostReactorLevelSection.tempCreateEntitiesList.Clear();
			GhostReactorSpawnConfig ghostReactorSpawnConfig = this.spawnConfigs[randomGenerator.NextInt(this.spawnConfigs.Count)];
			Debug.LogFormat("Spawn Ghost Reactor Level Section {0} {1}", new object[]
			{
				base.gameObject.name,
				ghostReactorSpawnConfig.name
			});
			for (int i = 0; i < this.spawnPointGroups.Count; i++)
			{
				this.spawnPointGroups[i].NeedsRandomization = true;
				this.spawnPointGroups[i].CurrentIndex = 0;
			}
			for (int j = 0; j < ghostReactorSpawnConfig.entitySpawnGroups.Count; j++)
			{
				int spawnCount = ghostReactorSpawnConfig.entitySpawnGroups[j].spawnCount;
				if (spawnCount > 0)
				{
					int spawnPointType = (int)ghostReactorSpawnConfig.entitySpawnGroups[j].spawnPointType;
					if (spawnPointType < this.spawnPointGroupLookup.Length)
					{
						GhostReactorLevelSection.SpawnPointGroup spawnPointGroup = this.spawnPointGroupLookup[spawnPointType];
						if (spawnPointGroup.NeedsRandomization)
						{
							spawnPointGroup.NeedsRandomization = false;
							GhostReactorLevelSection.RandomizeIndices(spawnPointGroup.SpawnPointIndexes, spawnPointGroup.spawnPoints.Count, ref randomGenerator);
						}
						for (int k = 0; k < spawnCount; k++)
						{
							GREntitySpawnPoint nextSpawnPoint = spawnPointGroup.GetNextSpawnPoint();
							int staticHash = ghostReactorSpawnConfig.entitySpawnGroups[j].entity.name.GetStaticHash();
							long num = -1L;
							if (nextSpawnPoint.applyScale)
							{
								num = BitPackUtils.PackWorldPosForNetwork(nextSpawnPoint.transform.localScale);
							}
							else if (nextSpawnPoint.patrolPath != null)
							{
								num = (long)(this.index * 100 + nextSpawnPoint.patrolPath.index);
							}
							GameEntityCreateData gameEntityCreateData = new GameEntityCreateData
							{
								entityTypeId = staticHash,
								localPosition = nextSpawnPoint.transform.position,
								localRotation = nextSpawnPoint.transform.rotation,
								createData = num
							};
							GhostReactorLevelSection.tempCreateEntitiesList.Add(gameEntityCreateData);
						}
					}
				}
			}
			for (int l = 0; l < this.prePlacedGameEntities.Count; l++)
			{
				int staticHash2 = this.prePlacedGameEntities[l].gameObject.name.GetStaticHash();
				if (!GameEntityManager.instance.FactoryHasEntity(staticHash2))
				{
					Debug.LogErrorFormat("Cannot Find Entity in Factory {0} {1}", new object[]
					{
						this.prePlacedGameEntities[l].gameObject.name,
						staticHash2
					});
				}
				else
				{
					GameEntityCreateData gameEntityCreateData2 = new GameEntityCreateData
					{
						entityTypeId = staticHash2,
						localPosition = this.prePlacedGameEntities[l].transform.position,
						localRotation = this.prePlacedGameEntities[l].transform.rotation,
						createData = 0L
					};
					GhostReactorLevelSection.tempCreateEntitiesList.Add(gameEntityCreateData2);
				}
			}
			GameEntityManager.instance.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
			GhostReactorLevelSection.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000ACBCD File Offset: 0x000AADCD
	public GRPatrolPath GetPatrolPath(int patrolPathIndex)
	{
		if (patrolPathIndex >= 0 && patrolPathIndex < this.patrolPaths.Count)
		{
			return this.patrolPaths[patrolPathIndex];
		}
		return null;
	}

	// Token: 0x04002693 RID: 9875
	[SerializeField]
	private Transform anchorTransform;

	// Token: 0x04002694 RID: 9876
	[SerializeField]
	private bool isClosedSection;

	// Token: 0x04002695 RID: 9877
	[SerializeField]
	private List<GhostReactorLevelSection.SpawnPointGroup> spawnPointGroups;

	// Token: 0x04002696 RID: 9878
	[SerializeField]
	private List<GhostReactorSpawnConfig> spawnConfigs;

	// Token: 0x04002697 RID: 9879
	[SerializeField]
	private List<GRPatrolPath> patrolPaths;

	// Token: 0x04002698 RID: 9880
	[SerializeField]
	private BoxCollider boundingCollider;

	// Token: 0x04002699 RID: 9881
	[HideInInspector]
	public GhostReactorLevelSectionConnector sectionConnector;

	// Token: 0x0400269A RID: 9882
	[HideInInspector]
	public int hubAnchorIndex;

	// Token: 0x0400269B RID: 9883
	private int index;

	// Token: 0x0400269C RID: 9884
	private GhostReactorLevelSection.SpawnPointGroup[] spawnPointGroupLookup;

	// Token: 0x0400269D RID: 9885
	private List<GameEntity> prePlacedGameEntities;

	// Token: 0x0400269E RID: 9886
	private static List<GameEntityCreateData> tempCreateEntitiesList = new List<GameEntityCreateData>(1024);

	// Token: 0x02000583 RID: 1411
	[Serializable]
	public class SpawnPointGroup
	{
		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06002274 RID: 8820 RVA: 0x000ACC00 File Offset: 0x000AAE00
		// (set) Token: 0x06002275 RID: 8821 RVA: 0x000ACC08 File Offset: 0x000AAE08
		public bool NeedsRandomization
		{
			get
			{
				return this.needsRandomization;
			}
			set
			{
				this.needsRandomization = value;
			}
		}

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06002276 RID: 8822 RVA: 0x000ACC11 File Offset: 0x000AAE11
		// (set) Token: 0x06002277 RID: 8823 RVA: 0x000ACC19 File Offset: 0x000AAE19
		public int CurrentIndex
		{
			get
			{
				return this.currentIndex;
			}
			set
			{
				this.currentIndex = value;
			}
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06002278 RID: 8824 RVA: 0x000ACC22 File Offset: 0x000AAE22
		// (set) Token: 0x06002279 RID: 8825 RVA: 0x000ACC2A File Offset: 0x000AAE2A
		public List<int> SpawnPointIndexes
		{
			get
			{
				return this.spawnPointIndexes;
			}
			set
			{
				this.spawnPointIndexes = value;
			}
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x000ACC33 File Offset: 0x000AAE33
		public GREntitySpawnPoint GetNextSpawnPoint()
		{
			GREntitySpawnPoint grentitySpawnPoint = this.spawnPoints[this.spawnPointIndexes[this.currentIndex]];
			this.currentIndex = (this.currentIndex + 1) % this.spawnPointIndexes.Count;
			return grentitySpawnPoint;
		}

		// Token: 0x0400269F RID: 9887
		public GhostReactorSpawnConfig.SpawnPointType type;

		// Token: 0x040026A0 RID: 9888
		public List<GREntitySpawnPoint> spawnPoints;

		// Token: 0x040026A1 RID: 9889
		private List<int> spawnPointIndexes;

		// Token: 0x040026A2 RID: 9890
		private bool needsRandomization;

		// Token: 0x040026A3 RID: 9891
		private int currentIndex;
	}
}
