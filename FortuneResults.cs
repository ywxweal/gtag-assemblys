using System;
using UnityEngine;

// Token: 0x02000550 RID: 1360
public class FortuneResults : ScriptableObject
{
	// Token: 0x060020F8 RID: 8440 RVA: 0x000A5A98 File Offset: 0x000A3C98
	private void OnValidate()
	{
		this.totalChance = 0f;
		for (int i = 0; i < this.fortuneResults.Length; i++)
		{
			this.totalChance += this.fortuneResults[i].weightedChance;
		}
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x000A5AE4 File Offset: 0x000A3CE4
	public FortuneResults.FortuneResult GetResult()
	{
		float num = Random.Range(0f, this.totalChance);
		int i = 0;
		while (i < this.fortuneResults.Length)
		{
			FortuneResults.FortuneCategory fortuneCategory = this.fortuneResults[i];
			if (num <= fortuneCategory.weightedChance)
			{
				if (fortuneCategory.textResults.Length == 0)
				{
					return new FortuneResults.FortuneResult(FortuneResults.FortuneCategoryType.Invalid, -1);
				}
				int num2 = Random.Range(0, fortuneCategory.textResults.Length);
				return new FortuneResults.FortuneResult(fortuneCategory.fortuneType, num2);
			}
			else
			{
				num -= fortuneCategory.weightedChance;
				i++;
			}
		}
		return new FortuneResults.FortuneResult(FortuneResults.FortuneCategoryType.Invalid, -1);
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x000A5B68 File Offset: 0x000A3D68
	public string GetResultText(FortuneResults.FortuneResult result)
	{
		for (int i = 0; i < this.fortuneResults.Length; i++)
		{
			if (this.fortuneResults[i].fortuneType == result.fortuneType && result.resultIndex >= 0 && result.resultIndex < this.fortuneResults[i].textResults.Length)
			{
				return this.fortuneResults[i].textResults[result.resultIndex];
			}
		}
		return "!! Invalid Fortune !!";
	}

	// Token: 0x04002520 RID: 9504
	[SerializeField]
	private FortuneResults.FortuneCategory[] fortuneResults;

	// Token: 0x04002521 RID: 9505
	[SerializeField]
	private float totalChance;

	// Token: 0x02000551 RID: 1361
	public enum FortuneCategoryType
	{
		// Token: 0x04002523 RID: 9507
		Invalid,
		// Token: 0x04002524 RID: 9508
		Positive,
		// Token: 0x04002525 RID: 9509
		Neutral,
		// Token: 0x04002526 RID: 9510
		Negative,
		// Token: 0x04002527 RID: 9511
		Seasonal
	}

	// Token: 0x02000552 RID: 1362
	[Serializable]
	public struct FortuneCategory
	{
		// Token: 0x04002528 RID: 9512
		public FortuneResults.FortuneCategoryType fortuneType;

		// Token: 0x04002529 RID: 9513
		public float weightedChance;

		// Token: 0x0400252A RID: 9514
		public string[] textResults;
	}

	// Token: 0x02000553 RID: 1363
	public struct FortuneResult
	{
		// Token: 0x060020FC RID: 8444 RVA: 0x000A5BE3 File Offset: 0x000A3DE3
		public FortuneResult(FortuneResults.FortuneCategoryType fortuneType, int resultIndex)
		{
			this.fortuneType = fortuneType;
			this.resultIndex = resultIndex;
		}

		// Token: 0x0400252B RID: 9515
		public FortuneResults.FortuneCategoryType fortuneType;

		// Token: 0x0400252C RID: 9516
		public int resultIndex;
	}
}
