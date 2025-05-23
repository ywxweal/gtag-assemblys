using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ACE RID: 2766
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WhackAMoleLevelSetting", order = 1)]
	public class WhackAMoleLevelSO : ScriptableObject
	{
		// Token: 0x060042DE RID: 17118 RVA: 0x001354B4 File Offset: 0x001336B4
		public int GetMinScore(bool isCoop)
		{
			if (!isCoop)
			{
				return this.minScore;
			}
			return this.minScore * 2;
		}

		// Token: 0x04004567 RID: 17767
		public int levelNumber;

		// Token: 0x04004568 RID: 17768
		public float levelDuration;

		// Token: 0x04004569 RID: 17769
		[Tooltip("For how long do the moles stay visible?")]
		public float showMoleDuration;

		// Token: 0x0400456A RID: 17770
		[Tooltip("How fast we pick a random new mole?")]
		public float pickNextMoleTime;

		// Token: 0x0400456B RID: 17771
		[Tooltip("Minimum score to get in order to be able to proceed to the next level")]
		[SerializeField]
		private int minScore;

		// Token: 0x0400456C RID: 17772
		[Tooltip("Chance of each mole being a hazard mole at the start, and end, of the level.")]
		public Vector2 hazardMoleChance = new Vector2(0f, 0.5f);

		// Token: 0x0400456D RID: 17773
		[Tooltip("Minimum number of moles selected as level progresses.")]
		public Vector2 minimumMoleCount = new Vector2(1f, 2f);

		// Token: 0x0400456E RID: 17774
		[Tooltip("Minimum number of moles selected as level progresses.")]
		public Vector2 maximumMoleCount = new Vector2(1.5f, 3f);
	}
}
