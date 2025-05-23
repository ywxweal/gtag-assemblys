using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200058C RID: 1420
[CreateAssetMenu(fileName = "GhostReactorSpawnConfig", menuName = "ScriptableObjects/GhostReactorSpawnConfig")]
public class GhostReactorSpawnConfig : ScriptableObject
{
	// Token: 0x040026E4 RID: 9956
	public List<GhostReactorSpawnConfig.EntitySpawnGroup> entitySpawnGroups;

	// Token: 0x0200058D RID: 1421
	public enum SpawnPointType
	{
		// Token: 0x040026E6 RID: 9958
		Enemy,
		// Token: 0x040026E7 RID: 9959
		Collectible,
		// Token: 0x040026E8 RID: 9960
		Barrier,
		// Token: 0x040026E9 RID: 9961
		HazardLiquid,
		// Token: 0x040026EA RID: 9962
		SpawnPointTypeCount
	}

	// Token: 0x0200058E RID: 1422
	[Serializable]
	public struct EntitySpawnGroup
	{
		// Token: 0x040026EB RID: 9963
		public GhostReactorSpawnConfig.SpawnPointType spawnPointType;

		// Token: 0x040026EC RID: 9964
		public GameEntity entity;

		// Token: 0x040026ED RID: 9965
		public int spawnCount;
	}
}
