using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000598 RID: 1432
[CreateAssetMenu(fileName = "GhostReactorBreakableItemSpawnConfig", menuName = "ScriptableObjects/GhostReactorBreakableItemSpawnConfig")]
public class GRBreakableItemSpawnConfig : ScriptableObject
{
	// Token: 0x06002307 RID: 8967 RVA: 0x000AF4CC File Offset: 0x000AD6CC
	public bool TryForRandomItem(out GameEntity entity)
	{
		float num = this.precomputedItemTotalWeight / this.spawnAnythingProbability;
		float num2 = Random.Range(0f, num);
		if (num2 < this.precomputedItemTotalWeight)
		{
			float num3 = 0f;
			for (int i = 0; i < this.perItemProbabilities.Count; i++)
			{
				num3 += this.perItemProbabilities[i].probability;
				if (num3 > num2)
				{
					entity = this.perItemProbabilities[i].entity;
					return true;
				}
			}
		}
		entity = null;
		return false;
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x000AF54C File Offset: 0x000AD74C
	private void OnValidate()
	{
		this.precomputedItemTotalWeight = 0f;
		for (int i = 0; i < this.perItemProbabilities.Count; i++)
		{
			this.precomputedItemTotalWeight += this.perItemProbabilities[i].probability;
		}
	}

	// Token: 0x04002731 RID: 10033
	[SerializeField]
	[Range(0f, 1f)]
	public float spawnAnythingProbability = 0.2f;

	// Token: 0x04002732 RID: 10034
	public List<GRBreakableItemSpawnConfig.ItemProbability> perItemProbabilities = new List<GRBreakableItemSpawnConfig.ItemProbability>();

	// Token: 0x04002733 RID: 10035
	[SerializeField]
	[ReadOnly]
	private float precomputedItemTotalWeight;

	// Token: 0x02000599 RID: 1433
	[Serializable]
	public struct ItemProbability
	{
		// Token: 0x04002734 RID: 10036
		public GameEntity entity;

		// Token: 0x04002735 RID: 10037
		public float probability;
	}
}
