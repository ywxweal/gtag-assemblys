using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000580 RID: 1408
public class GhostReactorLevelGenerator : MonoBehaviour
{
	// Token: 0x17000355 RID: 853
	// (get) Token: 0x0600225C RID: 8796 RVA: 0x000ABCA1 File Offset: 0x000A9EA1
	public int MinSections
	{
		get
		{
			return this.minSections;
		}
	}

	// Token: 0x17000356 RID: 854
	// (get) Token: 0x0600225D RID: 8797 RVA: 0x000ABCA9 File Offset: 0x000A9EA9
	public int MaxSections
	{
		get
		{
			return this.maxSections;
		}
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x000ABCB1 File Offset: 0x000A9EB1
	private void Awake()
	{
		this.randomGenerator = new SRand(Random.Range(0, int.MaxValue));
		this.levelGeneration.sectionIndexPerAnchor = new List<int>();
		this.levelGeneration.anchorOrderIndices = new List<int>();
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x000ABCEC File Offset: 0x000A9EEC
	public void GenerateRandomLevelConfiguration(int sectionsToSpawn, int seed)
	{
		this.randomGenerator = new SRand(seed);
		this.levelGeneration.seed = seed;
		this.levelGeneration.sectionCount = Mathf.Min(sectionsToSpawn, this.sectionAnchors.Count);
		this.levelGeneration.connectionDirectionOffset = this.randomGenerator.NextInt(4);
		this.levelGeneration.connectionSelectionOffset = this.randomGenerator.NextInt(this.forwardConnectors.Count);
		this.RandomizeIndices(this.randomizedIndices, this.sections.Count);
		this.RandomizeIndices(this.levelGeneration.anchorOrderIndices, this.sectionAnchors.Count);
		this.levelGeneration.sectionIndexPerAnchor.Clear();
		for (int i = 0; i < this.sectionAnchors.Count; i++)
		{
			this.levelGeneration.sectionIndexPerAnchor.Add(0);
		}
		for (int j = 0; j < this.levelGeneration.sectionCount; j++)
		{
			int num = j % this.randomizedIndices.Count;
			this.levelGeneration.sectionIndexPerAnchor[this.levelGeneration.anchorOrderIndices[j]] = this.randomizedIndices[num];
		}
		this.RandomizeIndices(this.randomizedIndices, this.blockades.Count);
		for (int k = this.levelGeneration.sectionCount; k < this.sectionAnchors.Count; k++)
		{
			int num2 = k % this.randomizedIndices.Count;
			this.levelGeneration.sectionIndexPerAnchor[this.levelGeneration.anchorOrderIndices[k]] = this.randomizedIndices[num2];
		}
	}

	// Token: 0x06002260 RID: 8800 RVA: 0x000ABE94 File Offset: 0x000AA094
	public void SpawnSectionsBasedOnLevelGenerationConfig()
	{
		if (this.sectionsSpawned)
		{
			this.ClearLevelSections();
		}
		for (int i = 0; i < this.levelGeneration.sectionIndexPerAnchor.Count; i++)
		{
			int num = this.levelGeneration.anchorOrderIndices[i];
			if (i < this.levelGeneration.sectionCount)
			{
				int num2 = (this.levelGeneration.connectionDirectionOffset + num) % 4;
				int num3 = this.levelGeneration.connectionSelectionOffset + num;
				this.SpawnSection(this.levelGeneration.sectionIndexPerAnchor[num], num, num3, this.connectorDirLookup[num2]);
			}
			else
			{
				this.SpawnBlockade(this.levelGeneration.sectionIndexPerAnchor[num], num);
			}
		}
		if (this.removeInterpenetratingSections)
		{
			for (int j = this.spawnedSections.Count - 1; j >= 0; j--)
			{
				BoxCollider boundingCollider = this.spawnedSections[j].BoundingCollider;
				bool flag = false;
				for (int k = 0; k < this.alwaysPresentSections.Count; k++)
				{
					BoxCollider boundingCollider2 = this.alwaysPresentSections[k].BoundingCollider;
					Vector3 vector;
					float num4;
					if (boundingCollider.bounds.Intersects(boundingCollider2.bounds) && Physics.ComputePenetration(boundingCollider, boundingCollider.transform.position, boundingCollider.transform.rotation, boundingCollider2, boundingCollider2.transform.position, boundingCollider2.transform.rotation, out vector, out num4))
					{
						int hubAnchorIndex = this.spawnedSections[j].hubAnchorIndex;
						this.spawnedSections[j].DeInit();
						Object.Destroy(this.spawnedSections[j].gameObject);
						this.spawnedSections.RemoveAt(j);
						this.SpawnBlockade(this.levelGeneration.sectionIndexPerAnchor[hubAnchorIndex], hubAnchorIndex);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					for (int l = j - 1; l >= 0; l--)
					{
						BoxCollider boundingCollider3 = this.spawnedSections[l].BoundingCollider;
						Vector3 vector2;
						float num5;
						if (boundingCollider.bounds.Intersects(boundingCollider3.bounds) && Physics.ComputePenetration(boundingCollider, boundingCollider.transform.position, boundingCollider.transform.rotation, boundingCollider3, boundingCollider3.transform.position, boundingCollider3.transform.rotation, out vector2, out num5))
						{
							int hubAnchorIndex2 = this.spawnedSections[j].hubAnchorIndex;
							this.spawnedSections[j].DeInit();
							Object.Destroy(this.spawnedSections[j].gameObject);
							this.spawnedSections.RemoveAt(j);
							this.SpawnBlockade(this.levelGeneration.sectionIndexPerAnchor[hubAnchorIndex2], hubAnchorIndex2);
						}
					}
				}
			}
		}
		for (int m = 0; m < this.alwaysPresentSections.Count; m++)
		{
			this.alwaysPresentSections[m].InitLevelSection(m);
		}
		for (int n = 0; n < this.spawnedSections.Count; n++)
		{
			this.spawnedSections[n].InitLevelSection(n + this.alwaysPresentSections.Count);
		}
		this.sectionsSpawned = true;
	}

	// Token: 0x06002261 RID: 8801 RVA: 0x000AC1E4 File Offset: 0x000AA3E4
	public void SpawnEntitiesInEachSection()
	{
		for (int i = 0; i < this.alwaysPresentSections.Count; i++)
		{
			this.alwaysPresentSections[i].SpawnSectionEntities(ref this.randomGenerator);
		}
		for (int j = 0; j < this.spawnedSections.Count; j++)
		{
			this.spawnedSections[j].SpawnSectionEntities(ref this.randomGenerator);
		}
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x000AC24C File Offset: 0x000AA44C
	public GRPatrolPath GetPatrolPath(int patrolPathId)
	{
		int num = patrolPathId / 100;
		int num2 = patrolPathId % 100;
		if (num < this.alwaysPresentSections.Count)
		{
			if (num < 0 || num >= this.alwaysPresentSections.Count)
			{
				return null;
			}
			return this.alwaysPresentSections[num].GetPatrolPath(num2);
		}
		else
		{
			int num3 = num - this.alwaysPresentSections.Count;
			if (num3 < 0 || num3 >= this.spawnedSections.Count)
			{
				return null;
			}
			return this.spawnedSections[num3].GetPatrolPath(num2);
		}
	}

	// Token: 0x06002263 RID: 8803 RVA: 0x000AC2CC File Offset: 0x000AA4CC
	private void RandomizeIndices(List<int> list, int count)
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		this.randomGenerator.Shuffle<int>(list);
	}

	// Token: 0x06002264 RID: 8804 RVA: 0x000AC300 File Offset: 0x000AA500
	private void SpawnSection(int sectionIndex, int anchorIndex, int connectorIndex, GhostReactorLevelSectionConnector.Direction connectorDirection)
	{
		Transform transform = this.sectionAnchors[anchorIndex];
		GhostReactorLevelSection component = Object.Instantiate<GameObject>(this.sections[sectionIndex], base.transform).GetComponent<GhostReactorLevelSection>();
		component.hubAnchorIndex = anchorIndex;
		if (component == null || component.Anchor == null)
		{
			Debug.LogError("Ghost Reactor Level Sections need to have the component GhostReactorLevelSection at the root. That component needs the Anchor to be set.");
			return;
		}
		GhostReactorLevelSectionConnector ghostReactorLevelSectionConnector;
		switch (connectorDirection)
		{
		case GhostReactorLevelSectionConnector.Direction.Down:
			connectorIndex = Mathf.Clamp(connectorIndex % this.downwardConnectors.Count, 0, this.downwardConnectors.Count - 1);
			ghostReactorLevelSectionConnector = Object.Instantiate<GameObject>(this.downwardConnectors[connectorIndex], base.transform).GetComponent<GhostReactorLevelSectionConnector>();
			break;
		default:
			connectorIndex = Mathf.Clamp(connectorIndex % this.forwardConnectors.Count, 0, this.forwardConnectors.Count - 1);
			ghostReactorLevelSectionConnector = Object.Instantiate<GameObject>(this.forwardConnectors[connectorIndex], base.transform).GetComponent<GhostReactorLevelSectionConnector>();
			break;
		case GhostReactorLevelSectionConnector.Direction.Up:
			connectorIndex = Mathf.Clamp(connectorIndex % this.upwardConnectors.Count, 0, this.upwardConnectors.Count - 1);
			ghostReactorLevelSectionConnector = Object.Instantiate<GameObject>(this.upwardConnectors[connectorIndex], base.transform).GetComponent<GhostReactorLevelSectionConnector>();
			break;
		}
		Quaternion quaternion = Quaternion.Inverse(ghostReactorLevelSectionConnector.hubAnchor.localRotation) * transform.rotation;
		Vector3 vector = quaternion * -ghostReactorLevelSectionConnector.hubAnchor.localPosition + transform.position;
		ghostReactorLevelSectionConnector.transform.position = vector;
		ghostReactorLevelSectionConnector.transform.rotation = quaternion;
		ghostReactorLevelSectionConnector.Init();
		Quaternion quaternion2 = Quaternion.Inverse(component.Anchor.localRotation) * ghostReactorLevelSectionConnector.sectionAnchor.rotation;
		Vector3 vector2 = quaternion2 * -component.Anchor.localPosition + ghostReactorLevelSectionConnector.sectionAnchor.position;
		component.transform.position = vector2;
		component.transform.rotation = quaternion2;
		component.sectionConnector = ghostReactorLevelSectionConnector;
		this.spawnedSections.Add(component);
	}

	// Token: 0x06002265 RID: 8805 RVA: 0x000AC510 File Offset: 0x000AA710
	private void SpawnBlockade(int blockadeIndex, int anchorIndex)
	{
		Transform transform = this.sectionAnchors[anchorIndex];
		GhostReactorLevelSection component = Object.Instantiate<GameObject>(this.blockades[blockadeIndex % this.blockades.Count], base.transform).GetComponent<GhostReactorLevelSection>();
		if (component == null || component.Anchor == null)
		{
			Debug.LogError("Ghost Reactor Level Sections need to have the component GhostReactorLevelSection at the root. That component needs the Anchor to be set.");
			return;
		}
		Quaternion quaternion = Quaternion.Inverse(component.Anchor.localRotation) * transform.rotation;
		Vector3 vector = quaternion * -component.Anchor.localPosition + transform.position;
		component.transform.position = vector;
		component.transform.rotation = quaternion;
		this.spawnedBlockades.Add(component);
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x000AC5D8 File Offset: 0x000AA7D8
	public void ClearLevelSections()
	{
		for (int i = 0; i < this.spawnedSections.Count; i++)
		{
			this.spawnedSections[i].DeInit();
			Object.Destroy(this.spawnedSections[i].gameObject);
		}
		this.spawnedSections.Clear();
		for (int j = 0; j < this.spawnedBlockades.Count; j++)
		{
			this.spawnedBlockades[j].DeInit();
			Object.Destroy(this.spawnedBlockades[j].gameObject);
		}
		this.spawnedBlockades.Clear();
		this.sectionsSpawned = false;
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x000AC67C File Offset: 0x000AA87C
	public void DebugSpawnSection()
	{
		if (this.debugSectionToSpawn >= this.sections.Count || this.debugSpawnAnchor >= this.sectionAnchors.Count)
		{
			Debug.LogError("Invalid sectionToSpawn or spawnAnchor index");
			return;
		}
		this.SpawnSection(this.debugSectionToSpawn, this.debugSpawnAnchor, 0, GhostReactorLevelSectionConnector.Direction.Forward);
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x000AC6CE File Offset: 0x000AA8CE
	public void DebugClearSpawnedSections()
	{
		this.ClearLevelSections();
	}

	// Token: 0x04002679 RID: 9849
	[SerializeField]
	[Tooltip("Must be ordered by adjacency. So anchors next to each other spatially should be next to each other in this list")]
	private List<Transform> sectionAnchors = new List<Transform>();

	// Token: 0x0400267A RID: 9850
	[SerializeField]
	private List<GameObject> sections = new List<GameObject>();

	// Token: 0x0400267B RID: 9851
	[SerializeField]
	private List<GhostReactorLevelSection> alwaysPresentSections = new List<GhostReactorLevelSection>();

	// Token: 0x0400267C RID: 9852
	[SerializeField]
	private List<GameObject> blockades = new List<GameObject>();

	// Token: 0x0400267D RID: 9853
	[SerializeField]
	private List<GameObject> forwardConnectors = new List<GameObject>();

	// Token: 0x0400267E RID: 9854
	[SerializeField]
	private List<GameObject> upwardConnectors = new List<GameObject>();

	// Token: 0x0400267F RID: 9855
	[SerializeField]
	private List<GameObject> downwardConnectors = new List<GameObject>();

	// Token: 0x04002680 RID: 9856
	[SerializeField]
	private bool removeInterpenetratingSections = true;

	// Token: 0x04002681 RID: 9857
	[SerializeField]
	private int debugSectionToSpawn;

	// Token: 0x04002682 RID: 9858
	[SerializeField]
	private int debugSpawnAnchor;

	// Token: 0x04002683 RID: 9859
	[SerializeField]
	private bool debugSectionOverlaps;

	// Token: 0x04002684 RID: 9860
	private List<GhostReactorLevelSection> spawnedSections = new List<GhostReactorLevelSection>();

	// Token: 0x04002685 RID: 9861
	private List<GhostReactorLevelSection> spawnedBlockades = new List<GhostReactorLevelSection>();

	// Token: 0x04002686 RID: 9862
	[SerializeField]
	private int minSections = 2;

	// Token: 0x04002687 RID: 9863
	[SerializeField]
	private int maxSections = 3;

	// Token: 0x04002688 RID: 9864
	private SRand randomGenerator;

	// Token: 0x04002689 RID: 9865
	private List<int> randomizedIndices = new List<int>();

	// Token: 0x0400268A RID: 9866
	private bool sectionsSpawned;

	// Token: 0x0400268B RID: 9867
	private GhostReactorLevelSectionConnector.Direction[] connectorDirLookup = new GhostReactorLevelSectionConnector.Direction[]
	{
		GhostReactorLevelSectionConnector.Direction.Forward,
		GhostReactorLevelSectionConnector.Direction.Up,
		GhostReactorLevelSectionConnector.Direction.Forward,
		GhostReactorLevelSectionConnector.Direction.Down
	};

	// Token: 0x0400268C RID: 9868
	public GhostReactorLevelGenerator.LevelGeneration levelGeneration;

	// Token: 0x02000581 RID: 1409
	public struct LevelGeneration
	{
		// Token: 0x0400268D RID: 9869
		public int seed;

		// Token: 0x0400268E RID: 9870
		public int sectionCount;

		// Token: 0x0400268F RID: 9871
		public int connectionDirectionOffset;

		// Token: 0x04002690 RID: 9872
		public int connectionSelectionOffset;

		// Token: 0x04002691 RID: 9873
		public List<int> sectionIndexPerAnchor;

		// Token: 0x04002692 RID: 9874
		public List<int> anchorOrderIndices;
	}
}
